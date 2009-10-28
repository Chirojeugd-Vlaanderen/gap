using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm.DataInterfaces;

namespace Cg2.Dummies
{
    /// <summary>
    /// Deze klasse kan gebruikt worden als IAuthorisatieDao om te testen.
    /// Ze geeft altijd true op IsGav-vragen.
    /// </summary>
    public class AutDaoAltijdGav: IAutorisatieDao
    {
        #region IAuthorisatieDao Members

        public Chiro.Gap.Orm.GebruikersRecht RechtenMbtGroepGet(string login, int groepID)
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.GebruikersRecht RechtenMbtGelieerdePersoonGet(string login, int GelieerdePersoonID)
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
            return true;
        }

        public IEnumerable<Chiro.Gap.Orm.Groep> GekoppeldeGroepenGet(string login)
        {
            throw new NotImplementedException();
        }

        public IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs, string login)
        {
            throw new NotImplementedException();
        }

        public IList<int> EnkelMijnPersonen(IEnumerable<int> personenIDs, string p)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDao<GebruikersRecht> Members

        public Chiro.Gap.Orm.GebruikersRecht Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.GebruikersRecht Ophalen(int id, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.GebruikersRecht, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public List<Chiro.Gap.Orm.GebruikersRecht> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.GebruikersRecht Bewaren(Chiro.Gap.Orm.GebruikersRecht nieuweEntiteit)
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.GebruikersRecht Bewaren(Chiro.Gap.Orm.GebruikersRecht entiteit, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.GebruikersRecht, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Chiro.Gap.Orm.GebruikersRecht> Bewaren(IList<Chiro.Gap.Orm.GebruikersRecht> es, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.GebruikersRecht, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.GebruikersRecht, object>>[] getConnectedEntities()
        {
            throw new NotImplementedException();
        }

        public bool IsGavAfdeling(string login, int afdelingsID)
        {
            return true;
        }

        public bool IsGavLid(string login, int lidID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavCategorie(int categorieID, string login)
        {
            throw new NotImplementedException();
        }

        public bool IsGavCommVorm(int commvormID, string login)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
