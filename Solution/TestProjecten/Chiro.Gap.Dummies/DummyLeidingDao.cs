using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
	/// <summary>
	/// Dummy LeidingDao die niets implementeert
	/// </summary>
	public class DummyLeidingDao : DummyDao<Leiding>, ILeidingDao
	{
		public IEnumerable<Leiding> OphalenUitGroepsWerkJaar(int groepsWerkJaarID, Expression<Func<Leiding, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Leiding> OphalenUitAfdelingsJaar(int groepsWerkJaarID, int afdelingID, Expression<Func<Leiding, object>>[] paths)
		{
			throw new NotImplementedException();
		}
	}
}
