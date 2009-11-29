using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data;
using System.Data.Objects.DataClasses;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;

using System.Linq.Expressions;

namespace Chiro.Cdf.Data.Entity
{
    class TestNieuweDao<TContext>
        where TContext : ObjectContext, new()
    {

        public TestNieuweDao()
        {
        }

        public virtual T Ophalen<T>(int id, params Expression<Func<T, object>>[] paths) where T : EntityObject, IEfBasisEntiteit
        {
            T result;

            using (TContext db = new TContext())
            {
                // Constructie db.CreateQuery<T>("[" + db.GetEntitySetName(typeof(T)) + "]").OfType<T>()
                // zorgt ervoor dat dit ook werkt voor overervende types.  (In dat geval is
                // de EntitySetName niet gelijk aan de naam van het type.)

                ObjectQuery<T> query = (from t in db.CreateQuery<T>("[" + db.GetEntitySetName(typeof(T)) + "]").OfType<T>()
                                        where t.ID == id
                                        select t) as ObjectQuery<T>;

                // query.MergeOption = MergeOption.NoTracking;
                // Je zou denken dat bovenstaande lijn ervoor zorgt dat de opgehaalde
                // objecten gedetacht zijn.  Maar dat is niet het geval.  (??) Vandaar
                // dat ik verderop toch DetachObjectGraph gebruik.

                result = (IncludesToepassen<T>(query, paths)).FirstOrDefault<T>();

            }

            if (result != null)
            {
                result = Utility.DetachObjectGraph(result);
            }


            return result;
        }

        /// <summary>
        /// 'Includet' gerelateerde entiteiten aan een objectquery.
        /// </summary>
        /// <param name="query">Betreffende query</param>
        /// <param name="paths">Paden naar mee op te halen gerelateerde entiteiten</param>
        /// <returns>Nieuwe query die de gevraagde entiteiten Includet</returns>
        protected ObjectQuery<T> IncludesToepassen<T>(ObjectQuery<T> query, Expression<Func<T, object>>[] paths)
        {
            ObjectQuery<T> resultaat = query;

            if (paths != null)
            {
                // Workaround om de Includes te laten werken
                // overgenomen van 
                // http://blogs.msdn.com/alexj/archive/2009/06/02/tip-22-how-to-make-include-really-include.aspx

                foreach (var v in paths)
                {
                    resultaat = resultaat.Include(v);
                }
            }

            return resultaat;
        }

        /// <summary>
        /// Bewaart/Updatet entiteit in database
        /// </summary>
        /// <param name="nieuweEntiteit">entiteit met nieuwe gegevens</param>
        /// <returns>De geupdatete entiteit</returns>
        /// <remarks>Nieuwe methode, die altijd attachobject graph gebruikt en dus als argument een array van lambda
        /// expressions meekrijgt</remarks>
        public virtual T Bewaren<T>(T entiteit, params Expression<Func<T, object>>[] paths) where T : EntityObject, IEfBasisEntiteit
        {
            using (TContext db = new TContext())
            {
                entiteit = db.AttachObjectGraph(entiteit, paths);

                // SetAllModified is niet meer nodig na AttachObjectGraph

                db.SaveChanges();

            }

            // Door Utility.DetachObjectGraph te gebruiken wanneer de context
            // niet meer bestaat, is het resultaat *echt* gedetacht.

            entiteit = Utility.DetachObjectGraph(entiteit);
            return entiteit;
        }
    }
}
