// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract voor beperkte informatie over een functie
	/// </summary>
	[DataContract]
	[KnownType(typeof(FunctieDetail))]
	public class FunctieInfo
	{
		/// <summary>
		/// Uniek identificatienummer
		/// </summary>
		[DataMember]
		public int ID { get; set; }

		/// <summary>
		/// De afkorting van de functie
		/// </summary>
		[Verplicht, StringLengte(10), StringMinimumLengte(2)]
		[DataMember]
		public string Code { get; set; }

		/// <summary>
		/// De volledige naam van de functie
		/// </summary>
		[Verplicht, StringLengte(80), StringMinimumLengte(2)]
		[DataMember]
		public string Naam { get; set; }
	}
}
