using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    [DataContract]
    [KnownType(typeof(DeelnemerDetail))]
    public class DeelnemerInfo
    {
        [DataMember]
        public int DeelnemerID { get; set; }

        [DataMember]
        [DisplayName(@"Logistieke ploeg")]
        public bool IsLogistieker { get; set; }

        [DataMember]
        [DisplayName(@"Inschrijvingsgeld betaald")]
        public bool HeeftBetaald { get; set; }

        [DataMember]
        [DisplayName(@"Medische fiche OK")]
        public bool MedischeFicheOk { get; set; }

        [DataMember]
        [DataType(DataType.MultilineText)]
        public string Opmerkingen { get; set; }
    }
}