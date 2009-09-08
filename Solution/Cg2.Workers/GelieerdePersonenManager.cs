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


        /// <summary>
        /// Maak een GelieerdePersoon voor gegeven persoon en groep
        /// </summary>
        /// <param name="persoon">te lieren persoon</param>
        /// <param name="groep">groep waaraan te lieren</param>
        /// <param name="chiroLeeftijd">chiroleeftijd gelieerde persoon</param>
        /// <returns>een nieuwe GelieerdePersoon</returns>
        public GelieerdePersoon PersoonAanGroepKoppelen(Persoon persoon, Groep groep, int chiroLeeftijd)
        {
            AuthorisatieManager aum = Factory.Maak<AuthorisatieManager>();

            if (aum.IsGavGroep(groep.ID))
            {
                GelieerdePersoon resultaat = new GelieerdePersoon();

                resultaat.Persoon = persoon;
                resultaat.Groep = groep;
                resultaat.ChiroLeefTijd = chiroLeeftijd;

                persoon.GelieerdePersoon.Add(resultaat);
                groep.GelieerdePersoon.Add(resultaat);

                return resultaat;
            }
            else
            {
                throw new GeenGavException("Je bent geen GAV van deze groep.");
            }
        }

        /// <summary>
        /// Maakt GelieerdePersoon, gekoppelde Persoon, Adressen en Communicatie allemaal
        /// te verwijderen.
        /// </summary>
        /// <param name="gp">Te verwijderen gelieerde persoon</param>
        /// <remarks>Deze wijziging wordt nog niet gepersisteerd in de database! Hiervoor
        /// moet eerst 'Bewaren' aangeroepen worden!</remarks>
        public void VolledigVerwijderen(GelieerdePersoon gp)
        {
            AuthorisatieManager aum = Factory.Maak<AuthorisatieManager>();

            if (aum.IsGavGelieerdePersoon(gp.ID))
            {
                gp.TeVerwijderen = true;
                gp.Persoon.TeVerwijderen = true;

                foreach (PersoonsAdres pa in gp.Persoon.PersoonsAdres)
                {
                    pa.TeVerwijderen = true;
                }

                foreach (CommunicatieVorm cv in gp.Communicatie)
                {
                    cv.TeVerwijderen = true;
                }
            }
            else
            {
                throw new GeenGavException("Je bent geen GAV van de gevraagde groep.");
            }
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
