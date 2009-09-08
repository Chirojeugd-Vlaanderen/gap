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

        public AuthorisatieManager(IAuthorisatieDao dao)
        {
            _dao = dao;
        }

        #endregion

        /// <summary>
        /// Bepaalt de gebruikersnaam van de huidig aangemelde gebruiker.
        /// (lege string indien niet van toepassing)
        /// </summary>
        /// <returns>Username aangemelde gebruiker</returns>
        private string GebruikersNaamGet()
        {
            return ServiceSecurityContext.Current == null ? ""
                : ServiceSecurityContext.Current.WindowsIdentity.Name;
        }

        /// <summary>
        /// IsGav geeft true als de aangelogde user
        /// gav is voor de groep met gegeven ID
        /// </summary>
        /// <param name="groepID">id van de groep</param>
        /// <returns>true (enkel) als user gav is</returns>
        public bool IsGavGroep(int groepID)
        {
            return _dao.IsGavGroep(GebruikersNaamGet(), groepID);
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
            return _dao.IsGavGelieerdePersoon(GebruikersNaamGet(), gelieerdePersoonID);
        }

        /// <summary>
        /// Geeft true als (en slechts als) de ingelogde user correspondeert
        /// met een GAV van een groep gelieerd aan de
        /// persoon met gegeven ID
        /// </summary>
        /// <param name="persoonID">ID van te checken Persoon</param>
        /// <returns>true indien de user de persoonsgegevens mag zien/bewerken</returns>
        public bool IsGavPersoon(int persoonID)
        {
            return _dao.IsGavPersoon(GebruikersNaamGet(), persoonID);
        }

        /// <summary>
        /// Geeft true alss de aangemelde user correspondeert
        /// met een GAV van de groep van een GroepsWerkJaar
        /// </summary>
        /// <param name="groepsWerkJaarID">ID gevraagde groepswerkjaar</param>
        /// <returns>true indien aangemelde gebruiker GAV is</returns>
        internal bool IsGavGroepsWerkJaar(int groepsWerkJaarID)
        {
            return _dao.IsGavGroepsWerkJaar(GebruikersNaamGet(), groepsWerkJaarID);
        }

        /// <summary>
        /// Ophalen van HUIDIGE gekoppelde groepen voor een aangemelde GAV
        /// </summary>
        /// <returns>ID's van gekoppelde groepen</returns>
        public IEnumerable<Groep> GekoppeldeGroepenGet()
        {
            return _dao.GekoppeldeGroepenGet(GebruikersNaamGet());
        }

        /// <summary>
        /// Verwijdert uit een lijst van GelieerdePersoonID's de ID's
        /// van GelieerdePersonen waarvoor de aangemelde gebruiker geen GAV is.
        /// </summary>
        /// <param name="gelieerdePersonenIDs">lijst met ID's van gelieerde personen</param>
        /// <returns>enkel de ID's van die personen waarvoor de gebruiker GAV is</returns>
        public IList<int> EnkelMijnGelieerdePersonen(IList<int> gelieerdePersonenIDs)
        {
            return _dao.EnkelMijnGelieerdePersonen(gelieerdePersonenIDs, GebruikersNaamGet());
        }

        /// <summary>
        /// Verwijdert uit een lijst van PersoonID's de ID's
        /// van Personen waarvoor de aangemelde gebruiker geen GAV is.
        /// </summary>
        /// <param name="personenIDs">lijst met ID's van personen</param>
        /// <returns>enkel de ID's van die personen waarvoor de gebruiker GAV is</returns>
        public IList<int> EnkelMijnPersonen(IList<int> personenIDs)
        {
            return _dao.EnkelMijnPersonen(personenIDs, GebruikersNaamGet());
        }

    }
}
