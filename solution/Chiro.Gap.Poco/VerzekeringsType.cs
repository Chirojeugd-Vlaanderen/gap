using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public partial class VerzekeringsType
    {
        public VerzekeringsType()
        {
            this.PersoonsVerzekering = new HashSet<PersoonsVerzekering>();
        }
    
        public int ID { get; set; }
        public string Code { get; set; }
        public string Naam { get; set; }
        public string Omschrijving { get; set; }
        public bool EnkelLeden { get; set; }
        public bool TotEindeWerkJaar { get; set; }
    
        public virtual ICollection<PersoonsVerzekering> PersoonsVerzekering { get; set; }
    }
    
}
