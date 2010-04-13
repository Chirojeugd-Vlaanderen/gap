using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Chiro.Gap.Orm;


namespace Chiro.Gap.ServiceContracts
{
	/// <summary>
	/// Datacontract voor gegevens van het afdelingsjaar.
	/// </summary>
	[DataContract]
	public class AfdelingsJaarDetail
	{
		[DataMember]
		public int AfdelingsJaarID { get; set; }
		[DataMember]
		public int AfdelingID { get; set; }
		[DataMember]
		public int OfficieleAfdelingID { get; set; }
		[DataMember]
		public int GeboorteJaarVan { get; set; }
		[DataMember]
		public int GeboorteJaarTot { get; set; }
		[DataMember]
		public GeslachtsType Geslacht { get; set; }
		[DataMember]
		public string VersieString { get; set; }
	}
}
