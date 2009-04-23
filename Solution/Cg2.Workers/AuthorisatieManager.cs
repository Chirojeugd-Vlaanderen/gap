using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm;
using Cg2.Orm.DataInterfaces;
using Cg2.Data.Ef;
using System.ServiceModel;

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
        /// IsGav geeft true als de aangelogde user
        /// gav is voor de groep met gegeven ID
        /// </summary>
        /// <param name="groepID">id van de groep</param>
        /// <returns>true (enkel) als user gav is</returns>
        public bool IsGavGroep(int groepID)
        {
            return _dao.IsGavGroep(ServiceSecurityContext.Current.WindowsIdentity.Name, groepID);
        }

        /// <summary>
        /// Geeft true als (en slechts als) de ingelogde user correspondeert
        /// met een GAV van een groep gelieerd aan de gelieerde
        /// persoon met gegeven ID
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van te checken gelieerde persoon</param>
        /// <returns>true indien de user de persoonsgegevens mag zien/bewerken</returns>
        public bool IsGavGelieerdePersoon(int gelieerdePersoonID)
        {
            return _dao.IsGavGelieerdePersoon(ServiceSecurityContext.Current.WindowsIdentity.Name, gelieerdePersoonID);
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

        /// <summary>
        /// Verwijdert uit een lijst van GelieerdePersoonID's de ID's
        /// van GelieerdePersonen waarvoor de gebruiker geen GAV is.
        /// </summary>
        /// <param name="gelieerdePersonenIDs">lijst met ID's van gelieerde personen</param>
        /// <param name="login">gebruikersnaam</param>
        /// <returns>enkel de ID's van die personen waarvoor de gebruiker GAV is</returns>
        public IList<int> EnkelMijnGelieerdePersonen(IList<int> gelieerdePersonenIDs, string login)
        {
            return _dao.EnkelMijnGelieerdePersonen(gelieerdePersonenIDs, login);
        }
    }
}
