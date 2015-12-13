using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManager.BusinessLogic.Validators;
using ExpenseManager.BusinessLogic.Validators.Extensions;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.Queryable;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.BusinessLogic.WalletServices
{
    /// <summary>
    ///     Class that handles logic of WalletAccessRightController
    /// </summary>
    public class WalletAccessRightService : ServiceWithWallet
    {
        private readonly CommonService _commonService;
        private readonly WalletAccessRightValidator _validator;
        private readonly IWalletsProvider _wallets;

        public WalletAccessRightService(IWalletsProvider db, CommonService commonService)
        {
            this._wallets = db;
            this._commonService = commonService;
            this._validator = new WalletAccessRightValidator();
        }

        #region protected

        protected override IWalletsQueryable WalletsProvider
        {
            get { return this._wallets; }
        }

        #endregion

        /// <summary>
        ///     Validates wallet access rights
        /// </summary>
        /// <param name="walletAccessRight">Wallet access right to validate</param>
        /// <returns>True if wallet access right is valid, false otherwise</returns>
        public void Validate(WalletAccessRight walletAccessRight)
        {
            if (walletAccessRight == null)
                throw new ArgumentNullException(nameof(walletAccessRight));

            this._validator.ValidateAndThrowCustomException(walletAccessRight);
        }

        /// <summary>
        ///     Returns access rights for walled by wallet owner
        /// </summary>
        /// <param name="id">ID of wallet owner</param>
        /// <returns>Access rights for wallet</returns>
        public async Task<IEnumerable<WalletAccessRight>> GetAccessRightsByWalletOwnerId(Guid id)
        {
            var result = await this._wallets.Wallets.Where(wallet => wallet.WalletAccessRights.Any(
                right => right.Permission == PermissionEnum.Owner
                         && right.UserProfile.Guid == id
                )).FirstOrDefaultAsync();
            return result.WalletAccessRights;
        }

        /// <summary>
        ///     Returns wallet access right ID by wallet ID
        /// </summary>
        /// <param name="id">ID of wallet</param>
        /// <returns>Wallet access right ID</returns>
        public async Task<Guid> GetWalletAccessRightIdByWalletId(Guid id)
        {
            return await this._wallets.Wallets
                .Where(
                    wallet =>
                        wallet.WalletAccessRights.Any(
                            right => right.UserProfile.Guid == id && right.Permission == PermissionEnum.Owner))
                .Select(wallet => wallet.Guid)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Returns wallet access right by ID
        /// </summary>
        /// <param name="id">ID of wallet access right</param>
        /// <returns>Desired wallet access right</returns>
        public async Task<WalletAccessRight> GetWalletAccessRightById(Guid id)
        {
            return await this._wallets.WalletAccessRights.Where(right => right.Guid.Equals(id)).FirstOrDefaultAsync();
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

            this.Validate(walletAccessRight);

            await this._wallets.AddOrUpdateAsync(walletAccessRight);
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

            this.Validate(walletAccessRight);

            await this._wallets.AddOrUpdateAsync(walletAccessRight);
        }

        /// <summary>
        ///     Deletes wallet access right
        /// </summary>
        /// <param name="id">ID of deleted wallet access right</param>
        /// <returns></returns>
        public async Task DeleteWalletAccessRight(Guid id)
        {
            var walletAccessRight = await this.GetWalletAccessRightById(id);

            await this._wallets.DeteleAsync(walletAccessRight);
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
        ///     Returns user profile by it's ID
        /// </summary>
        /// <param name="id">User profile ID</param>
        /// <returns>Desired user profile</returns>
        public async Task<UserProfile> GetUserProfileById(Guid id)
        {
            return await this._wallets.UserProfiles.Where(profile => profile.Guid.Equals(id)).FirstOrDefaultAsync();
        }
    }
}