using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WebApi.Models
{
    public class WerkjaarModel
    {
        public int Id { get; set; }
        public int WerkJaar { get; set; }
        
        
        // public virtual IQueryable<LiidModel> Leden { get; set; }
        //public virtual ICollection<AfdelingsJaar> AfdelingsJaar { get; set; }
        //public virtual ICollection<Uitstap> Uitstap { get; set; }
        //public virtual ICollection<Abonnement> Abonnement { get; set; }

        public WerkjaarModel(GroepsWerkJaar groepsWerkJaar)
        {
            Id = groepsWerkJaar.ID;
            WerkJaar = groepsWerkJaar.WerkJaar;
        }
    }
}