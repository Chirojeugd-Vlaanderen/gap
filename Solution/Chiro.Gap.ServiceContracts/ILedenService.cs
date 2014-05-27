/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
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
	    /// <returns>De lidIds van de personen die lid zijn gemaakt</returns>
	    [OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		List<InschrijvingsVoorstel> InschrijvingVoorstellen(IList<int> gelieerdePersoonIDs);

		/// <summary>
		/// Probeert de opgegeven personen in te schrijven met de meegegeven informatie. Als dit niet mogelijk blijkt te zijn, wordt er niemand ingeschreven.
		/// </summary>
		/// <param name="inschrijfInfo">Lijst van informatie over wie lid moet worden</param>
		/// <param name="foutBerichten">Als er sommige personen geen lid konden worden gemaakt, bevat foutBerichten een string waarin wat uitleg staat. </param>
		/// <returns>De lidIds van de personen die lid zijn gemaakt</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
        IEnumerable<int> Inschrijven(InschrijvingsVoorstel[] inschrijfInfo, out string foutBerichten);

	    /// <summary>
        /// Schrijft de leden met gegeven <paramref name="gelieerdePersoonIDs"/> uit voor het huidige
        /// werkjaar. We gaan ervan uit dat ze allemaal ingeschreven zijn.
	    /// </summary>
	    /// <param name="gelieerdePersoonIDs">ID's van de gelieerde personen</param>
	    /// <param name="foutBerichten">Als voor sommige personen die actie mislukte, bevat foutBerichten een
	    ///     string waarin wat uitleg staat.</param>
	    [OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void Uitschrijven(IList<int> gelieerdePersoonIDs, out string foutBerichten);

		/// <summary>
		/// Vervangt de functies van het lid bepaald door <paramref name="lidId"/> door de functies
		/// met ID's <paramref name="functieIDs"/>
		/// </summary>
		/// <param name="lidId">ID van lid met te vervangen functies</param>
		/// <param name="functieIDs">IDs van nieuwe functies voor het lid</param>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void FunctiesVervangen(int lidId, IEnumerable<int> functieIDs);

		/// <summary>
		/// Haalt de ID's van de afdelingsjaren van een lid op.
		/// </summary>
		/// <param name="lidId">ID van het lid waarin we geinteresseerd zijn</param>
		/// <returns>Een LidAfdelingInfo-object</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		LidAfdelingInfo AfdelingenOphalen(int lidId);

	    /// <summary>
	    /// Vervangt de afdelingen van het lid met ID <paramref name="lidId"/> door de afdelingen
	    /// met AFDELINGSJAARIDs gegeven door <paramref name="afdelingsJaarIDs"/>.
	    /// </summary>
	    /// <param name="lidId">Lid dat nieuwe afdelingen moest krijgen</param>
	    /// <param name="afdelingsJaarIDs">ID's van de te koppelen afdelingsjaren</param>
	    /// <returns>De GelieerdePersoonID van het lid</returns>
	    [OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		int AfdelingenVervangen(int lidId, IList<int> afdelingsJaarIDs);

		/// <summary>
		/// Vervangt de afdelingen van de leden met gegeven <paramref name="lidIds"/> door de afdelingen
		/// met AFDELINGSJAARIDs gegeven door <paramref name="afdelingsJaarIDs"/>.
		/// </summary>
		/// <param name="lidIds">ID's van leden die nieuwe afdelingen moeten krijgen</param>
		/// <param name="afdelingsJaarIDs">ID's van de te koppelen afdelingsjaren</param>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void AfdelingenVervangenBulk(IList<int> lidIds, IList<int> afdelingsJaarIDs);

		/// <summary>
		/// Verzekert lid met ID <paramref name="lidId"/> tegen loonverlies
		/// </summary>
		/// <param name="lidId">ID van te verzekeren lid</param>
		/// <returns>GelieerdePersoonID van het verzekerde lid</returns>
		/// <remarks>Dit is nogal een specifieke method.  In ons domain model is gegeven dat verzekeringen gekoppeld zijn aan
		/// personen, voor een bepaalde periode.  Maar in eerste instantie zal alleen de verzekering loonverlies gebruikt worden,
		/// die per definitie enkel voor leden bestaat.</remarks>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		int LoonVerliesVerzekeren(int lidId);

		#region Ophalen

        // TODO (#1049): we vragen leden op per groepswerkjaar. Waarom dit verschil met personen? Personen zijn altijd geldig, 
		// maar is dit wel de beste oplossing? Want alle leden zijn personen, maar wat dan als ze weggaan en dan terugkomen? 
		// Moeten ze dan expliciet gedeletet worden?...?

		/// <summary>
		/// Haalt lid op, inclusief gelieerde persoon, persoon, groep, afdelingen en functies
		/// </summary>
		/// <param name="lidId">ID op te halen lid</param>
		/// <returns>Lidinfo; bevat info over gelieerde persoon, persoon, groep, afdelingen 
		/// en functies </returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		PersoonLidInfo DetailsOphalen(int lidId);

        /// <summary>
        /// Haalt persoonsgegevens op voor (actief) lid met gegeven <paramref name="lidID"/>
        /// </summary>
        /// <param name="lidID">ID van een lid</param>
        /// <returns>beperkte informatie over de person</returns>
        [OperationContract]
	    PersoonInfo PersoonOphalen(int lidID);

        /// <summary>
        /// Haalt beperkte lidinfo op voor (actief) lid met gegeven <paramref name="lidID"/>
        /// </summary>
        /// <param name="lidID">ID van een lid</param>
        /// <returns>beperkte lidinfo voor lid met gegeven <paramref name="lidID" /></returns>
        [OperationContract]
	    LidInfo LidInfoOphalen(int lidID);

		/// <summary>
		/// Zoekt leden op, op basis van de gegeven <paramref name="filter"/>. Levert een lijst van LidOverzicht af.
		/// </summary>
		/// <param name="filter">De niet-nulle properties van de filter
		/// bepalen waarop gezocht moet worden</param>
        /// <param name="metAdressen">Indien <c>true</c>, worden de
        /// adressen mee opgehaald. (Adressen ophalen vertraagt aanzienlijk.)
        /// </param>
		/// <returns>Lijst met LidOverzicht over gevonden leden</returns>
		/// <remarks>
		/// Er worden enkel actieve leden opgehaald.
		/// Let er ook op dat je in de filter iets opgeeft als LidType
		/// (Kind, Leiding of Alles), want anders krijg je niets terug.
		/// </remarks>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		List<LidOverzicht> LijstZoekenLidOverzicht(LidFilter filter, bool metAdressen);

        /// <summary>
        /// Zoekt leden op, op basis van de gegeven <paramref name="filter"/>. Levert een lijst van PersoonLidInfo af.
        /// </summary>
        /// <param name="filter">De niet-nulle properties van de filter
        /// bepalen waarop gezocht moet worden</param>
        /// <returns>Lijst met PersoonLidInfo over gevonden leden</returns>
        /// <remarks>
        /// Er worden enkel actieve leden opgehaald.
        /// Let er ook op dat je in de filter iets opgeeft als LidType
        /// (Kind, Leiding of Alles), want anders krijg je niets terug.
        /// </remarks>
        [OperationContract]
        [FaultContract(typeof(GapFault))]
        [FaultContract(typeof(FoutNummerFault))]
        List<PersoonLidInfo> LijstZoekenPersoonLidInfo(LidFilter filter);

		#endregion

		/// <summary>
		/// Togglet het vlagje 'lidgeld betaald' van het lid met lidId <paramref name="lidId"/>.  Geeft als resultaat
		/// het GelieerdePersoonID.  (Niet proper, maar wel interessant voor redirect.)
		/// </summary>
		/// <param name="lidId">ID van lid met te togglen lidgeld</param>
		/// <returns>GelieerdePersoonID van lid</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		int LidGeldToggle(int lidId);

		/// <summary>
		/// Verandert een kind in leiding of vice versa
		/// </summary>
		/// <param name="lidId">ID van lid met te togglen lidtype</param>
		/// <returns>GelieerdePersoonID van lid</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		int TypeToggle(int lidId);

	}
}
