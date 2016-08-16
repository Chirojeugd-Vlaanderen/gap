/*
 * Copyright 2008-2013, 2016 the GAP developers. See the NOTICE file at the 
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

using System.Collections.Generic;
using Chiro.Gap.Domain;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Elk model dat door de master page gebruikt wordt, moet deze interface implementeren.
	/// </summary>
	public interface IMasterViewModel
	{
		/// <summary>
		/// Bepaalt of we op een live- of een testdatabase aan het werken zijn
		/// </summary>
		bool IsLive { get; }

		/// <summary>
		/// ID van de Chirogroep
		/// </summary>
		int GroepID { get; }

		/// <summary>
		/// Naam van de Chirogroep
		/// </summary>
		string GroepsNaam { get; }

		/// <summary>
		/// Plaats van de Chirogroep
		/// </summary>
		string Plaats { get; }

		/// <summary>
		/// Het stamnummer wordt niet meer gebruikt als primary key, maar zal nog wel
		/// lang gebruikt worden als handige manier om een groep op te zoeken.
		/// </summary>
		string StamNummer { get; }

		/// <summary>
		/// Titel van de webpagina
		/// </summary>
		string Titel { get; }

		/// <summary>
		/// Kan de GAV meerdere groepen beheren?
		/// </summary>
		bool? MeerdereGroepen { get; }

		/// <summary>
		/// Mededelingen die ergens getoond moeten worden
		/// </summary>
		IList<Mededeling> Mededelingen { get; }

		/// <summary>
		/// Int die het *jaartal* van het huidige werkJaar voor de groep bepaalt.
		/// (Bijv. 2010 voor 2010-2011)
		/// </summary>
		int HuidigWerkJaar { get; }

        /// <summary>
        /// 'Human readable' representatie van het huidige werkjaar, bijv. '2016-2017'.
        /// </summary>
        string WerkJaarWeergave { get; }

		/// <summary>
		/// Status van het huidige werkjaar.
		/// </summary>
		WerkJaarStatus WerkJaarStatus { get; }
	}
}
