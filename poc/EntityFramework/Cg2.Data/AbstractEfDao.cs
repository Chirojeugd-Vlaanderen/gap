using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Core.DataInterfaces;
using System.Data.Objects;

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

        #endregion
    }
}
