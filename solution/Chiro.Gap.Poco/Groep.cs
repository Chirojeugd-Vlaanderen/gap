using System;
using System.Collections.Generic;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Poco.Model
{
    public abstract partial class Groep: BasisEntiteit
    {
        public Groep()
        {
            GelieerdePersoon = new HashSet<GelieerdePersoon>();
            GroepsWerkJaar = new HashSet<GroepsWerkJaar>();
            GebruikersRecht = new HashSet<GebruikersRecht>();
            Categorie = new HashSet<Categorie>();
            Functie = new HashSet<Functie>();
            BivakPlaats = new HashSet<Plaats>();
        }
    
        public string Naam { get; set; }
        public string Code { get; set; }
        public int? OprichtingsJaar { get; set; }
        public DateTime? StopDatum { get; set; }
        public string WebSite { get; set; }
        public byte[] Logo { get; set; }
        public override int ID { get; set; }
        public override byte[] Versie { get; set; }
    
        public virtual ICollection<GelieerdePersoon> GelieerdePersoon { get; set; }
        public virtual ICollection<GroepsWerkJaar> GroepsWerkJaar { get; set; }
        public virtual ICollection<GebruikersRecht> GebruikersRecht { get; set; }
        public virtual ICollection<Categorie> Categorie { get; set; }
        public virtual ICollection<Functie> Functie { get; set; }
        public virtual ICollection<Plaats> BivakPlaats { get; set; }

        public abstract Niveau Niveau { get; }


    }
    
}
