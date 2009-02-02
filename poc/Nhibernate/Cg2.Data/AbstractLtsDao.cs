using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Core.DataInterfaces;
using System.Data;
using Cg2.Core.Domain;
using System.Data.Linq;

namespace Cg2.Data.LTS
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

            using (Cg2DataContext db = new Cg2DataContext())
            {
                Table<T> tabel = db.GetTable(typeof(T)) as Table<T>;

                result = (
                    from t in tabel
                    where t.ID == id
                    select t).FirstOrDefault<T>();
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
            using (Cg2DataContext db = new Cg2DataContext())
            {
                Table<T> t = db.GetTable(typeof(T)) as Table<T>;
                result = t.ToList<T>();
            }
            return result;
        }

        /// <summary>
        /// Entiteit persisteren in database
        /// </summary>
        /// <param name="entiteit">Te bewaren entiteit</param>
        /// <returns>Opnieuw de entiteit, met eventueel aangepast 
        /// ID.</returns>
        /// <remarks>Dit is nog geen update!</remarks>
        public T Bewaren(T entiteit)
        {
            using (Cg2DataContext db = new Cg2DataContext())
            {
                ITable tab = db.GetTable(entiteit.GetType().BaseType);
                tab.InsertOnSubmit(entiteit);
                db.SubmitChanges();
            }
            return entiteit;
        }
        
        /// <summary>
        /// Verwijdert entiteit uit de database
        /// </summary>
        /// <param name="entiteit">Te verwijderen entiteit</param>
        public void Verwijderen(T entiteit)
        {
            using (Cg2DataContext db = new Cg2DataContext())
            {
                ITable tab = db.GetTable(entiteit.GetType().BaseType);
                tab.Attach(entiteit);
                tab.DeleteOnSubmit(entiteit);
                db.SubmitChanges();
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
            using (Cg2DataContext db = new Cg2DataContext())
            {
                ITable tab = db.GetTable(nieuweEntiteit.GetType().BaseType);
                if (oorspronkelijkeEntiteit == null)
                {
                    tab.Attach(nieuweEntiteit, true);
                }
                else
                {
                    tab.Attach(nieuweEntiteit, oorspronkelijkeEntiteit);
                }
                db.SubmitChanges();
            }
            return nieuweEntiteit;
        }

        #endregion


    }


}
