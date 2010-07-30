// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.ServiceModel;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;

namespace Chiro.Gap.ServiceContracts
{
	// NOTE: If you change the interface name "ILedenService" here, you must also update the reference to "ILedenService" in Web.config.

	/// <summary>
	/// ServiceContract voor de LedenService
	/// </summary>
	[ServiceContract]
	public interface ILedenService
	{
		/// <summary>
		/// TODO is niet meer up to date (na combinatie met activeren)
		/// Gaat een gelieerde persoon ophalen en maakt die lid in het huidige werkjaar.  Als het om kindleden gaat,
		/// krjgen ze meteen een afdeling die overeenkomt met leeftijd en geslacht.
		/// </summary>
		/// <param name="gelieerdePersoonIDs">ID's van de gelieerde personen</param>
		/// <param name="foutBerichten">Als er sommige personen geen lid gemaakt werden, bevat foutBerichten een
		/// string waarin wat uitleg staat.  TODO: beter systeem vinden voor deze feedback.</param>
		/// <returns>De LidIDs van de personen die lid zijn gemaakt</returns>
		/// <remarks>
		/// Als er met bepaalde gelieerde personen een probleem is (geen geboortedatum,...), dan worden
		/// de personen die geen problemen vertonen *toch* lid gemaakt. 
		/// </remarks>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IEnumerable<int> Inschrijven(IEnumerable<int> gelieerdePersoonIDs, out string foutBerichten);
		
		/// <summary>
		/// Probeert de opgegeven gelieerde personen in te schrijven (aan te sluiten),
		/// en geeft per geval feedback als dat niet gelukt is
		/// </summary>
		/// <param name="gelieerdePersoonIDs">Lijst van ID's van gelieerde personen die ingeschreven moeten worden</param>
		/// <param name="type">Het type dat aangeeft of die gelieerde personen lid (kind) zijn of leiding</param>
		/// <param name="foutBerichten">Als er sommige personen geen lid gemaakt werden, bevat foutBerichten een
		/// string waarin wat uitleg staat. TODO: beter systeem vinden voor deze feedback.</param>
		/// <returns>De LidIDs van de personen die lid zijn gemaakt</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IEnumerable<int> NieuwInschrijven(IEnumerable<int> gelieerdePersoonIDs, LidType type, out string foutBerichten);

		/// <summary>
		/// Maakt lid met gegeven ID nonactief
		/// </summary>
		/// <param name="gelieerdePersoonIDs">ID's van de gelieerde personen</param>
		/// <param name="foutBerichten">Als voor sommige personen die actie mislukte, bevat foutBerichten een
		/// string waarin wat uitleg staat.  TODO: beter systeem vinden voor deze feedback.</param>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void Uitschrijven(IEnumerable<int> gelieerdePersoonIDs, out string foutBerichten);

		/// <summary>
		/// Slaat veranderingen op aan de eigenschappen van het lidobject zelf. Creëert of verwijdert geen leden, en leden
		/// kunnen ook niet van werkjaar of van gelieerdepersoon veranderen.
		/// </summary>
		/// <param name="lid">Te bewaren lid</param>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void Bewaren(PersoonLidInfo lid);

		/// <summary>
		/// Vervangt de functies van het lid bepaald door <paramref name="lidID"/> door de functies
		/// met ID's <paramref name="functieIDs"/>
		/// </summary>
		/// <param name="lidID">ID van lid met te vervangen functies</param>
		/// <param name="functieIDs">IDs van nieuwe functies voor het lid</param>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void FunctiesVervangen(int lidID, IEnumerable<int> functieIDs);

		/// <summary>
		/// Haalt de ID's van de groepswerkjaren van een lid op.
		/// </summary>
		/// <param name="lidID">ID van het lid waarin we geinteresseerd zijn</param>
		/// <returns>Een LidAfdelingInfo-object</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		LidAfdelingInfo AfdelingenOphalen(int lidID);

		/// <summary>
		/// Vervangt de afdelingen van het lid met ID <paramref name="lidID"/> door de afdelingen
		/// met AFDELINGSJAARIDs gegeven door <paramref name="afdelingsJaarIDs"/>.
		/// </summary>
		/// <param name="lidID">Lid dat nieuwe afdelingen moest krijgen</param>
		/// <param name="afdelingsJaarIDs">ID's van de te koppelen afdelingsjaren</param>
		/// <returns>De GelieerdePersoonID van het lid</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		int AfdelingenVervangen(int lidID, IEnumerable<int> afdelingsJaarIDs);

		/// <summary>
		/// Bewaart lidinfo, inclusief wat in vrije velden ingevuld werd
		/// </summary>
		/// <param name="lid">De info die opgeslagen moet worden</param>
		/// <returns>De bijgewerkte lidinfo</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		PersoonLidInfo BewarenMetVrijeVelden(PersoonLidInfo lid);

		/// <summary>
		/// Verzekert lid met ID <paramref name="lidID"/> tegen loonverlies
		/// </summary>
		/// <param name="lidID">ID van te verzekeren lid</param>
		/// <returns>GelieerdePersoonID van het verzekerde lid</returns>
		/// <remarks>Dit is nogal een specifieke method.  In ons domain model is gegeven dat verzekeringen gekoppeld zijn aan
		/// personen, voor een bepaalde periode.  Maar in eerste instantie zal alleen de verzekering loonverlies gebruikt worden,
		/// die per definitie enkel voor leden bestaat.</remarks>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		int LoonVerliesVerzekeren(int lidID);

		#region Ophalen

		// TODO: we vragen leden op per groepswerkjaar. Waarom dit verschil met personen? Personen zijn altijd geldig, 
		// maar is dit wel de beste oplossing? Want alle leden zijn personen, maar wat dan als ze weggaan en dan terugkomen? 
		// Moeten ze dan expliciet gedeletet worden?...?

		/// <summary>
		/// Haalt een pagina met ledengegevens in een bepaald groepswerkjaar op
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar waar het over gaat</param>
		/// <param name="sortering">Het soort gegevens waarop er gesorteerd moet worden</param>
		/// <returns>Lijst van leden met hun relevante informatie</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IList<PersoonLidInfo> PaginaOphalen(int groepsWerkJaarID, LedenSorteringsEnum sortering);

		/// <summary>
		/// Haalt een pagina met ledengegevens in een bepaald groepswerkjaar op, maar alleen leden uit de gegeven afdeling
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het betreffende groepwerkjaar</param>
		/// <param name="afdelingsID">ID van de betreffende afdeling</param>
		/// <param name="sortering">Het soort gegevens waarop er gesorteerd moet worden</param>
		/// <returns>Lijst van leen met hun relevante informatie</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IList<PersoonLidInfo> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID, LedenSorteringsEnum sortering);

		/// <summary>
		/// Haalt een pagina met ledengegevens in een bepaald groepswerkjaar op, maar alleen leden met de gegeven functie
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het betreffende groepwerkjaar</param>
		/// <param name="functieID">ID van de betreffende functie</param>
		/// <param name="sortering">Het soort gegevens waarop er gesorteerd moet worden</param>
		/// <returns>Lijst van leden met hun relevante informatie</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IList<PersoonLidInfo> PaginaOphalenVolgensFunctie(int groepsWerkJaarID, int functieID, LedenSorteringsEnum sortering);

		/// <summary>
		/// Haalt lid op, inclusief gelieerde persoon, persoon, groep, afdelingen en functies
		/// </summary>
		/// <param name="lidID">ID op te halen lid</param>
		/// <returns>Lidinfo; bevat info over gelieerde persoon, persoon, groep, afdelingen 
		/// en functies </returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		PersoonLidInfo DetailsOphalen(int lidID);

		/// <summary>
		/// Haalt informatie op over alle leden uit het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// die lid zijn in de afdeling bepaald door <paramref name="afdID"/>.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar waaruit de leden opgehaald moeten worden</param>
		/// <param name="afdID">ID van de afdeling waaruit de leden opgehaald moeten worden.</param>
		/// <returns>Een rij 'LidOverzicht'-objecten met informatie over de betreffende leden.</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IList<LidOverzicht> OphalenUitAfdelingsJaar(int groepsWerkJaarID, int afdID);

		/// <summary>
		/// Haalt informatie op over alle leden uit het groepswerkjaar bepaald door 
		/// <paramref name="groepsWerkJaarID"/> die de functie bepaald door <paramref name="functieID"/> hebben.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van groepswerkjaar waaruit leden moeten worden opgehaald</param>
		/// <param name="functieID">ID van functie die opgehaalde leden moeten hebben</param>
		/// <returns>Een rij `LidOverzicht'-objecten met informatie over de betreffende leden.</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IList<LidOverzicht> OphalenUitFunctie(int groepsWerkJaarID, int functieID);

		/// <summary>
		/// Haalt informatie op over alle leden uit een gegeven groepswerkjaar
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar waaruit de leden moeten worden opgehaald</param>
		/// <returns>Een rij `LidOverzicht'-objecten met informatie over de betreffende leden.</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IList<LidOverzicht> OphalenUitGroepsWerkJaar(int groepsWerkJaarID);

		#endregion
	}
}
