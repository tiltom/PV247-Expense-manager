using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Web.DatabaseContexts;
using ExpenseManager.Web.Models.Transaction;
using Microsoft.AspNet.Identity;

namespace ExpenseManager.Web.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext(); // instance of DB context

        /// <summary>
        ///     Shows transactions for users Wallet
        /// </summary>
        /// <returns>View with transaction</returns>
        public async Task<ActionResult> Index()
        {
            var id = HttpContext.User.Identity.GetUserId(); // get Id of currently logged user from HttpContext
            var list = await this._db.Transactions.Where(user => user.Wallet.Owner.Id == id).ToListAsync();
            //list of all transactions from user's wallet

            return this.View(list.Select(this.ConvertEntityToTransactionShowModel));
        }

        /// <summary>
        ///     Creates new transaction
        /// </summary>
        /// <returns>View with model</returns>
        public async Task<ActionResult> Create()
        {
            var walletId = await this.GetUserWalletId(); //get wallet Id for currently logged user
            var currency = await this._db.Wallets.FindAsync(walletId); //get default currency for wallet

            return
                this.View(new NewTransactionModel //fill NewTransaction model with needed informations
                {
                    WalletId = walletId,
                    CurrencyId = currency.Currency.Guid.ToString(),
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
            if (ModelState.IsValid) //check if model is valid
            {
                var transactionEntity =
                    await this.ConvertNewModelToEntity(transaction, new Transaction {Guid = Guid.NewGuid()});
                //create new Transaction entity and fill it from model
                this._db.Transactions.Add(transactionEntity);
                if (transaction.IsRepeatable) //check if transaction should be repeatable
                {
                    var repeatableTransaction = new RepeatableTransaction
                        //create new repeatable transaction entity and fill from model
                    {
                        FirstTransaction = transactionEntity,
                        Frequency = transaction.Frequency,
                        LastOccurence = transaction.LastOccurence.GetValueOrDefault(),
                        Guid = Guid.NewGuid()
                    };
                    this._db.RepeatableTransactions.Add(repeatableTransaction);
                }
                await this._db.SaveChangesAsync();
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
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null) //check if Id is not null
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var transaction = await this._db.Transactions.FindAsync(id); //find transaction by it's Id
            if (transaction == null)
            {
                return this.HttpNotFound();
            }
            var model = this.ConvertEntityToTransactionEditModel(transaction); //fill model from DB entity
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
            if (ModelState.IsValid) //check if model is valid
            {
                var transactionEntity = await this._db.Transactions.FindAsync(transaction.Id); //find transaction by Id
                await this.ConvertEditModelToEntity(transaction, transactionEntity);
                //update entity properties from model
                var repeatableTransaction =
                    await
                        this._db.RepeatableTransactions.FirstOrDefaultAsync(
                            a => a.FirstTransaction.Guid == transaction.Id); //find if transaction is repeatable in DB
                if (transaction.IsRepeatable) //check if transaction was set as repeatable in model
                {
                    if (repeatableTransaction == null) //check if transaction was also set repeatable in model
                    {
                        repeatableTransaction = new RepeatableTransaction
                            //if not create new repeatable transaction entity
                        {
                            FirstTransaction = transactionEntity,
                            Frequency = transaction.Frequency,
                            LastOccurence = transaction.LastOccurence.GetValueOrDefault(),
                            Guid = Guid.NewGuid()
                        };
                        this._db.RepeatableTransactions.Add(repeatableTransaction);
                    }
                    else // if transaction exists in repeatable transactions in DB update it
                    {
                        repeatableTransaction.Frequency = transaction.Frequency;
                        repeatableTransaction.LastOccurence = transaction.LastOccurence.GetValueOrDefault();
                    }
                }
                else // if transaction was set as not repeatable in model
                {
                    if (repeatableTransaction != null) //if exists in DB in repeatable transactions delete it
                    {
                        this._db.RepeatableTransactions.Remove(repeatableTransaction);
                    }
                }
                await this._db.SaveChangesAsync();
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
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null) //check if id is not null
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var transaction = await this._db.Transactions.FindAsync(id); //find transaction by its Id
            var repeatableTransaction =
                await this._db.RepeatableTransactions.FirstOrDefaultAsync(a => a.FirstTransaction.Guid == id);
            //get if transaction is also in repeatable transactions
            if (repeatableTransaction != null) //check if transaction was not repeatable
            {
                this._db.RepeatableTransactions.Remove(repeatableTransaction);
                //if true remove it from repeatable transactions first
            }
            this._db.Transactions.Remove(transaction); //removing transaction from DB
            await this._db.SaveChangesAsync();
            return this.RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._db.Dispose();
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
            entity.Wallet = await this._db.Wallets.FindAsync(model.WalletId);
            if (model.BudgetId != null) //check if budget was set to cattegory in model
            {
                entity.Budget = await this._db.Budgets.FindAsync(new Guid(model.BudgetId));
            }
            entity.Currency = await this._db.Currencies.FindAsync(new Guid(model.CurrencyId));
            entity.Category = await this._db.Categories.FindAsync(new Guid(model.CategoryId));

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
            entity.Wallet = await this._db.Wallets.FindAsync(model.WalletId);
            if (model.BudgetId == null) //check if budget was set in model
            {
                entity.Budget?.Transactions.Remove(entity); //remove transaction from Budget if it exists
                entity.Budget = null;
            }
            else
            {
                entity.Budget = await this._db.Budgets.FindAsync(new Guid(model.BudgetId));
            }
            entity.Currency = await this._db.Currencies.FindAsync(new Guid(model.CurrencyId));
            entity.Category = await this._db.Categories.FindAsync(new Guid(model.CategoryId));
            return entity;
        }

        /// <summary>
        ///     Converts entity of type Transaction into EditTransactionModel
        /// </summary>
        /// <param name="entity">entity of type Transaction</param>
        /// <returns>EditTransactionModel</returns>
        private EditTransactionModel ConvertEntityToTransactionEditModel(Transaction entity)
        {
            var repeatableTransaction =
                this._db.RepeatableTransactions.FirstOrDefault(a => a.FirstTransaction.Guid == entity.Guid);
                //get if transaction is repeatable
            string budgetId = null;
            if (entity.Budget != null)
                budgetId = entity.Budget.Guid.ToString();
            var transactionModel = new EditTransactionModel //fill model info from entity
            {
                Id = entity.Guid,
                Amount = entity.Amount,
                Date = entity.Date,
                Description = entity.Description,
                WalletId = entity.Wallet.Guid,
                BudgetId = budgetId,
                CurrencyId = entity.Currency.Guid.ToString(),
                CategoryId = entity.Category.Guid.ToString()
            };

            if (repeatableTransaction != null) //check if transaction is repeatable and fill it
            {
                transactionModel.IsRepeatable = true;
                transactionModel.Frequency = repeatableTransaction.Frequency;
                transactionModel.LastOccurence = repeatableTransaction.LastOccurence;
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
            var repeatableTransaction =
                this._db.RepeatableTransactions.FirstOrDefault(a => a.FirstTransaction.Guid == entity.Guid);
                //get if transaction is repeatable
            var transactionModel = new TransactionShowModel //fill model info from entity
            {
                Id = entity.Guid,
                Amount = entity.Amount,
                Date = entity.Date,
                Description = entity.Description,
                BudgetName = budgetName,
                CurrencySymbol = entity.Currency.Symbol,
                CategoryName = entity.Category.Name
            };

            if (repeatableTransaction != null) //check if transaction is repeatable and fill it
            {
                transactionModel.IsRepeatable = true;
            }

            return transactionModel;
        }

        /// <summary>
        ///     Provides selectable list of Budgets avaliable to user
        /// </summary>
        /// <returns>List of SelectListItem for Budgets</returns>
        private async Task<List<SelectListItem>> GetBudgets()
        {
            var id = HttpContext.User.Identity.GetUserId();
            return
                await
                    this._db.BudgetAccessRights.Where(
                        access =>
                            access.User.Id == id &&
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
        ///     Provides selectable list of Currencies which are avaliable
        /// </summary>
        /// <returns>List of SelectListItem for Currencies</returns>
        private async Task<List<SelectListItem>> GetCurrencies()
        {
            return
                await
                    this._db.Currencies.Select(
                        currency => new SelectListItem {Value = currency.Guid.ToString(), Text = currency.Name})
                        .ToListAsync();
        }

        /// <summary>
        ///     Provides selectable list of Categories which are avaliable
        /// </summary>
        /// <returns>List of SelectListItem for Categories</returns>
        private async Task<List<SelectListItem>> GetCategories()
        {
            return
                await
                    this._db.Categories.Select(
                        category => new SelectListItem {Value = category.Guid.ToString(), Text = category.Name})
                        .ToListAsync();
        }

        /// <summary>
        ///     Gets id of Wallet for currently logged user
        /// </summary>
        /// <returns>WalletId</returns>
        private async Task<Guid> GetUserWalletId()
        {
            var id = HttpContext.User.Identity.GetUserId();
            var walletEntity = await this._db.Wallets.Where(wallet => wallet.Owner.Id == id).FirstOrDefaultAsync();
            return walletEntity.Guid;
        }
    }
}