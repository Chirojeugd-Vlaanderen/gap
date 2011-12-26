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
    public class DummyLandenDao: IDao<Land>, ILandenDao
    {
        public Land Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Land Ophalen(int id, params Expression<Func<Land, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Land> Ophalen(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public IList<Land> Ophalen(IEnumerable<int> ids, params Expression<Func<Land, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Land> PaginaOphalen(int id, Expression<Func<Land, int>> f, int pagina, int paginaGrootte, out int aantalTotaal, params Expression<Func<Land, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Land> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Land Bewaren(Land entiteit)
        {
            throw new NotImplementedException();
        }

        public Land Bewaren(Land entiteit, params Expression<Func<Land, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Land> Bewaren(IEnumerable<Land> es, params Expression<Func<Land, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public Expression<Func<Land, object>>[] GetConnectedEntities()
        {
            throw new NotImplementedException();
        }

        public Land Ophalen(string landNaam)
        {
            throw new NotImplementedException();
        }
    }
}
