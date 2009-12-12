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
	public class DummyLedenDao: DummyDao<Lid>, ILedenDao
	{
		#region ILedenDao Members

		public IList<Lid> PaginaOphalen(int groepsWerkJaarID)
		{
			throw new NotImplementedException();
		}

		public IList<Lid> PaginaOphalen(int groepsWerkJaarID, int afdelingsID)
		{
			throw new NotImplementedException();
		}

        public IList<Lid> AllesOphalen(int groepsWerkJaarID)
        {
            throw new NotImplementedException();
        }

        public IList<Lid> PaginaOphalen(int groepsWerkJaarID, int pagina, int paginaGrootte, out int aantalTotaal)
        {
            throw new NotImplementedException();
        }

		public Lid Ophalen(int gelieerdePersoonID, int groepsWerkJaarID)
		{
			throw new NotImplementedException();
		}

		public Lid Ophalen(int gelieerdePersoonID, int groepsWerkJaarID, params System.Linq.Expressions.Expression<Func<Lid, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public Lid OphalenMetDetails(int lidID)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
