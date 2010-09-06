using System.Runtime.Serialization;

namespace Chiro.Kip.ServiceContracts.DataContracts
{
    [DataContract]
    public class Adres
    {
        [DataMember]
        public string Straat { get; set; }

        [DataMember]
        public int? HuisNr { get; set; }

        [DataMember]
        public string Bus { get; set; }

        [DataMember]
        public int PostNr { get; set; }

        [DataMember]
        public string PostCode { get; set; }

        [DataMember]
        public string WoonPlaats { get; set; }

        [DataMember]
        public string Land { get; set; }

    }
}
