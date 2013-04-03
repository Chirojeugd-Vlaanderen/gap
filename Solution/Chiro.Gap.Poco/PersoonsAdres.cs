using System.Collections.Generic;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Poco.Model
{
    public partial class PersoonsAdres: BasisEntiteit
    {
        public PersoonsAdres()
        {
            this.GelieerdePersoon = new HashSet<GelieerdePersoon>();
        }
    
        public string Opmerking { get; set; }
        public int AdresTypeInt { get; set; }
        public override byte[] Versie { get; set; }
        public override int ID { get; set; }
    
        public virtual Adres Adres { get; set; }
        public virtual Persoon Persoon { get; set; }
        public virtual ICollection<GelieerdePersoon> GelieerdePersoon { get; set; }

        /// <summary>
        /// Een enumwaarde voor het type adres (bv. 'kotadres')
        /// </summary>
        public AdresTypeEnum AdresType
        {
            get { return (AdresTypeEnum) AdresTypeInt; }
            set { AdresTypeInt = (int) value; }
        }

    }
    
}
