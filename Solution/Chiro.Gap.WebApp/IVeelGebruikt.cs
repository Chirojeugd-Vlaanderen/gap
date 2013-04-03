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
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp
{
	public interface IVeelGebruikt
	{
	    /// <summary>
	    /// Verwijdert de gecachete functieproblemen van een bepaalde groep
	    /// </summary>
	    /// <param name="groepID">ID van de groep waarvan de problemencache leeg te maken</param>
	    void FunctieProblemenResetten(int groepID);

		/// <summary>
		/// Haalt de problemen van de groep ivm functies op.  Uit de cache, of als die gegevens niet
		/// aanwezig zijn, via de service.
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan de functieproblemen opgehaald moeten worden</param>
		/// <returns>De rij functieproblemen</returns>
		IEnumerable<FunctieProbleemInfo> FunctieProblemenOphalen(int groepID);

		/// <summary>
		/// Haalt WoonPlaatsInfo op voor woonplaatsen met gegeven <paramref name="postNummer"/>
		/// </summary>
		/// <param name="postNummer">Postnummer waarvan de woonplaatsen gevraagd zijn</param>
		/// <returns>WoonPlaatsInfo voor woonplaatsen met gegeven <paramref name="postNummer"/></returns>
		IEnumerable<WoonPlaatsInfo> WoonPlaatsenOphalen(int postNummer);

		/// <summary>
		/// Levert een lijst op van alle woonplaatsen
		/// </summary>
		/// <returns>Een lijst met alle beschikbare woonplaatsen</returns>
		IEnumerable<WoonPlaatsInfo> WoonPlaatsenOphalen();

		/// <summary>
		/// Verwijdert het recentste groepswerkjaar van groep met ID <paramref name="groepID"/>
		/// uit de cache.
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan groepswerkjaarcache te resetten</param>
		void GroepsWerkJaarResetten(int groepID);

		/// <summary>
		/// Haalt het recentste groepswerkjaar van de groep met gegeven <paramref name="groepID"/>
		/// op uit de cache, of - indien niet beschikbaar - van backend.
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan we het groepswerkjaar willen weten.</param>
		/// <returns>Details over het gevraagde groepswerkjaar.</returns>
		GroepsWerkJaarDetail GroepsWerkJaarOphalen(int groepID);

		/// <summary>
		/// Haalt de status van de bivakaangifte op van de groep met gegeven <paramref name="groepID"/>
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan we het groepswerkjaar willen weten.</param>
		/// <returns>Details over de status van de bivakaangifte.</returns>
		BivakAangifteLijstInfo BivakStatusHuidigWerkjaarOphalen(int groepID);

		/// <summary>
		/// Als de gav met gegeven <paramref name="login"/> gav is van precies 1 groep, dan
		/// levert deze method het ID van die groep op.  Zo niet, is het resultaat
		/// <c>0</c>.
		/// </summary>
		/// <param name="login">Login van de GAV</param>
		/// <returns>GroepID van unieke groep van gegeven GAV, anders <c>0</c></returns>
		int UniekeGroepGav(string login);

		/// <summary>
		/// Indien <c>true</c> werken we in de liveomgeving, anders in de testomgeving.
		/// </summary>
		/// <returns><c>True</c> als we live bezig zijn</returns>
		bool IsLive();

		/// <summary>
		/// Haalt een lijstje op met informatie over ontbrekende gegevens bij leden.
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan we de problemen opvragen</param>
		/// <returns>Een rij LedenProbleemInfo met info over de ontbrekende gegevens</returns>
		IEnumerable<LedenProbleemInfo> LedenProblemenOphalen(int groepID);

		/// <summary>
		/// Verwijdert de gecachete ledenproblemen van een bepaalde groep
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan de problemencache leeg te maken</param>
		void LedenProblemenResetten(int groepID);

		/// <summary>
		/// Haalt alle landen op van de backend
		/// </summary>
		/// <returns>De landinfo van alle gekende landen</returns>
		IEnumerable<LandInfo> LandenOphalen();

	    /// <summary>
	    /// Verwijdert de bivakproblemen voor groep met id <paramref name="groepID"/> uit de cache.
	    /// </summary>
	    /// <param name="groepID">ID van groep met te verwijderen problemen</param>
	    void BivakStatusResetten(int groepID);

		/// <summary>
		/// Reset alle problemen omdat de jaarovergang wordt uitgevoerd
		/// </summary>
		/// <param name="groepID">ID van groep met te verwijderen problemen</param>
		void JaarOvergangReset(int groepID);
	}
}