using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
	public class DummyAfdelingenDao : IAfdelingenDao
	{
		#region IDao<Afdeling> Members

		public Chiro.Gap.Orm.Afdeling Ophalen(int id)
		{
			throw new NotImplementedException();
		}

		public Chiro.Gap.Orm.Afdeling Ophalen(int id, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Afdeling, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<Chiro.Gap.Orm.Afdeling> Ophalen(IEnumerable<int> ids)
		{
			throw new NotImplementedException();
		}

		public IList<Chiro.Gap.Orm.Afdeling> Ophalen(IEnumerable<int> ids, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Afdeling, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<Chiro.Gap.Orm.Afdeling> PaginaOphalen(int id, System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Afdeling, int>> f, int pagina, int paginaGrootte, out int aantalTotaal, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Afdeling, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<Chiro.Gap.Orm.Afdeling> AllesOphalen()
		{
			throw new NotImplementedException();
		}

		public Chiro.Gap.Orm.Afdeling Bewaren(Chiro.Gap.Orm.Afdeling entiteit)
		{
			throw new NotImplementedException();
		}

		public Chiro.Gap.Orm.Afdeling Bewaren(Chiro.Gap.Orm.Afdeling entiteit, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Afdeling, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Chiro.Gap.Orm.Afdeling> Bewaren(IEnumerable<Chiro.Gap.Orm.Afdeling> es, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Afdeling, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Afdeling, object>>[] getConnectedEntities()
		{
			throw new NotImplementedException();
		}

		#endregion


		#region IAfdelingenDao Members

		public IList<Chiro.Gap.Orm.Afdeling> OngebruikteOphalen(int groepsWerkJaarID)
		{
			throw new NotImplementedException();
		}

		public IList<Chiro.Gap.Orm.OfficieleAfdeling> OfficieleAfdelingenOphalen()
		{
			throw new NotImplementedException();
		}

		public Chiro.Gap.Orm.OfficieleAfdeling OfficieleAfdelingOphalen(int officieleAfdelingID)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
