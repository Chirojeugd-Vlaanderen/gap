using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
    /// <summary>
    /// Dummy LeidingDao die niets implementeert
    /// </summary>
    public class DummyLeidingDao: ILeidingDao
    {
        #region IDao<Leiding> Members

        public Leiding Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Leiding Ophalen(int id, params System.Linq.Expressions.Expression<Func<Leiding, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Leiding> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Leiding Bewaren(Leiding nieuweEntiteit)
        {
            throw new NotImplementedException();
        }

        public Leiding Bewaren(Leiding entiteit, params System.Linq.Expressions.Expression<Func<Leiding, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Leiding> Bewaren(IEnumerable<Leiding> es, params System.Linq.Expressions.Expression<Func<Leiding, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public System.Linq.Expressions.Expression<Func<Leiding, object>>[] getConnectedEntities()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
