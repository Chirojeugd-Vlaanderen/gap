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
		[DataMember]
		public int ID { get; set; }

		[DataMember]
		public String Bus { get; set; }

        [DataMember]
        public int PostNr { get; set; }

		[DataMember]
		public int HuisNr { get; set; }

		[DataMember]
		public String Straat { get; set; }

		[DataMember]
		public String Gemeente { get; set; }

		[DataMember]
		public List<GewonePersoonInfo> Bewoners { get; set; }
    }
}
