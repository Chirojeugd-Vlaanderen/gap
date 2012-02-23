// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;

using AutoMapper;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.Workers;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Services
{
    // OPM: als je de naam van de class "GroepenService" hier verandert, moet je ook de sectie "Services" in web.config aanpassen.

    // *BELANGRIJK*: Als het debuggen hier stopt owv een autorisatiefout, kijk dan na of de gebruiker waarmee
    // je aangemeld bent, op je lokale computer in de groep CgUsers zit.

    /// <summary>
    /// Service voor operaties op groepsniveau
    /// </summary>
    public class GroepenService : IGroepenService
    {
        #region Manager Injection

        private readonly GroepenManager _groepenMgr;
        private readonly ChiroGroepenManager _chiroGroepenMgr;
        private readonly AfdelingsJaarManager _afdelingsJaarMgr;
        private readonly AdressenManager _adresMgr;
        private readonly GroepsWerkJaarManager _groepsWerkJaarManager;
        private readonly JaarOvergangManager _jaarOvergangManager;
        private readonly IAutorisatieManager _autorisatieMgr;
        private readonly CategorieenManager _categorieenMgr;
        private readonly FunctiesManager _functiesMgr;
        private readonly LedenManager _ledenMgr;
        private readonly GebruikersRechtenManager _gebruikersRechtenManager;

        /// <summary>
        /// Constructor met via IoC toegekende workers
        /// </summary>
        /// <param name="groepenMgr">
        /// De worker voor Groepen
        /// </param>
        /// <param name="cgm">
        /// De worker voor Chirogroepen
        /// </param>
        /// <param name="ajm">
        /// De worker voor AfdelingsJaren
        /// </param>
        /// <param name="wm">
        /// De worker voor GroepsWerkJaren
        /// </param>
        /// <param name="adresMgr">
        /// De worker voor Adressen
        /// </param>
        /// <param name="cm">
        /// De worker voor Categorieën
        /// </param>
        /// <param name="fm">
        /// De worker voor Functies
        /// </param>
        /// <param name="lm">
        /// De worker voor Leden
        /// </param>
        /// <param name="am">
        /// De worker voor Autorisatie
        /// </param>
        /// <param name="jm">
        /// De worker voor Jaarovergang
        /// </param>
        /// <param name="gebruikersRechtenMgr">
        /// Worker ivm gebruikersrechten
        /// </param>
        public GroepenService(
            GroepenManager groepenMgr,
            ChiroGroepenManager cgm,
            AfdelingsJaarManager ajm,
            GroepsWerkJaarManager wm,
            AdressenManager adresMgr,
            CategorieenManager cm,
            FunctiesManager fm,
            LedenManager lm,
            IAutorisatieManager am,
            JaarOvergangManager jm,
            GebruikersRechtenManager gebruikersRechtenMgr)
        {
            _groepenMgr = groepenMgr;
            _chiroGroepenMgr = cgm;
            _afdelingsJaarMgr = ajm;
            _groepsWerkJaarManager = wm;
            _autorisatieMgr = am;
            _adresMgr = adresMgr;
            _categorieenMgr = cm;
            _functiesMgr = fm;
            _ledenMgr = lm;
            _jaarOvergangManager = jm;
            _gebruikersRechtenManager = gebruikersRechtenMgr;
        }

        #endregion

        #region algemene members

        /// <summary>
        /// Ophalen van Groepsinformatie
        /// </summary>
        /// <param name="groepID">GroepID van groep waarvan we de informatie willen opvragen</param>
        /// <returns>
        /// De gevraagde informatie over de groep met id <paramref name="groepID"/>
        /// </returns>
        public GroepInfo InfoOphalen(int groepID)
        {
            try
            {
                var g = _groepenMgr.Ophalen(groepID);
                return Mapper.Map<Groep, GroepInfo>(g);
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        /// <summary>
        /// Haalt info op, uitgaande van code (stamnummer)
        /// </summary>
        /// <param name="code">Stamnummer van de groep waarvoor info opgehaald moet worden</param>
        /// <returns>Groepsinformatie voor groep met code <paramref name="code"/></returns>
        public GroepInfo InfoOphalenCode(string code)
        {
            try
            {
                var g = _groepenMgr.Ophalen(code);
                return Mapper.Map<Groep, GroepInfo>(g);
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        /// <summary>
        /// Ophalen van gedetailleerde informatie over de groep met ID <paramref name="groepID"/>
        /// </summary>
        /// <param name="groepID">ID van de groep waarvoor de informatie opgehaald moet worden</param>
        /// <returns>Groepsdetails, inclusief categorieen en huidige actieve afdelingen</returns>
        public GroepDetail DetailOphalen(int groepID)
        {
            try
            {
                var resultaat = new GroepDetail();

                var g = _groepenMgr.Ophalen(groepID, GroepsExtras.Categorieen | GroepsExtras.Functies);
                Mapper.Map(g, resultaat);

                resultaat.Afdelingen = Mapper.Map<IEnumerable<AfdelingsJaar>, List<AfdelingDetail>>(
                    _groepsWerkJaarManager.RecentsteOphalen(groepID, GroepsWerkJaarExtras.Afdelingen).AfdelingsJaar);

                return resultaat;
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        /// <summary>
        /// Persisteert een groep in de database
        /// </summary>
        /// <param name="g">Te persisteren groep</param>
        public void Bewaren(GroepInfo g)
        {
            // FIXME (#1138): gedetailleerde exception
            try
            {
                var groep = _groepenMgr.Ophalen(g.ID);

                // Ik gebruik hier geen mapper, omdat de entity Groep (en eender welke entity in het algemeen)
                // heel veel members heeft.  Omdat Automapper.AssertConfigurationIsValid wil gebruiken, zou je
                // dan al die members moeten ignoren.

                groep.ID = g.ID;
                groep.Naam = g.Naam;
                groep.Code = g.StamNummer;

                // TODO: Hier gaat natuurlijk nooit een concurrency exception optreden,
                // aangezien GroepInfo (nog?) geen versiestring bevat.

                _groepenMgr.Bewaren(groep);
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
            }
        }

        /// <summary>
        /// Haalt GroepsWerkJaarID van het recentst gemaakte groepswerkjaar
        /// voor een gegeven groep op.
        /// </summary>
        /// <param name="groepID">GroepID van groep</param>
        /// <returns>ID van het recentste GroepsWerkJaar</returns>
        public int RecentsteGroepsWerkJaarIDGet(int groepID)
        {
            try
            {
                return _groepsWerkJaarManager.RecentsteGroepsWerkJaarIDGet(groepID);
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return 0;
            }
        }

        /// <summary>
        /// Haalt de details op over het recentste groepswerkjaar van de groep met ID
        /// <paramref name="groepID"/>
        /// </summary>
        /// <param name="groepID">ID van de groep waarvan de werkjaardetails gevraagd zijn</param>
        /// <returns>De details op over het recentste groepswerkjaar van de groep met ID
        /// <paramref name="groepID"/></returns>
        public GroepsWerkJaarDetail RecentsteGroepsWerkJaarOphalen(int groepID)
        {
            GroepsWerkJaar gwj;

            try
            {
                gwj = _groepsWerkJaarManager.RecentsteOphalen(groepID, GroepsWerkJaarExtras.Groep);
            }
            catch (GeenGavException ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }

            var result = Mapper.Map<GroepsWerkJaar, GroepsWerkJaarDetail>(gwj);

            result.Status = (DateTime.Now >= _groepsWerkJaarManager.StartOvergang(gwj.WerkJaar)
                                ? WerkJaarStatus.InOvergang
                                : WerkJaarStatus.Bezig);

            return result;
        }

        #endregion

        #region ophalen

        /// <summary>
        /// Haalt details over alle officiele afdelingen op.
        /// </summary>
        /// <param name="groepID">ID van een groep, zodat aan de hand van het recenste groepswerkjaar
        /// de standaardgeboortejaren van en tot bepaald kunnen worden</param>
        /// <returns>Rij met details over de officiele afdelingen</returns>
        public IEnumerable<OfficieleAfdelingDetail> OfficieleAfdelingenOphalen(int groepID)
        {
            try
            {
                return Mapper.Map<IEnumerable<OfficieleAfdeling>, IEnumerable<OfficieleAfdelingDetail>>(
                    _afdelingsJaarMgr.OfficieleAfdelingenOphalen().OrderBy(e => e.LeefTijdVan));
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        /// <summary>
        /// Haalt de groepen op waarvoor de gebruiker (GAV-)rechten heeft
        /// </summary>
        /// <returns>De (informatie over de) groepen van de gebruiker</returns>
        public IEnumerable<GroepInfo> MijnGroepenOphalen()
        {
            try
            {
                var result = _autorisatieMgr.MijnGroepenOphalen();
                return Mapper.Map<IEnumerable<Groep>, IEnumerable<GroepInfo>>(result);
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        #endregion

        #region afdelingen

        // Bedoeling van het afdelingsgedeelte:
        // er zijn een aantal officiële afdelingen, die een range van leeftijden hebben. Blijven dat altijd dezelfde?
        // Elke Chirogroep heeft elk werkjaar haar eigen afdelingen, die ook een range van leeftijden hebben.
        // 
        // Elke afdeling moet overeenkomen met een officiële afdeling.
        // Er is niet gespecifieerd of het mogelijk is om een eerste-jaar-rakkers en een tweede-jaar-rakkers te hebben
        // 
        // Omdat bovenstaande niet echt duidelijk is en misschien niet altijd voldoende:
        // waarom moet er een mapping zijn met een officiële afdeling? Als dit echt moet, dan is het bovenstaande niet duidelijk,
        // en stel ik het onderstaande voor
        // 
        // Elke afdeling heeft een naam, een afkorting en een boolean NOGINGEBRUIK?
        // Elk afdelingsjaar heeft een afdeling en een interval van leeftijden.
        // Voor elke leeftijd is er een mapping met een officiële afdeling
        // elke leeftijd kan maar op 1 officiële afdeling gemapt worden
        // 
        // Voorbeelden:
        // "de kleintjes" = {minis, speelclub}
        // "de 5de jaars" = {eerste jaar rakkers}
        // "rakwi's" = {tweede jaar speelclub, rakkers}

        /// <summary>
        /// Maakt een nieuwe afdeling voor een gegeven Chirogroep
        /// </summary>
        /// <param name="groepID">ID van de Chirogroep</param>
        /// <param name="naam">Naam van de afdeling</param>
        /// <param name="afkorting">Afkorting van de afdeling (voor lijsten, overzichten,...)</param>
        public void AfdelingAanmaken(int groepID, string naam, string afkorting)
        {
            ChiroGroep g = _chiroGroepenMgr.Ophalen(groepID, ChiroGroepsExtras.AlleAfdelingen);
            try
            {
                _chiroGroepenMgr.AfdelingToevoegen(g, naam, afkorting);
                _chiroGroepenMgr.Bewaren(g, ChiroGroepsExtras.AlleAfdelingen);
            }
            catch (BestaatAlException<Afdeling> ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
            }

            // Bij een onverwacht exception mag de toepassing gerust crashen.
        }

        /// <summary>
        /// Bewaart een afdeling met de nieuwe informatie.
        /// </summary>
        /// <param name="info">De afdelingsinfo die opgeslagen moet worden</param>
        public void AfdelingBewaren(AfdelingInfo info)
        {
            try
            {
                Afdeling ai = _afdelingsJaarMgr.AfdelingOphalen(info.ID);
                if (info.Naam != null && info.Naam.CompareTo(ai.Naam) != 0)
                {
                    ai.Naam = info.Naam;
                }
                if (info.Afkorting != null && info.Afkorting.CompareTo(ai.Afkorting) != 0)
                {
                    ai.Afkorting = info.Afkorting;
                }

                _afdelingsJaarMgr.Bewaren(ai);
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
            }
        }

        /// <summary>
        /// Maakt/bewerkt een AfdelingsJaar: 
        /// andere OfficieleAfdeling en/of andere leeftijden
        /// </summary>
        /// <param name="detail">AfdelingsJaarDetail met de gegevens over het aan te maken of te wijzigen
        /// afdelingsjaar.  <c>detail.AfdelingsJaarID</c> bepaalt of het om een bestaand afdelingsjaar gaat
        /// (ID > 0), of een bestaande (ID == 0)</param>
        public void AfdelingsJaarBewaren(AfdelingsJaarDetail detail)
        {
            try
            {
                AfdelingsJaar afdelingsJaar;

                Afdeling afd = _afdelingsJaarMgr.AfdelingOphalen(detail.AfdelingID);
                OfficieleAfdeling oa = _afdelingsJaarMgr.OfficieleAfdelingOphalen(detail.OfficieleAfdelingID);
                GroepsWerkJaar huidigGwj = _groepsWerkJaarManager.RecentsteOphalen(afd.ChiroGroep.ID);

                if (detail.AfdelingsJaarID == 0)
                {
                    // nieuw maken.
                    // OPM: als dit foutloopt, moet de juiste foutmelding doorgegeven worden (zie #553)
                    afdelingsJaar = _afdelingsJaarMgr.Aanmaken(
                                        afd,
                                        oa,
                                        huidigGwj,
                                        detail.GeboorteJaarVan,
                                        detail.GeboorteJaarTot,
                                        detail.Geslacht);
                }
                else
                {
                    // wijzigen

                    afdelingsJaar = _afdelingsJaarMgr.Ophalen(
                        detail.AfdelingsJaarID,
                        AfdelingsJaarExtras.OfficieleAfdeling | AfdelingsJaarExtras.Afdeling | AfdelingsJaarExtras.GroepsWerkJaar);

                    if (afdelingsJaar.GroepsWerkJaar.ID != huidigGwj.ID || afdelingsJaar.Afdeling.ID != detail.AfdelingID)
                    {
                        throw new NotSupportedException("Afdeling en Groepswerkjaar mogen niet gewijzigd worden.");
                    }

                    _afdelingsJaarMgr.Wijzigen(
                        afdelingsJaar,
                        _afdelingsJaarMgr.OfficieleAfdelingOphalen(detail.OfficieleAfdelingID),
                        detail.GeboorteJaarVan,
                        detail.GeboorteJaarTot,
                        detail.Geslacht,
                        detail.VersieString);
                }

                _afdelingsJaarMgr.Bewaren(afdelingsJaar);
            }
            catch (ValidatieException ex)
            {
                throw new FaultException<FoutNummerFault>(new FoutNummerFault { FoutNummer = ex.FoutNummer, Bericht = ex.Message }, new FaultReason(ex.Message));
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
            }

            // TODO: Concurrency exception catchen
            // OPM: FoutAfhandelaar.FoutAfhandelen vangt OptimisticConcurrencyException op. Zijn er nog andere?
        }

        /// <summary>
        /// Verwijdert een afdelingsjaar
        /// en controleert of er geen leden in zitten.
        /// </summary>
        /// <param name="afdelingsJaarID">ID van het afdelingsjaar dat verwijderd moet worden</param>
        public void AfdelingsJaarVerwijderen(int afdelingsJaarID)
        {
            try
            {
                _afdelingsJaarMgr.AfdJarenVerwijderen(afdelingsJaarID);
            }
            catch (InvalidOperationException)
            {
                /*var afdjaar = _afdelingsJaarMgr.Ophalen(afdelingsJaarID, AfdelingsJaarExtras.Afdeling);
                var afdjaardetail = Mapper.Map<AfdelingsJaar, AfdelingsJaarDetail>(afdjaar);*/
                throw new FaultException<FoutNummerFault>(new FoutNummerFault { FoutNummer = FoutNummer.AfdelingNietLeeg });
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
            }
        }

        /// <summary>
        /// Verwijdert een afdelingsjaar
        /// en controleert of er geen leden in zitten.
        /// </summary>
        /// <param name="afdelingID">ID van de afdeling die verwijderd moet worden</param>
        public void AfdelingVerwijderen(int afdelingID)
        {
            try
            {
                _afdelingsJaarMgr.AfdVerwijderen(afdelingID);
            }
            catch (InvalidOperationException)
            {
                /*var afdjaar = _afdelingsJaarMgr.Ophalen(afdelingsJaarID, AfdelingsJaarExtras.Afdeling);
                var afdjaardetail = Mapper.Map<AfdelingsJaar, AfdelingsJaarDetail>(afdjaar);*/
                throw new FaultException<FoutNummerFault>(new FoutNummerFault { FoutNummer = FoutNummer.AfdelingNietLeeg });
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
            }
        }

        /// <summary>
        /// Gegevens ophalen van het afdelingsjaar met de opgegeven ID
        /// </summary>
        /// <param name="afdelingsJaarID">De ID van het afdelingsjaar dat we nodig hebben</param>
        /// <returns>De gegevens van het AfdelingsJaar</returns>
        public AfdelingsJaarDetail AfdelingsJaarOphalen(int afdelingsJaarID)
        {
            try
            {
                AfdelingsJaar aj = _afdelingsJaarMgr.Ophalen(
                afdelingsJaarID,
                AfdelingsJaarExtras.Afdeling | AfdelingsJaarExtras.OfficieleAfdeling);

                return Mapper.Map<AfdelingsJaar, AfdelingsJaarDetail>(aj);
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        /// <summary>
        /// Haalt details op van een afdeling, gebaseerd op het <paramref name="afdelingsJaarID"/>
        /// </summary>
        /// <param name="afdelingsJaarID">ID van het AFDELINGSJAAR waarvoor de details opgehaald moeten 
        /// worden.</param>
        /// <returns>De details van de afdeling in het gegeven afdelingsjaar.</returns>
        public AfdelingDetail AfdelingDetailOphalen(int afdelingsJaarID)
        {
            try
            {
                AfdelingsJaar aj = _afdelingsJaarMgr.Ophalen(
                afdelingsJaarID,
                AfdelingsJaarExtras.Afdeling | AfdelingsJaarExtras.OfficieleAfdeling);

                return Mapper.Map<AfdelingsJaar, AfdelingDetail>(aj);
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        /// <summary>
        /// Haalt een afdeling op, op basis van <paramref name="afdelingID"/>
        /// </summary>
        /// <param name="afdelingID">ID van op te halen afdeling</param>
        /// <returns>Info van de gevraagde afdeling</returns>
        public AfdelingInfo AfdelingOphalen(int afdelingID)
        {
            try
            {
                Afdeling a = _afdelingsJaarMgr.AfdelingOphalen(afdelingID);
                return Mapper.Map<Afdeling, AfdelingInfo>(a);
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        /// <summary>
        /// Haalt informatie op over alle actieve afdelingen in het groepswerkjaar met 
        /// ID <paramref name="groepsWerkJaarID"/>
        /// </summary>
        /// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
        /// <returns>
        /// Informatie over alle actieve afdelingen in het groepswerkjaar met 
        /// ID <paramref name="groepsWerkJaarID"/>
        /// </returns>
        public IList<AfdelingDetail> ActieveAfdelingenOphalen(int groepsWerkJaarID)
        {
            try
            {
                var groepswerkjaar = _groepsWerkJaarManager.Ophalen(groepsWerkJaarID, GroepsWerkJaarExtras.Afdelingen | GroepsWerkJaarExtras.Leden);
                return Mapper.Map<IList<AfdelingsJaar>, IList<AfdelingDetail>>(groepswerkjaar.AfdelingsJaar.OrderBy(e => e.GeboorteJaarVan).ToList());
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        /// <summary>
        /// Haalt beperkte informatie op over alle afdelingen die een groep ooit gebruikt heeft.
        /// </summary>
        /// <param name="groepID">ID van de Chirogroep waarvoor de afdelingen gevraagd zijn</param>
        /// <returns>Lijst van AfdelingInfo</returns>
        public IList<AfdelingInfo> AlleAfdelingenOphalen(int groepID)
        {
            try
            {
                var groep = _chiroGroepenMgr.Ophalen(groepID, ChiroGroepsExtras.AlleAfdelingen);
                return Mapper.Map<IEnumerable<Afdeling>, IList<AfdelingInfo>>(groep.Afdeling);
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        /// <summary>
        /// Haalt informatie op over de beschikbare afdelingsjaren en hun gelinkte afdelingen van een groep in het huidige
        /// groepswerkjaar.
        /// </summary>
        /// <param name="groepID">ID van de groep waarvoor de info gevraagd is</param>
        /// <returns>Lijst van AfdelingInfo</returns>
        public IList<ActieveAfdelingInfo> HuidigeAfdelingsJarenOphalen(int groepID)
        {
            try
            {
                var gwj = _groepsWerkJaarManager.RecentsteOphalen(groepID, GroepsWerkJaarExtras.Afdelingen);
                return Mapper.Map<IEnumerable<AfdelingsJaar>, IList<ActieveAfdelingInfo>>(gwj.AfdelingsJaar.OrderBy(aj => aj.GeboorteJaarVan));
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        /// <summary>
        /// Haalt informatie op over de afdelingen van een groep die niet gebruikt zijn in een gegeven 
        /// groepswerkjaar, op basis van een <paramref name="groepswerkjaarID"/>
        /// </summary>
        /// <param name="groepswerkjaarID">ID van het groepswerkjaar waarvoor de niet-gebruikte afdelingen
        /// opgezocht moeten worden.</param>
        /// <returns>Info over de ongebruikte afdelingen van een groep in het gegeven groepswerkjaar</returns>
        public IList<AfdelingInfo> OngebruikteAfdelingenOphalen(int groepswerkjaarID)
        {
            try
            {
                IList<Afdeling> ongebruikteAfdelingen = _groepsWerkJaarManager.OngebruikteAfdelingenOphalen(groepswerkjaarID);
                return Mapper.Map<IList<Afdeling>, IList<AfdelingInfo>>(ongebruikteAfdelingen);
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        #endregion

        #region Functies

        /// <summary>
        /// Haalt uit groepswerkjaar met ID <paramref name="groepsWerkJaarID"/> alle beschikbare functies
        /// op voor een lid van type <paramref name="lidType"/>.
        /// </summary>
        /// <param name="groepsWerkJaarID">ID van het groepswerkjaar van de gevraagde functies</param>
        /// <param name="lidType"><c>LidType.Kind</c> of <c>LidType.Leiding</c></param>
        /// <returns>De gevraagde lijst afdelingsinfo</returns>
        public IEnumerable<FunctieDetail> FunctiesOphalen(int groepsWerkJaarID, LidType lidType)
        {
            try
            {
                var relevanteFuncties = _functiesMgr.OphalenRelevant(groepsWerkJaarID, lidType);
                return Mapper.Map<IList<Functie>, IList<FunctieDetail>>(relevanteFuncties);
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        /// <summary>
        /// Zoekt naar problemen ivm de maximum- en minimumaantallen van functies voor het
        /// huidige werkjaar.
        /// </summary>
        /// <param name="groepID">ID van de groep waarvoor de functies gecontroleerd moeten worden.</param>
        /// <returns>
        /// Als er problemen zijn, wordt een rij FunctieProbleemInfo opgeleverd.
        /// </returns>
        public IEnumerable<FunctieProbleemInfo> FunctiesControleren(int groepID)
        {
            try
            {
                GroepsWerkJaar gwj = _groepsWerkJaarManager.RecentsteOphalen(
                groepID, GroepsWerkJaarExtras.GroepsFuncties | GroepsWerkJaarExtras.LidFuncties);

                IEnumerable<Telling> problemen = _functiesMgr.AantallenControleren(gwj);

                // Blijkbaar kan ik hier niet anders dan de functies weer ophalen.

                var resultaat = (from p in problemen
                                 let f = _functiesMgr.Ophalen(p.ID)
                                 select new FunctieProbleemInfo
                                 {
                                     Code = f.Code,
                                     EffectiefAantal = p.Aantal,
                                     ID = f.ID,
                                     MaxAantal = p.Max,
                                     MinAantal = p.Min,
                                     Naam = f.Naam
                                 }).ToList();

                // Ter info: return resultaat.ToArray() werkt niet; problemen met (de)serializeren?

                return resultaat.ToList();
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        /// <summary>
        /// Controleert de verplicht in te vullen lidgegevens.
        /// </summary>
        /// <param name="groepID">ID van de groep waarvan de leden te controleren zijn</param>
        /// <returns>Een rij LedenProbleemInfo.  Leeg bij gebrek aan problemen.</returns>
        public IEnumerable<LedenProbleemInfo> LedenControleren(int groepID)
        {
            var resultaat = new List<LedenProbleemInfo>();
            int gwjid;

            try
            {
                gwjid = _groepsWerkJaarManager.RecentsteGroepsWerkJaarIDGet(groepID);
            }
            catch (GeenGavException ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }

            int ledenZonderAdres = _ledenMgr.Zoeken(new LidFilter
                                                        {
                                                            GroepsWerkJaarID = gwjid,
                                                            GroepID = groepID,
                                                            HeeftVoorkeurAdres = false,
                                                            LidType = LidType.Alles
                                                        },
                                                    LidExtras.Geen).Count();

            int ledenZonderTelefoonNummer = _ledenMgr.Zoeken(new LidFilter
                                    {
                                        GroepsWerkJaarID = gwjid,
                                        GroepID = groepID,
                                        HeeftTelefoonNummer = false,
                                        LidType = LidType.Alles
                                    },
                                LidExtras.Geen).Count();

            int leidingZonderEmail = _ledenMgr.Zoeken(new LidFilter
                                                        {
                                                            GroepsWerkJaarID = gwjid,
                                                            GroepID = groepID,
                                                            HeeftEmailAdres = false,
                                                            LidType = LidType.Leiding
                                                        },
                                                      LidExtras.Geen).Count();

            if (ledenZonderAdres > 0)
            {
                resultaat.Add(new LedenProbleemInfo { Probleem = LidProbleem.AdresOntbreekt, Aantal = ledenZonderAdres });
            }

            if (ledenZonderTelefoonNummer > 0)
            {
                resultaat.Add(new LedenProbleemInfo { Probleem = LidProbleem.TelefoonNummerOntbreekt, Aantal = ledenZonderTelefoonNummer });
            }

            if (leidingZonderEmail > 0)
            {
                resultaat.Add(new LedenProbleemInfo { Probleem = LidProbleem.EmailOntbreekt, Aantal = leidingZonderEmail });
            }

            return resultaat;
        }

        /// <summary>
        /// Maakt een nieuwe Functie voor de groep met ID <paramref name="groepID"/>
        /// </summary>
        /// <param name="groepID">ID van de groep waarvoor nieuwe functie wordt gemaakt</param>
        /// <param name="naam">Naam voor de nieuwe functie</param>
        /// <param name="code">Code voor de nieuwe functie</param>
        /// <param name="maxAantal">Eventueel het maximumaantal leden met die functie in een werkjaar</param>
        /// <param name="minAantal">Het minimumaantal leden met die functie in een werkjaar</param>
        /// <param name="lidType">Gaat het over een functie voor leden, leiding of beide?</param>
        /// <param name="werkJaarVan">Eventueel het vroegste werkjaar waarvoor de functie beschikbaar moet zijn</param>
        /// <returns>De ID van de aangemaakte Functie</returns>
        public int FunctieToevoegen(int groepID, string naam, string code, int? maxAantal, int minAantal, LidType lidType, int? werkJaarVan)
        {
            try
            {
                GroepsWerkJaar gwj = _groepsWerkJaarManager.RecentsteOphalen(groepID,
                                                                             GroepsWerkJaarExtras.GroepsFuncties);
                Groep g = gwj.Groep;

                _groepenMgr.FunctieToevoegen(g, naam, code, maxAantal, minAantal, lidType);
                g = _groepenMgr.Bewaren(g, e => e.Functie);

                return (from fun in g.Functie
                        where fun.Code == code
                        select fun.ID).FirstOrDefault();
            }
            catch (BestaatAlException<Functie> ex)
            {
                var fault = Mapper.Map<BestaatAlException<Functie>,
                        BestaatAlFault<FunctieDetail>>(ex);

                throw new FaultException<BestaatAlFault<FunctieDetail>>(fault);
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return 0;
            }
        }

        /// <summary>
        /// Verwijdert de functie met gegeven <paramref name="functieID"/>
        /// </summary>
        /// <param name="functieID">ID van de te verwijderen functie</param>
        /// <param name="forceren">Indien <c>true</c>, worden eventuele personen uit de
        /// te verwijderen functie eerst uit de functie weggehaald.  Indien
        /// <c>false</c> krijg je een exception als de functie niet leeg is.</param>
        public void FunctieVerwijderen(int functieID, bool forceren)
        {
            try
            {
                // Leden, groepswerkjaar en groep mee ophalen, anders werkt 
                // FunctiesManager.Verwijderen niet.
                Functie f = _functiesMgr.Ophalen(functieID, true);

                _functiesMgr.Verwijderen(f, forceren);
            }
            catch (BlokkerendeObjectenException<Lid> ex)
            {
                var fault = Mapper.Map<BlokkerendeObjectenException<Lid>,
                    BlokkerendeObjectenFault<PersoonLidInfo>>(ex);

                throw new FaultException<BlokkerendeObjectenFault<PersoonLidInfo>>(fault);
            }
            catch (GeenGavException ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
            }

            // onverwachte exception mag gewoon gethrowd worden.
        }

        #endregion

        /// <summary>
        /// Haalt de werkjaren op waarin de groep aangesloten was (te beginnen met het werkjaar voor
        /// deze applicatie in gebruik genomen werd)
        /// </summary>
        /// <param name="groepID">De ID van de Groep die we willen bekijken</param>
        /// <returns>Een lijstje van werkjaren</returns>
        public IEnumerable<WerkJaarInfo> WerkJarenOphalen(int groepID)
        {
            try
            {
                var werkjaren = (from gwj in _groepenMgr.Ophalen(groepID, GroepsExtras.GroepsWerkJaren).GroepsWerkJaar
                                 orderby gwj.WerkJaar descending
                                 select gwj);

                return Mapper.Map<IEnumerable<GroepsWerkJaar>, IEnumerable<WerkJaarInfo>>(werkjaren);
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        #region categorieën

        /// <summary>
        /// Maakt een nieuwe categorie voor de groep met ID <paramref name="groepID"/>
        /// </summary>
        /// <param name="groepID">ID van de groep waarvoor nieuwe categorie wordt gemaakt</param>
        /// <param name="naam">Naam voor de nieuwe categorie</param>
        /// <param name="code">Code voor de nieuwe categorie</param>
        /// <returns>De ID van de aangemaakte categorie</returns>
        public int CategorieToevoegen(int groepID, string naam, string code)
        {
            try
            {
                Groep g = _groepenMgr.Ophalen(groepID, GroepsExtras.Categorieen);

                _groepenMgr.CategorieToevoegen(g, naam, code);

                g = _groepenMgr.Bewaren(g, e => e.Categorie);

                return (from ctg in g.Categorie
                        where ctg.Code == code
                        select ctg.ID).FirstOrDefault();
            }
            catch (BestaatAlException<Categorie> ex)
            {
                var fault = Mapper.Map<BestaatAlException<Categorie>,
                        BestaatAlFault<CategorieInfo>>(ex);

                throw new FaultException<BestaatAlFault<CategorieInfo>>(fault);
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return 0;
            }
        }

        /// <summary>
        /// Verwijdert de categorie met gegeven <paramref name="categorieID"/>
        /// </summary>
        /// <param name="categorieID">ID van de te verwijderen categorie</param>
        /// <param name="forceren">Indien <c>true</c>, worden eventuele personen uit de
        /// te verwijderen categorie eerst uit de categorie weggehaald.  Indien
        /// <c>false</c> krijg je een exception als de categorie niet leeg is.</param>
        public void CategorieVerwijderen(int categorieID, bool forceren)
        {
            // Personen moeten mee opgehaald worden; anders werkt 
            // CategorieenManager.Verwijderen niet.
            try
            {
                Categorie c = _categorieenMgr.Ophalen(categorieID, true);
                _categorieenMgr.Verwijderen(c, forceren);
            }
            catch (BlokkerendeObjectenException<GelieerdePersoon> ex)
            {
                var fault = Mapper.Map<BlokkerendeObjectenException<GelieerdePersoon>,
                    BlokkerendeObjectenFault<PersoonDetail>>(ex);

                throw new FaultException<BlokkerendeObjectenFault<PersoonDetail>>(fault);
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
            }
        }

        /// <summary>
        /// Past de naam van een categorie aan
        /// </summary>
        /// <param name="categorieID">De ID van de categorie waar het over gaat</param>
        /// <param name="nieuwenaam">De nieuwe naam die de categorie moet krijgen</param>
        public void CategorieAanpassen(int categorieID, string nieuwenaam)
        {
            /*Groep g = OphalenMetCategorieen(groepID);
            Categorie c = null;*/
            throw new NotImplementedException();
        }

        /// <summary>
        /// Zoekt de categorieID op van de categorie bepaald door de gegeven 
        /// <paramref name="groepID"/> en <paramref name="code"/>.
        /// </summary>
        /// <param name="groepID">ID van groep waaraan de gezochte categorie gekoppeld is</param>
        /// <param name="code">Code van de te zoeken categorie</param>
        /// <returns>Het categorieID als de categorie gevonden is, anders 0.</returns>
        public int CategorieIDOphalen(int groepID, string code)
        {
            try
            {
                Categorie cat = _categorieenMgr.Ophalen(groepID, code);
                return (cat == null) ? 0 : cat.ID;
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return 0;
            }
        }

        /// <summary>
        /// Zoekt een categorie op, op basis van <paramref name="groepID"/> en
        /// <paramref name="categorieCode"/>
        /// </summary>
        /// <param name="groepID">ID van de groep waaraan de categorie gekoppeld moet zijn.</param>
        /// <param name="categorieCode">Code van de categorie</param>
        /// <returns>De categorie met code <paramref name="categorieCode"/> die van toepassing is op
        /// de groep met ID <paramref name="groepID"/>.</returns>
        public CategorieInfo CategorieOpzoeken(int groepID, string categorieCode)
        {
            try
            {
                Mapper.CreateMap<Categorie, CategorieInfo>();
                return Mapper.Map<Categorie, CategorieInfo>(_categorieenMgr.Ophalen(groepID, categorieCode));
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        /// <summary>
        /// Haalt alle categorieeen op van de groep met ID <paramref name="groepID"/>
        /// </summary>
        /// <param name="groepID">ID van de groep waarvan de categorieen zijn gevraagd</param>
        /// <returns>Lijst met categorie-info van de categorieen van de gevraagde groep</returns>
        public IList<CategorieInfo> CategorieenOphalen(int groepID)
        {
            try
            {
                var result = _categorieenMgr.AllesOphalen(groepID);
                return Mapper.Map<IList<Categorie>, IList<CategorieInfo>>(result);
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        #endregion categorieën

        #region adressen

        /// <summary>
        /// Maakt een lijst met alle deelgemeentes uit de database; nuttig voor autocompletion
        /// in de ui.
        /// </summary>
        /// <returns>Lijst met alle beschikbare deelgemeentes</returns>
        public IEnumerable<WoonPlaatsInfo> GemeentesOphalen()
        {
            try
            {
                return Mapper.Map<IEnumerable<WoonPlaats>, IList<WoonPlaatsInfo>>(_adresMgr.GemeentesOphalen());
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        /// <summary>
        /// Maakt een lijst met alle landen uit de database
        /// </summary>
        /// <returns>Lijst met alle beschikbare landen</returns>
        public IEnumerable<LandInfo> LandenOphalen()
        {
            return Mapper.Map<IEnumerable<Land>, IList<LandInfo>>(_adresMgr.LandenOphalen());
        }

        /// <summary>
        /// Haalt alle straten op uit een gegeven <paramref name="postNr"/>, waarvan de naam begint
        /// met het gegeven <paramref name="straatBegin"/>.
        /// </summary>
        /// <param name="straatBegin">Eerste letters van de te zoeken straatnamen</param>
        /// <param name="postNr">Postnummer waarin te zoeken</param>
        /// <returns>Gegevens van de gevonden straten</returns>
        public IEnumerable<StraatInfo> StratenOphalen(String straatBegin, int postNr)
        {
            try
            {
                return Mapper.Map<IList<StraatNaam>, IList<StraatInfo>>(_adresMgr.StratenOphalen(straatBegin, postNr));
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        /// <summary>
        /// Haalt alle straten op uit een gegeven rij <paramref name="postNrs"/>, waarvan de naam begint
        /// met het gegeven <paramref name="straatBegin"/>.
        /// </summary>
        /// <param name="straatBegin">Eerste letters van de te zoeken straatnamen</param>
        /// <param name="postNrs">Postnummers waarin te zoeken</param>
        /// <returns>Gegevens van de gevonden straten</returns>
        /// <remarks>Ik had deze functie ook graag StratenOphalen genoemd, maar je mag geen 2 
        /// WCF-functies met dezelfde naam in 1 service hebben.  Spijtig.</remarks>
        public IEnumerable<StraatInfo> StratenOphalenMeerderePostNrs(String straatBegin, IEnumerable<int> postNrs)
        {
            try
            {
                return Mapper.Map<IList<StraatNaam>, IList<StraatInfo>>(_adresMgr.StratenOphalen(straatBegin, postNrs));
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        /// <summary>
        /// Deze method geeft gewoon de gebruikersnaam weer waaronder je de service aanroept.  Vooral om de
        /// authenticate te testen.
        /// </summary>
        /// <returns>Gebruikersnaam waarmee aangemeld</returns>
        public string WieBenIk()
        {
            try
            {
                return _autorisatieMgr.GebruikersNaamGet();
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        /// <summary>
        /// Deze method geeft weer of we op een liveomgeving werken (<c>true</c>) of niet (<c>false</c>)
        /// </summary>
        /// <returns><c>True</c> als we op een liveomgeving werken, <c>false</c> als we op een testomgeving werken</returns>
        public bool IsLive()
        {
            // We zoeken dit uit op basis van de connectionstring.

            string connectionString = ConfigurationManager.ConnectionStrings["ChiroGroepEntities"].ConnectionString.ToUpper();
            return connectionString.Contains(Properties.Settings.Default.LiveConnSubstring.ToUpper());
        }

        /// <summary>
        /// Haalt informatie over alle gebruikersrechten van de gegeven groep op.
        /// </summary>
        /// <param name="groepID">ID van de groep waarvan de gebruikersrechten op te vragen zijn</param>
        /// <returns>Lijstje met details van de gebruikersrechten</returns>
        public IEnumerable<GebruikersDetail> GebruikersOphalen(int groepID)
        {
            var rechten = _gebruikersRechtenManager.AllesOphalen(groepID);
            var resultaat = Mapper.Map<IEnumerable<GebruikersRecht>, GebruikersDetail[]>(rechten);
            return resultaat;
        }

        #endregion

        #region jaarovergang

        /// <summary>
        /// Eens de gebruiker alle informatie heeft ingegeven, wordt de gewenste afdelingsverdeling naar de server gestuurd.
        /// <para />
        /// Dit in de vorm van een lijst van afdelingsjaardetails, met volgende info:
        ///		AFDELINGID van de afdelingen die geactiveerd zullen worden
        ///		Geboortejaren voor elk van die afdelingen
        /// </summary>
        /// <param name="teActiveren">Lijst van de afdelingen die geactiveerd moeten worden in het nieuwe werkjaar</param>
        /// <param name="groepID">ID van de groep voor wie een nieuw groepswerkjaar aangemaakt moet worden</param>
        /// <remarks>Voor kadergroepen laat je teActiveren gewoon leeg.</remarks>
        public void JaarovergangUitvoeren(IEnumerable<AfdelingsJaarDetail> teActiveren, int groepID)
        {
            try
            {
                _jaarOvergangManager.JaarOvergangUitvoeren(teActiveren, groepID);
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
            }
        }

        /// <summary>
        /// Berekent wat het nieuwe werkjaar zal zijn als op dit moment de jaarovergang zou gebeuren.
        /// </summary>
        /// <returns>Een jaartal (bv. 2011 voor 2011-2012)</returns>
        public int NieuwWerkJaarOphalen()
        {
            return _groepsWerkJaarManager.NieuweWerkJaar();
        }

        #endregion
    }
}
