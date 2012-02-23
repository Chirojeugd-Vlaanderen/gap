// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;

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
		public IEnumerable<Leiding> Zoeken(LidFilter filter, LidExtras extras)
		{
			throw new NotImplementedException();
		}

		public Leiding Bewaren(Leiding entiteit, LidExtras extras)
		{
			throw new NotImplementedException();
		}

		public Leiding Ophalen(int lidID, LidExtras extras)
		{
			throw new NotImplementedException();
		}
	}
}
