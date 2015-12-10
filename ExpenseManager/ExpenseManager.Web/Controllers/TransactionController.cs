using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using ExpenseManager.BusinessLogic;
using ExpenseManager.BusinessLogic.TransactionServices;
using ExpenseManager.BusinessLogic.TransactionServices.Models;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Resources;
using ExpenseManager.Resources.TransactionResources;
using ExpenseManager.Web.Constants;
using ExpenseManager.Web.Helpers;
using ExpenseManager.Web.Models.Transaction;
using PagedList;

namespace ExpenseManager.Web.Controllers
{
    [Authorize]
    public class TransactionController : AbstractController
    {
        private readonly TransactionService _transactionService =
            new TransactionService(ProvidersFactory.GetNewBudgetsProviders(),
                ProvidersFactory.GetNewTransactionsProviders(), ProvidersFactory.GetNewWalletsProviders());

        /// <summary>
        ///     Shows transactions for users Wallet
        /// </summary>
        /// <returns>View with transactions</returns>
        public async Task<ActionResult> Index(Guid? wallet, Guid? category, Guid? budget, int? page)
        {
            // get Id of currently logged UserProfile from HttpContext
            var id = await this.CurrentProfileId();
            // If no wallet was given set default wallet
            if (wallet == null)
            {
                wallet = await this._transactionService.GetWalletIdByUserId(id);
            }
            var walletId = wallet.Value;
            await this.SetIndexViewData(category, budget, walletId);
            // get all Transactions in selected wallet
            IEnumerable<TransactionShowServiceModel> list;
            try
            {
                list =
                    await this._transactionService.GetAllTransactionsInWallet(id, walletId);
            }
            catch (SecurityException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                    TransactionResource.PermissionError);
            }
            list = TransactionService.FilterTransactions(category, budget, list);

            var showModels = Enumerable.Reverse(list.Select(Mapper.Map<TransactionShowModel>)).ToList();
            var pageNumber = (page ?? 1);
            return this.View("Index",
                showModels.OrderByDescending(model => model.Date).ToPagedList(pageNumber, SharedConstant.PageSize));
        }

        /// <summary>
        ///     Provides selection whether transaction is expense or income
        /// </summary>
        /// <param name="wallet">Id of wallet where transaction will be added</param>
        /// <returns>>View with expense or income selection</returns>
        public async Task<ActionResult> ExpenseIncome(Guid? wallet)
        {
            if (wallet == null)
            {
                wallet = await this._transactionService.GetWalletIdByUserId(await this.CurrentProfileId());
            }
            ViewBag.wallet = wallet;
            return
                this.View();
        }

        /// <summary>
        ///     Creates new transaction
        /// </summary>
        /// <returns>View with model</returns>
        public async Task<ActionResult> Create(bool expense, Guid wallet)
        {
            var userId = await this.CurrentProfileId();
            var permissions = await this._transactionService.GetPermission(userId, wallet);
            if (permissions == null || PermissionEnum.Read == permissions.Permission)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                    TransactionResource.PermissionErrorCreate);
            }
            //get default currency for wallet
            var currency = await this._transactionService.GetDefaultCurrencyInWallet(wallet);
            //fill NewTransaction model with needed informations
            var model = new NewTransactionModel
            {
                Expense = expense,
                WalletId = wallet,
                CurrencyId = currency.Guid
            };
            await this.SetTransactionFormsDropdowns(model);
            return
                this.View(model);
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
            var model = Mapper.Map<TransactionServiceModel>(transaction);

            try
            {
                await this._transactionService.Create(model);
            }
            catch (ServiceValidationException exception)
            {
                ModelState.AddModelErrors(exception);
            }

            //Check server validation
            if (ModelState.IsValid)
            {
                return this.RedirectToAction("Index", new {wallet = transaction.WalletId});
            }
            await this.SetTransactionFormsDropdowns(transaction);
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
            var userId = await this.CurrentProfileId();
            TransactionServiceModel transaction;
            try
            {
                //find transaction by it's Id
                transaction = await this._transactionService.GetTransactionById(id, userId);
            }
            catch (SecurityException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                    TransactionResource.PermissionErrorEdit);
            }
            if (transaction == null)
            {
                return this.HttpNotFound();
            }
            var model = Mapper.Map<EditTransactionModel>(transaction);
            await this.SetTransactionFormsDropdowns(model);

            return this.View(model);
        }


        /// <summary>
        ///     Editing of transaction
        /// </summary>
        /// <param name="transaction">Instance of EditTransactionModel</param>
        /// <returns>Redirect to Index</returns>
        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditTransactionModel transaction)
        {
            var model = Mapper.Map<TransactionServiceModel>(transaction);
            try
            {
                await this._transactionService.Edit(model);
            }
            catch (ServiceValidationException exception)
            {
                ModelState.AddModelErrors(exception);
            }

            //check if model is valid
            if (ModelState.IsValid)
            {
                return this.RedirectToAction("Index", new {wallet = transaction.WalletId});
            }
            await this.SetTransactionFormsDropdowns(transaction);
            return this.View(transaction);
        }


        /// <summary>
        ///     Deleting transactions
        /// </summary>
        /// <param name="id">Id of transaction</param>
        /// <returns>View with delete confirmation</returns>
        public async Task<ActionResult> Delete(Guid id)
        {
            var userId = await this.CurrentProfileId();

            TransactionServiceModel transaction;

            try
            {
                //find transaction by it's Id
                transaction = await this._transactionService.GetTransactionById(id, userId);
            }
            catch (SecurityException)
            {
                //error "You don't have permission to edit this transaction"
                return this.RedirectToAction("Index");
            }
            if (transaction == null)
            {
                //error "Transaction not found"
                return this.RedirectToAction("Index");
            }

            return this.View(Mapper.Map<EditTransactionModel>(transaction));
        }

        /// <summary>
        ///     Deleting transactions confirmation
        /// </summary>
        /// <param name="model">
        ///     TransactionShowModel of transaction to delete<</param>
        /// <returns>Redirect to Index</returns>
        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed([Bind(Include = "Id")] TransactionShowModel model)
        {
            if (!ModelState.IsValid)
            {
                this.AddError(SharedResource.ModelStateIsNotValid);
                return this.RedirectToAction("Index");
            }

            Guid walletId;

            try
            {
                //removing transaction from DB
                walletId = await this._transactionService.RemoveTransaction(await this.CurrentProfileId(), model.Id);
            }
            catch (SecurityException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                    TransactionResource.PermissionErrorDelete);
            }
            return this.RedirectToAction("Index", new {wallet = walletId});
        }

        /// <summary>
        ///     Export of all transactions with specified parameters
        /// </summary>
        /// <param name="wallet">Id of wallet</param>
        /// <param name="category">Id of category</param>
        /// <param name="budget">Id of budgets</param>
        /// <returns>File with all transactions</returns>
        public async Task<ActionResult> Export(Guid? wallet, Guid? category, Guid? budget)
        {
            var id = await this.CurrentProfileId();
            try
            {
                var file = await this._transactionService.ExportToCsv(id, wallet, category, budget);
                return this.File(new UTF8Encoding().GetBytes(file), "text/csv", "transactions.csv");
            }
            catch (SecurityException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                    TransactionResource.PermissionError);
            }
        }

        /// <summary>
        ///     Import of transaction
        /// </summary>
        /// <returns>Import transaction view</returns>
        public ActionResult Import()
        {
            return this.View();
        }

        /// <summary>
        ///     Importation of uploaded file with transactions
        /// </summary>
        /// <param name="file">File to import transactions from</param>
        /// <returns>Redirect to Index</returns>
        [HttpPost, ActionName("Import")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Import(HttpPostedFileBase file)
        {
            var id = await this.CurrentProfileId();
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    using (var reader = new StreamReader(file.InputStream))
                    {
                        await this._transactionService.ImportFromCsv(id, reader.ReadToEnd());
                    }
                    return this.RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("File", TransactionResource.FileFormatError);
            }

            return this.View();
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
        ///     Sets data in drop down lists and if transactions are editable for index view
        /// </summary>
        /// <param name="category">Id of category</param>
        /// <param name="budget">Id of budget</param>
        /// <param name="walletId">Id of wallet</param>
        private async Task SetIndexViewData(Guid? category, Guid? budget, Guid walletId)
        {
            var id = await this.CurrentProfileId();
            // get all Wallets user has access to
            ViewBag.wallet = await this._transactionService.GetViewableWalletsSelection(id, walletId);
            ViewBag.displayedWalletId = walletId;
            ViewBag.category =
                await this._transactionService.GetCategoriesSelectionFilter(walletId, category.GetValueOrDefault());
            ViewBag.selectedCategoryId = category;
            ViewBag.budget =
                await this._transactionService.GetBudgetsSelectionFilter(id, walletId, budget.GetValueOrDefault());
            ViewBag.selectedBudgetId = budget;
            // when user doesn't have permission to manipulate with transaction show view without edit and delete
            ViewBag.editable = (await this._transactionService.GetPermission(id, walletId)).Permission !=
                               PermissionEnum.Read;
        }

        private async Task SetTransactionFormsDropdowns(NewTransactionModel transaction)
        {
            transaction.Currencies = await this._transactionService.GetCurrenciesSelection();
            transaction.Categories = await this._transactionService.GetCategoriesSelection(transaction.Expense);
            transaction.Budgets = await this._transactionService.GetBudgetsSelection(await this.CurrentProfileId());
        }
    }
}