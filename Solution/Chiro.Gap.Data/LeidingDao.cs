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
	public class LeidingDao : Dao<Leiding, ChiroGroepEntities>, ILeidingDao
	{
		public LeidingDao()
		{
			connectedEntities = new Expression<Func<Leiding, object>>[3] { 
                                        e => e.GroepsWerkJaar.WithoutUpdate(), 
                                        e => e.GelieerdePersoon.WithoutUpdate(), 
                                        e => e.AfdelingsJaar.First().WithoutUpdate() };
		}
	}
}
