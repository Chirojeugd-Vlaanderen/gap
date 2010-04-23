// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
	/// <summary>
	/// Interface voor een gegevenstoegangsobject voor Leden
	/// </summary>
	public interface ILedenDao : IDao<Lid>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="groepsWerkJaarID"></param>
		/// <returns></returns>
		IList<Lid> AllesOphalen(int groepsWerkJaarID);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="groepsWerkJaarID"></param>
		/// <param name="afdelingsID"></param>
		/// <returns></returns>
		IList<Lid> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID);

		/// <summary>
		/// Zoekt lid op op basis van GelieerdePersoonID en GroepsWerkJaarID
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van gelieerde persoon</param>
		/// <param name="groepsWerkJaarID">ID van groepswerkjaar</param>
		/// <returns>Lidobject indien gevonden, anders null</returns>
		Lid Ophalen(int gelieerdePersoonID, int groepsWerkJaarID);

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
		/// 
		/// </summary>
		/// <param name="lidID"></param>
		/// <returns></returns>
		Lid OphalenMetDetails(int lidID);

		/// <summary>
		/// Geeft <c>true</c> indien het lid met <paramref name="lidID"/> leiding is, anders <c>false</c>
		/// </summary>
		/// <param name="lidID">ID van lid waarvoor na te gaan of het al dan niet leiding is</param>
		/// <returns><c>true</c> indien het lid met <paramref name="lidID"/> leiding is, anders <c>false</c></returns>
		bool IsLeiding(int lidID);

		// void LidMaken(int gelieerdeID);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gelieerdePersoonID"></param>
		/// <param name="groepsWerkJaarID"></param>
		/// <returns></returns>
		Lid OphalenViaPersoon(int gelieerdePersoonID, int groepsWerkJaarID);
	}
}
