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


namespace Cg2.Data.Ef
{
    public class LeidingDao : Dao<Leiding>, ILeidingDao
    {
        private Expression<Func<Leiding, object>>[] connectedEntities = { e => e.GroepsWerkJaar, e => e.GelieerdePersoon, e => e.AfdelingsJaar };

        public override Expression<Func<Leiding, object>>[] getConnectedEntities()
        {
            return connectedEntities;
        }

    }
}
