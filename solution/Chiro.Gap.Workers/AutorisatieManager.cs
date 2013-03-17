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

        /// <summary>
        /// Controleert of de aangelogde persoon GAV is voor alle meegegeven
        /// <paramref name="gelieerdePersonen"/>
        /// </summary>
        /// <param name="gelieerdePersonen">Gelieerde personen waarvoor gebruikersrechten
        /// nagekeken moeten worden.</param>
        /// <returns>
        /// <c>true</c> als de aangelogde persoon GAV is voor alle meegegeven
        /// <paramref name="gelieerdePersonen"/>
        /// </returns>
        public bool IsGav(IList<GelieerdePersoon> gelieerdePersonen)
        {
            // Als er een gelieerde persoon is waarvoor alle GAV's een gebruikersnaam hebben
            // verschillend van de mijne, dan ben ik geen GAV voor alle gegeven personen.

            var gebruikersNaam = _authenticatieMgr.GebruikersNaamGet();
            return (from gp in gelieerdePersonen
                    where
                        gp.Groep.GebruikersRecht.All(
                            gr => String.Compare(gr.Gav.Login, gebruikersNaam, StringComparison.OrdinalIgnoreCase) != 0)
                    select gp).FirstOrDefault() == null;
        }

        /// <summary>
        /// Vertrekt van een lijst <paramref name="personen"/>. Van al die personen
        /// waarvoor de aangelogde gebruiker GAV is, worden nu de overeenkomstige
        /// gelieerde personen opgeleverd. (Dat kunnen dus meer gelieerde personen
        /// per persoon bij zitten.)
        /// </summary>
        /// <param name="personen">
        ///     Lijst van personen
        /// </param>
        /// <returns>
        /// Voor de <paramref name="personen"/>
        /// waarvoor de aangelogde gebruiker GAV is, de overeenkomstige
        /// gelieerde personen
        /// </returns>
        /// <remarks>
        /// Mogelijk zijn er meerdere gelieerde personen per persoon.
        /// </remarks>
        public List<GelieerdePersoon> MijnGelieerdePersonen(IList<Persoon> personen)
        {
            return (from gp in personen.SelectMany(p => p.GelieerdePersoon)
                    where IsGav(gp)
                    select gp).ToList();
        }
    }
}