using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Chiro.Cdf.Poco
{
    public class Repository<TEntity>: IRepository<TEntity> where TEntity: BasisEntiteit
    {
        protected readonly IContext _context;

        public Repository(IContext context)
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

        /// <summary>
        /// Zoekt de entiteit op met de gegeven ID
        /// </summary>
        /// <param name="id">ID van op te zoeken entiteit</param>
        /// <returns>Entiteit met de gegeven ID. <c>null</c> als ze niet is gevonden.</returns>
        public TEntity ByID(int id)
        {
            return _context.Set<TEntity>().FirstOrDefault(x => x.ID == id);
        }

        /// <summary>
        /// Zoekt de entiteiten met gegeven <paramref name="IDs"/>.
        /// </summary>
        /// <param name="IDs">ID's van op te halen entiteiten</param>
        /// <returns>
        /// Entiteiten met gegeven <paramref name="IDs"/>. Onbestaande entiteiten worden
        /// genegeerd.
        /// </returns>
        public List<TEntity> ByIDs(IEnumerable<int> IDs)
        {
            return _context.Set<TEntity>().Where(x => IDs.Contains(x.ID)).ToList();
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

        /// <summary>
        /// Hash van de achterliggende context. Enkel voor diagnostic purposes.
        /// </summary>
        public int ContextHash
        {
            get { return _context.GetHashCode(); }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        #region Disposable etc

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    //Debug.WriteLine("Disposing context {0}", _context.GetHashCode());
                   _context.Dispose();
                }
                disposed = true;
            }
        }

        ~Repository()
        {
            Dispose(false);
        }

        #endregion

    }
}
