using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using Cg2.Data.Ef;
using Cg2.Orm.DataInterfaces;

namespace Cg2.Workers
{
    public class LedenManager
    {
        private ILedenDao _dao;
        private IGroepenDao _groepenDao;
        private IGelieerdePersonenDao _gelPersDao;

        public ILedenDao Dao
        {
            get { return _dao; }
        }

        #region Constructors

        public LedenManager(ILedenDao dao, IGroepenDao groepenDao, IGelieerdePersonenDao gelPersDao)
        {
            _dao = dao;
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
            // Kijken of er al een lid bestaat, moet nog gebeuren!

            Lid l = new Lid();

            l.GroepsWerkJaar = gwj;
            l.GelieerdePersoon = gp;

            // het op deze manier doen, haalde ook niets uit:
            // l.GroepsWerkJaarReference.EntityKey = gwj.EntityKey;
            // l.GelieerdePersoonReference.EntityKey = gp.EntityKey;

            gwj.Lid.Add(l);
            gp.Lid.Add(l);

            // TODO: Einde instapperiode moet ook nog berekend worden.
            // 
            //l.EindeInstapPeriode = DateTime.Now;

            return l;
        }

        /// <summary>
        /// Maakt gelieerde persoon lid voor huidig werkjaar
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

            return LidMaken(gp, gm.Dao.OphalenHuidigeGroepsWerkjaar(gp.Groep.ID));
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
            Dao.Bewaren(lid);
        }
    }
}
