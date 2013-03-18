using System;
using System.Collections.Generic;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Poco.Model
{
    public partial class Persoon: BasisEntiteit
    {
        public Persoon()
        {
            this.GelieerdePersoon = new HashSet<GelieerdePersoon>();
            this.PersoonsAdres = new HashSet<PersoonsAdres>();
            this.PersoonsVerzekering = new HashSet<PersoonsVerzekering>();
            this.Gav = new HashSet<Gav>();
        }
    
        public Nullable<int> AdNummer { get; set; }
        public string Naam { get; set; }
        public string VoorNaam { get; set; }
        public Nullable<System.DateTime> GeboorteDatum { get; set; }
        internal int GeslachtsInt { get; set; }
        public Nullable<System.DateTime> SterfDatum { get; set; }
        public override int ID { get; set; }
        public override byte[] Versie { get; set; }
        public bool AdInAanvraag { get; set; }
        public string SeNaam { get; set; }
        public string SeVoornaam { get; set; }
    
        public virtual ICollection<GelieerdePersoon> GelieerdePersoon { get; set; }
        public virtual ICollection<PersoonsAdres> PersoonsAdres { get; set; }
        public virtual ICollection<PersoonsVerzekering> PersoonsVerzekering { get; set; }
        public virtual ICollection<Gav> Gav { get; set; }

        /// <summary>
        /// Een enumwaarde voor het geslacht van de persoon
        /// </summary>
        public GeslachtsType Geslacht
        {
            get { return (GeslachtsType) GeslachtsInt; }
            set { GeslachtsInt = (int) value; }
        }

        /// <summary>
        /// Concatenatie van voornaam en naam
        /// </summary>
        public string VolledigeNaam
        {
            get { return String.Format("{0} {1}", VoorNaam, Naam); }
        }

    }
    
}
