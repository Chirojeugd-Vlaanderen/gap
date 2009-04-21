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
        /// VanAdresMetBewoners bevat het adres waarvan verhuisd wordt,
        /// met daaraan gekoppeld alle bewoners die de aangelogde gebruiker
        /// mag zien.
        /// </summary>
        public Adres VanAdresMetBewoners { get; set; }

        /// <summary>
        /// NaarAdres bevat adresgegevens van het nieuwe adres.
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
        /// <param name="vanAdresID">relevante vanAdresID</param>
        public VerhuisInfo(int vanAdresID)
        {
            VanAdresMetBewoners = ServiceCalls.GelieerdePersonen.AdresMetBewonersOphalen(vanAdresID);

            // Bij de constructie van verhuisinfo zijn vanadres naaradres
            // dezelfde.  Van zodra er een postback gebeurt van het form,
            // wordt NaarAdres gebind met de gegevens uit het form; op dat
            // moment wordt een nieuwe instantie van het naaradres
            // gemaakt.

            NaarAdres = VanAdresMetBewoners;

            // Standaard verhuist iedereen mee.
            GelieerdePersoonIDs = (
                from PersoonsAdres pa in VanAdresMetBewoners.PersoonsAdres
                select pa.GelieerdePersoon.ID).ToList<int>();
        }

        /// <summary>
        /// Haalt de gegevens van het vanadres opnieuw op, op basis van
        /// VanAdresMetBewoners.ID.
        /// </summary>
        public void HerstelVanAdres()
        {
            VanAdresMetBewoners = ServiceCalls.GelieerdePersonen.AdresMetBewonersOphalen(VanAdresMetBewoners.ID);          
        }
    }
}
