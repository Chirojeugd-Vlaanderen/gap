using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm;
using System.Diagnostics;
using System.Data.Objects;
using System.Data;
using Chiro.Gap.Orm.DataInterfaces;

using Chiro.Cdf.Data.Entity;
using System.Linq.Expressions;

namespace Chiro.Gap.Data.Ef
{
	public class KindDao : Dao<Kind, ChiroGroepEntities>, IKindDao
	{
		public KindDao()
		{
			connectedEntities = new Expression<Func<Kind, object>>[3] { 
                                        e => e.GroepsWerkJaar, 
                                        e => e.GelieerdePersoon, 
                                        e => e.AfdelingsJaar };
		}
	}
}
