using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
    /// <summary>
    /// Dummy KindDao; doet niets
    /// </summary>
    public class DummyKindDao: IKindDao
    {
        #region IDao<Kind> Members

        public Chiro.Gap.Orm.Kind Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.Kind Ophalen(int id, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Kind, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public List<Chiro.Gap.Orm.Kind> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.Kind Bewaren(Chiro.Gap.Orm.Kind nieuweEntiteit)
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.Kind Bewaren(Chiro.Gap.Orm.Kind entiteit, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Kind, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Chiro.Gap.Orm.Kind> Bewaren(IList<Chiro.Gap.Orm.Kind> es, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Kind, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Kind, object>>[] getConnectedEntities()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
