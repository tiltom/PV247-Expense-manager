using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.BusinessLogic.WalletServices
{
    /// <summary>
    ///     Class that handles logic of WalletAccessRightController
    /// </summary>
    public class WalletAccessRightService
    {
        private readonly CommonService _commonService;
        private readonly IWalletsProvider _db;

        public WalletAccessRightService(IWalletsProvider db, CommonService commonService)
        {
            this._db = db;
            this._commonService = commonService;
        }

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
        /// <param name="walletId">id of wallet</param>
        /// <param name="userId">id of user</param>
        /// <param name="permission">string of permission</param>
        /// <returns></returns>
        public async Task CreateWalletAccessRight(Guid walletId, Guid userId, string permission)
        {
            var walletAccessRight = new WalletAccessRight
            {
                Guid = Guid.NewGuid(),
                Wallet = await this.GetWalletById(walletId),
                UserProfile = await this.GetUserProfileById(userId),
                Permission = this._commonService.ConvertPermissionStringToEnum(permission)
            };

            this.ValidateWalletAccessRight(walletAccessRight);

            await this._db.AddOrUpdateAsync(walletAccessRight);
        }


        /// <summary>
        ///     Returns wallet by it's ID
        /// </summary>
        /// <param name="id">Wallet ID</param>
        /// <returns>Desired wallet</returns>
        public async Task<Wallet> GetWalletById(Guid id)
        {
            return await this._db.Wallets.Where(x => x.Guid.Equals(id)).FirstOrDefaultAsync();
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


        /// <summary>
        ///     Validates wallet access rights
        /// </summary>
        /// <param name="walletAccessRight">Wallet access right to validate</param>
        /// <returns>True if wallet access right is valid, false otherwise</returns>
        public bool ValidateWalletAccessRight(WalletAccessRight walletAccessRight)
        {
            if (walletAccessRight == null)
            {
                return false;
            }

            if (walletAccessRight.UserProfile == null)
            {
                return false;
            }

            if (walletAccessRight.Wallet == null)
            {
                return false;
            }

            return true;
        }

        #region private

        /// <summary>
        ///     Returns user profile by it's ID
        /// </summary>
        /// <param name="id">User profile ID</param>
        /// <returns>Desired user profile</returns>
        public async Task<UserProfile> GetUserProfileById(Guid id)
        {
            return await this._db.UserProfiles.Where(x => x.Guid.Equals(id)).FirstOrDefaultAsync();
        }

        #endregion
    }
}