using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
    /// <summary>
    /// Dummy AfdelingsJarenDao, die niets implementeert
    /// </summary>
    class DummyAfdelingsJarenDao: IAfdelingsJarenDao
    {
        #region IAfdelingsJaarDao Members

        public Chiro.Gap.Orm.AfdelingsJaar Ophalen(int groepsWerkJaarID, int afdelingID)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDao<AfdelingsJaar> Members

        public Chiro.Gap.Orm.AfdelingsJaar Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.AfdelingsJaar Ophalen(int id, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.AfdelingsJaar, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public List<Chiro.Gap.Orm.AfdelingsJaar> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.AfdelingsJaar Bewaren(Chiro.Gap.Orm.AfdelingsJaar nieuweEntiteit)
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.AfdelingsJaar Bewaren(Chiro.Gap.Orm.AfdelingsJaar entiteit, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.AfdelingsJaar, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Chiro.Gap.Orm.AfdelingsJaar> Bewaren(IList<Chiro.Gap.Orm.AfdelingsJaar> es, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.AfdelingsJaar, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.AfdelingsJaar, object>>[] getConnectedEntities()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
