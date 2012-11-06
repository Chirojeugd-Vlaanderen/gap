using System;
using System.Collections.Generic;
using System.Linq;

namespace Chiro.Cdf.Poco
{
    public interface IRepository<TEntity> where TEntity: class
    {
        IQueryable<TEntity> Select();
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Where(Func<TEntity, bool> predicate);
        TEntity GetSingle(Func<TEntity, bool> predicate);
        TEntity GetFirst(Func<TEntity, bool> predicate);
        void Add(TEntity entity);
        void Delete(TEntity entity);
        void Attach(TEntity entity);
    }
}
