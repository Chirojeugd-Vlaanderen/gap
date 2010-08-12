using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Kip.Services.DataContracts
{
	/// <summary>
	/// Datacontract voor een 'bewoner'
	/// </summary>
	[DataContract]
	public class Bewoner
	{
		/// <summary>
		/// AD-nummer van de bewoner
		/// </summary>
		[DataMember]
		public int AdNummer { get; set; }

		/// <summary>
		/// Adrestype
		/// </summary>
		[DataMember]
		public AdresTypeEnum AdresType { get; set; }
	}
}
