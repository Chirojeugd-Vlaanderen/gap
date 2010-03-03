// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Model gebruikt om iemand een nieuwe communicatievorm te geven:
    /// telefoonnummer, mailadres, enz.
    /// </summary>
    public class NieuweCommVormModel : MasterViewModel 
    {
        /// <summary>
        /// ID van GelieerdePersoon waarvoor aangeklikt dat
        /// hij/zij een extra adres nodig heeft
        /// </summary>
        public GelieerdePersoon Aanvrager { get; set; }

        /// <summary>
        /// Nieuwe communicatievorm (telefoonnummer, mailadres, ...)
        /// voor de gegeven gelieerde personen
        /// </summary>
        public CommunicatieVorm NieuweCommVorm { get; set; }

        public int geselecteerdeCommVorm { get; set; }

        public IEnumerable<CommunicatieType> Types { get; set; }

        /// <summary>
        /// Standaardconstructor - creëert leeg NieuweCommVorm
        /// </summary>
        public NieuweCommVormModel()
        {
            Aanvrager = new GelieerdePersoon();
            NieuweCommVorm = new CommunicatieVorm();
        }

        public NieuweCommVormModel(GelieerdePersoon aanvrager, IEnumerable<CommunicatieType> types) : this()
        {
            Aanvrager = aanvrager;
            Types = types;
            NieuweCommVorm = new CommunicatieVorm();
        }
    }
}
