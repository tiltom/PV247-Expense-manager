using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using ExpenseManager.BusinessLogic.ExchangeRates;
using ExpenseManager.BusinessLogic.ServicesConstants;
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
    /// <summary>
    ///     Class that handles logic of TransactionController
    /// </summary>
    public class TransactionService : ServiceWithWallet
    {
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

        /// <summary>
        ///     Validates transaction service model
        /// </summary>
        /// <param name="transaction">Transaction to be validated</param>
        public void Validate(TransactionServiceModel transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            this._validator.ValidateAndThrowCustomException(transaction);
        }

        /// <summary>
        ///     Removes transactions without specified category and budgets, if they are not null, from list
        /// </summary>
        /// <param name="categoryId">Id of category</param>
        /// <param name="budgetId">Id of budget</param>
        /// <param name="list">List to be updated</param>
        /// <returns></returns>
        public static IEnumerable<TransactionShowServiceModel> FilterTransactions(Guid? categoryId, Guid? budgetId,
            IEnumerable<TransactionShowServiceModel> list)
        {
            var filteredList = list;
            if (categoryId != null)
            {
                filteredList = filteredList.Where(model => model.CategoryId == categoryId.Value);
            }
            if (budgetId != null)
            {
                filteredList = filteredList.Where(model => model.BudgetId == budgetId.Value);
            }
            return filteredList;
        }

        /// <summary>
        ///     Creates new transaction
        /// </summary>
        /// <param name="transaction">New transaction</param>
        /// <returns></returns>
        public async Task Create(TransactionServiceModel transaction)
        {
            if (transaction.LastOccurrence == null)
            {
                transaction.LastOccurrence = DateTime.MaxValue;
            }
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

        /// <summary>
        ///     Adds new transaction if it's next repetition occurred
        /// </summary>
        /// <returns>HTTP status code OK if no error occurred</returns>
        public async Task<HttpStatusCodeResult> UpdateRepeatableTransactions()
        {
            var repeatableTransactions = await this._transactionsProvider.RepeatableTransactions.ToListAsync();
            foreach (var repeatableTransaction in repeatableTransactions)
            {
                var expectedNewOccurance = this.nextOccurance(repeatableTransaction);
                while (expectedNewOccurance.Subtract(DateTime.Today).Days <= 0)
                {
                    var transactionToAdd = new Transaction
                    {
                        Amount = repeatableTransaction.FirstTransaction.Amount,
                        Budget = repeatableTransaction.FirstTransaction.Budget,
                        Category = repeatableTransaction.FirstTransaction.Category,
                        Currency = repeatableTransaction.FirstTransaction.Currency,
                        Date = expectedNewOccurance,
                        Description = repeatableTransaction.FirstTransaction.Description,
                        Wallet = repeatableTransaction.FirstTransaction.Wallet
                    };
                    await this._transactionsProvider.AddOrUpdateAsync(transactionToAdd);

                    repeatableTransaction.LastOccurrence = transactionToAdd.Date;
                    await this._transactionsProvider.AddOrUpdateAsync(repeatableTransaction);

                    expectedNewOccurance = this.nextOccurance(repeatableTransaction);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        /// <summary>
        ///     Edit transaction
        /// </summary>
        /// <param name="transaction">Edited transaction</param>
        /// <returns></returns>
        public async Task Edit(TransactionServiceModel transaction)
        {
            this.Validate(transaction);
            //find transaction by Id
            var transactionEntity = await this.GetTransactionById(transaction.Id);
            //update entity properties from DTO
            await this.AddOrUpdate(await this.FillTransaction(transaction, transactionEntity));

            //find if transaction is repeatable in DB
            var repeatableTransaction =
                await this.GetRepeatableTransactionByFirstTransactionId(transaction.Id);
            //check if transaction was set as repeatable in model
            if (transaction.IsRepeatable)
            {
                await this.EditRepeatableTransaction(transaction, repeatableTransaction, transactionEntity);
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

        /// <summary>
        ///     Remove transaction
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="transactionId">Id of transaction</param>
        /// <returns>Id of wallet which contained transaction</returns>
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

        /// <summary>
        ///     Export of transaction
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="wallet">Wallet from which transactions should be exported</param>
        /// <param name="category">Category from which transactions should be exported</param>
        /// <param name="budget">Budget from which transactions should be exported</param>
        /// <returns>Exported transactions</returns>
        public async Task<string> ExportToCsv(Guid userId, Guid? wallet, Guid? category, Guid? budget)
        {
            if (wallet == null)
            {
                wallet = await this.GetWalletIdByUserId(userId);
            }
            var walletId = wallet.Value;
            IEnumerable<TransactionShowServiceModel> list =
                await this.GetAllTransactionsInWalletWithCurrency(userId, walletId);
            list = FilterTransactions(category, budget, list);
            TextWriter textWriter = new StringWriter();
            var writer = new CsvWriter(textWriter);
            writer.Configuration.RegisterClassMap<TransactionExportMap>();
            writer.Configuration.CultureInfo = CultureInfo.GetCultureInfo("sk-SK");
            var options = new TypeConverterOptions
            {
                Format = TransactionConstant.DateFormat
            };
            TypeConverterOptionsFactory.AddOptions<DateTime>(options);
            writer.WriteRecords(list);
            return textWriter.ToString();
        }

        /// <summary>
        ///     Import of transaction
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="file">File from which transactions will be imported</param>
        /// <returns></returns>
        public async Task ImportFromCsv(Guid userId, string file)
        {
            var reader = new CsvReader(new StringReader(file));
            reader.Configuration.RegisterClassMap<TransactionExportMap>();
            reader.Configuration.CultureInfo = CultureInfo.GetCultureInfo("sk-SK");
            reader.Configuration.HasHeaderRecord = true;
            while (reader.Read())
            {
                var categoryName = reader.GetField<string>(TransactionResource.CategoryName);
                var budgetName = reader.GetField<string>(TransactionResource.BudgetName);
                var currencyCode = reader.GetField<string>(SharedResource.Currency);
                var model = new TransactionServiceModel
                {
                    Amount = reader.GetField<decimal>(SharedResource.Amount),
                    Date =
                        DateTime.ParseExact(reader.GetField<string>(SharedResource.Date), TransactionConstant.DateFormat,
                            CultureInfo.GetCultureInfo("sk-SK")),
                    Description = reader.GetField<string>(SharedResource.Description),
                    WalletId = await this.GetWalletIdByUserId(userId),
                    CurrencyId = (await this.GetCurrencyByCode(currencyCode)).Guid,
                    CategoryId = (await this.GetCategoryByName(categoryName)).Guid
                };

                if (budgetName != string.Empty)
                {
                    model.BudgetId = (await this.GetBudgetByName(budgetName)).Guid;
                }

                if (model.Amount < 0)
                {
                    model.Expense = true;
                    model.Amount = Math.Abs(model.Amount);
                }
                await this.Create(model);
            }
        }

        /// <summary>
        ///     Returns transaction by it's Id
        /// </summary>
        /// <param name="transactionId">Id of transaction</param>
        /// <param name="userId">Id of user</param>
        /// <returns>Desired transaction</returns>
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

        /// <summary>
        ///     Returns repeatable transaction by first transaction Id
        /// </summary>
        /// <param name="transactionId">Id of transaction</param>
        /// <returns>Desired repeatable transaction</returns>
        public async Task<RepeatableTransaction> GetRepeatableTransactionByFirstTransactionId(Guid transactionId)
        {
            return await
                this._transactionsProvider.RepeatableTransactions.FirstOrDefaultAsync(
                    repeatableTransaction => repeatableTransaction.FirstTransaction.Guid == transactionId);
        }

        /// <summary>
        ///     Returns category by it's Id
        /// </summary>
        /// <param name="categoryId">Id of category</param>
        /// <returns>Desired category</returns>
        public async Task<Category> GetCategoryById(Guid categoryId)
        {
            return
                await
                    this._transactionsProvider.Categories.Where(category => category.Guid == categoryId)
                        .FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Returns category by it's name
        /// </summary>
        /// <param name="categoryName">Name of Category</param>
        /// <returns>Desired category</returns>
        public async Task<Category> GetCategoryByName(string categoryName)
        {
            return
                await
                    this._transactionsProvider.Categories.Where(category => category.Name == categoryName)
                        .FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Returns currency by it's Id
        /// </summary>
        /// <param name="currencyId">Id of currency</param>
        /// <returns>Desired currency</returns>
        public async Task<Currency> GetCurrencyById(Guid currencyId)
        {
            return
                await
                    this._transactionsProvider.Currencies.Where(currency => currency.Guid == currencyId)
                        .FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Returns currency by it's code
        /// </summary>
        /// <param name="currencyCode">Code of currency</param>
        /// <returns>Desired currency</returns>
        private async Task<Currency> GetCurrencyByCode(string currencyCode)
        {
            return
                await
                    this._transactionsProvider.Currencies.Where(currency => currency.Code == currencyCode)
                        .FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Returns budget by it's Id
        /// </summary>
        /// <param name="budgetId">Id of budget</param>
        /// <returns>Desired budget</returns>
        public async Task<Budget> GetBudgetById(Guid budgetId)
        {
            return
                await this._transactionsProvider.Budgets.Where(budget => budget.Guid == budgetId).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Returns budget by it's name
        /// </summary>
        /// <param name="budgetName">Name of budget</param>
        /// <returns>Desired budget</returns>
        public async Task<Budget> GetBudgetByName(string budgetName)
        {
            return
                await
                    this._transactionsProvider.Budgets.Where(budget => budget.Name == budgetName).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Returns all transactions in wallet
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="walletId">Id of wallet</param>
        /// <returns>List of all transactions in wallet</returns>
        public async Task<List<TransactionShowServiceModel>> GetAllTransactionsInWallet(Guid userId, Guid walletId)
        {
            var modelList = await this.GetAllTransactionsInWalletLoader(userId, walletId);
            return new List<TransactionShowServiceModel>(modelList);
        }

        /// <summary>
        ///     Returns all transactions with currency code in wallet
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="walletId">Id of wallet</param>
        /// <returns>List of all transactions in wallet</returns>
        public async Task<List<TransactionServiceExportModel>> GetAllTransactionsInWalletWithCurrency(Guid userId,
            Guid walletId)
        {
            return await this.GetAllTransactionsInWalletLoader(userId, walletId);
        }

        /// <summary>
        ///     Returns default currency in wallet
        /// </summary>
        /// <param name="walletId">Id of wallet</param>
        /// <returns>Desired currency</returns>
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
        ///     Provides selectable list of Currencies which are available for specified parameter
        /// </summary>
        /// <param name="expense">Bool if categories should be for expense or income</param>
        /// <returns>List of SelectListItem for Currencies</returns>
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

        /// <summary>
        ///     Provides selectable list of all categories with selected category
        /// </summary>
        /// <param name="walletId">Id of wallet</param>
        /// <param name="categoryId">Id of category to be selected</param>
        /// <returns>List of SelectListItem for Categories</returns>
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
            return new SelectList(selectList, TransactionConstant.DefaultValue, TransactionConstant.DefaultText,
                categoryId);
        }

        /// <summary>
        ///     Returns user permission for wallet
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="walletId">Id of wallet</param>
        /// <returns>Access right to wallet</returns>
        public async Task<WalletAccessRight> GetPermission(Guid userId, Guid walletId)
        {
            return await
                this._walletsProvider.WalletAccessRights
                    .FirstOrDefaultAsync(
                        right =>
                            right.UserProfile.Guid == userId && right.Wallet.Guid == walletId);
        }

        /// <summary>
        ///     Provides selectable list of wallets available to user
        /// </summary>
        /// <param name="userId">Id of user</param>
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

        /// <summary>
        ///     Provides selectable list of all wallets with selected wallet
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="walletId">Id of wallet</param>
        /// <returns>List of SelectListItem for Wallet</returns>
        public async Task<SelectList> GetViewableWalletsSelection(Guid userId, Guid walletId)
        {
            return new SelectList(await this.GetAllReadableWalletsSelection(userId), TransactionConstant.DefaultValue,
                TransactionConstant.DefaultText, walletId);
        }

        /// <summary>
        ///     Provides selectable list of Budgets where can user write transactions
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <returns>List of SelectListItem for Budgets</returns>
        public async Task<List<SelectListItem>> GetBudgetsSelection(Guid userId)
        {
            return await this.GetBudgetsSelection(userId, PermissionEnum.Write);
        }


        /// <summary>
        ///     Provides selectable list of Budgets which can user read
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <returns>List of SelectListItem for Budgets</returns>
        public async Task<List<SelectListItem>> GetReadableBudgetsSelection(Guid userId)
        {
            return await this.GetBudgetsSelection(userId, PermissionEnum.Read);
        }

        /// <summary>
        ///     Provides selectable list of all budgets with selected budget
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="walletId">Id of wallet</param>
        /// <param name="budgetId">Id of budget to be selected</param>
        /// <returns>List of SelectListItem for Budgets</returns>
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
            return new SelectList(selectList, TransactionConstant.DefaultValue, TransactionConstant.DefaultText,
                budgetId);
        }

        /// <summary>
        ///     Loads all transactions in wallet for database
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="walletId">Id of wallet</param>
        /// <returns>List of all transactions in wallet</returns>
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

        public async Task<Dictionary<Guid, string>> GetCategoryIconDictionary()
        {
            return
                await
                    this._transactionsProvider.Categories.ToDictionaryAsync(category => category.Guid,
                        category => category.IconPath);
        }

        /// <summary>
        ///     Map for transaction exportation
        /// </summary>
        public sealed class TransactionExportMap : CsvClassMap<TransactionServiceExportModel>
        {
            public TransactionExportMap()
            {
                this.Map(model => model.Amount)
                    .Name(SharedResource.Amount)
                    .TypeConverterOption(CultureInfo.GetCultureInfo("sk-SK"));
                this.Map(model => model.CurrencyCode).Name(SharedResource.Currency);
                this.Map(model => model.Date).Name(SharedResource.Date);
                this.Map(model => model.Description).Name(SharedResource.Description);
                this.Map(model => model.CategoryName).Name(TransactionResource.CategoryName);
                this.Map(model => model.BudgetName).Name(TransactionResource.BudgetName);
            }
        }

        #region private

        /// <summary>
        ///     Returns category type
        /// </summary>
        /// <param name="expense">Bool if transaction is expense</param>
        /// <returns>Desired Category type</returns>
        private CategoryType GetCategoryType(bool expense)
        {
            return expense ? CategoryType.Expense : CategoryType.Income;
        }

        /// <summary>
        ///     Stores transaction in database and updates converts currency if needed
        /// </summary>
        /// <param name="transaction">Transaction to save</param>
        /// <returns></returns>
        private async Task AddOrUpdate(Transaction transaction)
        {
            this._transactionsProvider.AttachTransaction(transaction);
            var walletCurrency = await this.GetDefaultCurrencyInWallet(transaction.Wallet.Guid);
            var recomputedTransaction = transaction;
            if (transaction.Currency.Name != walletCurrency.Name)
            {
                recomputedTransaction = Transformation.ChangeCurrency(transaction, walletCurrency);
            }

            await this._transactionsProvider.AddOrUpdateAsync(recomputedTransaction);
        }

        /// <summary>
        ///     Transforms collection of categories to select list
        /// </summary>
        /// <param name="result">Categories to transform</param>
        /// <returns>List of SelectListItem for Budgets</returns>
        private static async Task<List<SelectListItem>> ReturnSelectionForCategory(IQueryable<Category> result)
        {
            return await result.Select(
                category => new SelectListItem {Value = category.Guid.ToString(), Text = category.Name})
                .ToListAsync();
        }

        /// <summary>
        ///     Provides selectable list of Budgets where has at least specified permission
        /// </summary>
        /// <param name="userId">Id of permission</param>
        /// <param name="minimalPermission">Minimal permission to have</param>
        /// <returns>List of SelectListItem for Budgets</returns>
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

        /// <summary>
        ///     Returns transaction by it's Id
        /// </summary>
        /// <param name="transactionId">Id of transaction</param>
        /// <returns>Desired transaction</returns>
        private async Task<Transaction> GetTransactionById(Guid transactionId)
        {
            return
                await
                    this._transactionsProvider.Transactions.Where(transaction => transaction.Guid == transactionId)
                        .FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Returns all transactions in wallet
        /// </summary>
        /// <param name="walletId">Id of wallet</param>
        /// <returns>List of all transactions</returns>
        private async Task<List<Transaction>> GetAllTransactionsInWallet(Guid walletId)
        {
            return
                await
                    this._transactionsProvider.Transactions.Where(transaction => transaction.Wallet.Guid == walletId)
                        .ToListAsync();
        }

        /// <summary>
        ///     Returns if user has permission to write for wallet
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="walletId">Id of wallet</param>
        /// <returns>True if user can write to wallet or false if not</returns>
        private async Task<bool> HasWritePermission(Guid userId, Guid walletId)
        {
            var permission = await this._walletsProvider.WalletAccessRights
                .FirstOrDefaultAsync(
                    right =>
                        right.Wallet.Guid == walletId &&
                        right.UserProfile.Guid == userId);
            return permission != null && permission.Permission != PermissionEnum.Read;
        }

        /// <summary>
        ///     Fills transaction entity from service model
        /// </summary>
        /// <param name="transaction">Transaction service model</param>
        /// <param name="entity">Transaction to be filled</param>
        /// <returns>Filled transaction</returns>
        private async Task<Transaction> FillTransaction(TransactionServiceModel transaction, Transaction entity)
        {
            //setting properties from transaction
            entity.Amount = transaction.Amount;
            if (transaction.Expense)
            {
                entity.Amount = -entity.Amount;
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

        /// <summary>
        ///     Fills repeatable transaction from service model
        /// </summary>
        /// <param name="transaction">Transaction service model</param>
        /// <param name="entity">Repeatable transaction to be filled</param>
        private void FillRepeatableTransaction(TransactionServiceModel transaction, RepeatableTransaction entity)
        {
            entity.NextRepeat = transaction.NextRepeat.GetValueOrDefault();
            entity.FrequencyType = transaction.FrequencyType;
            entity.LastOccurrence = transaction.LastOccurrence.GetValueOrDefault();
        }

        /// <summary>
        ///     Updates transaction in database according to transaction service model
        /// </summary>
        /// <param name="transaction">Transaction service model</param>
        /// <param name="repeatableTransaction">Repeatable transaction to be edited</param>
        /// <param name="firstTransaction">First transaction of repeatable transaction</param>
        /// <returns></returns>
        private async Task EditRepeatableTransaction(TransactionServiceModel transaction,
            RepeatableTransaction repeatableTransaction, Transaction firstTransaction)
        {
            //check if transaction was also set repeatable in model
            if (repeatableTransaction == null)
            {
                //if not create new repeatable transaction entity
                repeatableTransaction = new RepeatableTransaction
                {
                    Guid = new Guid(),
                    FirstTransaction = firstTransaction
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

        private DateTime nextOccurance(RepeatableTransaction repeatableTransaction)
        {
            DateTime expectedNewOccurance;
            switch (repeatableTransaction.FrequencyType)
            {
                case FrequencyType.Day:
                    expectedNewOccurance = repeatableTransaction.LastOccurrence.AddDays(repeatableTransaction.NextRepeat);
                    break;
                case FrequencyType.Week:
                    expectedNewOccurance =
                        repeatableTransaction.LastOccurrence.AddDays(repeatableTransaction.NextRepeat*7);
                    break;
                case FrequencyType.Month:
                    expectedNewOccurance =
                        repeatableTransaction.LastOccurrence.AddMonths(repeatableTransaction.NextRepeat);
                    break;
                case FrequencyType.Year:
                    expectedNewOccurance =
                        repeatableTransaction.LastOccurrence.AddYears(repeatableTransaction.NextRepeat);
                    break;
                default:
                    throw new ArgumentException("uknown day type");
            }
            return expectedNewOccurance;
        }

        #endregion
    }
}