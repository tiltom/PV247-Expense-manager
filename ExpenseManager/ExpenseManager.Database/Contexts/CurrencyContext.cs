using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Providers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseManager.Entity.Providers.infrastructure;
using System.Data.Entity.Migrations;

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
            get
            {
                return Currencies;
            }
        }

        public async Task<bool> AddOrUpdateAsync(Currency entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existingCurrency = entity.Guid == Guid.Empty
                ? null
                : await Currencies.FindAsync(entity.Guid);

            Currencies.AddOrUpdate(x => x.Guid, entity);

            await SaveChangesAsync();
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

            await SaveChangesAsync();
            return new DeletedEntity<Currency>(deletedCurrency);
        }
    }
}
