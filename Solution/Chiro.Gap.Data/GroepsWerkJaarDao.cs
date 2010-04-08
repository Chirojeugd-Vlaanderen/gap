// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Gegevenstoegangsobject voor GroepsWerkJaren
	/// </summary>
	public class GroepsWerkJaarDao : Dao<GroepsWerkJaar, ChiroGroepEntities>, IGroepsWerkJaarDao
	{
		/// <summary>
		/// Haalt recentste groepswerkjaar op van groep met ID <paramref name="groepID"/>, inclusief 
		/// afdelingsjaren.
		/// </summary>
		/// <param name="groepID">ID van groep waarvan het recentste groepswerkjaar gevraagd is.</param>
		/// <param name="paths">lambda-expressies die bepalen welke gekoppelde entiteiten mee opgehaald moeten worden.</param>
		/// <returns>Groepswerkjaar van groep met ID <paramref name="groepID"/>, met daaraan gekoppeld de
		/// groep en de afdelingsjaren.</returns>
		public GroepsWerkJaar RecentsteOphalen(int groepID, params Expression<Func<GroepsWerkJaar, object>>[] paths)
		{
			GroepsWerkJaar result;
			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				var query = (
					from wj in db.GroepsWerkJaar
					where wj.Groep.ID == groepID
					orderby wj.WerkJaar descending
					select wj) as ObjectQuery<GroepsWerkJaar>;

				query = IncludesToepassen(query, paths);

				result = query.FirstOrDefault<GroepsWerkJaar>();
			}
			result = Utility.DetachObjectGraph(result);

			return result;
		}
	}
}
