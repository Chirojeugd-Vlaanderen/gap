using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
	/// <summary>
	/// Dummy CategorieenDao, die niets doet
	/// </summary>
	public class DummyCategorieenDao : DummyDao<Categorie>, ICategorieenDao
	{
		#region ICategorieenDao Members

		public IList<Categorie> AllesOphalen(int groepID)
		{
			throw new NotImplementedException();
		}

		public Categorie Ophalen(int groepID, string code)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
