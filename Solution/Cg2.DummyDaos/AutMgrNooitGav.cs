using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Workers;
using Cg2.Orm;

namespace Cg2.Dummies
{
    /// <summary>
    /// Autorisatiemanager die er steeds van uitgaat dat
    /// de gebruiker geen rechten heeft.
    /// (nuttig voor authorisatietests..)
    /// </summary>
    public class AutMgrNooitGav: IAutorisatieManager
    {
        #region IAutorisatieManager Members

        public IList<int> EnkelMijnGelieerdePersonen(IList<int> gelieerdePersonenIDs)
        {
            return new List<int>();
        }

        public IList<int> EnkelMijnPersonen(IList<int> personenIDs)
        {
            return new List<int>();
        }

        public IEnumerable<Groep> GekoppeldeGroepenGet()
        {
            return new List<Groep>();
        }

        public bool IsGavGelieerdePersoon(int gelieerdePersoonID)
        {
            return false;
        }

        public bool IsGavGroep(int groepID)
        {
            return false;
        }

        public bool IsGavGroepsWerkJaar(int groepsWerkJaarID)
        {
            return false;
        }

        public bool IsGavPersoon(int persoonID)
        {
            return false;
        }

        public string GebruikersNaamGet()
        {
            return "Paria";
        }

        public bool IsGavAfdeling(int afdelingsID)
        {
            return false;
        }

        public bool IsGavLid(int lidID)
        {
            return false;
        }

        public bool IsGavCategorie(int categorieID)
        {
            return false;
        }

        #endregion
    }
}
