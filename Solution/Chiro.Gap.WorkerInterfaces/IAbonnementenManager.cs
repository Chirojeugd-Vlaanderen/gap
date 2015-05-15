/*
 * Copyright 2015 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
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

using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface IAbonnementenManager
    {
        /// <summary>
        /// Levert voor de gegeven <paramref name="gelieerdePersoon"/> het type van het huidige abonnement op
        /// de publicatie met gegeven <paramref name="publicatieID"/>, als er zo'n abonnement is voor dit jaar.
        /// </summary>
        /// <param name="gelieerdePersoon"></param>
        /// <param name="publicatieID"></param>
        /// <returns>Type van het abonnement, of <c>null</c> als er geen abonnement is.</returns>
        AbonnementType? HuidigAbonnementGet(GelieerdePersoon gelieerdePersoon, int publicatieID);
    }
}
