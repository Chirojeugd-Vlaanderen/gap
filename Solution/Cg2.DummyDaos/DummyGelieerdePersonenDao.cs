using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;

namespace Cg2.Dummies
{
    /// <summary>
    /// Dummy GelieerdePersonenDao, die niets implementeert
    /// </summary>
    class DummyGelieerdePersonenDao: IGelieerdePersonenDao
    {
        #region IGelieerdePersonenDao Members

        public IList<Cg2.Orm.GelieerdePersoon> LijstOphalen(IEnumerable<int> gelieerdePersonenIDs)
        {
            throw new NotImplementedException();
        }

        public IList<Cg2.Orm.GelieerdePersoon> ZoekenOpNaam(int groepID, string zoekStringNaam)
        {
            throw new NotImplementedException();
        }

        public IList<Cg2.Orm.GelieerdePersoon> AllenOphalen(int GroepID)
        {
            throw new NotImplementedException();
        }

        public IList<Cg2.Orm.GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
        {
            throw new NotImplementedException();
        }

        public IList<Cg2.Orm.GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.GelieerdePersoon DetailsOphalen(int gelieerdePersoonID)
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.GelieerdePersoon GroepLaden(Cg2.Orm.GelieerdePersoon p)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDao<GelieerdePersoon> Members

        public Cg2.Orm.GelieerdePersoon Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.GelieerdePersoon Ophalen(int id, params System.Linq.Expressions.Expression<Func<Cg2.Orm.GelieerdePersoon, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public List<Cg2.Orm.GelieerdePersoon> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.GelieerdePersoon Bewaren(Cg2.Orm.GelieerdePersoon nieuweEntiteit)
        {
            throw new NotImplementedException();
        }

        public Cg2.Orm.GelieerdePersoon Bewaren(Cg2.Orm.GelieerdePersoon entiteit, params System.Linq.Expressions.Expression<Func<Cg2.Orm.GelieerdePersoon, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Cg2.Orm.GelieerdePersoon> Bewaren(IList<Cg2.Orm.GelieerdePersoon> es, params System.Linq.Expressions.Expression<Func<Cg2.Orm.GelieerdePersoon, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public System.Linq.Expressions.Expression<Func<Cg2.Orm.GelieerdePersoon, object>>[] getConnectedEntities()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
