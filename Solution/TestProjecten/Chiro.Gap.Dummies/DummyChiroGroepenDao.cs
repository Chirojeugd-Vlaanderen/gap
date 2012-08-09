using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
    public class DummyChiroGroepenDao: IChiroGroepenDao
    {
        public ChiroGroep Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public ChiroGroep Ophalen(int id, params Expression<Func<ChiroGroep, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<ChiroGroep> Ophalen(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public IList<ChiroGroep> Ophalen(IEnumerable<int> ids, params Expression<Func<ChiroGroep, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<ChiroGroep> PaginaOphalen(int id, Expression<Func<ChiroGroep, int>> f, int pagina, int paginaGrootte, out int aantalTotaal, params Expression<Func<ChiroGroep, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<ChiroGroep> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public ChiroGroep Bewaren(ChiroGroep entiteit)
        {
            throw new NotImplementedException();
        }

        public ChiroGroep Bewaren(ChiroGroep entiteit, params Expression<Func<ChiroGroep, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ChiroGroep> Bewaren(IEnumerable<ChiroGroep> es, params Expression<Func<ChiroGroep, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public Expression<Func<ChiroGroep, object>>[] GetConnectedEntities()
        {
            throw new NotImplementedException();
        }

        public ChiroGroep Bewaren(ChiroGroep chiroGroep, ChiroGroepsExtras extras)
        {
            throw new NotImplementedException();
        }

        public ChiroGroep Ophalen(int groepID, ChiroGroepsExtras extras)
        {
            throw new NotImplementedException();
        }
    }
}
