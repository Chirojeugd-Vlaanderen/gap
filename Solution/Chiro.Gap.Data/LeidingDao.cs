using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm;
using System.Diagnostics;
using System.Data.Objects;
using System.Data;
using Chiro.Gap.Orm.DataInterfaces;

using System.Linq.Expressions;
using Chiro.Cdf.Data.Entity;


namespace Chiro.Gap.Data.Ef
{
	public class LeidingDao : Dao<Leiding, ChiroGroepEntities>, ILeidingDao
	{
		public LeidingDao()
		{
			connectedEntities = new Expression<Func<Leiding, object>>[3] { 
                                        e => e.GroepsWerkJaar, 
                                        e => e.GelieerdePersoon, 
                                        e => e.AfdelingsJaar };
		}
	}
}
