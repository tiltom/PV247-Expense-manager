using System;
using System.Linq;
using System.Linq.Expressions;

namespace ExpenseManager.Entity
{
    internal interface IExpenseManagerRepository<TEntity>
    {
        IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> where = null);
        bool SaveChanges();
    }
}