using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using ExpenseManager.BusinessLogic;
using ExpenseManager.BusinessLogic.TransactionServices;
using ExpenseManager.BusinessLogic.TransactionServices.Models;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Providers.Factory;
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
                wallet = await this._transactionService.GetDefaultWallet(id);
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
                    "You don't have permission to view this wallet");
            }

            if (category != null)
            {
                list = list.Where(s => s.CategoryId == category.Value);
            }
            if (budget != null)
            {
                list = list.Where(s => s.BudgetId == budget.Value);
            }

            var showModels = Enumerable.ToList(list.Select(Mapper.Map<TransactionShowModel>));
            var pageNumber = (page ?? 1);

            // when user doesn't have permission to manipulate with transaction show view without edit and delete
            ViewBag.editable = permission.Permission != PermissionEnum.Read;
            return this.View("Index", showModels.OrderByDescending(t => t.Date).ToPagedList(pageNumber, PageSize));
        }


        public async Task<ActionResult> ExpenseIncome(Guid? wallet)
        {
            if (wallet == null)
            {
                wallet = await this._transactionService.GetDefaultWallet(await this.CurrentProfileId());
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
                    "You don't have permission to create transactions in this wallet");
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
            var dto = Mapper.Map<TransactionServiceModel>(transaction);

            try
            {
                await this._transactionService.Create(dto);
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelErrors(ex);
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
                    "You don't have permission to edit this transaction");
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
            var dto = Mapper.Map<TransactionServiceModel>(transaction);
            try
            {
                await this._transactionService.Edit(dto);
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelErrors(ex);
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


        /// <summary>
        ///     Deleting transactions
        /// </summary>
        /// <param name="id">Id of transaction to edit</param>
        /// <returns>Redirect to Index</returns>
        // POST: Transactions/Delete/5
        public async Task<ActionResult> Delete(Guid id)
        {
            Guid walletId;
            try
            {
                //removing transaction from DB
                walletId = await this._transactionService.RemoveTransaction(id, await this.CurrentProfileId());
            }
            catch (SecurityException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                    "You don't have permission to delete this transaction");
            }
            return this.RedirectToAction("Index", new {wallet = walletId});
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