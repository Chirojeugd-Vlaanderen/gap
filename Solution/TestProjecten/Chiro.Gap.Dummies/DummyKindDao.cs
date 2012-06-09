// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
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
	/// Dummy KindDao; doet niets
	/// </summary>
	public class DummyKindDao : DummyDao<Kind>, IKindDao
	{
	    private int _id = 0;
	    private Dictionary<int, Kind> id2kind = new Dictionary<int, Kind>(); 

		public IEnumerable<Kind> Zoeken(LidFilter filter, params Expression<Func<Kind, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Kind> Zoeken(LidFilter filter, LidExtras extras)
		{
			throw new NotImplementedException();
		}

		public Kind Bewaren(Kind kind, LidExtras extras)
		{
		    kind.ID = ++_id;
            id2kind.Add(kind.ID, kind);
		    return kind;
		}

		public Kind Ophalen(int lidID, LidExtras extras)
		{
		    Kind kind = null;
		    id2kind.TryGetValue(lidID, out kind);
		    return kind;
		}
	}
}
