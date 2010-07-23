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
	/// Dummy KindDao; doet niets
	/// </summary>
	public class DummyKindDao : DummyDao<Kind>, IKindDao
	{
		public IEnumerable<Kind> OphalenUitGroepsWerkJaar(int groepsWerkJaarID, Expression<Func<Kind, object>>[] paths)
		{
			throw new NotImplementedException();
		}
	}
}
