using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;

namespace Cg2.DummyDaos
{
    /// <summary>
    /// Deze klasse kan gebruikt worden als IAuthorisatieDao om te testen.
    /// Ze geeft altijd true op IsGav-vragen.
    /// </summary>
    public class AutDaoAltijdGav: IAutorisatieDao
    {
        #region IAuthorisatieDao Members

        public Cg2.Orm.GebruikersRecht RechtenMbtGroepGet(string login, int groepID)
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.GebruikersRecht RechtenMbtGelieerdePersoonGet(string login, int GelieerdePersoonID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavGroep(string login, int groepID)
        {
            return true;
        }

        public bool IsGavGelieerdePersoon(string login, int gelieerdePersoonID)
        {
            return true;
        }

        public bool IsGavPersoon(string login, int persoonID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavGroepsWerkJaar(string login, int groepsWerkJaarID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Cg2.Orm.Groep> GekoppeldeGroepenGet(string login)
        {
            throw new NotImplementedException();
        }

        public IList<int> EnkelMijnGelieerdePersonen(IList<int> gelieerdePersonenIDs, string login)
        {
            throw new NotImplementedException();
        }

        public IList<int> EnkelMijnPersonen(IList<int> personenIDs, string p)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDao<GebruikersRecht> Members

        public Cg2.Orm.GebruikersRecht Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.GebruikersRecht Ophalen(int id, params System.Linq.Expressions.Expression<Func<Cg2.Orm.GebruikersRecht, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public List<Cg2.Orm.GebruikersRecht> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.GebruikersRecht Bewaren(Cg2.Orm.GebruikersRecht nieuweEntiteit)
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.GebruikersRecht Bewaren(Cg2.Orm.GebruikersRecht entiteit, params System.Linq.Expressions.Expression<Func<Cg2.Orm.GebruikersRecht, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Cg2.Orm.GebruikersRecht> Bewaren(IList<Cg2.Orm.GebruikersRecht> es, params System.Linq.Expressions.Expression<Func<Cg2.Orm.GebruikersRecht, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public System.Linq.Expressions.Expression<Func<Cg2.Orm.GebruikersRecht, object>>[] getConnectedEntities()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
