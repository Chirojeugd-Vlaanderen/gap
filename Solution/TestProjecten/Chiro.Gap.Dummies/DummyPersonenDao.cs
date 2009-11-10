using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;


namespace Chiro.Gap.Dummies
{
    /// <summary>
    /// Dummy PersonenDao, die niets implementeert
    /// </summary>
    public class DummyPersonenDao: IPersonenDao
    {
        #region IPersonenDao Members

        public IList<Persoon> LijstOphalen(IList<int> personenIDs)
        {
            throw new NotImplementedException();
        }

        public IList<Persoon> HuisGenotenOphalen(int gelieerdePersoonID)
        {
            throw new NotImplementedException();
        }

        public Persoon CorresponderendePersoonOphalen(int gelieerdePersoonID)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDao<Persoon> Members

        public Persoon Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Persoon Ophalen(int id, params System.Linq.Expressions.Expression<Func<Persoon, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Persoon> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Persoon Bewaren(Persoon nieuweEntiteit)
        {
            throw new NotImplementedException();
        }

        public Persoon Bewaren(Persoon entiteit, params System.Linq.Expressions.Expression<Func<Persoon, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Persoon> Bewaren(IEnumerable<Persoon> es, params System.Linq.Expressions.Expression<Func<Persoon, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public System.Linq.Expressions.Expression<Func<Persoon, object>>[] getConnectedEntities()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
