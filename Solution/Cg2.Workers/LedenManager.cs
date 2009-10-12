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
        private LedenDaoCollectie _daos;
        private IAutorisatieManager _authorisatieMgr;

        /// <summary>
        /// Maakt een nieuwe ledenmanager aan
        /// </summary>
        /// <param name="daos">Een hele reeks van IDao-objecten, nodig
        /// voor data access.</param>
        /// <param name="authorisatie">een IAuthorisatieManager, die
        /// de GAV-permissies van de huidige user controleert.</param>
        public LedenManager(LedenDaoCollectie daos, IAutorisatieManager autorisatie)
        {
            _daos = daos;
            _authorisatieMgr = autorisatie;
        }

        /// <summary>
        /// Deze functie met stichtende naam maakt een gelieerde persoon
        /// lid in een gegeven afdelingsjaar.  Er wordt niet gepersisteerd.
        /// </summary>
        /// <param name="gp">Lid te maken gelieerde persoon, met Groep
        /// eraan gekoppeld</param>
        /// <param name="aj">Gewenste afdelingsjaar, met daaraan gekoppeld 
        /// GroepsWerkJaar, en daaraan dan weer Groep</param>
        /// <returns></returns>
        public Kind KindMaken(GelieerdePersoon gp, AfdelingsJaar aj)
        {
            if (!_authorisatieMgr.IsGavGelieerdePersoon(gp.ID))
            {
                throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
            }

            if (!_authorisatieMgr.IsGavGroepsWerkJaar(aj.GroepsWerkJaar.ID))
            {
                throw new GeenGavException(Properties.Resources.GeenGavAfdeling);
            }

            if (aj.GroepsWerkJaar.Groep.ID != gp.Groep.ID)
            {
                throw new FoutieveGroepException("De persoon is niet gelieerd aan de groep van het afdelingsjaar.");
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
        /// <returns>Lidobject, niet gepersisteerd</returns>
        /// <remarks>Normaalgezien worden er alleen leden bijgemaakt in het 
        /// huidige werkjaar.
        /// </remarks>
        public Lid LidMaken(GelieerdePersoon gp, GroepsWerkJaar gwj)
        {
            if (!_authorisatieMgr.IsGavGelieerdePersoon(gp.ID))
            {
                throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
            }
            if (!_authorisatieMgr.IsGavGroepsWerkJaar(gwj.ID))
            {
                throw new GeenGavException(Properties.Resources.GeenGavGroepsWerkJaar);
            }

            // JOHAN: Ik zou precies de afdelingsjaren van de groep in het
            // gegeven GroepsWerkJaar meegeven via de parameters.  Op die manier
            // moet deze businessmethod geen Data Access gaan doen.

            // TODO: geslacht in rekening brengen bij automatisch keuze afdeling
            // TODO: controle of lid nog niet bestaat
            // TODO: berekening EindeInstapPeriode
            // TODO: controle of groepswerkjaar van zelfde groep als gelieerde persoon.
            
            Lid lid;

            // Details van GelieerdePersoon ophalen
            GelieerdePersonenManager gpm = Factory.Maak<GelieerdePersonenManager>();
            GelieerdePersoon gpMetDetails = gpm.DetailsOphalen(gp.ID);

            // Afdelingen ophalen
            GroepenManager gm = Factory.Maak<GroepenManager>();
            IList<AfdelingsJaar> jaren = gm.AfdelingsJarenOphalen(gwj);

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
        /// <returns>Nieuw lidobject, niet gepersisteerd</returns>
        public Lid LidMaken(GelieerdePersoon gp)
        {
            if (_authorisatieMgr.IsGavGelieerdePersoon(gp.ID))
            {
                GroepenManager gm = Factory.Maak<GroepenManager>();

                if (gp.Groep == null)
                {
                    _daos.GelieerdePersoonDao.GroepLaden(gp);
                }

                GroepsWerkJaar gwj = gm.RecentsteGroepsWerkJaarGet(gp.Groep.ID);
                return LidMaken(gp, gm.RecentsteGroepsWerkJaarGet(gp.Groep.ID));
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
            }
        }

        /// <summary>
        /// Verwijdert lid uit database
        /// </summary>
        /// <param name="id">LidID</param>
        /// <returns>true on successful</returns>
        public Boolean LidVerwijderen(int id)
        {
            if (_authorisatieMgr.IsGavLid(id))
            {
                // TODO: controleren huidige werkjaar
                // TODO: controleren instapperiode
                // TODO: fallback naar LidOpNonactiefZetten?
                // TODO: controleren of verwijderen effectief gelukt is
                // TODO: probleem oplossen met Leiding bewaren 

                Lid lid = _daos.LedenDao.OphalenMetDetails(id);
                lid.TeVerwijderen = true;

                Debug.Assert(lid is Kind || lid is Leiding, "Lid moet ofwel Kind ofwel Leiding zijn!");

                // voor een Kind is _dao.Bewaren(lid) voldoende
                if (lid is Kind)
                {
                    _daos.LedenDao.Bewaren(lid);
                }
                // voor Leiding moet er blijkbaar meer gebeuren
                // onderstaande code werkt niet
                else if (lid is Leiding)
                {
                    _daos.LedenDao.Bewaren(lid);
                }

                return true;
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavLid);
            }
        }

        /// <summary>
        /// Haalt een 'pagina' op met leden uit een bepaald GroepsWerkJaar
        /// </summary>
        /// <param name="groepsWerkJaarID">ID gevraagde GroepsWerkJaar</param>
        /// <returns></returns>
        public IList<Lid> PaginaOphalen(int groepsWerkJaarID)
        {
            if (_authorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
            {
                return _daos.LedenDao.PaginaOphalen(groepsWerkJaarID);
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
            if (_authorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
            {
                return _daos.LedenDao.PaginaOphalen(groepsWerkJaarID, afdelingsID);
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
            if (_authorisatieMgr.IsGavLid(lid.ID))
            {
                lid.NonActief = true;
                //TODO save
                // JOHAN: Bewaren is niet per se nodig in deze method.  Wel
                // goed documenteren of bewaard moet worden of niet.

                throw new NotImplementedException();
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavLid);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lid"></param>
        public void LidActiveren(Lid lid)
        {
            if (_authorisatieMgr.IsGavLid(lid.ID))
            {
                lid.NonActief = false;
                //TODO er moet betaald worden + save
                // JOHAN: Bewaren is niet per se nodig in deze method.  Wel
                // goed documenteren of bewaard moet worden of niet.
                throw new NotImplementedException();
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavLid);
            }
        }

        public void LidBewaren(Lid lid)
        {
            if (_authorisatieMgr.IsGavLid(lid.ID))
            {
                Debug.Assert(lid is Kind || lid is Leiding, "Lid moet ofwel Kind ofwel Leiding zijn!");
                if (lid is Kind)
                {
                    _daos.KindDao.Bewaren((Kind)lid);
                }
                else if (lid is Leiding)
                {
                    _daos.LeidingDao.Bewaren((Leiding)lid);
                }
                else
                {
                    // hier komen we in principe nooit (zie Assert)
                    _daos.LedenDao.Bewaren(lid);
                }
            }
            else
            {
                // hier komen we in principe nooit (zie Assert)
                throw new InvalidOperationException("Lid is kind noch leiding");
            }

        }
    }
}
