using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;


namespace Chiro.Gap.Dummies
{
    public class DummyGroepenDao: IGroepenDao
    {
        #region IGroepenDao Members

        public GroepsWerkJaar RecentsteGroepsWerkJaarGet(int groepID)
        {
            throw new NotImplementedException();
        }

        public Groep OphalenMetAfdelingen(int groepID)
        {
            throw new NotImplementedException();
        }

        public IList<OfficieleAfdeling> OphalenOfficieleAfdelingen()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDao<Groep> Members

        public Groep Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Groep Ophalen(int id, params System.Linq.Expressions.Expression<Func<Groep, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Groep> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Groep Bewaren(Groep nieuweEntiteit)
        {
            throw new NotImplementedException();
        }

        public Groep Bewaren(Groep entiteit, params System.Linq.Expressions.Expression<Func<Groep, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Groep> Bewaren(IEnumerable<Groep> es, params System.Linq.Expressions.Expression<Func<Groep, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public System.Linq.Expressions.Expression<Func<Groep, object>>[] getConnectedEntities()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
