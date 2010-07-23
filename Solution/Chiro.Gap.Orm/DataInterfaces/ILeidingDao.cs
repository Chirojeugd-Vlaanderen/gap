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
		/// Haalt alle leiding op uit een gegeven groepswerkjaar
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <param name="paths">Geeft aan welke entiteiten mee opgehaald moeten worden</param>
		/// <returns>Rij opgehaalde leiding</returns>
		IEnumerable<Leiding> OphalenUitGroepsWerkJaar(int groepsWerkJaarID, Expression<Func<Leiding, object>>[] paths);
	}
}
