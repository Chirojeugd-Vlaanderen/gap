// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;

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
