// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// DataContract voor summiere info over communicatievormen
	/// </summary>
	[DataContract]
	public class CommunicatieInfo : ICommunicatie
	{
		/// <summary>
		/// Uniek identificatienummer
		/// </summary>
		[DataMember]
		public int ID { get; set; }

		/// <summary>
		/// De waarde voor de communicatievorm (kan ook bv. een e-mailadres zijn)
		/// </summary>
		[Verplicht]
		[DataMember]
		[StringLengte(160)]
		public string Nummer { get; set; }

		/// <summary>
		/// Geeft aan of de gebruiker toestemming geeft aan Chirojeugd Vlaanderen
		/// om communicatievorm te gebruiken als dat voor het communicatietype 
		/// van toepassing is
		/// </summary>
		/// <remarks>Voorlopig is dat alleen voor e-mailadressen van belang, voor de 
		/// Snelleberichtenlijsten. Bij <c>true</c> geeft de gebruiker aan dat het adres
		/// ingeschreven mag worden op de Snelleberichtenlijst.</remarks>
		[DataMember]
		[DisplayName(@"Mag gebruikt worden voor Snelleberichtenlijsten?")]
		public bool IsVoorOptIn { get; set; }

		/// <summary>
		/// Geeft aan of deze communicatievorm de voorkeur krijgt als de persoon
		/// verschillende communicatievormen van dit type heeft
		/// </summary>
		[DataMember]
		[DisplayName(@"Gebruiken om persoon te contacteren?")]
		public bool Voorkeur { get; set; }

		/// <summary>
		/// Extra info over de persoon, die niet in een specifieke property thuishoort
		/// </summary>
		[DataMember]
		[StringLengte(320)]
		[DataType(DataType.MultilineText)]
		public string Nota { get; set; }

		/// <summary>
		/// Geeft aan of de communicatievorm persoonlijk is (<c>false</c>) of dat ze
		/// door het hele gezin gebruikt wordt (<c>true</c>). Een vaste telefoon op het thuisadres
		/// is typisch gezinsgebonden, een gsm-nummer niet.
		/// </summary>
		[DataMember]
		[DisplayName(@"Voor heel het gezin?")]
		public bool IsGezinsGebonden { get; set; }

		/// <summary>
		/// Geeft stringrepresentatie van Versie weer (hex).
		/// Nodig om versie te bewaren in MVC view, voor concurrencycontrole.
		/// </summary>
		[DataMember]
		public string VersieString { get; set; }

		/// <summary>
		/// De ID van het communicatietype
		/// </summary>
		/// <remarks>Overgenomen van geassocieerde CommunicatieType</remarks>
		[DataMember]
		[Verplicht]
		public int CommunicatieTypeID { get; set; }

		/// <summary>
		/// Geeft aan of iemand toestemming moet geven voor Chirojeugd Vlaanderen
		/// waarden voor dit communicatietype mag gebruiken
		/// </summary>
		/// <remarks>Overgenomen van geassocieerde CommunicatieType</remarks>
		[DataMember]
		public bool CommunicatieTypeIsOptIn { get; set; }

		/// <summary>
		/// De 'naam' van het communicatietype
		/// </summary>
		/// <remarks>Overgenomen van geassocieerde CommunicatieType</remarks>
		[DataMember]
		public string CommunicatieTypeOmschrijving { get; set; }

		/// <summary>
		/// Een regular expression die aangeeft welke vorm de waarde voor dat type moet hebben
		/// </summary>
		/// <remarks>Overgenomen van geassocieerde CommunicatieType</remarks>
		[DataMember]
		public string CommunicatieTypeValidatie { get; set; }

		/// <summary>
		/// Een voorbeeld van een communicatievorm die volgens de validatieregels gestructureerd is
		/// </summary>
		/// <remarks>Overgenomen van geassocieerde CommunicatieType</remarks>
		[DataMember]
		public string CommunicatieTypeVoorbeeld { get; set; }
	}
}
