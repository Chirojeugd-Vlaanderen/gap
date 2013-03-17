// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// DataContract voor summiere info over aangesloten leden/leiding en hun afdeling(en)
	/// </summary>
	[DataContract]
	public class LidAfdelingInfo
	{
        /// <summary>
        /// Instantieert een LidAfdelingInfo-object
        /// </summary>
		public LidAfdelingInfo()
		{
			AfdelingsJaarIDs = new List<int>();
		}

		/// <summary>
		/// Volledige naam van het lid
		/// </summary>
		[DataMember]
		public string VolledigeNaam { get; set; }

		/// <summary>
		/// ID's van de afdelingsjaren gekoppeld aan het lid
		/// </summary>
		[DataMember]
		public IList<int> AfdelingsJaarIDs { get; set; }

		/// <summary>
		/// Type lid (kind/leiding)
		/// </summary>
		[DataMember]
		public LidType Type { get; set; }
	}
}
