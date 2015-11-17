using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.BusinessLogic
{
    public class TransactionService
    {
        private readonly ITransactionsProvider _transactionsProvider;

        public TransactionService()
        {
            this._transactionsProvider = ProvidersFactory.GetNewTransactionsProviders();
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

        /// <summary>
        ///     Returns guid of User's default Wallet
        /// </summary>
        /// <param name="userId">guid of user</param>
        /// <returns></returns>
        public async Task<Guid> GetDefaultWallet(Guid userId)
        {
            var currentUserWallet = await this._transactionsProvider.Wallets.Where(w => w.Owner.Guid == userId)
                .Select(w => w.Guid)
                .FirstOrDefaultAsync();
            return currentUserWallet;
        }

        public async Task<Currency> GetDefaultCurrencyInWallet(Guid walletId)
        {
            return
                await
                    this._transactionsProvider.Wallets.Where(wallet => wallet.Guid == walletId)
                        .Select(w => w.Currency)
                        .FirstOrDefaultAsync();
        }

        /*
        public async Task<List<Wallet>> GetViewableWallets(Guid userId, Guid walletId)
        {
            return await this._transactionsProvider.WalletAccessRights.Where(
                user => user.Permission >= PermissionEnum.Read && user.UserProfile.Guid == userId).Select(w => w.Wallet)
                .ToListAsync();
        }*/
        /*
        public async Task<SelectList> GetViewableWalletsSelection(Guid userId, Guid walletId)
        {
            var select = await this.GetViewableWallets(userId, walletId);
            var selection =
                select.Select(wallet => new SelectListItem {Value = wallet.Guid.ToString(), Text = wallet.Name});
            return new SelectList(selection, "Value", "Text", walletId);
        }*/
        /*
        public async Task<bool> HasWritePermission(Guid userId, Guid walletId)
        {
            var permission = await this._walletsProvider.WalletAccessRights
                .FirstOrDefaultAsync(
                    r =>
                        r.Wallet.Guid == walletId &&
                        r.UserProfile.Guid == userId);
            return permission != null && permission.Permission != PermissionEnum.Read;
        }*/
        //TODO WalletAccessRights are not yet implemented
    }
}