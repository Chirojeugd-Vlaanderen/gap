using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using Cg2.Data.Ef;
using Cg2.Orm.DataInterfaces;
using Cg2.Ioc;
using Cg2.Fouten.Exceptions;

using System.Diagnostics;

namespace Cg2.Workers
{
    public class LedenManager
    {
        private ILedenDao _dao;
        private IKindDao _kindDao;
        private ILeidingDao _leidingDao;
        private IGroepenDao _groepenDao;
        private IGelieerdePersonenDao _gelPersDao;
        private IDao<AfdelingsJaar> _afdao;

        private ILedenDao Dao
        {
            get { return _dao; }
        }

        #region Constructors

        public LedenManager(ILedenDao dao, IKindDao kindDao, ILeidingDao leidingDao, IGroepenDao groepenDao, IGelieerdePersonenDao gelPersDao, IDao<AfdelingsJaar> afdao)
        {
            _dao = dao;
            _kindDao = kindDao;
            _leidingDao = leidingDao;
            _groepenDao = groepenDao;
            _gelPersDao = gelPersDao;
            _afdao = afdao;
        }

        #endregion

        /// <summary>
        /// Deze functie met stichtende naam maakt een gelieerde persoon
        /// lid in een gegeven afdelingsjaar
        /// </summary>
        /// <param name="gp">Lid te maken gelieerde persoon, met Groep
        /// eraan gekoppeld</param>
        /// <param name="aj">Gewenste afdelingsjaar, met daaraan gekoppeld 
        /// GroepsWerkJaar, en daaraan dan weer Groep</param>
        /// <returns></returns>
        public Kind KindMaken(GelieerdePersoon gp, AfdelingsJaar aj)
        {
            if (aj.GroepsWerkJaar.Groep.ID != gp.Groep.ID)
            {
                throw new FoutieveGroepException("De gelieerde persoon is geen lid van de groep van het afdelingsjaar.");
            }
            else
            {
                Kind k = new Kind();
                k.GelieerdePersoon = gp;
                k.AfdelingsJaar = aj;
                k.GroepsWerkJaar = aj.GroepsWerkJaar;

                gp.Lid.Add(k);
                aj.Kind.Add(k);
                aj.GroepsWerkJaar.Lid.Add(k);

                return k;
            }
        }

        /// <summary>
        /// 'Opwaarderen' van een gelieerd persoon tot een lid.
        /// </summary>
        /// <param name="gp">Gelieerde persoon</param>
        /// <param name="gwj">Groepswerkjaar</param>
        /// <returns>Lidobject</returns>
        /// <remarks>Normaalgezien worden er alleen leden bijgemaakt in het 
        /// huidige werkjaar.
        /// </remarks>
        public Lid LidMaken(GelieerdePersoon gp, GroepsWerkJaar gwj)
        {
            // JOHAN: Ik zou precies de afdelingsjaren van de groep in het
            // gegeven GroepsWerkJaar meegeven via de parameters.  Op die manier
            // moet deze businessmethod geen Data Access gaan doen.

            // TODO: geslacht in rekening brengen bij automatisch keuze afdeling
            // TODO: controle of lid nog niet bestaat
            // TODO: berekening EindeInstapPeriode
            
            Lid lid;

            // Details van GelieerdePersoon ophalen
            GelieerdePersonenManager gpm = Factory.Maak<GelieerdePersonenManager>();
            GelieerdePersoon gpMetDetails = gpm.DetailsOphalen(gp.ID);

            // Afdelingen ophalen
            GroepenManager gm = new GroepenManager(_groepenDao, _afdao);
            IList<AfdelingsJaar> jaren = gm.OphalenAfdelingsJaren(gp.Groep, gwj);

            // Geschikte afdeling zoeken
            // Als er geen geschikte afdeling is, dan null (wordt Leiding)
            AfdelingsJaar aj = null;
            if (jaren.Count > 0)
            {
                int geboorte = 0;
                if (gpMetDetails.Persoon.GeboorteDatum != null)
                {
                    geboorte = ((DateTime) gpMetDetails.Persoon.GeboorteDatum).Year - gp.ChiroLeefTijd;
                }
                foreach (AfdelingsJaar jaar in jaren)
                {
                    if (jaar.GeboorteJaarVan <= geboorte && geboorte <= jaar.GeboorteJaarTot)
                    {
                        aj = jaar;
                    }
                }
            }

            // Specifieke dingen voor Leiding of Kind
            if (aj == null)
            {
                Leiding leiding = new Leiding();
                foreach (AfdelingsJaar ajw in jaren)
                {
                    Random seed = new Random();
                    double r = seed.NextDouble();
                    if (r < 0.2)
                    {
                        leiding.AfdelingsJaar.Add(ajw);
                        ajw.Leiding.Add(leiding);
                    }
                }
                lid = leiding;
            }
            else
            {
                Kind kind = new Kind();
                kind.AfdelingsJaar = aj;
                aj.Kind.Add(kind);
                lid = kind;
            }
            
            // GroepsWerkJaar en GelieerdePersoon invullen
            lid.GroepsWerkJaar = gwj;
            lid.GelieerdePersoon = gp;
            gp.Lid.Add(lid);
            gwj.Lid.Add(lid);

            return lid;
        }

        /// <summary>
        /// Maakt gelieerde persoon lid voor recentste werkjaar
        /// </summary>
        /// <param name="gp">Gelieerde persoon</param>
        /// <returns>Nieuw lidobject</returns>
        public Lid LidMaken(GelieerdePersoon gp)
        {
            GroepenManager gm = new GroepenManager(_groepenDao, _afdao);

            if (gp.Groep == null)
            {
                GelieerdePersonenManager gpm = new GelieerdePersonenManager(_gelPersDao, _groepenDao);
                gpm.GroepLaden(gp);
            }

            GroepsWerkJaar gwj = gm.RecentsteGroepsWerkJaarGet(gp.Groep.ID);
            return LidMaken(gp, gm.RecentsteGroepsWerkJaarGet(gp.Groep.ID));
        }

        /// <summary>
        /// Verwijdert lid
        /// </summary>
        /// <param name="lid">Lid</param>
        /// <returns>true on successful</returns>
        public Boolean LidVerwijderen(int id)
        {
            // TODO: controleren huidige werkjaar
            // TODO: controleren instapperiode
            // TODO: fallback naar LidOpNonactiefZetten?
            // TODO: controleren of verwijderen effectief gelukt is
            // TODO: probleem oplossen met Leiding bewaren 

            Lid lid = _dao.OphalenMetDetails(id);
            lid.TeVerwijderen = true;

            Debug.Assert(lid is Kind || lid is Leiding, "Lid moet ofwel Kind ofwel Leiding zijn!");

            // voor een Kind is _dao.Bewaren(lid) voldoende
            if (lid is Kind)
            {
                _dao.Bewaren(lid);
            }
            // voor Leiding moet er blijkbaar meer gebeuren
            // onderstaande code werkt niet
            else if (lid is Leiding)
            {
                _dao.Bewaren(lid);
            }

            return true;
        }

        /// <summary>
        /// Haalt een 'pagina' op met leden uit een bepaald GroepsWerkJaar
        /// </summary>
        /// <param name="groepsWerkJaarID">ID gevraagde GroepsWerkJaar</param>
        /// <returns></returns>
        public IList<Lid> PaginaOphalen(int groepsWerkJaarID)
        {
            AuthorisatieManager am = Factory.Maak<AuthorisatieManager>();

            if (am.IsGavGroepsWerkJaar(groepsWerkJaarID))
            {
                return _dao.PaginaOphalen(groepsWerkJaarID);
            }
            else
            {
                throw new GeenGavException("Dit GroepsWerkJaar hoort niet bij je groep(en).");
            }
        }

        /// <summary>
        /// Haalt een 'pagina' op met leden uit een bepaald GroepsWerkJaar
        /// </summary>
        /// <param name="groepsWerkJaarID">ID gevraagde GroepsWerkJaar</param>
        /// <param name="afdelingsID">ID gevraagde afdeling</param>
        /// <returns></returns>
        public IList<Lid> PaginaOphalen(int groepsWerkJaarID, int afdelingsID)
        {
            AuthorisatieManager am = Factory.Maak<AuthorisatieManager>();

            if (am.IsGavGroepsWerkJaar(groepsWerkJaarID))
            {
                return _dao.PaginaOphalen(groepsWerkJaarID, afdelingsID);
            }
            else
            {
                throw new GeenGavException("Dit GroepsWerkJaar hoort niet bij je groep(en).");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lid"></param>
        public void LidOpNonactiefZetten(Lid lid)
        {
            lid.NonActief = true;
            //TODO save
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lid"></param>
        public void LidActiveren(Lid lid)
        {
            lid.NonActief = false;
            //TODO er moet betaald worden + save
            throw new NotImplementedException();
        }

        public void LidBewaren(Lid lid)
        {
            Debug.Assert(lid is Kind || lid is Leiding, "Lid moet ofwel Kind ofwel Leiding zijn!");
            if (lid is Kind)
            {
                _kindDao.Bewaren((Kind) lid);
            }
            else if (lid is Leiding)
            {
                _leidingDao.Bewaren((Leiding)lid);
            }
            else
            {
                // hier komen we in principe nooit (zie Assert)
                _dao.Bewaren(lid);
            }
        }
    }
}
