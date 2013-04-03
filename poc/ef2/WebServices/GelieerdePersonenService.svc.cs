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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.ServiceContracts;
using Cg2.Workers;
using Cg2.Orm;
using Cg2.Orm.Exceptions;
using System.Security.Permissions;

namespace Cg2.Services
{
    // NOTE: If you change the class name "GelieerdePersonenService" here, you must also update the reference to "GelieerdePersonenService" in Web.config.
    public class GelieerdePersonenService : IGelieerdePersonenService
    {
        [PrincipalPermission(SecurityAction.Demand, Role = Settings.GebruikersGroep)]
        public IList<GelieerdePersoon> AllenOphalen(int groepID)
        {
            GelieerdePersonenManager pm = new GelieerdePersonenManager();

            var result = pm.Dao.AllenOphalen(groepID);
            return result;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = Settings.GebruikersGroep)]
        public IList<GelieerdePersoon>  PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald)
        {
            GelieerdePersonenManager pm = new GelieerdePersonenManager();

            var result = pm.Dao.PaginaOphalen(groepID, pagina, paginaGrootte, out aantalOpgehaald);

            return result;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = Settings.GebruikersGroep)]
        public IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald)
        {
            GelieerdePersonenManager pm = new GelieerdePersonenManager();
            AuthorisatieManager am = new AuthorisatieManager();

            if (am.IsGav(ServiceSecurityContext.Current.WindowsIdentity.Name, groepID))
            {
                var result = pm.Dao.PaginaOphalenMetLidInfo(groepID, pagina, paginaGrootte, out aantalOpgehaald);

                return result;
            }
            else
            {
                throw new GeenGavException("Je bent geen GAV van deze groep.");
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = Settings.GebruikersGroep)]
        public void PersoonBewaren(GelieerdePersoon persoon)
        {
            GelieerdePersonenManager pm = new GelieerdePersonenManager();
            pm.Dao.Bewaren(persoon);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = Settings.GebruikersGroep)]
        public IList<GelieerdePersoon> zoekPersonen(string naamgedeelte, int pagina, int paginagrootte)
        {
            throw new NotImplementedException();
        }
        //... andere zoekmogelijkheden

        [PrincipalPermission(SecurityAction.Demand, Role = Settings.GebruikersGroep)]
        public GelieerdePersoon PersoonOphalenMetDetails(int gelieerdePersoonID)
        {
            GelieerdePersonenManager pm = new GelieerdePersonenManager();

            var result = pm.Dao.DetailsOphalen(gelieerdePersoonID);

            return result;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = Settings.GebruikersGroep)]
        public GelieerdePersoon PersoonOphalenMetDetails(int gelieerdePersoonID, PersoonsInfo gevraagd)
        {
            throw new NotImplementedException();
        }
    }
}
