using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using System.Web.Mvc;


namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Model gebruikt om iemand een nieuw adres te geven.
    /// </summary>
    public class NieuweCommVormModel : MasterViewModel 
    {
        // ID van GelieerdePersoon waarvoor aangeklikt dat
        // hij/zij een extra adres nodig heeft
        public GelieerdePersoon Aanvrager { get; set; }

        /// <summary>
        /// Nieuw adres voor de gegeven gelieerde personen
        /// </summary>
        public CommunicatieVorm NieuweCommVorm { get; set; }
        public int geselecteerdeCommVorm { get; set; }

        public IEnumerable<CommunicatieType> Types { get; set; }

        /// <summary>
        /// Standaardconstructor - creeert lege NieuwAdresInfo
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
