// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	public class KindDao : Dao<Kind, ChiroGroepEntities>, IKindDao
	{
		public KindDao()
		{
			connectedEntities = new Expression<Func<Kind, object>>[] 
            { 
                                        e => e.GroepsWerkJaar.WithoutUpdate(), 
                                        e => e.GelieerdePersoon.WithoutUpdate(), 
										e => e.GelieerdePersoon.Persoon.WithoutUpdate(), 
										e => e.AfdelingsJaar.WithoutUpdate(),
                                        e => e.AfdelingsJaar.Afdeling.WithoutUpdate() 
            };
		}
	}
}
