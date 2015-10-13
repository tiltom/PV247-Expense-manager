using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using ExpenseManager.BusinessLogic.DataAccessObjects.API;
using ExpenseManager.BusinessLogic.DatabaseContext;

namespace ExpenseManager.BusinessLogic.DataAccessObjects
{
    internal class Repository<TEntity> : IRepository<TEntity>, IDisposable
        where TEntity : class
    {
        protected ExpenseManagerContext Context;

        public Repository()
        {
            this.Context = new ExpenseManagerContext();
        }

        /// <summary>
        ///     Releases all resources used by the Entities
        /// </summary>
        public void Dispose()
        {
            if (null != this.Context)
            {
                this.Context.Dispose();
            }
        }

        public virtual TEntity Create()
        {
            return this.Context.Set<TEntity>().Create();
        }

        public virtual TEntity Create(TEntity entity)
        {
            return this.Context.Set<TEntity>().Add(entity);
        }

        public virtual TEntity Update(TEntity entity)
        {
            this.Context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public virtual void Delete(TEntity entity)
        {
            this.Context.Set<TEntity>().Remove(entity);
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> where)
        {
            var objects = this.Context.Set<TEntity>().Where(where).AsEnumerable();
            foreach (var item in objects)
            {
                this.Context.Set<TEntity>().Remove(item);
            }
        }

        public virtual TEntity FindById(int id)
        {
            return this.Context.Set<TEntity>().Find(id);
        }

        public virtual TEntity FindOne(Expression<Func<TEntity, bool>> where = null)
        {
            return this.FindAll(where).FirstOrDefault();
        }

        public IQueryable<T> All<T>() where T : class
        {
            return this.Context.Set<T>();
        }

        public virtual IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> where = null)
        {
            return null != where ? this.Context.Set<TEntity>().Where(where) : this.Context.Set<TEntity>();
        }

        public virtual bool SaveChanges()
        {
            return 0 < this.Context.SaveChanges();
        }

        public virtual void Delete(int id)
        {
            var item = this.Context.Set<TEntity>().Find(id);
            this.Context.Set<TEntity>().Remove(item);
        }
    }
}