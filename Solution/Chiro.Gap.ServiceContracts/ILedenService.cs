// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.ServiceModel;

namespace Chiro.Gap.ServiceContracts
{
	// NOTE: If you change the interface name "ILedenService" here, you must also update the reference to "ILedenService" in Web.config.

	[ServiceContract]
	public interface ILedenService
	{
		/// <summary>
		/// Gaat een gelieerde persoon ophalen en maakt die lid in het huidige werkjaar. Hiervoor krijgt het
		/// de eigenschappen die overeenkomen met zijn/haar leeftijd, maar er moet wel de juiste methode worden
		/// aangeroepen om een kind of een leiding te maken
		/// </summary>
		/// <param name="gelieerdePersoonIDs">ID's van de gelieerde persoon</param>
		/// <returns>De ids van de personen die lid zijn gemaakt</returns>
		/// <throws>OngeldigeActieException als om een of andere reden, minstens 1 van de personen geen lid gemaakt 
		/// kan worden. In dat geval wordt geen enkele persoon lid. </throws>
		/// <remarks>De methode is reentrant, dus zal niet klagen als er personen al lid zijn.</remarks>
		[OperationContract]
		IEnumerable<int> LedenMakenEnBewaren(IEnumerable<int> gelieerdePersoonIDs);

		/// <summary>
		/// </summary>
		/// <param name="gelieerdePersoonIDs"></param>
		/// <returns></returns>
		[OperationContract]
		IEnumerable<int> LeidingMakenEnBewaren(IEnumerable<int> gelieerdePersoonIDs);

		/// <summary>
		/// Slaat veranderingen op aan de eigenschappen van het lidobject zelf. Creëert of verwijdert geen leden, en leden
		/// kunnen ook niet van werkjaar of van gelieerdepersoon veranderen.
		/// </summary>
		/// <param name="lid">Te bewaren lid</param>
		[OperationContract]
		void Bewaren(LidInfo lid);

		/// <summary>
		/// Vervangt de functies van het lid bepaald door <paramref name="lidID"/> door de functies
		/// met ID's <paramref name="functieIDs"/>
		/// </summary>
		/// <param name="lidID">ID van lid met te vervangen functies</param>
		/// <param name="functieIDs">IDs van nieuwe functies voor het lid</param>
		[OperationContract]
		void FunctiesVervangen(int lidID, IEnumerable<int> functieIDs);

		/// <summary>
		/// Haalt de ID's van de groepswerkjaren van een lid op.
		/// </summary>
		/// <param name="lidID">ID van het lid waarin we geinteresseerd zijn</param>
		/// <returns>Een LidAfdelingInfo-object</returns>
		[OperationContract]
		LidAfdelingInfo AfdelingenOphalen(int lidID);

		/// <summary>
		/// Vervangt de afdelingen van het lid met ID <paramref name="lidID"/> door de afdelingen
		/// met AFDELINGSJAARIDs gegeven door <paramref name="afdelingsJaarIDs"/>.
		/// </summary>
		/// <param name="lidID">Lid dat nieuwe afdelingen moest krijgen</param>
		/// <param name="afdelingsJaarIDs">ID's van de te koppelen afdelingsjaren</param>
		/// <returns>De GelieerdePersoonID van het lid</returns>
		[OperationContract]
		int AfdelingenVervangen(int lidID, IEnumerable<int> afdelingsJaarIDs);

		/// <summary>
		/// </summary>
		/// <param name="lid"></param>
		/// <returns></returns>
		[OperationContract]
		LidInfo BewarenMetVrijeVelden(LidInfo lid);

		/// <summary>
		/// Verwijdert het lid met ID <paramref name="lidID"/>
		/// </summary>
		/// <param name="lidID">ID van het te verwijderen lid</param>
		[OperationContract]
		void Verwijderen(int lidID);

		// TODO: we vragen leden op per groepswerkjaar. Waarom dit verschil met personen? Personen zijn altijd geldig, 
		// maar is dit wel de beste oplossing? Want alle leden zijn personen, maar wat dan als ze weggaan en dan terugkomen? 
		// Moeten ze dan expliciet gedeletet worden?...?

		/// <summary>
		/// Haalt een pagina met ledengegevens in een bepaald groepswerkjaar
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar waar het over gaat</param>
		/// <param name="paginas">Het totaal aantal pagina's</param>
		/// <returns>Lijst van leden met hun relevante informatie</returns>
		[OperationContract]
		IList<LidInfo> PaginaOphalen(int groepsWerkJaarID, out int paginas);

		/// <summary>
		/// Haalt een pagina met ledengegevens in een bepaald groepswerkjaar, maar alleen leden uit de gegeven afdeling
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het betreffende groepwerkjaar</param>
		/// <param name="afdelingsID">ID van de betreffende afdeling</param>
		/// <param name="paginas">Het totaal aantal pagians</param>
		/// <returns>Lijst van leen met hun relevante informatie</returns>
		[OperationContract]
		IList<LidInfo> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID, out int paginas);

		/// <summary>
		/// Haalt lid op, inclusief gelieerde persoon, persoon, groep, afdelingen en functies
		/// </summary>
		/// <param name="lidID">ID op te halen lid</param>
		/// <returns>Lidinfo; bevat info over gelieerde persoon, persoon, groep, afdelingen 
		/// en functies </returns>
		[OperationContract]
		LidInfo DetailsOphalen(int lidID);
	}
}
