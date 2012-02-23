// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
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
	/// Dummy UitstappenDao, die niets implementeert
	/// </summary>
	public class DummyUitstappenDao : DummyDao<Uitstap>, IUitstappenDao
	{
		public IEnumerable<Uitstap> OphalenVanGroep(int groepID, bool inschrijvenMogelijk)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Deelnemer> DeelnemersOphalen(int uitstapID)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Uitstap> AlleBivakkenOphalen(int werkjaar)
		{
			throw new NotImplementedException();
		}
	}
}
