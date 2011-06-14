// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Linq.Expressions;

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

		public AfdelingsJaar Ophalen(int groepsWerkJaarID, int afdelingID, params Expression<Func<AfdelingsJaar, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
