using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm.DataInterfaces;

namespace Cg2.Dummies
{
    /// <summary>
    /// Dummy LeidingDao die niets implementeert
    /// </summary>
    public class DummyLeidingDao: ILeidingDao
    {
        #region IDao<Leiding> Members

        public Chiro.Gap.Orm.Leiding Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.Leiding Ophalen(int id, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Leiding, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public List<Chiro.Gap.Orm.Leiding> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.Leiding Bewaren(Chiro.Gap.Orm.Leiding nieuweEntiteit)
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.Leiding Bewaren(Chiro.Gap.Orm.Leiding entiteit, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Leiding, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Chiro.Gap.Orm.Leiding> Bewaren(IList<Chiro.Gap.Orm.Leiding> es, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Leiding, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Leiding, object>>[] getConnectedEntities()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
