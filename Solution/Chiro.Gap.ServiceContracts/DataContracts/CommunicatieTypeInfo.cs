// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// DataContract voor summiere info over communicatietypes
	/// </summary>
	[DataContract]
	public class CommunicatieTypeInfo
	{
		/// <summary>
		/// Uniek identificatienummer
		/// </summary>
		[DataMember]
		public int ID { get; set; }

		/// <summary>
		/// De 'naam' van het communicatietype
		/// </summary>
		[DataMember]
		public string Omschrijving { get; set; }

		/// <summary>
		/// Geeft aan of iemand toestemming moet geven voor Chirojeugd Vlaanderen
		/// waarden voor dit communicatietype mag gebruiken
		/// </summary>
		[DataMember]
		public bool IsOptIn { get; set; }

		/// <summary>
		/// Een regular expression die aangeeft welke vorm de waarde voor dat type moet hebben
		/// </summary>
		[DataMember]
		public string Validatie { get; set; }

		/// <summary>
		/// Een voorbeeld van een communicatievorm die volgens de validatieregels gestructureerd is
		/// </summary>
		[DataMember]
		public string Voorbeeld { get; set; }
	}
}
