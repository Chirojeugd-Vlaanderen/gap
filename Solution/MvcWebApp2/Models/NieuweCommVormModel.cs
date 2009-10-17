using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Cg2.Adf.ServiceModel;
using Cg2.Orm;
using Cg2.ServiceContracts;
using System.Web.Mvc;


namespace MvcWebApp2.Models
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
