using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.Orm;
using Cg2.Fouten.FaultContracts;

namespace Cg2.ServiceContracts
{
    // NOTE: If you change the interface name "IGelieerdePersonenService" here, you must also update the reference to "IGelieerdePersonenService" in Web.config.
    [ServiceContract]
    public interface IGelieerdePersonenService
    {
        /// <summary>
        /// Haalt een pagina met persoonsgegevens op van gelieerde personen van een groep
        /// </summary>
        /// <param name="groepID">ID van de betreffende groep</param>
        /// <param name="pagina">paginanummer (1 of hoger)</param>
        /// <param name="paginaGrootte">aantal records per pagina (1 of meer)</param>
        /// <param name="aantalOpgehaald">outputparameter; geeft effectief aantal opgehaalde personen weer</param>
        /// <returns>lijst van gelieerde personen met persoonsinfo</returns>
        [OperationContract]
        IList<GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald);

        /// <summary>
        /// Haalt een pagina met persoonsgegevens op van gelieerde personen van een groep,
        /// inclusief eventueel lidobject voor het recentste werkjaar.
        /// </summary>
        /// <param name="groepID">ID van de betreffende groep</param>
        /// <param name="pagina">paginanummer (1 of hoger)</param>
        /// <param name="paginaGrootte">aantal records per pagina (1 of meer)</param>
        /// <param name="aantalOpgehaald">outputparameter; geeft effectief aantal opgehaalde personen weer</param>
        /// <returns>lijst van gelieerde personen met persoonsinfo</returns>
        [OperationContract]
        IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald);

        /// <summary>
        /// Zoekt alle personen die aan de criteria voldoen en geeft daarvan een bepaalde pagina weer
        /// </summary>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        IList<GelieerdePersoon> zoekPersonen(string naamgedeelte, int pagina, int paginagrootte);
        //... andere zoekmogelijkheden

        /// <summary>
        /// Haalt gelieerd persoon op, incl. persoonsgegevens, communicatievormen en adressen
        /// </summary>
        /// <param name="gelieerdePersoonID">ID op te halen GelieerdePersoon</param>
        /// <returns>GelieerdePersoon met persoonsgegevens, communicatievorm en adressen</returns>
        [OperationContract]
        GelieerdePersoon PersoonOphalenMetDetails(int gelieerdePersoonID);
        
        /// <summary>
        /// Haalt gelieerd persoon op met extra gevraagde info.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID op te halen GelieerdePersoon</param>
        /// <param name="gevraagd">Stelt voor welke informatie opgehaald moet worden</param>
        /// <returns>GelieerdePersoon uitbreiden met meer info mbt het gevraagde onderwerp </returns>
        [OperationContract(Name = "PersoonOphalenMetCustomDetails")]
        GelieerdePersoon PersoonOphalenMetDetails(int gelieerdePersoonID, PersoonsInfo gevraagd);

        /// <summary>
        /// Bewaart nieuwe/gewijzigde gelieerde persoon
        /// </summary>
        /// <param name="persoon">Te bewaren persoon</param>
        [OperationContract]
        void PersoonBewaren(GelieerdePersoon persoon);

        /// <summary>
        /// Haalt adres op, met daaraan gekoppeld de bewoners waarop de
        /// gebruiker GAV-rechten heeft.
        /// </summary>
        /// <param name="adresID">ID op te halen adres</param>
        /// <returns>Adresobject met gekoppelde personen</returns>
        [OperationContract]
        Adres AdresMetBewonersOphalen(int adresID);

        /// <summary>
        /// Verhuist gelieerde personen van een oud naar een nieuw
        /// adres.
        /// (De koppelingen GelieerdePersoon-Oudadres worden aangepast 
        /// naar GelieerdePersoon-NieuwAdres.)
        /// </summary>
        /// <param name="verhuizers">ID's van te verhuizen gelieerde personen</param>
        /// <param name="nieuwAdres">Adresobject met nieuwe adresgegevens</param>
        /// <param name="oudAdresID">ID van het oude adres</param>
        /// <remarks>nieuwAdres.ID wordt genegeerd.  Het adresID wordt altijd
        /// opnieuw opgezocht in de bestaande adressen.  Bestaat het adres nog niet,
        /// dan krijgt het adres een nieuw ID.</remarks>
        [OperationContract]
        [FaultContract(typeof(AdresFault))]
        void Verhuizen(IList<int> gelieerdePersonen, Adres nieuwAdres, int oudAdresID);

        /// <summary>
        /// Haalt alle gelieerde personen op die een adres gemeen hebben met de
        /// GelieerdePersoon bepaald door aanvragerID
        /// </summary>
        /// <param name="aanvragerID">ID van GelieerdePersoon</param>
        /// <returns>lijst met GelieerdePersonen die huisgenoot zijn van gegeven gelieerde 
        /// persoon</returns>
        [OperationContract]
        IList<GelieerdePersoon> HuisGenotenOphalen(int aanvragerID);

        /// <summary>
        /// Voegt een adres toe aan een verzameling gelieerde personen
        /// </summary>
        /// <param name="gelieerdePersonenIDs">ID's van gelieerde personen
        /// waaraan het nieuwe adres toegevoegd moet worden.</param>
        /// <param name="adres">Toe te voegen adres</param>
        /// <returns></returns>
        [OperationContract]
        void AdresToevoegen(List<int> gelieerdePersonenIDs, Adres adres);
    }
}
