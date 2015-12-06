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
using ExpenseManager.Resources.TransactionResources;
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
        /// <returns>View with transaction</returns>
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
            // get user permission for selected wallet
            //TODO this should probably be made in other way
            var permission =
                await this._transactionService.GetPermission(id, walletId);

            // get all Wallets user has access to
            ViewBag.wallet = await this._transactionService.GetViewableWalletsSelection(id, walletId);
            ViewBag.displayedWalletId = walletId;
            ViewBag.category =
                await this._transactionService.GetCategoriesSelectionFilter(walletId, category.GetValueOrDefault());
            ViewBag.selectedCategoryId = category;
            ViewBag.budget =
                await this._transactionService.GetBudgetsSelectionFilter(id, walletId, budget.GetValueOrDefault());
            ViewBag.selectedBudgetId = budget;
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

            if (category != null)
            {
                list = list.Where(model => model.CategoryId == category.Value);
            }
            if (budget != null)
            {
                list = list.Where(model => model.BudgetId == budget.Value);
            }

            var showModels = Enumerable.Reverse(list.Select(Mapper.Map<TransactionShowModel>)).ToList();
            var pageNumber = (page ?? 1);

            // when user doesn't have permission to manipulate with transaction show view without edit and delete
            ViewBag.editable = permission.Permission != PermissionEnum.Read;
            return this.View("Index",
                showModels.OrderByDescending(model => model.Date).ToPagedList(pageNumber, PageSize));
        }


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
            return
                this.View(new NewTransactionModel
                {
                    Expense = expense,
                    WalletId = wallet,
                    CurrencyId = currency.Guid,
                    Categories = await this._transactionService.GetCategoriesSelection(expense),
                    Currencies = await this._transactionService.GetCurrenciesSelection(),
                    Budgets = await this._transactionService.GetBudgetsSelection(userId)
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
            var model = Mapper.Map<TransactionServiceModel>(transaction);

            try
            {
                await this._transactionService.Create(model);
            }
            catch (ValidationException exception)
            {
                ModelState.AddModelErrors(exception);
            }

            //Check server validation
            if (ModelState.IsValid)
            {
                return this.RedirectToAction("Index", new {wallet = transaction.WalletId});
            }
            transaction.Currencies = await this._transactionService.GetCurrenciesSelection();
            transaction.Categories = await this._transactionService.GetCategoriesSelection(transaction.Expense);
            transaction.Budgets = await this._transactionService.GetBudgetsSelection(await this.CurrentProfileId());
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

            model.Currencies = await this._transactionService.GetCurrenciesSelection();
            model.Categories = await this._transactionService.GetCategoriesSelection(model.Expense);
            model.Budgets = await this._transactionService.GetBudgetsSelection(userId);

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
            catch (ValidationException exception)
            {
                ModelState.AddModelErrors(exception);
            }

            //check if model is valid
            if (ModelState.IsValid)
            {
                return this.RedirectToAction("Index", new {wallet = transaction.WalletId});
            }
            transaction.Currencies = await this._transactionService.GetCurrenciesSelection();
            transaction.Categories = await this._transactionService.GetCategoriesSelection(transaction.Expense);
            transaction.Budgets = await this._transactionService.GetBudgetsSelection(await this.CurrentProfileId());
            return this.View(transaction);
        }

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
        ///     Deleting transactions
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
                // error
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

        public ActionResult Import()
        {
            return this.View();
        }

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
    }

    public static class MvcValidationExtension
    {
        public static void AddModelErrors(this ModelStateDictionary state,
            ValidationException exception)
        {
            foreach (var error in exception.Erorrs)
            {
                state.AddModelError(error.Key, error.Value);
            }
        }
    }
}