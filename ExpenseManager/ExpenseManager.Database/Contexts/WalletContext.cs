using System.Data.Entity;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.infrastructure;
using ExpenseManager.Entity.Transactions;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;

namespace ExpenseManager.Database.Contexts
{
    internal class WalletContext : CurrencyContext, IWalletContext, IWalletsProvider
    {
        public WalletContext() 
            : base("DefaultConnection")
        {
        }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletAccessRight> WalletAccessRights { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        IQueryable<Wallet> IWalletsProvider.Wallets
        {
            get
            {
                return Wallets;
            }
        }

        IQueryable<WalletAccessRight> IWalletAccessRightsProvider.WalletAccessRights
        {
            get
            {
                return WalletAccessRights
                    .Include(war => war.Wallet)
                    .Include(war => war.Permission)
                    .Include(war => war.UserProfile);
            }
        }

        IQueryable<UserProfile> IUserProfilesProvider.UserProfiles
        {
            get
            {
                return UserProfiles;
            }
        }

        public async Task<bool> AddOrUpdateAsync(Wallet entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existingWallet = entity.Guid == Guid.Empty
                ? null
                : await Wallets.FindAsync(entity.Guid);

            Wallets.AddOrUpdate(x => x.Guid, entity);

            await SaveChangesAsync();
            return existingWallet == null;
        }

        public async Task<DeletedEntity<Wallet>> DeteleAsync(Wallet entity)
        {
            var walletToDelete = entity.Guid == Guid.Empty
                ? null
                : await Wallets.FindAsync(entity.Guid);

            walletToDelete.WalletAccessRights.Clear();
            var deletedWallet = walletToDelete == null
                ? null
                : Wallets.Remove(walletToDelete);

            await SaveChangesAsync();
            return new DeletedEntity<Wallet>(deletedWallet);
        }

        public async Task<bool> AddOrUpdateAsync(WalletAccessRight entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existingWalleAccessRightt = entity.Guid == Guid.Empty
                ? null
                : await WalletAccessRights.FindAsync(entity.Guid);

            WalletAccessRights.AddOrUpdate(x => x.Guid, entity);

            await SaveChangesAsync();
            return existingWalleAccessRightt == null;
        }

        public async Task<DeletedEntity<WalletAccessRight>> DeteleAsync(WalletAccessRight entity)
        {
            var walletAccessRightToDelete = entity.Guid == Guid.Empty
                ? null
                : await WalletAccessRights.FindAsync(entity.Guid);
            var deletedWalletAccessRight = walletAccessRightToDelete == null
                ? null
                : WalletAccessRights.Remove(walletAccessRightToDelete);

            await SaveChangesAsync();
            return new DeletedEntity<WalletAccessRight>(deletedWalletAccessRight);
        }

        public async Task<bool> AddOrUpdateAsync(UserProfile entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existingUserProfile = entity.Guid == Guid.Empty
                ? null
                : await UserProfiles.FindAsync(entity.Guid);

            UserProfiles.AddOrUpdate(x => x.Guid, entity);

            await SaveChangesAsync();
            return existingUserProfile == null;
        }

        public async Task<DeletedEntity<UserProfile>> DeteleAsync(UserProfile entity)
        {
            var userProfileToDelete = entity.Guid == Guid.Empty
                ? null
                : await UserProfiles.FindAsync(entity.Guid);
            var deletedUserProfile = userProfileToDelete == null
                ? null
                : UserProfiles.Remove(userProfileToDelete);

            await SaveChangesAsync();
            return new DeletedEntity<UserProfile>(deletedUserProfile);
        }
    }
}
