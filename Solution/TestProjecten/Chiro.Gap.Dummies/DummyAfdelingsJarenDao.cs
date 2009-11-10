using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;


namespace Chiro.Gap.Dummies
{
    /// <summary>
    /// Dummy AfdelingsJarenDao, die niets implementeert
    /// </summary>
    class DummyAfdelingsJarenDao: IAfdelingsJarenDao
    {
        #region IAfdelingsJaarDao Members

        public AfdelingsJaar Ophalen(int groepsWerkJaarID, int afdelingID)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDao<AfdelingsJaar> Members

        public AfdelingsJaar Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public AfdelingsJaar Ophalen(int id, params System.Linq.Expressions.Expression<Func<AfdelingsJaar, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<AfdelingsJaar> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public AfdelingsJaar Bewaren(AfdelingsJaar nieuweEntiteit)
        {
            throw new NotImplementedException();
        }

        public AfdelingsJaar Bewaren(AfdelingsJaar entiteit, params System.Linq.Expressions.Expression<Func<AfdelingsJaar, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AfdelingsJaar> Bewaren(IEnumerable<AfdelingsJaar> es, params System.Linq.Expressions.Expression<Func<AfdelingsJaar, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public System.Linq.Expressions.Expression<Func<AfdelingsJaar, object>>[] getConnectedEntities()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
