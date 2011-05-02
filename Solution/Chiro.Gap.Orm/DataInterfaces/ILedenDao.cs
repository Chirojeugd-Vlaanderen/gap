// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Chiro.Cdf.Data;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Orm.DataInterfaces
{
	/// <summary>
	/// Interface voor een gegevenstoegangsobject voor Leden
	/// </summary>
	public interface ILedenDao : IDao<Lid>
	{
		/// <summary>
		/// Een lijst ophalen van alle leden voor het opgegeven groepswerkjaar
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <param name="sortering">Parameter waarop de gegevens gesorteerd moeten worden</param>
		/// <returns>Een lijst alle leden voor het opgegeven groepswerkjaar</returns>
		IList<Lid> AllesOphalen(int groepsWerkJaarID, LidEigenschap sortering);

		/// <summary>
		/// Een lijst ophalen van alle actieve leden voor het opgegeven groepswerkjaar
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <param name="sortering">Parameter waarop de gegevens gesorteerd moeten worden</param>
		/// <returns>Een lijst alle actieve leden voor het opgegeven groepswerkjaar</returns>
		IList<Lid> ActieveLedenOphalen(int groepsWerkJaarID, LidEigenschap sortering);
        
		/// <summary>
		/// Haalt een pagina op van de gevraagde gegevens:
		/// leden van een bepaalde groep in een gegeven werkjaar, die in de gegeven afdeling zitten
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het aan een groep gekoppelde werkjaar</param>
		/// <param name="afdelingsID">ID van de afdeling waar de leden in moeten zitten</param>
		/// <param name="sortering">Parameter waarop de gegevens gesorteerd zijn</param>
		/// <returns>De leden die de groep in dat werkjaar heeft/had en die in de gegeven afdeling zitten/zaten</returns>
		/// <remarks>
		/// Pagineren gebeurt per werkjaar.
		/// </remarks>
		IList<Lid> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID, LidEigenschap sortering);

		/// <summary>
		/// Haalt een pagina op van de gevraagde gegevens:
		/// leden van een bepaalde groep in een gegeven werkjaar, die een bepaalde functie hebben/hadden
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het aan een groep gekoppelde werkjaar</param>
		/// <param name="functieID">ID van de functie die de leden moeten hebben</param>
		/// <param name="sortering">Parameter waarop de gegevens gesorteerd zijn</param>
		/// <returns>De leden met de gegeven functie die de groep in dat werkjaar heeft/had</returns>
		/// <remarks>
		/// Pagineren gebeurt per werkjaar.
		/// Haalt GEEN afdeling mee op (nakijken of dit ook effectief niet nodig is?)
		/// </remarks>
		IList<Lid> PaginaOphalenVolgensFunctie(int groepsWerkJaarID, int functieID, LidEigenschap sortering);

		/// <summary>
		/// Zoekt lid op op basis van GelieerdePersoonID en GroepsWerkJaarID
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van gelieerde persoon</param>
		/// <param name="groepsWerkJaarID">ID van groepswerkjaar</param>
		/// <returns>Lidobject indien gevonden, anders null</returns>
		Lid Ophalen(int gelieerdePersoonID, int groepsWerkJaarID);

		/// <summary>
		/// Haalt een gemengde lijst leden (leden en leiding) op, samen met
		/// de gekoppelde entiteiten bepaald door <paramref name="extras"/>
		/// </summary>
		/// <param name="lidIDs">LidIDs op te halen leden</param>
		/// <param name="extras">bepaalt op te halen gekoppelde entiteiten</param>
		/// <returns>De gevraagde lijst leden</returns>
		IEnumerable<Lid> Ophalen(IEnumerable<int> lidIDs, LidExtras extras);

		/// <summary>
		/// Haalt lid (kind of leiding) op, samen met
		/// de gekoppelde entiteiten bepaald door <paramref name="extras"/>
		/// </summary>
		/// <param name="lidID">LidID op te halen lid</param>
		/// <param name="extras">bepaalt op te halen gekoppelde entiteiten</param>
		/// <returns>De gevraagde lijst leden</returns>
		Lid Ophalen(int lidID, LidExtras extras);

		/// <summary>
		/// Haalt de leden op die in het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// de functie bepaald door <paramref name="functieID"/> hebben
		/// </summary>
		/// <param name="functieID">ID van een functie</param>
		/// <param name="groepsWerkJaarID">ID van een groepswerkjaar</param>
		/// <param name="paths">Bepaalt de mee op te halen gekoppelde entiteiten</param>
		/// <returns>Lijst leden die in het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// de functie bepaald door <paramref name="functieID"/> hebben.</returns>
		IList<Lid> OphalenUitFunctie(
			int functieID,
			int groepsWerkJaarID,
			params Expression<Func<Lid, object>>[] paths);

		/// <summary>
		/// Haalt de leden op die in het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// de gepredefinieerde functie met type <paramref name="f"/> hebben.
		/// </summary>
		/// <param name="f">Type gepredefinieerde functie</param>
		/// <param name="groepsWerkJaarID">ID van een groepswerkjaar</param>
		/// <param name="paths">Bepaalt de mee op te halen gekoppelde entiteiten</param>
		/// <returns>Lijst leden die in het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// de gepredefinieerde functie met type <paramref name="f"/> hebben.</returns>
		IList<Lid> OphalenUitFunctie(
			NationaleFunctie f,
			int groepsWerkJaarID,
			params Expression<Func<Lid, object>>[] paths);

		/// <summary>
		/// Haalt lid met gerelateerde entity's op, op basis van 
		/// GelieerdePersoonID en GroepsWerkJaarID
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van gelieerde persoon</param>
		/// <param name="groepsWerkJaarID">ID van groepswerkjaar</param>
		/// <param name="paths">Lambda-expressies die de extra op te halen
		/// informatie definiëren</param>
		/// <returns>Lidobject indien gevonden, anders <c>null</c></returns>
		Lid Ophalen(int gelieerdePersoonID, int groepsWerkJaarID, params Expression<Func<Lid, object>>[] paths);

		/// <summary>
		/// Lid met afdelingsjaren, afdelingen en gelieerdepersoon
		/// </summary>
		/// <param name="lidID">ID van het lid waarvan we gegevens willen opvragen</param>
		/// <returns>Een lid met afdelingsjaren, afdelingen en gelieerdepersoon</returns>
		Lid OphalenMetDetails(int lidID);

		/// <summary>
		/// Geeft <c>true</c> indien het lid met <paramref name="lidID"/> leiding is, anders <c>false</c>
		/// </summary>
		/// <param name="lidID">ID van lid waarvoor na te gaan of het al dan niet leiding is</param>
		/// <returns><c>true</c> indien het lid met <paramref name="lidID"/> leiding is, anders <c>false</c></returns>
		bool IsLeiding(int lidID);

		// void LidMaken(int gelieerdeID);

		/// <summary>
		/// Haalt het lid op bepaald door <paramref name="gelieerdePersoonID"/> en
		/// <paramref name="groepsWerkJaarID"/>, inclusief persoon, afdelingen, functies, groepswerkjaar
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van de gelieerde persoon waarvoor het lidobject gevraagd is.</param>
		/// <param name="groepsWerkJaarID">ID van groepswerkjaar in hetwelke het lidobject gevraagd is</param>
		/// <returns>
		/// Het lid bepaald door <paramref name="gelieerdePersoonID"/> en
		/// <paramref name="groepsWerkJaarID"/>, inclusief persoon, afdelingen, functies, groepswerkjaar
		/// </returns>
		Lid OphalenViaPersoon(int gelieerdePersoonID, int groepsWerkJaarID);

		/// <summary>
		/// Haalt hoogstens <paramref name="maxAantal"/> leden op met probeerperiode die voorbij is, 
		/// inclusief persoonsgegevens, adressen, communicatie, functies, afdelingen
		/// </summary>
		/// <param name="maxAantal">max aantal leden op te halen</param>
		/// <returns>alle leden met probeerperiode die voorbij is, inclusief persoonsgegevens, adressen,
		/// functies, afdelingen.  Communicatie niet!</returns>
		IEnumerable<Lid> OverTeZettenOphalen(int maxAantal);
	}
}
