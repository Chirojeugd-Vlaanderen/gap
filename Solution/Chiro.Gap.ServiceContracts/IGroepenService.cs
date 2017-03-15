/*
 * Copyright 2008-2014 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.ServiceModel;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;

namespace Chiro.Gap.ServiceContracts
{
	// NOTE: If you change the interface name "IGroepenService" here, you must also update the reference to "IGroepenService" in Web.config.

	/// <summary>
	/// ServiceContract voor de GroepenService
	/// </summary>
	[ServiceContract]
    [ServiceKnownType(typeof(AfdelingDetail))]
	public interface IGroepenService
	{
	    [OperationContract]
	    string Hello();

		/// <summary>
		/// Ophalen van Groepsinformatie
		/// </summary>
		/// <param name="groepID">GroepID van groep waarvan we de informatie willen opvragen</param>
		/// <returns>
		/// De gevraagde informatie over de groep met id <paramref name="groepID"/>
		/// </returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		GroepInfo InfoOphalen(int groepID);

		/// <summary>
		/// Haalt info op, uitgaande van code (stamnummer)
		/// </summary>
		/// <param name="code">Stamnummer van de groep waarvoor info opgehaald moet worden</param>
		/// <returns>Groepsinformatie voor groep met code <paramref name="code"/></returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		GroepInfo InfoOphalenCode(string code);

		/// <summary>
		/// Ophalen van gedetailleerde informatie over de groep met ID <paramref name="groepId"/>
		/// </summary>
		/// <param name="groepId">ID van de groep waarvoor de informatie opgehaald moet worden</param>
		/// <returns>Groepsdetails, inclusief categorieen en huidige actieve afdelingen</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		GroepDetail DetailOphalen(int groepId);

		/// <summary>
		/// Haalt de groepen op waarvoor de gebruiker (GAV-)rechten heeft
		/// </summary>
		/// <returns>De (informatie over de) groepen van de gebruiker</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IEnumerable<GroepInfo> MijnGroepenOphalen();

		/// <summary>
		/// Haalt informatie op over alle werkjaren waarin een groep actief was/is.
		/// </summary>
		/// <param name="groepsID">ID van de groep</param>
		/// <returns>Info over alle werkjaren waarin een groep actief was/is.</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IEnumerable<WerkJaarInfo> WerkJarenOphalen(int groepsID);

		/// <summary>
		/// Persisteert een groep in de database
		/// </summary>
		/// <param name="groepInfo">Te persisteren groep</param>
		/// <remarks>FIXME: gedetailleerde exception</remarks>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void Bewaren(GroepInfo groepInfo);

		#region werkjaren

		/// <summary>
		/// Haalt GroepsWerkJaarID van het recentst gemaakte groepswerkjaar
		/// voor een gegeven groep op.
		/// </summary>
		/// <param name="groepID">GroepID van groep</param>
		/// <returns>ID van het recentste GroepsWerkJaar</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		int RecentsteGroepsWerkJaarIDGet(int groepID);

	    /// <summary>
        /// Haalt gedetailleerde gegevens op van het recentst gemaakte groepswerkjaar
        /// voor een gegeven groep op.
	    /// </summary>
        /// <param name="groepid">GroepID van groep</param>
	    /// <returns>
	    /// De details van het recentste groepswerkjaar
	    /// </returns>
	    [OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		GroepsWerkJaarDetail RecentsteGroepsWerkJaarOphalen(int groepid);

		// Alles om gelieerdepersonen op te halen zit in igelieerdepersonenservice

		#endregion

		#region afdelingen

		/// <summary>
		/// Maakt een nieuwe afdeling voor een gegeven ChiroGroep
		/// </summary>
		/// <param name="chiroGroepId">ID van de groep</param>
		/// <param name="naam">Naam van de afdeling</param>
		/// <param name="afkorting">Afkorting van de afdeling (voor lijsten, overzichten,...)</param>
		[OperationContract]
		[FaultContract(typeof(BestaatAlFault<AfdelingInfo>))]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void AfdelingAanmaken(int chiroGroepId, string naam, string afkorting);

		/// <summary>
		/// Bewaart een afdeling met de nieuwe informatie.
		/// </summary>
		/// <param name="info">De afdelingsinfo die opgeslagen moet worden</param>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
        [FaultContract(typeof(BestaatAlFault<AfdelingInfo>))]
		void AfdelingBewaren(AfdelingInfo info);

		/// <summary>
		/// Uitgebreide info ophalen over het afdelingsjaar met de opgegeven ID
		/// </summary>
		/// <param name="afdelingsJaarID">De ID van het afdelingsjaar in kwestie</param>
		/// <returns>Uitgebreide info over het afdelingsjaar met de opgegeven ID</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		AfdelingsJaarDetail AfdelingsJaarOphalen(int afdelingsJaarID);

		/// <summary>
		/// Maakt/bewerkt een AfdelingsJaar: 
		/// andere OfficieleAfdeling en/of andere leeftijden
		/// </summary>
		/// <param name="aj">AfdelingsJaarDetail met de gegevens over het aan te maken of te wijzigen
		/// afdelingsjaar.  <c>aj.AfdelingsJaarID</c> bepaat of het om een bestaand afdelingsjaar gaat
		/// (ID > 0), of een bestaand (ID == 0)</param>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void AfdelingsJaarBewaren(AfdelingsJaarDetail aj);

		/// <summary>
		/// Verwijdert een afdelingsjaar 
		/// en controleert of er geen leden in zitten.
		/// </summary>
		/// <param name="afdelingsJaarID">ID van het afdelingsjaar waarover het gaat</param>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void AfdelingsJaarVerwijderen(int afdelingsJaarID);

        /// <summary>
        /// Verwijdert een afdeling
        /// </summary>
        /// <param name="afdelingID">ID van de afdeling waarover het gaat</param>
        [OperationContract]
        [FaultContract(typeof(GapFault))]
        [FaultContract(typeof(FoutNummerFault))]
        void AfdelingVerwijderen(int afdelingID);

	    /// <summary>
	    /// Haalt details over alle officiele afdelingen op.
	    /// </summary>
	    /// <returns>Rij met details over de officiele afdelingen</returns>
	    [OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IEnumerable<OfficieleAfdelingDetail> OfficieleAfdelingenOphalen();

		/// <summary>
		/// Haat een afdeling op, op basis van <paramref name="afdelingId"/>
		/// </summary>
		/// <param name="afdelingId">ID van op te halen afdeling</param>
		/// <returns>Info van de gevraagde afdeling</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		AfdelingInfo AfdelingOphalen(int afdelingId);

		/// <summary>
		/// Haalt details op van een afdeling, gebaseerd op het <paramref name="afdelingsJaarId"/>
		/// </summary>
		/// <param name="afdelingsJaarId">ID van het AFDELINGSJAAR waarvoor de details opgehaald moeten 
		/// worden.</param>
		/// <returns>De details van de afdeling in het gegeven afdelingsjaar.</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		AfdelingDetail AfdelingDetailOphalen(int afdelingsJaarId);

		/// <summary>
		/// Haalt details op over alle actieve afdelingen in het groepswerkjaar met 
		/// ID <paramref name="groepsWerkJaarId"/>
		/// </summary>
		/// <param name="groepsWerkJaarId">ID van het groepswerkjaar</param>
		/// <returns>
		/// Informatie over alle actieve afdelingen in het groepswerkjaar met 
		/// ID <paramref name="groepsWerkJaarId"/>
		/// </returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		List<AfdelingDetail> ActieveAfdelingenOphalen(int groepsWerkJaarId);

        /// <summary>
        /// Haalt beperkte informatie op over alle afdelingen van een groep
        /// (zowel actief als inactief)
        /// </summary>
        /// <param name="groepId">ID van de groep waarvoor de afdelingen gevraagd zijn</param>
        /// <returns>Lijst met AfdelingInfo</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IList<AfdelingInfo> AlleAfdelingenOphalen(int groepId);

		/// <summary>
		/// Haalt informatie op over de beschikbare afdelingsjaren en hun gelinkte afdelingen van een groep in het huidige
		/// groepswerkjaar.
		/// </summary>
		/// <param name="groepID">ID van de groep waarvoor de info gevraagd is</param>
		/// <returns>Lijst van AfdelingInfo</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		List<ActieveAfdelingInfo> HuidigeAfdelingsJarenOphalen(int groepID);

	    /// <summary>
	    /// Haalt informatie op over de afdelingen van een groep die niet gebruikt zijn in een gegeven 
	    /// groepswerkjaar, op basis van een <paramref name="groepswerkjaarID"/>
	    /// </summary>
	    /// <param name="groepswerkjaarID">ID van het groepswerkjaar waarvoor de niet-gebruikte afdelingen
	    ///     opgezocht moeten worden.</param>
	    /// <returns>Info de ongebruikte afdelingen van een groep in het gegeven groepswerkjaar</returns>
	    [OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		List<AfdelingInfo> OngebruikteAfdelingenOphalen(int groepswerkjaarID);

		#endregion

		#region functies
		/// <summary>
		/// Haalt uit groepswerkjaar met ID <paramref name="groepsWerkJaarID"/> alle beschikbare functies
		/// op voor een lid van type <paramref name="lidType"/>.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar van de gevraagde functies</param>
		/// <param name="lidType"><c>LidType.Kind</c> of <c>LidType.Leiding</c></param>
		/// <returns>De gevraagde lijst afdelingsinfo</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IEnumerable<FunctieDetail> FunctiesOphalen(int groepsWerkJaarID, LidType lidType);

		/// <summary>
		/// Zoekt naar problemen ivm de maximum- en minimumaantallen van functies voor het
		/// huidige werkJaar.
		/// </summary>
		/// <param name="groepID">ID van de groep waarvoor de functies gecontroleerd moeten worden.</param>
		/// <returns>
		/// Een rij FunctieProbleemInfo.  Als er geen problemen zijn, is deze leeg.
		/// </returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IEnumerable<FunctieProbleemInfo> FunctiesControleren(int groepID);

		/// <summary>
		/// Controleert de verplicht in te vullen lidgegevens.
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan de leden te controleren zijn</param>
		/// <returns>Een rij LedenProbleemInfo.  Leeg bij gebrek aan problemen.</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IEnumerable<LedenProbleemInfo> LedenControleren(int groepID);

		/// <summary>
		/// Voegt een functie toe aan de groep
		/// </summary>
		/// <param name="groepID">De groep waaraan het wordt toegevoegd</param>
		/// <param name="naam">De naam van de nieuwe functie</param>
		/// <param name="code">Code voor de nieuwe functie</param>
		/// <param name="maxAantal">Eventueel het maximumaantal leden met die functie in een werkJaar</param>
		/// <param name="minAantal">Het minimumaantal leden met die functie in een werkJaar</param>
		/// <param name="lidType">Gaat het over een functie voor leden, leiding of beide?</param>
		/// <param name="werkJaarVan">Eventueel het vroegste werkJaar waarvoor de functie beschikbaar moet zijn</param>
		/// <returns>De ID van de aangemaakte Functie</returns>
		[OperationContract]
        [FaultContract(typeof(BestaatAlFault<FunctieInfo>))]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		int FunctieToevoegen(int groepID, string naam, string code, int? maxAantal, int minAantal, LidType lidType, int? werkJaarVan);

		/// <summary>
		/// Verwijdert de functie met gegeven <paramref name="functieID"/>
		/// </summary>
		/// <param name="functieID">ID van de te verwijderen functie</param>
		/// <param name="forceren">Indien <c>true</c>, worden eventuele personen uit de
		/// te verwijderen functie eerst uit de functie weggehaald.  Indien
		/// <c>false</c> krijg je een exception als de functie niet leeg is.</param>
		[OperationContract]
		[FaultContract(typeof(BlokkerendeObjectenFault<PersoonLidInfo>))]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void FunctieVerwijderen(int functieID, bool forceren);

		#endregion

		#region categorieën

		/// <summary>
		/// Voegt een categorie toe aan de groep
		/// </summary>
		/// <param name="groepID">De groep waaraan het wordt toegevoegd</param>
		/// <param name="naam">De naam van de nieuwe categorie</param>
		/// <param name="code">Code voor de nieuwe categorie</param>
		/// <returns>De ID van de aangemaakte categorie</returns>
		[OperationContract]
		[FaultContract(typeof(BestaatAlFault<CategorieInfo>))]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		int CategorieToevoegen(int groepID, String naam, String code);

		/// <summary>
		/// Verwijdert de gegeven categorie
		/// </summary>
		/// <param name="categorieID">De ID van de te verwijderen categorie</param>
		/// <param name="forceren">Indien <c>true</c>, worden eventuele personen uit de
		/// te verwijderen categorie eerst uit de categorie weggehaald.  Indien
		/// <c>false</c> krijg je een exception als de categorie niet leeg is.</param>
		[OperationContract]
		[FaultContract(typeof(BlokkerendeObjectenFault<PersoonDetail>))]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void CategorieVerwijderen(int categorieID, bool forceren);

		/// <summary>
		/// Het veranderen van de naam van een categorie
		/// </summary>
		/// <param name="categorieID">De ID van de categorie</param>
		/// <param name="nieuwenaam">De nieuwe naam van de categorie</param>
		/// <exception cref="InvalidOperationException">Gegooid als de naam al bestaat, leeg is of null is</exception>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void CategorieAanpassen(int categorieID, string nieuwenaam);

		/// <summary>
		/// Zoekt een categorie op, op basis van <paramref name="groepID"/> en
		/// <paramref name="categorieCode"/>
		/// </summary>
		/// <param name="groepID">ID van de groep waaraan de categorie gekoppeld moet zijn.</param>
		/// <param name="categorieCode">Code van de categorie</param>
		/// <returns>De categorie met code <paramref name="categorieCode"/> die van toepassing is op
		/// de groep met ID <paramref name="groepID"/>.</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		CategorieInfo CategorieOpzoeken(int groepID, string categorieCode);

		/// <summary>
		/// Haalt alle categorieeen op van de groep met ID <paramref name="groepID"/>
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan de categorieen zijn gevraagd</param>
		/// <returns>Lijst met categorie-info van de categorieen van de gevraagde groep</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IList<CategorieInfo> CategorieenOphalen(int groepID);

		/// <summary>
		/// Zoekt de categorieID op van de categorie bepaald door de gegeven 
		/// <paramref name="groepID"/> en <paramref name="code"/>.
		/// </summary>
		/// <param name="groepID">ID van groep waaraan de gezochte categorie gekoppeld is</param>
		/// <param name="code">Code van de te zoeken categorie</param>
		/// <returns>Het categorieID als de categorie gevonden is, anders 0.</returns>
		int CategorieIDOphalen(int groepID, string code);

		#endregion categorieën

        #region groepsadres
        /// <summary>
        /// Stelt het groepsadres in.
        /// </summary>
        /// <param name="groepID">ID van groep waarvan adres in te stellen.</param>
        /// <param name="adresInfo">Nieuw adres van de groep.</param>
        [OperationContract]
        [FaultContract(typeof(OngeldigObjectFault))]
	    void AdresInstellen(int groepID, AdresInfo adresInfo);
        #endregion

        #region straten en landen

        // Dat adressengedoe staat een beetje verloren in de groepenservice.
		// Maar bij gebrek aan adressenservice, staat het hier wel goed.

		/// <summary>
		/// Maakt een lijst met alle deelgemeentes uit de database; nuttig voor autocompletion
		/// in de ui.
		/// </summary>
		/// <returns>Lijst met alle beschikbare deelgemeentes</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IEnumerable<WoonPlaatsInfo> GemeentesOphalen();

		/// <summary>
		/// Maakt een lijst met alle landen uit de database.
		/// </summary>
		/// <returns>Lijst met alle beschikbare landen</returns>
		[OperationContract]
		List<LandInfo> LandenOphalen();

		/// <summary>
		/// Haalt alle straten op uit een gegeven <paramref name="postNr"/>, waarvan de naam begint
		/// met het gegeven <paramref name="straatStukje"/>.
		/// </summary>
		/// <param name="straatStukje">Eerste letters van de te zoeken straatnamen</param>
		/// <param name="postNr">Postnummer waarin te zoeken</param>
		/// <returns>Gegevens van de gevonden straten</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IEnumerable<StraatInfo> StratenOphalen(String straatStukje, int postNr);

		/// <summary>
		/// Haalt alle straten op uit een gegeven rij <paramref name="postNrs"/>, waarvan de naam begint
		/// met het gegeven <paramref name="straatBegin"/>.
		/// </summary>
		/// <param name="straatBegin">Eerste letters van de te zoeken straatnamen</param>
		/// <param name="postNrs">Postnummers waarin te zoeken</param>
		/// <returns>Gegevens van de gevonden straten</returns>
		/// <remarks>Ik had deze functie ook graag StratenOphalen genoemd, maar je mag geen 2 
		/// WCF-functies met dezelfde naam in 1 service hebben.  Spijtig.</remarks>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IEnumerable<StraatInfo> StratenOphalenMeerderePostNrs(String straatBegin, IEnumerable<int> postNrs);

		#endregion

		#region jaarovergang

        ///  <summary>
        ///  Eens de gebruiker alle informatie heeft ingegeven, wordt de gewenste afdelingsverdeling naar de server gestuurd.
        ///  <para />
        ///  Dit in de vorm van een lijst van afdelingsjaardetails, met volgende info:
        /// 		AFDELINGID van de afdelingen die geactiveerd zullen worden
        /// 		Geboortejaren, geslacht en officiele afdeling voor elk van die afdelingen
        ///  </summary>
        /// <param name="teActiveren">Lijst van de afdelingen die geactiveerd moeten worden in het nieuwe werkJaar</param>
        /// <param name="groepID">ID van de groep voor wie een nieuw groepswerkjaar aangemaakt moet worden</param>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void JaarOvergangUitvoeren(IEnumerable<AfdelingsJaarDetail> teActiveren, int groepID);

		/// <summary>
		/// Berekent wat het nieuwe werkJaar zal zijn als op deze moment de jaarovergang zou gebeuren.
		/// </summary>
		/// <returns>Een jaartal</returns>
		[OperationContract]
		int NieuwWerkJaarOphalen(int groepID);

		#endregion

		/// <summary>
		/// Deze method geeft gewoon de gebruikersnaam weer waaronder je de service aanroept.  Vooral om de
		/// authenticate te testen.
		/// </summary>
		/// <returns>Gebruikersnaam waarmee aangemeld</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		string WieBenIk();

		/// <summary>
		/// Deze method geeft weer of we op een liveomgeving werken (<c>true</c>) of niet (<c>false</c>)
		/// </summary>
		/// <returns><c>True</c> als we op een liveomgeving werken, <c>false</c> als we op een testomgeving werken</returns>
		[OperationContract]
		bool IsLive();

        /// <summary>
        /// Haalt informatie over alle gebruikersrechten van de gegeven groep op.
        /// </summary>
        /// <param name="groepID">ID van de groep waarvan de gebruikersrechten op te vragen zijn</param>
        /// <returns>Lijstje met details van de gebruikersrechten</returns>
        [OperationContract]
	    IEnumerable<GebruikersDetail> GebruikersOphalen(int groepID);

        /// <summary>
        /// Stelt afdelingsjaren voor voor het volgende werkjaar, gegeven de <paramref name="afdelingsIDs"/> van de
        /// afdelingen die je volgend werkjaar wilt hebben.
        /// </summary>
        /// <param name="afdelingsIDs">ID's van de afdelingen die je graag wilt activeren</param>
        /// <param name="groepID">ID van je groep</param>
        /// <returns>Een voorstel voor de afdelingsjaren, in de vorm van een lijstje AfdelingDetails.</returns>
        [OperationContract]
        IList<AfdelingDetail> NieuweAfdelingsJarenVoorstellen(int[] afdelingsIDs, int groepID);

        [OperationContract]
        [FaultContract(typeof(BestaatAlFault<FunctieInfo>))]
	    void FunctieBewerken(FunctieDetail detail);

	    /// <summary>
	    /// Haalt functie met gegeven <paramref name="functieId"/> op
	    /// </summary>
	    /// <param name="functieId"></param>
	    /// <returns></returns>
	    [OperationContract]
	    FunctieDetail FunctieOphalen(int functieId);

        /// <summary>
        /// Verwijdert (zo mogelijk) het groepswerkjaar met gegeven <paramref name="groepsWerkJaarId"/>, en
        /// herstelt de situatie zoals op het einde van vorig groepswerkjaar.
        /// </summary>
        /// <param name="groepsWerkJaarId">ID van groepswerkjaar.</param>
        [OperationContract]
	    void JaarOvergangTerugDraaien(int groepsWerkJaarId);

	    /// <summary>
	    /// Dit levert gewoon een groepsnaam op, om te kijken of de DB werkt.
	    /// </summary>
	    /// <returns></returns>
	    [OperationContract]
	    string TestDatabase();
	}
}
