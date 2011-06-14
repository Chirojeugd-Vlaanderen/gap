// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// DataContract voor summiere info over straten
	/// </summary>
	[DataContract]
	public class StraatInfo
	{
		/// <summary>
		/// Het postnummer van de gemeente waar de straat ligt
		/// </summary>
		[DataMember]
		public int PostNummer { get; set; }

		/// <summary>
		/// De straatnaam zelf
		/// </summary>
		[DataMember]
		public String Naam { get; set; }
	}
}
