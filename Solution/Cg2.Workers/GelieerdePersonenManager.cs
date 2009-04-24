using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;
using Cg2.Data.Ef;
using Cg2.Orm;
using Cg2.Fouten.Exceptions;

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
            AuthorisatieManager am = new AuthorisatieManager();

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
            AuthorisatieManager am = new AuthorisatieManager();

            if (am.IsGavGroep(groepID))
            {
                return _dao.PaginaOphalenMetLidInfo(groepID, pagina, paginaGrootte, out aantalOpgehaald);
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

        #endregion

        /// <summary>
        /// Verhuist een persoon van oudAdres naar nieuwAdres
        /// </summary>
        /// <param name="verhuizer">te verhuizen GelieerdePersoon</param>
        /// <param name="oudAdres">oud adres</param>
        /// <param name="nieuwAdres">nieuw adres</param>
        /// <remarks>Als de persoon niet gekoppeld is aan het oude adres,
        /// zal hij ook niet verhiuzen</remarks>
        public void Verhuizen(GelieerdePersoon verhuizer, Adres oudAdres, Adres nieuwAdres)
        {
            PersoonsAdres persoonsadres
                = (from PersoonsAdres pa in verhuizer.PersoonsAdres
                  where pa.Adres.ID == oudAdres.ID
                  select pa).FirstOrDefault();

            if (oudAdres.PersoonsAdres != null)
            {
                oudAdres.PersoonsAdres.Remove(persoonsadres);
            }
            persoonsadres.Adres = nieuwAdres;

            if (nieuwAdres.PersoonsAdres != null)
            {
                nieuwAdres.PersoonsAdres.Add(persoonsadres);
            }
        }
    }
}
