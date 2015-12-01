using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using ExpenseManager.BusinessLogic.DTOs;
using ExpenseManager.BusinessLogic.ExchangeRates;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Enums;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.BusinessLogic.TransactionServices
{
    public class TransactionService
    {
        private readonly IBudgetsProvider _budgetsProvider;
        private readonly ITransactionsProvider _transactionsProvider;
        private readonly IWalletsProvider _walletsProvider;

        public TransactionService(IBudgetsProvider budgetsProvider, ITransactionsProvider transactionsProvider,
            IWalletsProvider walletsProvider)
        {
            this._budgetsProvider = budgetsProvider;
            this._transactionsProvider = transactionsProvider;
            this._walletsProvider = walletsProvider;

            //TODO MOVE initialization
            Mapper.CreateMap<TransactionDTO, Transaction>()
                .ForMember(entity => entity.Guid, options => options.MapFrom(dto => dto.Id))
                .ForMember(entity => entity.Amount,
                    options => options.MapFrom(dto => dto.Expense ? dto.Amount*-1 : dto.Amount))
                .ForMember(entity => entity.Date, options => options.MapFrom(dto => dto.Date))
                .ForMember(entity => entity.Description, options => options.MapFrom(dto => dto.Description));

            Mapper.CreateMap<TransactionDTO, RepeatableTransaction>()
                .ForMember(entity => entity.FrequencyType, options => options.MapFrom(dto => dto.FrequencyType))
                .ForMember(entity => entity.LastOccurrence, options => options.MapFrom(dto => dto.LastOccurrence.Value))
                .ForMember(entity => entity.NextRepeat, options => options.MapFrom(dto => dto.NextRepeat));
        }

        //TODO make private
        public void ValidateTransaction(TransactionDTO transaction)
        {
            var ex = new ValidationException();
            if (transaction.Amount <= 0)
            {
                ex.Erorrs.Add("Amount", "Transaction amount must be greater than zero");
            }
            if (transaction.CategoryId == Guid.Empty)
            {
                ex.Erorrs.Add("Category", "Category field is required.");
            }
            if (transaction.CurrencyId == Guid.Empty)
            {
                ex.Erorrs.Add("Currency", "Currency field is required.");
            }
            if (transaction.IsRepeatable)
            {
                if (transaction.LastOccurrence == null)
                {
                    ex.Erorrs.Add("LastOccurrence", "Date of last occurrence must be set");
                }
                else if (transaction.Date >= transaction.LastOccurrence.GetValueOrDefault())
                {
                    ex.Erorrs.Add("LastOccurrence",
                        "Date until which transaction should be repeated must be after first transaction occurrence");
                }
                if (transaction.NextRepeat == null || transaction.NextRepeat <= 0)
                {
                    ex.Erorrs.Add("NextRepeat", "Frequency must be positive number");
                }
            }

            if (ex.Erorrs.Count != 0)
                throw ex;
        }

        public async Task Create(TransactionDTO transaction)
        {
            this.ValidateTransaction(transaction);
            //create new Transaction entity and fill it from DTO
            var transactionEntity = await this.FillTransaction(transaction);
            transactionEntity.Guid = new Guid();
            //check if transaction should be repeatable
            await this.AddOrUpdate(transactionEntity);
            if (transaction.IsRepeatable)
            {
                //create new repeatable transaction entity and fill from DTO
                var repeatableTransaction = Mapper.Map<RepeatableTransaction>(transaction);
                repeatableTransaction.Guid = new Guid();
                await this.AddOrUpdate(repeatableTransaction);
            }
        }

        public async Task AddOrUpdate(Transaction transaction)
        {
            var walletCurrency = await this.GetDefaultCurrencyInWallet(transaction.Wallet.Guid);

            if (transaction.Currency.Name != walletCurrency.Name)
            {
                Transformation.ChangeCurrency(transaction, walletCurrency);
            }

            await this._transactionsProvider.AddOrUpdateAsync(transaction);
        }

        public async Task AddOrUpdate(RepeatableTransaction repeatableTransaction)
        {
            await this._transactionsProvider.AddOrUpdateAsync(repeatableTransaction);
        }

        public async Task<Guid> RemoveTransaction(Guid userId, Guid transactionId)
        {
            //find transaction by its Id
            var transaction =
                await this.GetTransactionById(transactionId);
            //if (!await this.HasWritePermission(transaction))
            if (
                !await
                    this.HasWritePermission(userId, transaction.Wallet.Guid))
            {
                throw new SecurityException();
            }
            var walletId = transaction.Wallet.Guid;
            //get if transaction is also in repeatable transactions
            var repeatableTransaction = await this.GetRepeatableTransactionByFirstTransactionId(transactionId);
            //check if transaction was not repeatable
            if (repeatableTransaction != null)
            {
                //if true remove it from repeatable transactions first
                await this.Remove(repeatableTransaction);
            }
            await this._transactionsProvider.DeteleAsync(transaction);
            return walletId;
        }

        public async Task Remove(RepeatableTransaction repeatableTransaction)
        {
            await this._transactionsProvider.DeteleAsync(repeatableTransaction);
        }

        public async Task<Transaction> GetTransactionById(Guid transactionId)
        {
            return
                await this._transactionsProvider.Transactions.Where(t => t.Guid == transactionId).FirstOrDefaultAsync();
        }

        public async Task<RepeatableTransaction> GetRepeatableTransactionByFirstTransactionId(Guid transactionId)
        {
            return await
                this._transactionsProvider.RepeatableTransactions.FirstOrDefaultAsync(
                    a => a.FirstTransaction.Guid == transactionId);
        }

        public async Task<Category> GetCategoryById(Guid categoryId)
        {
            return
                await this._transactionsProvider.Categories.Where(t => t.Guid == categoryId).FirstOrDefaultAsync();
        }

        public async Task<Currency> GetCurrencyById(Guid currencyId)
        {
            return
                await this._transactionsProvider.Currencies.Where(t => t.Guid == currencyId).FirstOrDefaultAsync();
        }

        public async Task<Budget> GetBudgetById(Guid budgetId)
        {
            return
                await this._transactionsProvider.Budgets.Where(b => b.Guid == budgetId).FirstOrDefaultAsync();
        }


        public async Task<List<Transaction>> GetAllTransactionsInWallet(Guid walletId)
        {
            return
                await
                    this._transactionsProvider.Transactions.Where(user => user.Wallet.Guid == walletId)
                        .ToListAsync();
        }

        public async Task<RepeatableTransaction> GetRepeatableTransaction(Guid transactionId)
        {
            return
                await
                    this._transactionsProvider.RepeatableTransactions.FirstOrDefaultAsync(
                        a => a.FirstTransaction.Guid == transactionId);
        }

        public async Task<Wallet> GetWalletById(Guid walletId)
        {
            return
                await this._transactionsProvider.Wallets.Where(w => w.Guid == walletId).FirstOrDefaultAsync();
        }


        public async Task<Currency> GetDefaultCurrencyInWallet(Guid walletId)
        {
            return
                await
                    this._transactionsProvider.Wallets.Where(wallet => wallet.Guid == walletId)
                        .Select(w => w.Currency)
                        .FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Provides selectable list of Currencies which are available
        /// </summary>
        /// <returns>List of SelectListItem for Currencies</returns>
        public async Task<List<SelectListItem>> GetCurrenciesSelection()
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
        public async Task<List<SelectListItem>> GetCategoriesSelection(bool expense)
        {
            var transactionType = this.GetCategoryType(expense);
            var result = this._transactionsProvider.Categories.Where(
                m => m.Type == transactionType || m.Type == CategoryType.IncomeAndExpense);
            return await ReturnSelectionForCategory(result);
        }


        /// <summary>
        ///     Provides selectable list of all categories
        /// </summary>
        /// <returns>List of SelectListItem for Categories</returns>
        public async Task<List<SelectListItem>> GetAllCategoriesSelection()
        {
            return await ReturnSelectionForCategory(this._transactionsProvider.Categories);
        }

        public async Task<SelectList> GetCategoriesSelectionFilter(Guid walletId, Guid categoryId)
        {
            var usedCategories = (await this.GetAllTransactionsInWallet(walletId)).GroupBy(c => c.Category.Guid)
                .Select(g => g.First());
            var selectList = usedCategories.Select(
                category =>
                    new SelectListItem
                    {
                        Value = category.Category.Guid.ToString(),
                        Text = category.Category.Name
                    });
            return new SelectList(selectList, "Value", "Text", categoryId);
        }

        /// <summary>
        ///     Returns guid of User's default Wallet
        /// </summary>
        /// <param name="userId">guid of user</param>
        /// <returns></returns>
        public async Task<Guid> GetDefaultWallet(Guid userId)
        {
            return await this._walletsProvider.Wallets.Where(w => w.Owner.Guid == userId)
                .Select(w => w.Guid)
                .FirstOrDefaultAsync();
        }

        //TODO will be only private
        public async Task<WalletAccessRight> GetPermission(Guid userId, Guid walletId)
        {
            return await
                this._walletsProvider.WalletAccessRights // TODO NOT IMPLEMENTED< SHOULDNT BE IMPLEMENTED FIX THIS
                    .FirstOrDefaultAsync(
                        r =>
                            r.UserProfile.Guid == userId && r.Wallet.Guid == walletId);
        }

        //TODO will be only private
        public async Task<bool> HasWritePermission(Guid userId, Guid walletId)
        {
            var permission = await this._walletsProvider.WalletAccessRights
                .FirstOrDefaultAsync(
                    r =>
                        r.Wallet.Guid == walletId &&
                        r.UserProfile.Guid == userId);
            return permission != null && permission.Permission != PermissionEnum.Read;
        }

        /// <summary>
        ///     Provides selectable list of wallets available to user
        /// </summary>
        /// <returns>List of SelectListItem for Budgets</returns>
        public async Task<List<SelectListItem>> GetAllReadableWalletsSelection(Guid userId)
        {
            return
                await
                    this._walletsProvider.WalletAccessRights.Where(
                        access =>
                            access.UserProfile.Guid == userId)
                        .Select(
                            budget =>
                                new SelectListItem
                                {
                                    Value = budget.Wallet.Guid.ToString(),
                                    Text = budget.Wallet.Name
                                })
                        .ToListAsync();
        }

        public async Task<SelectList> GetViewableWalletsSelection(Guid userId, Guid walletId)
        {
            return new SelectList(await this.GetAllReadableWalletsSelection(userId), "Value", "Text", walletId);
        }

        /// <summary>
        ///     Provides selectable list of Budgets where can user write transactions
        /// </summary>
        /// <returns>List of SelectListItem for Budgets</returns>
        public async Task<List<SelectListItem>> GetBudgetsSelection(Guid userId)
        {
            return await this.GetBudgetsSelection(userId, PermissionEnum.Write);
        }


        /// <summary>
        ///     Provides selectable list of Budgets which can user read
        /// </summary>
        /// <returns>List of SelectListItem for Budgets</returns>
        public async Task<List<SelectListItem>> GetReadableBudgetsSelection(Guid userId)
        {
            return await this.GetBudgetsSelection(userId, PermissionEnum.Read);
        }


        public async Task<SelectList> GetBudgetsSelectionFilter(Guid userId, Guid walletId, Guid budgetId)
        {
            var allAccessibleBudgetsId = await
                this._budgetsProvider.BudgetAccessRights.Where(
                    access =>
                        access.UserProfile.Guid == userId &&
                        access.Permission >= PermissionEnum.Write)
                    .Select(budget => budget.Budget.Guid)
                    .ToListAsync();
            var usedBudgets =
                (await this.GetAllTransactionsInWallet(walletId)).Where(
                    b => b.Budget != null && allAccessibleBudgetsId.Contains(b.Budget.Guid)).GroupBy(b => b.Budget.Guid)
                    .Select(g => g.First());
            var selectList = usedBudgets.Select(
                budget =>
                    new SelectListItem
                    {
                        Value = budget.Budget.Guid.ToString(),
                        Text = budget.Budget.Name
                    }).Distinct();
            return new SelectList(selectList, "Value", "Text", budgetId);
        }

        #region private

        private CategoryType GetCategoryType(bool expense)
        {
            return expense ? CategoryType.Expense : CategoryType.Income;
        }

        private static async Task<List<SelectListItem>> ReturnSelectionForCategory(IQueryable<Category> result)
        {
            return await result.Select(
                category => new SelectListItem {Value = category.Guid.ToString(), Text = category.Name})
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> GetBudgetsSelection(Guid userId, PermissionEnum minimalPermission)
        {
            return
                await
                    this._budgetsProvider.BudgetAccessRights.Where(
                        access =>
                            access.UserProfile.Guid == userId &&
                            access.Permission >= minimalPermission)
                        .Select(
                            budget =>
                                new SelectListItem
                                {
                                    Value = budget.Budget.Guid.ToString(),
                                    Text = budget.Budget.Name
                                })
                        .ToListAsync();
        }

        private async Task<Transaction> FillTransaction(TransactionDTO transaction)
        {
            var entity = Mapper.Map<Transaction>(transaction);
            entity.Wallet =
                await this.GetWalletById(transaction.WalletId);
            //check if budget was set to category in model
            if (transaction.BudgetId != null)
            {
                entity.Budget =
                    await this.GetBudgetById(transaction.BudgetId.Value);
            }
            entity.Currency =
                await this.GetCurrencyById(transaction.CurrencyId);
            entity.Category =
                await this.GetCategoryById(transaction.CategoryId);
            return entity;
        }

        #endregion
    }
}