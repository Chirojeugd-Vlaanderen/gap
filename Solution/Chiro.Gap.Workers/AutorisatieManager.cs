using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Gap.Data.Ef;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Workers
{
    public class AutorisatieManager : IAutorisatieManager
    {
        private IAutorisatieDao _dao;
        private IAuthenticatieManager _am;

        public AutorisatieManager(IAutorisatieDao dao, IAuthenticatieManager am)
        {
            _dao = dao;
            _am = am;
        }

        /// <summary>
        /// Bepaalt de gebruikersnaam van de huidig aangemelde gebruiker.
        /// (lege string indien niet van toepassing)
        /// </summary>
        /// <returns>Username aangemelde gebruiker</returns>
        public string GebruikersNaamGet()
        {
            return _am.GebruikersNaamGet();
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
        public bool IsGavGroepsWerkJaar(int groepsWerkJaarID)
        {
            return _dao.IsGavGroepsWerkJaar(GebruikersNaamGet(), groepsWerkJaarID);
        }

        /// <summary>
        /// Controleert of een afdeling gekoppeld is aan een groep waarvan
        /// de gebruiker GAV is.
        /// </summary>
        /// <param name="afdelingsID">ID gevraagde afdeling</param>
        /// <returns>True indien de gebruiker GAV is van de groep van de
        /// afdeling.</returns>
        public bool IsGavAfdeling(int afdelingsID)
        {
            return _dao.IsGavAfdeling(GebruikersNaamGet(), afdelingsID);
        }

        /// <summary>
        /// Controleert of een lid lid is van een groep waarvan de gebruiker
        /// GAV is.
        /// </summary>
        /// <param name="lidID">ID van het betreffende lid</param>
        /// <returns>True indien het een lid van een eigen groep is</returns>
        public bool IsGavLid(int lidID)
        {
            return _dao.IsGavLid(GebruikersNaamGet(), lidID);
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
        public IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs)
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

        /// <summary>
        /// Controleert of de huidig aangelogde gebruiker momenteel
        /// GAV is van de groep gekoppeld aan een zekere categorie.
        /// </summary>
        /// <param name="categorieID">ID van de categorie</param>
        /// <returns>true indien GAV</returns>
        public bool IsGavCategorie(int categorieID)
        {
            return _dao.IsGavCategorie(categorieID, GebruikersNaamGet());
        }

        /// <summary>
        /// Controleert of de huidig aangelogde gebruiker momenteel
        /// GAV is van de groep gekoppeld aan een zekere commvorm.
        /// </summary>
        /// <param name="commvormID">ID van de commvorm</param>
        /// <returns>true indien GAV</returns>
        public bool IsGavCommVorm(int commvormID)
        {
            return _dao.IsGavCommVorm(commvormID, GebruikersNaamGet());
        }
    }
}
