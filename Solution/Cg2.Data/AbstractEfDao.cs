using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data;
using System.Data.Objects.DataClasses;

using Cg2.Orm.DataInterfaces;
using Cg2.Orm;
using Cg2.EfWrapper;
using Cg2.EfWrapper.Entity;

using System.Linq.Expressions;

namespace Cg2.Data.Ef
{
    /// <summary>
    /// Algemene implementatie van IDao; generieke CRUD-operaties voor
    /// een DAO-object.
    /// </summary>
    /// <remarks>Alle entity's die de DAO oplevert, moeten gedetacht zijn.
    /// Dit kan via MergeOption.NoTracking, of via Utility.DetachObjectGraph.
    /// </remarks>

    public class Dao<T>: IDao<T> where T:EntityObject, IBasisEntiteit
    {
        #region IDao<T> Members

        protected Expression<Func<T, object>>[] connectedEntities;

        public Dao()
        {
            connectedEntities = new Expression<Func<T, object>>[0];
        }

        public virtual Expression<Func<T, object>>[] getConnectedEntities()
        {
            return connectedEntities;
        }


        /// <summary>
        /// Ophalen van een entity op basis van ID
        /// </summary>
        /// <param name="id">ID van op te halen object</param>
        /// <returns></returns>
        public virtual T Ophalen(int id) 
        {
            Expression<Func<T, object>>[] paths = {};
            return Ophalen(id, paths);
        }

        public virtual T Ophalen(int id, params Expression<Func<T, object>>[] paths)
        {
            T result;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                // Constructie db.CreateQuery<T>("[" + db.GetEntitySetName(typeof(T)) + "]").OfType<T>()
                // zorgt ervoor dat dit ook werkt voor overervende types.  (In dat geval is
                // de EntitySetName niet gelijk aan de naam van het type.)

                ObjectQuery<T> query = (from t in db.CreateQuery<T>("[" + db.GetEntitySetName(typeof(T)) + "]").OfType<T>()
                             where t.ID == id
                             select t) as ObjectQuery<T>;

                query.MergeOption = MergeOption.NoTracking;

                result = (IncludesToepassen(query,paths)).FirstOrDefault<T>(); 

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
        protected ObjectQuery<T> IncludesToepassen(ObjectQuery<T> query, Expression<Func<T, object>>[] paths)
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
        /// Alle entity's van het gegeven type ophalen
        /// </summary>
        /// <returns>Een lijst met objecten</returns>
        public virtual List<T> AllesOphalen()
        {
            List<T> result;
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                // Constructie met OfType zodat overerving ook werkt.
                ObjectQuery<T> oq = db.CreateQuery<T>("[" + db.GetEntitySetName(typeof(T)) + "]").OfType<T>();
                oq.MergeOption = MergeOption.NoTracking;
                result = oq.ToList<T>();
            }
            return result;
        }

        /// <summary>
        /// Bewaart/Updatet entiteit in database
        /// </summary>
        /// <param name="nieuweEntiteit">entiteit met nieuwe gegevens</param>
        /// <returns>De geupdatete entiteit</returns>
        /// <remarks>Deze functie mag ook gebruikt worden voor het toevoegen
        /// van een nieuwe entiteit, of het verwijderen van een bestaande.
        /// In dat laatste geval is de terugkeerwaarde null.</remarks>
        public virtual T Bewaren(T entiteit)
        {
            return Bewaren(entiteit, getConnectedEntities());
        }

        /// <summary>
        /// Bewaart/Updatet entiteit in database
        /// </summary>
        /// <param name="nieuweEntiteit">entiteit met nieuwe gegevens</param>
        /// <returns>De geupdatete entiteit</returns>
        /// <remarks>Nieuwe methode, die altijd attachobject graph gebruikt en dus als argument een array van lambda
        /// expressions meekrijgt</remarks>
        public virtual T Bewaren(T entiteit, params Expression<Func<T, object>>[] paths)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                entiteit = db.AttachObjectGraph(entiteit, paths);

                // SetAllModified is niet meer nodig na AttachObjectGraph

                db.SaveChanges();

            }

            // Door Utility.DetachObjectGraph te gebruiken wanneer de context
            // niet meer bestaat, is het resultaat *echt* gedetacht.

            return Utility.DetachObjectGraph(entiteit);
        }


        /// <summary>
        /// Bewaren van een lijst entiteiten
        /// </summary>
        /// <param name="es">lijst entiteiten</param>
        /// <param name="paths">paden met gerelateerde te bewaren
        /// entiteiten</param>
        /// <returns>De bewaarde entiteiten, maar gedetacht</returns>
        public virtual IList<T> Bewaren(IList<T> es, params Expression<Func<T, object>>[] paths)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.AttachObjectGraphs(es);
                db.SaveChanges();
            }

            return Utility.DetachObjectGraph(es);
        }


        /// <summary>
        /// Markeert entity als 'volledig gewijzigd'
        /// </summary>
        /// <param name="key">Key van te markeren entity</param>
        /// <param name="context">Context om wijzigingen in te markeren</param>
        /// <remarks>Deze functie staat hier mogelijk niet op zijn plaats</remarks>
        protected static void SetAllModified(EntityKey key, ObjectContext context)
        {
            var stateEntry = context.ObjectStateManager.GetObjectStateEntry(key);
            var propertyNameList = stateEntry.CurrentValues.DataRecordInfo.FieldMetadata.Select
              (pn => pn.FieldType.Name);
            foreach (var propName in propertyNameList)
            {
                try
                {
                    stateEntry.SetModifiedProperty(propName);
                }catch(InvalidOperationException)
                {
                    // WTF???
                }
                
            }
        }

        #endregion


    }


}
