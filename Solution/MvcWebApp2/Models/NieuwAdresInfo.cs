using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Capgemini.Adf.ServiceModel;
using Cg2.Orm;
using Cg2.ServiceContracts;


namespace MvcWebApp2.Models
{
    /// <summary>
    /// Model gebruikt om iemand een nieuw adres te geven.
    /// </summary>
    public class NieuwAdresInfo : MasterViewModel 
    {
        // ID van GelieerdePersoon waarvoor aangeklikt dat
        // hij/zij een extra adres nodig heeft
        public int AanvragerID { get; set; }

        /// <summary>
        /// Lijst met mogelijke bewoners voor het nieuwe adres
        /// </summary>
        public IList<GelieerdePersoon> MogelijkeBewoners { get; set; }

        /// <summary>
        /// Nieuw adres voor de gegeven gelieerde personen
        /// </summary>
        public Adres NieuwAdres { get; set; }

        /// <summary>
        /// ID's van GelieerdePersonen aan wie het adres gekoppeld moet
        /// worden.  (In de meeste gevallen zal AanvragerID daarbij
        /// inzitten.)
        /// </summary>
        public List<int> GelieerdePersoonIDs { get; set; }

        /// <summary>
        /// Standaardconstructor - creeert lege NieuwAdresInfo
        /// </summary>
        public NieuwAdresInfo()
        {
            AanvragerID = 0;
            MogelijkeBewoners = new List<GelieerdePersoon>();
            NieuwAdres = new Adres();
            GelieerdePersoonIDs = new List<int>();
        }

        public NieuwAdresInfo(int aanvragerID): this()
        {
            AanvragerID = aanvragerID;
            MogelijkeBewoners = ServiceHelper.CallService<IGelieerdePersonenService, IList<GelieerdePersoon>>(l => l.HuisGenotenOphalen(aanvragerID));

            // Standaard enkel een nieuw adres voor de aanvrager
            GelieerdePersoonIDs.Add(AanvragerID);
        }
    }
}
