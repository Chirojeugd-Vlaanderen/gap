using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WebApi.Models
{
    public class ContactgegevenModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Waarde { get; set; }

        public virtual PersoonModel Persoon { get; set; }

        public ContactgegevenModel(CommunicatieVorm communicatieVorm)
        {
            Id = communicatieVorm.ID;
            Type = communicatieVorm.CommunicatieType.Omschrijving;
            Waarde = communicatieVorm.Nummer;
        }
    }
}