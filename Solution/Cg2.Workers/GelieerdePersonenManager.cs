using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;
using Cg2.Data.Ef;
using Cg2.Orm;
using Cg2.Fouten.Exceptions;
using Cg2.Ioc;

namespace Cg2.Workers
{
    public class GelieerdePersonenManager
    {
        private IGelieerdePersonenDao _dao;
        private IGroepenDao _groepenDao;

        public GelieerdePersonenManager(IGelieerdePersonenDao dao, IGroepenDao groepenDao)
        {
            _dao = dao;
            _groepenDao = groepenDao;
        }

        #region proxy naar data access

        // Wel altijd rekening houden met authorisatie!

        public GelieerdePersoon Ophalen(int gelieerdePersoonID)
        {
            return _dao.Ophalen(gelieerdePersoonID);
        }

        public IList<GelieerdePersoon> LijstOphalen(List<int> gelieerdePersonenIDs)
        {
            AuthorisatieManager authMgr = Factory.Maak<AuthorisatieManager>();

            return _dao.LijstOphalen(authMgr.EnkelMijnGelieerdePersonen(gelieerdePersonenIDs));
        }

        public GelieerdePersoon DetailsOphalen(int gelieerdePersoonID)
        {
            AuthorisatieManager am = Factory.Maak<AuthorisatieManager>();

            if (am.IsGavGelieerdePersoon(gelieerdePersoonID))
            {
                return _dao.DetailsOphalen(gelieerdePersoonID);
            }
            else
            {
                throw new GeenGavException("Deze persoon is niet gelieerd aan je groep(en).");
            }
        }

        public GelieerdePersoon Bewaren(GelieerdePersoon p)
        {
            if (p.Persoon != null)
            {
                PersonenDao pd = new PersonenDao();
                pd.Bewaren(p.Persoon);
            }
            return _dao.Bewaren(p);
        }

        public IList<GelieerdePersoon> AllenOphalen(int groepID)
        {
            return _dao.AllenOphalen(groepID);
        }

        public IList<GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
        {
            return _dao.PaginaOphalen(groepID, pagina, paginaGrootte, out aantalTotaal);
        }

        public IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
        {
            AuthorisatieManager am = Factory.Maak<AuthorisatieManager>();

            if (am.IsGavGroep(groepID))
            {
                return _dao.PaginaOphalenMetLidInfo(groepID, pagina, paginaGrootte, out aantalTotaal);
            }
            else
            {
                throw new GeenGavException("Je bent geen GAV van deze groep.");
            }
        }

        public GelieerdePersoon GroepLaden(GelieerdePersoon gp)
        {
            return _dao.GroepLaden(gp);
        }

        /// <summary>
        /// Haalt personen op die een adres gemeen hebben met de 
        /// GelieerdePersoon
        /// met gegeven ID
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van 
        /// GelieerdePersoon waarvan huisgenoten
        /// gevraagd zijn</param>
        /// <returns>Lijstje met personen</returns>
        /// <remarks>Parameter: GelieerdePersoonID, return value: Personen!</remarks>
        public IList<Persoon> HuisGenotenOphalen(int gelieerdePersoonID)
        {
            AuthorisatieManager am = Factory.Maak<AuthorisatieManager>();

            if (am.IsGavGelieerdePersoon(gelieerdePersoonID))
            {
                return _dao.HuisGenotenOphalen(gelieerdePersoonID);
            }
            else
            {
                throw new GeenGavException("Je bent geen GAV voor deze persoon.");
            }
        }

        #endregion

        public void Bewaren(IList<GelieerdePersoon> personenLijst)
        {
            throw new NotImplementedException();
        }

        
        public bool IsLid(int gelieerdePersoonID)
        {
            throw new NotImplementedException();
        }
    }
}
