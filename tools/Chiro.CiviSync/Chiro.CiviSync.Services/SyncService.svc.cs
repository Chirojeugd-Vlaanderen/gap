/*
   Copyright 2013-2015 Chirojeugd-Vlaanderen vzw

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

using System;
using System.Diagnostics;
using Chiro.Cdf.ServiceHelper;
using Chiro.CiviSync.Logic;
using Chiro.CiviSync.Services.Properties;
using Chiro.CiviSync.Workers;
using Chiro.Gap.Log;
using Chiro.Gap.UpdateApi.Client;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;
using Chiro.Mailchimp.Sync;

namespace Chiro.CiviSync.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SyncService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SyncService.svc or SyncService.svc.cs at the Solution Explorer and start debugging.
    public partial class SyncService : ISyncPersoonService
    {
        private readonly string _siteKey = Settings.Default.SiteKey;
        private readonly string _apiKey = Settings.Default.ApiKey;

        private readonly IMiniLog _log;
        private readonly ServiceHelper _serviceHelper;
        private readonly IGapUpdateClient _gapUpdateClient;
        private readonly RelationshipLogic _relationshipLogic;
        private readonly MembershipLogic _membershipLogic;

        private readonly ContactWorker _contactWorker;
        private readonly LidWorker _lidWorker;
        private readonly BivakWorker _bivakWorker;
        private readonly CommunicatieWorker _communicatieWorker;

        private readonly IChimpSyncHelper _chimpSyncHelper;

        protected ServiceHelper ServiceHelper
        {
            get { return _serviceHelper; }
        }

        /// <summary>
        /// Creates a new service instance.
        /// </summary>
        /// <param name="serviceHelper">Servicehelper that will connect to the CiviCRM API</param>
        /// <param name="gapUpdateClient">Wrapper rond de UpdateApi.</param>
        /// <param name="relationshipLogic">Zorgt vooral voor start- en einddata van relaties.</param>
        /// <param name="membershipLogic">Logica voor memberships. Ook vooral start- en einddata.</param>
        /// <param name="bivakWorker">Bivak goodies.</param>
        /// <param name="contactWorker">Contact goodies.</param>
        /// <param name="communicatieWorker">Communicatie goodies.</param>
        /// <param name="lidWorker">Lid goodies.</param>
        /// <param name="chimpSyncHelper">Communicatie met Mailchimp.</param>
        /// <param name="log">Logger</param>
        public SyncService(ServiceHelper serviceHelper, IGapUpdateClient gapUpdateClient,
            RelationshipLogic relationshipLogic, MembershipLogic membershipLogic, BivakWorker bivakWorker,
            ContactWorker contactWorker, CommunicatieWorker communicatieWorker, LidWorker lidWorker,
            IChimpSyncHelper chimpSyncHelper,
            IMiniLog log)
        {
            _serviceHelper = serviceHelper;
            _gapUpdateClient = gapUpdateClient;
            _relationshipLogic = relationshipLogic;
            _membershipLogic = membershipLogic;
            _bivakWorker = bivakWorker;
            _contactWorker = contactWorker;
            _communicatieWorker = communicatieWorker;
            _lidWorker = lidWorker;
            _chimpSyncHelper = chimpSyncHelper;
            _log = log;

            // Configureer externe API's van GapUpdate en workers.
            _gapUpdateClient.Configureren(Settings.Default.GapUpdateServer, Settings.Default.GapUpdatePath,
                Settings.Default.GapUpdateUser, Settings.Default.GapUpdatePass);

            _bivakWorker.Configureren(_apiKey, _siteKey);
            _contactWorker.Configureren(_apiKey, _siteKey);
            _communicatieWorker.Configureren(_apiKey, _siteKey);
            _lidWorker.Configureren(_apiKey, _siteKey);

            Debug.Assert(_gapUpdateClient != null);
        }

        public void GroepUpdaten(Groep g)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Invalideer gecachete data.
        /// </summary>
        public void CacheInvalideren()
        {
            _contactWorker.CacheInvalideren();
        }

        /// <summary>
        /// Zoekt het Civi-ID (contact-ID) op op basis van de gegevens in <paramref name="persoon"/>.
        /// </summary>
        /// <param name="persoon">Persoon waarvoor we het Civi-ID zoeken.</param>
        /// <param name="waarvoorNodig">Korte uitleg waarvoor het Civi-ID nodig is;
        /// wordt gebruikt als er fouten gelogd moeten worden.</param>
        /// <returns>Civi-ID van de opgegeven <paramref name="persoon"/>, of
        /// <c>null</c> als dat niet gevonden werd.</returns>
        /// <remarks>
        /// Persoon is een beperkt datacontract. We hopen stiekem dat er een
        /// AD-nummer of een GAP-ID is, op basis waarvan we de persoon makkelijk
        /// kunnen vinden.
        /// 
        /// Deze functie staat wat raar in de SyncService, ik zou ze eerder in de
        /// ContactWorker zetten. Maar dat vraagt dan wat geknoei om die updateClient
        /// goed te krijgen.
        /// 
        /// Deze methode wordt enkel gebruikt voor methods waarvoor er geen aparte
        /// variant bestaat met en zonder AD-nummer. Misschien kan ze dus op termijn
        /// weg (zie #3711).
        /// </remarks>
        private int? CiviIdGet(Persoon persoon, string waarvoorNodig)
        {
            if (persoon.AdNummer == null)
            {
                persoon.AdNummer = _contactWorker.AdNummerZoeken(persoon);
            }

            if (persoon.AdNummer == null)
            {
                _log.Loggen(Niveau.Error,
                    String.Format("{0}: persoon {1} niet gevonden.", waarvoorNodig, persoon),
                    null, null, null);
                return null;
            }

            int? contactId = _contactWorker.ContactIdGet(persoon.AdNummer.Value);

            if (contactId != null) return contactId;

            // Ongeldig AD moet terug naar GAP.
            _log.Loggen(Niveau.Error, String.Format("{0}: Onbestaand AD-nummer {1} terug naar GAP.", waarvoorNodig, persoon),
                null, persoon.AdNummer, null);

            _gapUpdateClient.OngeldigAdNaarGap(persoon.AdNummer.Value);
            return null;
        }
    }
}
