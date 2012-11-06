using System;
using System.Collections.Generic;
using System.Linq;

namespace Chiro.Cdf.Poco.Entity
{
    public class EfRepository<TContext, TEntity>: IRepository<TEntity> where TContext:IContext where TEntity: class
    {
        private readonly TContext _context;

        public EfRepository(TContext context)
        {
            _context = context;
        }

        public IQueryable<TEntity> Select()
        {
            return _context.Set<TEntity>().AsQueryable();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().AsEnumerable();
        }

        public IEnumerable<TEntity> Where(Func<TEntity, bool> predicate)
        {
            return _context.Set<TEntity>().Where(predicate);
        }

        public TEntity GetSingle(Func<TEntity, bool> predicate)
        {
            return _context.Set<TEntity>().Single(predicate);
        }

        public TEntity GetFirst(Func<TEntity, bool> predicate)
        {
            return _context.Set<TEntity>().First(predicate);
        }

        public void Add(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentException("Cannot add a null entity");

            _context.Set<TEntity>().Add(entity);
        }

        public void Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentException("Cannot delete a null entity");

            _context.Set<TEntity>().Remove(entity);
        }

        public void Attach(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentException("Cannot attach a null entity");

            _context.Set<TEntity>().Attach(entity);
        }
    }
}
