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
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviSync.Helpers.Properties;

namespace Chiro.CiviSync.Helpers
{
    public class RelationshipHelper
    {
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
    }
}
