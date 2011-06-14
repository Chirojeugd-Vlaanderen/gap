// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
	public class DummyGroepenDao : DummyDao<Groep>, IGroepenDao
	{
		#region IGroepenDao Members

		public GroepsWerkJaar RecentsteGroepsWerkJaarGet(int groepID)
		{
			throw new NotImplementedException();
		}

		public Groep OphalenMetGroepsWerkJaren(int groepID)
		{
			throw new NotImplementedException();
		}

		public Groep Ophalen(string code)
		{
			throw new NotImplementedException();
		}

		public Groep OphalenMetAfdelingen(int groepID)
		{
			throw new NotImplementedException();
		}

		public GroepsWerkJaar GroepsWerkJaarOphalen(int groepsWerkJaarID)
		{
			throw new NotImplementedException();
		}

	#endregion
    }
}
