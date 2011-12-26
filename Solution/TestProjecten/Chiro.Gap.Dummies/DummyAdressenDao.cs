using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
    public class DummyAdressenDao: IDao<Adres>, IAdressenDao
    {
        public Adres Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Adres Ophalen(int id, params Expression<Func<Adres, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Adres> Ophalen(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public IList<Adres> Ophalen(IEnumerable<int> ids, params Expression<Func<Adres, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Adres> PaginaOphalen(int id, Expression<Func<Adres, int>> f, int pagina, int paginaGrootte, out int aantalTotaal, params Expression<Func<Adres, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Adres> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Adres Bewaren(Adres entiteit)
        {
            throw new NotImplementedException();
        }

        public Adres Bewaren(Adres entiteit, params Expression<Func<Adres, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Adres> Bewaren(IEnumerable<Adres> es, params Expression<Func<Adres, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public Expression<Func<Adres, object>>[] GetConnectedEntities()
        {
            throw new NotImplementedException();
        }

        public Adres Ophalen(AdresInfo adresInfo, bool metBewoners)
        {
            throw new NotImplementedException();
        }

        public Adres BewonersOphalen(int adresID, IEnumerable<int> groepIDs, bool metAlleGelieerdePersonen)
        {
            throw new NotImplementedException();
        }
    }
}
