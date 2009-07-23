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

        // TODO: Dao mag niet geexposed worden!
        public ILedenDao Dao
        {
            get { return _dao; }
        }

        #region Constructors

        public LedenManager(ILedenDao dao, IKindDao kindDao, ILeidingDao leidingDao, IGroepenDao groepenDao, IGelieerdePersonenDao gelPersDao)
        {
            _dao = dao;
            _kindDao = kindDao;
            _leidingDao = leidingDao;
            _groepenDao = groepenDao;
            _gelPersDao = gelPersDao;
        }

        #endregion

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
            // TODO: functie wordt aangepast om Kind aan te maken 
            // bij de eerste afdeling van het GroepsWerkJaar
            // later uitbreiden om:
            // * te kiezen Kind/Leiding
            // * juiste afdeling (eventueel pagina tonen om te kiezen??)
            // * checken of het Lid nog niet bestaat (geen dubbels maken)

            Lid lid;

            AfdelingsJaar aj = null;

            // Afdelingen opzoeken
            // TODO: effectief controleren
            // Voorlopig altijd Kind
            GroepenManager gm = new GroepenManager(_groepenDao);

            IList<AfdelingsJaar> jaren = gm.OphalenAfdelingsJaren(gp.Groep, gwj);
            // voorlopig gewoon eerste afdeling uit de lijst nemen
            // als er geen afdelingen zijn, dan null ===> leiding
            if (jaren.Count > 0)
            {
                aj = jaren.ElementAt(0);
            }

            // specifieke dingen voor leiding of voor leden
            // keuze voor leiding als geen geschikte afdeling gevonden kon worden
            if (aj == null)
            {
                Leiding leiding = new Leiding();
                lid = leiding;
            }
            else
            {
                Kind kind = new Kind();
                kind.AfdelingsJaar.Kind.Add(kind);
                lid = kind;
            }
            
            // algemeen: GroepsWerkJaar en GelieerdePersoon invullen
            lid.GroepsWerkJaar = gwj;
            lid.GelieerdePersoon = gp;
            gp.Lid.Add(lid);
            gwj.Lid.Add(lid);

            // Kijken of er al een lid bestaat, moet nog gebeuren!

            // TODO: Einde instapperiode moet ook nog berekend worden.
            // 
            //l.EindeInstapPeriode = DateTime.Now;

            return lid;
        }

        /// <summary>
        /// Maakt gelieerde persoon lid voor recentste werkjaar
        /// </summary>
        /// <param name="gp">Gelieerde persoon</param>
        /// <returns>Nieuw lidobject</returns>
        public Lid LidMaken(GelieerdePersoon gp)
        {
            GroepenManager gm = new GroepenManager(_groepenDao);

            if (gp.Groep == null)
            {
                GelieerdePersonenManager gpm = new GelieerdePersonenManager(_gelPersDao, _groepenDao);
                gpm.GroepLaden(gp);
            }

            GroepsWerkJaar gwj = gm.Dao.RecentsteGroepsWerkJaarGet(gp.Groep.ID);
            return LidMaken(gp, gm.Dao.RecentsteGroepsWerkJaarGet(gp.Groep.ID));
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

        /*public void LidMaken(int gelieerdePersoonID)
        {
            Dao.LidMaken(gelieerdePersoonID);
        }*/

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
