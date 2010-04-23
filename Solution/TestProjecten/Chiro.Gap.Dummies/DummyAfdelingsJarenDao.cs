using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;


namespace Chiro.Gap.Dummies
{
	/// <summary>
	/// Dummy AfdelingsJarenDao, die niets implementeert
	/// </summary>
	class DummyAfdelingsJarenDao : DummyDao<AfdelingsJaar>, IAfdelingsJarenDao
	{
		#region IAfdelingsJaarDao Members

		public AfdelingsJaar Ophalen(int groepsWerkJaarID, int afdelingID)
		{
			throw new NotImplementedException();
		}

		public AfdelingsJaar Ophalen(int groepsWerkJaarID, int afdelingID, params System.Linq.Expressions.Expression<Func<AfdelingsJaar, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
