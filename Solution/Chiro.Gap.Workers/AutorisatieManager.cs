// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. autorisatie bevat
    /// </summary>
    public class AutorisatieManager : IAutorisatieManager
    {
        private readonly IAuthenticatieManager _authenticatieMgr;

        /// <summary>
        /// Creeert een nieuwe autorisatiemanager
        /// </summary>
        /// <param name="authenticatieMgr">Deze zal de gebruikersnaam van de user opleveren</param>
        public AutorisatieManager(IAuthenticatieManager authenticatieMgr)
        {
            _authenticatieMgr = authenticatieMgr;
        }

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
        public IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs)
        {
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
        }

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
        public IList<int> EnkelMijnPersonen(IEnumerable<int> personenIDs)
        {
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
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
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Verwijdert uit een lijst <paramref name="afdelingIDs"/> de ID's van afdelingen voor wie de
        /// aangemelde gebruiker geen GAV is.
        /// </summary>
        /// <param name="afdelingIDs">ID's van afdelingen</param>
        /// <returns>Enkel de <paramref name="afdelingIDs"/> van afdelingen waarvoor de gebruiker GAV is.</returns>
        public IEnumerable<int> EnkelMijnAfdelingen(IEnumerable<int> afdelingIDs)
        {
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Ophalen van HUIDIGE gekoppelde groepen voor een aangemelde GAV
        /// </summary>
        /// <returns>
        /// ID's van gekoppelde groepen
        /// </returns>
        public IEnumerable<Groep> MijnGroepenOphalen()
        {
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
        }

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
        public bool IsGavGelieerdePersoon(int gelieerdePersoonID)
        {
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
        }

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
        public bool IsGavGroep(int groepID)
        {
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
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
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
        }

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
        public bool IsGavGroepsWerkJaar(int groepsWerkJaarID)
        {
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
        }

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
        public bool IsGavPersoon(int persoonID)
        {
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
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
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
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
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
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
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
        }

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
        public bool IsGavCategorie(int categorieID)
        {
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
        }

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
        public bool IsGavCommVorm(int commvormID)
        {
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
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
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
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
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
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
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
        }

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
        public bool IsGavUitstap(int uitstapID)
        {
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Geeft true als de aangelogde user
        /// 'superrechten' heeft
        /// (zoals het verwijderen van leden uit vorig werkjaar, het 
        /// verwijderen van leden waarvan de probeerperiode voorbij is,...)
        /// </summary>
        /// <returns>
        /// <c>True</c> (enkel) als user supergav is
        /// </returns>
        public bool IsSuperGav()
        {
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
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
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Levert het lijstje groepID's op van de groepen waarvoor de gebruiker GAV is.
        /// </summary>
        /// <returns>
        /// GroepID's van de goepen waarvoor de gebruiker GAV is.
        /// </returns>
        public IEnumerable<int> MijnGroepIDsOphalen()
        {
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
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
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
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
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
        }

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
        public bool IsGavGebruikersRecht(int gebruikersRechtID)
        {
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
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
        public GebruikersRecht GebruikersRechtToekennen(string gav, int groep, DateTime vervalDatum)
        {
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Kijkt na of de aangelogde gebruiker GAV is van de account met gegeven <paramref name="accountID"/>.
        /// </summary>
        /// <param name="accountID">ID van de te controleren account</param>
        /// <returns><c>true</c> als de aangelogde gebruiker GAV is van de account met gegeven <paramref name="accountID"/></returns>
        /// <remarks>met account bedoel ik hetgeen nu de Gav-klasse is. Maar op termijn moet die klasse hernoemd worden naar
        /// account (zie #1357)</remarks>
        public bool IsGavAccount(int accountID)
        {
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Controleert of de aangelogde gebruiker GAV-rechten heeft op de gebruikersrechten met gegeven
        /// <paramref name="gebruikersRechtIDs"/>
        /// </summary>
        /// <param name="gebruikersRechtIDs">ID's van gebruikersrechten die gecontroleerd moeten worden</param>
        /// <returns><c>true</c> als de aangelogde gebruiker GAV-rechten heeft op de gebruikersrechten met gegeven
        /// <paramref name="gebruikersRechtIDs"/></returns>
        public bool IsGavGebruikersRechten(int[] gebruikersRechtIDs)
        {
            throw new NotImplementedException(Domain.NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Geeft <c>true</c> als de momenteel aangelogde gebruiker beheerder is van de gegeven
        /// <paramref name="groep"/>.
        /// </summary>
        /// <param name="groep">Groep waarvoor gebruikersrecht nagekeken moet worden</param>
        /// <returns>
        /// <c>true</c> als de momenteel aangelogde gebruiker beheerder is van de gegeven
        /// <paramref name="groep"/>.
        /// </returns>
        public bool IsGav(Groep groep)
        {
            string gebruikersNaam = _authenticatieMgr.GebruikersNaamGet();

            return
                groep.GebruikersRecht.Any(
                    gr => String.Compare(gr.Gav.Login, gebruikersNaam, StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// Geeft <c>true</c> als de momenteel aangelogde gebruiker beheerder is van de gegeven
        /// <paramref name="communicatieVorm"/>.
        /// </summary>
        /// <param name="communicatieVorm">Een communicatievorm</param>
        /// <returns><c>true</c> als de momenteel aangelogde gebruiker beheerder is van de gegeven
        /// <paramref name="communicatieVorm"/>.</returns>
        public bool IsGav(CommunicatieVorm communicatieVorm)
        {
            string gebruikersNaam = _authenticatieMgr.GebruikersNaamGet();

            return communicatieVorm.GelieerdePersoon.Groep.GebruikersRecht.Any(
                gr => String.Compare(gr.Gav.Login, gebruikersNaam, StringComparison.OrdinalIgnoreCase) == 0);
        }
    }
}