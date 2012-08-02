// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

using Chiro.Cdf.Data;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Exceptions;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. autorisatie bevat
    /// </summary>
    public class AutorisatieManager : IAutorisatieManager
    {
        #region Private members

        private readonly IAutorisatieDao _autorisatieDao;
        private readonly IGavDao _gavDao;
        private readonly IGroepenDao _groepenDao;
        private readonly IFunctiesDao _functiesDao;

        private readonly IAuthenticatieManager _am;

        // Eigenlijk IAutorisatieDao ook een IDao<GebruikersRecht>.  Maar voor IAutorisatieDao
        // wordt soms een mock gebruikt, om bijv. altijd gebruikersrechten te krijgen.
        // De _gebruikersRechtDao moet gebruikt worden om nieuwe gebruikersrechten te persisteren.
        private readonly IDao<GebruikersRecht> _gebruikersrechtDao;

        #endregion

        #region Constructor

        /// <summary>
        /// Maakt een nieuwe AutorisatieManager
        /// </summary>
        /// <param name="autorisatieDao">
        /// Repository om te kijken of de gebruiker wel recht heeft om de methods
        /// van deze worker te gebruiken
        /// </param>
        /// <param name="gavDao">
        /// Repository voor GAV's
        /// </param>
        /// <param name="groepenDao">
        /// Repository voor groepen
        /// </param>
        /// <param name="functiesDao">
        /// Repository voor functies
        /// </param>
        /// <param name="gebruikersrechtDao">
        /// Repository om gebruikersrechten te persisteren
        /// </param>
        /// <param name="am">
        /// AuthenticatieManager (bepaalt gebruikersnaam)
        /// </param>
        public AutorisatieManager(
            IAutorisatieDao autorisatieDao, 
            IGavDao gavDao, 
            IGroepenDao groepenDao, 
            IFunctiesDao functiesDao, 
            IDao<GebruikersRecht> gebruikersrechtDao, 
            IAuthenticatieManager am)
        {
            _autorisatieDao = autorisatieDao;
            _gavDao = gavDao;
            _groepenDao = groepenDao;
            _functiesDao = functiesDao;
            _gebruikersrechtDao = gebruikersrechtDao;

            _am = am;
        }

        #endregion

        #region IsGav...

        /// <summary>
        /// IsGav geeft true als de aangelogde user
        /// gav is voor de groep met gegeven ID
        /// </summary>
        /// <param name="groepID">
        /// ID van de groep
        /// </param>
        /// <returns>
        /// <c>True</c> (enkel) als user gav is
        /// </returns>
        public bool IsGavGroep(int groepID)
        {
            return _autorisatieDao.IsGavGroep(GebruikersNaamGet(), groepID);
        }

        /// <summary>
        /// IsGavGroepen geeft <c>true</c> als de aangelogde user
        /// gav is voor alle groepen met gegeven ID's
        /// </summary>
        /// <param name="groepIDs">
        /// ID's van de groepen
        /// </param>
        /// <returns>
        /// <c>True</c> (enkel) als user GAV is van alle groepen
        /// </returns>
        public bool IsGavGroepen(IEnumerable<int> groepIDs)
        {
            // TODO (#1039): functionalieit in autorisatieDao inbouwen zoat dit efficienter kan.
            return groepIDs.All(id => _autorisatieDao.IsGavGroep(GebruikersNaamGet(), id));
        }

        /// <summary>
        /// Nagaan of de ingelogde user overeenkomt met een GAV van een groep 
        /// gelieerd aan de gelieerde persoon met gegeven ID
        /// </summary>
        /// <param name="gelieerdePersoonID">
        /// ID van te checken gelieerde persoon
        /// </param>
        /// <returns>
        /// <c>True</c> als de user de persoonsgegevens mag zien/bewerken
        /// </returns>
        public bool IsGavGelieerdePersoon(int gelieerdePersoonID)
        {
            return gelieerdePersoonID == 0 ||
                   _autorisatieDao.IsGavGelieerdePersoon(GebruikersNaamGet(), gelieerdePersoonID);
        }

        /// <summary>
        /// Nagaan of de ingelogde user overeenkomt met een GAV van een groep gelieerd aan de
        /// persoon met gegeven ID
        /// </summary>
        /// <param name="persoonID">
        /// ID van te checken Persoon
        /// </param>
        /// <returns>
        /// <c>True</c> Als de user de persoonsgegevens mag zien/bewerken
        /// </returns>
        public bool IsGavPersoon(int persoonID)
        {
            return _autorisatieDao.IsGavPersoon(GebruikersNaamGet(), persoonID);
        }

        /// <summary>
        /// Nagaan of de ingelogde user overeenkomt met een GAV van de groep van een GroepsWerkJaar
        /// </summary>
        /// <param name="groepsWerkJaarID">
        /// ID gevraagde groepswerkjaar
        /// </param>
        /// <returns>
        /// <c>True</c> als aangemelde gebruiker GAV is
        /// </returns>
        public bool IsGavGroepsWerkJaar(int groepsWerkJaarID)
        {
            return _autorisatieDao.IsGavGroepsWerkJaar(GebruikersNaamGet(), groepsWerkJaarID);
        }

        /// <summary>
        /// Controleert of een afdeling gekoppeld is aan een groep waarvan
        /// de gebruiker GAV is.
        /// </summary>
        /// <param name="afdelingsID">
        /// ID gevraagde afdeling
        /// </param>
        /// <returns>
        /// <c>True</c> als de gebruiker GAV is van de groep van de
        /// afdeling
        /// </returns>
        public bool IsGavAfdeling(int afdelingsID)
        {
            return _autorisatieDao.IsGavAfdeling(GebruikersNaamGet(), afdelingsID);
        }

        /// <summary>
        /// Controleert of een afdelingsjaar gekoppeld is aan een groep waarvan
        /// de gebruiker GAV is.
        /// </summary>
        /// <param name="afdelingsJaarID">
        /// ID gevraagde afdelingsJaar
        /// </param>
        /// <returns>
        /// <c>True</c> als de gebruiker GAV is van de groep van het
        /// afdelingsjaar
        /// </returns>
        public bool IsGavAfdelingsJaar(int afdelingsJaarID)
        {
            return _autorisatieDao.IsGavAfdelingsJaar(GebruikersNaamGet(), afdelingsJaarID);
        }

        /// <summary>
        /// Controleert of een lid lid is van een groep waarvan de gebruiker
        /// GAV is.
        /// </summary>
        /// <param name="lidID">
        /// ID van het betreffende lid
        /// </param>
        /// <returns>
        /// <c>True</c> als het een lid van een eigen groep is
        /// </returns>
        public bool IsGavLid(int lidID)
        {
            return _autorisatieDao.IsGavLid(GebruikersNaamGet(), lidID);
        }

        /// <summary>
        /// Geeft <c>true</c> als de functie met ID <paramref name="functieID"/> nationaal gedefinieerd is
        /// of gekoppeld is aan een groep waar de aangelogde gebruiker momenteel GAV van is.  Anders
        /// <c>false</c>.
        /// </summary>
        /// <param name="functieID">
        /// ID van de functie
        /// </param>
        /// <returns>
        /// <c>true</c> als de functie met ID <paramref name="functieID"/> nationaal gedefinieerd is
        /// of gekoppeld is aan een groep waar de aangelogde gebruiker momenteel GAV van is.  Anders
        /// <c>false</c>.
        /// </returns>
        public bool IsGavFunctie(int functieID)
        {
            Functie f = _functiesDao.Ophalen(functieID, fnc => fnc.Groep);

            return f.IsNationaal || IsGavGroep(f.Groep.ID);
        }

        /// <summary>
        /// Controleert of de huidig aangelogde gebruiker momenteel
        /// GAV is van de groep gekoppeld aan een zekere categorie.
        /// </summary>
        /// <param name="categorieID">
        /// ID van de categorie
        /// </param>
        /// <returns>
        /// <c>True</c> als GAV
        /// </returns>
        public bool IsGavCategorie(int categorieID)
        {
            return _autorisatieDao.IsGavCategorie(categorieID, GebruikersNaamGet());
        }

        /// <summary>
        /// Controleert of de huidig aangelogde gebruiker momenteel
        /// GAV is van de groep gekoppeld aan een zekere commvorm.
        /// </summary>
        /// <param name="commvormID">
        /// ID van de commvorm
        /// </param>
        /// <returns>
        /// <c>True</c> als GAV
        /// </returns>
        public bool IsGavCommVorm(int commvormID)
        {
            return _autorisatieDao.IsGavCommVorm(commvormID, GebruikersNaamGet());
        }

        /// <summary>
        /// Geeft <c>true</c> als het persoonsAdres met ID <paramref name="persoonsAdresID"/> gekoppeld is aan een persoon
        /// waarop de aangelogde gebruiker momenteel GAV-rechten op heeft.  Anders
        /// <c>false</c>.
        /// </summary>
        /// <param name="persoonsAdresID">
        /// ID van de functie
        /// </param>
        /// <returns>
        /// <c>true</c> als het persoonsAdres met ID <paramref name="persoonsAdresID"/> gekoppeld is aan een persoon
        /// waarop de aangelogde gebruiker momenteel GAV-rechten op heeft.  Anders
        /// <c>false</c>.
        /// </returns>
        public bool IsGavPersoonsAdres(int persoonsAdresID)
        {
            return _autorisatieDao.IsGavPersoonsAdres(persoonsAdresID, GebruikersNaamGet());
        }

        /// <summary>
        /// Geeft <c>true</c> als alle persoonsAdressen met ID in <paramref name="persoonsAdresIDs"/> gekoppeld zijn aan een 
        /// personen waarop de aangelogde gebruiker momenteel GAV-rechten op heeft.  Anders
        /// <c>false</c>.
        /// </summary>
        /// <param name="persoonsAdresIDs">
        /// ID van de functie
        /// </param>
        /// <returns>
        /// <c>true</c> als alle persoonsAdressen met ID in <paramref name="persoonsAdresIDs"/> gekoppeld zijn aan een 
        /// personen waarop de aangelogde gebruiker momenteel GAV-rechten op heeft.  Anders
        /// <c>false</c>.
        /// </returns>
        public bool IsGavPersoonsAdressen(IEnumerable<int> persoonsAdresIDs)
        {
            // TODO (#1040): functionalieit in autorisatieDao inbouwen zoat dit efficienter kan.
            return persoonsAdresIDs.All(id => _autorisatieDao.IsGavPersoonsAdres(id, GebruikersNaamGet()));
        }

        /// <summary>
        /// Controleert of de aangelogde gebruiker GAV-rechten heeft op de uitstap
        /// met id <paramref name="uitstapID"/>.
        /// </summary>
        /// <param name="uitstapID">
        /// ID van een uitstap
        /// </param>
        /// <returns>
        /// <c>True</c> als de gebruiker GAV-rechten heeft op de uitstap, <c>false</c> als dat niet zo is
        /// </returns>
        public bool IsGavUitstap(int uitstapID)
        {
            return _autorisatieDao.IsGavUitstap(uitstapID, GebruikersNaamGet());
        }

        #endregion

        #region Ophalen/uitfilteren

        /// <summary>
        /// Ophalen van HUIDIGE gekoppelde groepen voor een aangemelde GAV
        /// </summary>
        /// <returns>
        /// Gekoppelde groepen
        /// </returns>
        public IEnumerable<Groep> MijnGroepenOphalen()
        {
            return _autorisatieDao.MijnGroepenOphalen(GebruikersNaamGet());
        }

        /// <summary>
        /// Ophalen van ID's van HUIDIGE gekoppelde groepen voor een aangemelde GAV
        /// </summary>
        /// <returns>
        /// ID's van gekoppelde groepen
        /// </returns>
        public IEnumerable<int> MijnGroepIDsOphalen()
        {
            return (from g in MijnGroepenOphalen() select g.ID).ToArray();
        }

        /// <summary>
        /// Controleert of de aangelogde gebruiker op dit moment GAV-rechten heeft op de plaats
        /// met id <paramref name="plaatsID"/>.
        /// </summary>
        /// <param name="plaatsID">
        /// ID van een bivakplaats
        /// </param>
        /// <returns>
        /// <c>true</c> als de aangemelde user nu GAV is voor de plaats, anders <c>false</c>. 
        /// </returns>
        public bool IsGavPlaats(int plaatsID)
        {
            return _autorisatieDao.IsGavPlaats(plaatsID, GebruikersNaamGet());
        }

        /// <summary>
        /// Controleert of de aangelogde gebruiker op dit moment GAV-rechten heeft op de deelnemer
        /// met ID <paramref name="deelnemerID"/>
        /// </summary>
        /// <param name="deelnemerID">
        /// ID van een (uitstap)deelnemer
        /// </param>
        /// <returns>
        /// <c>true</c> als de aangemelde gebruiker GAV-rechten heeft voor de gevraagde 
        /// deelnemer, anders <c>false</c>
        /// </returns>
        public bool IsGavDeelnemer(int deelnemerID)
        {
            return _autorisatieDao.IsGavDeelnemer(deelnemerID, GebruikersNaamGet());
        }

        /// <summary>
        /// Controleert of de aangelogde gebruiker op dit moment GAV-rechten heeft op het gebruikersrecht
        /// met ID <paramref name="gebruikersRechtID"/>.  Dit is het geval als het gebruikersrecht
        /// betrekking heeft op een groep waar de huidige gebruiker GAV van is.
        /// </summary>
        /// <param name="gebruikersRechtID">
        /// ID van een gebruikersrecht
        /// </param>
        /// <returns>
        /// <c>true</c> als de aangemelde gebruiker GAV-rechten heeft voor het gevraagde 
        /// gebruikersrecht, anders <c>false</c>
        /// </returns>
        public bool IsGavGebruikersRecht(int gebruikersRechtID)
        {
            return _autorisatieDao.IsGavGebruikersRecht(gebruikersRechtID, GebruikersNaamGet());
        }

        /// <summary>
        /// Verwijdert uit een lijst van GelieerdePersoonID's de ID's
        /// van GelieerdePersonen voor wie de aangemelde gebruiker geen GAV is
        /// </summary>
        /// <param name="gelieerdePersonenIDs">
        /// Lijst met ID's van gelieerde personen
        /// </param>
        /// <returns>
        /// Enkel de ID's van die personen voor wie de gebruiker GAV is
        /// </returns>
        public IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs)
        {
            return _autorisatieDao.EnkelMijnGelieerdePersonen(gelieerdePersonenIDs, GebruikersNaamGet());
        }

        /// <summary>
        /// Verwijdert uit een lijst van PersoonID's de ID's
        /// van Personen voor wie de aangemelde gebruiker geen GAV is
        /// </summary>
        /// <param name="personenIDs">
        /// Lijst met ID's van personen
        /// </param>
        /// <returns>
        /// Enkel de ID's van die personen voor wie de gebruiker GAV is
        /// </returns>
        public IList<int> EnkelMijnPersonen(IEnumerable<int> personenIDs)
        {
            return _autorisatieDao.EnkelMijnPersonen(personenIDs, GebruikersNaamGet());
        }

        /// <summary>
        /// Verwijdert uit een lijst van LidID's de ID's
        /// van leden voor wie de aangemelde gebruiker geen GAV is.
        /// </summary>
        /// <param name="lidIDs">
        /// ID's van leden
        /// </param>
        /// <returns>
        /// Enkel de ID's van leden waarvoor de gebruiker GAV is.
        /// </returns>
        public IEnumerable<int> EnkelMijnLeden(IEnumerable<int> lidIDs)
        {
            return _autorisatieDao.EnkelMijnLeden(lidIDs, GebruikersNaamGet());
        }

        #endregion

        #region Misc

        /// <summary>
        /// Controleert of de aangelogde user
        /// 'superrechten' heeft
        /// (zoals leden verwijderen uit vorig werkJaar, 
        /// leden verwijderen voor wie de probeerperiode voorbij is, enz.)
        /// </summary>
        /// <returns>
        /// <c>True</c> (enkel) als user supergav is
        /// </returns>
        public bool IsSuperGav()
        {
            return false; // voorlopig zijn er geen supergebruikers
        }

        /// <summary>
        /// Geeft de gegeven <paramref name="gav"/> gebruikersrecht voor de gegeven <paramref name="groep"/>,
        /// met een zekere <paramref name="vervalDatum"/>.  Persisteert niet.
        /// </summary>
        /// <param name="gav">
        /// GAV die gebruikersrecht moet krijgen
        /// </param>
        /// <param name="groep">
        /// Groep waarvoor gebruikersrecht verleend moet worden
        /// </param>
        /// <param name="vervalDatum">
        /// Vervaldatum van het gebruikersrecht
        /// </param>
        /// <returns>
        /// Het gegeven GebruikersRecht
        /// </returns>
        /// <remarks>
        /// Aan de GAV moeten al zijn gebruikersrechten op voorhand gekoppeld zijn.
        /// Als er al een gebruikersrecht bestaat, wordt gewoon de vervaldatum aangepast.
        /// </remarks>
        public GebruikersRecht GebruikersRechtToekennen(Gav gav, Groep groep, DateTime vervalDatum)
        {
            // Je mag enkel gebruikersrechten toekennen aan groepen waarvan je zelf GAV bent.
            if (IsGavGroep(groep.ID))
            {
                GebruikersRecht resultaat = null;

                // Bestaat er al een gebruikersrecht?
                resultaat = (from gr in gav.GebruikersRecht
                             where gr.Groep.ID == groep.ID
                             select gr).FirstOrDefault();

                if (resultaat == null)
                {
                    // nee: creëren
                    resultaat = new GebruikersRecht
                                    {
                                        Groep = groep, 
                                        Gav = gav
                                    };
                    groep.GebruikersRecht.Add(resultaat);
                    gav.GebruikersRecht.Add(resultaat);
                }

                resultaat.VervalDatum = vervalDatum;

                return resultaat;
            }
            else
            {
                throw new GeenGavException(Resources.GeenGav);
            }
        }

        /// <summary>
        /// Geeft de GAV met gegeven <paramref name="login"/> gebruikersrecht voor de groep met gegeven 
        /// <paramref name="groepID"/>,
        /// met een zekere <paramref name="vervalDatum"/>.  Persisteert WEL.
        /// </summary>
        /// <param name="login">
        /// ID van GAV die gebruikersrecht moet krijgen
        /// </param>
        /// <param name="groepID">
        /// ID van groep waarvoor gebruikersrecht verleend moet worden
        /// </param>
        /// <param name="vervalDatum">
        /// Vervaldatum van het gebruikersrecht
        /// </param>
        /// <returns>
        /// Het gegeven GebruikersRecht
        /// </returns>
        /// <remarks>
        /// Als er al een gebruikersrecht bestaat, wordt gewoon de vervaldatum aangepast.
        /// </remarks>
        public GebruikersRecht GebruikersRechtToekennen(string login, int groepID, DateTime vervalDatum)
        {
            // Je mag enkel gebruikersrechten toekennen aan groepen waarvan je zelf GAV bent.
            if (IsGavGroep(groepID))
            {
                var gav = _gavDao.Ophalen(login);

                // probeer groep al te vinden via huidige gebruikersrechten
                var groep = (from recht in gav.GebruikersRecht
                             where recht.Groep.ID == groepID
                             select recht.Groep).FirstOrDefault();

                if (groep == null)
                {
                    // indien niet gevonden: apart ophalen
                    groep = _groepenDao.Ophalen(groepID, grp => grp.GebruikersRecht.First().Gav);
                }

                var resultaat = GebruikersRechtToekennen(gav, groep, vervalDatum);

                _gebruikersrechtDao.Bewaren(
                    resultaat, 
                    grecht => grecht.Groep.WithoutUpdate(), 
                    grecht => grecht.Gav.WithoutUpdate());

                return resultaat;
            }
            else
            {
                throw new GeenGavException(Resources.GeenGav);
            }
        }

        /// <summary>
        /// Bepaalt de gebruikersnaam van de huidig aangemelde gebruiker.
        /// (lege string indien niet van toepassing)
        /// </summary>
        /// <returns>
        /// Username aangemelde gebruiker
        /// </returns>
        public string GebruikersNaamGet()
        {
            return _am.GebruikersNaamGet();
        }

        #endregion
    }
}