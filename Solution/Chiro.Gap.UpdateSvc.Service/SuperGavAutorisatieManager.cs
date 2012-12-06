// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.ServiceModel;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.UpdateSvc.Service
{
    /// <summary>
    /// Autorisatiemanager die slechts 2 dingen doet:
    /// 1. Zeggen 'Ik ben super-GAV'.
    /// 2. De gebruikersnaam opleveren
    /// Alle andere zaken zijn niet-geïmplementeerd, en voor de veiligheid
    /// blijven ze dat best ook.
    /// </summary>
    /// <remarks>Deze klasse is een kopie van die in Chiro.Gap.UpdataSvc.Service.  Het lijkt
    /// me zo dom om hiervoor een apart project te maken.  Maar ik weet ook niet
    /// goed waar ik dit anders kwijt moet.  Ik wil dit uit Chiro.Gap.Workers houden,
    /// om te vermijden dat je super-gav kunt worden door gewoon de unity-configuration
    /// aan te passen.  Nu kun je dat enkel als Chiro.Gap.Diagnostics.Service.dll
    /// beschikbaar is.</remarks>
    public class SuperGavAutorisatieManager: IAutorisatieManager
    {

        /// <summary>
        /// Verwijdert uit een lijst van GelieerdePersoonID's de ID's
        /// van GelieerdePersonen voor wie de aangemelde gebruiker geen GAV is.
        /// </summary>
        /// <param name="gelieerdePersonenIDs">Lijst met ID's van gelieerde personen</param>
        /// <returns>Enkel de ID's van die personen voor wie de gebruiker GAV is</returns>
        /// <remarks></remarks>
        public IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Verwijdert uit een lijst van PersoonID's de ID's
        /// van personen voor wie de aangemelde gebruiker geen GAV is.
        /// </summary>
        /// <param name="personenIDs">Lijst met ID's van personen</param>
        /// <returns>Enkel de ID's van die personen voor wie de gebruiker GAV is</returns>
        /// <remarks></remarks>
        public IList<int> EnkelMijnPersonen(IEnumerable<int> personenIDs)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Verwijdert uit een lijst van LidID's de ID's
        /// van leden voor wie de aangemelde gebruiker geen GAV is.
        /// </summary>
        /// <param name="lidIDs">ID's van leden</param>
        /// <returns>Enkel de ID's van leden waarvoor de gebruiker GAV is.</returns>
        /// <remarks></remarks>
        public IEnumerable<int> EnkelMijnLeden(IEnumerable<int> lidIDs)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> EnkelMijnAfdelingen(IEnumerable<int> afdelingIDs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Ophalen van HUIDIGE gekoppelde groepen voor een aangemelde GAV
        /// </summary>
        /// <returns>ID's van gekoppelde groepen</returns>
        /// <remarks></remarks>
        public IEnumerable<Groep> MijnGroepenOphalen()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Geeft true als (en slechts als) de ingelogde user correspondeert
        /// met een GAV van een groep gelieerd aan de gelieerde
        /// persoon met gegeven ID
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van te checken gelieerde persoon</param>
        /// <returns><c>True</c> als de user de persoonsgegevens mag zien/bewerken</returns>
        /// <remarks></remarks>
        public bool IsGavGelieerdePersoon(int gelieerdePersoonID)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// IsGav geeft true als de aangelogde user
        /// gav is voor de groep met gegeven ID
        /// </summary>
        /// <param name="groepID">ID van de groep</param>
        /// <returns><c>True</c> (enkel) als user GAV is</returns>
        /// <remarks></remarks>
        public bool IsGavGroep(int groepID)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// IsGavGroepen geeft <c>true</c> als de aangelogde user
        /// gav is voor alle groepen met gegeven ID's
        /// </summary>
        /// <param name="groepIDs">ID's van de groepen</param>
        /// <returns><c>True</c> (enkel) als user GAV is van alle groepen</returns>
        /// <remarks></remarks>
        public bool IsGavGroepen(IEnumerable<int> groepIDs)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Geeft true alss de aangemelde user correspondeert
        /// met een GAV van de groep van een GroepsWerkJaar
        /// </summary>
        /// <param name="groepsWerkJaarID">ID gevraagde groepswerkjaar</param>
        /// <returns><c>True</c> als aangemelde gebruiker GAV is</returns>
        /// <remarks></remarks>
        public bool IsGavGroepsWerkJaar(int groepsWerkJaarID)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Geeft true als (en slechts als) de ingelogde user correspondeert
        /// met een GAV van een groep gelieerd aan de
        /// persoon met gegeven ID
        /// </summary>
        /// <param name="persoonID">ID van te checken Persoon</param>
        /// <returns><c>True</c> als de user de persoonsgegevens mag zien/bewerken</returns>
        /// <remarks></remarks>
        public bool IsGavPersoon(int persoonID)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Controleert of een afdeling gekoppeld is aan een groep waarvan
        /// de gebruiker GAV is.
        /// </summary>
        /// <param name="afdelingsID">ID gevraagde afdeling</param>
        /// <returns><c>True</c> als de gebruiker GAV is van de groep van de
        /// afdeling</returns>
        /// <remarks></remarks>
        public bool IsGavAfdeling(int afdelingsID)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Controleert of een afdelingsjaar gekoppeld is aan een groep waarvan
        /// de gebruiker GAV is.
        /// </summary>
        /// <param name="afdelingsJaarID">ID gevraagde afdelingsJaar</param>
        /// <returns><c>True</c> als de gebruiker GAV is van de groep van het
        /// afdelingsjaar</returns>
        /// <remarks></remarks>
        public bool IsGavAfdelingsJaar(int afdelingsJaarID)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Controleert of een lid lid is van een groep waarvan de gebruiker
        /// GAV is.
        /// </summary>
        /// <param name="lidID">ID van het betreffende lid</param>
        /// <returns><c>True</c> als het een lid van een eigen groep is</returns>
        /// <remarks></remarks>
        public bool IsGavLid(int lidID)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Controleert of de huidig aangelogde gebruiker momenteel
        /// GAV is van de groep gekoppeld aan een zekere categorie.
        /// </summary>
        /// <param name="categorieID">ID van de categorie</param>
        /// <returns><c>True</c> indien GAV</returns>
        /// <remarks></remarks>
        public bool IsGavCategorie(int categorieID)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Controleert of de huidig aangelogde gebruiker momenteel
        /// GAV is van de groep gekoppeld aan een zekere communicatievorm.
        /// </summary>
        /// <param name="commvormID">ID van de communicatievorm</param>
        /// <returns><c>True</c> indien GAV</returns>
        /// <remarks></remarks>
        public bool IsGavCommVorm(int commvormID)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Geeft <c>true</c> als de functie met ID <paramref name="functieID"/> nationaal gedefinieerd is
        /// of gekoppeld is aan een groep waar de aangelogde gebruiker momenteel GAV van is.  Anders
        /// <c>false</c>.
        /// </summary>
        /// <param name="functieID">ID van de functie</param>
        /// <returns><c>true</c> als de functie met ID <paramref name="functieID"/> nationaal gedefinieerd is
        /// of gekoppeld is aan een groep waar de aangelogde gebruiker momenteel GAV van is.  Anders
        /// <c>false</c>.</returns>
        /// <remarks></remarks>
        public bool IsGavFunctie(int functieID)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Geeft <c>true</c> als het persoonsAdres met ID <paramref name="persoonsAdresID"/> gekoppeld is aan een persoon
        /// waarop de aangelogde gebruiker momenteel GAV-rechten op heeft.  Anders
        /// <c>false</c>.
        /// </summary>
        /// <param name="persoonsAdresID">ID van de functie</param>
        /// <returns><c>true</c> als het persoonsAdres met ID <paramref name="persoonsAdresID"/> gekoppeld is aan een persoon
        /// waarop de aangelogde gebruiker momenteel GAV-rechten op heeft.  Anders
        /// <c>false</c>.</returns>
        /// <remarks></remarks>
        public bool IsGavPersoonsAdres(int persoonsAdresID)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Geeft <c>true</c> als alle persoonsAdressen met ID in <paramref name="persoonsAdresIDs"/> gekoppeld zijn aan een
        /// personen waarop de aangelogde gebruiker momenteel GAV-rechten op heeft.  Anders
        /// <c>false</c>.
        /// </summary>
        /// <param name="persoonsAdresIDs">ID van de functie</param>
        /// <returns><c>true</c> als alle persoonsAdressen met ID in <paramref name="persoonsAdresIDs"/> gekoppeld zijn aan een
        /// personen waarop de aangelogde gebruiker momenteel GAV-rechten op heeft.  Anders
        /// <c>false</c>.</returns>
        /// <remarks></remarks>
        public bool IsGavPersoonsAdressen(IEnumerable<int> persoonsAdresIDs)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Controleert of de aangelogde gebruiker GAV-rechten heeft op de uitstap
        /// met id <paramref name="uitstapID"/>.
        /// </summary>
        /// <param name="uitstapID">ID van een uitstap</param>
        /// <returns><c>true</c> als de aangemelde user GAV is voor de uitstap, anders <c>false</c>.</returns>
        /// <remarks></remarks>
        public bool IsGavUitstap(int uitstapID)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Geeft true als de aangelogde user
        /// 'superrechten' heeft
        /// (zoals het verwijderen van leden uit vorig werkJaar, het
        /// verwijderen van leden waarvan de probeerperiode voorbij is,...)
        /// </summary>
        /// <returns><c>True</c> (enkel) als user supergav is</returns>
        /// <remarks></remarks>
        public bool IsSuperGav()
        {
            return true;
        }


        /// <summary>
        /// Bepaalt de gebruikersnaam van de huidig aangemelde gebruiker.
        /// (lege string indien niet van toepassing)
        /// </summary>
        /// <returns>Username aangemelde gebruiker</returns>
        /// <remarks></remarks>
        public string GebruikersNaamGet()
        {
            return ServiceSecurityContext.Current == null ? String.Empty
                : ServiceSecurityContext.Current.WindowsIdentity.Name;
        }


        /// <summary>
        /// Levert het lijstje groepID's op van de groepen waarvoor de gebruiker GAV is.
        /// </summary>
        /// <returns>GroepID's van de goepen waarvoor de gebruiker GAV is.</returns>
        /// <remarks></remarks>
        public IEnumerable<int> MijnGroepIDsOphalen()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Controleert of de aangelogde gebruiker op dit moment GAV-rechten heeft op de plaats
        /// met id <paramref name="plaatsID"/>.
        /// </summary>
        /// <param name="plaatsID">ID van een bivakplaats</param>
        /// <returns><c>true</c> als de aangemelde user nu GAV is voor de plaats, anders <c>false</c>.</returns>
        /// <remarks></remarks>
        public bool IsGavPlaats(int plaatsID)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Controleert of de aangelogde gebruiker op dit moment GAV-rechten heeft op de deelnemer
        /// met ID <paramref name="deelnemerID"/>
        /// </summary>
        /// <param name="deelnemerID">ID van een (uitstap)deelnemer</param>
        /// <returns><c>true</c> als de aangemelde gebruiker GAV-rechten heeft voor de gevraagde
        /// deelnemer, anders <c>false</c></returns>
        /// <remarks></remarks>
        public bool IsGavDeelnemer(int deelnemerID)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Controleert of de aangelogde gebruiker op dit moment GAV-rechten heeft op het gebruikersrecht
        /// met ID <paramref name="gebruikersRechtID"/>
        /// </summary>
        /// <param name="gebruikersRechtID">ID van een gebruikersrecht</param>
        /// <returns><c>true</c> als de aangemelde gebruiker GAV-rechten heeft voor het gevraagde
        /// gebruikersrecht, anders <c>false</c></returns>
        /// <remarks></remarks>
        public bool IsGavGebruikersRecht(int gebruikersRechtID)
        {
            throw new NotImplementedException();
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
        public Chiro.Gap.Poco.Model.GebruikersRecht GebruikersRechtToekennen(string gav, int groep, DateTime vervalDatum)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Geeft de gegeven <paramref name="gav"/> gebruikersrecht voor de gegeven <paramref name="groep"/>,
        /// met een zekere <paramref name="vervalDatum"/>.  Persisteert niet.
        /// </summary>
        /// <param name="gav">GAV die gebruikersrecht moet krijgen</param>
        /// <param name="groep">Groep waarvoor gebruikersrecht verleend moet worden</param>
        /// <param name="vervalDatum">Vervaldatum van het gebruikersrecht</param>
        /// <returns>Het gegeven GebruikersRecht</returns>
        /// <remarks></remarks>
        public Poco.Model.GebruikersRecht GebruikersRechtToekennen(Gav gav, Groep groep, DateTime vervalDatum)
        {
            throw new NotImplementedException();
        }

        public bool IsGavAccount(int accountID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavGebruikersRechten(int[] gebruikersRechtIDs)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(Groep groep)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(CommunicatieVorm communicatieVorm)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(GroepsWerkJaar groepsWerkJaar)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(GelieerdePersoon gelieerdePersoon)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Gebruikerses the recht gelieerde persoon.
        /// </summary>
        /// <param name="gelieerdePersoonID">The gelieerde persoon ID.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public GebruikersRecht GebruikersRechtGelieerdePersoon(int gelieerdePersoonID)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Determines whether the specified gebruikersrecht is verlengbaar.
        /// </summary>
        /// <param name="gebruikersrecht">The gebruikersrecht.</param>
        /// <returns><c>true</c> if the specified gebruikersrecht is verlengbaar; otherwise, <c>false</c>.</returns>
        /// <remarks></remarks>
        public bool IsVerlengbaar(GebruikersRecht gebruikersrecht)
        {
            throw new NotImplementedException();
        }
    }
}
