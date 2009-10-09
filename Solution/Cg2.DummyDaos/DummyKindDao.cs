using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;

namespace Cg2.Dummies
{
    /// <summary>
    /// Dummy KindDao; doet niets
    /// </summary>
    public class DummyKindDao: IKindDao
    {
        #region IDao<Kind> Members

        public Cg2.Orm.Kind Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.Kind Ophalen(int id, params System.Linq.Expressions.Expression<Func<Cg2.Orm.Kind, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public List<Cg2.Orm.Kind> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.Kind Bewaren(Cg2.Orm.Kind nieuweEntiteit)
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.Kind Bewaren(Cg2.Orm.Kind entiteit, params System.Linq.Expressions.Expression<Func<Cg2.Orm.Kind, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Cg2.Orm.Kind> Bewaren(IList<Cg2.Orm.Kind> es, params System.Linq.Expressions.Expression<Func<Cg2.Orm.Kind, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public System.Linq.Expressions.Expression<Func<Cg2.Orm.Kind, object>>[] getConnectedEntities()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
