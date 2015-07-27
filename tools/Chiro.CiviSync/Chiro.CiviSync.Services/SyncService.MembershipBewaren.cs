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
using System.Linq;
using System.ServiceModel;
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
        /// Bewaart een membership voor de persoon met gegeven <paramref name="adNummer"/> in het gegeven <paramref name="werkJaar"/>.
        /// </summary>
        /// <param name="adNummer">AD-nummer van persoon met te bewaren membership.</param>
        /// <param name="stamNummer">Stamnummer van ploeg die het membership aanmaakt.</param>
        /// <param name="werkJaar">Werkjaar waarvoor membership bewaard moet worden.</param>
        /// <param name="metLoonVerlies">Geeft aan of er een verzekering loonverlies nodig is.</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public async void MembershipBewaren(int adNummer, string stamNummer, int werkJaar, bool metLoonVerlies)
        {
            int? civiGroepId = _contactWorker.ContactIdGet(stamNummer);
            if (civiGroepId == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaande groep {0} - lid niet bewaard.", stamNummer),
                    stamNummer, adNummer, null);
                return;
            }

            var contact = _contactWorker.PersoonMetRecentsteMembership(adNummer, MembershipType.Aansluiting);
            if (contact == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaand AD-nummer {0} voor te bewaren membership - als dusdanig terug naar GAP.", adNummer),
                    null, adNummer, null);

                await _gapUpdateClient.OngeldigAdNaarGap(adNummer);
                return;
            }

            // request dat we als het nodig is naar de API zullen sturen.
            var membershipRequest = _membershipLogic.VanWerkjaar(MembershipType.Aansluiting, contact.Id,
                civiGroepId.Value, werkJaar);
            membershipRequest.VerzekeringLoonverlies = metLoonVerlies;

            if (contact.MembershipResult.Count == 1)
            {
                var bestaandMembership = contact.MembershipResult.Values.First();
                if (_membershipLogic.WerkjaarGet(bestaandMembership) == werkJaar)
                {
                    _membershipWorker.BestaandeBijwerken(bestaandMembership, stamNummer, metLoonVerlies);
                    return;
                }

                // Het recentste membership was van een vorig werkjaar. Neem join date over.
                membershipRequest.JoinDate = bestaandMembership.JoinDate;
            }
            membershipRequest.FactuurStatus = FactuurStatus.VolledigTeFactureren;

            var result = ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Membership>>(
                svc => svc.MembershipSave(_apiKey, _siteKey, membershipRequest));
            result.AssertValid();

            _log.Loggen(Niveau.Info,
                String.Format(
                    "Membership (aansluiting) {5} {3} {4}: AD {0} werkjaar {2} - memID {1}",
                    adNummer, result.Id, werkJaar, contact.FirstName, contact.LastName,
                    metLoonVerlies ? "met loonverlies" : String.Empty),
                null, adNummer, contact.GapId);
        }

        /// <summary>
        /// Bewaart een membership voor de persoon met gegeven <paramref name="details"/> in het gegeven <paramref name="werkJaar"/>
        /// </summary>
        /// <param name="details">Details van persoon met te bewaren membership.</param>
        /// <param name="stamNummer">Stamnummer van ploeg die het membership aanmaakt.</param>
        /// <param name="werkJaar">Werkjaar waarvoor het membership bewaard moet worden.</param>
        /// <param name="metLoonVerlies">Geeft aan of er een verzekering loonverlies nodig is.</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void MembershipNieuwePersoonBewaren(PersoonDetails details, string stamNummer, int werkJaar, bool metLoonVerlies)
        {
            // Update of maak de persoon, en vind zijn AD-nummer
            int adNr = UpdatenOfMaken(details);

            MembershipBewaren(adNr, stamNummer, werkJaar, metLoonVerlies);
        }
    }
}