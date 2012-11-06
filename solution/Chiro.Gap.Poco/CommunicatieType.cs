using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public partial class CommunicatieType
    {
        public CommunicatieType()
        {
            this.CommunicatieVorm = new HashSet<CommunicatieVorm>();
        }
    
        public string Omschrijving { get; set; }
        public string Validatie { get; set; }
        public int ID { get; set; }
        public byte[] Versie { get; set; }
        public string Voorbeeld { get; set; }
        public bool IsOptIn { get; set; }
    
        public virtual ICollection<CommunicatieVorm> CommunicatieVorm { get; set; }
    }
    
}
