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

        private Expression<Func<T, object>>[] connectedEntities = {};

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
        /*public virtual T Creeren(T entiteit)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.AddObject(typeof(T).Name, entiteit as object);
                db.SaveChanges();
            }
            return entiteit;
        }*/
        
        /// <summary>
        /// Verwijdert entiteit uit de database
        /// </summary>
        /// <param name="entiteit">Te verwijderen entiteit</param>
        /*public void Verwijderen(T entiteit)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                // Ik gebruik AttachObjectGraph, zodat er een nieuwe
                // instantie gemaakt wordt van entiteit.  Op die manier
                // vermijd ik dat eventuele gerelateerde objecten van
                // entiteit mee geattacht worden.

                T geattacht = db.AttachObjectGraph(entiteit);

                db.DeleteObject(geattacht);
                db.SaveChanges();

            }
        }*/

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
                if (entiteit.ID != 0 && entiteit.EntityKey == null)
                {
                    //FIXME: voor een object van Leiding crasht dit, want dat is geen entity in de database
                    entiteit.EntityKey = db.CreateEntityKey(typeof(T).Name, entiteit);

                    //TODO als de lambda expressie gebruikt worden, moeten ook al hun entitykeys terug geladen worden!!!
                    //of toch eens kijken waarom die keys verloren gaan?
                }

                //FIXME entiteit verwijderen werkt nog niet

                //db.Attach(entiteit);
                entiteit = db.AttachObjectGraph(entiteit, paths);
                SetAllModified(entiteit.EntityKey, db);

                db.SaveChanges();
            }
            return entiteit;
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
