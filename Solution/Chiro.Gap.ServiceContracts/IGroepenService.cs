// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.ServiceModel;

using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.FaultContracts;

namespace Chiro.Gap.ServiceContracts
{
	// NOTE: If you change the interface name "IGroepenService" here, you must also update the reference to "IGroepenService" in Web.config.
	[ServiceContract]
	public interface IGroepenService
	{
		/// <summary>
		/// Ophalen van Groepsinformatie
		/// </summary>
		/// <param name="groepID">GroepID van groep waarvan we de informatie willen opvragen</param>
		/// <returns>
		/// De gevraagde informatie over de groep met id <paramref name="groepID"/>
		/// </returns>
		[OperationContract]
		GroepInfo InfoOphalen(int groepID);

		/// <summary>
		/// Haalt info op, uitgaande van code (stamnummer)
		/// </summary>
		/// <param name="code">Stamnummer van de groep waarvoor info opgehaald moet worden</param>
		/// <returns>Groepsinformatie voor groep met code <paramref name="code"/></returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		GroepInfo InfoOphalenCode(string code);

		/// <summary>
		/// Ophalen van gedetailleerde informatie over de groep met ID <paramref name="groepID"/>
		/// </summary>
		/// <param name="groepID">ID van de groep waarvoor de informatie opgehaald moet worden</param>
		/// <returns>Groepsdetails, inclusief categorieen en huidige actieve afdelingen</returns>
		[OperationContract]
		GroepDetail DetailOphalen(int groepID);

		/// <summary>
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		IEnumerable<GroepInfo> MijnGroepenOphalen();

		/// <summary>
		/// Haalt informatie op over alle werkjaren waarin een groep actief was/is.
		/// </summary>
		/// <param name="groepsID">ID van de groep</param>
		/// <returns>Info over alle werkjaren waarin een groep actief was/is.</returns>
		[OperationContract]
		IEnumerable<WerkJaarInfo> WerkJarenOphalen(int groepsID);

		/// <summary>
		/// Persisteert een groep in de database
		/// </summary>
		/// <param name="g">Te persisteren groep</param>
		/// <remarks>FIXME: gedetailleerde exception</remarks>
		[OperationContract]
		void Bewaren(GroepInfo g);

		#region werkjaren

		/// <summary>
		/// Haalt GroepsWerkJaarID van het recentst gemaakte groepswerkjaar
		/// voor een gegeven groep op.
		/// </summary>
		/// <param name="groepID">GroepID van groep</param>
		/// <returns>ID van het recentste GroepsWerkJaar</returns>
		[OperationContract]
		int RecentsteGroepsWerkJaarIDGet(int groepID);

		// Alles om gelieerdepersonen op te halen zit in igelieerdepersonenservice

		#endregion

		#region afdelingen

		/// <summary>
		/// Maakt een nieuwe afdeling voor een gegeven groep
		/// </summary>
		/// <param name="groepID">ID van de groep</param>
		/// <param name="naam">Naam van de afdeling</param>
		/// <param name="afkorting">Afkorting van de afdeling (voor lijsten, overzichten,...)</param>
		[OperationContract]
		[FaultContract(typeof(BestaatAlFault<AfdelingInfo>))]
		void AfdelingAanmaken(int groepID, string naam, string afkorting);

		[OperationContract]
		AfdelingsJaarDetail AfdelingsJaarOphalen(int afdelingsJaarID);

		/// <summary>
		/// Maakt/bewerkt een AfdelingsJaar: 
		/// andere OfficieleAfdeling en/of andere leeftijden
		/// </summary>
		/// <param name="aj">AfdelingsJaarDetail met de gegevens over het aan te maken of te wijzigen
		/// afdelingsjaar.  <c>aj.AfdelingsJaarID</c> bepaat of het om een bestaand afdelingsjaar gaat
		/// (ID > 0), of een bestaand (ID == 0)</param>
		[OperationContract]
		void AfdelingsJaarBewaren(AfdelingsJaarDetail aj);

		/// <summary>
		/// Verwijdert een afdelingsjaar 
		/// en controleert of er geen leden in zitten.
		/// </summary>
		/// <param name="afdelingsJaarID">ID van het afdelingsjaar waarover het gaat</param>
		[OperationContract]
		void AfdelingsJaarVerwijderen(int afdelingsJaarID);

		/// <summary>
		/// Haalt details over alle officiele afdelingen op.
		/// </summary>
		/// <param name="groepID">ID van een groep, zodat aan de hand van het recenste groepswerkjaar
		/// de standaardgeboortejaren van en tot bepaald kunnen worden</param>
		/// <returns>Rij met details over de officiele afdelingen</returns>
		[OperationContract]
		IEnumerable<OfficieleAfdelingDetail> OfficieleAfdelingenOphalen(int groepID);

		/// <summary>
		/// Haat een afdeling op, op basis van <paramref name="afdelingID"/>
		/// </summary>
		/// <param name="afdelingID">ID van op te halen afdeling</param>
		/// <returns>Info van de gevraagde afdeling</returns>
		[OperationContract]
		AfdelingInfo AfdelingOphalen(int afdelingID);

		/// <summary>
		/// Haalt details op van een afdeling, gebaseerd op het <paramref name="afdelingsJaarID"/>
		/// </summary>
		/// <param name="afdelingsJaarID">ID van het AFDELINGSJAAR waarvoor de details opgehaald moeten 
		/// worden.</param>
		/// <returns>De details van de afdeling in het gegeven afdelingsjaar.</returns>
		[OperationContract]
		AfdelingDetail AfdelingDetailOphalen(int afdelingsJaarID);

		/// <summary>
		/// Haalt details op over alle actieve afdelingen in het groepswerkjaar met 
		/// ID <paramref name="groepsWerkJaarID"/>
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <returns>
		/// Informatie over alle actieve afdelingen in het groepswerkjaar met 
		/// ID <paramref name="groepsWerkJaarID"/>
		/// </returns>
		[OperationContract]
		IList<AfdelingDetail> AfdelingenOphalen(int groepsWerkJaarID);

		/// <summary>
		/// Haalt beperkte informatie op over de beschikbare afdelingen van een groep in het huidige
		/// groepswerkjaar.
		/// </summary>
		/// <param name="groepID">ID van de groep waarvoor de afdelingen gevraagd zijn</param>
		/// <returns>Lijst van ActieveAfdelingInfo</returns>
		[OperationContract]
		IList<ActieveAfdelingInfo> BeschikbareAfdelingenOphalen(int groepID);

		/// <summary>
		/// Haalt informatie op over de afdelingen van een groep die niet gebruikt zijn in een gegeven 
		/// groepswerkjaar, op basis van een <paramref name="groepswerkjaarID"/>
		/// </summary>
		/// <param name="groepswerkjaarID">ID van het groepswerkjaar waarvoor de niet-gebruikte afdelingen
		/// opgezocht moeten worden.</param>
		/// <returns>Info de ongebruikte afdelingen van een groep in het gegeven groepswerkjaar</returns>
		[OperationContract]
		IList<AfdelingInfo> OngebruikteAfdelingenOphalen(int groepswerkjaarID);

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
		IList<FunctieInfo> FunctiesOphalen(int groepsWerkJaarID, LidType lidType);

		/// <summary>
		/// Zoekt naar problemen ivm de maximum- en minimumaantallen van functies voor het
		/// huidige werkjaar.
		/// </summary>
		/// <param name="groepID">ID van de groep waarvoor de functies gecontroleerd moeten worden.</param>
		/// <returns>
		/// Een rij FunctieProbleemInfo.  Als er geen problemen zijn, is deze leeg.
		/// </returns>
		[OperationContract]
		IEnumerable<FunctieProbleemInfo> FunctiesControleren(int groepID);
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
		void CategorieVerwijderen(int categorieID, bool forceren);

		/// <summary>
		/// Het veranderen van de naam van een categorie
		/// </summary>
		/// <param name="categorieID">De ID van de categorie</param>
		/// <param name="nieuwenaam">De nieuwe naam van de categorie</param>
		/// <exception cref="InvalidOperationException">Gegooid als de naam al bestaat, leeg is of null is</exception>
		[OperationContract]
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
		CategorieInfo CategorieOpzoeken(int groepID, string categorieCode);

		/// <summary>
		/// Haalt alle categorieeen op van de groep met ID <paramref name="groepID"/>
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan de categorieen zijn gevraagd</param>
		/// <returns>Lijst met categorie-info van de categorieen van de gevraagde groep</returns>
		[OperationContract]
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

		#region adressen

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		IEnumerable<WoonPlaatsInfo> GemeentesOphalen();

		/// <summary>
		/// Haalt alle straten op uit een gegeven <paramref name="postNr"/>, waarvan de naam begint
		/// met het gegeven <paramref name="straatBegin"/>.
		/// </summary>
		/// <param name="straatBegin">Eerste letters van de te zoeken straatnamen</param>
		/// <param name="postNr">Postnummer waarin te zoeken</param>
		/// <returns>Gegevens van de gevonden straten</returns>
		[OperationContract]
		IEnumerable<StraatInfo> StratenOphalen(String straatBegin, int postNr);

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
		IEnumerable<StraatInfo> StratenOphalenMeerderePostNrs(String straatBegin, IEnumerable<int> postNrs);

		#endregion

		/// <summary>
		/// Deze method geeft gewoon de gebruikersnaam weer waaronder je de service aanroept.  Vooral om de
		/// authenticate te testen.
		/// </summary>
		/// <returns>Gebruikersnaam waarmee aangemeld</returns>
		[OperationContract]
		string WieBenIk();
	}
}
