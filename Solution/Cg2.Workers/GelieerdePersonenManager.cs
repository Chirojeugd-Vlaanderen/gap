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
        private IAutorisatieManager _autorisatieMgr;

        public GelieerdePersonenManager(IGelieerdePersonenDao dao, IGroepenDao groepenDao, IAutorisatieManager autorisatieMgr)
        {
            _dao = dao;
            _groepenDao = groepenDao;
            _autorisatieMgr = autorisatieMgr;
        }

        #region proxy naar data access

        // Wel altijd rekening houden met authorisatie!

        /// <summary>
        /// Haalt enkel de gelieerde persoon op.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van de gelieerde persoon.</param>
        /// <returns>Gelieerde persoon, zonder enige koppelingen.</returns>
        public GelieerdePersoon Ophalen(int gelieerdePersoonID)
        {
            if (_autorisatieMgr.IsGavGelieerdePersoon(gelieerdePersoonID))
            {
                return _dao.Ophalen(gelieerdePersoonID);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
            }
        }

        /// <summary>
        /// Haalt een lijst op van gelieerde personen.
        /// </summary>
        /// <param name="gelieerdePersonenIDs">ID's van de op te vragen
        /// gelieerde personen.</param>
        /// <returns>Lijst met gelieerde personen</returns>
        public IList<GelieerdePersoon> LijstOphalen(List<int> gelieerdePersonenIDs)
        {
            return _dao.LijstOphalen(_autorisatieMgr.EnkelMijnGelieerdePersonen(gelieerdePersonenIDs));
        }

        /// <summary>
        /// Haalt gelieerde persoon op met persoonsgegevens, adressen en
        /// communicatievormen.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID gevraagde gelieerde persoon</param>
        /// <returns>GelieerdePersoon met persoonsgegevens, adressen en 
        /// communicatievormen.</returns>
        public GelieerdePersoon DetailsOphalen(int gelieerdePersoonID)
        {
            if (_autorisatieMgr.IsGavGelieerdePersoon(gelieerdePersoonID))
            {
                return _dao.DetailsOphalen(gelieerdePersoonID);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
            }
        }

        /// <summary>
        /// Bewaart een gelieerde persoon, zonder koppelingen
        /// </summary>
        /// <param name="p">Te bewaren gelieerde persoon</param>
        /// <returns>De bewaarde gelieerde persoon</returns>
        public GelieerdePersoon Bewaren(GelieerdePersoon p)
        {
            if (_autorisatieMgr.IsGavGelieerdePersoon(p.ID))
            {
                // Hier mapping gebruiken om te vermijden dat het AD-nummer
                // overschreven wordt, lijkt me wat overkill.  Ik vergelijk
                // hiet nieuwe AD-nummer gewoon met het bestaande.

                GelieerdePersoon origineel = _dao.Ophalen(p.ID);
                if (origineel.Persoon.AdNummer == p.Persoon.AdNummer)
                {
                    return _dao.Bewaren(p);
                }
                else
                {
                    throw new InvalidOperationException(Properties.Resources.AdNummerNietWijzigen);
                }
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
            }
        }

        /// <summary>
        /// Haal een lijst op met alle gelieerde personen van een groep.
        /// </summary>
        /// <param name="groepID">GroepID van gevraagde groep</param>
        /// <returns>Lijst met alle gelieerde personen</returns>
        public IList<GelieerdePersoon> AllenOphalen(int groepID)
        {
            if (_autorisatieMgr.IsGavGroep(groepID))
            {
                return _dao.AllenOphalen(groepID);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavGroep);
            }
        }

        /// <summary>
        /// Haal een pagina op met gelieerde personen van een groep.
        /// </summary>
        /// <param name="groepID">GroepID gevraagde groep</param>
        /// <param name="pagina">paginanummer (>=1)</param>
        /// <param name="paginaGrootte">grootte van de pagina's</param>
        /// <param name="aantalTotaal">totaal aantal personen in de groep</param>
        /// <returns>Lijst met een pagina aan gelieerde personen.</returns>
        public IList<GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
        {
            if (_autorisatieMgr.IsGavGroep(groepID))
            {
                return _dao.PaginaOphalen(groepID, pagina, paginaGrootte, out aantalTotaal);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavGroep);
            }
        }

        /// <summary>
        /// Haalt een pagina op met gelieerde personen van een groep,
        /// inclusief eventuele lidobjecten voor deze groep
        /// </summary>
        /// <param name="groepID">GroepID gevraagde groep</param>
        /// <param name="pagina">paginanummer (>=1)</param>
        /// <param name="paginaGrootte">aantal personen per pagina</param>
        /// <param name="aantalTotaal">output parameter voor totaal aantal
        /// personen in de groep</param>
        /// <returns>Lijst met GelieerdePersonen</returns>
        public IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
        {
            if (_autorisatieMgr.IsGavGroep(groepID))
            {
                return _dao.PaginaOphalenMetLidInfo(groepID, pagina, paginaGrootte, out aantalTotaal);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavGroep);
            }
        }

        /// <summary>
        /// Koppelt het relevante groepsobject aan de gegeven
        /// gelieerde persoon.
        /// </summary>
        /// <param name="gp">gelieerde persoon</param>
        /// <returns>diezelfde gelieerde persoon, met zijn groep eraan
        /// gekoppeld.</returns>
        public GelieerdePersoon GroepLaden(GelieerdePersoon gp)
        {
            if (_autorisatieMgr.IsGavGelieerdePersoon(gp.ID))
            {
                return _dao.GroepLaden(gp);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
            }
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
            if (_autorisatieMgr.IsGavGelieerdePersoon(gelieerdePersoonID))
            {
                // Haal alle huisgenoten op

                IList<Persoon> allen = _dao.HuisGenotenOphalen(gelieerdePersoonID);

                // Welke mag ik zien?

                IList<int> selectie = _autorisatieMgr.EnkelMijnPersonen(
                    (from p in allen select p.ID).ToList());

                // Enkel de geselecteerde personen afleveren.

                var resultaat = from p in allen
                                where selectie.Contains(p.ID)
                                select p;

                return resultaat.ToList();
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
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
            if (_autorisatieMgr.IsGavGroep(groep.ID))
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
                throw new GeenGavException(Properties.Resources.GeenGavGroep);
            }
        }

        /// <summary>
        /// Maakt GelieerdePersoon, gekoppelde Persoon, Adressen en Communicatie allemaal
        /// te verwijderen.  Persisteert niet!
        /// </summary>
        /// <param name="gp">Te verwijderen gelieerde persoon</param>
        /// <remarks>Deze wijziging wordt nog niet gepersisteerd in de database! Hiervoor
        /// moet eerst 'Bewaren' aangeroepen worden!</remarks>
        public void VolledigVerwijderen(GelieerdePersoon gp)
        {
            if (_autorisatieMgr.IsGavGelieerdePersoon(gp.ID))
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
                throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
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
