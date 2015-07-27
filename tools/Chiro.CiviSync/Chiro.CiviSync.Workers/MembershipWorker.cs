/*
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
using System.Text;
using System.Threading.Tasks;
using Chiro.Cdf.ServiceHelper;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic;
using Chiro.Gap.Log;

namespace Chiro.CiviSync.Workers
{
    /// <summary>
    /// Aardigheidjes voor memberships.
    /// </summary>
    public class MembershipWorker: BaseWorker
    {
        private readonly MembershipLogic _membershipLogic;

        public MembershipWorker(ServiceHelper serviceHelper, IMiniLog log, MembershipLogic membershipLogic)
            : base(serviceHelper, log)
        {
            _membershipLogic = membershipLogic;
        }

        /// <summary>
        /// Werkt een bestaand membership bij.
        /// </summary>
        /// <param name="bestaandMembership">Bestaand membership.</param>
        /// <param name="metLoonVerlies">Als dit <c>true</c> is, en het huidige membership heeft
        /// nog geen verzekering loonverlies, dan krijgt het bestaande membership een verzekering
        /// loonverlies, en wordt zo nodig de factuurstatus aangepast (zie #3973 en #3995).
        /// Is dit <c>calse</c>, dan gebeurt er niets mee.</param>
        /// <param name="stamNummer">Stamnummer van de aanvrager. Als er iets wijzigt aan het
        /// mebership, wordt ook de aanvrager geupdatet. Als het membership ongewijzigd blijft,
        /// dan gebeurt er niets met dit stamnummer.</param>
        public void BestaandeBijwerken(Membership bestaandMembership, string stamNummer, bool metLoonVerlies)
        {
            int werkJaar = _membershipLogic.WerkjaarGet(bestaandMembership);
            // We halen de persoon opnieuw op. Wat een beetje overkill is, aangezien
            // dat enkel dient om te kunnen loggen. Maar het bijwerken van een bestaand
            // membership is hopelijk niet zodanig vaak nodig dat dit vertragend zal worden.

            var result =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Contact>>(
                    svc =>
                        svc.ContactGet(ApiKey, SiteKey,
                            new ContactRequest { Id = bestaandMembership.ContactId }));
            result.AssertValid();
            if (result.Count == 0)
            {
                Log.Loggen(Niveau.Error,
                    String.Format(
                        "Onbestaand contact {0} voor te verlengen menbership (id {1}, werkjaar {2}). Genegeerd. WTF.",
                        bestaandMembership.ContactId, bestaandMembership.Id, werkJaar),
                    stamNummer, null, null);
                return;                
            }
            var contact = result.Values.First();
            int adNummer = int.Parse(contact.ExternalIdentifier);

            int? civiGroepId = ContactIdGet(stamNummer);
            if (civiGroepId == null)
            {
                Log.Loggen(Niveau.Error, String.Format("Onbestaande groep {0} - lid niet bewaard.", stamNummer),
                    stamNummer, adNummer, null);
                return;
            }

            if (metLoonVerlies && !bestaandMembership.VerzekeringLoonverlies)
            {
                var membershipRequest = new MembershipRequest
                {
                    Id = bestaandMembership.Id,
                    VerzekeringLoonverlies = true,
                    AangemaaktDoorPloegId = civiGroepId,
                    FactuurStatus =
                        bestaandMembership.FactuurStatus == FactuurStatus.FactuurOk
                            ? FactuurStatus.ExtraVerzekeringTeFactureren
                            : bestaandMembership.FactuurStatus
                };

                var updateResult = ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Membership>>(
                    svc => svc.MembershipSave(ApiKey, SiteKey, membershipRequest));
                updateResult.AssertValid();
                Log.Loggen(Niveau.Info,
                    String.Format(
                        "Membership met ID {5} door {6} bijgewerkt voor {0} {1} (AD {2}, ID {3}) met verzekering loonverlies voor werkjaar {4}.",
                        contact.FirstName, contact.LastName, contact.ExternalIdentifier, contact.GapId,
                        werkJaar, updateResult.Id, stamNummer), stamNummer, adNummer, contact.GapId);
            }
            else
            {
                Log.Loggen(Niveau.Info,
                    String.Format(
                        "{0} {1} (AD {3}, ID {2}) was al aangesloten in werkjaar {4}. Nieuwe aansluiting genegeerd.",
                        contact.FirstName, contact.LastName, contact.GapId, contact.ExternalIdentifier, werkJaar),
                    null, adNummer, contact.GapId);
            }
        }
    }
}
