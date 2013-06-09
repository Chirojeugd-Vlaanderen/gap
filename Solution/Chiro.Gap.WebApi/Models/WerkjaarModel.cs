using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WebApi.Models
{
    public class WerkjaarModel
    {
        public WerkjaarModel(GroepsWerkJaar groepsWerkJaar)
        {
            Id = groepsWerkJaar.ID;
            WerkJaar = groepsWerkJaar.WerkJaar;
        }

        public int Id { get; set; }
        public int WerkJaar { get; set; }

        public virtual GroepModel Groep { get; set; }


        // public virtual IQueryable<LidModel> Leden { get; set; }
        //public virtual ICollection<AfdelingsJaar> AfdelingsJaar { get; set; }
        //public virtual ICollection<Uitstap> Uitstap { get; set; }
        //public virtual ICollection<Abonnement> Abonnement { get; set; }
    }
}