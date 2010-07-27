using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
	/// <summary>
	/// Dummy GroepsWerkJaarDao voor unit tests
	/// </summary>
	public class DummyGroepsWerkJaarDao : IGroepsWerkJaarDao
	{
		#region IGroepsWerkJaarDao Members

		public Chiro.Gap.Orm.GroepsWerkJaar RecentsteOphalen(int groepID, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.GroepsWerkJaar, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Voor 't gemak gaan we er altijd vanuit dat we in het recentste groepswerkjaar werken.
		/// </summary>
		/// <param name="groepsWerkJaarID"></param>
		/// <returns></returns>
		public bool IsRecentste(int groepsWerkJaarID)
		{
			return true;
		}

		#endregion

		#region IDao<GroepsWerkJaar> Members

		public Chiro.Gap.Orm.GroepsWerkJaar Ophalen(int id)
		{
			throw new NotImplementedException();
		}

		public Chiro.Gap.Orm.GroepsWerkJaar Ophalen(int id, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.GroepsWerkJaar, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<Chiro.Gap.Orm.GroepsWerkJaar> Ophalen(IEnumerable<int> ids)
		{
			throw new NotImplementedException();
		}

		public IList<Chiro.Gap.Orm.GroepsWerkJaar> Ophalen(IEnumerable<int> ids, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.GroepsWerkJaar, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<Chiro.Gap.Orm.GroepsWerkJaar> PaginaOphalen(int id, System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.GroepsWerkJaar, int>> f, int pagina, int paginaGrootte, out int aantalTotaal, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.GroepsWerkJaar, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<Chiro.Gap.Orm.GroepsWerkJaar> AllesOphalen()
		{
			throw new NotImplementedException();
		}

		public Chiro.Gap.Orm.GroepsWerkJaar Bewaren(Chiro.Gap.Orm.GroepsWerkJaar entiteit)
		{
			throw new NotImplementedException();
		}

		public Chiro.Gap.Orm.GroepsWerkJaar Bewaren(Chiro.Gap.Orm.GroepsWerkJaar entiteit, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.GroepsWerkJaar, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Chiro.Gap.Orm.GroepsWerkJaar> Bewaren(IEnumerable<Chiro.Gap.Orm.GroepsWerkJaar> es, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.GroepsWerkJaar, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.GroepsWerkJaar, object>>[] getConnectedEntities()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
