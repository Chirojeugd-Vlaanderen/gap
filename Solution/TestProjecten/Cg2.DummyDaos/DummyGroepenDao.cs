using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm.DataInterfaces;

namespace Cg2.Dummies
{
    public class DummyGroepenDao: IGroepenDao
    {
        #region IGroepenDao Members

        public Chiro.Gap.Orm.GroepsWerkJaar RecentsteGroepsWerkJaarGet(int groepID)
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.Groep OphalenMetAfdelingen(int groepID)
        {
            throw new NotImplementedException();
        }

        public IList<Chiro.Gap.Orm.OfficieleAfdeling> OphalenOfficieleAfdelingen()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDao<Groep> Members

        public Chiro.Gap.Orm.Groep Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.Groep Ophalen(int id, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Groep, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public List<Chiro.Gap.Orm.Groep> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.Groep Bewaren(Chiro.Gap.Orm.Groep nieuweEntiteit)
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.Groep Bewaren(Chiro.Gap.Orm.Groep entiteit, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Groep, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Chiro.Gap.Orm.Groep> Bewaren(IList<Chiro.Gap.Orm.Groep> es, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Groep, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.Groep, object>>[] getConnectedEntities()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
