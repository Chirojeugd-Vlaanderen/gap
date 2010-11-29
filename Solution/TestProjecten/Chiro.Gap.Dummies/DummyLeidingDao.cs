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
	/// Dummy LeidingDao die niets implementeert
	/// </summary>
	public class DummyLeidingDao : DummyDao<Leiding>, ILeidingDao
	{
		public IEnumerable<Leiding> OphalenUitGroepsWerkJaar(int groepsWerkJaarID, Expression<Func<Leiding, object>>[] paths)
		{
			return new List<Leiding>();
		}

		public IEnumerable<Leiding> OphalenUitAfdelingsJaar(int groepsWerkJaarID, int afdelingID, Expression<Func<Leiding, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Leiding> OphalenUitFunctie(int groepsWerkJaarID, int functieID, Expression<Func<Leiding, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Leiding> ProbeerLedenOphalen(int groepID, Expression<Func<Leiding, object>>[] paths)
		{
			throw new NotImplementedException();
		}
	}
}
