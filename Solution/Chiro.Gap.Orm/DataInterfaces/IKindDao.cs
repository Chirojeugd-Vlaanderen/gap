// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Linq.Expressions;

using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
	/// <summary>
	/// Interface voor een gegevenstoegangsobject voor Kinderen
	/// </summary>
	public interface IKindDao : IDao<Kind>
	{
		/// <summary>
		/// Haalt alle kinderen op uit een gegeven groepswerkjaar
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <param name="paths">Geeft aan welke entiteiten mee opgehaald moeten worden</param>
		/// <returns>Rij opgehaalde kinderen</returns>
		IEnumerable<Kind> OphalenUitGroepsWerkJaar(int groepsWerkJaarID, Expression<System.Func<Kind, object>>[] paths);
	}
}
