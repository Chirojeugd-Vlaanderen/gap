/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
﻿using System;
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
