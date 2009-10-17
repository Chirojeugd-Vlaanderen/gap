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
        /// <param name="aantalTotaal">outputparameter; geeft het totaal aantal personen weer in de lijst</param>
        /// <returns>lijst van gelieerde personen met persoonsinfo</returns>
        [OperationContract]
        IList<GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalTotaal);

        /// <summary>
        /// Haalt een pagina met persoonsgegevens op van gelieerde personen van een groep,
        /// inclusief eventueel lidobject voor het recentste werkjaar.
        /// </summary>
        /// <param name="groepID">ID van de betreffende groep</param>
        /// <param name="pagina">paginanummer (1 of hoger)</param>
        /// <param name="paginaGrootte">aantal records per pagina (1 of meer)</param>
        /// <param name="aantalTotaal">outputparameter; geeft het totaal aantal personen weer in de lijst</param>
        /// <returns>lijst van gelieerde personen met persoonsinfo</returns>
        [OperationContract]
        IList<PersoonInfo> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalTotaal);

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
        //[OperationContract(Name = "PersoonOphalenMetCustomDetails")]
       // GelieerdePersoon PersoonOphalenMetDetails(int gelieerdePersoonID, PersoonsInfo gevraagd);

        /// <summary>
        /// Bewaart gewijzigde gelieerde persoon met zijn groep en zijn persoon
        /// </summary>
        /// <param name="persoon">Te bewaren persoon</param>
        /// <returns>ID van de bewaarde persoon</returns>
        [OperationContract]
        int PersoonBewaren(GelieerdePersoon persoon);

        /// <summary>
        /// Maakt een nieuwe gelieerdepersoon en persoon aan
        /// </summary>
        /// <param name="persoon">Te bewaren gelieerdepersoon, gelinkt met een nieuwe persoon persoon</param>
        /// <returns>ID van de bewaarde persoon</returns>
        [OperationContract]
        int PersoonAanmaken(GelieerdePersoon persoon, int groepID);

        #region adressen

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
        /// (De koppelingen Persoon-Oudadres worden aangepast 
        /// naar Persoon-NieuwAdres.)
        /// </summary>
        /// <param name="persoonIDs">ID's van te verhuizen Personen (niet gelieerd!)</param>
        /// <param name="nieuwAdres">Adresobject met nieuwe adresgegevens</param>
        /// <param name="oudAdresID">ID van het oude adres</param>
        /// <remarks>nieuwAdres.ID wordt genegeerd.  Het adresID wordt altijd
        /// opnieuw opgezocht in de bestaande adressen.  Bestaat het adres nog niet,
        /// dan krijgt het adres een nieuw ID.</remarks>
        [OperationContract]
        [FaultContract(typeof(AdresFault))]
        void PersonenVerhuizen(IList<int> persoonIDs, Adres nieuwAdres, int oudAdresID);

        /// <summary>
        /// Haalt alle personen op die een adres gemeen hebben met de
        /// Persoon bepaald door gelieerdePersoonID
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van GelieerdePersoon</param>
        /// <returns>lijst met Personen die huisgenoot zijn van gegeven
        /// persoon</returns>
        /// <remarks>parameters: GELIEERDEpersoonID, returns PERSONEN</remarks>
        [OperationContract]
        IList<Persoon> HuisGenotenOphalen(int gelieerdePersoonID);

        /// <summary>
        /// Voegt een adres toe aan een verzameling personen
        /// </summary>
        /// <param name="personenIDs">ID's van Personen
        /// waaraan het nieuwe adres toegevoegd moet worden.</param>
        /// <param name="adres">Toe te voegen adres</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(AdresFault))]
        void AdresToevoegenAanPersonen(List<int> personenIDs, Adres adres);
        [OperationContract]
        [FaultContract(typeof(AdresFault))]
        void AdresVerwijderenVanPersonen(List<int> personenIDs, int adresID);

        #endregion adressen

        #region commvormen
        /// <summary>
        /// Voegt een commvorm toe aan een verzameling personen
        /// </summary>
        /// <param name="personenIDs">ID's van Personen
        /// waaraan het nieuwe commvorm toegevoegd moet worden.</param>
        /// <param name="adres">Toe te voegen commvorm</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(BusinessFault<CommunicatieVorm>))]
        void CommVormToevoegenAanPersoon(int gelieerdepersonenID, CommunicatieVorm commvorm);
        [OperationContract]
        [FaultContract(typeof(BusinessFault<CommunicatieVorm>))]
        void CommVormVerwijderenVanPersoon(int gelieerdepersonenID, int commvormID);

        /// <summary>
        /// Gaat aanpassingen aan een bestaande commvorm persisteren, gelinkt met gelieerdepersoon
        /// </summary>
        /// <param name="v">De aangepaste commvorm met gelieerdepersoon</param>
        [OperationContract]
        [FaultContract(typeof(BusinessFault<CommunicatieVorm>))]
        void AanpassenCommVorm(CommunicatieVorm v);
        #endregion commvormen
    }
}
