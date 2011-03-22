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
	/// Interface voor een gegevenstoegangsobject voor Kinderen
	/// </summary>
	public interface IKindDao : IDao<Kind>
	{
		/// <summary>
		/// Zoekt ingeschreven kinderen op, op basis van de gegeven <paramref name="filter"/>.
		/// </summary>
		/// <param name="filter">De niet-nulle properties van de filter
		/// bepalen waarop gezocht moet worden</param>
		/// <param name="extras">Bepaalt de mee op te halen gekoppelde entiteiten. 
		/// (Adressen ophalen vertraagt aanzienlijk.)
		/// </param>
		/// <returns>Lijst met info over gevonden kinderen</returns>
		/// <remarks>
		/// Er worden enkel actieve leden opgehaald
		/// </remarks>
		IEnumerable<Kind> Zoeken(LidFilter filter, LidExtras extras);

		/// <summary>
		/// Bewaart een kind, inclusief de extras gegeven in <paramref name="extras"/>
		/// </summary>
		/// <param name="kind">Te bewaren kind</param>
		/// <param name="extras">Bepaalt de gekoppelde entiteiten die mee bewaard moeten worden</param>
		/// <returns>Kopie van het bewaarde kind</returns>
		Kind Bewaren(Kind kind, LidExtras extras);

		/// <summary>
		/// Haalt een kind op, samen met de gekoppelde entiteiten bepaald door <paramref name="extras"/>.
		/// </summary>
		/// <param name="lidID">ID van op te halen kind</param>
		/// <param name="extras">bepaalt de mee op te halen gekoppelde entiteiten</param>
		/// <returns>Het gevraagde kind</returns>
		Kind Ophalen(int lidID, LidExtras extras);
	}
}
