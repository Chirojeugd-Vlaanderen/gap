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
    public class DummyPersonenDao: DummyDao<Persoon>, IPersonenDao
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
    }
}
