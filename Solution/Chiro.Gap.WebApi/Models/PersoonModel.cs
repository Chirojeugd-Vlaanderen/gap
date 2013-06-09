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

        public virtual GroepModel Groep { get; set; }

        public PersoonModel(GelieerdePersoon gelieerdePersoon)
        {
            Id = gelieerdePersoon.ID;
            Naam = gelieerdePersoon.Persoon.Naam;
            Voornaam = gelieerdePersoon.Persoon.VoorNaam;
            GeboorteDatum = gelieerdePersoon.Persoon.GeboorteDatum;
        }

    }
}