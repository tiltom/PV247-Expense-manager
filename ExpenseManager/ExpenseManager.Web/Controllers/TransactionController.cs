using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Web.Models.Transaction;

namespace ExpenseManager.Web.Controllers
{
    [Authorize]
    public class TransactionController : AbstractController
    {
        private static string _walletId;
        private readonly IBudgetsProvider _budgetsProvider = ProvidersFactory.GetNewBudgetsProviders();
        private readonly ITransactionsProvider _transactionsProvider = ProvidersFactory.GetNewTransactionsProviders();
        private readonly IWalletsProvider _walletsProvider = ProvidersFactory.GetNewWalletsProviders();


        /// <summary>
        ///     Shows transactions for users Wallet
        /// </summary>
        /// <returns>View with transaction</returns>
        public async Task<ActionResult> Index(string walletId)
        {
            // get Id of currently logged UserProfile from HttpContext
            var id = await this.CurrentProfileId();

            // get all Wallets user has access to
            ViewBag.WalletList = await this.GetViewableWallets();

            // if walletId was selected remember it
            if (!string.IsNullOrEmpty(walletId))
            {
                _walletId = walletId;
            }
            var selectedWalletId = await this.GetCurrentWallet();
            // get all Transactions in selected wallet
            var list =
                await
                    this._transactionsProvider.Transactions.Where(user => user.Wallet.Guid == new Guid(selectedWalletId))
                        .ToListAsync();

            // get user permission for selected wallet
            var permission =
                await
                    this._walletsProvider.WalletAccessRights // TODO NOT IMPLEMENTED< SHOULDNT BE IMPLEMENTED FIX THIS
                        .FirstOrDefaultAsync(
                            r =>
                                r.UserProfile.Guid == id && r.Wallet.Guid.ToString() == selectedWalletId);
            // when user doesn't have permission to manipulate with transaction show view without edit and delete
            if (permission != null && permission.Permission == PermissionEnum.Read)
            {
                return this.View("IndexRead", list.Select(this.ConvertEntityToTransactionShowModel));
            }

            //list of all transactions from UserProfile's wallet
            return this.View(list.Select(this.ConvertEntityToTransactionShowModel));
        }

        /// <summary>
        ///     Creates new transaction
        /// </summary>
        /// <returns>View with model</returns>
        public async Task<ActionResult> Create()
        {
            //get wallet Id for currently selected wallet
            var walletId = new Guid(await this.GetCurrentWallet());
            //get default currency for wallet
            var wallet =
                await this._transactionsProvider.Wallets.Where(w => w.Guid.Equals(walletId)).FirstOrDefaultAsync();
            //fill NewTransaction model with needed informations
            return
                this.View(new NewTransactionModel
                {
                    WalletId = walletId,
                    CurrencyId = wallet.Currency.Guid,
                    Categories = await this.GetCategories(),
                    Currencies = await this.GetCurrencies(),
                    Budgets = await this.GetBudgets()
                });
        }

        /// <summary>
        ///     Creates new transaction
        /// </summary>
        /// <param name="transaction">NewTransaction model instance</param>
        /// <returns>Redirect to index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(NewTransactionModel transaction)
        {
            //If transaction is repeatable date of last Occurrence must be set
            if (transaction.IsRepeatable)
            {
                if (transaction.LastOccurrence == null)
                {
                    ModelState.AddModelError("LastOccurrence", "Date of last occurrence must be set");
                }
                if (transaction.NextRepeat <= 0)
                {
                    ModelState.AddModelError("NextRepeat", "Frequency must be positive number");
                }
            }

            if (ModelState.IsValid)
            {
                //create new Transaction entity and fill it from model
                var transactionEntity =
                    await this.ConvertNewModelToEntity(transaction, new Transaction {Guid = Guid.NewGuid()});
                //check if transaction should be repeatable
                await this._transactionsProvider.AddOrUpdateAsync(transactionEntity);
                if (transaction.IsRepeatable)
                {
                    //create new repeatable transaction entity and fill from model
                    var repeatableTransaction = new RepeatableTransaction

                    {
                        FirstTransaction = transactionEntity,
                        NextRepeat = transaction.NextRepeat,
                        FrequencyType = transaction.FrequencyType,
                        LastOccurrence = transaction.LastOccurrence.GetValueOrDefault(),
                        Guid = Guid.NewGuid()
                    };
                    await this._transactionsProvider.AddOrUpdateAsync(repeatableTransaction);
                }
                return this.RedirectToAction("Index");
            }
            transaction.Currencies = await this.GetCurrencies();
            transaction.Categories = await this.GetCategories();
            transaction.Budgets = await this.GetBudgets();
            return this.View(transaction);
        }

        /// <summary>
        ///     Action for editing transaction
        /// </summary>
        /// <param name="id"></param>
        /// <returns>View with model</returns>
        // GET: Transactions/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            //find transaction by it's Id
            var transaction =
                await this._transactionsProvider.Transactions.Where(t => t.Guid.Equals(id)).FirstOrDefaultAsync();
            if (transaction == null)
            {
                return this.HttpNotFound();
            }
            // if user doesn't have permission to modify transaction show error
            if (!await this.HasWritePermission(transaction))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                    "You don't have permission to edit this transaction");
            }

            //fill model from DB entity
            var model = this.ConvertEntityToTransactionEditModel(transaction);
            model.Currencies = await this.GetCurrencies();
            model.Categories = await this.GetCategories();
            model.Budgets = await this.GetBudgets();

            return this.View(model);
        }

        /// <summary>
        ///     Editing of transaction
        /// </summary>
        /// <param name="transaction">Instance od EditTransactionModel</param>
        /// <returns>Redirect to Index</returns>
        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditTransactionModel transaction)
        {
            if (transaction.IsRepeatable)
            {
                if (transaction.LastOccurrence == null)
                {
                    ModelState.AddModelError("LastOccurrence", "Date of last occurrence must be set");
                }
                if (transaction.NextRepeat <= 0)
                {
                    ModelState.AddModelError("NextRepeat", "Frequency must be positive number");
                }
            }

            //check if model is valid
            if (ModelState.IsValid)
            {
                //find transaction by Id
                var transactionEntity =
                    await
                        this._transactionsProvider.Transactions.Where(t => t.Guid.Equals(transaction.Id))
                            .FirstOrDefaultAsync();
                //update entity properties from model
                await this.ConvertEditModelToEntity(transaction, transactionEntity);
                //find if transaction is repeatable in DB
                var repeatableTransaction =
                    await
                        this._transactionsProvider.RepeatableTransactions.FirstOrDefaultAsync(
                            a => a.FirstTransaction.Guid == transaction.Id);
                //check if transaction was set as repeatable in model
                if (transaction.IsRepeatable)
                {
                    //check if transaction was also set repeatable in model
                    if (repeatableTransaction == null)
                    {
                        //if not create new repeatable transaction entity
                        repeatableTransaction = new RepeatableTransaction
                        {
                            FirstTransaction = transactionEntity,
                            NextRepeat = transaction.NextRepeat,
                            FrequencyType = transaction.FrequencyType,
                            LastOccurrence = transaction.LastOccurrence.GetValueOrDefault(),
                            Guid = Guid.NewGuid()
                        };
                        await this._transactionsProvider.AddOrUpdateAsync(repeatableTransaction);
                    }
                    // if transaction exists in repeatable transactions in DB update it
                    else
                    {
                        repeatableTransaction.NextRepeat = transaction.NextRepeat;
                        repeatableTransaction.FrequencyType = transaction.FrequencyType;
                        repeatableTransaction.LastOccurrence = transaction.LastOccurrence.GetValueOrDefault();
                        await this._transactionsProvider.AddOrUpdateAsync(repeatableTransaction);
                    }
                }
                // if transaction was set as not repeatable in model
                else
                {
                    //if exists in DB in repeatable transactions delete it
                    if (repeatableTransaction != null)
                    {
                        await this._transactionsProvider.DeteleAsync(repeatableTransaction);
                    }
                    else
                    {
                        await this._transactionsProvider.AddOrUpdateAsync(transactionEntity);
                    }
                }
                return this.RedirectToAction("Index");
            }
            transaction.Currencies = await this.GetCurrencies();
            transaction.Categories = await this.GetCategories();
            transaction.Budgets = await this.GetBudgets();
            return this.View(transaction);
        }

        /// <summary>
        ///     Deleting transactions
        /// </summary>
        /// <param name="id">Id of transaction to edit</param>
        /// <returns>Redirect to Index</returns>
        // POST: Transactions/Delete/5
        public async Task<ActionResult> Delete(Guid id)
        {
            //find transaction by its Id
            var transaction =
                await this._transactionsProvider.Transactions.Where(t => t.Guid.Equals(id)).FirstOrDefaultAsync();
            if (!await this.HasWritePermission(transaction))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                    "You don't have permission to delete this transaction");
            }
            //get if transaction is also in repeatable transactions
            var repeatableTransaction =
                await
                    this._transactionsProvider.RepeatableTransactions.FirstOrDefaultAsync(
                        a => a.FirstTransaction.Guid.Equals(id));

            if (repeatableTransaction != null) //check if transaction was not repeatable
            {
                //if true remove it from repeatable transactions first
                await this._transactionsProvider.DeteleAsync(repeatableTransaction);
            }
            //removing transaction from DB
            await this._transactionsProvider.DeteleAsync(transaction);
            return this.RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //this._db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///     Converts NewTransactionModel into entity Transaction
        /// </summary>
        /// <param name="model">NewTransactionModel</param>
        /// <param name="entity">entity Transaction</param>
        /// <returns>Task Transaction</returns>
        private async Task<Transaction> ConvertNewModelToEntity(NewTransactionModel model, Transaction entity)
        {
            //setting properties from model
            entity.Amount = model.Amount;
            entity.Date = model.Date;
            entity.Description = model.Description;
            entity.Wallet =
                await this._transactionsProvider.Wallets.Where(w => w.Guid.Equals(model.WalletId)).FirstOrDefaultAsync();
            //check if budget was set to category in model
            if (model.BudgetId != null)
            {
                var budgetId = model.BudgetId.GetValueOrDefault();
                entity.Budget =
                    await this._transactionsProvider.Budgets.Where(b => b.Guid == budgetId).FirstOrDefaultAsync();
            }
            entity.Currency =
                await this._transactionsProvider.Currencies.Where(c => c.Guid == model.CurrencyId).FirstOrDefaultAsync();
            entity.Category =
                await this._transactionsProvider.Categories.Where(c => c.Guid == model.CategoryId).FirstOrDefaultAsync();

            return entity;
        }

        /// <summary>
        ///     Converts EditTransactionModel into entity Transaction
        /// </summary>
        /// <param name="model">EditTransactionModel</param>
        /// <param name="entity">entity Transaction</param>
        /// <returns>Task Transaction</returns>
        private async Task<Transaction> ConvertEditModelToEntity(EditTransactionModel model, Transaction entity)
        {
            //setting properties from model
            entity.Amount = model.Amount;
            entity.Date = model.Date;
            entity.Description = model.Description;
            entity.Wallet =
                await this._transactionsProvider.Wallets.Where(w => w.Guid.Equals(model.WalletId)).FirstOrDefaultAsync();
            //check if budget was set in model
            if (model.BudgetId == null)
            {
                //remove transaction from Budget if it exists
                entity.Budget?.Transactions.Remove(entity);
                entity.Budget = null;
            }
            else
            {
                // TODO: REMOVE THESE SUPER UGLY TOSTRINGS JUST TEST
                var budgetId = model.BudgetId.GetValueOrDefault();
                entity.Budget =
                    await
                        this._transactionsProvider.Budgets.Where(b => b.Guid.Equals(budgetId))
                            .FirstOrDefaultAsync();
            }
            // TODO: REMOVE THESE SUPER UGLY TOSTRINGS JUST TEST
            entity.Currency =
                await
                    this._transactionsProvider.Currencies.Where(c => c.Guid.Equals(model.CurrencyId))
                        .FirstOrDefaultAsync();
            entity.Category =
                await
                    this._transactionsProvider.Categories.Where(c => c.Guid.Equals(model.CategoryId))
                        .FirstOrDefaultAsync();
            return entity;
        }

        /// <summary>
        ///     Converts entity of type Transaction into EditTransactionModel
        /// </summary>
        /// <param name="entity">entity of type Transaction</param>
        /// <returns>EditTransactionModel</returns>
        private EditTransactionModel ConvertEntityToTransactionEditModel(Transaction entity)
        {
            //get if transaction is repeatable
            var repeatableTransaction =
                this._transactionsProvider.RepeatableTransactions.FirstOrDefault(
                    a => a.FirstTransaction.Guid == entity.Guid);

            Guid? budgetId = null;
            if (entity.Budget != null)
                budgetId = entity.Budget.Guid;
            //fill model info from entity
            var transactionModel = Mapper.Map<EditTransactionModel>(entity);
            transactionModel.BudgetId = budgetId;

            //check if transaction is repeatable and fill it
            if (repeatableTransaction != null)
            {
                transactionModel.IsRepeatable = true;
                transactionModel.NextRepeat = repeatableTransaction.NextRepeat;
                transactionModel.FrequencyType = repeatableTransaction.FrequencyType;
                transactionModel.LastOccurrence = repeatableTransaction.LastOccurrence;
            }

            return transactionModel;
        }

        /// <summary>
        ///     Converts entity of type Transaction into TransactionShowModel
        /// </summary>
        /// <param name="entity">entity of type Transaction</param>
        /// <returns>TransactionShowModel</returns>
        private TransactionShowModel ConvertEntityToTransactionShowModel(Transaction entity)
        {
            string budgetName = null;
            if (entity.Budget != null)
                budgetName = entity.Budget.Name;
            //get if transaction is repeatable
            var repeatableTransaction =
                this._transactionsProvider.RepeatableTransactions.FirstOrDefault(
                    a => a.FirstTransaction.Guid == entity.Guid);
            //fill model info from entity
            var transactionModel = Mapper.Map<TransactionShowModel>(entity);
            transactionModel.BudgetName = budgetName;

            //check if transaction is repeatable and fill it
            if (repeatableTransaction != null)
            {
                transactionModel.IsRepeatable = true;
            }

            return transactionModel;
        }

        /// <summary>
        ///     Provides selectable list of Budgets available to UserProfile
        /// </summary>
        /// <returns>List of SelectListItem for Budgets</returns>
        private async Task<List<SelectListItem>> GetBudgets()
        {
            var id = await this.CurrentProfileId();
            return
                await
                    this._budgetsProvider.BudgetAccessRights.Where(
                        access =>
                            access.UserProfile.Guid == id &&
                            access.Permission >= PermissionEnum.Write)
                        .Select(
                            budget =>
                                new SelectListItem
                                {
                                    Value = budget.Budget.Guid.ToString(),
                                    Text = budget.Budget.Name
                                })
                        .ToListAsync();
        }

        /// <summary>
        ///     Provides selectable list of Currencies which are available
        /// </summary>
        /// <returns>List of SelectListItem for Currencies</returns>
        private async Task<List<SelectListItem>> GetCurrencies()
        {
            return
                await
                    this._transactionsProvider.Currencies.Select(
                        currency => new SelectListItem {Value = currency.Guid.ToString(), Text = currency.Name})
                        .ToListAsync();
        }

        /// <summary>
        ///     Provides selectable list of Categories which are available
        /// </summary>
        /// <returns>List of SelectListItem for Categories</returns>
        private async Task<List<SelectListItem>> GetCategories()
        {
            return
                await
                    this._transactionsProvider.Categories.Select(
                        category => new SelectListItem {Value = category.Guid.ToString(), Text = category.Name})
                        .ToListAsync();
        }

        /// <summary>
        ///     Provides selectable list of Wallets user has at least permission to read
        /// </summary>
        /// <returns></returns>
        private async Task<List<SelectListItem>> GetViewableWallets()
        {
            var id = await this.CurrentProfileId();
            return
                await this._walletsProvider.WalletAccessRights.Where(
                    user => user.Permission >= PermissionEnum.Read && user.UserProfile.Guid == id)
                    .Select(
                        wallet => new SelectListItem {Value = wallet.Wallet.Guid.ToString(), Text = wallet.Wallet.Name})
                    .ToListAsync();
        }

        /// <summary>
        ///     Checks if user has permission to write to Wallet
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private async Task<bool> HasWritePermission(Transaction transaction)
        {
            var id = await this.CurrentProfileId();
            //TODO When i change FirstOrDefault to FirstOrDefaultAsync there is an exception: A second operation started on this context before a previous asynchronous operation completed. Use 'await' to ensure that any asynchronous operations have completed before calling another method on this context. Any instance members are not guaranteed to be thread safe.
            var permission = this._walletsProvider.WalletAccessRights
                .FirstOrDefault(
                    r =>
                        r.Wallet.Guid == transaction.Wallet.Guid &&
                        r.UserProfile.Guid == id);
            return permission != null && permission.Permission != PermissionEnum.Read;
        }

        /// <summary>
        ///     Gets id of currently selected Wallet, if none is selected returns id of User's default Wallet and saves it
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetCurrentWallet()
        {
            var profileId = await this.CurrentProfileId();
            var currentUserWallet = await this._walletsProvider.Wallets.Where(w => w.Owner.Guid == profileId)
                .Select(w => w.Guid)
                .FirstOrDefaultAsync();
            return _walletId ?? (_walletId = currentUserWallet.ToString());
        }
    }
}