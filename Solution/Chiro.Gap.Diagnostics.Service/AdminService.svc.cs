﻿using System;
using System.Linq;
using System.Security.Permissions;

using AutoMapper;

using Chiro.Gap.Diagnostics.ServiceContracts;
using Chiro.Gap.Diagnostics.ServiceContracts.DataContracts;
using Chiro.Gap.Diagnostics.Workers;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.Workers;

namespace Chiro.Gap.Diagnostics.Service
{
    /// <summary>
    /// Webservice voor diagnostische en administratieve zaken
    /// </summary>
    public class AdminService : IAdminService
    {
        private readonly GroepenManager _groepenManager;
        private readonly GebruikersRechtenManager _gebruikersRechtenManager;
        private readonly LedenManager _ledenManager;
        private readonly GelieerdePersonenManager _gelieerdePersonenManager;
        private readonly GroepsWerkJaarManager _groepsWerkJaarManager;
        private readonly ProblemenManager _problemenManager;

        // De sync-opdrachten worden rechtstreeks vanuit deze service gegeven.
        // Anderzijds bevat iedere worker ook een sync-object.  Moet de sync
        // dan niet via de workers gebeuren? Bijv. _ledenManager.FunctiesOpnieuwSyncen
        // ipv. _ledenSync.FunctiesOpnieuwSyncen.
        // Of wordt het dan te complex?

        private readonly IAdressenSync _adressenSync;
        private readonly ILedenSync _ledenSync;

        // LIVE
        //private const string SECURITYGROEP = @"KIPDORP\g-GapSuper";

        // DEV
        private const string SECURITYGROEP = @".\Users";

        /// <summary>
        /// Constructor voor de AdminService.  De workers uit de backend (en ihb
        /// diens dependency's) worden geinjecteerd via de DependencyInjectionBehavior.
        /// </summary>
        /// <param name="groepenManager">Bevat groepsgerelateerde methods van de backend</param>
        /// <param name="gebruikersRechtenManager">Bevat gebruikersrechtgerelateerde methods van de backend</param>
        /// <param name="ledenManager">Bevat ledengerelateerde methods van de backend</param>
        /// <param name="gelieerdePersonenManager">Methods van backend m.b.t. gelieerde personen</param>
        /// <param name="groepsWerkJaarManager">Methods van backend m.b.t. groepswerkjaar</param>
        /// <param name="problemenManager">Methods van backend m.b.t. diagnostics</param>
        /// <param name="adressenSync">zorgt voor de synchronisatie van de adressen naar Kipadmin</param>
        /// <param name="ledenSync">zorgt voor de synchronisatie van de leden naar Kipadmin</param>
        public AdminService(
            GroepenManager groepenManager, 
            GebruikersRechtenManager gebruikersRechtenManager,
            GelieerdePersonenManager gelieerdePersonenManager,
            GroepsWerkJaarManager groepsWerkJaarManager,
            LedenManager ledenManager,
            ProblemenManager problemenManager,
            IAdressenSync adressenSync,
            ILedenSync ledenSync)
        {
            _groepenManager = groepenManager;
            _gebruikersRechtenManager = gebruikersRechtenManager;
            _ledenManager = ledenManager;
            _gelieerdePersonenManager = gelieerdePersonenManager;
            _groepsWerkJaarManager = groepsWerkJaarManager;
            _problemenManager = problemenManager;
            _adressenSync = adressenSync;
            _ledenSync = ledenSync;
        }

        /// <summary>
        /// Hello-world method, enkel voor testing purposes
        /// </summary>
        /// <returns>"Hello World!"</returns>
        [PrincipalPermission(SecurityAction.Demand, Role = SECURITYGROEP)]
        public string Hello()
        {
            return "Hello World!";
        }

        /// <summary>
        /// Haalt basisgegevens van de groep met stamnr <paramref name="code"/> op, 
        /// samen met de e-mailadressen van contactpersoon en gekende GAV's
        /// </summary>
        /// <param name="code">stamnummer van de groep</param>
        /// <returns>basisgegevens van de groep, en e-mailadressen van contactpersoon
        /// en gekende GAV's</returns>
        [PrincipalPermission(SecurityAction.Demand, Role = SECURITYGROEP)]
        public GroepContactInfo ContactInfoOphalen(string code)
        {
            var groepsWerkJaar = _groepsWerkJaarManager.RecentsteOphalen(code, GroepsWerkJaarExtras.Groep);
            
            var result = Mapper.Map<Groep, GroepContactInfo>(groepsWerkJaar.Groep);

            var contactpersonenMetEmail =
                (from l in _ledenManager.Zoeken(new LidFilter
                                                    {
                                                        FunctieID = (int) NationaleFunctie.ContactPersoon,
                                                        LidType = LidType.Leiding,
                                                        GroepsWerkJaarID = groepsWerkJaar.ID
                                                    },
                                                LidExtras.Communicatie|LidExtras.Persoon)
                 where !String.IsNullOrEmpty(GelieerdePersonenManager.EMailKiezen(l.GelieerdePersoon))
                 select new MailContactInfo
                            {
                                GelieerdePersoonID = l.GelieerdePersoon.ID,
                                EmailAdres = GelieerdePersonenManager.EMailKiezen(l.GelieerdePersoon),
                                IsContact = false,
                                IsGav = false,
                                VolledigeNaam = l.GelieerdePersoon.Persoon.VolledigeNaam
                            }).ToArray();

            var gekendeGavs = (from gp in _gebruikersRechtenManager.GavGelieerdePersonenOphalen(groepsWerkJaar.Groep.ID)
                               where !String.IsNullOrEmpty(GelieerdePersonenManager.EMailKiezen(gp))
                               select new MailContactInfo
                                          {
                                              GelieerdePersoonID = gp.ID,
                                              EmailAdres = GelieerdePersonenManager.EMailKiezen(gp),
                                              IsContact = false,
                                              IsGav = false,
                                              VolledigeNaam = gp.Persoon.VolledigeNaam
                                          }).ToArray();

            result.Contacten = contactpersonenMetEmail.Union(gekendeGavs)
                // Ik had hier graag een .Distinct() gedaan, maar dat werkt niet.  Ook  niet als ik
                // Equals en GetHashCode overload.  Dus doe ik het omslachtig
                .GroupBy(src => src.GelieerdePersoonID).Select(src => src.First()).ToArray();

            // omslachtige loops, maar normaal gezien zal het over een beperkt aantal gaan))

            foreach (var c in result.Contacten)
            {
                int gpid = c.GelieerdePersoonID;
                c.IsContact = (contactpersonenMetEmail.Any(cp => cp.GelieerdePersoonID == gpid));
                c.IsGav = (gekendeGavs.Any(cp => cp.GelieerdePersoonID == gpid));
            }
            
            return result;
        }

        /// <summary>
        /// Geeft de momenteel aangelogde gebruiker tijdelijke rechten voor de groep 
        /// van de gelieerde persoon met ID <paramref name="notificatieGelieerdePersoonID"/>.
        /// Deze gelieerde persoon wordt hiervan via  e-mail op de hoogte gebracht.  
        /// Uiteraard moet die gelieerde persoon een e-mailadres hebben. 
        /// De tijdelijke rechten zijn geldig voor het aantal dagen gegeven in de settings van
        /// <see name="Chiro.Gap.Diagnostics.Service" />
        /// Zo nodig kan een tijdelijke gebruiker zelf zijn eigen rechten verlengen.
        /// </summary>
        /// <param name="notificatieGelieerdePersoonID">GelieerdePersoonID die de groep
        /// bepaalt, en meteen ook diegene die via e-mail
        /// verwittigd wordt over de tijdelijke login</param>
        /// <param name="reden">Extra informatie die naar de notificatie-ontvanger wordt
        /// gestuurd.</param>
        [PrincipalPermission(SecurityAction.Demand, Role = SECURITYGROEP)]
        public void TijdelijkeRechtenGeven(int notificatieGelieerdePersoonID, string reden)
        {
            var gav = _gebruikersRechtenManager.MijnGavOphalen();

            var notificatieOntvanger = _gelieerdePersonenManager.Ophalen(notificatieGelieerdePersoonID, PersoonsExtras.Communicatie|PersoonsExtras.Groep);
            _gebruikersRechtenManager.ToekennenOfVerlengen(gav,
                                                               notificatieOntvanger,
                                                               DateTime.Now.AddDays(
                                                                   Properties.Settings.Default.
                                                                       DagenTijdelijkGebruikersRecht), reden);
        }

        /// <summary>
        /// Haalt het aantal adressen op dat niet doorgekomen is naar Kipadmin.
        /// </summary>
        /// <returns>Het aantal (voorkeurs)adressen in GAP waarvoor de persoon in Kipadmin geen
        /// adres heeft.</returns>
        [PrincipalPermission(SecurityAction.Demand, Role = SECURITYGROEP)]
        public int AantalVerdwenenAdressenOphalen()
        {
            return _problemenManager.VerdwenenAdressenOphalen().Count();
        }

        /// <summary>
        /// Synct de adressen die niet doorkwamen naar Kipadmin opnieuw
        /// </summary>
        [PrincipalPermission(SecurityAction.Demand, Role = SECURITYGROEP)]
        public void OntbrekendeAdressenSyncen()
        {
            var gpIds = _problemenManager.VerdwenenAdressenOphalen().Select(row => row.GelieerdePersoonID).ToArray();
            var gelieerdePersonen = _gelieerdePersonenManager.Ophalen(gpIds, PersoonsExtras.VoorkeurAdres);
            var persoonsAdressen = gelieerdePersonen.Select(gp => gp.PersoonsAdres).ToArray();

            _adressenSync.StandaardAdressenBewaren(persoonsAdressen);
        }

        /// <summary>
        /// Haalt het aantal functie-inconsistenties op voor het huidige werkjaar
        /// </summary>
        /// <returns>Het aantal functies in GAP dat niet in Kipadmin gevonden wordt, plus
        /// het aantal functies in Kipadmin dat niet in GAP gevonden wordt.</returns>
        [PrincipalPermission(SecurityAction.Demand, Role = SECURITYGROEP)]
        public int AantalFunctieFoutenOphalen()
        {
            return _problemenManager.FunctieProblemenOphalen().Count();
        }


        /// <summary>
        /// Hersynchroniseert de functies van de leden met functieproblemen (huidig werkjaar)
        /// </summary>
        [PrincipalPermission(SecurityAction.Demand, Role = SECURITYGROEP)]
        public void FunctieProbleemLedenOpnieuwSyncen()
        {
            var lidIDs = _problemenManager.FunctieProblemenOphalen().Select(fp => fp.GapLidID).ToArray();
            var leden = _ledenManager.Ophalen(lidIDs, LidExtras.Persoon | LidExtras.Functies | LidExtras.Groep);

            foreach (var lid in leden)
            {
                _ledenSync.FunctiesUpdaten(lid);
            }
        }
    }
}