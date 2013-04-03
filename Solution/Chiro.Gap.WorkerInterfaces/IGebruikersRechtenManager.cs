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
using System;
using System.Collections.Generic;
using Chiro.Gap.Poco.Model;

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
        void Verlengen(GebruikersRecht gebruikersRecht);

        /// <summary>
        /// Zoekt een account voor de gegeven <paramref name="gelieerdePersoon"/>, en maakt een account aan
        /// als die niet gevonden wordt. Als een nieuwe account wordt gemaakt, krijgt de gelieerde persoon
        /// een e-mail via zijn voorkeursmailadres.
        /// </summary>
        /// <param name="gelieerdePersoon">Gelieerde persoon waarvoor een account gezocht of gemaakt moet 
        /// worden. Er wordt verondersteld dat die al gekoppeld is aan zijn accounts. Wat mogelijk een
        /// beetje stom is. Soit.</param>
        /// <returns>Account voor de gelieerde persoon (klasse Gav zou beter hernoemd worden als account, 
        /// zie #1357)</returns>
        /// <remarks>De gemaakte account heeft geen rechten.</remarks>
        Gav AccountZoekenOfMaken(GelieerdePersoon gelieerdePersoon);

        /// <summary>
        /// Zoekt een account voor de gegeven <paramref name="gelieerdePersoon"/>. Als geen account wordt
        /// gevonden, en <paramref name="makenAlsNietGevonden"/> <c>true</c> is, wordt een nieuwe account 
        /// aangemaakt, en krijgt de gelieerde persoon een e-mail via zijn voorkeursmailadres.
        /// </summary>
        /// <param name="gelieerdePersoon">Gelieerde persoon waarvoor een account gezocht of gemaakt moet 
        /// worden. Er wordt verondersteld dat die al gekoppeld is aan zijn accounts. Wat mogelijk een
        /// beetje stom is. Soit.</param>
        /// <param name="makenAlsNietGevonden"> Indien <c>true</c> wordt een nieuwe account gemaakt als
        /// er geen is gevonden.</param>
        /// <returns>Account voor de gelieerde persoon (klasse Gav zou beter hernoemd worden als account, 
        /// zie #1357)</returns>
        /// <remarks>De gemaakte account heeft geen rechten.</remarks>
        Gav AccountZoekenOfMaken(GelieerdePersoon gelieerdePersoon, bool makenAlsNietGevonden);

        /// <summary>
        /// Pas de vervaldatum van het gegeven <paramref name="gebruikersRecht"/> aan, zodanig dat
        /// het niet meer geldig is.  ZONDER TE PERSISTEREN.
        /// </summary>
        /// <param name="gebruikersRecht">
        /// Te vervallen gebruikersrecht
        /// </param>
        void Intrekken(GebruikersRecht gebruikersRecht);

        /// <summary>
        /// Pas de vervaldatum van het de <paramref name="gebruikersRechten"/> aan, zodanig dat
        /// ze niet meer geldig zijn.  ZONDER TE PERSISTEREN.
        /// </summary>
        /// <param name="gebruikersRechten">
        /// Te vervallen gebruikersrecht
        /// </param>
        void Intrekken(GebruikersRecht[] gebruikersRechten);

        /// <summary>
        /// Kent gebruikersrechten toe voor gegeven <paramref name="groep"/> aan gegeven <paramref name="account"/>.
        /// Standaard zijn deze rechten 14 maand geldig. Als de gebruikersrechten al bestonden, worden ze indien
        /// mogelijk verlengd.
        /// </summary>
        /// <param name="account">account die gebruikersrecht moet krijgen op <paramref name="groep"/></param>
        /// <param name="groep">groep waarvoor <paramref name="account"/> gebruikersrecht moet krijgen</param>
        /// <returns>Het gebruikersrecht</returns>
        /// <remarks>Persisteert niet.</remarks>
        GebruikersRecht ToekennenOfVerlengen(Gav account, Groep groep);

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
        GebruikersRecht GebruikersRechtGet(GelieerdePersoon gelieerdePersoon);
    }
}