using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public partial class Deelnemer
    {
        public Deelnemer()
        {
            this.UitstapWaarvoorVerantwoordelijk = new HashSet<Uitstap>();
        }
    
        public int ID { get; set; }
        public bool IsLogistieker { get; set; }
        public bool HeeftBetaald { get; set; }
        public bool MedischeFicheOk { get; set; }
        public string Opmerkingen { get; set; }
        public byte[] Versie { get; set; }
    
        public virtual GelieerdePersoon GelieerdePersoon { get; set; }
        public virtual Uitstap Uitstap { get; set; }
        public virtual ICollection<Uitstap> UitstapWaarvoorVerantwoordelijk { get; set; }
    }
    
}
