using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;
using System.Data.Objects;
using System.Data;
using Cg2.Orm;
using System.Data.Objects.DataClasses;

namespace Cg2.Data.Ef
{
    /// <summary>
    /// Algemene implementatie van IDao; generieke CRUD-operaties voor
    /// een DAO-object.
    /// </summary>
    public class Dao<T>: IDao<T> where T:EntityObject, IBasisEntiteit
    {
        #region IDao<T> Members


        /// <summary>
        /// Ophalen van een entity op basis van ID
        /// </summary>
        /// <param name="id">ID van op te halen object</param>
        /// <returns></returns>
        public virtual T Ophalen(int id) 
        {
            T result;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                ObjectQuery<T> oq = db.CreateQuery<T>("[" + typeof(T).Name + "]");
                result = (
                    from t in oq
                    where t.ID == id
                    select t).FirstOrDefault<T>();
                db.Detach(result);
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
        /// Nieuwe entiteit persisteren in database
        /// </summary>
        /// <param name="entiteit">Te bewaren entiteit</param>
        /// <returns>Opnieuw de entiteit, met eventueel aangepast 
        /// ID.</returns>
        public virtual T Creeren(T entiteit)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.AddObject(typeof(T).Name, entiteit as object);
                db.SaveChanges();
            }
            return entiteit;
        }
        
        /// <summary>
        /// Verwijdert entiteit uit de database
        /// </summary>
        /// <param name="entiteit">Te verwijderen entiteit</param>
        public void Verwijderen(T entiteit)
        {
            // TODO: dit gaat niet werken, omdat de entiteit niet
            // aan de context gekoppeld wordt.

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.Attach(entiteit);
                db.DeleteObject(entiteit as object);
                db.SaveChanges();
            }
        }


        public void Commit()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Bewaart/Updatet entiteit in database
        /// </summary>
        /// <param name="nieuweEntiteit">entiteit met nieuwe gegevens</param>
        /// <returns>De geupdatete entiteit</returns>
        /// <remarks>Deze functie mag ook gebruikt worden voor het toevoegen
        /// van een nieuwe entiteit.</remarks>
        public virtual T Bewaren(T nieuweEntiteit)
        {
            // Code uit het boek aangepast, met dank aan
            // http://msdn.microsoft.com/en-us/magazine/cc700340.aspx

            if (nieuweEntiteit.ID == 0)
            {
                return Creeren(nieuweEntiteit);
            }
            else
            {
                using (ChiroGroepEntities db = new ChiroGroepEntities())
                {
                    // Als de entity key verloren is gegaan
                    // (wat typisch gebeurt bij mvc)
                    // dan moeten we hem terug genereren alvorens
                    // de entity terug geattacht kan worden.

                    nieuweEntiteit.EntityKey = db.CreateEntityKey(typeof(T).Name, nieuweEntiteit);

                    db.Attach(nieuweEntiteit);
                    SetAllModified(nieuweEntiteit.EntityKey, db);

                    db.SaveChanges();
                }
                return nieuweEntiteit;
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
                stateEntry.SetModifiedProperty(propName);
        }

        #endregion


    }


}
