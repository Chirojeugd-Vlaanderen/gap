using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Cg2.Orm;

namespace Cg2.ServiceContracts
{
    [DataContract]
    public class PersoonInfo
    {
        [DataMember]
        public int? AdNummer { get; set; }

        [DataMember]
        public int GelieerdePersoonID { get; set; }

        [DataMember]
        public string VolledigeNaam { get; set; }

        [DataMember]
        public DateTime? GeboorteDatum { get; set; }

        [DataMember]
        public GeslachtsType Geslacht { get; set; }

        [DataMember]
        public Boolean IsLid { get; set; }

    }
}
