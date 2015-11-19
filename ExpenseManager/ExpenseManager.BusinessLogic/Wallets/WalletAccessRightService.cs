using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.BusinessLogic.Wallets
{
    /// <summary>
    ///     Class that handles logic of WalletAccessRightController
    /// </summary>
    public class WalletAccessRightService
    {
        private readonly IWalletsProvider _db = ProvidersFactory.GetNewWalletsProviders();

        /// <summary>
        ///     Returns access rights for walled by wallet owner
        /// </summary>
        /// <param name="id">ID of wallet owner</param>
        /// <returns>Access rights for wallet</returns>
        public IQueryable GetAccessRightsByWalletOwnerId(Guid id)
        {
            return this._db.WalletAccessRights.Where(right => right.Wallet.Owner.Guid.Equals(id));
        }

        /// <summary>
        ///     Returns wallet access right ID by wallet ID
        /// </summary>
        /// <param name="id">ID of wallet</param>
        /// <returns>Wallet access right ID</returns>
        public async Task<Guid> GetWalletAccessRightIdByWalletId(Guid id)
        {
            return await this._db.Wallets
                .Where(w => w.Owner.Guid.Equals(id))
                .Select(w => w.Guid)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Returns wallet access right by ID
        /// </summary>
        /// <param name="id">ID of wallet access right</param>
        /// <returns>Desired wallet access right</returns>
        public async Task<WalletAccessRight> GetWalletAccessRightById(Guid id)
        {
            return await this._db.WalletAccessRights.Where(x => x.Guid.Equals(id)).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Creates new wallet access right
        /// </summary>
        /// <param name="walletAccessRight">Instance of new wallet access right</param>
        /// <returns></returns>
        public async Task CreateWalletAccessRight(WalletAccessRight walletAccessRight)
        {
            this.ValidateWalletAccessRight(walletAccessRight);

            await this._db.AddOrUpdateAsync(walletAccessRight);
        }

        /// <summary>
        ///     Returns wallet by it's ID
        /// </summary>
        /// <param name="id">Wallet ID</param>
        /// <returns>Desired wallet</returns>
        public async Task<Entity.Wallets.Wallet> GetWalletById(Guid id)
        {
            return await this._db.Wallets.Where(x => x.Guid.Equals(id)).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Returns user profile by it's ID
        /// </summary>
        /// <param name="id">User profile ID</param>
        /// <returns>Desired user profile</returns>
        public async Task<UserProfile> GetUserProfileById(Guid id)
        {
            return await this._db.UserProfiles.Where(x => x.Guid.Equals(id)).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Returns user profile by user ID or wallet owner ID
        /// </summary>
        /// <param name="userId">User profile ID</param>
        /// <param name="walletOwnerId">ID of wallet owner</param>
        /// <returns>Desired user profile</returns>
        public IQueryable<UserProfile> GetUserProfileByIds(Guid? userId, Guid walletOwnerId)
        {
            return this._db.UserProfiles
                .Where(
                    u =>
                        u.WalletAccessRights.All(war => war.Wallet.Owner.Guid != walletOwnerId) ||
                        u.Guid == userId);
        }

        /// <summary>
        ///     Edits wallet access right
        /// </summary>
        /// <param name="id">ID of edited wallet access right</param>
        /// <param name="permission">New permission for wallet access right</param>
        /// <param name="userId">ID of new user for wallet access right</param>
        /// <returns></returns>
        public async Task EditWalletAccessRight(Guid id, PermissionEnum permission, Guid userId)
        {
            var walletAccessRight = await this.GetWalletAccessRightById(id);

            walletAccessRight.UserProfile = await this.GetUserProfileById(userId);
            walletAccessRight.Permission = permission;

            this.ValidateWalletAccessRight(walletAccessRight);

            await this._db.AddOrUpdateAsync(walletAccessRight);
        }

        /// <summary>
        ///     Deletes wallet access right
        /// </summary>
        /// <param name="id">ID of deleted wallet access right</param>
        /// <returns></returns>
        public async Task DeleteWalletAccessRight(Guid id)
        {
            var walletAccessRight = await this.GetWalletAccessRightById(id);

            await this._db.DeteleAsync(walletAccessRight);
        }

        /// <summary>
        ///     Returns permission of wallet access right
        /// </summary>
        /// <param name="id">ID of wallet access right</param>
        /// <returns>Desired permission</returns>
        public async Task<PermissionEnum> GetPermissionByWalletAccessRightId(Guid id)
        {
            var walletAccessRight = await this.GetWalletAccessRightById(id);

            return walletAccessRight.Permission;
        }

        #region private

        private void ValidateWalletAccessRight(WalletAccessRight walletAccessRight)
        {
            // TODO: Do some epic validation
        }

        #endregion
    }
}