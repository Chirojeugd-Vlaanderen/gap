using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm;

namespace Cg2.Dummies
{
    /// <summary>
    /// Dummy GelieerdePersonenDao, die niets implementeert
    /// </summary>
    class DummyGelieerdePersonenDao: IGelieerdePersonenDao
    {
        #region IGelieerdePersonenDao Members

        public IList<Chiro.Gap.Orm.GelieerdePersoon> LijstOphalen(IEnumerable<int> gelieerdePersonenIDs)
        {
            throw new NotImplementedException();
        }

        public IList<Chiro.Gap.Orm.GelieerdePersoon> ZoekenOpNaam(int groepID, string zoekStringNaam)
        {
            throw new NotImplementedException();
        }

        public IList<Chiro.Gap.Orm.GelieerdePersoon> AllenOphalen(int GroepID)
        {
            throw new NotImplementedException();
        }

        public IList<Chiro.Gap.Orm.GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
        {
            throw new NotImplementedException();
        }

        public IList<Chiro.Gap.Orm.GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.GelieerdePersoon DetailsOphalen(int gelieerdePersoonID)
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.GelieerdePersoon GroepLaden(Chiro.Gap.Orm.GelieerdePersoon p)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CommunicatieType> ophalenCommunicatieTypes()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDao<GelieerdePersoon> Members

        public Chiro.Gap.Orm.GelieerdePersoon Ophalen(int id)
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.GelieerdePersoon Ophalen(int id, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.GelieerdePersoon, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public List<Chiro.Gap.Orm.GelieerdePersoon> AllesOphalen()
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.GelieerdePersoon Bewaren(Chiro.Gap.Orm.GelieerdePersoon nieuweEntiteit)
        {
            throw new NotImplementedException();
        }

        public Chiro.Gap.Orm.GelieerdePersoon Bewaren(Chiro.Gap.Orm.GelieerdePersoon entiteit, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.GelieerdePersoon, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public IList<Chiro.Gap.Orm.GelieerdePersoon> Bewaren(IList<Chiro.Gap.Orm.GelieerdePersoon> es, params System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.GelieerdePersoon, object>>[] paths)
        {
            throw new NotImplementedException();
        }

        public System.Linq.Expressions.Expression<Func<Chiro.Gap.Orm.GelieerdePersoon, object>>[] getConnectedEntities()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
