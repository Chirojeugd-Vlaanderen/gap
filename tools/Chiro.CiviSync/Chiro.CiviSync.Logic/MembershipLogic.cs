/*
   Copyright 2015, 2016 Chirojeugd-Vlaanderen vzw

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
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic.Properties;
using System.Diagnostics;

namespace Chiro.CiviSync.Logic
{
    /// <summary>
    /// Voornamelijk datums bepalen voor lidmaatschappen.
    /// 
    /// Dit is geen statische klasse, want we heben een datumprovider nodig.
    /// </summary>
    public class MembershipLogic
    {
        private readonly IDatumProvider _datumProvider;

        public MembershipLogic(IDatumProvider datumProvider)
        {
            _datumProvider = datumProvider;
        }

        /// <summary>
        /// Levert het werkjaar van het gegeven <paramref name="membership"/> op. We bepalen
        /// een werkjaar aan de hand van het jaar waarin het start.
        /// </summary>
        /// <param name="membership">Membership waarvoor het werkjaar te bepalen is.</param>
        /// <returns>Het gevraagde werkjaar.</returns>
        public int WerkjaarGet(Membership membership)
        {
            if (membership.StartDate == null)
            {
                throw new InvalidOperationException("Kan geen werkjaar bepalen van relatie zonder startdatum.");
            }
            DateTime werkJaarStart = Settings.Default.WerkjaarStart;
            DateTime overgang = new DateTime(membership.StartDate.Value.Year, werkJaarStart.Month, werkJaarStart.Day);

            if (membership.StartDate.Value >= overgang)
            {
                return overgang.Year;
            }
            return overgang.Year - 1;
        }

        /// <summary>
        /// Creeert een membershiprequest van het gegeven <paramref name="type"/> voor de
        /// contact met gegeven <paramref name="contactId"/> in het gegeven
        /// <paramref name="werkJaar"/>.
        /// </summary>
        /// <param name="type">Type membership.</param>
        /// <param name="contactId">ID van het contact dat het membership krijgt.</param>
        /// <param name="civiPloegId">ID van de ploeg die het membership aanvraagt.</param>
        /// <param name="werkJaar">Werkjaar waarin het contact het membership krijgt.</param>
        /// <returns>Membershiprequest van het gegeven <paramref name="type"/> voor de
        /// contact met gegeven <paramref name="contactId"/> in het gegeven
        /// <paramref name="werkJaar"/>.</returns>
        public MembershipRequest VanWerkjaar(MembershipType type, int contactId, int? civiPloegId, int werkJaar)
        {
            // We bekijken de datums zonder uren, dus discrete dagen. De EndDate valt volledig binnen de
            // relationship.

            DateTime overgangDatum = Settings.Default.WerkjaarStart;
            DateTime beginWerkjaar = new DateTime(werkJaar, overgangDatum.Month, overgangDatum.Day);
            DateTime eindeWerkJaar = new DateTime(werkJaar + 1, overgangDatum.Month, overgangDatum.Day).AddDays(-1);
            DateTime vandaag = _datumProvider.Vandaag();

            var result = new MembershipRequest
            {
                ContactId = contactId,
                AangemaaktDoorPloegId = civiPloegId,
                JoinDate = vandaag,
                EndDate = eindeWerkJaar,
                MembershipTypeId = (int) type,
            };

            if (vandaag < result.StartDate)
            {
                result.Status = MembershipStatus.Pending;
                result.StartDate = beginWerkjaar;
            }
            else if (vandaag > result.EndDate)
            {
                result.Status = MembershipStatus.Expired;
                result.StartDate = eindeWerkJaar;
            }
            else
            {
                // We bewaren hier enkel de datum (geen tijd), om rariteiten te vermijden als de
                // startdatum op de laatste dag van het werkjaar is.
                result.StartDate = vandaag.Date;
            }

            return result;
        }

        /// <summary>
        /// Kijkt na of <paramref name="bestaandMembership"/> een gratis membership was.
        /// We gaan er hier wel vanuit dat MembershipPayment mee opgevraagd is.
        /// </summary>
        /// <param name="bestaandMembership">Membership waarvan we moeten controleren of het gratis is.</param>
        /// <returns><c>true</c> als <paramref name="bestaandMembership"/> een gratis membership is.</returns>
        public bool IsGratis(Membership bestaandMembership)
        {
            Debug.Assert(bestaandMembership.MembershipPaymentResult != null);
            return bestaandMembership.FactuurStatus == FactuurStatus.FactuurOk &&
                   bestaandMembership.MembershipPaymentResult.Count == 0;
        }

        /// <summary>
        /// Geeft <c>true</c> als de einddatum van het <paramref name="membership"/> voorbij is.
        /// </summary>
        /// <param name="membership">Na te kijken membership.</param>
        /// <returns><c>true</c> als de einddatum van het <paramref name="membership"/> voorbij is.</returns>
        public bool IsVervallen(Membership membership)
        {
            return membership.EndDate != null && membership.EndDate < _datumProvider.Vandaag();
        }

        /// <summary>
        /// Vervangt de einddatum van het gegeven <paramref name="membership"/> door de datum van gisteren.
        /// </summary>
        /// <param name="membership">Te beeindigen membership.</param>
        /// <returns>Het beeindigde membership. Of <c>null</c> als het membership beter verwijderd zou
        /// worden (als de einddatum niet na de startdatum valt).</returns>
        public Membership Beeindigen(Membership membership)
        {
            membership.EndDate = _datumProvider.Vandaag().AddDays(-1);
            return membership.EndDate > membership.StartDate ? membership : (Membership) null;
        }
    }
}
