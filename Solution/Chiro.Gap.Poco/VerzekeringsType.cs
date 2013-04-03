using System.Collections.Generic;
using Chiro.Cdf.Poco;

namespace Chiro.Gap.Poco.Model
{
    public partial class VerzekeringsType: BasisEntiteit
    {
        public VerzekeringsType()
        {
            this.PersoonsVerzekering = new HashSet<PersoonsVerzekering>();
        }

        public override byte[] Versie { get; set; }
        public override int ID { get; set; }
        public string Code { get; set; }
        public string Naam { get; set; }
        public string Omschrijving { get; set; }
        public bool EnkelLeden { get; set; }
        public bool TotEindeWerkJaar { get; set; }
    
        public virtual ICollection<PersoonsVerzekering> PersoonsVerzekering { get; set; }
    }
    
}
