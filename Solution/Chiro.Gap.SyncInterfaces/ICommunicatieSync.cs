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
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.SyncInterfaces
{
	/// <summary>
	/// Interface voor synchronisatie van communicatiemiddelen naar Kipadmin
	/// </summary>
	public interface ICommunicatieSync
	{
		/// <summary>
		/// Verwijdert een communicatievorm uit Kipadmin
		/// </summary>
		/// <param name="communicatieVorm">Te verwijderen communicatievorm, gekoppeld aan een gelieerde persoon 
		/// met ad-nummer.</param>
		void Verwijderen(CommunicatieVorm communicatieVorm);

		/// <summary>
		/// Bewaart een communicatievorm in Kipadmin
		/// </summary>
		/// <param name="commvorm">Te bewaren communicatievorm, gekoppeld aan persoon</param>
		void Toevoegen(CommunicatieVorm commvorm);

        /// <summary>
        /// Stuurt de gegeven <paramref name="communicatieVorm"/> naar Kipadmin. Om te weten welk de
        /// originele communicatievorm is, kijken we naar de gekoppelde persoon, en gebruiken we
        /// het oorspronkelijke nummer (<paramref name="origineelNummer"/>)
        /// </summary>
        /// <param name="communicatieVorm">Te updaten communicatievorm</param>
        /// <param name="origineelNummer">Oorspronkelijk nummer van die communicatievorm</param>
        /// <remarks>Het is best mogelijk dat het 'nummer' niet is veranderd, maar bijv. enkel de vlag 
        /// 'opt-in'</remarks>
        void Bijwerken(CommunicatieVorm communicatieVorm, string origineelNummer);
    }
}