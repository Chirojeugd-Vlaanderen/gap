using System.Collections.Generic;
using System.ServiceModel;

using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;

namespace Chiro.Gap.ServiceContracts
{
	/// <summary>
	/// Service voor beheer van uitstappen en bivakken
	/// </summary>
	[ServiceContract]
	public interface IUitstappenService
	{
		/// <summary>
		/// Bewaart een uitstap aan voor de groep met gegeven <paramref name="groepID"/>
		/// </summary>
		/// <param name="groepID">ID van de groep horende bij de uitstap.
		///   Is eigenlijk enkel relevant als het om een nieuwe uitstap gaat.</param>
		/// <param name="info">Details over de uitstap.  Als <c>uitstap.ID</c> <c>0</c> is,
		///   dan wordt een nieuwe uitstap gemaakt.  Anders wordt de bestaande overschreven.</param>
		/// <returns>ID van de uitstap</returns>
		[OperationContract]
		[FaultContract(typeof(OngeldigObjectFault))]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		int Bewaren(int groepID, UitstapInfo info);

		/// <summary>
		/// Haalt alle uitstappen van een gegeven groep op.
		/// </summary>
		/// <param name="groepID">ID van de groep</param>
		/// <param name="inschrijvenMogelijk">Als deze <c>true</c> is, worden enkel de uitstappen opgehaald
		/// waarvoor je nog kunt inschrijven.  In praktijk zijn dit de uitstappen van het huidige werkjaar.
		/// </param>
		/// <returns>Details van uitstappen</returns>
		/// <remarks>We laten toe om inschrijvingen te doen voor uitstappen uit het verleden, om als dat
		/// nodig is achteraf fouten in de administratie recht te zetten.</remarks>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IEnumerable<UitstapInfo> OphalenVanGroep(int groepID, bool inschrijvenMogelijk);

		/// <summary>
		/// Haalt details over uitstap met gegeven <paramref name="uitstapID"/> op.
		/// </summary>
		/// <param name="uitstapID">ID van de uitstap</param>
		/// <returns>Details over de uitstap</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		UitstapDetail DetailsOphalen(int uitstapID);

		/// <summary>
		/// Bewaart de plaats voor een uitstap
		/// </summary>
		/// <param name="id">ID van de uitstap</param>
		/// <param name="plaatsNaam">naam van de plaats</param>
		/// <param name="adres">adres van de plaats</param>
		[OperationContract]
		[FaultContract(typeof(OngeldigObjectFault))]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		void PlaatsBewaren(int id, string plaatsNaam, AdresInfo adres);

		/// <summary>
		/// Schrijft de gelieerde personen met ID's <paramref name="gelieerdePersoonIDs"/> in voor de
		/// uitstap met ID <paramref name="geselecteerdeUitstapID" />.  Als
		/// <paramref name="logistiekDeelnemer" /> <c>true</c> is, wordt er ingeschreven als
		/// logistiek deelnemer.
		/// </summary>
		/// <param name="gelieerdePersoonIDs">ID's van in te schrijven gelieerde personen</param>
		/// <param name="geselecteerdeUitstapID">ID van uitstap waarvoor in te schrijven</param>
		/// <param name="logistiekDeelnemer">Bepaalt of al dan niet ingeschreven wordt als 
		/// logistieker</param>
		/// <returns>De basisgegevens van de uitstap, zodat die in de feedback gebruikt kan worden</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		UitstapInfo Inschrijven(IList<int> gelieerdePersoonIDs, int geselecteerdeUitstapID, bool logistiekDeelnemer);

		/// <summary>
		/// Haalt informatie over alle deelnemers van de uitstap met gegeven <paramref name="uitstapID"/> op.
		/// </summary>
		/// <param name="uitstapID">ID van de relevante uitstap</param>
		/// <returns>informatie over alle deelnemers van de uitstap met gegeven <paramref name="uitstapID"/></returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		IEnumerable<DeelnemerDetail> DeelnemersOphalen(int uitstapID);

		/// <summary>
		/// Stelt de deelnemer met gegeven <paramref name="deelnemerID" /> in als contactpersoon voor de uitstap
		/// waaraan hij deelneemt
		/// </summary>
		/// <param name="deelnemerID">ID van de als contact in te stellen deelnemer</param>
		/// <returns>De ID van de uitstap, ter controle, en misschien handig voor feedback</returns>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		[FaultContract(typeof(FoutNummerFault))]
		int ContactInstellen(int deelnemerID);

        /// <summary>
        /// Schrijft de deelnemer met gegeven <paramref name="deelnemerID"/> uit voor zijn uitstap.
        /// </summary>
        /// <param name="deelnemerID">ID uit te schrijven deelnemer</param>
        /// <returns>ID van de uitstap, ter controle, en h andig voor feedback</returns>
        [OperationContract]
        [FaultContract(typeof(GapFault))]
        [FaultContract(typeof(FoutNummerFault))]
	    int Uitschrijven(int deelnemerID);

        /// <summary>
        /// Haalt informatie over de deelnemer met ID <paramref name="deelnemerID"/> op.
        /// </summary>
        /// <param name="deelnemerID">ID van de relevante deelnemer</param>
        /// <returns>informatie over de deelnemer met ID <paramref name="deelnemerID"/></returns>
        [OperationContract]
        [FaultContract(typeof(GapFault))]
        [FaultContract(typeof(FoutNummerFault))]
	    DeelnemerDetail DeelnemerOphalen(int deelnemerID);

        /// <summary>
        /// Updatet een deelnemer op basis van de info in <paramref name="info"/>
        /// </summary>
        /// <param name="info">info nodig voor de update</param>
        [OperationContract]
        [FaultContract(typeof(GapFault))]
        [FaultContract(typeof(FoutNummerFault))]
	    void DeelnemerBewaren(DeelnemerInfo info);

		/// <summary>
		/// Haalt informatie over de bivakaangifte op van de groep <paramref name="groepID"/> in het gegeven werkjaar
		/// </summary>
		/// <param name="groepID">De groep waarvan info wordt gevraagd</param>
		/// <param name="gwjID">Het werkjaar waarover info wordt gevraagd</param>
		[OperationContract]
		[FaultContract(typeof(GapFault))]
		BivakAangifteLijstInfo BivakStatusOphalen(int groepID, int gwjID);
	}
}
