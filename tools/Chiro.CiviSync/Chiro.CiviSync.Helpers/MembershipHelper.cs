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
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Helpers.Properties;

namespace Chiro.CiviSync.Helpers
{
    public class MembershipHelper
    {
        private readonly IDatumHelper _datumHelper;

        public MembershipHelper(IDatumHelper datumHelper)
        {
            _datumHelper = datumHelper;
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
        /// <param name="werkJaar">Werkjaar waarin het contact het membership krijgt.</param>
        /// <returns>Membershiprequest van het gegeven <paramref name="type"/> voor de
        /// contact met gegeven <paramref name="contactId"/> in het gegeven
        /// <paramref name="werkJaar"/>.</returns>
        public MembershipRequest VanWerkjaar(MembershipType type, int contactId, int werkJaar)
        {
            // We bekijken de datums zonder uren, dus discrete dagen. De EndDate valt volledig binnen de
            // relationship.

            DateTime overgangDatum = Settings.Default.WerkjaarStart;
            DateTime beginWerkjaar = new DateTime(werkJaar, overgangDatum.Month, overgangDatum.Day);
            DateTime eindeWerkJaar = new DateTime(werkJaar + 1, overgangDatum.Month, overgangDatum.Day).AddDays(-1);
            DateTime vandaag = _datumHelper.Vandaag();

            var result = new MembershipRequest
            {
                ContactId = contactId,
                JoinDate = vandaag,
                StartDate = beginWerkjaar,
                EndDate = eindeWerkJaar,
                MembershipTypeId = (int) type,
            };

            if (vandaag < result.StartDate)
            {
                result.Status = MembershipStatus.Pending;
            }
            if (vandaag > result.EndDate)
            {
                result.Status = MembershipStatus.Expired;
            }

            return result;
        }
    }
}