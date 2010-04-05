using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
	/// <summary>
	/// Fake LedenDao voor tests.  Doet niets.
	/// </summary>
	public class DummyLedenDao : DummyDao<Lid>, ILedenDao
	{
		#region ILedenDao Members

		public IList<Lid> PaginaOphalen(int groepsWerkJaarID)
		{
			return new List<Lid>();
		}
		public IList<Lid> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID)
		{
			return new List<Lid>();
		}

		public IList<Lid> AllesOphalen(int groepsWerkJaarID)
		{
			return new List<Lid>();
		}

		public IList<Lid> PaginaOphalen(int groepsWerkJaarID, int pagina, int paginaGrootte, out int aantalTotaal)
		{
			aantalTotaal = 0;
			return new List<Lid>();
		}

		public Lid Ophalen(int gelieerdePersoonID, int groepsWerkJaarID)
		{
			return null;
		}

		public Lid Ophalen(int gelieerdePersoonID, int groepsWerkJaarID, params System.Linq.Expressions.Expression<Func<Lid, object>>[] paths)
		{
			return null;
		}

		public Lid OphalenMetDetails(int lidID)
		{
			return null;
		}

		public IList<Lid> OphalenUitFunctie(int functieID, int groepsWerkJaarID, params System.Linq.Expressions.Expression<Func<Lid, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<Lid> OphalenUitFunctie(NationaleFunctie f, int groepsWerkJaarID, params System.Linq.Expressions.Expression<Func<Lid, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public bool IsLeiding(int lidID)
		{
			throw new NotImplementedException();
		}

		public Lid OphalenViaPersoon(int gelieerdePersoonID, int groepsWerkJaarID)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
