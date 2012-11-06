namespace Chiro.Gap.Poco.Model
{
    public partial class PersoonsVerzekering
    {
        public System.DateTime Van { get; set; }
        public System.DateTime Tot { get; set; }
        public int ID { get; set; }
        public byte[] Versie { get; set; }
    
        public virtual Persoon Persoon { get; set; }
        public virtual VerzekeringsType VerzekeringsType { get; set; }
    }
    
}
