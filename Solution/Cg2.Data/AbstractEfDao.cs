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
                db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;

                ObjectQuery<T> oq = db.CreateQuery<T>("[" + typeof(T).Name + "]");

                foreach (var v in paths)
                {
                    oq.Include(v);
                }

                result = (
                    from t in oq
                    where t.ID == id
                    select t).FirstOrDefault<T>();

                if (result != null)
                {
                    db.Detach(result);
                }
            }

            return result;
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
                ObjectQuery<T> oq = db.CreateQuery<T>("[" + typeof(T).Name + "]");
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
                // Als de entity key verloren is gegaan
                // (wat typisch gebeurt bij mvc)
                // dan moeten we hem terug genereren alvorens
                // de entity terug geattacht kan worden.

                //FIXME: voor een object van Leiding crasht dit, want dat is geen entity in de database
                EntityKeysHerstellen(entiteit, db);

                entiteit = db.AttachObjectGraph(entiteit, paths);

                // SetAllModified is niet meer nodig na AttachObjectGraph

                db.SaveChanges();
            }
            return entiteit;
        }


        //FIXME kan dit beter zonder foreach, maar het behandelen van de hele lijst?
        public virtual IList<T> Bewaren(IList<T> es, params Expression<Func<T, object>>[] paths)
        {
            List<T> list = new List<T>() ;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.Lid.MergeOption = MergeOption.NoTracking;



                foreach (T entiteit in es)
                {
                    EntityKeysHerstellen(entiteit, db);
                    list.Add(db.AttachObjectGraph(entiteit, paths));
                    SetAllModified(entiteit.EntityKey, db);
                }

                db.SaveChanges();
            }
            return list;
        }


        /// <summary>
        /// Herstelt zo nodig de entity key van een entiteit.
        /// </summary>
        /// <param name="entiteit">Entiteit waarvan key te herstellen is</param>
        /// <param name="db">Object context</param>
        public virtual void EntityKeysHerstellen(T entiteit, ChiroGroepEntities db) 
        {
            if (entiteit.ID != 0 && entiteit.EntityKey == null)
            {
                entiteit.EntityKey = db.CreateEntityKey(typeof(T).Name, entiteit);
            }
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
