// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
	/// <summary>
	/// Dummy LeidingDao die niets implementeert
	/// </summary>
	public class DummyLeidingDao : DummyDao<Leiding>, ILeidingDao
	{
		public IEnumerable<Leiding> Zoeken(LidFilter filter, Expression<Func<Leiding, object>>[] paths)
		{
			throw new NotImplementedException();
		}
	}
}
