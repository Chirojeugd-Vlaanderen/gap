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

        public IList<GelieerdePersoon> HuisGenotenOphalen(int gelieerdePersoonID)
        {
            AuthorisatieManager am = Factory.Maak<AuthorisatieManager>();

            if (am.IsGavGelieerdePersoon(gelieerdePersoonID))
            {
                return _dao.HuisGenotenOphalen(gelieerdePersoonID);
            }
            else
            {
                throw new GeenGavException("Je kan geen adres toevoegen aan een persoon die niet gelieerd is aan je groep(en).");
            }
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

        /// <summary>
        /// Koppelt het gegeven Adres via een nieuw PersoonsAdresObject
        /// aan de gegeven GelieerdePersoon
        /// </summary>
        /// <param name="p">GelieerdePersoon die er een adres bij krijgt</param>
        /// <param name="adres">Toe te voegen adres</param>
        public void AdresToevoegen(GelieerdePersoon p, Adres adres)
        {
            PersoonsAdres pa = new PersoonsAdres { Adres = adres, GelieerdePersoon = p, AdresTypeID = 1 };
            p.PersoonsAdres.Add(pa);
            adres.PersoonsAdres.Add(pa);
        }

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
