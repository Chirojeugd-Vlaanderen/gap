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
using Chiro.CiviSync.Logic.Properties;

namespace Chiro.CiviSync.Logic
{
    public class RelationshipLogic
    {
        private readonly IDatumProvider _datumProvider;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="datumProvider">Datumprovider die gebruikt zal worden om te kijken welke datum het
        /// vandaag is.</param>
        public RelationshipLogic(IDatumProvider datumProvider)
        {
            _datumProvider = datumProvider;
        }

        /// <summary>
        /// Berekent de startdatum van het gegeven <paramref name="werkJaar"/>.
        /// </summary>
        /// <param name="werkJaar"></param>
        /// <returns>De startdatum van het gegeven <paramref name="werkJaar"/>.</returns>
        /// <remarks>Dit hoort mogelijk beter thuis in een aparte WerkJaarWorker.</remarks>
        public DateTime WerkJaarStart(int werkJaar)
        {
            DateTime overgangDatum = Settings.Default.WerkjaarStart;
            return new DateTime(werkJaar, overgangDatum.Month, overgangDatum.Day);
        }

        /// <summary>
        /// Berekent de einddatum van het gegeven <paramref name="werkJaar"/>.
        /// </summary>
        /// <param name="werkJaar"></param>
        /// <returns>Einddatum van het gegeven <paramref name="werkJaar"/></returns>
        /// <remarks>Dit hoort mogelijk beter thuis in een aparte WerkJaarWorker.</remarks>
        public DateTime WerkJaarEinde(int werkJaar)
        {
            DateTime overgangDatum = Settings.Default.WerkjaarStart;
            return new DateTime(werkJaar + 1, overgangDatum.Month, overgangDatum.Day).AddDays(-1);
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
        /// de contacten met ID's <paramref name="contact1Id"/> en <paramref name="contact2Id"/>.
        /// </summary>
        /// <param name="type">Relatietype</param>
        /// <param name="contact1Id">Civi-ID van het eerste contact.</param>
        /// <param name="contact2Id">Civi-ID van het tweede contact.</param>
        /// <param name="uitschrijfDatum">Als het om een inactieve relatie gaat: de uitschrijfdatum.</param>
        /// <returns>Een relationshiprequest van het gegeven <paramref name="type"/> voor een relatie tussen
        /// de contacten met ID's <paramref name="contact1Id"/> en <paramref name="contact2Id"/>.</returns>
        public RelationshipRequest RequestMaken(RelatieType type, int contact1Id, int contact2Id, DateTime? uitschrijfDatum)
        {
            // We bekijken de datums zonder uren, dus discrete dagen. De EndDate valt volledig binnen de
            // relationship.

            DateTime vandaag = _datumProvider.Vandaag();

            var result = new RelationshipRequest
            {
                ContactIdA = contact1Id,
                ContactIdB = contact2Id,
                StartDate = vandaag,
                EndDate = uitschrijfDatum,
                RelationshipTypeId = (int)type,
            };

            result.IsActive = result.EndDate == null || result.EndDate > vandaag;
            return result;
        }

        /// <summary>
        /// Geeft <c>true</c> als de <paramref name="relatie"/> 'recent genoeg' is om terug te
        /// activeren.
        /// </summary>
        /// <param name="relatie"></param>
        /// <returns><c>true</c> als de relatie recent genoeg is om terug te activeren</returns>
        public bool IsHuidig(Relationship relatie)
        {
            DateTime vandaag = _datumProvider.Vandaag();
            if (relatie.IsActive)
            {
                // Actieve relaties zijn recente relaties
                return true;
            }
            if (relatie.StartDate == null)
            {
                // Inactieve relaties waarvan we niet meer weten wanneer ze begonnen, zijn oude
                // relaties.
                return false;
            }
            // Als de relatie inactief is, maar de startdatum ligt nog niet zo ver in het verleden,
            // dan is het een recente relatie. Ze kan dus terug geactiveerd worden.
            return ((DateTime) relatie.StartDate).AddDays(Settings.Default.RecupereerPeriode) > vandaag;
        }
    }
}
