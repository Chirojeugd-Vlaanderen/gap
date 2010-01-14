using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Chiro.Gap.Orm;

namespace Chiro.Gap.ServiceContracts
{
	[DataContract]
	public class AdresInfo
	{
		private string _bus;

		[DataMember]
		public int ID { get; set; }

		[DataMember]
		public string Bus
		{
			// Vervangt eventuele null door String.Empty
			// Zie ticket #202: https://develop.chiro.be/trac/cg2/ticket/202
			get { return _bus; }
			set { _bus = value ?? String.Empty; }
		}

		[DataMember]
		public int PostNr { get; set; }

		[DataMember]
		public int HuisNr { get; set; }

		[DataMember]
		public String Straat { get; set; }

		[DataMember]
		public String Gemeente { get; set; }

		[DataMember]
		public List<BewonersInfo> Bewoners { get; set; }
	}
}
