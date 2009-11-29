using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Cdf.Data;
using System.Linq.Expressions;

namespace Chiro.Gap.Dummies
{
	public class DummyDao<T>: IDao<T>
	{
		#region IDao<T> Members

		public T Ophalen(int id)
		{
			throw new NotImplementedException();
		}

		public T Ophalen(int id, params System.Linq.Expressions.Expression<Func<T, object>>[] paths)
		{
			throw new NotImplementedException();
		}

        public IList<T> PaginaOphalen(int id, int pagina, int paginaGrootte, out int aantalTotaal, params Expression<Func<T, object>>[] paths)
        {
            throw new NotImplementedException();
        }

		public IList<T> AllesOphalen()
		{
			throw new NotImplementedException();
		}

		public T Bewaren(T nieuweEntiteit)
		{
			throw new NotImplementedException();
		}

		public T Bewaren(T entiteit, params System.Linq.Expressions.Expression<Func<T, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<T> Bewaren(IEnumerable<T> es, params System.Linq.Expressions.Expression<Func<T, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public System.Linq.Expressions.Expression<Func<T, object>>[] getConnectedEntities()
		{
			throw new NotImplementedException();
		}

		public IList<T> Ophalen(IEnumerable<int> ids)
		{
			throw new NotImplementedException();
		}

		public IList<T> Ophalen(IEnumerable<int> ids, params System.Linq.Expressions.Expression<Func<T, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
