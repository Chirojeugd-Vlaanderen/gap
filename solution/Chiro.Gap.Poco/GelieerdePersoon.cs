using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public partial class GelieerdePersoon
    {
        public GelieerdePersoon()
        {
            this.Communicatie = new HashSet<CommunicatieVorm>();
            this.Lid = new HashSet<Lid>();
            this.Categorie = new HashSet<Categorie>();
            this.Plaats = new HashSet<Plaats>();
            this.Deelnemer = new HashSet<Deelnemer>();
            this.Abonnement = new HashSet<Abonnement>();
        }
    
        public int ID { get; set; }
        public int ChiroLeefTijd { get; set; }
        public byte[] Versie { get; set; }
    
        public virtual Groep Groep { get; set; }
        public virtual ICollection<CommunicatieVorm> Communicatie { get; set; }
        public virtual Persoon Persoon { get; set; }
        public virtual ICollection<Lid> Lid { get; set; }
        public virtual ICollection<Categorie> Categorie { get; set; }
        public virtual PersoonsAdres PersoonsAdres { get; set; }
        public virtual ICollection<Plaats> Plaats { get; set; }
        public virtual ICollection<Deelnemer> Deelnemer { get; set; }
        public virtual ICollection<Abonnement> Abonnement { get; set; }
    }
    
}
