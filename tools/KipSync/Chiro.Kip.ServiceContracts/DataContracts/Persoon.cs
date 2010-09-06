using System;
using System.Runtime.Serialization;

namespace Chiro.Kip.ServiceContracts.DataContracts
{

    [DataContract]
    public class Persoon
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public int? AdNummer { get; set; }

        [DataMember]
        public string VoorNaam { get; set; }

        [DataMember]
        public string Naam { get; set; }

        [DataMember]
        public DateTime? GeboorteDatum { get; set; }

        [DataMember]
        public DateTime? SterfDatum { get; set; }

        [DataMember]
        public GeslachtsEnum Geslacht { get; set; }

    }
}
