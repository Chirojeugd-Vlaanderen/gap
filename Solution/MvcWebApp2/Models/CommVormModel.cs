using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Orm;
using Cg2.ServiceContracts;


namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Model gebruikt om iemand een nieuw adres te geven.
    /// </summary>
    public class CommVormModel : MasterViewModel 
    {
        // ID van GelieerdePersoon waarvoor aangeklikt dat
        // hij/zij een extra adres nodig heeft
        public GelieerdePersoon Aanvrager { get; set; }

        /// <summary>
        /// Nieuw adres voor de gegeven gelieerde personen
        /// </summary>
        public CommunicatieVorm NieuweCommVorm { get; set; }

        /// <summary>
        /// Standaardconstructor - creeert lege NieuwAdresInfo
        /// </summary>
        public CommVormModel()
        {
            Aanvrager = new GelieerdePersoon();
            NieuweCommVorm = new CommunicatieVorm();
        }

        public CommVormModel(GelieerdePersoon aanvrager, CommunicatieVorm v) : this()
        {
            Aanvrager = aanvrager;
            NieuweCommVorm = v;
        }
    }
}
