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
    public class WalletContext : DbContext, IWalletContext, IWalletsProvider
    {
        public WalletContext() 
            : base("DefaultConnection")
        {
        }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletAccessRight> WalletAccessRights { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Currency> Currencies { get; set; }

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
                return WalletAccessRights;
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

            return new DeletedEntity<WalletAccessRight>(deletedWalletAccessRight);
        }
    }
}
