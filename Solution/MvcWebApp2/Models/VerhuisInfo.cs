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
        /// VanAdresID bevat het AdresID van het oorspronkelijke
        /// adres.
        /// </summary>
        public int VanAdresID { get; set; }

        /// <summary>
        /// NaarAdres bevat adresgegevens van het nieuwe adres.
        /// Aan NaarAdres zijn de bewoners van het VanAdres gekoppeld
        /// die de gebruiker mag zien.
        /// 
        /// (Het AdresID van NaarAdres is van geen belang, want dat
        /// kennen we niet aan de UI-kant)
        /// </summary>
        public Adres NaarAdres { get; set; }

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
            NaarAdres = new Adres();
            GelieerdePersoonIDs = new List<int>();
        }

        /// <summary>
        /// Creeert verhuisinfo voor alle (zichtbare) bewoners van het
        /// adres met gegeven ID
        /// </summary>
        /// <param name="adresID">adresID waarvan sprake</param>
        public VerhuisInfo(int adresID)
        {
            VanAdresID = adresID;
            NaarAdres = ServiceCalls.GelieerdePersonen.AdresMetBewonersOphalen(adresID);

            // Standaard verhuist iedereen mee.
            GelieerdePersoonIDs = (
                from PersoonsAdres pa in NaarAdres.PersoonsAdres
                select pa.GelieerdePersoon.ID).ToList<int>();
        }

        /// <summary>
        /// Haalt de bewoners van het VanAdres opnieuw op, en
        /// plakt die aan het NaarAdres
        /// </summary>
        public void HerstelBewoners()
        {
            Adres vanAdres = ServiceCalls.GelieerdePersonen.AdresMetBewonersOphalen(VanAdresID);
            
            IList<PersoonsAdres> verhuizers = (from PersoonsAdres pa in vanAdres.PersoonsAdres
                                              select pa).ToList();

            foreach (PersoonsAdres v in verhuizers)
            {
                vanAdres.PersoonsAdres.Remove(v);
                NaarAdres.PersoonsAdres.Add(v);
            }
        }
    }
}
