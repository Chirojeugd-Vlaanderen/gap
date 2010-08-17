using System;
using System.Runtime.Serialization;

namespace Chiro.Kip.Services.DataContracts
{
    [DataContract]
    public enum GeslachtsEnum
    {
        [EnumMember]
        Onbekend = 0,
        [EnumMember]
        Man = 1,
        [EnumMember]
        Vrouw = 2

    }

    [DataContract]
    public class Persoon
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public int? AdNr { get; set; }

        [DataMember]
        public string Voornaam { get; set; }

        [DataMember]
        public string Naam { get; set; }

        [DataMember]
        public DateTime? Geboortedatum { get; set; }

        [DataMember]
        public DateTime? Sterfdatum { get; set; }

        [DataMember]
        public GeslachtsEnum Geslacht { get; set; }

    }
}
