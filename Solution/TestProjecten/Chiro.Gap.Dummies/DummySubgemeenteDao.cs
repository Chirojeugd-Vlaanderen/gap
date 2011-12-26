using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
    public class DummySubgemeenteDao: IDao<WoonPlaats>, ISubgemeenteDao
    {
        public WoonPlaats Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public WoonPlaats Ophalen(int id, params Expression<Func<WoonPlaats, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<WoonPlaats> Ophalen(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public IList<WoonPlaats> Ophalen(IEnumerable<int> ids, params Expression<Func<WoonPlaats, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<WoonPlaats> PaginaOphalen(int id, Expression<Func<WoonPlaats, int>> f, int pagina, int paginaGrootte, out int aantalTotaal, params Expression<Func<WoonPlaats, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<WoonPlaats> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public WoonPlaats Bewaren(WoonPlaats entiteit)
        {
            throw new NotImplementedException();
        }

        public WoonPlaats Bewaren(WoonPlaats entiteit, params Expression<Func<WoonPlaats, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WoonPlaats> Bewaren(IEnumerable<WoonPlaats> es, params Expression<Func<WoonPlaats, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public Expression<Func<WoonPlaats, object>>[] GetConnectedEntities()
        {
            throw new NotImplementedException();
        }

        public WoonPlaats Ophalen(string naam, int postNr)
        {
            throw new NotImplementedException();
        }
    }
}
