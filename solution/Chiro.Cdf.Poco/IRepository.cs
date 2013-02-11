using System;
using System.Collections.Generic;
using System.Linq;

namespace Chiro.Cdf.Poco
{
    public interface IRepository<TEntity> : IDisposable where TEntity: BasisEntiteit
    {
        IQueryable<TEntity> Select();
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Where(Func<TEntity, bool> predicate);
        TEntity GetSingle(Func<TEntity, bool> predicate);
        TEntity GetFirst(Func<TEntity, bool> predicate);
        /// <summary>
        /// Zoekt de entiteit op met de gegeven ID
        /// </summary>
        /// <param name="id">ID van op te zoeken entiteit</param>
        /// <returns>Entiteit met de gegeven ID. <c>null</c> als ze niet is gevonden.</returns>
        TEntity ByID(int id);
        void Add(TEntity entity);
        void Delete(TEntity entity);
        void Attach(TEntity entity);

        /// <summary>
        /// Hash code van de achterliggende context. Enkel voor diagnostic purposes.
        /// </summary>
        int ContextHash { get; }
        
        void SaveChanges();
    }
}
