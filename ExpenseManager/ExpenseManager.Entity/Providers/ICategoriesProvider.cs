using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Providers.infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.Entity.Providers
{
    public interface ICategoriesProvider : IAddOrUpdateDeleteEntityProvider<Category>, ITransactionsProvider
    {
        IQueryable<Category> Categories { get; }
    }
}
