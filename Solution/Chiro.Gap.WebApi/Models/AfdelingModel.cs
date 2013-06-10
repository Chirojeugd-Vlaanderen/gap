using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WebApi.Models
{
    public class AfdelingModel
    {
        public int Id { get; set; }
        public string Naam { get; set; }

        public virtual GroepModel Groep { get; set; }
        public virtual IQueryable<PersoonModel> Personen { get; set; }


        public AfdelingModel(AfdelingsJaar afdelingsJaar)
        {
            Id = afdelingsJaar.ID;
            Naam = afdelingsJaar.Afdeling.Naam;

        }
    }
}