using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;


namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Model gebruikt om iemand een nieuw adres te geven.
    /// </summary>
    public class NieuwAdresModel : MasterViewModel 
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
        /// Adrestype voor het nieuwe adres
        /// </summary>
        public AdresTypeEnum NieuwAdresType { get; set; }

        /// <summary>
        /// ID's van Personen aan wie het adres gekoppeld moet
        /// worden.  (In de meeste gevallen zal AanvragerID daarbij
        /// inzitten.)
        /// </summary>
        public List<int> PersoonIDs { get; set; }

        /// <summary>
        /// Standaardconstructor - creeert lege NieuwAdresInfo
        /// </summary>
        public NieuwAdresModel()
        {
            AanvragerID = 0;
            MogelijkeBewoners = new List<Persoon>();
            NieuwAdres = new Adres();
            PersoonIDs = new List<int>();
        }

        /// <summary>
        /// Haalt eventuele mogelijke bewoners van het nieuwe
        /// adres opnieuw op uit de database, op basis van
        /// AanvragerID
        /// </summary>
        public void HerstelMogelijkeBewoners()
        {
            MogelijkeBewoners = ServiceHelper.CallService<IGelieerdePersonenService, IList<Persoon>>(l => l.HuisGenotenOphalen(AanvragerID));
        }
    }
}
