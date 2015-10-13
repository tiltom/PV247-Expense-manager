﻿using System;
using System.Linq;
using System.Linq.Expressions;

namespace ExpenseManager.BusinessLogic.DataAccessObjects.API
{
    /// <summary>
    ///     Used Repository Pattern
    ///     See:
    ///     http://blog.swink.com.au/index.php/c-sharp/generic-repository-for-entity-framework-4-3-dbcontext-with-code-first/
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal interface IRepository<TEntity>
    {
        /// <summary>
        ///     Creates a new empty entity.
        /// </summary>
        TEntity Create();

        /// <summary>
        ///     Creates the existing entity.
        /// </summary>
        TEntity Create(TEntity entity);

        /// <summary>
        ///     Updates the existing entity.
        /// </summary>
        TEntity Update(TEntity entity);

        /// <summary>
        ///     Delete an entity using its primary key.
        /// </summary>
        void Delete(int id);

        /// <summary>
        ///     Delete the given entity.
        /// </summary>
        void Delete(TEntity entity);

        /// <summary>
        ///     Deletes the existing entity.
        /// </summary>
        void Delete(Expression<Func<TEntity, bool>> where);

        /// <summary>
        ///     Finds one entity based on provided criteria.
        /// </summary>
        TEntity FindOne(Expression<Func<TEntity, bool>> where = null);

        /// <summary>
        ///     Finds one entity based on its Identifier.
        /// </summary>
        TEntity FindById(int id);

        /// <summary>
        ///     Finds entities based on provided criteria.
        /// </summary>
        IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> where = null);

        /// <summary>
        ///     Finds other related entities based of type T for queries.
        /// </summary>
        IQueryable<T> All<T>() where T : class;

        /// <summary>
        ///     Save any changes to the TContext
        /// </summary>
        bool SaveChanges();
    }
}