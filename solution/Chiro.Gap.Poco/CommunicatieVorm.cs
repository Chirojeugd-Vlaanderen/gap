using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Poco.Model
{
    public partial class CommunicatieVorm: BasisEntiteit, ICommunicatie
    {
        public string Nota { get; set; }
        public string Nummer { get; set; }


        /// <summary>
        /// De regular expressie waar de waarde voor dit communicatietype
        /// aan moet voldoen
        /// </summary>
        string ICommunicatie.CommunicatieTypeValidatie
        {
            get
            {
                return CommunicatieType.Validatie;
            }
            set
            {
                CommunicatieType.Validatie = value;
            }
        }

        public override int ID { get; set; }
        public bool IsGezinsgebonden { get; set; }
        public bool Voorkeur { get; set; }
        public override byte[] Versie { get; set; }
        public bool IsVoorOptIn { get; set; }
    
        public virtual GelieerdePersoon GelieerdePersoon { get; set; }
        public virtual CommunicatieType CommunicatieType { get; set; }
    }
    
}