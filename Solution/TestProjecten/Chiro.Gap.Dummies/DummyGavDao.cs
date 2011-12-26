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
    public class DummyGavDao: IDao<Gav>, IGavDao
    {
        public Gav Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Gav Ophalen(int id, params Expression<Func<Gav, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Gav> Ophalen(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public IList<Gav> Ophalen(IEnumerable<int> ids, params Expression<Func<Gav, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Gav> PaginaOphalen(int id, Expression<Func<Gav, int>> f, int pagina, int paginaGrootte, out int aantalTotaal, params Expression<Func<Gav, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Gav> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Gav Bewaren(Gav entiteit)
        {
            throw new NotImplementedException();
        }

        public Gav Bewaren(Gav entiteit, params Expression<Func<Gav, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Gav> Bewaren(IEnumerable<Gav> es, params Expression<Func<Gav, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public Expression<Func<Gav, object>>[] GetConnectedEntities()
        {
            throw new NotImplementedException();
        }

        public Gav Ophalen(string login)
        {
            throw new NotImplementedException();
        }

        public int IdOphalen(string login)
        {
            throw new NotImplementedException();
        }
    }
}
