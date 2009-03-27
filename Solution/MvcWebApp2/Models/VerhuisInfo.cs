using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cg2.Orm;

namespace MvcWebApp2.Models
{
    /// <summary>
    /// Model voor verhuis:
    ///   . op een adres A wonen personen
    ///   . een aantal van die personen verhuizen naar een nieuw adres B
    /// </summary>
    public class VerhuisInfo
    {
        /// <summary>
        /// Adres bevat het AdresID van het oorspronkelijke adres,
        /// en de adresgegevens van het nieuwe adres.
        /// 
        /// (Aan de UI kant kunnen we immers niet gemakkelijk adressen
        /// opzoeken; ervoor zorgen dat AdresID goed zit, is iets dat
        /// de service zal moeten doen.  Vandaar dit oneigenlijke gebruik
        /// van AdresID)
        /// 
        /// Aan Adres zijn alle gelieerde personen gekoppeld die de
        /// gebruiker mag zien.
        /// 
        /// </summary>
        public Adres Adres { get; set; }

        /// <summary>
        /// Het lijstje GelieerdePersoonIDs bevat de GelieerdePersoonID's van
        /// de personen die mee verhuizen.
        /// </summary>
        public List<int> GelieerdePersoonIDs { get; set; }

        /// <summary>
        /// Saaie standaardconstructor
        /// </summary>
        public VerhuisInfo()
        {
            Adres = new Adres();
            GelieerdePersoonIDs = new List<int>();
        }

        /// <summary>
        /// Creeert verhuisinfo voor alle (zichtbare) bewoners van het
        /// adres met gegeven ID
        /// </summary>
        /// <param name="adresID">adresID waarvan sprake</param>
        public VerhuisInfo(int adresID)
        {
            using (GelieerdePersonenServiceReference.GelieerdePersonenServiceClient service = new MvcWebApp2.GelieerdePersonenServiceReference.GelieerdePersonenServiceClient())
            {
                Adres = service.AdresMetBewonersOphalen(adresID);
            }

            // Standaard verhuist iedereen mee.
            GelieerdePersoonIDs = (
                from PersoonsAdres pa in Adres.PersoonsAdres
                select pa.GelieerdePersoonID).ToList<int>();
        }
    }
}
