using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Core.DataInterfaces;
using System.Data;
using Cg2.Core.Domain;
using NHibernate;
using NHibernate.Exceptions;

namespace Cg2.Data.Nh
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
            using (ISession sessie = SessionFactory.Factory.OpenSession())
            {
                using (sessie.BeginTransaction())
                {
                    result = sessie.Get<T>(id);
                    sessie.Transaction.Commit();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Persisteert een transiente entiteit
        /// </summary>
        /// <param name="entiteit">Te bewaren entiteit</param>
        /// <returns>Opnieuw de entiteit, met eventueel aangepast 
        /// ID.</returns>
        /// <remarks>Dit is nog geen update!</remarks>
        public T Bewaren(T entiteit)
        {
            using (ISession sessie = SessionFactory.Factory.OpenSession())
            {
                using (sessie.BeginTransaction())
                {
                    sessie.Save(entiteit);
                    sessie.Transaction.Commit();
                }
            }
            return entiteit;
        }
        
        /// <summary>
        /// Verwijdert entiteit uit de database
        /// </summary>
        /// <param name="entiteit">Te verwijderen entiteit</param>
        public void Verwijderen(T entiteit)
        {
            using (ISession sessie = SessionFactory.Factory.OpenSession())
            {
                using (sessie.BeginTransaction())
                {
                    sessie.Delete(entiteit);
                    sessie.Transaction.Commit();
                }
            }
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
            using (ISession sessie = SessionFactory.Factory.OpenSession())
            {
                using (sessie.BeginTransaction())
                {
                    if (oorspronkelijkeEntiteit == null)
                    {
                        // Attach entiteit als volledig 'dirty'
                        sessie.Update(nieuweEntiteit);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    sessie.Transaction.Commit();
                }
            }
            return nieuweEntiteit;
        }

        /// <summary>
        /// Bewaart een transiente entiteit, of updatet een detached entiteit.
        /// </summary>
        /// <param name="entiteit">Te bewaren/updaten entiteit</param>
        /// <returns>referentie naar bewaarde/geupdatete entiteit</returns>
        public T BewarenOfUpdaten(T entiteit)
        {
            using (ISession sessie = SessionFactory.Factory.OpenSession())
            {
                using (sessie.BeginTransaction())
                {
                    sessie.SaveOrUpdate(entiteit);
                    sessie.Transaction.Commit();
                }
            }
            return entiteit;

        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        #endregion


    }


}
