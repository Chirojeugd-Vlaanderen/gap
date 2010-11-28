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
	public class CommunicatieDetail : CommunicatieInfo, ICommunicatie
	{
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
