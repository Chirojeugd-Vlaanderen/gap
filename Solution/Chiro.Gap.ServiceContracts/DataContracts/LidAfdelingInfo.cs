using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Chiro.Gap.Domain;
using Chiro.Gap.Orm;

namespace Chiro.Gap.ServiceContracts
{
	[DataContract]
	public class LidAfdelingInfo
	{
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
