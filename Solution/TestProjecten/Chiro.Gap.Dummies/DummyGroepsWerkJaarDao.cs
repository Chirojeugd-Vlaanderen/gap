// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
	/// <summary>
	/// Dummy GroepsWerkJaarDao voor unit tests
	/// </summary>
	public class DummyGroepsWerkJaarDao : IGroepsWerkJaarDao
	{
		#region IGroepsWerkJaarDao Members

		public GroepsWerkJaar RecentsteOphalen(int groepID, params Expression<Func<GroepsWerkJaar, object>>[] paths)
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

		public GroepsWerkJaar Ophalen(int id)
		{
			throw new NotImplementedException();
		}

		public GroepsWerkJaar Ophalen(int id, params Expression<Func<GroepsWerkJaar, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<GroepsWerkJaar> Ophalen(IEnumerable<int> ids)
		{
			throw new NotImplementedException();
		}

		public IList<GroepsWerkJaar> Ophalen(IEnumerable<int> ids, params Expression<Func<GroepsWerkJaar, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<GroepsWerkJaar> PaginaOphalen(int id, Expression<Func<GroepsWerkJaar, int>> f, int pagina, int paginaGrootte, out int aantalTotaal, params Expression<Func<GroepsWerkJaar, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<GroepsWerkJaar> AllesOphalen()
		{
			throw new NotImplementedException();
		}

		public GroepsWerkJaar Bewaren(GroepsWerkJaar entiteit)
		{
			throw new NotImplementedException();
		}

		public GroepsWerkJaar Bewaren(GroepsWerkJaar entiteit, params Expression<Func<GroepsWerkJaar, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<GroepsWerkJaar> Bewaren(IEnumerable<GroepsWerkJaar> es, params Expression<Func<GroepsWerkJaar, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public Expression<Func<GroepsWerkJaar, object>>[] GetConnectedEntities()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
