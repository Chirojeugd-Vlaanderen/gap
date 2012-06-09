// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
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
		/// Genereert de lijst van inteschrijven leden met de informatie die ze zouden krijgen als ze automagisch zouden worden ingeschreven, gebaseerd op een lijst van in te schrijven gelieerde personen.
		/// </summary>
		/// <param name="gelieerdePersoonIDs">Lijst van gelieerde persoonIDs waarover we inforamtie willen</param>
		/// <param name="foutBerichten">Als er sommige personen geen lid gemaakt werden, bevat foutBerichten een string waarin wat uitleg staat.</param>
		/// <returns>De LidIDs van de personen die lid zijn gemaakt</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IEnumerable<InTeSchrijvenLid> VoorstelTotInschrijvenGenereren(IEnumerable<int> gelieerdePersoonIDs, out string foutBerichten);

		/// <summary>
		/// Probeert de opgegeven personen in te schrijven met de meegegeven informatie. Als dit niet mogelijk blijkt te zijn, wordt er niemand ingeschreven.
		/// </summary>
		/// <param name="lidInformatie">Lijst van informatie over wie lid moet worden</param>
		/// <param name="foutBerichten">Als er sommige personen geen lid konden worden gemaakt, bevat foutBerichten een string waarin wat uitleg staat. </param>
		/// <returns>De LidIDs van de personen die lid zijn gemaakt</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IEnumerable<int> Inschrijven(IEnumerable<InTeSchrijvenLid> lidInformatie, out string foutBerichten);

		/// <summary>
		/// Maakt lid met gegeven ID nonactief
		/// </summary>
		/// <param name="gelieerdePersoonIDs">ID's van de gelieerde personen</param>
		/// <param name="foutBerichten">Als voor sommige personen die actie mislukte, bevat foutBerichten een
		/// string waarin wat uitleg staat.</param>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void Uitschrijven(IEnumerable<int> gelieerdePersoonIDs, out string foutBerichten);

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
		/// Vervangt de afdelingen van de leden met gegeven <paramref name="lidIDs"/> door de afdelingen
		/// met AFDELINGSJAARIDs gegeven door <paramref name="afdelingsJaarIDs"/>.
		/// </summary>
		/// <param name="lidIDs">ID's van leden die nieuwe afdelingen moeten krijgen</param>
		/// <param name="afdelingsJaarIDs">ID's van de te koppelen afdelingsjaren</param>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void AfdelingenVervangenBulk(IEnumerable<int> lidIDs, IEnumerable<int> afdelingsJaarIDs);

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

        // TODO (#1049): we vragen leden op per groepswerkjaar. Waarom dit verschil met personen? Personen zijn altijd geldig, 
		// maar is dit wel de beste oplossing? Want alle leden zijn personen, maar wat dan als ze weggaan en dan terugkomen? 
		// Moeten ze dan expliciet gedeletet worden?...?

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
		/// Zoekt leden op, op basis van de gegeven <paramref name="filter"/>.
		/// </summary>
		/// <param name="filter">De niet-nulle properties van de filter
		/// bepalen waarop gezocht moet worden</param>
		/// <param name="metAdressen">Indien <c>true</c>, worden de
		/// adressen mee opgehaald. (Adressen ophalen vertraagt aanzienlijk.)
		/// </param>
		/// <returns>Lijst met info over gevonden leden</returns>
		/// <remarks>
		/// Er worden enkel actieve leden opgehaald.
		/// Let er ook op dat je in de filter iets opgeeft als LidType
		/// (Kind, Leiding of Alles), want anders krijg je niets terug.
		/// </remarks>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IList<LidOverzicht> Zoeken(LidFilter filter, bool metAdressen);

		#endregion

		/// <summary>
		/// Togglet het vlagje 'lidgeld betaald' van het lid met LidID <paramref name="id"/>.  Geeft als resultaat
		/// het GelieerdePersoonID.  (Niet proper, maar wel interessant voor redirect.)
		/// </summary>
		/// <param name="id">ID van lid met te togglen lidgeld</param>
		/// <returns>GelieerdePersoonID van lid</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		int LidGeldToggle(int id);

		/// <summary>
		/// Verandert een kind in leiding of vice versa
		/// </summary>
		/// <param name="id">ID van lid met te togglen lidtype</param>
		/// <returns>GelieerdePersoonID van lid</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		int TypeToggle(int id, out string FoutBerichten);
	}
}
