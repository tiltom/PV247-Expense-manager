using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseManager.Database
{
    /// <summary>
    ///     Mainly asynchronous extensions of <see cref="IQueryable{T}" /> that would otherwise only be available via
    ///     EntityFramework.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        ///     Creates a System.Collections.Generic.List`1 from an System.Linq.IQueryable`1 by enumerating it asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">The type of the elements of source.</typeparam>
        /// <param name="source">The type of the elements of source.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains a
        ///     System.Collections.Generic.List`1 that contains elements from the input sequence.
        /// </returns>
        /// <remarks>Copied from <see cref="System.Data.Entity.QueryableExtensions.ToListAsync(IQueryable)" />.</remarks>
        public static Task<List<TEntity>> ToListAsync<TEntity>(this IQueryable<TEntity> source)
        {
            return System.Data.Entity.QueryableExtensions.ToListAsync(source);
        }
    }
}