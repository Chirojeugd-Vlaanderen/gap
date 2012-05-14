using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Chiro.Cdf.Data;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
    public class DummyStratenDao: IDao<StraatNaam>, IStratenDao
    {
        public StraatNaam Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public StraatNaam Ophalen(int id, params Expression<Func<StraatNaam, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<StraatNaam> Ophalen(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public IList<StraatNaam> Ophalen(IEnumerable<int> ids, params Expression<Func<StraatNaam, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<StraatNaam> PaginaOphalen(int id, Expression<Func<StraatNaam, int>> f, int pagina, int paginaGrootte, out int aantalTotaal, params Expression<Func<StraatNaam, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<StraatNaam> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public StraatNaam Bewaren(StraatNaam entiteit)
        {
            throw new NotImplementedException();
        }

        public StraatNaam Bewaren(StraatNaam entiteit, params Expression<Func<StraatNaam, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<StraatNaam> Bewaren(IEnumerable<StraatNaam> es, params Expression<Func<StraatNaam, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public Expression<Func<StraatNaam, object>>[] GetConnectedEntities()
        {
            throw new NotImplementedException();
        }

        public StraatNaam Ophalen(string naam, int postNr)
        {
            throw new NotImplementedException();
        }

        public IList<StraatNaam> MogelijkhedenOphalen(string naamBegin, int postNr)
        {
            throw new NotImplementedException();
        }

        public IList<StraatNaam> MogelijkhedenOphalen(string naamBegin, IEnumerable<int> postNrs)
        {
            throw new NotImplementedException();
        }
    }
}
