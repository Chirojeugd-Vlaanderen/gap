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
    public class NieuwAdresInfo : MasterViewModel 
    {
        // ID van GelieerdePersoon waarvoor aangeklikt dat
        // hij/zij een extra adres nodig heeft
        public int AanvragerID { get; set; }

        /// <summary>
        /// Lijst met mogelijke bewoners voor het nieuwe adres
        /// </summary>
        public IList<Persoon> MogelijkeBewoners { get; set; }

        /// <summary>
        /// Nieuw adres voor de gegeven gelieerde personen
        /// </summary>
        public Adres NieuwAdres { get; set; }

        /// <summary>
        /// ID's van Personen aan wie het adres gekoppeld moet
        /// worden.  (In de meeste gevallen zal AanvragerID daarbij
        /// inzitten.)
        /// </summary>
        public List<int> PersoonIDs { get; set; }

        /// <summary>
        /// Standaardconstructor - creeert lege NieuwAdresInfo
        /// </summary>
        public NieuwAdresInfo()
        {
            AanvragerID = 0;
            MogelijkeBewoners = new List<Persoon>();
            NieuwAdres = new Adres();
            PersoonIDs = new List<int>();
        }

        public NieuwAdresInfo(int aanvragerID): this()
        {
            AanvragerID = aanvragerID;
            MogelijkeBewoners = ServiceHelper.CallService<IGelieerdePersonenService, IList<Persoon>>(l => l.HuisGenotenOphalen(aanvragerID));

            // FIXME: overkill om aan het persoonID van de aanvragerID op te vragen
            // (persoonID van de aanvrager al selecteren)
            PersoonIDs.Add(ServiceHelper.CallService<IGelieerdePersonenService, GelieerdePersoon>(l => l.PersoonOphalenMetDetails(aanvragerID)).Persoon.ID);
        }
    }
}
