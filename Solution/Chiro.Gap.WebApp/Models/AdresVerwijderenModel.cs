using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chiro.Adf.ServiceModel;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.Orm;

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Model om adres te verwijderen.  Bevat een adres met daaraan
    /// gekoppeld de bewoners van wie het adres mogelijk vervalt
    /// </summary>
    /// <remarks>Je zou je kunnen afvragen waarom dit model opgebowd
    /// wordt op basis van AdresID en GelieerdePersoonID, en niet via
    /// enkel PersoonsAdresID.  Het heeft ermee te maken dat je toch
    /// steeds de oorspronkelijke GelieerdePersoonsID zal moeten onthouden,
    /// omdat je na het verwijderen van het persoonsadres wel terug moet
    /// kunnen redirecten naar de juiste persoonsinfo.
    /// Je moet dus 2 ID's bewaren, en dat kunnen dus net zo goed
    /// GelieerdePersoonID en AdresID zijn.</remarks>
    public class AdresVerwijderenModel: MasterViewModel
    {
        /// <summary>
        /// ID van GelieerdePersoon met het te verwijderen adres.
        /// Wordt bewaard om achteraf terug naar de details van de
        /// aanvrager te kunnen redirecten.
        /// </summary>
        public int AanvragerGelieerdePersoonID { get; set; }

        /// <summary>
        /// AdresMetBewoners bevat te verwijderen adres,
        /// met daaraan gekoppeld alle bewoners die de aangelogde gebruiker
        /// mag zien.
        /// </summary>
        public Adres AdresMetBewoners { get; set; }

        /// <summary>
        /// Het lijstje PersoonIDs bevat de ID's van de personen van wie
        /// het adres uiteindelijk zal vervallen.
        /// </summary>
        public List<int> PersoonIDs { get; set; }

        /// <summary>
        /// Saaie standaardconstructor
        /// </summary>
        public AdresVerwijderenModel()
        {
            PersoonIDs = new List<int>();
        }

        /// <summary>
        /// Haalt de gegevens van het adres opnieuw op, op basis van
        /// AdresMetBewoners.ID.
        /// </summary>
        public void HerstelAdres()
        {
            AdresMetBewoners = ServiceHelper.CallService<IGelieerdePersonenService, Adres>(l => l.AdresMetBewonersOphalen(AdresMetBewoners.ID));
        }
    }
}
