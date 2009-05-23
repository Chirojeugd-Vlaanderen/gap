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
    /// Model voor verhuis:
    ///   . op een adres A wonen personen
    ///   . een aantal van die personen verhuizen naar een nieuw adres B
    /// </summary>
    public class VerhuisInfo : MasterViewModel 
    {
        /// <summary>
        /// ID van GelieerdePersoon die graag zou verhuizen.
        /// Wordt bewaard om achteraf terug naar de details van de
        /// aanvrager te kunnen redirecten.
        /// </summary>
        public int AanvragerID { get; set; }

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
        /// Het lijstje persoonIDs bevat de GelieerdePersoonID's van
        /// de personen die mee verhuizen.
        /// </summary>
        public List<int> PersoonIDs { get; set; }

        /// <summary>
        /// Saaie standaardconstructor
        /// </summary>
        public VerhuisInfo()
        {
            NaarAdres = new Adres();
            PersoonIDs = new List<int>();
        }

        // TODO: Ik ben er niet zeker van of het model de service rechtstreeks mag aanspreken, of of dat via de controller moet gebeuren...

        /// <summary>
        /// Creeert verhuisinfo voor gegeven gelieerde persoon en
        /// gegeven adres.
        /// </summary>
        /// <param name="gelieerdePersoonID">persoon waarvoor verhuis geinitieerd werd</param>
        /// <param name="vanAdresID">relevante vanAdresID</param>
        public VerhuisInfo(int vanAdresID, int gelieerdePersoonID)
        {
            AanvragerID = gelieerdePersoonID;
            VanAdresMetBewoners = ServiceHelper.CallService<IGelieerdePersonenService, Adres>(l => l.AdresMetBewonersOphalen(vanAdresID));

            // Bij de constructie van verhuisinfo zijn vanadres naaradres
            // dezelfde.  Van zodra er een postback gebeurt van het form,
            // wordt NaarAdres gebind met de gegevens uit het form; op dat
            // moment wordt een nieuwe instantie van het naaradres
            // gemaakt.

            NaarAdres = VanAdresMetBewoners;

            // Standaard verhuist iedereen mee.
            PersoonIDs = (
                from PersoonsAdres pa in VanAdresMetBewoners.PersoonsAdres
                select pa.Persoon.ID).ToList<int>();
        }

        /// <summary>
        /// Haalt de gegevens van het vanadres opnieuw op, op basis van
        /// VanAdresMetBewoners.ID.
        /// </summary>
        public void HerstelVanAdres()
        {
            VanAdresMetBewoners = ServiceHelper.CallService<IGelieerdePersonenService, Adres>(l => l.AdresMetBewonersOphalen(VanAdresMetBewoners.ID));
        }
    }
}