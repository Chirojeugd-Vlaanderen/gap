﻿// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;

namespace Chiro.Gap.ServiceContracts
{
	// NOTE: If you change the interface name "IGelieerdePersonenService" here, you must also update the reference to "IGelieerdePersonenService" in Web.config.
	
	/// <summary>
	/// ServiceContract voor de GelieerdePersonenService
	/// </summary>
	[ServiceContract]
    public interface IGelieerdePersonenService
	{
		#region ophalen

		/// <summary>
		/// Haalt een persoonsgegevens op van gelieerde personen van een groep,
		/// inclusief eventueel lidobject voor het recentste werkJaar.
		/// </summary>
		/// <param name="selectieGelieerdePersoonIDs">GelieerdePersoonIDs van op te halen personen</param>
		/// <returns>Lijst van gelieerde personen met persoonsinfo</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IList<PersoonDetail> OphalenMetLidInfo(IEnumerable<int> selectieGelieerdePersoonIDs);

        
        /// <summary>
        /// Haalt de persoonsgegevens op van gelieerde personen van een groep
        /// wiens familienaam begint met de letter <paramref name="letter"/>.
        /// inclusief eventueel lidobject voor het recentste werkJaar.
        /// </summary>
        /// <param name="groepID">ID van de betreffende groep</param>
        /// <param name="letter">Beginletter van de achternaam</param>
        /// <param name="aantalTotaal">Outputparameter; levert het totaal aantal personen in de groep op</param>
        /// <returns>Lijst van gelieerde personen met persoonsinfo</returns>
        [OperationContract]
        [FaultContract(typeof(GapFault))]
        [FaultContract(typeof(FoutNummerFault))]
        IList<PersoonDetail> OphalenMetLidInfoViaLetter(int groepID, string letter, out int aantalTotaal);

	    /// <summary>
	    /// Haalt persoonsgegevens op van gelieerde personen van een groep die tot de gegeven categorie behoren,
	    /// waarvan de naam begint met de gegeven <paramref name="letter"/>
	    /// inclusief eventueel lidobject voor het recentste werkJaar.
	    /// </summary>
	    /// <param name="categorieID">ID van de gevraagde categorie</param>
	    /// <param name="letter">letter waarmee de naam moet beginnen</param>
	    /// <param name="sortering">Geeft aan hoe de pagina gesorteerd moet worden</param>
	    /// <param name="aantalTotaal">Outputparameter; geeft het totaal aantal personen weer in de lijst</param>
	    /// <returns>Lijst van gelieerde personen met persoonsinfo</returns>
	    [OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IList<PersoonDetail> OphalenUitCategorieMetLidInfo(int categorieID, string letter, PersoonSorteringsEnum sortering, out int aantalTotaal);

		/// <summary>
		/// Haalt persoonsgegevens op voor alle gegeven gelieerde personen.
		/// </summary>
		/// <param name="gelieerdePersoonIDs">GelieerdePersoonIDs van op te halen personen</param>
		/// <returns>List van PersoonInfo overeenkomend met die IDs</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IList<PersoonInfo> InfoOphalen(IList<int> gelieerdePersoonIDs);

	    /// <summary>
	    /// Haalt een lijst op van de eerste letters van de achternamen van gelieerde personen van een groep
	    /// </summary>
	    /// <param name="groepID">De ID van de groep waaruit we de gelieerde persoonsnamen gaan halen</param>
	    /// <returns>Lijst met de eerste letter van de namen</returns>
	    [OperationContract]
	    [FaultContract(typeof(GapFault))]
	    [FaultContract(typeof(FoutNummerFault))]
	    IList<String> EersteLetterNamenOphalen(int groepID);

	    /// <summary>
	    /// Haalt een lijst op van de eerste letters van de achternamen van gelieerde personen van een categorie
	    /// </summary>
	    /// <param name="categorie">Categorie waaruit we de letters willen halen</param>
	    /// <returns>Lijst met de eerste letter van de namen</returns>
	    [OperationContract]
	    [FaultContract(typeof(GapFault))]
	    [FaultContract(typeof(FoutNummerFault))]
        IList<string> EersteLetterNamenOphalenCategorie(int categorie);

		/// <summary>
		/// Haalt gelieerd persoon op, incl. persoonsgegevens, communicatievormen en adressen
		/// </summary>
		/// <param name="gelieerdePersoonID">ID op te halen GelieerdePersoon</param>
		/// <returns>GelieerdePersoon met persoonsgegevens, communicatievorm en adressen</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		PersoonDetail DetailOphalen(int gelieerdePersoonID);

		/// <summary>
		/// Haalt gelieerde persoon op met ALLE nodige info om het persoons-bewerken scherm te vullen:
		/// persoonsgegevens, categorieen, communicatievormen, lidinfo, afdelingsinfo, adressen
		/// functies
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van de gevraagde gelieerde persoon</param>
		/// <returns>
		/// Gelieerde persoon met ALLE nodige info om het persoons-bewerken scherm te vullen:
		/// persoonsgegevens, categorieen, communicatievormen, lidinfo, afdelingsinfo, adressen
		/// functies
		/// </returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		PersoonLidInfo AlleDetailsOphalen(int gelieerdePersoonID);

		/// <summary>
		/// Haalt gegevens op van alle personen uit categorie met ID <paramref name="categorieID"/>
		/// </summary>
		/// <param name="categorieID">Indien verschillend van 0, worden alle personen uit de categore met
		/// gegeven CategoreID opgehaald.  Anders alle personen tout court.</param>
		/// <returns>Lijst 'PersoonOverzicht'-objecten van alle gelieerde personen uit de categorie</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IList<PersoonOverzicht> AllenOphalenUitCategorie(int categorieID);

		/// <summary>
		/// Haalt gegevens op van alle personen uit groep met ID <paramref name="groepID"/>.
		/// </summary>
		/// <param name="groepID">ID van de groep waaruit de personen gehaald moeten worden</param>
		/// <returns>Rij 'PersoonOverzicht'-objecten van alle gelieerde personen uit de groep.</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IList<PersoonOverzicht> AllenOphalenUitGroep(int groepID);

		/// <summary>
		/// Haalt gegevens op van alle gelieerdepersonen met IDs in <paramref name="gelieerdePersoonIDs"/>.
		/// </summary>
		/// <param name="gelieerdePersoonIDs">IDs van de gelieerdepersonen waarover informatie opgehaald moet worden</param>
		/// <returns>Rij 'PersoonOverzicht'-objecten van alle gelieerde personen uit de groep.</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IEnumerable<PersoonOverzicht> OverzichtOphalen(IList<int> gelieerdePersoonIDs);

		#endregion ophalen

		#region bewaren

		/// <summary>
		/// Updatet een persoon op basis van <paramref name="persoonInfo"/>
		/// </summary>
		/// <param name="persoonInfo">Info over te bewaren persoon</param>
		/// <returns>GelieerderPersoonID van de bewaarde gelieerde persoon</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		int Bewaren(PersoonInfo persoonInfo);

		/// <summary>
		/// Maakt een nieuwe persoon aan, en koppelt die als gelieerde persoon aan de groep met gegeven
		/// <paramref>groepID</paramref>
		/// </summary>
		/// <param name="info">Informatie om de nieuwe (gelieerde) persoon te construeren</param>
		/// <param name="groepID">ID van de groep waaraan de nieuwe persoon gekoppeld moet worden</param>
		/// <returns>ID's van de bewaarde persoon en gelieerde persoon</returns>
		[OperationContract]
		[FaultContract(typeof(BlokkerendeObjectenFault<PersoonDetail>))]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IDPersEnGP Aanmaken(PersoonInfo info, int groepID);

		/// <summary>
		/// Maakt een nieuwe persoon aan, en koppelt die als gelieerde persoon aan de groep met gegeven <paramref>groepID</paramref>
		/// </summary>
		/// <param name="info">Informatie om de nieuwe (gelieerde) persoon te construeren</param>
		/// <param name="groepID">ID van de groep waaraan de nieuwe persoon gekoppeld moet worden</param>
		/// <returns>ID's van de bewaarde persoon en gelieerde persoon</returns>
		/// <param name="forceer">Als deze <c>true</c> is, wordt de nieuwe persoon sowieso gemaakt, ook
		/// al lijkt hij op een bestaande gelieerde persoon.  Is <paramref>force</paramref>
		/// <c>false</c>, dan wordt er een exceptie opgegooid als de persoon te hard lijkt op een
		/// bestaande.</param>
		/// <remarks>Adressen, Communicatievormen,... worden niet mee gepersisteerd; enkel de persoonsinfo
		/// en de Chiroleeftijd.  Ik had deze functie ook graag 'aanmaken' genoemd (zie coding guideline
		/// 190), maar dat mag blijkbaar niet bij services.</remarks>
		[OperationContract]
		[FaultContract(typeof(BlokkerendeObjectenFault<PersoonDetail>))]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IDPersEnGP AanmakenForceer(PersoonInfo info, int groepID, bool forceer);

		/// <summary>
		/// Haalt PersoonID op van een gelieerde persoon
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van de gelieerde persoon</param>
		/// <returns>PersoonID van de persoon gekoppeld aan de gelieerde persoon bepaald door <paramref name="gelieerdePersoonID"/></returns>
		/// <remarks>Eigenlijk is dit een domme method, maar ze wordt gemakshalve nog gebruikt.</remarks>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		int PersoonIDGet(int gelieerdePersoonID);

	    /// <summary>
	    /// Zoekt naar gelieerde personen van een bepaalde groep (met ID <paramref name="groepID"/> waarbij
	    /// naam of voornaam ongeveer begint met <paramref name="teZoeken"/>
	    /// </summary>
	    /// <param name="groepID">GroepID dat bepaalt in welke gelieerde personen gezocht mag worden</param>
	    /// <param name="teZoeken">Te zoeken voor- of achternaam (ongeveer)</param>
	    /// <returns>Lijst met gevonden matches</returns>
        /// <remarks>Deze method levert enkel naam, voornaam en gelieerdePersoonID op!</remarks>
	    [OperationContract]
	    [FaultContract(typeof(GapFault))]
	    [FaultContract(typeof(FoutNummerFault))]
	    IEnumerable<PersoonInfo> ZoekenOpNaamVoornaamBegin(int groepID, string teZoeken);

		#endregion

		#region adressen

		/// <summary>
		/// Haalt adres op, met daaraan gekoppeld de bewoners uit de groep met ID <paramref name="groepID"/>.
		/// </summary>
		/// <param name="adresID">ID op te halen adres</param>
		/// <param name="groepID">ID van de groep</param>
		/// <returns>Adresobject met gekoppelde personen</returns>
		/// <remarks>GelieerdePersoonID's van bewoners worden niet mee opgehaald</remarks>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		GezinInfo GezinOphalen(int adresID, int groepID);

		/// <summary>
		/// Verhuist gelieerde personen van een oud naar een nieuw adres
		/// (De koppelingen Persoon-Oudadres worden aangepast 
		/// naar Persoon-NieuwAdres.)
		/// </summary>
		/// <param name="gelieerdePersoonIDs">ID's van te verhuizen *GELIEERDE* Personen </param>
		/// <param name="nieuwAdres">AdresInfo-object met nieuwe adresgegevens</param>
		/// <param name="oudAdresID">ID van het oude adres</param>
        /// <remarks>De ID van <paramref name="nieuwAdres"/> wordt genegeerd.  Het adresID wordt altijd
		/// opnieuw opgezocht in de bestaande adressen.  Bestaat het adres nog niet,
		/// dan krijgt het adres een nieuw ID.</remarks>
		[OperationContract]
		[FaultContract(typeof(BlokkerendeObjectenFault<PersoonsAdresInfo2>))]
		[FaultContract(typeof(OngeldigObjectFault))]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void GelieerdePersonenVerhuizen(IEnumerable<int> gelieerdePersoonIDs, PersoonsAdresInfo nieuwAdres, int oudAdresID);

        /// <summary>
        /// Gegeven een gelieerde persoon met gegeven <paramref name="gelieerdePersoonID"/>, haal al diens
        /// huisgenoten uit zijn eigen groep op.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van GelieerdePersoon</param>
        /// <returns>Lijst met Personen uit dezelfde groep die huisgenoot zijn van gegeven
        /// persoon</returns>
        /// <remarks>Parameters: GELIEERDEpersoonID, returns PERSONEN</remarks>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		List<BewonersInfo> HuisGenotenOphalenZelfdeGroep(int gelieerdePersoonID);

		/// <summary>
		/// Voegt een adres toe aan een verzameling *GELIEERDE* personen
		/// </summary>
		/// <param name="gelieerdePersonenIDs">ID's van de gelieerde personen
		/// waaraan het nieuwe adres toegevoegd moet worden.</param>
		/// <param name="adr">Toe te voegen adres</param>
		/// <param name="voorkeur"><c>True</c> als het nieuwe adres het voorkeursadres moet worden.</param>
		[OperationContract]
		[FaultContract(typeof(OngeldigObjectFault))]
		[FaultContract(typeof(BlokkerendeObjectenFault<PersoonsAdresInfo2>))]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void AdresToevoegenGelieerdePersonen(List<int> gelieerdePersonenIDs, PersoonsAdresInfo adr, bool voorkeur);

		/// <summary>
		/// Verwijdert een adres van een verzameling personen
		/// </summary>
		/// <param name="personenIDs">ID's van de personen over wie het gaat</param>
		/// <param name="adresID">ID van het adres dat losgekoppeld moet worden</param>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void AdresVerwijderenVanPersonen(IList<int> personenIDs, int adresID);

		/// <summary>
		/// Maakt het PersoonsAdres met ID <paramref name="persoonsAdresID"/> het voorkeursadres van de gelieerde persoon
		/// met ID <paramref name="gelieerdePersoonID"/>
		/// </summary>
		/// <param name="persoonsAdresID">ID van het persoonsadres dat voorkeursadres moet worden</param>
		/// <param name="gelieerdePersoonID">ID van de gelieerde persoon die het gegeven persoonsadres als voorkeur 
		/// moet krijgen.</param>
		/// <remarks>Goed opletten: een PersoonsAdres is gekoppeld aan een persoon; het voorkeursadres is gekoppeld
		/// aan een *gelieerde* persoon.</remarks>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void VoorkeursAdresMaken(int persoonsAdresID, int gelieerdePersoonID);

		#endregion adressen

		#region commvormen
		/// <summary>
		/// Voegt een commvorm toe aan een gelieerde persoon
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van de gelieerde persoon</param>
		/// <param name="commInfo">De communicatievorm die aan die persoon gekoppeld moet worden</param>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void CommunicatieVormToevoegen(int gelieerdePersoonID, CommunicatieInfo commInfo);

		/// <summary>
		/// Verwijdert een communicatievorm van een gelieerde persoon
		/// </summary>
		/// <param name="commvormID">ID van de communicatievorm</param>
		/// <returns>De ID van de gelieerdepersoon die bij de commvorm hoort</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		int CommunicatieVormVerwijderenVanPersoon(int commvormID);

		/// <summary>
		/// Persisteert de wijzigingen aan een bestaande communicatievorm
		/// </summary>
		/// <param name="c">De aan te passen communicatievorm</param>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void CommunicatieVormAanpassen(CommunicatieInfo c);

		/// <summary>
		/// Haalt info over een bepaald communicatietype op, op basis van ID
		/// </summary>
		/// <param name="commTypeID">De ID van het communicatietype</param>
		/// <returns>Info over het gevraagde communicatietype</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		CommunicatieTypeInfo CommunicatieTypeOphalen(int commTypeID);

		/// <summary>
		/// Haalt een lijst op met alle communicatietypes
		/// </summary>
		/// <returns>Een lijst op met alle communicatietypes</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IEnumerable<CommunicatieTypeInfo> CommunicatieTypesOphalen();

		/// <summary>
		/// Haalt detail van een communicatievorm op
		/// </summary>
		/// <param name="commvormID">ID van de communicatievorm waarover het gaat</param>
		/// <returns>De communicatievorm met de opgegeven ID</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		CommunicatieDetail CommunicatieVormOphalen(int commvormID);

		#endregion commvormen

		#region categorieën

		/// <summary>
		/// Voegt een collectie gelieerde personen op basis van hun ID toe aan een collectie categorieën
		/// </summary>
		/// <param name="gelieerdepersonenIDs">ID's van de gelieerde personen</param>
		/// <param name="categorieIDs">ID's van de categorieën waaraan ze toegevoegd moeten worden</param>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void CategorieKoppelen(IList<int> gelieerdepersonenIDs, IList<int> categorieIDs);

		/// <summary>
		/// Haalt een collectie gelieerde personen uit de opgegeven categorie
		/// </summary>
		/// <param name="gelieerdepersonenIDs">ID's van de gelieerde personen over wie het gaat</param>
		/// <param name="categorieID">ID van de categorie waaruit ze verwijderd moeten worden</param>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void CategorieVerwijderen(IList<int> gelieerdepersonenIDs, int categorieID);

		#endregion categorieën

	}
}