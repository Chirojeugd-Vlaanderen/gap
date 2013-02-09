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
        GebruikersRecht GebruikersRechtToekennen(string gav, int groep, System.DateTime vervalDatum);

        /// <summary>
        /// Geeft <c>true</c> als de momenteel aangelogde gebruiker beheerder is van het gegeven object
        /// <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">Object waarvoor gebruikersrecht nagekeken moet worden</param>
        /// <returns>
        /// <c>true</c> als de momenteel aangelogde gebruiker beheerder is van de gegeven
        /// <paramref name="obj"/>.
        /// </returns>
        bool IsGav(Groep groep);
        bool IsGav(CommunicatieVorm communicatie);
        bool IsGav(GroepsWerkJaar g);
        bool IsGav(GelieerdePersoon gelieerdePersoon);
        bool IsGav(Deelnemer d);
        bool IsGav(Plaats p);
        bool IsGav(Uitstap u);
        bool IsGav(GebruikersRecht g);
        bool IsGav(Lid l);
        bool IsGav(Afdeling a);
        bool IsGav(Categorie c);
    }
}
