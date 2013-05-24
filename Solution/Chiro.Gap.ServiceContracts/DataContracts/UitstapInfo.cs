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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Basisinformatie over een uitstap.
	/// </summary>
	/// <remarks>Startdatum en einddatum zijn <c>DateTime?</c>, opdat we dit
	/// datacontract ook als model zouden kunnen gebruiken in de webappl.  Als
	/// Startdatum en einddatum nullable zijn, dan zijn ze bij het aanmaken
	/// van een nieuwe uitstap gewoon leeg, ipv een nietszeggende datum in het
	/// jaar 1 als ze niet nullable zijn.</remarks>
	[DataContract]
	[KnownType(typeof(UitstapOverzicht))]
	public class UitstapInfo: IPeriode
	{
	    /// <summary>
	    /// De unieke ID van de uitstap
	    /// </summary>
	    [DataMember]
		public int ID { get; set; }

	    /// <summary>
	    /// De naam die de groep zelf geeft aan de uitstap (bv. 'Groot kamp 2011')
	    /// </summary>
	    [DataMember]
		[Verplicht]
		[DisplayName(@"Omschrijving van de uitstap")]
		public string Naam { get; set; }

	    /// <summary>
	    /// Geeft aan of het om een bivak gaat, waarvan de gegevens dus als bivakaangifte
	    /// naar KipAdmin moeten gaan, of een andere uitstap, waarvan de gegevens enkel bij
	    /// de groep zelf gekend moeten zijn (en die dus onvolledig mogen zijn).
	    /// </summary>
	    [DataMember]
        [DisplayName(@"Registreren als bivakaangifte")]
		public bool IsBivak { get; set; }

	    /// <summary>
	    /// De datum van vertrek
	    /// </summary>
	    /// <remarks>Nullable, om te kunnen opmerken dat de gebruiker niets invulde</remarks>
	    [DataMember]
		[DisplayName(@"Begindatum")]
		[DataType(DataType.Date)]
		[Verplicht]
		public DateTime? DatumVan { get; set; }

	    /// <summary>
	    /// De datum van thuiskomst
	    /// </summary>
        /// <remarks>Nullable, om te kunnen opmerken dat de gebruiker niets invulde</remarks>
	    [DataMember]
		[DisplayName(@"Einddatum")]
		[DataType(DataType.Date)]
		[Verplicht]
		public DateTime? DatumTot { get; set; }

	    /// <summary>
	    /// Eventuele extra info
	    /// </summary>
	    [DataMember]
		[DataType(DataType.MultilineText)]
		[DisplayName(@"Opmerkingen")]
		public string Opmerkingen { get; set; }

        /// <summary>
        /// Geeft stringrepresentatie van Versie weer (hex).
        /// Nodig om versie te bewaren in MVC view.
        /// </summary>
	    [DataMember]
		public string VersieString { get; set; }
    }
}
