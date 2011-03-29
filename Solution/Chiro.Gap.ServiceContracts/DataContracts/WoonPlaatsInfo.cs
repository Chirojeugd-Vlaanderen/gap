// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// DataContract voor summiere info over iemands woonplaats
	/// </summary>
	[DataContract]
	public class WoonPlaatsInfo
	{
		/// <summary>
		/// ID van de woonplaats
		/// </summary>
		[DataMember]
		public int ID { get; set; }

		/// <summary>
		/// Het postnummer van de gemeente waar de woonplaats zich bevindt
		/// </summary>
		[DataMember]
		public int PostNummer { get; set; }

		/// <summary>
		/// Naam van de woonplaats: (deel)gemeente/gehucht/...
		/// </summary>
		[DataMember]
		public String Naam { get; set; }
	}
}
