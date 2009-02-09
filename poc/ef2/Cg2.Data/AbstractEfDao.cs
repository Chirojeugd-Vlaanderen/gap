using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Core.DataInterfaces;
using System.Data.Objects;
using System.Data;
using Cg2.Core.Domain;

namespace Cg2.Data.Ef
{
    /// <summary>
    /// Algemene implementatie van IDao; generieke CRUD-operaties voor
    /// een DAO-object.
    /// </summary>
    public class Dao<T>: IDao<T> where T:BasisEntiteit
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

            using (Cg2ObjectContext db = new Cg2ObjectContext())
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
            using (Cg2ObjectContext db = new Cg2ObjectContext())
            {
                ObjectQuery<T> oq = db.CreateQuery<T>("[" + typeof(T).Name + "]");
                result = oq.ToList<T>();
            }
            return result;
        }

        /// <summary>
        /// Entiteit persisteren in database
        /// </summary>
        /// <param name="entiteit">Te bewaren entiteit</param>
        /// <returns>Opnieuw de entiteit, met eventueel aangepast 
        /// ID.</returns>
        public T Bewaren(T entiteit)
        {
            using (Cg2ObjectContext db = new Cg2ObjectContext())
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

            using (Cg2ObjectContext db = new Cg2ObjectContext())
            {
                db.DeleteObject(entiteit as object);
                db.SaveChanges();
            }
        }


        public void Commit()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updatet entiteit in database
        /// </summary>
        /// <param name="nieuweEntiteit">entiteit met nieuwe gegevens</param>
        /// <param name="oorspronkelijkeEntiteit">entiteit met oorspronkelijke
        /// gegevens, indien nog beschikbaar.  Anders null.</param>
        /// <returns>De geupdatete entiteit</returns>
        public T Updaten(T nieuweEntiteit, T oorspronkelijkeEntiteit)
        {
            // Code uit het boek aangepast, met dank aan
            // http://msdn.microsoft.com/en-us/magazine/cc700340.aspx

            using (Cg2ObjectContext db = new Cg2ObjectContext())
            {
                EntityKey sleutel;
                if (oorspronkelijkeEntiteit == null)
                {
                    db.Attach(nieuweEntiteit);
                    sleutel = db.CreateEntityKey(typeof(T).Name, nieuweEntiteit);
                    SetAllModified(sleutel, db);
                }
                else
                {
                    db.Attach(oorspronkelijkeEntiteit);
                    sleutel = db.CreateEntityKey(typeof(T).Name, nieuweEntiteit);
                    db.ApplyPropertyChanges(sleutel.EntitySetName, nieuweEntiteit as object);
                }
                db.SaveChanges();
            }
            return nieuweEntiteit;
        }

        /// <summary>
        /// Markeert entity als 'volledig gewijzigd'
        /// </summary>
        /// <param name="key">Key van te markeren entity</param>
        /// <param name="context">Context om wijzigingen in te markeren</param>
        /// <remarks>Deze functie staat hier mogelijk niet op zijn plaats</remarks>
        private static void SetAllModified(EntityKey key, ObjectContext context)
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
