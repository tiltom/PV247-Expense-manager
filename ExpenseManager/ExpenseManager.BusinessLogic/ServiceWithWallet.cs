using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Providers.Queryable;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.BusinessLogic
{
    public abstract class ServiceWithWallet
    {
        protected abstract IWalletsQueryable WalletsProvider { get; }


        /// <summary>
        ///     Returns a wallet by specified ID
        /// </summary>
        /// <param name="id">ID of returned wallet</param>
        /// <returns>Desired wallet</returns>
        public async Task<Wallet> GetWalletById(Guid id)
        {
            return await WalletsProvider.Wallets.Where(w => w.Guid.Equals(id)).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Returns a wallet by specified userId
        /// </summary>
        /// <param name="userId">ID of user profile</param>
        /// <returns>Desired wallet</returns>
        public async Task<Wallet> GetWalletByUserId(Guid userId)
        {
            return
                await
                    WalletsProvider.Wallets.Where(
                        w =>
                            w.WalletAccessRights.Any(
                                right =>
                                    right.Permission == PermissionEnum.Owner
                                    && right.UserProfile.Guid == userId)
                        )
                        .FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Returns a wallet id for specified userId
        /// </summary>
        /// <param name="userId">ID of user profile</param>
        /// <returns>Desired wallet</returns>
        public async Task<Guid> GetWalletIdByUserId(Guid userId)
        {
            var result = await this.GetWalletByUserId(userId);
            return result.Guid;
        }
    }
}