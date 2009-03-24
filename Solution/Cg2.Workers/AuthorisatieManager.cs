using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using Cg2.Orm.DataInterfaces;
using Cg2.Data.Ef;

namespace Cg2.Workers
{
    public class AuthorisatieManager
    {
        private IAuthorisatieDao _dao;

        #region Constructors

        public AuthorisatieManager()
        {
            _dao = new AuthorisatieDao();
        }

        public AuthorisatieManager(IAuthorisatieDao dao)
        {
            _dao = dao;
        }

        #endregion

        /// <summary>
        /// IsGav geeft true als de user met gegeven login
        /// gav is voor de groep met gegeven ID
        /// </summary>
        /// <param name="login">username</param>
        /// <param name="groepID">id van de groep</param>
        /// <returns>true (enkel) als user gav is</returns>
        public bool IsGavGroep(string login, int groepID)
        {
            GebruikersRecht r = _dao.RechtenMbtGroepGet(login, groepID);

            return r != null && (r.VervalDatum == null || r.VervalDatum > DateTime.Now);
        }

        /// <summary>
        /// Geeft true als (en slechts als) de login overeenkomt
        /// met een GAV van een groep gelieerd aan de gelieerde
        /// persoon met gegeven ID
        /// </summary>
        /// <param name="login">username</param>
        /// <param name="gelieerdePersoonID">ID van te checken gelieerde persoon</param>
        /// <returns>true indien login de persoonsgegevens mag zien/bewerken</returns>
        public bool IsGavGelieerdePersoon(string login, int gelieerdePersoonID)
        {
            GebruikersRecht r = _dao.RechtenMbtGelieerdePersoonGet(login, gelieerdePersoonID);

            return r != null && (r.VervalDatum == null || r.VervalDatum > DateTime.Now);
        }

        /// <summary>
        /// Ophalen van HUIDIGE gekoppelde groepen voor een GAV
        /// </summary>
        /// <param name="login">login van de betreffende GAV</param>
        /// <returns>ID's van gekoppelde groepen</returns>
        public IList<int> GekoppeldeGroepenGet(string login)
        {
            return _dao.GekoppeldeGroepenGet(login);
        }
    }
}
