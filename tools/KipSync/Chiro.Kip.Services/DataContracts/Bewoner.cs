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
		/// De persoon die woont
		/// </summary>
		[DataMember]
		public Persoon Persoon { get; set; }

		/// <summary>
		/// Hoedanigheid van het adres mbt de persoon (kot, thuis, werk,...)
		/// </summary>
		[DataMember]
		public AdresTypeEnum AdresType { get; set; }
	}
}
