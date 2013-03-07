using System.Collections.Generic;
using Chiro.Cdf.Poco;

namespace Chiro.Gap.Poco.Model
{
    public partial class CommunicatieType: BasisEntiteit
    {
        public CommunicatieType()
        {
            this.CommunicatieVorm = new HashSet<CommunicatieVorm>();
        }
    
        public string Omschrijving { get; set; }
        public string Validatie { get; set; }
        public override int ID { get; set; }
        public override byte[] Versie { get; set; }
        public string Voorbeeld { get; set; }
        public bool IsOptIn { get; set; }
    
        public virtual ICollection<CommunicatieVorm> CommunicatieVorm { get; set; }
    }
    
}
