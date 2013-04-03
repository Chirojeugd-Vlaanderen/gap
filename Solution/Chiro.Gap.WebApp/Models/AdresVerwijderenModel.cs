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

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model om adres te verwijderen.  Bevat een adres met daaraan
	/// gekoppeld de bewoners van wie het adres mogelijk vervalt
	/// </summary>
	/// <remarks>Je zou je kunnen afvragen waarom dit model opgebowd
	/// wordt op basis van AdresID en GelieerdePersoonID, en niet via
	/// enkel PersoonsAdresID.  Het heeft ermee te maken dat je toch
	/// steeds de oorspronkelijke GelieerdePersoonsID zal moeten onthouden,
	/// omdat je na het verwijderen van het persoonsadres wel terug moet
	/// kunnen redirecten naar de juiste persoonsinfo.
	/// Je moet dus 2 ID's bewaren, en dat kunnen dus net zo goed
	/// GelieerdePersoonID en AdresID zijn.</remarks>
	public class AdresVerwijderenModel : MasterViewModel
	{
        /// <summary>
        /// Saaie standaardconstructor
        /// </summary>
        public AdresVerwijderenModel()
        {
            PersoonIDs = new List<int>();
            Adres = new GezinInfo();
        }

		/// <summary>
		/// ID van GelieerdePersoon met het te verwijderen adres.
		/// Wordt bewaard om achteraf terug naar de details van de
		/// aanvrager te kunnen redirecten.
		/// </summary>
		public int AanvragerID { get; set; }

		/// <summary>
		/// AdresMetBewoners bevat te verwijderen adres,
		/// met daaraan gekoppeld alle bewoners die de aangelogde gebruiker
		/// mag zien.
		/// </summary>
		public GezinInfo Adres { get; set; }

		/// <summary>
		/// Het lijstje PersoonIDs bevat de ID's van de personen van wie
		/// het adres uiteindelijk zal vervallen.
		/// </summary>
		public IList<int> PersoonIDs { get; set; }
	}
}
