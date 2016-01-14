/*
 * Copyright 2015 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Linq;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Abonnemneten goodies.
    /// </summary>
    public class AbonnementenManager: IAbonnementenManager
    {
        /// <summary>
        /// Levert voor de gegeven <paramref name="gelieerdePersoon"/> het huidige abonnement op
        /// de publicatie met gegeven <paramref name="publicatieID"/>, als er zo'n abonnement is voor dit jaar.
        /// </summary>
        /// <param name="gelieerdePersoon"></param>
        /// <param name="publicatieID"></param>
        /// <returns>Abonnement, of <c>null</c> als er geen abonnement is.</returns>
        public Abonnement HuidigAbonnementGet(GelieerdePersoon gelieerdePersoon, int publicatieID)
        {
            var groepsWerkjaar =
                gelieerdePersoon.Groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar).FirstOrDefault();
            if (groepsWerkjaar == null)
            {
                return null;
            }

            // Als je toepassing hierop crasht, dan is de abonnemententabel nog niet bijgewerkt voor issue
            // #1537. Je kunt dat doen met deze query:
            // alter table abo.Abonnement add Type int not null default 1;
            var abonnement = gelieerdePersoon.Abonnement.FirstOrDefault(ab => ab.GroepsWerkJaar.ID == groepsWerkjaar.ID && ab.Publicatie.ID == publicatieID);
            return abonnement;
        }

        /// <summary>
        /// Levert voor de gegeven <paramref name="gelieerdePersoon"/> het type van het huidige abonnement van
        /// de publicatie met gegeven <paramref name="publicatieID"/>, als er zo'n abonnement is voor dit jaar.
        /// </summary>
        /// <param name="gelieerdePersoon"></param>
        /// <param name="publicatieID"></param>
        /// <returns>Abonnement, of <c>null</c> als er geen abonnement is.</returns>
        public AbonnementType? HuidigAbonnementTypeGet(GelieerdePersoon gelieerdePersoon, int publicatieID)
        {
            var abonnement = HuidigAbonnementGet(gelieerdePersoon, publicatieID);
            return abonnement == null ? (AbonnementType?) null : abonnement.Type;
        }
    }
}
