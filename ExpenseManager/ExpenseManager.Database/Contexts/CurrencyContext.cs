using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Providers;
using ExpenseManager.Entity.Providers.infrastructure;

namespace ExpenseManager.Database.Contexts
{
    internal class CurrencyContext : DbContext, ICurrenciesProvider
    {
        public CurrencyContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public DbSet<Currency> Currencies { get; set; }

        IQueryable<Currency> ICurrenciesProvider.Currencies
        {
            get { return Currencies; }
        }

        public async Task<bool> AddOrUpdateAsync(Currency entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existingCurrency = entity.Guid == Guid.Empty
                ? null
                : await Currencies.FindAsync(entity.Guid);

            Currencies.AddOrUpdate(x => x.Guid, entity);

            await this.SaveChangesAsync();
            return existingCurrency == null;
        }

        public async Task<DeletedEntity<Currency>> DeteleAsync(Currency entity)
        {
            var currencyToDelete = entity.Guid == Guid.Empty
                ? null
                : await Currencies.FindAsync(entity.Guid);
            var deletedCurrency = currencyToDelete == null
                ? null
                : Currencies.Remove(currencyToDelete);

            await this.SaveChangesAsync();
            return new DeletedEntity<Currency>(deletedCurrency);
        }
    }
}