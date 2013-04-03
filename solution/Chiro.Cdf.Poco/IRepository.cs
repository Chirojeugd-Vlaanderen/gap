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

        /// <summary>
        /// Zoekt de entiteiten met gegeven <paramref name="IDs"/>.
        /// </summary>
        /// <param name="IDs">ID's van op te halen entiteiten</param>
        /// <returns>
        /// Entiteiten met gegeven <paramref name="IDs"/>. Onbestaande entiteiten worden
        /// genegeerd.
        /// </returns>
        List<TEntity> ByIDs(IEnumerable<int> IDs);

        void Add(TEntity entity);

        /// <summary>
        /// Verwijdert de gegeven <paramref name="entity"/>
        /// </summary>
        /// <param name="entity">Te verwijderen entity</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Verwijdert de gegeven <paramref name="entities"/>
        /// </summary>
        /// <param name="entities">Te verwijderen entititeiten</param>
        void Delete(IList<TEntity> entities);

        void Attach(TEntity entity);

        /// <summary>
        /// Hash code van de achterliggende context. Enkel voor diagnostic purposes.
        /// </summary>
        int ContextHash { get; }
        
        void SaveChanges();
    }
}
