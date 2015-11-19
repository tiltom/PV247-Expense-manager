using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Transactions;

namespace ExpenseManager.BusinessLogic.DashboardServices
{
    public class DashBoardService
    {
        private readonly ITransactionsProvider _transactionsProvider;

        public DashBoardService(ITransactionsProvider transactionsProvider)
        {
            this._transactionsProvider = transactionsProvider;
        }

        /// <summary>
        ///     Will return just transaction which can be seen by this user
        /// </summary>
        /// <param name="userId"> id of logged user</param>
        /// <returns> queryable for accessible transactions</returns>
        public IQueryable<Transaction> GetAccessibleResults(Guid userId)
        {
            return
                this._transactionsProvider.Transactions
                    .Where(t =>
                        (t.Wallet.WalletAccessRights.Any(war => war.UserProfile.Guid == userId) ||
                         t.Budget.AccessRights.Any(bar => bar.UserProfile.Guid == userId))
                    );
        }

        /// <summary>
        ///     provides list of wallets available for user
        /// </summary>
        /// <param name="userId"> user with access to provided wallets</param>
        /// <returns>select item for front end</returns>
        public async Task<List<SelectListItem>> GetAccessibleWallets(Guid userId)
        {
            return await
                this._transactionsProvider.Wallets.Where(
                    w => w.WalletAccessRights.Any(war => war.UserProfile.Guid == userId)).Select(w => new SelectListItem
                    {
                        Value = w.Guid.ToString(),
                        Text = w.Name
                    }).ToListAsync();
        }
    }
}