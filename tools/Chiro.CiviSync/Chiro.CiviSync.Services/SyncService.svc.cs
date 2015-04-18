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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using AutoMapper;
using Chiro.Cdf.ServiceHelper;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic;
using Chiro.CiviSync.Services.Properties;
using Chiro.CiviSync.Workers;
using Chiro.Gap.Log;
using Chiro.Gap.UpdateApi.Client;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;

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
        private readonly BivakWorker _bivakWorker;
        private readonly CommunicatieWorker _communicatieWorker;

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
        /// <param name="log">Logger</param>
        public SyncService(ServiceHelper serviceHelper, IGapUpdateClient gapUpdateClient,
            RelationshipLogic relationshipLogic, MembershipLogic membershipLogic, BivakWorker bivakWorker,
            ContactWorker contactWorker, CommunicatieWorker communicatieWorker,
            IMiniLog log)
        {
            _serviceHelper = serviceHelper;
            _gapUpdateClient = gapUpdateClient;
            _relationshipLogic = relationshipLogic;
            _membershipLogic = membershipLogic;
            _bivakWorker = bivakWorker;
            _contactWorker = contactWorker;
            _communicatieWorker = communicatieWorker;
            _log = log;

            // Configureer externe API's van GapUpdate en workers.
            _gapUpdateClient.Configureren(Settings.Default.GapUpdateServer, Settings.Default.GapUpdatePath,
                Settings.Default.GapUpdateUser, Settings.Default.GapUpdatePass);
            _bivakWorker.Configureren(Settings.Default.ApiKey, Settings.Default.SiteKey);
            _contactWorker.Configureren(_apiKey, _siteKey);
            _communicatieWorker.Configureren(_apiKey, _siteKey);

            Debug.Assert(_gapUpdateClient != null);
        }

        public void FunctiesUpdaten(Persoon persoon, string stamNummer, int werkJaar, IEnumerable<FunctieEnum> functies)
        {
            throw new NotImplementedException();
        }

        public void LidTypeUpdaten(Persoon persoon, string stamNummer, int werkJaar, LidTypeEnum lidType)
        {
            throw new NotImplementedException();
        }

        public void AfdelingenUpdaten(Persoon persoon, string stamNummer, int werkJaar, IEnumerable<AfdelingEnum> afdelingen)
        {
            throw new NotImplementedException();
        }

        public void LoonVerliesVerzekeren(int adNummer, string stamNummer, int werkJaar)
        {
            throw new NotImplementedException();
        }

        public void LoonVerliesVerzekerenAdOnbekend(PersoonDetails details, string stamNummer, int werkJaar)
        {
            throw new NotImplementedException();
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
    }
}
