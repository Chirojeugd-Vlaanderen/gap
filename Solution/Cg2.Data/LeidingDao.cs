using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using System.Diagnostics;
using System.Data.Objects;
using System.Data;
using Cg2.Orm.DataInterfaces;

using System.Linq.Expressions;
using Cg2.EfWrapper.Entity;


namespace Chiro.Gap.Data.Ef
{
    public class LeidingDao : Dao<Leiding>, ILeidingDao
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
