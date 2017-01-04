/*
 * Copyright 2013 Ben Bridts.
 * Aangepaste gebruikersrechten Copyright 2014 Chirojeugd-Vlaanderen vzw.
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

using System;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Poco.Context;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts;
using System.Collections.Generic;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApi.Helpers
{
    public class ApiHelper
    {
        private readonly ServiceHelper _serviceHelper;
        protected ServiceHelper ServiceHelper { get { return _serviceHelper; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serviceHelper">De servicehelper die het groepwerkjaar zal
        /// opleveren.</param>
        public ApiHelper(ServiceHelper serviceHelper)
        {
            _serviceHelper = serviceHelper;
        }

        /// <summary>
        /// Deze functie haalt het huidige groepswerkjaar van de eerste groep van de
        /// aangelogde gebruiker op.
        /// </summary>
        /// <param name="context">Object context</param>
        /// <returns>Huidige groepswerkjaar van de eerste groep van de aangelogde
        /// gebruiker.</returns>        
        public GroepsWerkJaar GetGroepsWerkJaar(ChiroGroepEntities context)
        {
            Debug.Assert(HttpContext.Current.Request.LogonUserIdentity != null);

            var mijnGroepen = ServiceHelper.CallService<IGroepenService, IEnumerable<GroepInfo>>(
                svc => svc.MijnGroepenOphalen());

            // Code gestolen uit JaarOvergangController. Kunnen we dit zonder de service?
            // Haal de ID van het huidige groepswerkjaar op
            var id = ServiceHelper.CallService<IGroepenService, int>(
                svc => svc.RecentsteGroepsWerkJaarIDGet(mijnGroepen.First().ID));

            // Haal het huidige groepswerkjaar uit de DB
            return context.GroepsWerkJaar.Find(id);
        }
    }
}