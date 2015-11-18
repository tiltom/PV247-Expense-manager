﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Enums;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.BusinessLogic
{
    public class TransactionService
    {
        private readonly IBudgetsProvider _budgetsProvider;
        private readonly ITransactionsProvider _transactionsProvider;
        private readonly IWalletsProvider _walletsProvider;

        public TransactionService()
        {
            this._transactionsProvider = ProvidersFactory.GetNewTransactionsProviders();
            this._walletsProvider = ProvidersFactory.GetNewWalletsProviders();
            this._budgetsProvider = ProvidersFactory.GetNewBudgetsProviders();
        }

        public async Task AddOrUpdate(Transaction transaction)
        {
            await this._transactionsProvider.AddOrUpdateAsync(transaction);
        }

        public async Task AddOrUpdate(RepeatableTransaction repeatableTransaction)
        {
            await this._transactionsProvider.AddOrUpdateAsync(repeatableTransaction);
        }

        public async Task Remove(Transaction transaction)
        {
            await this._transactionsProvider.DeteleAsync(transaction);
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
            return
                await
                    this._transactionsProvider.Categories.Where(
                        m => m.Type == transactionType || m.Type == CategoryType.IncomeAndExpense).Select(
                            category => new SelectListItem {Value = category.Guid.ToString(), Text = category.Name})
                        .ToListAsync();
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

        public async Task<WalletAccessRight> GetPermission(Guid userId, Guid walletId)
        {
            return await
                this._walletsProvider.WalletAccessRights // TODO NOT IMPLEMENTED< SHOULDNT BE IMPLEMENTED FIX THIS
                    .FirstOrDefaultAsync(
                        r =>
                            r.UserProfile.Guid == userId && r.Wallet.Guid == walletId);
        }

        public async Task<bool> HasWritePermission(Guid userId, Guid walletId)
        {
            var permission = await this._walletsProvider.WalletAccessRights
                .FirstOrDefaultAsync(
                    r =>
                        r.Wallet.Guid == walletId &&
                        r.UserProfile.Guid == userId);
            return permission != null && permission.Permission != PermissionEnum.Read;
        }

        public async Task<SelectList> GetViewableWalletsSelection(Guid userId, Guid walletId)
        {
            var select = await this._walletsProvider.WalletAccessRights.Where(
                user => user.Permission >= PermissionEnum.Read && user.UserProfile.Guid == userId).Select(w => w.Wallet)
                .ToListAsync();
            var selection =
                select.Select(wallet => new SelectListItem {Value = wallet.Guid.ToString(), Text = wallet.Name});
            return new SelectList(selection, "Value", "Text", walletId);
        }

        /// <summary>
        ///     Provides selectable list of Budgets available to UserProfile
        /// </summary>
        /// <returns>List of SelectListItem for Budgets</returns>
        public async Task<List<SelectListItem>> GetBudgetsSelection(Guid userGuid)
        {
            return
                await
                    this._budgetsProvider.BudgetAccessRights.Where(
                        access =>
                            access.UserProfile.Guid == userGuid &&
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

        private CategoryType GetCategoryType(bool expense)
        {
            return expense ? CategoryType.Expense : CategoryType.Income;
        }
    }
}