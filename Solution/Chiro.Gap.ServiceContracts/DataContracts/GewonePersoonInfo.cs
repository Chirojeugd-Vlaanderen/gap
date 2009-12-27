using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Chiro.Gap.Orm;

namespace Chiro.Gap.ServiceContracts
{
    [DataContract]
    public class GewonePersoonInfo
    {
        [DataMember]
        public int? AdNummer { get; set; }

		[DataMember]
		public int PersoonID { get; set; }

        [DataMember]
        public string VolledigeNaam { get; set; }

        [DataMember]
        public DateTime? GeboorteDatum { get; set; }

        [DataMember]
        public GeslachtsType Geslacht { get; set; }
    }
}
