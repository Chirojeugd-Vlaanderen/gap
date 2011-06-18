using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Algemeen.Data.Entity
{
    public class Entities : DbContext, IEntities
    {
        public void WijzigingenBewaren()
        {
            SaveChanges();
        }

        public IEnumerable<TEntiteit> Alles<TEntiteit>() where TEntiteit: class, IBasisEntiteit
        {
            return this.Set<TEntiteit>();
        }

        public TEntiteit Toevoegen<TEntiteit>(TEntiteit entiteit) where TEntiteit: class, IBasisEntiteit
        {
            return (TEntiteit) this.Set<TEntiteit>().Add(entiteit);
        }

        public TEntiteit Ophalen<TEntiteit>(int ID) where TEntiteit : class, IBasisEntiteit
        {
            return (TEntiteit) this.Set<TEntiteit>().Where(ent => ent.ID == ID).FirstOrDefault();
        }

        public TEntiteit Ophalen<TEntiteit>(int ID, params Expression<Func<TEntiteit, object>>[] paths) where TEntiteit : class, IBasisEntiteit
        {
            var query = this.Set<TEntiteit>().Where(ent => ent.ID == ID);
            var result = IncludesToepassen(query, paths);
            return result.FirstOrDefault();
        }

        public void Verwijderen<TEntiteit>(TEntiteit entiteit) where TEntiteit : class, IBasisEntiteit
        {
            this.Set<TEntiteit>().Remove(entiteit);
        }

        /// <summary>
        /// Includet gerelateerde entiteiten in een objectquery
        /// </summary>
        /// <param name="query">Betreffende query</param>
        /// <param name="paths">Paden naar mee op te halen gerelateerde entiteiten</param>
        /// <returns>Nieuwe query die de gevraagde entiteiten Includet</returns>
        protected IQueryable<TEntiteit> IncludesToepassen<TEntiteit>(IQueryable<TEntiteit> query, Expression<Func<TEntiteit, object>>[] paths) where TEntiteit : class, IBasisEntiteit
        {
            var resultaat = query;

            if (paths != null)
            {
                // Workaround om de Includes te laten werken
                // overgenomen van 
                // http://blogs.msdn.com/alexj/archive/2009/06/02/tip-22-how-to-make-include-really-include.aspx
                // en daarna via ReSharper omgezet in een Linq-expressie
                resultaat = paths.Aggregate(resultaat, (current, v) => current.Include(v));
            }

            return resultaat;
        }
    }
}