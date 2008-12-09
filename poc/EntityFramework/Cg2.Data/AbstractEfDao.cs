using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Core.DataInterfaces;
using System.Data.Objects;
using System.Data;

namespace Cg2.Data.Ef
{
    /// <summary>
    /// Algemene implementatie van IDao; generieke CRUD-operaties voor
    /// een DAO-object.
    /// </summary>
    public class AbstractEfDao<T, TId>: IDao<T, TId> where T:class
    {
        #region IDao<T,TId> Members


        /// <summary>
        /// Dit doet echt niks, en moet door de descendants geimplementeerd
        /// worden.
        /// </summary>
        /// <param name="id">ID van op te halen object</param>
        /// <returns></returns>
        public virtual T Ophalen(TId id)
        {
            return default(T);
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
            using (Cg2ObjectContext db = new Cg2ObjectContext())
            {
                EntityKey sleutel;
                if (oorspronkelijkeEntiteit == null)
                {
                    db.AttachTo(typeof(T).BaseType.Name, nieuweEntiteit as object);
                    sleutel = db.CreateEntityKey(typeof(T).BaseType.Name, nieuweEntiteit);
                    ObjectStateEntry en = db.ObjectStateManager.GetObjectStateEntry(sleutel);
                    en.SetModified();

                    // TODO: mogelijk is de refresh niet meer nodig, nu EF uit
                    // beta is.

                    db.Refresh(RefreshMode.ClientWins, nieuweEntiteit as object);
                }
                else
                {
                    sleutel = db.CreateEntityKey(typeof(T).BaseType.Name, oorspronkelijkeEntiteit);
                    if (sleutel == null)
                    {
                        db.AttachTo(typeof(T).BaseType.Name, nieuweEntiteit as object);
                    }
                    else
                    {
                        db.Attach(oorspronkelijkeEntiteit as System.Data.Objects.DataClasses.IEntityWithKey);
                        db.ApplyPropertyChanges(sleutel.EntitySetName, nieuweEntiteit as object);
                    }

                    ObjectStateEntry en = db.ObjectStateManager.GetObjectStateEntry(sleutel);

                    // Ik zou denken dat de 'SetModified' niet nodig is als de
                    // oorsrpnkelijke entity meegegeven is.

                    en.SetModified();
                    db.Refresh(RefreshMode.ClientWins, oorspronkelijkeEntiteit as object);

                }
                db.SaveChanges();
            }
            return nieuweEntiteit;
        }

        #endregion
    }
}
