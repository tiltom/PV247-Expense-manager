using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using ExpenseManager.BusinessLogic.ExchangeRates;
using ExpenseManager.BusinessLogic.TransactionServices.Models;
using ExpenseManager.BusinessLogic.Validators;
using ExpenseManager.BusinessLogic.Validators.Extensions;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Enums;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.Queryable;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Wallets;
using ExpenseManager.Resources;
using ExpenseManager.Resources.TransactionResources;

namespace ExpenseManager.BusinessLogic.TransactionServices
{
    public class TransactionService : ServiceWithWallet, IServiceValidation<TransactionServiceModel>
    {
        public const string DateFormat = "dd.MM.yyyy";
        private readonly IBudgetsProvider _budgetsProvider;
        private readonly ITransactionsProvider _transactionsProvider;
        private readonly TransactionValidator _validator;
        private readonly IWalletsProvider _walletsProvider;

        public TransactionService(IBudgetsProvider budgetsProvider, ITransactionsProvider transactionsProvider,
            IWalletsProvider walletsProvider)
        {
            this._budgetsProvider = budgetsProvider;
            this._transactionsProvider = transactionsProvider;
            this._walletsProvider = walletsProvider;
            this._validator = new TransactionValidator();
        }

        protected override IWalletsQueryable WalletsProvider
        {
            get { return this._transactionsProvider; }
        }

        public void Validate(TransactionServiceModel transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            this._validator.ValidateAndThrowCustomException(transaction);
        }

        public async Task Create(TransactionServiceModel transaction)
        {
            this.Validate(transaction);
            //create new Transaction entity and fill it from DTO
            var transactionEntity = await this.FillTransaction(transaction, new Transaction {Guid = new Guid()});
            transactionEntity.Guid = new Guid();
            //check if transaction should be repeatable
            await this.AddOrUpdate(transactionEntity);
            if (transaction.IsRepeatable)
            {
                //create new repeatable transaction entity and fill from DTO
                var repeatableTransaction = new RepeatableTransaction
                {
                    Guid = new Guid(),
                    FirstTransaction = transactionEntity
                };
                this.FillRepeatableTransaction(transaction, repeatableTransaction);
                await this._transactionsProvider.AddOrUpdateAsync(repeatableTransaction);
            }
        }

        public async Task<HttpStatusCodeResult> UpdateRepeatableTransactions()
        {
            var repeatableTransactions = await this._transactionsProvider.RepeatableTransactions.ToListAsync();
            foreach (var rt in repeatableTransactions)
            {
                var expectedNewOccurance = rt.LastOccurrence.AddDays(rt.NextRepeat);

                while (expectedNewOccurance.Subtract(DateTime.Today).Days <= 0)
                {
                    var transactionToAdd = new Transaction();
                    transactionToAdd.Amount = rt.FirstTransaction.Amount;
                    transactionToAdd.Budget = rt.FirstTransaction.Budget;
                    transactionToAdd.Category = rt.FirstTransaction.Category;
                    transactionToAdd.Currency = rt.FirstTransaction.Currency;
                    transactionToAdd.Date = expectedNewOccurance;
                    transactionToAdd.Description = rt.FirstTransaction.Description;
                    transactionToAdd.Wallet = rt.FirstTransaction.Wallet;
                    await this._transactionsProvider.AddOrUpdateAsync(transactionToAdd);

                    rt.LastOccurrence = transactionToAdd.Date;
                    await this._transactionsProvider.AddOrUpdateAsync(rt);

                    expectedNewOccurance = expectedNewOccurance.AddDays(rt.NextRepeat);
                }
            }

            return new HttpStatusCodeResult(200);
        }

        public async Task Edit(TransactionServiceModel transaction)
        {
            this.Validate(transaction);
            //find transaction by Id
            var transactionEntity = await this.GetTransactionById(transaction.Id);
            //update entity properties from DTO
            await this.FillTransaction(transaction, transactionEntity);

            await this.AddOrUpdate(transactionEntity);

            //find if transaction is repeatable in DB
            var repeatableTransaction =
                await this.GetRepeatableTransactionByFirstTransactionId(transaction.Id);
            //check if transaction was set as repeatable in model
            if (transaction.IsRepeatable)
            {
                //check if transaction was also set repeatable in model
                if (repeatableTransaction == null)
                {
                    //if not create new repeatable transaction entity
                    repeatableTransaction = new RepeatableTransaction
                    {
                        Guid = new Guid(),
                        FirstTransaction = transactionEntity
                    };
                    this.FillRepeatableTransaction(transaction, repeatableTransaction);
                    await this._transactionsProvider.AddOrUpdateAsync(repeatableTransaction);
                }
                // if transaction exists in repeatable transactions in DB update it
                else
                {
                    this.FillRepeatableTransaction(transaction, repeatableTransaction);
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
            }
        }

        public async Task<Guid> RemoveTransaction(Guid userId, Guid transactionId)
        {
            //find transaction by its Id
            var transaction =
                await this.GetTransactionById(transactionId);
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
                await this._transactionsProvider.DeteleAsync(repeatableTransaction);
            }
            await this._transactionsProvider.DeteleAsync(transaction);
            return walletId;
        }

        public async Task<string> ExportToCsv(Guid userId, Guid? wallet, Guid? category, Guid? budget)
        {
            if (wallet == null)
            {
                wallet = await this.GetWalletIdByUserId(userId);
            }
            var walletId = wallet.Value;
            IEnumerable<TransactionShowServiceModel> list =
                await this.GetAllTransactionsInWalletWithCurrency(userId, walletId);
            if (category != null)
            {
                list = list.Where(model => model.CategoryId == category.Value);
            }
            if (budget != null)
            {
                list = list.Where(model => model.BudgetId == budget.Value);
            }
            TextWriter textWriter = new StringWriter();
            var writer = new CsvWriter(textWriter);
            writer.Configuration.RegisterClassMap<TransactionExportMap>();
            var options = new TypeConverterOptions
            {
                Format = DateFormat
            };
            TypeConverterOptionsFactory.AddOptions<DateTime>(options);
            writer.WriteRecords(list);
            return textWriter.ToString();
        }

        public async Task ImportFromCsv(Guid userId, string file)
        {
            var reader = new CsvReader(new StringReader(file));
            reader.Configuration.RegisterClassMap<TransactionExportMap>();
            reader.Configuration.HasHeaderRecord = true;
            while (reader.Read())
            {
                var categoryName = reader.GetField<string>(TransactionResource.CategoryName);
                var budgetName = reader.GetField<string>(TransactionResource.BudgetName);
                var currencyCode = reader.GetField<string>(SharedResource.Currency);
                var model = new TransactionServiceModel
                {
                    Amount = reader.GetField<decimal>(SharedResource.Amount),
                    Date = DateTime.Parse(reader.GetField<string>(SharedResource.Date)),
                    Description = reader.GetField<string>(SharedResource.Description),
                    WalletId = await this.GetWalletIdByUserId(userId)
                };
                model.CurrencyId = (await this.GetCurrencyByCode(currencyCode)).Guid;
                model.CategoryId = (await this.GetCategoryByName(categoryName)).Guid;
                if (budgetName != string.Empty)
                {
                    model.BudgetId = (await this.GetBudgetByName(budgetName)).Guid;
                }
                if (model.Amount < 0)
                {
                    model.Expense = true;
                    model.Amount *= -1;
                }
                await this.Create(model);
            }
        }

        public async Task<TransactionServiceModel> GetTransactionById(Guid transactionId, Guid userId)
        {
            var entity =
                await
                    this._transactionsProvider.Transactions.Where(transaction => transaction.Guid == transactionId)
                        .FirstOrDefaultAsync();
            if (!await this.HasWritePermission(userId, entity.Wallet.Guid))
            {
                throw new SecurityException();
            }
            var model = Mapper.Map<TransactionServiceModel>(entity);
            var repeatableTransaction =
                await this.GetRepeatableTransactionByFirstTransactionId(entity.Guid);
            if (repeatableTransaction != null)
            {
                model.IsRepeatable = true;
                model.NextRepeat = repeatableTransaction.NextRepeat;
                model.FrequencyType = repeatableTransaction.FrequencyType;
                model.LastOccurrence = repeatableTransaction.LastOccurrence;
            }

            return model;
        }

        public async Task<RepeatableTransaction> GetRepeatableTransactionByFirstTransactionId(Guid transactionId)
        {
            return await
                this._transactionsProvider.RepeatableTransactions.FirstOrDefaultAsync(
                    repeatableTransaction => repeatableTransaction.FirstTransaction.Guid == transactionId);
        }

        public async Task<Category> GetCategoryById(Guid categoryId)
        {
            return
                await
                    this._transactionsProvider.Categories.Where(category => category.Guid == categoryId)
                        .FirstOrDefaultAsync();
        }

        public async Task<Category> GetCategoryByName(string categoryName)
        {
            return
                await
                    this._transactionsProvider.Categories.Where(category => category.Name == categoryName)
                        .FirstOrDefaultAsync();
        }

        public async Task<Currency> GetCurrencyById(Guid currencyId)
        {
            return
                await
                    this._transactionsProvider.Currencies.Where(currency => currency.Guid == currencyId)
                        .FirstOrDefaultAsync();
        }

        private async Task<Currency> GetCurrencyByCode(string currencyCode)
        {
            return
                await
                    this._transactionsProvider.Currencies.Where(currency => currency.Code == currencyCode)
                        .FirstOrDefaultAsync();
        }

        public async Task<Budget> GetBudgetById(Guid budgetId)
        {
            return
                await this._transactionsProvider.Budgets.Where(budget => budget.Guid == budgetId).FirstOrDefaultAsync();
        }

        public async Task<Budget> GetBudgetByName(string budgetName)
        {
            return
                await
                    this._transactionsProvider.Budgets.Where(budget => budget.Name == budgetName).FirstOrDefaultAsync();
        }

        public async Task<List<TransactionShowServiceModel>> GetAllTransactionsInWallet(Guid userId, Guid walletId)
        {
            var modelList = await this.GetAllTransactionsInWalletLoader(userId, walletId);
            return new List<TransactionShowServiceModel>(modelList);
        }

        public async Task<List<TransactionServiceExportModel>> GetAllTransactionsInWalletWithCurrency(Guid userId,
            Guid walletId)
        {
            return await this.GetAllTransactionsInWalletLoader(userId, walletId);
        }

        public async Task<Currency> GetDefaultCurrencyInWallet(Guid walletId)
        {
            return
                await
                    this._transactionsProvider.Wallets.Where(wallet => wallet.Guid == walletId)
                        .Select(wallet => wallet.Currency)
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
                category => category.Type == transactionType || category.Type == CategoryType.IncomeAndExpense);
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
            var usedCategories = (await this.GetAllTransactionsInWallet(walletId)).GroupBy(
                transaction => transaction.Category.Guid)
                .Select(transactions => transactions.First());
            var selectList = usedCategories.Select(
                category =>
                    new SelectListItem
                    {
                        Value = category.Category.Guid.ToString(),
                        Text = category.Category.Name
                    });
            return new SelectList(selectList, "Value", "Text", categoryId);
        }


        //TODO will be only private
        public async Task<WalletAccessRight> GetPermission(Guid userId, Guid walletId)
        {
            return await
                this._walletsProvider.WalletAccessRights // TODO NOT IMPLEMENTED< SHOULDNT BE IMPLEMENTED FIX THIS
                    .FirstOrDefaultAsync(
                        right =>
                            right.UserProfile.Guid == userId && right.Wallet.Guid == walletId);
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
                    transaction =>
                        transaction.Budget != null && allAccessibleBudgetsId.Contains(transaction.Budget.Guid))
                    .GroupBy(transaction => transaction.Budget.Guid)
                    .Select(transactions => transactions.First());
            var selectList = usedBudgets.Select(
                budget =>
                    new SelectListItem
                    {
                        Value = budget.Budget.Guid.ToString(),
                        Text = budget.Budget.Name
                    }).Distinct();
            return new SelectList(selectList, "Value", "Text", budgetId);
        }

        private async Task<List<TransactionServiceExportModel>> GetAllTransactionsInWalletLoader(Guid userId,
            Guid walletId)
        {
            if (await this.GetPermission(userId, walletId) == null)
            {
                throw new SecurityException();
            }
            var list =
                await
                    this._transactionsProvider.Transactions.Where(transaction => transaction.Wallet.Guid == walletId)
                        .ToListAsync();
            var modelList = new List<TransactionServiceExportModel>();
            foreach (var transaction in list)
            {
                var model = Mapper.Map<TransactionServiceExportModel>(transaction);
                model.IsRepeatable = await this.GetRepeatableTransactionByFirstTransactionId(transaction.Guid) != null;
                modelList.Add(model);
            }
            return modelList;
        }

        public sealed class TransactionExportMap : CsvClassMap<TransactionServiceExportModel>
        {
            public TransactionExportMap()
            {
                this.Map(model => model.Amount).Name(SharedResource.Amount);
                this.Map(model => model.CurrencyCode).Name(SharedResource.Currency);
                this.Map(model => model.Date).Name(SharedResource.Date);
                this.Map(model => model.Description).Name(SharedResource.Description);
                this.Map(model => model.CategoryName).Name(TransactionResource.CategoryName);
                this.Map(model => model.BudgetName).Name(TransactionResource.BudgetName);
            }
        }

        #region private

        private CategoryType GetCategoryType(bool expense)
        {
            return expense ? CategoryType.Expense : CategoryType.Income;
        }

        private async Task AddOrUpdate(Transaction transaction)
        {
            var walletCurrency = await this.GetDefaultCurrencyInWallet(transaction.Wallet.Guid);

            if (transaction.Currency.Name != walletCurrency.Name)
            {
                Transformation.ChangeCurrency(transaction, walletCurrency);
            }

            await this._transactionsProvider.AddOrUpdateAsync(transaction);
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

        private async Task<Transaction> GetTransactionById(Guid transactionId)
        {
            return
                await
                    this._transactionsProvider.Transactions.Where(transaction => transaction.Guid == transactionId)
                        .FirstOrDefaultAsync();
        }

        public async Task<List<Transaction>> GetAllTransactionsInWallet(Guid walletId)
        {
            return
                await
                    this._transactionsProvider.Transactions.Where(transaction => transaction.Wallet.Guid == walletId)
                        .ToListAsync();
        }

        private async Task<bool> HasWritePermission(Guid userId, Guid walletId)
        {
            var permission = await this._walletsProvider.WalletAccessRights
                .FirstOrDefaultAsync(
                    right =>
                        right.Wallet.Guid == walletId &&
                        right.UserProfile.Guid == userId);
            return permission != null && permission.Permission != PermissionEnum.Read;
        }

        private async Task<Transaction> FillTransaction(TransactionServiceModel transaction, Transaction entity)
        {
            //setting properties from transaction
            entity.Amount = transaction.Amount;
            if (transaction.Expense)
            {
                entity.Amount *= -1;
            }
            entity.Date = transaction.Date;
            entity.Description = transaction.Description;
            entity.Wallet = await this.GetWalletById(transaction.WalletId);
            //check if budget was set in transaction
            if (transaction.BudgetId == null)
            {
                //remove transaction from Budget if it exists
                entity.Budget?.Transactions.Remove(entity);
                entity.Budget = null;
            }
            else
            {
                entity.Budget = await this.GetBudgetById(transaction.BudgetId.Value);
            }
            entity.Currency =
                await
                    this.GetCurrencyById(transaction.CurrencyId);
            entity.Category =
                await
                    this.GetCategoryById(transaction.CategoryId);
            return entity;
        }

        private void FillRepeatableTransaction(TransactionServiceModel transaction, RepeatableTransaction entity)
        {
            entity.NextRepeat = transaction.NextRepeat.GetValueOrDefault();
            entity.FrequencyType = transaction.FrequencyType;
            entity.LastOccurrence = transaction.LastOccurrence.GetValueOrDefault();
        }

        #endregion
    }
}