// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts.FaultContracts;

namespace Chiro.Gap.ServiceContracts
{
	// NOTE: If you change the interface name "IGelieerdePersonenService" here, you must also update the reference to "IGelieerdePersonenService" in Web.config.
	[ServiceContract]
	public interface IGelieerdePersonenService
	{
		/// <summary>
		/// Haalt een pagina met persoonsgegevens op van gelieerde personen van een groep
		/// </summary>
		/// <param name="groepID">ID van de betreffende groep</param>
		/// <param name="pagina">Paginanummer (1 of hoger)</param>
		/// <param name="paginaGrootte">Aantal records per pagina (1 of meer)</param>
		/// <param name="aantalTotaal">Outputparameter; geeft het totaal aantal personen weer in de lijst</param>
		/// <returns>Lijst van gelieerde personen met persoonsinfo</returns>
		[OperationContract]
		IList<GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalTotaal);

		/// <summary>
		/// Haalt een pagina met persoonsgegevens op van gelieerde personen van een groep,
		/// inclusief eventueel lidobject voor het recentste werkjaar.
		/// </summary>
		/// <param name="groepID">ID van de betreffende groep</param>
		/// <param name="pagina">Paginanummer (1 of hoger)</param>
		/// <param name="paginaGrootte">Aantal records per pagina (1 of meer)</param>
		/// <param name="aantalTotaal">Outputparameter; geeft het totaal aantal personen weer in de lijst</param>
		/// <returns>Lijst van gelieerde personen met persoonsinfo</returns>
		[OperationContract]
		IList<PersoonInfo> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalTotaal);

		/// <summary>
		/// Haalt een pagina met persoonsgegevens op van gelieerde personen van een groep die tot de gegeven categorie behoren,
		/// inclusief eventueel lidobject voor het recentste werkjaar.
		/// </summary>
		/// <param name="categorieID">ID van de gevraagde categorie</param>
		/// <param name="pagina">Paginanummer (1 of hoger)</param>
		/// <param name="paginaGrootte">Aantal records per pagina (1 of meer)</param>
		/// <param name="aantalTotaal">Outputparameter; geeft het totaal aantal personen weer in de lijst</param>
		/// <returns>Lijst van gelieerde personen met persoonsinfo</returns>
		[OperationContract]
		IList<PersoonInfo> PaginaOphalenUitCategorieMetLidInfo(int categorieID, int pagina, int paginaGrootte, out int aantalTotaal);

		/// <summary>
		/// Zoekt alle personen die aan de criteria voldoen en geeft daarvan een bepaalde pagina weer
		/// </summary>
		/// <param name="naamgedeelte">Een deel van de naam, dat als zoekterm opgegeven wordt</param>
		/// <param name="pagina">De hoeveelste pagina je wilt zien</param>
		/// <param name="paginagrootte">Het aantal personen dat je per pagina wilt zien</param>
		/// <returns>Een lijst van gelieerde personen bij wie het <paramref name="naamgedeelte" />
		/// in de naam voorkomt</returns>
		IList<GelieerdePersoon> zoekPersonen(string naamgedeelte, int pagina, int paginagrootte);
		// ... andere zoekmogelijkheden

		/// <summary>
		/// Haalt gelieerd persoon op, incl. persoonsgegevens, communicatievormen en adressen
		/// </summary>
		/// <param name="gelieerdePersoonID">ID op te halen GelieerdePersoon</param>
		/// <returns>GelieerdePersoon met persoonsgegevens, communicatievorm en adressen</returns>
		[OperationContract]
		GelieerdePersoon DetailsOphalen(int gelieerdePersoonID);

		/// <summary>
		/// Haalt gelieerd persoon op met extra gevraagde info.
		/// </summary>
		/// <param name="gelieerdePersoonID">ID op te halen GelieerdePersoon</param>
		/// <param name="gevraagd">Stelt voor welke informatie opgehaald moet worden</param>
		/// <returns>GelieerdePersoon uitbreiden met meer info mbt het gevraagde onderwerp </returns>
		// [OperationContract(Name = "PersoonOphalenMetCustomDetails")]
		// GelieerdePersoon PersoonOphalenMetDetails(int gelieerdePersoonID, PersoonsInfo gevraagd);

		/// <summary>
		/// Bewaart gewijzigde gelieerde persoon met zijn groep en zijn persoon
		/// </summary>
		/// <param name="persoon">Te bewaren persoon</param>
		/// <returns>ID van de bewaarde persoon</returns>
		[OperationContract]
		int PersoonBewaren(GelieerdePersoon persoon);

		/// <summary>
		/// Maakt een nieuwe persoon aan, en koppelt die als gelieerde persoon aan de groep met gegeven
		/// <paramref>groepID</paramref>
		/// </summary>
		/// <param name="info">Informatie om de nieuwe (gelieerde) persoon te construeren: Chiroleeftijd, en
		/// de velden van <c>info.Persoon</c></param>
		/// <param name="groepID">ID van de groep waaraan de nieuwe persoon gekoppeld moet worden</param>
		/// <returns>ID van de bewaarde persoon</returns>
		/// <remarks>Adressen, Communicatievormen,... worden niet mee gepersisteerd; enkel de persoonsinfo
		/// en de Chiroleeftijd.  Als de info lijkt op die van een bestaande gelieerde persoon, dan
		/// wordt een exceptie opgegooid.</remarks>
		[OperationContract]
		[FaultContract(typeof(GelijkaardigePersoonFault))]
		int Aanmaken(GelieerdePersoon info, int groepID);

		/// <summary>
		/// Maakt een nieuwe persoon aan, en koppelt die als gelieerde persoon aan de groep met gegeven
		/// <paramref>groepID</paramref>
		/// </summary>
		/// <param name="info">Informatie om de nieuwe (gelieerde) persoon te construeren: Chiroleeftijd, en
		/// de velden van <c>info.Persoon</c></param>
		/// <param name="groepID">ID van de groep waaraan de nieuwe persoon gekoppeld moet worden</param>
		/// <returns>ID van de bewaarde persoon</returns>
		/// <param name="forceer">Als deze <c>true</c> is, wordt de nieuwe persoon sowieso gemaakt, ook
		/// al lijkt hij op een bestaande gelieerde persoon.  Is <paramref>force</paramref>
		/// <c>false</c>, dan wordt er een exceptie opgegooid als de persoon te hard lijkt op een
		/// bestaande.</param>
		/// <remarks>Adressen, Communicatievormen,... worden niet mee gepersisteerd; enkel de persoonsinfo
		/// en de Chiroleeftijd.  Ik had deze functie ook graag 'aanmaken' genoemd (zie coding guideline
		/// 190), maar dat mag blijkbaar niet bij services.</remarks>
		[OperationContract]
		[FaultContract(typeof(GelijkaardigePersoonFault))]
		int GeforceerdAanmaken(GelieerdePersoon info, int groepID, bool forceer);

		/// <summary>
		/// Haalt PersoonID op van een gelieerde persoon
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van de gelieerde persoon</param>
		/// <returns>PersoonID van de persoon gekoppeld aan de gelieerde persoon bepaald door <paramref name="gelieerdePersoonID"/></returns>
		/// <remarks>Eigenlijk is dit een domme method, maar ze wordt gemakshalve nog gebruikt.</remarks>
		[OperationContract]
		int PersoonIDGet(int gelieerdePersoonID);

		#region adressen

		/// <summary>
		/// Haalt adres op, met daaraan gekoppeld de bewoners waarop de
		/// gebruiker GAV-rechten heeft.
		/// </summary>
		/// <param name="adresID">ID op te halen adres</param>
		/// <returns>Adresobject met gekoppelde personen</returns>
		[OperationContract]
		AdresInfo AdresMetBewonersOphalen(int adresID);

		/// <summary>
		/// Verhuist gelieerde personen van een oud naar een nieuw
		/// adres.
		/// (De koppelingen Persoon-Oudadres worden aangepast 
		/// naar Persoon-NieuwAdres.)
		/// </summary>
		/// <param name="persoonIDs">ID's van te verhuizen Personen (niet gelieerd!)</param>
		/// <param name="nieuwAdres">AdresInfo-object met nieuwe adresgegevens</param>
		/// <param name="oudAdresID">ID van het oude adres</param>
		/// <param name="adresType">Adrestype dat alle aangepaste PersoonsAdressen zullen krijgen</param>
		/// <remarks>nieuwAdres.ID wordt genegeerd.  Het adresID wordt altijd
		/// opnieuw opgezocht in de bestaande adressen.  Bestaat het adres nog niet,
		/// dan krijgt het adres een nieuw ID.</remarks>
		[OperationContract]
		[FaultContract(typeof(AdresFault))]
		void PersonenVerhuizen(
			IList<int> persoonIDs,
			AdresInfo nieuwAdres,
			int oudAdresID,
			AdresTypeEnum adresType);

		/// <summary>
		/// Haalt alle personen op die een adres gemeen hebben met de
		/// Persoon bepaald door gelieerdePersoonID
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van GelieerdePersoon</param>
		/// <returns>Lijst met Personen die huisgenoot zijn van gegeven
		/// persoon</returns>
		/// <remarks>Parameters: GELIEERDEpersoonID, returns PERSONEN</remarks>
		[OperationContract]
		IList<BewonersInfo> HuisGenotenOphalen(int gelieerdePersoonID);

		/// <summary>
		/// Voegt een adres toe aan een verzameling personen
		/// </summary>
		/// <param name="personenIDs">ID's van Personen
		/// waaraan het nieuwe adres toegevoegd moet worden.</param>
		/// <param name="adres">Toe te voegen adres</param>
		/// <param name="adresType">Soort adres (thuis, kot, enz.)</param>
		[OperationContract]
		[FaultContract(typeof(AdresFault))]
		void AdresToevoegenAanPersonen(List<int> personenIDs, AdresInfo adres, AdresTypeEnum adresType);

		/// <summary>
		/// Verwijdert een adres van een verzameling personen
		/// </summary>
		/// <param name="personenIDs">ID's van de personen over wie het gaat</param>
		/// <param name="adresID">ID van het adres dat losgekoppeld moet worden</param>
		[OperationContract]
		[FaultContract(typeof(AdresFault))]
		void AdresVerwijderenVanPersonen(List<int> personenIDs, int adresID);

		#endregion adressen

		#region commvormen
		/// <summary>
		/// Voegt een commvorm toe aan een gelieerde persoon
		/// </summary>
		/// <param name="gelieerdepersonenID">ID van de gelieerde persoon</param>
		/// <param name="commvorm">De communicatievorm die aan die persoon gekoppeld moet worden</param>
		/// <param name="typeID">De ID van het communicatietype waartoe <paramref name="commvorm"/> behoort</param>
		[OperationContract]
		[FaultContract(typeof(DataContractFault<CommunicatieVorm>))]
		void CommunicatieVormToevoegenAanPersoon(int gelieerdepersonenID, CommunicatieVorm commvorm, int typeID);

		/// <summary>
		/// Verwijdert een communicatievorm van een gelieerde persoon
		/// </summary>
		/// <param name="gelieerdepersonenID">ID van de gelieerde persoon</param>
		/// <param name="commvormID">ID van de communicatievorm</param>
		[OperationContract]
		[FaultContract(typeof(DataContractFault<CommunicatieVorm>))]
		void CommunicatieVormVerwijderenVanPersoon(int gelieerdepersonenID, int commvormID);

		/// <summary>
		/// Gaat aanpassingen aan een bestaande communicatievorm persisteren zonder links naar andere objecten
		/// </summary>
		/// <param name="c">De aangepaste communicatievorm</param>
		[OperationContract]
		[FaultContract(typeof(DataContractFault<CommunicatieVorm>))]
		void CommunicatieVormAanpassen(CommunicatieVorm c);

		/// <summary>
		/// Haalt een bepaald communicatietype op, op basis van ID
		/// </summary>
		/// <param name="commTypeID">De ID van het communicatietype</param>
		/// <returns></returns>
		[OperationContract]
		CommunicatieType CommunicatieTypeOphalen(int commTypeID);

		/// <summary>
		/// Haalt een lijst op met alle communicatietypes
		/// </summary>
		/// <returns>Een lijst op met alle communicatietypes</returns>
		[OperationContract]
		IEnumerable<CommunicatieType> CommunicatieTypesOphalen();

		/// <summary>
		/// Haalt een communicatievorm op
		/// </summary>
		/// <param name="commvormID">ID van de communicatievorm waarover het gaat</param>
		/// <returns>De communicatievorm met de opgegeven ID</returns>
		[OperationContract]
		CommunicatieVorm CommunicatieVormOphalen(int commvormID);

		#endregion commvormen

		#region categorieën

		/// <summary>
		/// Voegt een collectie gelieerde personen op basis van hun ID toe aan een collectie categorieën
		/// </summary>
		/// <param name="gelieerdepersonenIDs">ID's van de gelieerde personen</param>
		/// <param name="categorieIDs">ID's van de categorieën waaraan ze toegevoegd moeten worden</param>
		[OperationContract]
		[FaultContract(typeof(DataContractFault<Categorie>))]
		void CategorieKoppelen(IList<int> gelieerdepersonenIDs, IList<int> categorieIDs);

		/// <summary>
		/// Haalt een collectie gelieerde personen uit de opgegeven categorie
		/// </summary>
		/// <param name="gelieerdepersonenIDs">ID's van de gelieerde personen over wie het gaat</param>
		/// <param name="categorieID">ID van de categorie waaruit ze verwijderd moeten worden</param>
		[OperationContract]
		[FaultContract(typeof(DataContractFault<Categorie>))]
		void CategorieVerwijderen(IList<int> gelieerdepersonenIDs, int categorieID);

		/// <summary>
		/// Haalt een lijst op van personen die tot de opgegeven categorie behoren
		/// </summary>
		/// <param name="categorieID">ID van de categorie</param>
		/// <returns>Een lijst van personen die tot de opgegeven categorie behoren</returns>
		[OperationContract]
		IList<GelieerdePersoon> OphalenUitCategorie(int categorieID);

		/// <summary>
		/// Haalt een lijst op met alle categorieën voor de opgegeven groep
		/// </summary>
		/// <param name="groepID">ID van de groep waarover het gaat</param>
		/// <returns></returns>
		[OperationContract]
		IEnumerable<Categorie> CategorieenOphalen(int groepID);

		#endregion categorieën
	}
}
