using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;

namespace Cg2.Dummies
{
    /// <summary>
    /// Dummy AfdelingsJarenDao, die niets implementeert
    /// </summary>
    class DummyAfdelingsJarenDao: IAfdelingsJarenDao
    {
        #region IAfdelingsJaarDao Members

        public Cg2.Orm.AfdelingsJaar Ophalen(int groepsWerkJaarID, int afdelingID)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDao<AfdelingsJaar> Members

        public Cg2.Orm.AfdelingsJaar Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.AfdelingsJaar Ophalen(int id, params System.Linq.Expressions.Expression<Func<Cg2.Orm.AfdelingsJaar, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public List<Cg2.Orm.AfdelingsJaar> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.AfdelingsJaar Bewaren(Cg2.Orm.AfdelingsJaar nieuweEntiteit)
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.AfdelingsJaar Bewaren(Cg2.Orm.AfdelingsJaar entiteit, params System.Linq.Expressions.Expression<Func<Cg2.Orm.AfdelingsJaar, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Cg2.Orm.AfdelingsJaar> Bewaren(IList<Cg2.Orm.AfdelingsJaar> es, params System.Linq.Expressions.Expression<Func<Cg2.Orm.AfdelingsJaar, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public System.Linq.Expressions.Expression<Func<Cg2.Orm.AfdelingsJaar, object>>[] getConnectedEntities()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
