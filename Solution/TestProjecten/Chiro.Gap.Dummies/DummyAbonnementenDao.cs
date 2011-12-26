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
    public class DummyAbonnementenDao: IDao<Abonnement>, IAbonnementenDao
    {
        public Abonnement Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Abonnement Ophalen(int id, params Expression<Func<Abonnement, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Abonnement> Ophalen(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public IList<Abonnement> Ophalen(IEnumerable<int> ids, params Expression<Func<Abonnement, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Abonnement> PaginaOphalen(int id, Expression<Func<Abonnement, int>> f, int pagina, int paginaGrootte, out int aantalTotaal, params Expression<Func<Abonnement, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Abonnement> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Abonnement Bewaren(Abonnement entiteit)
        {
            throw new NotImplementedException();
        }

        public Abonnement Bewaren(Abonnement entiteit, params Expression<Func<Abonnement, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Abonnement> Bewaren(IEnumerable<Abonnement> es, params Expression<Func<Abonnement, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public Expression<Func<Abonnement, object>>[] GetConnectedEntities()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Abonnement> OphalenUitGroepsWerkJaar(int gwjID)
        {
            throw new NotImplementedException();
        }
    }
}
