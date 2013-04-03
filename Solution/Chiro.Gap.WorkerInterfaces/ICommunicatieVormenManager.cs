/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
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
using System.Collections.Generic;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface ICommunicatieVormenManager
    {
        /// <summary>
        /// Koppelt een communicatievorm aan een gelieerde persoon.
        /// </summary>
        /// <param name="gelieerdePersoon">
        /// De gelieerde persoon voor wie de communicatievorm toegevoegd of aangepast wordt
        /// </param>
        /// <param name="nieuweCommunicatieVorm">
        /// De nieuwe gegevens voor de communicatievorm
        /// </param>
        /// <returns>
        /// De lijst van effectief gekoppelde communicatievormen. Als <paramref name="nieuweCommunicatieVorm"/>
        /// gezinsgebonden is, kunnen dat er meer zijn.
        /// </returns>
        /// <remarks>
        /// Als de communicatievorm de eerste van een bepaald type is, dan wordt dat ook de voorkeur.
        /// </remarks>
        List<CommunicatieVorm> Koppelen(GelieerdePersoon gelieerdePersoon, CommunicatieVorm nieuweCommunicatieVorm);

        /// <summary>
        /// Stelt de gegeven communicatievorm in als voorkeurscommunicatievorm voor zijn
        /// type en gelieerde persoon
        /// </summary>
        /// <param name="cv">
        /// Communicatievorm die voorkeurscommunicatievorm moet worden,
        /// gegeven zijn type en gelieerde persoon
        /// </param>
        void VoorkeurZetten(CommunicatieVorm cv);

        /// <summary>
        /// Werkt de gegeven <paramref name="communicatieVorm"/> bij op basis van de informatie
        /// in <paramref name="communicatieInfo"/>.
        /// </summary>
        /// <param name="communicatieVorm">Bij te werken communicatievorm</param>
        /// <param name="communicatieInfo">Nieuwe informatie communicatievorm</param>
        void Bijwerken(CommunicatieVorm communicatieVorm, CommunicatieInfo communicatieInfo);
    }
}