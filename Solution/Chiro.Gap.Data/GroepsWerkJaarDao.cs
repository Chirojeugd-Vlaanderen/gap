// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Cdf.Data.Entity;
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
		/// <param name="paths">Lambda-expressies die bepalen welke gekoppelde entiteiten mee opgehaald moeten worden.</param>
		/// <returns>Groepswerkjaar van groep met ID <paramref name="groepID"/>, met daaraan gekoppeld de
		/// groep en de afdelingsjaren.</returns>
		public GroepsWerkJaar RecentsteOphalen(int groepID, params Expression<Func<GroepsWerkJaar, object>>[] paths)
		{
			GroepsWerkJaar result;
			using (var db = new ChiroGroepEntities())
			{
				var query = (
					from wj in db.GroepsWerkJaar
					where wj.Groep.ID == groepID
					orderby wj.WerkJaar descending
					select wj) as ObjectQuery<GroepsWerkJaar>;

				query = IncludesToepassen(query, paths);

				result = query.FirstOrDefault();
			}
			result = Utility.DetachObjectGraph(result);

			return result;
		}

		/// <summary>
		/// Haalt het recentste groepswerkjaar op van de groep waar de gelieerde persoon <paramref name="gp"/>
		/// aan gekoppeld is.
		/// </summary>
		/// <param name="gp">Gelieerde persoon met op te halen groepswerkjaar</param>
		/// <param name="paths">Bepaalt de mee op te halen gekoppelde entiteiten</param>
		/// <returns>Het gevraagde groepswerkjaar met de gevraagde gekoppelde entiteiten</returns>
		public GroepsWerkJaar RecentsteOphalen(GelieerdePersoon gp, params Expression<Func<GroepsWerkJaar, object>>[] paths)
		{
			GroepsWerkJaar result;
			using (var db = new ChiroGroepEntities())
			{
				var query = (
					from wj in db.GroepsWerkJaar
					where wj.Groep.GelieerdePersoon.Any(gelp => gelp.ID == gp.ID)
					orderby wj.WerkJaar descending
					select wj) as ObjectQuery<GroepsWerkJaar>;

				query = IncludesToepassen(query, paths);

				result = query.FirstOrDefault();
			}
			result = Utility.DetachObjectGraph(result);

			return result;
		}

		/// <summary>
		/// Kijkt na of het groepswerkjaar met ID <paramref name="groepsWerkJaarID"/> het recentste groepswerkjaar
		/// van zijn groep is.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <returns><c>True</c> alss het groepswerkjaar het recentste is</returns>
		public bool IsRecentste(int groepsWerkJaarID)
		{
			int recentsteID;

			using (var db = new ChiroGroepEntities())
			{
				var gwjQuery = (from g in db.Groep
				             where g.GroepsWerkJaar.Any(gwj => gwj.ID == groepsWerkJaarID)
				             select g.GroepsWerkJaar).FirstOrDefault();

				recentsteID = (from gwj in gwjQuery
				                      orderby gwj.WerkJaar descending
				                      select gwj.ID).FirstOrDefault();
			}
			return recentsteID == groepsWerkJaarID;
		}
	}
}
