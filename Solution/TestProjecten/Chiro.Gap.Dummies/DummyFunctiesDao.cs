using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
	public class DummyFunctiesDao: IFunctiesDao
	{
		#region IFunctiesDao Members

		public Chiro.Gap.Orm.Functie Ophalen(Chiro.Gap.Orm.GepredefinieerdeFunctieType f)
		{
			throw new NotImplementedException();
		}

		public int AantalLeden(int groepID, int functieID)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IDao<Functie> Members

		public Chiro.Gap.Orm.Functie Ophalen(int id)
		{
			throw new NotImplementedException();
		}

		public Chiro.Gap.Orm.Functie Ophalen(int id, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Functie, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<Chiro.Gap.Orm.Functie> Ophalen(IEnumerable<int> ids)
		{
			throw new NotImplementedException();
		}

		public IList<Chiro.Gap.Orm.Functie> Ophalen(IEnumerable<int> ids, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Functie, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<Chiro.Gap.Orm.Functie> PaginaOphalen(int id, System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Functie, int>> f, int pagina, int paginaGrootte, out int aantalTotaal, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Functie, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<Chiro.Gap.Orm.Functie> AllesOphalen()
		{
			throw new NotImplementedException();
		}

		public Chiro.Gap.Orm.Functie Bewaren(Chiro.Gap.Orm.Functie entiteit)
		{
			throw new NotImplementedException();
		}

		public Chiro.Gap.Orm.Functie Bewaren(Chiro.Gap.Orm.Functie entiteit, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Functie, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Chiro.Gap.Orm.Functie> Bewaren(IEnumerable<Chiro.Gap.Orm.Functie> es, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Functie, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Functie, object>>[] getConnectedEntities()
		{
			throw new NotImplementedException();
		}

		public int AantalLeden(int groepID, Chiro.Gap.Orm.GepredefinieerdeFunctieType f)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
