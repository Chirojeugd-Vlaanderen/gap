using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using System.Diagnostics;
using System.Data.Objects;
using System.Data;
using Cg2.Orm.DataInterfaces;

using Cg2.EfWrapper.Entity;
using System.Linq.Expressions;

namespace Cg2.Data.Ef
{
    public class KindDao : Dao<Kind>, IKindDao
    {
        private Expression<Func<Kind, object>>[] connectedEntities = { e => e.GroepsWerkJaar, e => e.GelieerdePersoon, e => e.AfdelingsJaar };

        public override Expression<Func<Kind, object>>[] getConnectedEntities()
        {
            return connectedEntities;
        }
    }
}
