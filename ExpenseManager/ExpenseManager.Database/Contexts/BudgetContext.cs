using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.infrastructure;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.Database.Contexts
{
    internal class BudgetContext : CurrencyContext, IBudgetContext, IBudgetsProvider
    {
        public BudgetContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<BudgetAccessRight> BudgetAccessRights { get; set; }

        public DbSet<Budget> Budgets { get; set; }


        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        IQueryable<Budget> IBudgetsProvider.Budgets
        {
            get { return Budgets; }
        }

        IQueryable<BudgetAccessRight> IBudgetAccessRightsProvider.BudgetAccessRights
        {
            get { return BudgetAccessRights.Include(w => w.UserProfile).Include(w => w.Budget); }
        }

        IQueryable<UserProfile> IUserProfilesProvider.UserProfiles
        {
            get { return UserProfiles; }
        }

        public async Task<bool> AddOrUpdateAsync(Budget entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existingBudget = entity.Guid == Guid.Empty
                ? null
                : await Budgets.FindAsync(entity.Guid);

            Budgets.AddOrUpdate(x => x.Guid, entity);
            await this.SaveChangesAsync();
            return existingBudget == null;
        }

        public async Task<DeletedEntity<Budget>> DeteleAsync(Budget entity)
        {
            var budgetToDelete = entity.Guid == Guid.Empty
                ? null
                : await Budgets.FindAsync(entity.Guid);

            BudgetAccessRights.RemoveRange(budgetToDelete.AccessRights);

            var deletedBudget = budgetToDelete == null
                ? null
                : Budgets.Remove(budgetToDelete);

            await this.SaveChangesAsync();
            return new DeletedEntity<Budget>(deletedBudget);
        }

        public async Task<bool> AddOrUpdateAsync(BudgetAccessRight entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existingBudgetAccessRight = entity.Guid == Guid.Empty
                ? null
                : await BudgetAccessRights.FindAsync(entity.Guid);

            BudgetAccessRights.AddOrUpdate(x => x.Guid, entity);

            await this.SaveChangesAsync();
            return existingBudgetAccessRight == null;
        }

        public async Task<DeletedEntity<BudgetAccessRight>> DeteleAsync(BudgetAccessRight entity)
        {
            var budgetAccessRightToDelete = entity.Guid == Guid.Empty
                ? null
                : await BudgetAccessRights.FindAsync(entity.Guid);

            var deletedBudgetAccessRight = budgetAccessRightToDelete == null
                ? null
                : BudgetAccessRights.Remove(budgetAccessRightToDelete);

            await this.SaveChangesAsync();
            return new DeletedEntity<BudgetAccessRight>(deletedBudgetAccessRight);
        }

        public async Task<bool> AddOrUpdateAsync(UserProfile entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existinguserProfile = entity.Guid == Guid.Empty
                ? null
                : await UserProfiles.FindAsync(entity.Guid);

            UserProfiles.AddOrUpdate(x => x.Guid, entity);

            await this.SaveChangesAsync();
            return existinguserProfile == null;
        }

        public async Task<DeletedEntity<UserProfile>> DeteleAsync(UserProfile entity)
        {
            var userProfileToDelete = entity.Guid == Guid.Empty
                ? null
                : await UserProfiles.FindAsync(entity.Guid);

            var deletedUserProfile = userProfileToDelete == null
                ? null
                : UserProfiles.Remove(userProfileToDelete);

            await this.SaveChangesAsync();
            return new DeletedEntity<UserProfile>(deletedUserProfile);
        }

        public Task<bool> AddOrUpdateAsync(Category entity)
        {
            throw new NotImplementedException();
        }

        public Task<DeletedEntity<Category>> DeteleAsync(Category entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddOrUpdateAsync(Wallet entity)
        {
            throw new NotImplementedException();
        }

        public Task<DeletedEntity<Wallet>> DeteleAsync(Wallet entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddOrUpdateAsync(WalletAccessRight entity)
        {
            throw new NotImplementedException();
        }

        public Task<DeletedEntity<WalletAccessRight>> DeteleAsync(WalletAccessRight entity)
        {
            throw new NotImplementedException();
        }
    }
}