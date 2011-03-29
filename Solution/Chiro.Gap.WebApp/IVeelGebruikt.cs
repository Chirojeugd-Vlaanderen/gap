using System.Collections.Generic;

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp
{
	public interface IVeelGebruikt
	{
		/// <summary>
		/// Verwijdert de gecachete functieproblemen van een bepaalde groep
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan de problemencache leeg te maken</param>
		/// <param name="cache">cache die de problemen bevat</param>
		void FunctieProblemenResetten(int groepID);

		/// <summary>
		/// Haalt de problemen van de groep ivm functies op.  Uit de cache, of als die gegevens niet
		/// aanwezig zijn, via de service.
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan de functieproblemen opgehaald moeten worden</param>
		/// <returns>De rij functieproblemen</returns>
		IEnumerable<FunctieProbleemInfo> FunctieProblemenOphalen(int groepID);

		/// <summary>
		/// Haalt WoonPlaatsInfo op voor woonplaatsen met gegeven <paramref name="postNummer"/>
		/// </summary>
		/// <param name="postNummer">Postnummer waarvan de woonplaatsen gevraagd zijn</param>
		/// <returns>WoonPlaatsInfo voor woonplaatsen met gegeven <paramref name="postNummer"/></returns>
		IEnumerable<WoonPlaatsInfo> WoonPlaatsenOphalen(int postNummer);

		/// <summary>
		/// Levert een lijst op van alle woonplaatsen
		/// </summary>
		/// <returns>Een lijst met alle beschikbare woonplaatsen</returns>
		IEnumerable<WoonPlaatsInfo> WoonPlaatsenOphalen();

		/// <summary>
		/// Verwijdert het recentste groepswerkjaar van groep met ID <paramref name="groepID"/>
		/// uit de cache.
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan groepswerkjaarcache te resetten</param>
		void GroepsWerkJaarResetten(int groepID);

		/// <summary>
		/// Haalt het recentste groepswerkjaar van de groep met gegeven <paramref name="GroepID"/>
		/// op uit de cache, of - indien niet beschikbaar - van backend.
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan we het groepswerkjaar willen weten.</param>
		/// <returns>Details over het gevraagde groepswerkjaar.</returns>
		GroepsWerkJaarDetail GroepsWerkJaarOphalen(int groepID);

		/// <summary>
		/// Als de gav met gegeven <paramref name="login"/> gav is van precies 1 groep, dan
		/// levert deze method het ID van die groep op.  Zo niet, is het resultaat
		/// <c>0</c>.
		/// </summary>
		/// <param name="login">Login van de GAV</param>
		/// <returns>GroepID van unieke groep van gegeven GAV, anders <c>0</c></returns>
		int UniekeGroepGav(string login);

		/// <summary>
		/// Indien <c>true</c> werken we in de liveomgeving, anders in de testomgeving.
		/// </summary>
		/// <returns><c>true</c> als we live bezig zijn</returns>
		bool IsLive();

		/// <summary>
		/// Haalt een lijstje op met informatie over ontbrekende gegevens bij leden.
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan we de problemen opvragen</param>
		/// <returns>Een rij LedenProbleemInfo met info over de ontbrekende gegevens</returns>
		IEnumerable<LedenProbleemInfo> LedenProblemenOphalen(int groepID);

		/// <summary>
		/// Verwijdert de gecachete ledenproblemen van een bepaalde groep
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan de problemencache leeg te maken</param>
		void LedenProblemenResetten(int groepID);

		/// <summary>
		/// Haalt alle landen op van de backend
		/// </summary>
		/// <returns>De landinfo van alle gekende landen</returns>
		IEnumerable<LandInfo> LandenOphalen();
	}
}