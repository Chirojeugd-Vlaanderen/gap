﻿/*
   Copyright 2015 Chirojeugd-Vlaanderen vzw

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
using System.Linq;
using System.ServiceModel;
using System.Web;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic;
using Chiro.Gap.Log;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {
        /// <summary>
        /// Updatet de functies van een lid.
        /// </summary>
        /// <param name="persoon">
        /// Persoon van wie de lidfuncties geüpdatet moeten worden
        /// </param>
        /// <param name="stamNummer">
        /// Stamnummer van de groep waarin de persoon lid is
        /// </param>
        /// <param name="werkJaar">
        /// Werkjaar waarin de persoon lid is
        /// </param>
        /// <param name="functies">
        /// Toe te kennen functies.  Eventuele andere reeds toegekende functies worden verwijderd.
        /// </param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void FunctiesUpdaten(Persoon persoon, string stamNummer, int werkJaar, FunctieEnum[] functies)
        {
            int? contactIdPersoon = CiviIdGet(persoon, "Functies updaten");
            if (contactIdPersoon == null) return;

            int? contactIdGroep = _contactWorker.ContactIdGet(stamNummer);
            if (contactIdGroep == null)
            {
                _log.Loggen(Niveau.Error,
                    String.Format("Onbestaande groep {0} - functies van lid {1} niet bewaard.", stamNummer, persoon),
                    stamNummer, persoon.AdNummer, null);
                return;
            }

            var lid = _lidWorker.LidOphalen(contactIdPersoon, contactIdGroep, werkJaar);
            if (lid == null)
            {
                _log.Loggen(Niveau.Error,
                    String.Format("Lid {1} niet gevonden in groep {0}, werkjaar {2}. Kon functies niet updaten.",
                        stamNummer, persoon, werkJaar),
                    stamNummer, persoon.AdNummer, null);
                return;
            }

            // Nieuw request
            var request = new RelationshipRequest
            {
                Id = lid.Id,
                Functies = FunctieLogic.KipCodes(functies)
            };

            var result =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Relationship>>(
                    svc => svc.RelationshipSave(_apiKey, _siteKey, request));
            result.AssertValid();

            _log.Loggen(Niveau.Info,
                String.Format(
                    "{0} stamnr {1} werkjaar {2} - nieuwe functie(s) {3} relID {4}",
                    persoon, stamNummer, werkJaar, String.Join(",", functies.Select(afd => afd.ToString())), result.Id),
                stamNummer, persoon.AdNummer, persoon.ID);

        }
    }
}