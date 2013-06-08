using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chiro.Gap.WebApi.Models
{
    public class PersoonModel
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public string Voornaam { get; set; }
        public DateTime? GeboorteDatum { get; set; }

        public int GroepId { get; set; }

    }
}