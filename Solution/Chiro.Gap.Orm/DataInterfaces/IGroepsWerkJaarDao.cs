// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Linq.Expressions;

using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
	/// <summary>
	/// Interface voor een gegevenstoegangsobject voor GroepsWerkjaren
	/// </summary>
	public interface IGroepsWerkJaarDao : IDao<GroepsWerkJaar>
	{
		/// <summary>
		/// Haalt recentste groepswerkjaar op van groep met ID <paramref name="groepID"/>, inclusief 
		/// afdelingsjaren.
		/// </summary>
		/// <param name="groepID">ID van groep waarvan het recentste groepswerkjaar gevraagd is.</param>
		/// <param name="paths">Lambda-expressies die bepalen welke gekoppelde entiteiten mee opgehaald moeten worden.</param>
		/// <returns>Groepswerkjaar van groep met ID <paramref name="groepID"/>, met daaraan gekoppeld de
		/// groep en de afdelingsjaren.</returns>
		GroepsWerkJaar RecentsteOphalen(int groepID, params Expression<Func<GroepsWerkJaar, object>>[] paths);
	}
}
