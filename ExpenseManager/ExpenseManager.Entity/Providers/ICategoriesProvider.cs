using System.Linq;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Providers.infrastructure;

namespace ExpenseManager.Entity.Providers
{
    public interface ICategoriesProvider : IAddOrUpdateDeleteEntityProvider<Category>
    {
        IQueryable<Category> Categories { get; }
    }
}