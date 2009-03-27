using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;
using Cg2.Data.Ef;
using Cg2.Orm;

namespace Cg2.Workers
{
    public class GelieerdePersonenManager
    {
        private IGelieerdePersonenDao _dao;
        private IGroepenDao _groepenDao;

        public GelieerdePersonenManager()
        {
            _dao = new GelieerdePersonenDao();
        }

        public GelieerdePersonenManager(IGelieerdePersonenDao dao, IGroepenDao groepenDao)
        {
            _dao = dao;
            _groepenDao = groepenDao;
        }

        #region proxy naar data access

        public GelieerdePersoon Ophalen(int gelieerdePersoonID)
        {
            return _dao.Ophalen(gelieerdePersoonID);
        }

        public GelieerdePersoon DetailsOphalen(int gelieerdePersoonID)
        {
            return _dao.DetailsOphalen(gelieerdePersoonID);
        }

        public GelieerdePersoon Bewaren(GelieerdePersoon p)
        {
            return _dao.Bewaren(p);
        }

        public IList<GelieerdePersoon> AllenOphalen(int groepID)
        {
            return _dao.AllenOphalen(groepID);
        }

        public IList<GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald)
        {
            return _dao.PaginaOphalen(groepID, pagina, paginaGrootte, out aantalOpgehaald);
        }

        public IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald)
        {
            return _dao.PaginaOphalenMetLidInfo(groepID, pagina, paginaGrootte, out aantalOpgehaald);
        }

        public GelieerdePersoon GroepLaden(GelieerdePersoon gp)
        {
            return _dao.GroepLaden(gp);
        }

        #endregion

    }
}
