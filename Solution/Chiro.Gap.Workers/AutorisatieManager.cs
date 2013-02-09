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
        /// Geeft <c>true</c> als de momenteel aangelogde gebruiker beheerder is van het gegeven <paramref name="object"/>.
        /// </summary>
        /// <param name="groep">Groep waarvoor gebruikersrecht nagekeken moet worden</param>
        /// <returns>
        /// <c>true</c> als de momenteel aangelogde gebruiker beheerder is van de gegeven
        /// <paramref name="groep"/>.
        /// </returns>
        public bool IsGav(Groep groep)
        {
            var gebruikersNaam = _authenticatieMgr.GebruikersNaamGet();
            return groep.GebruikersRecht.Any(gr => String.Compare(gr.Gav.Login, gebruikersNaam, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public bool IsGav(CommunicatieVorm communicatieVorm)
        {
            return IsGav(communicatieVorm.GelieerdePersoon.Groep);
        }
        public bool IsGav(GroepsWerkJaar groepsWerkJaar)
        {
            return IsGav(groepsWerkJaar.Groep);
        }
        public bool IsGav(GelieerdePersoon gelieerdePersoon)
        {
            return IsGav(gelieerdePersoon.Groep);
        }
        public bool IsGav(Deelnemer deelnemer)
        {
            return IsGav(deelnemer.GelieerdePersoon.Groep);
        }
        public bool IsGav(Plaats gelieerdePersoon)
        {
            return IsGav(gelieerdePersoon.Groep);
        }
        public bool IsGav(Uitstap uitstap)
        {
            return IsGav(uitstap.GroepsWerkJaar.Groep);
        }
        public bool IsGav(GebruikersRecht recht)
        {
            return IsGav(recht.Groep);
        }
        public bool IsGav(Afdeling afdeling)
        {
            return IsGav(afdeling.ChiroGroep);
        }
        public bool IsGav(Lid lid)
        {
            return IsGav(lid.GelieerdePersoon.Groep);
        }
        public bool IsGav(Categorie categorie)
        {
            return IsGav(categorie.Groep);
        }
    }
}