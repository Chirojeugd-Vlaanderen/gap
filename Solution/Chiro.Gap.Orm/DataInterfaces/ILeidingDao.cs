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
	/// Interface voor een gegevenstoegangsobject voor Leiding
	/// </summary>
	public interface ILeidingDao : IDao<Leiding>
	{
		/// <summary>
		/// Zoekt leiding op, op basis van de gegeven <paramref name="filter"/>.
		/// </summary>
		/// <param name="filter">De niet-nulle properties van de filter
		/// bepalen waarop gezocht moet worden</param>
		/// <param name="paths">Bepaalt de mee op te halen gekoppelde entiteiten. 
		/// (Adressen ophalen vertraagt aanzienlijk.)
		/// </param>
		/// <returns>Lijst met info over gevonden leiding</returns>
		/// <remarks>
		/// Er wordt enkel actieve leiding opgehaald
		/// </remarks>
		IEnumerable<Leiding> Zoeken(Chiro.Gap.Domain.LidFilter filter, params Expression<Func<Leiding, object>>[] paths);
	}
}
