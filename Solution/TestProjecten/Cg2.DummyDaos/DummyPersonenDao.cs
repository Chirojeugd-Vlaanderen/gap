using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm.DataInterfaces;

namespace Cg2.Dummies
{
    /// <summary>
    /// Dummy PersonenDao, die niets implementeert
    /// </summary>
    public class DummyPersonenDao: IPersonenDao
    {
        #region IPersonenDao Members

        public IList<Chiro.Gap.Orm.Persoon> LijstOphalen(IList<int> personenIDs)
        {
            throw new NotImplementedException();
        }

        public IList<Chiro.Gap.Orm.Persoon> HuisGenotenOphalen(int gelieerdePersoonID)
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.Persoon CorresponderendePersoonOphalen(int gelieerdePersoonID)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDao<Persoon> Members

        public Chiro.Gap.Orm.Persoon Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.Persoon Ophalen(int id, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Persoon, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public List<Chiro.Gap.Orm.Persoon> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.Persoon Bewaren(Chiro.Gap.Orm.Persoon nieuweEntiteit)
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.Persoon Bewaren(Chiro.Gap.Orm.Persoon entiteit, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Persoon, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Chiro.Gap.Orm.Persoon> Bewaren(IList<Chiro.Gap.Orm.Persoon> es, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Persoon, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Persoon, object>>[] getConnectedEntities()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
