using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WebApi.Models
{
    public class GroepModel
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public string StamNummer { get; set; }

        public virtual IQueryable<PersoonModel> Personen { get; set; }
        public virtual IQueryable<WerkjaarModel> Werkjaren { get; set; }

        public GroepModel(Groep groep)
        {
            Id = groep.ID;
            Naam = groep.Naam;
            StamNummer = groep.Code;
        }
    }
}