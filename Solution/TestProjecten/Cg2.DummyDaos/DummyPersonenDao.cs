using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;

namespace Cg2.Dummies
{
    /// <summary>
    /// Dummy PersonenDao, die niets implementeert
    /// </summary>
    public class DummyPersonenDao: IPersonenDao
    {
        #region IPersonenDao Members

        public IList<Cg2.Orm.Persoon> LijstOphalen(IList<int> personenIDs)
        {
            throw new NotImplementedException();
        }

        public IList<Cg2.Orm.Persoon> HuisGenotenOphalen(int gelieerdePersoonID)
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.Persoon CorresponderendePersoonOphalen(int gelieerdePersoonID)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDao<Persoon> Members

        public Cg2.Orm.Persoon Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.Persoon Ophalen(int id, params System.Linq.Expressions.Expression<Func<Cg2.Orm.Persoon, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public List<Cg2.Orm.Persoon> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.Persoon Bewaren(Cg2.Orm.Persoon nieuweEntiteit)
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.Persoon Bewaren(Cg2.Orm.Persoon entiteit, params System.Linq.Expressions.Expression<Func<Cg2.Orm.Persoon, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Cg2.Orm.Persoon> Bewaren(IList<Cg2.Orm.Persoon> es, params System.Linq.Expressions.Expression<Func<Cg2.Orm.Persoon, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public System.Linq.Expressions.Expression<Func<Cg2.Orm.Persoon, object>>[] getConnectedEntities()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
