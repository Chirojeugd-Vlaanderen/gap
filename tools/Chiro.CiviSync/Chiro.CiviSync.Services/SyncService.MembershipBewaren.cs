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
        /// <param name="werkJaar">Werkjaar waarvoor membership bewaard moet worden.</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public async void MembershipBewaren(int adNummer, int werkJaar)
        {
            var contact = _contactHelper.PersoonMetRecentsteMembership(adNummer, MembershipType.Aansluiting);

            if (contact == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaand AD-nummer {0} voor te bewaren membership - als dusdanig terug naar GAP.", adNummer),
                    null, adNummer, null);

                await _gapUpdateHelper.OngeldigAdNaarGap(adNummer);
                return;
            }

            // request dat we als het nodig is naar de API zullen sturen.
            var membershipRequest = _membershipHelper.VanWerkjaar(MembershipType.Aansluiting, contact.Id, werkJaar);

            if (contact.MembershipResult.Count == 1)
            {
                var bestaandMembership = contact.MembershipResult.Values.First();
                if (_membershipHelper.WerkjaarGet(bestaandMembership) == werkJaar)
                {
                    // We hebben al een membership dit jaar.
                    _log.Loggen(Niveau.Info,
                        String.Format(
                            "{0} {1} (AD {3}, ID {2}) was al aangesloten in werkjaar {4}. Nieuwe aansluiting genegeerd.",
                            contact.FirstName, contact.LastName, contact.GapId, contact.ExternalIdentifier, werkJaar),
                        null, adNummer, contact.GapId);
                    return;
                }
                else
                {
                    // Er was al een membership vorig jaar. Neem join date over.
                    membershipRequest.JoinDate = bestaandMembership.JoinDate;
                }
            }

            var result = ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Membership>>(
                svc => svc.MembershipSave(_apiKey, _siteKey, membershipRequest));
            AssertValid(result);

            _log.Loggen(Niveau.Info,
                String.Format(
                    "Membership (aansluiting) {3} {4}: AD {0} werkjaar {2} - memID {1}",
                    adNummer, result.Id, werkJaar, contact.FirstName, contact.LastName),
                null, adNummer, contact.GapId);
        }

        /// <summary>
        /// Bewaart een membership voor de persoon met gegeven <paramref name="details"/> in het gegeven <paramref name="werkJaar"/>
        /// </summary>
        /// <param name="details">Details van persoon met te bewaren membership.</param>
        /// <param name="werkJaar">Werkjaar waarvoor het membership bewaard moet worden.</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void MembershipNieuwePersoonBewaren(PersoonDetails details, int werkJaar)
        {
            // Update of maak de persoon, en vind zijn AD-nummer
            int adNr = UpdatenOfMaken(details);

            MembershipBewaren(adNr, werkJaar);
        }
    }
}