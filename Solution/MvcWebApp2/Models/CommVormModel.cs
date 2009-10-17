using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Cg2.Adf.ServiceModel;
using Cg2.Orm;
using Cg2.ServiceContracts;


namespace MvcWebApp2.Models
{
    /// <summary>
    /// Model gebruikt om iemand een nieuw adres te geven.
    /// </summary>
    public class CommVormModel : MasterViewModel 
    {
        // ID van GelieerdePersoon waarvoor aangeklikt dat
        // hij/zij een extra adres nodig heeft
        public int AanvragerID { get; set; }

        /// <summary>
        /// Nieuw adres voor de gegeven gelieerde personen
        /// </summary>
        public CommunicatieVorm NieuweCommVorm { get; set; }

        /// <summary>
        /// Standaardconstructor - creeert lege NieuwAdresInfo
        /// </summary>
        public CommVormModel()
        {
            AanvragerID = 0;
            NieuweCommVorm = new CommunicatieVorm();
        }

        public CommVormModel(int aanvragerID) : this()
        {
            AanvragerID = aanvragerID;
            NieuweCommVorm = new CommunicatieVorm();
        }
    }
}
