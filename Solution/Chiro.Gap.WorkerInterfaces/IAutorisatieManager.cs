// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WorkerInterfaces
{
    /// <summary>
    /// Een autorisatiemanager bepaalt de rechten van een gebruiker op entiteiten.
    /// </summary>
    public interface IAutorisatieManager
    {
        /// <summary>
        /// Verwijdert uit een lijst van GelieerdePersoonID's de ID's
        /// van GelieerdePersonen voor wie de aangemelde gebruiker geen GAV is.
        /// </summary>
        /// <param name="gelieerdePersonenIDs">
        /// Lijst met ID's van gelieerde personen
        /// </param>
        /// <returns>
        /// Enkel de ID's van die personen voor wie de gebruiker GAV is
        /// </returns>
        IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs);

        /// <summary>
        /// Verwijdert uit een lijst van PersoonID's de ID's
        /// van personen voor wie de aangemelde gebruiker geen GAV is.
        /// </summary>
        /// <param name="personenIDs">
        /// Lijst met ID's van personen
        /// </param>
        /// <returns>
        /// Enkel de ID's van die personen voor wie de gebruiker GAV is
        /// </returns>
        IList<int> EnkelMijnPersonen(IEnumerable<int> personenIDs);

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
        IEnumerable<int> EnkelMijnLeden(IEnumerable<int> lidIDs);

        /// <summary>
        /// Verwijdert uit een lijst <paramref name="afdelingIDs"/> de ID's van afdelingen voor wie de
        /// aangemelde gebruiker geen GAV is.
        /// </summary>
        /// <param name="afdelingIDs">ID's van afdelingen</param>
        /// <returns>Enkel de <paramref name="afdelingIDs"/> van afdelingen waarvoor de gebruiker GAV is.</returns>
        IEnumerable<int> EnkelMijnAfdelingen(IEnumerable<int> afdelingIDs);


        /// <summary>
        /// Ophalen van HUIDIGE gekoppelde groepen voor een aangemelde GAV
        /// </summary>
        /// <returns>
        /// ID's van gekoppelde groepen
        /// </returns>
        IEnumerable<Groep> MijnGroepenOphalen();

        /// <summary>
        /// Geeft true als (en slechts als) de ingelogde user correspondeert
        /// met een GAV van een groep gelieerd aan de gelieerde
        /// persoon met gegeven ID
        /// </summary>
        /// <param name="gelieerdePersoonID">
        /// ID van te checken gelieerde persoon
        /// </param>
        /// <returns>
        /// <c>True</c> als de user de persoonsgegevens mag zien/bewerken
        /// </returns>
        bool IsGavGelieerdePersoon(int gelieerdePersoonID);

        /// <summary>
        /// IsGav geeft true als de aangelogde user
        /// gav is voor de groep met gegeven ID
        /// </summary>
        /// <param name="groepID">
        /// ID van de groep
        /// </param>
        /// <returns>
        /// <c>True</c> (enkel) als user GAV is
        /// </returns>
        bool IsGavGroep(int groepID);

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
        bool IsGavGroepen(IEnumerable<int> groepIDs);

        /// <summary>
        /// Geeft true alss de aangemelde user correspondeert
        /// met een GAV van de groep van een GroepsWerkJaar
        /// </summary>
        /// <param name="groepsWerkJaarID">
        /// ID gevraagde groepswerkjaar
        /// </param>
        /// <returns>
        /// <c>True</c> als aangemelde gebruiker GAV is
        /// </returns>
        bool IsGavGroepsWerkJaar(int groepsWerkJaarID);

        /// <summary>
        /// Geeft true als (en slechts als) de ingelogde user correspondeert
        /// met een GAV van een groep gelieerd aan de
        /// persoon met gegeven ID
        /// </summary>
        /// <param name="persoonID">
        /// ID van te checken Persoon
        /// </param>
        /// <returns>
        /// <c>True</c> als de user de persoonsgegevens mag zien/bewerken
        /// </returns>
        bool IsGavPersoon(int persoonID);

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
        bool IsGavAfdeling(int afdelingsID);

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
        bool IsGavAfdelingsJaar(int afdelingsJaarID);

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
        bool IsGavLid(int lidID);

        /// <summary>
        /// Controleert of de huidig aangelogde gebruiker momenteel
        /// GAV is van de groep gekoppeld aan een zekere categorie.
        /// </summary>
        /// <param name="categorieID">
        /// ID van de categorie
        /// </param>
        /// <returns>
        /// <c>True</c> indien GAV
        /// </returns>
        bool IsGavCategorie(int categorieID);

        /// <summary>
        /// Controleert of de huidig aangelogde gebruiker momenteel
        /// GAV is van de groep gekoppeld aan een zekere communicatievorm.
        /// </summary>
        /// <param name="commvormID">
        /// ID van de communicatievorm
        /// </param>
        /// <returns>
        /// <c>True</c> indien GAV
        /// </returns>
        bool IsGavCommVorm(int commvormID);

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
        bool IsGavFunctie(int functieID);

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
        bool IsGavPersoonsAdres(int persoonsAdresID);

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
        bool IsGavPersoonsAdressen(IEnumerable<int> persoonsAdresIDs);

        /// <summary>
        /// Controleert of de aangelogde gebruiker GAV-rechten heeft op de uitstap
        /// met id <paramref name="uitstapID"/>.
        /// </summary>
        /// <param name="uitstapID">
        /// ID van een uitstap
        /// </param>
        /// <returns>
        /// <c>true</c> als de aangemelde user GAV is voor de uitstap, anders <c>false</c>. 
        /// </returns>
        bool IsGavUitstap(int uitstapID);

        /// <summary>
        /// Geeft true als de aangelogde user
        /// 'superrechten' heeft
        /// (zoals het verwijderen van leden uit vorig werkjaar, het 
        /// verwijderen van leden waarvan de probeerperiode voorbij is,...)
        /// </summary>
        /// <returns>
        /// <c>True</c> (enkel) als user supergav is
        /// </returns>
        bool IsSuperGav();

        /// <summary>
        /// Bepaalt de gebruikersnaam van de huidig aangemelde gebruiker.
        /// (lege string indien niet van toepassing)
        /// </summary>
        /// <returns>
        /// Username aangemelde gebruiker
        /// </returns>
        string GebruikersNaamGet();

        /// <summary>
        /// Levert het lijstje groepID's op van de groepen waarvoor de gebruiker GAV is.
        /// </summary>
        /// <returns>
        /// GroepID's van de goepen waarvoor de gebruiker GAV is.
        /// </returns>
        IEnumerable<int> MijnGroepIDsOphalen();

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
        bool IsGavPlaats(int plaatsID);

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
        bool IsGavDeelnemer(int deelnemerID);

        /// <summary>
        /// Controleert of de aangelogde gebruiker op dit moment GAV-rechten heeft op het gebruikersrecht
        /// met ID <paramref name="gebruikersRechtID"/>
        /// </summary>
        /// <param name="gebruikersRechtID">
        /// ID van een gebruikersrecht
        /// </param>
        /// <returns>
        /// <c>true</c> als de aangemelde gebruiker GAV-rechten heeft voor het gevraagde 
        /// gebruikersrecht, anders <c>false</c>
        /// </returns>
        bool IsGavGebruikersRecht(int gebruikersRechtID);

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
        Chiro.Gap.Poco.Model.GebruikersRecht GebruikersRechtToekennen(string gav, int groep, System.DateTime vervalDatum);

        /// <summary>
        /// Kijkt na of de aangelogde gebruiker GAV is van de account met gegeven <paramref name="accountID"/>.
        /// </summary>
        /// <param name="accountID">ID van de te controleren account</param>
        /// <returns><c>true</c> als de aangelogde gebruiker GAV is van de account met gegeven <paramref name="accountID"/></returns>
        /// <remarks>met account bedoel ik hetgeen nu de Gav-klasse is. Maar op termijn moet die klasse hernoemd worden naar
        /// account (zie #1357)</remarks>
        bool IsGavAccount(int accountID);

        /// <summary>
        /// Controleert of de aangelogde gebruiker GAV-rechten heeft op de gebruikersrechten met gegeven
        /// <paramref name="gebruikersRechtIDs"/>
        /// </summary>
        /// <param name="gebruikersRechtIDs">ID's van gebruikersrechten die gecontroleerd moeten worden</param>
        /// <returns><c>true</c> als de aangelogde gebruiker GAV-rechten heeft op de gebruikersrechten met gegeven
        /// <paramref name="gebruikersRechtIDs"/></returns>
        bool IsGavGebruikersRechten(int[] gebruikersRechtIDs);
    }
}
