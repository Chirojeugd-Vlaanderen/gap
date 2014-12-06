/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * Bijgewerkte authenticatie Copyright 2014 Johan Vervloet
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
using Chiro.Cdf.Poco;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WorkerInterfaces
{
    /// <summary>
    /// Levert veel gebruikte basislijstje op, eventueel via een cache
    /// </summary>
    public interface IVeelGebruikt
    {
        /// <summary>
        /// Haalt het beginjaar van het huidig werkjaar van de gegeven <paramref name="groep"/> op.
        /// (Bijv. 2012 voor 2012-2013)
        /// </summary>
        /// <param name="groep">groep waarvan het werkjaar opgezocht moet worden</param>
        /// <returns>
        /// het beginjaar van het huidig werkjaar van de <paramref name="groep"/>.
        /// (Bijv. 2012 voor 2012-2013)
        /// </returns>
        int WerkJaarOphalen(Groep groep);

        /// <summary>
        /// Invalideert het gecachete huidige werkjaar van de gegeven <paramref name="groep"/>
        /// </summary>
        /// <param name="groep">Groep waarvan gecachete werkjaar moet worden geinvalideerd.</param>
        void WerkJaarInvalideren(Groep groep);

        /// <summary>
        /// Haalt het AD-nummer op van de user met gegeven <paramref name="gebruikersNaam"/>.
        /// </summary>
        /// <param name="gebruikersNaam">Een gebruikersnaam.</param>
        /// <returns>Het AD-nummer van de user met die gebruikersnaam.</returns>
        int? AdNummerOphalen(string gebruikersNaam);

        /// <summary>
        /// Invalideert het gecachete AD-nummer voor de gebruiker met gegeven <paramref name="gebruikersNaam"/>.
        /// </summary>
        /// <param name="gebruikersNaam">Gebruikersnaam van gebruiker waarvan gecachete AD-nummer
        /// geinvalideerd moet worden.</param>
        void AdNummerInvalideren(string gebruikersNaam);

        /// <summary>
        /// Vraagt de gebruikersnaam op van de persoon met gegeven <paramref name="adNummer"/>.
        /// </summary>
        /// <param name="adNummer">AD-nummer van persoon wiens gebruikersnaam gezocht is.</param>
        /// <returns>De gebruikersnaam van de persoon met gegeven AD-nummer.</returns>
        string GebruikersNaamOphalen(int adNummer);

        /// <summary>
        /// Invalideert de gecachete gebruikersnaam van de persoon met gegeven <paramref name="adNummer"/>.
        /// </summary>
        /// <param name="adNummer">AD-nummer van persoon waarvan gebruikersnaam geinvalideerd moet worden.</param>
        void GebruikersNaamInvalideren(int? adNummer);
    }
}