using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.ServiceContracts
{
	/// <summary>
	/// DataContract voor informatie mbt functies
	/// </summary>
	[DataContract]
	public class FunctieInfo
	{
		[DataMember]
		public int ID { get; set; }
		[DataMember]
		public string Code { get; set; }
		[DataMember]
		public string Naam { get; set; }
		[DataMember]
		public int MaxAantal { get; set; }
		[DataMember]
		public int MinAantal { get; set; }
		[DataMember]
		public int WerkJaarVan { get; set; }
		[DataMember]
		public int WerkJaarTot { get; set; }
		[DataMember]
		public bool IsNationaalBepaald { get; set; }
	}
}
