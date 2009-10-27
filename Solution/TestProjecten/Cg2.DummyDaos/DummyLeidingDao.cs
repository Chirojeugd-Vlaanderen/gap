using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;

namespace Cg2.Dummies
{
    /// <summary>
    /// Dummy LeidingDao die niets implementeert
    /// </summary>
    public class DummyLeidingDao: ILeidingDao
    {
        #region IDao<Leiding> Members

        public Cg2.Orm.Leiding Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.Leiding Ophalen(int id, params System.Linq.Expressions.Expression<Func<Cg2.Orm.Leiding, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public List<Cg2.Orm.Leiding> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.Leiding Bewaren(Cg2.Orm.Leiding nieuweEntiteit)
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.Leiding Bewaren(Cg2.Orm.Leiding entiteit, params System.Linq.Expressions.Expression<Func<Cg2.Orm.Leiding, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Cg2.Orm.Leiding> Bewaren(IList<Cg2.Orm.Leiding> es, params System.Linq.Expressions.Expression<Func<Cg2.Orm.Leiding, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public System.Linq.Expressions.Expression<Func<Cg2.Orm.Leiding, object>>[] getConnectedEntities()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
