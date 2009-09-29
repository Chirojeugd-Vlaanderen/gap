using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Workers;

namespace Cg2.Dummies
{
    /// <summary>
    /// Autorisatiemanager die altijd alle rechten toekent
    /// (nuttig voor testen van niet-autorisatiegebonden 
    /// business logica.)
    /// </summary>
    public class AutMgrAltijdGav: IAutorisatieManager
    {
        #region IAutorisatieManager Members

        public IList<int> EnkelMijnGelieerdePersonen(IList<int> gelieerdePersonenIDs)
        {
            throw new NotImplementedException();
        }

        public IList<int> EnkelMijnPersonen(IList<int> personenIDs)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Cg2.Orm.Groep> GekoppeldeGroepenGet()
        {
            throw new NotImplementedException();
        }

        public bool IsGavGelieerdePersoon(int gelieerdePersoonID)
        {
            return true;
        }

        public bool IsGavGroep(int groepID)
        {
            return true;
        }

        public bool IsGavGroepsWerkJaar(int groepsWerkJaarID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavPersoon(int persoonID)
        {
            throw new NotImplementedException();
        }

        public string GebruikersNaamGet()
        {
            throw new NotImplementedException();
        }

        public bool IsGavAfdeling(int afdelingsID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavLid(int lidID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavCategorie(int categorieID)
        {
            return true;
        }

        #endregion
    }
}
