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
        /// Als de persoon met gegeven <paramref name="adNummer"/> een membership heeft, dan
        /// wordt op dat membership 'verzekering loonverlies' aangevinkt. In het andere geval gebeurt
        /// er niets, en is het de bedoeling dat 'verzekering loonverlies' meegestuurd wordt wanneer
        /// het membership wordt gemaakt.
        /// </summary>
        /// <param name="adNummer">AD-nummer van te verzekeren persoon</param>
        /// <param name="stamNummer">Stamnummer van de groep die de verzekering aanvraagt. Wordt enkel
        ///     gebruikt om te loggen.</param>
        /// <param name="werkJaar">Werkjaar voor de verzekering</param>
        /// <param name="gratis"></param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void LoonVerliesVerzekeren(int adNummer, string stamNummer, int werkJaar, bool gratis)
        {
            // Vind contact met zijn recentste membership.

            var contact = _contactWorker.PersoonMetRecentsteMembership(adNummer, MembershipType.Aansluiting);
            if (contact == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaand AD-nummer {0} voor te bewaren membership - als dusdanig terug naar GAP.", adNummer),
                    null, adNummer, null);

                _gapUpdateClient.OngeldigAdNaarGap(adNummer);
                return;
            }

            if (contact.MembershipResult.Count == 0 ||
                _membershipLogic.WerkjaarGet(contact.MembershipResult.Values.First()) != werkJaar)
            {
                // Geen membership gevonden in het gevraagde werkjaar. Doe niets, maar log.
                _log.Loggen(Niveau.Info,
                    String.Format(
                        "Nog geen membership voor {0} {1} (AD {2}, ID {3}). Verzekeringsvraag van {4} voor werkjaar {5} genegeerd.",
                        contact.FirstName, contact.LastName, contact.ExternalIdentifier, contact.GapId, stamNummer,
                        werkJaar), stamNummer, adNummer, contact.GapId);
                return;
            }

            _membershipWorker.BestaandeBijwerken(contact.MembershipResult.Values.First(),
                new MembershipGedoe {Gratis = gratis, MetLoonVerlies = true, StamNummer = stamNummer});
        }

        /// <summary>
        /// Als de persoon met gegeven <paramref name="details"/> een membership heeft, dan
        /// wordt op dat membership 'verzekering loonverlies' aangevinkt. In het andere geval gebeurt
        /// er niets, en is het de bedoeling dat 'verzekering loonverlies' meegestuurd wordt wanneer
        /// het membership wordt gemaakt.
        /// </summary>
        /// <param name="details">details van te verzekeren persoon</param>
        /// <param name="stamNummer">Stamnummer van de groep die de verzekering aanvraagt. Wordt enkel
        ///     gebruikt om te loggen.</param>
        /// <param name="werkJaar">Werkjaar voor de verzekering</param>
        /// <param name="gratis"></param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void LoonVerliesVerzekerenAdOnbekend(PersoonDetails details, string stamNummer, int werkJaar, bool gratis)
        {
            // Update of maak de persoon, en vind zijn AD-nummer
            int adNr = PersoonUpdatenOfMaken(details); 
            
            LoonVerliesVerzekeren(adNr, stamNummer, werkJaar, gratis);
        }
    }
}