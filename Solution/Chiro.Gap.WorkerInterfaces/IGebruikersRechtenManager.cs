/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * Verfijnen gebruikersrechten Copyright 2015 Chirojeugd-Vlaanderen vzw
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
using System;
using System.Collections.Generic;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Domain;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface IGebruikersRechtenManager
    {
        /// <summary>
        /// Verlengt het gegeven <paramref name="gebruikersRecht"/> (indien mogelijk) tot het standaard aantal maanden
        /// na vandaag.
        /// </summary>
        /// <param name="gebruikersRecht">
        /// Te verlengen gebruikersrecht
        /// </param>
        void Verlengen(GebruikersRechtV2 gebruikersRecht);

        /// <summary>
        /// Pas de vervaldatum van het gegeven <paramref name="gebruikersRecht"/> aan, zodanig dat
        /// het niet meer geldig is.  ZONDER TE PERSISTEREN.
        /// </summary>
        /// <param name="gebruikersRecht">
        /// Te vervallen gebruikersrecht
        /// </param>
        void Intrekken(GebruikersRechtV2 gebruikersRecht);

        /// <summary>
        /// Pas de vervaldatum van het de <paramref name="gebruikersRechten"/> aan, zodanig dat
        /// ze niet meer geldig zijn.  ZONDER TE PERSISTEREN.
        /// </summary>
        /// <param name="gebruikersRechten">
        /// Te vervallen gebruikersrecht
        /// </param>
        void Intrekken(GebruikersRechtV2[] gebruikersRechten);

        /// <summary>
        /// Levert het gebruikersrecht op dat een <paramref name="gelieerdePersoon"/> heeft op zijn eigen groep. 
        /// If any.  Als <paramref name="gelieerdePersoon"/> geen gebruikersrechten heeft op zijn groep, wordt 
        /// <c>null</c> opgeleverd.
        /// </summary>
        /// <param name="gelieerdePersoon">Een gelieerde persoon</param>
        /// <returns>
        /// Het gebruikersrecht op dat een <paramref name="gelieerdePersoon"/> heeft op zijn eigen groep. If any. 
        /// Als <paramref name="gelieerdePersoon"/> geen gebruikersrechten heeft op zijn groep, wordt <c>null</c> 
        /// opgeleverd.
        /// </returns>
        GebruikersRechtV2 GebruikersRechtGet(GelieerdePersoon gelieerdePersoon);

        /// <summary>
        /// Zoekt het gebruikersrecht op van <paramref name="persoon"/> op <paramref name="groep"/>. Als dat nog niet
        /// bestaat, maak er een aan. Voeg de gevraagde permissies toe.
        /// </summary>
        /// <param name="persoon">Persoon die gebruikersrechten moet krijgen.</param>
        /// <param name="groep">Groep waarvoor de persoon gebruikersrechten moet krijgen.</param>
        /// <param name="persoonlijkeGegevens">Permissies op persoonlijke gegevens.</param>
        /// <param name="groepsGegevens">Permissies op de gegevens van de groep.</param>
        /// <param name="personenInAfdeling">Permissies op de leden in de eigen afdeling.</param>
        /// <param name="personenInGroep">Permissies op alle personen van de eigen groep.</param>
        void ToekennenOfWijzigen(Persoon persoon, Groep groep, Permissies persoonlijkeGegevens, Permissies groepsGegevens, Permissies personenInAfdeling, Permissies personenInGroep);
    }
}