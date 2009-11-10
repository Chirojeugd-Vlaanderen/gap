using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
    /// <summary>
    /// Dummy KindDao; doet niets
    /// </summary>
    public class DummyKindDao: IKindDao
    {
        #region IDao<Kind> Members

        public Kind Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Kind Ophalen(int id, params System.Linq.Expressions.Expression<Func<Kind, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Kind> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Kind Bewaren(Kind nieuweEntiteit)
        {
            throw new NotImplementedException();
        }

        public Kind Bewaren(Kind entiteit, params System.Linq.Expressions.Expression<Func<Kind, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Kind> Bewaren(IEnumerable<Kind> es, params System.Linq.Expressions.Expression<Func<Kind, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public System.Linq.Expressions.Expression<Func<Kind, object>>[] getConnectedEntities()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
