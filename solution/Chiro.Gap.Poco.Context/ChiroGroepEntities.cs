using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Chiro.Cdf.Poco;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.Poco.Context
{
    public partial class ChiroGroepEntities : DbContext, IContext
    {
        public ChiroGroepEntities()
            : base("name=ChiroGroepEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Groep> Groep { get; set; }
        public DbSet<Persoon> Persoon { get; set; }
        public DbSet<CommunicatieVorm> CommunicatieVorm { get; set; }
        public DbSet<GelieerdePersoon> GelieerdePersoon { get; set; }
        public DbSet<Adres> Adres { get; set; }
        public DbSet<PersoonsAdres> PersoonsAdres { get; set; }
        public DbSet<GroepsWerkJaar> GroepsWerkJaar { get; set; }
        public DbSet<Lid> Lid { get; set; }
        public DbSet<Gav> Gav { get; set; }
        public DbSet<GebruikersRecht> GebruikersRecht { get; set; }
        public DbSet<Afdeling> Afdeling { get; set; }
        public DbSet<AfdelingsJaar> AfdelingsJaar { get; set; }
        public DbSet<OfficieleAfdeling> OfficieleAfdeling { get; set; }
        public DbSet<Categorie> Categorie { get; set; }
        public DbSet<CommunicatieType> CommunicatieType { get; set; }
        public DbSet<Functie> Functie { get; set; }
        public DbSet<StraatNaam> StraatNaam { get; set; }
        public DbSet<WoonPlaats> WoonPlaats { get; set; }
        public DbSet<PersoonsVerzekering> PersoonsVerzekering { get; set; }
        public DbSet<VerzekeringsType> VerzekeringsType { get; set; }
        public DbSet<Land> Land { get; set; }
        public DbSet<Plaats> Plaats { get; set; }
        public DbSet<Uitstap> Uitstap { get; set; }
        public DbSet<Deelnemer> Deelnemer { get; set; }
        public DbSet<Abonnement> Abonnement { get; set; }
        public DbSet<Publicatie> Publicatie { get; set; }
    }
}
