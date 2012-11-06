using System;
using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public abstract partial class Groep
    {
        public Groep()
        {
            this.GelieerdePersoon = new HashSet<GelieerdePersoon>();
            this.GroepsWerkJaar = new HashSet<GroepsWerkJaar>();
            this.GebruikersRecht = new HashSet<GebruikersRecht>();
            this.Categorie = new HashSet<Categorie>();
            this.Functie = new HashSet<Functie>();
            this.BivakPlaats = new HashSet<Plaats>();
        }
    
        public string Naam { get; set; }
        public string Code { get; set; }
        public Nullable<int> OprichtingsJaar { get; set; }
        public string WebSite { get; set; }
        public byte[] Logo { get; set; }
        public int ID { get; set; }
        public byte[] Versie { get; set; }
    
        public virtual ICollection<GelieerdePersoon> GelieerdePersoon { get; set; }
        public virtual ICollection<GroepsWerkJaar> GroepsWerkJaar { get; set; }
        public virtual ICollection<GebruikersRecht> GebruikersRecht { get; set; }
        public virtual ICollection<Categorie> Categorie { get; set; }
        public virtual ICollection<Functie> Functie { get; set; }
        public virtual ICollection<Plaats> BivakPlaats { get; set; }
    }
    
}
