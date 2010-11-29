// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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
			return new List<Kind>();
		}

		public IEnumerable<Kind> OphalenUitAfdelingsJaar(int groepsWerkJaarID, int afdelingID, Expression<Func<Kind, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Kind> OphalenUitFunctie(int groepsWerkJaarID, int functieID, Expression<Func<Kind, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Kind> ProbeerLedenOphalen(int groepID, Expression<Func<Kind, object>>[] paths)
		{
			throw new NotImplementedException();
		}
	}
}
