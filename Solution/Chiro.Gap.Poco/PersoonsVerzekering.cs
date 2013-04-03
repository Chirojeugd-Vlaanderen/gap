using Chiro.Cdf.Poco;

namespace Chiro.Gap.Poco.Model
{
    public partial class PersoonsVerzekering: BasisEntiteit
    {
        public System.DateTime Van { get; set; }
        public System.DateTime Tot { get; set; }
        public override int ID { get; set; }
        public override byte[] Versie { get; set; }
    
        public virtual Persoon Persoon { get; set; }
        public virtual VerzekeringsType VerzekeringsType { get; set; }
    }
    
}
