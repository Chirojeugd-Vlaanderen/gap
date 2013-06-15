using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WebApi.Models
{
    public class PersoonModel
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public string Voornaam { get; set; }
        public DateTime? GeboorteDatum { get; set; }
        public string Type { get; set; }

        public virtual GroepModel Groep { get; set; }
        public virtual ContactgegevenModel Contactgegevens { get; set; }
        public virtual IQueryable<AfdelingModel> Afdelingen { get; set; }

        private PersoonModel(GelieerdePersoon gelieerdePersoon)
        {
            Id = gelieerdePersoon.ID;
            Naam = gelieerdePersoon.Persoon.Naam;
            Voornaam = gelieerdePersoon.Persoon.VoorNaam;
            GeboorteDatum = gelieerdePersoon.Persoon.GeboorteDatum;
        }

        public PersoonModel(GelieerdePersoon gelieerdePersoon, GroepsWerkJaar groepsWerkJaar) : this(gelieerdePersoon)
        { 
            var lid = gelieerdePersoon.Lid.FirstOrDefault(l => Equals(l.GroepsWerkJaar, groepsWerkJaar));
            if (lid == null)
            {
                // dit jaar geen lid
                Type = "Uitgeschreven";
            }
            else if ( lid is Leiding)
            {
                Type = "Leiding";
            }
            else if (lid is Kind)
            {
                Type = "Lid";
            }
            else
            {
                // Wel in werkjaar, maar geen lid of leiding
                Type = "Ingeschreven";
            }
        }

        public PersoonModel(Leiding leiding) : this(leiding.GelieerdePersoon)
        {
            Type = "Leiding";
        }

        public PersoonModel(Kind kind) : this(kind.GelieerdePersoon)
        {
            Type = "Lid";
        }

        public PersoonModel(Lid lid) : this(lid.GelieerdePersoon)
        {
            Type = "Ingeschreven persoon";
        }

    }
}