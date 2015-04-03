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
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Helpers.Properties;
using Chiro.CiviSync.Logic;

namespace Chiro.CiviSync.Helpers
{
    public class RelationshipHelper
    {
        private IDatumProvider _datumProvider;

        /// <summary>
        /// Constructor voor RelationshipHelper.
        /// </summary>
        /// <param name="datumProvider">Datumprovider die gebruikt zal worden om te kijken welke datum het
        /// vandaag is.</param>
        public RelationshipHelper(IDatumProvider datumProvider)
        {
            _datumProvider = datumProvider;
        }

        /// <summary>
        /// Levert het werkjaar van de gegeven relatie <paramref name="r"/> op. We bepalen
        /// een werkjaar aan de hand van het jaar waarin het start.
        /// </summary>
        /// <param name="r">Relatie waarvoor het werkjaar te bepalen is.</param>
        /// <returns>Het gevraagde werkjaar.</returns>
        public int WerkjaarGet(Relationship r)
        {
            if (r.StartDate == null)
            {
                throw new InvalidOperationException("Kan geen werkjaar bepalen van relatie zonder startdatum.");
            }
            DateTime werkJaarStart = Settings.Default.WerkjaarStart;
            DateTime overgangDitJaar = new DateTime(r.StartDate.Value.Year, werkJaarStart.Month, werkJaarStart.Day);

            if (r.StartDate.Value >= overgangDitJaar)
            {
                return overgangDitJaar.Year;
            }
            return overgangDitJaar.Year - 1;
        }

        /// <summary>
        /// Creeert een relationshiprequest van het gegeven <paramref name="type"/> voor een relatie tussen
        /// de contacten met ID's <paramref name="contact1Id"/> en <paramref name="contact2Id"/> voor het 
        /// gegeven <paramref name="werkJaar"/>.
        /// </summary>
        /// <param name="type">Relatietype</param>
        /// <param name="contact1Id">Civi-ID van het eerste contact.</param>
        /// <param name="contact2Id">Civi-ID van het tweede contact.</param>
        /// <param name="werkJaar">Werkjaar voor de relatie.</param>
        /// <param name="uitschrijfDatum">Als het om een inactieve relatie gaat: de uitschrijfdatum.</param>
        /// <returns>Een relationshiprequest van het gegeven <paramref name="type"/> voor een relatie tussen
        /// de contacten met ID's <paramref name="contact1Id"/> en <paramref name="contact2Id"/> voor het 
        /// gegeven <paramref name="werkJaar"/>.</returns>
        public RelationshipRequest VanWerkjaar(RelatieType type, int contact1Id, int contact2Id, int werkJaar, DateTime? uitschrijfDatum)
        {
            // We bekijken de datums zonder uren, dus discrete dagen. De EndDate valt volledig binnen de
            // relationship.

            DateTime overgangDatum = Settings.Default.WerkjaarStart;
            DateTime beginWerkjaar = new DateTime(werkJaar, overgangDatum.Month, overgangDatum.Day);
            DateTime eindeWerkJaar = new DateTime(werkJaar + 1, overgangDatum.Month, overgangDatum.Day).AddDays(-1);
            DateTime vandaag = _datumProvider.Vandaag();

            var result = new RelationshipRequest
            {
                ContactIdA = contact1Id,
                ContactIdB = contact2Id,
                StartDate = vandaag,
                EndDate = new DateTime(werkJaar + 1, overgangDatum.Month, overgangDatum.Day).AddDays(-1),
                RelationshipTypeId = (int)type,
            };
            // Als een inschrijving gebeurde voor het werkjaar begon, wordt de startdatum de eerste dag van het
            // werkjaar
            if (result.StartDate < beginWerkjaar)
            {
                result.StartDate = beginWerkjaar;
            }
            // Als een inschrijving gebeurde wanneer het werkjaar al voorbij was, wordt de startdatum de laatste
            // dag van het werkjaar.
            if (result.StartDate > eindeWerkJaar)
            {
                result.StartDate = eindeWerkJaar;
            }
            if (uitschrijfDatum != null && uitschrijfDatum < result.EndDate)
            {
                // Het gaat om een uitschrijving voor het einde van het werkjaar.
                result.EndDate = uitschrijfDatum;
            }

            result.IsActive = IsActief(result);
            return result;
        }

        /// <summary>
        /// Geeft <c>true</c> als (afgaande op start- en einddatum) de gegeven relatie
        /// <paramref name="r"/> op dit moment actief is.
        /// </summary>
        /// <param name="r">relatie</param>
        /// <returns><c>true</c> als (afgaande op start- en einddatum) de gegeven relatie
        /// <paramref name="r"/> op dit moment actief is.</returns>
        public bool IsActief(Relationship r)
        {
            DateTime vandaag = _datumProvider.Vandaag();
            return r.StartDate <= vandaag && r.EndDate >= vandaag;
        }

        /// <summary>
        /// Geeft <c>true</c> als (afgaande op start- en einddatum) gegeven realtionship request
        /// <paramref name="r"/> op dit moment actief is.
        /// </summary>
        /// <param name="r">relationship request</param>
        /// <returns><c>true</c> als (afgaande op start- en einddatum) gegeven relationship request
        /// <paramref name="r"/> op dit moment actief is.</returns>
        public bool IsActief(RelationshipRequest r)
        {
            DateTime vandaag = _datumProvider.Vandaag();
            return r.StartDate <= vandaag && r.EndDate >= vandaag;
        }
    }
}
