/*
   Copyright 2016 Chirojeugd-Vlaanderen vzw

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

using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic;
using Chiro.Gap.Log;
using System;
using System.Linq;
using System.ServiceModel;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {
        /// <summary>
        /// Desactiveert een actieve lidrelatie in CiviCRM.
        /// </summary>
        /// <param name="adNummer">
        /// AD-nummer te desactiveren lid.
        /// </param>
        /// <param name="stamNummer">
        /// Stamnummer te desactiveren lid.
        /// </param>
        /// <param name="uitschrijfDatum">te registreren uitschrijfdatum in CiviCRM.</param>
        /// <remarks>
        /// In principe kun je een lid ook uitschrijven m.b.v. LidBewaren, waarbij het te bewaren
        /// lid inactief is. Maar dat wil zeggen dat je in GAP een lid moet hebben. Deze functie
        /// kunnen we gebruiken als het te desactiveren lid niet bestaat in GAP.
        /// (Dat is alleen zo als er iets louche aan de hadn is, zie #4554.)
        /// </remarks>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void LidUitschrijven(int adNummer, string stamNummer, DateTime uitschrijfDatum)
        {
            int? civiGroepId = _contactWorker.ContactIdGet(stamNummer);
            if (civiGroepId == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaande groep {0} - lid niet uitgeschreven.", stamNummer),
                    stamNummer, adNummer, null);
                return;
            }

            var contact = _contactWorker.PersoonMetActiefLid(adNummer, civiGroepId);

            if (contact == null || contact.ExternalIdentifier == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaand AD-nummer {0} voor uit te schrijven lid - genegeerd.", adNummer),
                    stamNummer, adNummer, null);
                return;
            }

            if (contact.RelationshipResult.Count >= 1)
            {
                if (contact.RelationshipResult.Count > 1)
                {
                    _log.Loggen(Niveau.Warning,
                        String.Format(
                            "Meer dan 1 actieve lidrelatie voor {0} {1} (AD {2}) in groep {3}.",
                            contact.FirstName, contact.LastName, contact.ExternalIdentifier, stamNummer),
                        stamNummer, adNummer, contact.GapId);

                }
                var relatie = contact.RelationshipResult.Values.First();
                _log.Loggen(Niveau.Info,
                    String.Format(
                        "{0} {1} (AD {2}) uitschrijven voor groep {3}. Startdatum was {4:dd/MM/yyyy}.*",
                        contact.FirstName, contact.LastName, contact.ExternalIdentifier, stamNummer, relatie.StartDate),
                    stamNummer, adNummer, contact.GapId);

                var request = new RelationshipRequest
                {
                    Id = contact.RelationshipResult.Id,
                    IsActive = false,
                    EndDate = uitschrijfDatum
                };

                var result = ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Relationship>>(
                    svc =>
                        svc.RelationshipSave(_apiKey, _siteKey, request));
                result.AssertValid();
            }
            else
            {
                _log.Loggen(Niveau.Warning,
                    String.Format(
                        "{0} {1} (AD {2}) niet uitgeschreven voor groep {3} - geen actieve lidrelatie gevonden.*",
                        contact.FirstName, contact.LastName, contact.ExternalIdentifier, stamNummer),
                    stamNummer, adNummer, contact.GapId);
            }
        }
    }
}