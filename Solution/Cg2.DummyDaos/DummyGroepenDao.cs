using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;

namespace Cg2.Dummies
{
    public class DummyGroepenDao: IGroepenDao
    {
        #region IGroepenDao Members

        public Cg2.Orm.GroepsWerkJaar RecentsteGroepsWerkJaarGet(int groepID)
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.Groep OphalenMetAfdelingen(int groepID)
        {
            throw new NotImplementedException();
        }

        public IList<Cg2.Orm.OfficieleAfdeling> OphalenOfficieleAfdelingen()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDao<Groep> Members

        public Cg2.Orm.Groep Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.Groep Ophalen(int id, params System.Linq.Expressions.Expression<Func<Cg2.Orm.Groep, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public List<Cg2.Orm.Groep> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.Groep Bewaren(Cg2.Orm.Groep nieuweEntiteit)
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.Groep Bewaren(Cg2.Orm.Groep entiteit, params System.Linq.Expressions.Expression<Func<Cg2.Orm.Groep, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Cg2.Orm.Groep> Bewaren(IList<Cg2.Orm.Groep> es, params System.Linq.Expressions.Expression<Func<Cg2.Orm.Groep, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public System.Linq.Expressions.Expression<Func<Cg2.Orm.Groep, object>>[] getConnectedEntities()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
