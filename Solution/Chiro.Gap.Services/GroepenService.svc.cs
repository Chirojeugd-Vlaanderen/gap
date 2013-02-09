// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.Services
{
    // OPM: als je de naam van de class "GroepenService" hier verandert, moet je ook de sectie "Services" in web.config aanpassen.

    // *BELANGRIJK*: Als het debuggen hier stopt owv een autorisatiefout, kijk dan na of de gebruiker waarmee
    // je aangemeld bent, op je lokale computer in de groep CgUsers zit.

    /// <summary>
    /// Service voor operaties op groepsniveau
    /// </summary>
    public class GroepenService : IGroepenService, IDisposable
    {
        /// <summary>
        /// _context is verantwoordelijk voor het tracken van de wijzigingen aan de
        /// entiteiten. Via _context.SaveChanges() kunnen wijzigingen gepersisteerd
        /// worden.
        /// 
        /// Context is IDisposable. De context wordt aangemaakt door de IOC-container,
        /// en gedisposed op het moment dat de service gedisposed wordt. Dit gebeurt
        /// na iedere call.
        /// </summary>
        private readonly IContext _context;

        // Repositories, verantwoordelijk voor data access.

        private readonly IRepository<Groep> _groepenRepo;
        private readonly IRepository<Categorie> _categorieenRepo;
        private readonly IRepository<Afdeling> _afdelingenRepo;
        private readonly IRepository<OfficieleAfdeling> _officieleAfdelingenRepo;
        private readonly IRepository<AfdelingsJaar> _afdelingsJaarRepo;
        private readonly IRepository<Functie> _functiesRepo;
        private readonly IRepository<GroepsWerkJaar> _groepsWerkJarenRepo;

        // Managers voor niet-triviale businesslogica

        private readonly IAfdelingsJaarManager _afdelingsJaarMgr;
        private readonly IAuthenticatieManager _authenticatieMgr;
        private readonly IAutorisatieManager _autorisatieMgr;
        private readonly IGroepenManager _groepenMgr;
        private readonly IChiroGroepenManager _chiroGroepenMgr;
        private readonly IGroepsWerkJarenManager _groepsWerkJarenMgr;
        private readonly IFunctiesManager _functiesMgr;
        private readonly IVeelGebruikt _veelGebruikt;

        /// <summary>
        /// Nieuwe groepenservice
        /// </summary>
        /// <param name="afdelingsJaarMgr">Verantwoordelijk voor authenticatie</param>
        /// <param name="authenticatieMgr">Verantwoordelijk voor authenticatie</param>
        /// <param name="autorisatieMgr">Verantwoordelijke voor autorisatie</param>
        /// <param name="groepenMgr">Businesslogica aangaande groepen</param>
        /// <param name="chiroGroepenMgr">Businesslogica aangaande chirogroepen</param>
        /// <param name="groepsWerkJarenMgr">Businesslogica wat betreft groepswerkjaren</param>
        /// <param name="functiesMgr">Businesslogica aangaande functies</param>
        /// <param name="veelGebruikt">Cache voor veelgebruikte zaken</param>
        /// <param name="repositoryProvider">De repository provider levert alle nodige repository's op.</param>
        public GroepenService(IAfdelingsJaarManager afdelingsJaarMgr, IAuthenticatieManager authenticatieMgr, IAutorisatieManager autorisatieMgr,
                              IGroepenManager groepenMgr, IChiroGroepenManager chiroGroepenMgr, IGroepsWerkJarenManager groepsWerkJarenMgr,
                              IFunctiesManager functiesMgr, IVeelGebruikt veelGebruikt,
                              IRepositoryProvider repositoryProvider)
        {
            _context = repositoryProvider.ContextGet();
            _categorieenRepo = repositoryProvider.RepositoryGet<Categorie>();
            _groepenRepo = repositoryProvider.RepositoryGet<Groep>();
            _afdelingsJaarRepo = repositoryProvider.RepositoryGet<AfdelingsJaar>();
            _afdelingenRepo = repositoryProvider.RepositoryGet<Afdeling>();
            _officieleAfdelingenRepo = repositoryProvider.RepositoryGet<OfficieleAfdeling>();
            _functiesRepo = repositoryProvider.RepositoryGet<Functie>();
            _groepsWerkJarenRepo = repositoryProvider.RepositoryGet<GroepsWerkJaar>();

            _groepenMgr = groepenMgr;
            _chiroGroepenMgr = chiroGroepenMgr;
            _groepsWerkJarenMgr = groepsWerkJarenMgr;
            _functiesMgr = functiesMgr;
            _afdelingsJaarMgr = afdelingsJaarMgr;
            _authenticatieMgr = authenticatieMgr;
            _autorisatieMgr = autorisatieMgr;
            _veelGebruikt = veelGebruikt;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        /// <summary>
        /// Ophalen van Groepsinformatie
        /// </summary>
        /// <param name="groepId">groepId van groep waarvan we de informatie willen opvragen</param>
        /// <returns>
        /// De gevraagde informatie over de groep met id <paramref name="groepId"/>
        /// </returns>
        public GroepInfo InfoOphalen(int groepId)
        {
            var groep = GetGroepEnCheckGav(groepId);
            return Mapper.Map<Groep, GroepInfo>(groep);
        }

        /// <summary>
        /// Haalt info op, uitgaande van code (stamnummer)
        /// </summary>
        /// <param name="code">Stamnummer van de groep waarvoor info opgehaald moet worden</param>
        /// <returns>Groepsinformatie voor groep met code <paramref name="code"/></returns>
        public GroepInfo InfoOphalenCode(string code)
        {
            var groep = GetGroepEnCheckGav(code);
            return Mapper.Map<Groep, GroepInfo>(groep);
        }

        void CheckGav(Afdeling g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g.ChiroGroep))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        void CheckGav(Functie g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g.Groep))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        void CheckGav(Categorie g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g.Groep))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        void CheckGav(AfdelingsJaar g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g.GroepsWerkJaar))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        void CheckGav(GroepsWerkJaar g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        void CheckGav(Groep g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        GroepsWerkJaar GetRecentsteGroepsWerkJaarEnCheckGav(int groepId)
        {
            var groepsWerkJaar = _groepsWerkJarenRepo.Select()
                .Where(gwj => gwj.Groep.ID == groepId)
                .OrderByDescending(gwj => gwj.WerkJaar)
                .First();
            CheckGav(groepsWerkJaar);
            return groepsWerkJaar;
        }

        /// <summary>
        /// Ophalen van gedetailleerde informatie over de groep met ID <paramref name="groepId"/>
        /// </summary>
        /// <param name="groepId">ID van de groep waarvoor de informatie opgehaald moet worden</param>
        /// <returns>Groepsdetails, inclusief categorieen en huidige actieve afdelingen</returns>
        public GroepDetail DetailOphalen(int groepId)
        {
            var groepsWerkJaar = GetRecentsteGroepsWerkJaarEnCheckGav(groepId);

            var resultaat = Mapper.Map<Groep, GroepDetail>(groepsWerkJaar.Groep);
            Mapper.Map(groepsWerkJaar.AfdelingsJaar, resultaat.Afdelingen);
            return resultaat;
        }

        /// <summary>
        /// Haalt de groepen op waarvoor de gebruiker (GAV-)rechten heeft
        /// </summary>
        /// <returns>De (informatie over de) groepen van de gebruiker</returns>
        public IEnumerable<GroepInfo> MijnGroepenOphalen()
        {
            var mijnLogin = _authenticatieMgr.GebruikersNaamGet();
            var groepen = from g in _groepenRepo.Select()
                          where g.GebruikersRecht.Any(gr => gr.Gav.Login == mijnLogin)
                          select g;

            return Mapper.Map<IEnumerable<Groep>, IEnumerable<GroepInfo>>(groepen);
        }

        /// <summary>
        /// Haalt informatie op over alle werkjaren waarin een groep actief was/is.
        /// </summary>
        /// <param name="groepId">ID van de groep</param>
        /// <returns>Info over alle werkjaren waarin een groep actief was/is.</returns>
        public IEnumerable<WerkJaarInfo> WerkJarenOphalen(int groepId)
        {
            var groepsWerkJaren = _groepsWerkJarenRepo.Select()
                                  .Where(gwj => gwj.Groep.ID == groepId)
                                  .OrderByDescending(gwj => gwj.WerkJaar).ToList();
            if (groepsWerkJaren.Count > 0)
            {
                CheckGav(groepsWerkJaren.First());
            }

            return Mapper.Map<IEnumerable<GroepsWerkJaar>, IEnumerable<WerkJaarInfo>>(groepsWerkJaren);
        }

        static bool Equal(string links, string rechts)
        {
            if (links == null || rechts == null)
            {
                return links == null && rechts == null;
            }
            return String.Compare(links, rechts, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Persisteert een groep in de database
        /// Momenteel ondersteunen we enkel het wijzigen van groepsnaam
        /// en stamnummer. (En dat stamnummer wijzigen, mag dan nog enkel
        /// als we super-gav zijn.)
        /// </summary>
        /// <param name="groepInfo">Te persisteren groep</param>
        public void Bewaren(GroepInfo groepInfo)
        {
            var groep = GetGroepEnCheckGav(groepInfo.ID);

            if (!Equal(groepInfo.StamNummer, groep.Code) && !_autorisatieMgr.IsSuperGav())
            {
                throw FaultExceptionHelper.GeenGav();
            }

            groep.Naam = groepInfo.Naam;
            groep.Code = groepInfo.StamNummer;

            _context.SaveChanges();
        }

        /// <summary>
        /// Haalt groepswerkjaarId van het recentst gemaakte groepswerkjaar
        /// voor een gegeven groep op.
        /// </summary>
        /// <param name="groepId">groepId van groep</param>
        /// <returns>ID van het recentste GroepsWerkJaar</returns>
        public int RecentsteGroepsWerkJaarIDGet(int groepId)
        {
            return GetRecentsteGroepsWerkJaarEnCheckGav(groepId).ID;
        }

        /// <summary>
        /// Haalt gedetailleerde gegevens op van het recentst gemaakte groepswerkjaar
        /// voor een gegeven groep op.
        /// </summary>
        /// <param name="groepId">groepId van groep</param>
        /// <returns>
        /// De details van het recentste groepswerkjaar
        /// </returns>
        public GroepsWerkJaarDetail RecentsteGroepsWerkJaarOphalen(int groepId)
        {
            var groepsWerkJaar = GetRecentsteGroepsWerkJaarEnCheckGav(groepId);

            var result = Mapper.Map<GroepsWerkJaar, GroepsWerkJaarDetail>(groepsWerkJaar);
            result.Status = _groepsWerkJarenMgr.OvergangMogelijk(DateTime.Now, result.WerkJaar)
                                ? WerkJaarStatus.InOvergang
                                : WerkJaarStatus.Bezig;

            return result;
        }

        /// <summary>
        /// Maakt een nieuwe afdeling voor een gegeven ChiroGroep
        /// </summary>
        /// <param name="chirogroepId">ID van de groep</param>
        /// <param name="naam">Naam van de afdeling</param>
        /// <param name="afkorting">Afkorting van de afdeling (voor lijsten, overzichten,...)</param>
        public void AfdelingAanmaken(int chirogroepId, string naam, string afkorting)
        {
            var g = GetGroepEnCheckGav(chirogroepId);
            if (!(g is ChiroGroep))
            {
                FaultExceptionHelper.GeenGav();
            }
            try
            {
                _chiroGroepenMgr.AfdelingToevoegen((ChiroGroep)g, naam, afkorting);
            }
            catch (BestaatAlException<Afdeling> ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
            }

            _context.SaveChanges();
        }

        /// <summary>
        /// Bewaart een afdeling met de nieuwe informatie.
        /// </summary>
        /// <param name="info">De afdelingsinfo die opgeslagen moet worden</param>
        public void AfdelingBewaren(AfdelingInfo info)
        {
            var ai = (from g in _afdelingenRepo.Select()
                      where g.ID == info.ID
                      select g).FirstOrDefault();
            CheckGav(ai);
            Debug.Assert(ai != null, "ai != null");
            if (!Equal(info.Naam, ai.Naam))
            {
                ai.Naam = info.Naam;
            }
            if (!Equal(info.Afkorting, ai.Afkorting))
            {
                ai.Afkorting = info.Afkorting;
            }
            _context.SaveChanges();
        }

        /// <summary>
        /// Uitgebreide info ophalen over het afdelingsjaar met de opgegeven ID
        /// </summary>
        /// <param name="afdelingsJaarId">De ID van het afdelingsjaar in kwestie</param>
        /// <returns>Uitgebreide info over het afdelingsjaar met de opgegeven ID</returns>
        public AfdelingsJaarDetail AfdelingsJaarOphalen(int afdelingsJaarId)
        {
            var afd = (from g in _afdelingsJaarRepo.Select()
                       where g.ID == afdelingsJaarId
                       select g).FirstOrDefault();
            CheckGav(afd);
            return Mapper.Map<AfdelingsJaar, AfdelingsJaarDetail>(afd);
        }

        /// <summary>
        /// Maakt/bewerkt een AfdelingsJaar: 
        /// andere OfficieleAfdeling en/of andere leeftijden
        /// </summary>
        /// <param name="detail">AfdelingsJaarDetail met de gegevens over het aan te maken of te wijzigen
        /// afdelingsjaar.  <c>aj.AfdelingsJaarID</c> bepaat of het om een bestaand afdelingsjaar gaat
        /// (ID > 0), of een bestaand (ID == 0)</param>
        public void AfdelingsJaarBewaren(AfdelingsJaarDetail detail)
        {
            var afdeling = (from g in _afdelingenRepo.Select()
                            where g.ID == detail.AfdelingID
                            select g).FirstOrDefault();
            CheckGav(afdeling);

            var officieleAfdeling = (from g in _officieleAfdelingenRepo.Select()
                                     where g.ID == detail.OfficieleAfdelingID
                                     select g).FirstOrDefault();

            Debug.Assert(afdeling != null, "afdeling != null");
            var huidigGwj = GetRecentsteGroepsWerkJaarEnCheckGav(afdeling.ChiroGroep.ID);

            try
            {
                if (detail.AfdelingsJaarID == 0)
                {
                    // nieuw maken.
                    // OPM: als dit foutloopt, moet de juiste foutmelding doorgegeven worden (zie #553)
                    var afdelingsJaar = _afdelingsJaarMgr.Aanmaken(
                                        afdeling,
                                        officieleAfdeling,
                                        huidigGwj,
                                        detail.GeboorteJaarVan,
                                        detail.GeboorteJaarTot,
                                        detail.Geslacht);
                    huidigGwj.AfdelingsJaar.Add(afdelingsJaar);
                }
                else
                {
                    // wijzigen
                    var afdelingsJaar = (from g in _afdelingsJaarRepo.Select()
                                         where g.ID == detail.AfdelingsJaarID
                                         select g).FirstOrDefault();
                    CheckGav(afdelingsJaar);

                    Debug.Assert(afdelingsJaar != null, "afdelingsJaar != null");
                    if (afdelingsJaar.GroepsWerkJaar.ID != huidigGwj.ID || afdelingsJaar.Afdeling.ID != detail.AfdelingID)
                    {
                        throw new NotSupportedException("Afdeling en Groepswerkjaar mogen niet gewijzigd worden.");
                    }

                    afdelingsJaar.OfficieleAfdeling = officieleAfdeling;
                    afdelingsJaar.GeboorteJaarVan = detail.GeboorteJaarVan;
                    afdelingsJaar.GeboorteJaarTot = detail.GeboorteJaarTot;
                    afdelingsJaar.Geslacht = detail.Geslacht;
                    afdelingsJaar.VersieString = detail.VersieString;
                }
            }
            catch (ValidatieException ex)
            {
                throw FaultExceptionHelper.FoutNummer(ex.FoutNummer, ex.Message);
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
            }

            _context.SaveChanges();
        }

        /// <summary>
        /// Verwijdert een afdelingsjaar 
        /// en controleert of er geen leden in zitten.
        /// </summary>
        /// <param name="afdelingsJaarId">ID van het afdelingsjaar waarover het gaat</param>
        public void AfdelingsJaarVerwijderen(int afdelingsJaarId)
        {
            var afdelingsJaar = (from g in _afdelingsJaarRepo.Select()
                                 where g.ID == afdelingsJaarId
                                 select g).FirstOrDefault();
            CheckGav(afdelingsJaar);

            _afdelingsJaarRepo.Delete(afdelingsJaar);
            _context.SaveChanges();
        }

        /// <summary>
        /// Verwijdert een afdeling
        /// </summary>
        /// <param name="afdelingId">ID van de afdeling waarover het gaat</param>
        public void AfdelingVerwijderen(int afdelingId)
        {
            var afdeling = (from g in _afdelingenRepo.Select()
                            where g.ID == afdelingId
                            select g).FirstOrDefault();
            CheckGav(afdeling);

            _afdelingenRepo.Delete(afdeling);
            _context.SaveChanges();
        }

        /// <summary>
        /// Haalt details over alle officiele afdelingen op.
        /// </summary>
        /// <param name="groepId">ID van een groep, zodat aan de hand van het recenste groepswerkjaar
        /// de standaardgeboortejaren van en tot bepaald kunnen worden</param>
        /// <returns>Rij met details over de officiele afdelingen</returns>
        public IEnumerable<OfficieleAfdelingDetail> OfficieleAfdelingenOphalen(int groepId)
        {
            return Mapper.Map<IEnumerable<OfficieleAfdeling>, IEnumerable<OfficieleAfdelingDetail>>(GetRecentsteGroepsWerkJaarEnCheckGav(groepId).AfdelingsJaar.Select(e => e.OfficieleAfdeling));
        }

        /// <summary>
        /// Haat een afdeling op, op basis van <paramref name="afdelingId"/>
        /// </summary>
        /// <param name="afdelingId">ID van op te halen afdeling</param>
        /// <returns>Info van de gevraagde afdeling</returns>
        public AfdelingInfo AfdelingOphalen(int afdelingId)
        {
            var afdeling = (from g in _afdelingenRepo.Select()
                            where g.ID == afdelingId
                            select g).FirstOrDefault();
            CheckGav(afdeling);
            return Mapper.Map<Afdeling, AfdelingInfo>(afdeling);
        }

        /// <summary>
        /// Haalt details op van een afdeling, gebaseerd op het <paramref name="afdelingsJaarId"/>
        /// </summary>
        /// <param name="afdelingsJaarId">ID van het AFDELINGSJAAR waarvoor de details opgehaald moeten 
        /// worden.</param>
        /// <returns>De details van de afdeling in het gegeven afdelingsjaar.</returns>
        public AfdelingDetail AfdelingDetailOphalen(int afdelingsJaarId)
        {
            var afdelingsJaar = (from g in _afdelingsJaarRepo.Select()
                                 where g.ID == afdelingsJaarId
                                 select g).FirstOrDefault();
            CheckGav(afdelingsJaar);
            Debug.Assert(afdelingsJaar != null, "afdelingsJaar != null");
            return Mapper.Map<Afdeling, AfdelingDetail>(afdelingsJaar.Afdeling);
        }

        /// <summary>
        /// Haalt details op over alle actieve afdelingen in het groepswerkjaar met 
        /// ID <paramref name="groepswerkjaarId"/>
        /// </summary>
        /// <param name="groepswerkjaarId">ID van het groepswerkjaar</param>
        /// <returns>
        /// Informatie over alle actieve afdelingen in het groepswerkjaar met 
        /// ID <paramref name="groepswerkjaarId"/>
        /// </returns>
        public IList<AfdelingDetail> ActieveAfdelingenOphalen(int groepswerkjaarId)
        {
            var gwj = (from g in _groepsWerkJarenRepo.Select()
                       where g.ID == groepswerkjaarId
                       select g).FirstOrDefault();
            CheckGav(gwj);
            Debug.Assert(gwj != null, "gwj != null");
            return Mapper.Map<IEnumerable<Afdeling>, IList<AfdelingDetail>>(gwj.AfdelingsJaar.Select(e => e.Afdeling));
        }

        /// <summary>
        /// Haalt beperkte informatie op over de beschikbare afdelingen van een groep in het huidige
        /// groepswerkjaar.
        /// </summary>
        /// <param name="groepId">ID van de groep waarvoor de afdelingen gevraagd zijn</param>
        /// <returns>Lijst van ActieveAfdelingInfo</returns>
        public IList<AfdelingInfo> AlleAfdelingenOphalen(int groepId)
        {
            var gwj = GetRecentsteGroepsWerkJaarEnCheckGav(groepId);
            Debug.Assert(gwj != null, "gwj != null");
            return Mapper.Map<IEnumerable<Afdeling>, IList<AfdelingInfo>>(gwj.AfdelingsJaar.Select(e => e.Afdeling));
        }

        /// <summary>
        /// Haalt informatie op over de beschikbare afdelingsjaren en hun gelinkte afdelingen van een groep in het huidige
        /// groepswerkjaar.
        /// </summary>
        /// <param name="groepId">ID van de groep waarvoor de info gevraagd is</param>
        /// <returns>Lijst van AfdelingInfo</returns>
        public IList<ActieveAfdelingInfo> HuidigeAfdelingsJarenOphalen(int groepId)
        {
            var gwj = GetRecentsteGroepsWerkJaarEnCheckGav(groepId);
            Debug.Assert(gwj != null, "gwj != null");
            return Mapper.Map<IEnumerable<AfdelingsJaar>, IList<ActieveAfdelingInfo>>(gwj.AfdelingsJaar);
        }

        /// <summary>
        /// Haalt informatie op over de afdelingen van een groep die niet gebruikt zijn in een gegeven 
        /// groepswerkjaar, op basis van een <paramref name="groepswerkjaarId"/> (die dus geen afdelingsjaar hebben in het huidige werkjaar)
        /// </summary>
        /// <param name="groepswerkjaarId">ID van het groepswerkjaar waarvoor de niet-gebruikte afdelingen
        /// opgezocht moeten worden.</param>
        /// <returns>Info de ongebruikte afdelingen van een groep in het gegeven groepswerkjaar</returns>
        public IList<AfdelingInfo> OngebruikteAfdelingenOphalen(int groepswerkjaarId)
        {
            var gwj = (from g in _groepsWerkJarenRepo.Select()
                       where g.ID == groepswerkjaarId
                       select g).FirstOrDefault();
            CheckGav(gwj);
            Debug.Assert(gwj != null, "gwj != null");
            var ongebruikteAfdelingen = (from g in _afdelingenRepo.Select()
                                         where g.ChiroGroep.ID == gwj.Groep.ID && g.AfdelingsJaar.Count == 0
                                         select g);
            return Mapper.Map<IEnumerable<Afdeling>, IList<AfdelingInfo>>(ongebruikteAfdelingen);
        }

        /// <summary>
        /// Haalt uit groepswerkjaar met ID <paramref name="groepswerkjaarId"/> alle beschikbare functies
        /// op voor een lid van type <paramref name="lidType"/>.
        /// </summary>
        /// <param name="groepswerkjaarId">ID van het groepswerkjaar van de gevraagde functies</param>
        /// <param name="lidType"><c>LidType.Kind</c> of <c>LidType.Leiding</c></param>
        /// <returns>De gevraagde lijst afdelingsinfo</returns>
        public IEnumerable<FunctieDetail> FunctiesOphalen(int groepswerkjaarId, LidType lidType)
        {
            var gwj = (from g in _groepsWerkJarenRepo.Select()
                       where g.ID == groepswerkjaarId
                       select g).FirstOrDefault();
            CheckGav(gwj);
            var functies = (from g in _functiesRepo.Select()
                            where g.Type == lidType
                            select g);
            return Mapper.Map<IEnumerable<Functie>, IEnumerable<FunctieDetail>>(functies);
        }

        /// <summary>
        /// Zoekt naar problemen ivm de maximum- en minimumaantallen van functies voor het
        /// huidige werkJaar.
        /// </summary>
        /// <param name="groepId">ID van de groep waarvoor de functies gecontroleerd moeten worden.</param>
        /// <returns>
        /// Een rij FunctieProbleemInfo.  Als er geen problemen zijn, is deze leeg.
        /// </returns>
        public IEnumerable<FunctieProbleemInfo> FunctiesControleren(int groepId)
        {
            var groepsWerkJaar = GetRecentsteGroepsWerkJaarEnCheckGav(groepId);

            var problemen = _functiesMgr.AantallenControleren(groepsWerkJaar, groepsWerkJaar.Groep.Functie.Union(_veelGebruikt.NationaleFunctiesOphalen(_functiesRepo)));
            var resultaat = (from f in groepsWerkJaar.Groep.Functie
                             join p in problemen on f.ID equals p.ID
                             select new FunctieProbleemInfo
                                        {
                                            Code = f.Code,
                                            EffectiefAantal = p.Aantal,
                                            ID = f.ID,
                                            MaxAantal = p.Max,
                                            MinAantal = p.Min
                                        }).ToList();
            return resultaat;
        }

        /// <summary>
        /// Controleert de verplicht in te vullen lidgegevens.
        /// </summary>
        /// <param name="groepId">ID van de groep waarvan de leden te controleren zijn</param>
        /// <returns>Een rij LedenProbleemInfo.  Leeg bij gebrek aan problemen.</returns>
        public IEnumerable<LedenProbleemInfo> LedenControleren(int groepId)
        {
            var resultaat = new List<LedenProbleemInfo>();

            var groepsWerkJaar = GetRecentsteGroepsWerkJaarEnCheckGav(groepId);

            var aantalLedenZonderAdres = (from ld in groepsWerkJaar.Lid
                                          where ld.GelieerdePersoon.PersoonsAdres == null // geen voorkeursadres
                                          select ld).Count();

            if (aantalLedenZonderAdres > 0)
            {
                resultaat.Add(new LedenProbleemInfo
                {
                    Probleem = LidProbleem.AdresOntbreekt,
                    Aantal = aantalLedenZonderAdres
                });
            }

            var aantalLedenZonderTelefoonNr = (from ld in groepsWerkJaar.Lid
                                               where
                                                   ld.GelieerdePersoon.Communicatie.All(cmm => cmm.CommunicatieType.ID != (int)CommunicatieTypeEnum.TelefoonNummer)
                                               select ld).Count();

            if (aantalLedenZonderTelefoonNr > 0)
            {
                resultaat.Add(new LedenProbleemInfo
                {
                    Probleem = LidProbleem.TelefoonNummerOntbreekt,
                    Aantal = aantalLedenZonderTelefoonNr
                });
            }

            var aantalLeidingZonderEmail = (from ld in groepsWerkJaar.Lid
                                            where ld.Type == LidType.Leiding &&
                                                ld.GelieerdePersoon.Communicatie.All(cmm => cmm.CommunicatieType.ID != (int)CommunicatieTypeEnum.Email)
                                            select ld).Count();

            if (aantalLeidingZonderEmail > 0)
            {
                resultaat.Add(new LedenProbleemInfo
                                  {
                                      Probleem = LidProbleem.EmailOntbreekt,
                                      Aantal = aantalLeidingZonderEmail
                                  });
            }

            return resultaat;
        }

        Groep GetGroepEnCheckGav(int groepId)
        {
            var groep = (from g in _groepenRepo.Select()
                         where g.ID == groepId
                         select g).FirstOrDefault();
            CheckGav(groep);
            return groep;
        }

        Groep GetGroepEnCheckGav(string groepCode)
        {
            var groep = (from g in _groepenRepo.Select()
                         where Equal(g.Code, groepCode)
                         select g).FirstOrDefault();
            CheckGav(groep);
            return groep;
        }

        /// <summary>
        /// Voegt een functie toe aan de groep
        /// </summary>
        /// <param name="groepId">De groep waaraan het wordt toegevoegd</param>
        /// <param name="naam">De naam van de nieuwe functie</param>
        /// <param name="code">Code voor de nieuwe functie</param>
        /// <param name="maxAantal">Eventueel het maximumaantal leden met die functie in een werkJaar</param>
        /// <param name="minAantal">Het minimumaantal leden met die functie in een werkJaar</param>
        /// <param name="lidType">Gaat het over een functie voor leden, leiding of beide?</param>
        /// <param name="werkJaarVan">Eventueel het vroegste werkJaar waarvoor de functie beschikbaar moet zijn</param>
        /// <returns>De ID van de aangemaakte Functie</returns>
        public int FunctieToevoegen(int groepId, string naam, string code, int? maxAantal, int minAantal, LidType lidType, int? werkJaarVan)
        {
            var groep = GetGroepEnCheckGav(groepId);

            // Bestaat er al een eigen of nationale functie met dezelfde code?
            var bestaandeFunctie = _groepenMgr.FunctieZoeken(groep, code, _functiesRepo);
            if (bestaandeFunctie != null)
            {
                throw FaultExceptionHelper.BestaatAl(Mapper.Map<Functie, FunctieInfo>(
                    bestaandeFunctie));
            }

            var recentsteWerkJaar = _groepenMgr.RecentsteWerkJaar(groep);
            if (recentsteWerkJaar == null)
            {
                throw FaultExceptionHelper.FoutNummer(FoutNummer.GroepsWerkJaarNietBeschikbaar,
                                                Properties.Resources.GeenWerkJaar);
            }

            var f = new Functie
                        {
                            Code = code,
                            Groep = groep,
                            MaxAantal = maxAantal,
                            MinAantal = minAantal,
                            Niveau = _groepenMgr.LidTypeNaarMiveau(lidType, groep.Niveau),
                            Naam = naam,
                            WerkJaarTot = null,
                            WerkJaarVan = recentsteWerkJaar.WerkJaar,
                            IsNationaal = false
                        };

            groep.Functie.Add(f);

            _context.SaveChanges();

            return f.ID;
        }

        /// <summary>
        /// Verwijdert de functie met gegeven <paramref name="functieId"/>
        /// </summary>
        /// <param name="functieId">ID van de te verwijderen functie</param>
        /// <param name="forceren">Indien <c>true</c>, worden eventuele personen uit de
        /// te verwijderen functie eerst uit de functie weggehaald.  Indien
        /// <c>false</c> krijg je een exception als de functie niet leeg is.</param>
        public void FunctieVerwijderen(int functieId, bool forceren)
        {
            var functie = (from g in _functiesRepo.Select()
                            where g.ID == functieId
                            select g).FirstOrDefault();
            CheckGav(functie);
            Debug.Assert(functie != null, "functie != null");
            if (forceren)
            {
                functie.Lid.Clear();
            }
            _functiesRepo.Delete(functie);
            _context.SaveChanges();
        }

        /// <summary>
        /// Voegt een categorie toe aan de groep
        /// </summary>
        /// <param name="groepId">De groep waaraan het wordt toegevoegd</param>
        /// <param name="naam">De naam van de nieuwe categorie</param>
        /// <param name="code">Code voor de nieuwe categorie</param>
        /// <returns>De ID van de aangemaakte categorie</returns>
        public int CategorieToevoegen(int groepId, string naam, string code)
        {
            var groep = GetGroepEnCheckGav(groepId);

            var bestaandeCategorie = (from c in groep.Categorie
                                      where String.Compare(c.Code, code, StringComparison.OrdinalIgnoreCase) == 0
                                      select c).FirstOrDefault();

            if (bestaandeCategorie != null)
            {
                var info = Mapper.Map<Categorie, CategorieInfo>(bestaandeCategorie);
                throw FaultExceptionHelper.BestaatAl(info);
            }

            var nieuweCategorie = new Categorie { Code = code, Naam = naam };
            groep.Categorie.Add(nieuweCategorie);
            _context.SaveChanges();

            return nieuweCategorie.ID;
        }

        /// <summary>
        /// Verwijdert de gegeven categorie
        /// </summary>
        /// <param name="categorieId">De ID van de te verwijderen categorie</param>
        /// <param name="forceren">Indien <c>true</c>, worden eventuele personen uit de
        /// te verwijderen categorie eerst uit de categorie weggehaald.  Indien
        /// <c>false</c> krijg je een exception als de categorie niet leeg is.</param>
        public void CategorieVerwijderen(int categorieId, bool forceren)
        {
            var categorie = (from g in _categorieenRepo.Select()
                           where g.ID == categorieId
                           select g).FirstOrDefault();
            CheckGav(categorie);
            Debug.Assert(categorie != null, "categorie != null");
            if (forceren)
            {
                categorie.GelieerdePersoon.Clear();
            }
            _categorieenRepo.Delete(categorie);
        }

        /// <summary>
        /// Het veranderen van de naam van een categorie
        /// </summary>
        /// <param name="categorieId">De ID van de categorie</param>
        /// <param name="nieuwenaam">De nieuwe naam van de categorie</param>
        /// <exception cref="FoutNummerException">Gegooid als de naam leeg is of null is</exception>
        public void CategorieAanpassen(int categorieId, string nieuwenaam)
        {
            var categorie = (from g in _categorieenRepo.Select()
                             where g.ID == categorieId
                             select g).FirstOrDefault();
            CheckGav(categorie);
            Debug.Assert(categorie != null, "categorie != null");

            if (string.IsNullOrEmpty(nieuwenaam))
            {
                throw FaultExceptionHelper.FoutNummer(FoutNummer.ValidatieFout, Properties.Resources.OngeldigeCategorieNaam);
            }
            bool bestaatal = (from g in _categorieenRepo.Select()
                              where Equal(g.Naam,nieuwenaam)
                              select g).Any();
            if (bestaatal)
            {
                throw FaultExceptionHelper.BestaatAl(nieuwenaam);
            }
            categorie.Naam = nieuwenaam;

            _context.SaveChanges();
        }

        /// <summary>
        /// Zoekt een categorie op, op basis van <paramref name="groepId"/> en
        /// <paramref name="code"/>
        /// </summary>
        /// <param name="groepId">ID van de groep waaraan de categorie gekoppeld moet zijn.</param>
        /// <param name="code">Code van de categorie</param>
        /// <returns>De categorie met code <paramref name="code"/> die van toepassing is op
        /// de groep met ID <paramref name="groepId"/>.</returns>
        public CategorieInfo CategorieOpzoeken(int groepId, string code)
        {
            var groep = GetGroepEnCheckGav(groepId);
            var categorie = groep.Categorie.FirstOrDefault(e => Equal(e.Code, code));
            if (categorie == null)
            {
                throw FaultExceptionHelper.GeenGav();
            }
            return Mapper.Map<Categorie, CategorieInfo>(categorie);
        }

        /// <summary>
        /// Haalt alle categorieeen op van de groep met ID <paramref name="groepId"/>
        /// </summary>
        /// <param name="groepId">ID van de groep waarvan de categorieen zijn gevraagd</param>
        /// <returns>Lijst met categorie-info van de categorieen van de gevraagde groep</returns>
        public IList<CategorieInfo> CategorieenOphalen(int groepId)
        {
            var groep = GetGroepEnCheckGav(groepId);
            return Mapper.Map<IEnumerable<Categorie>, IList<CategorieInfo>>(groep.Categorie);
        }

        /// <summary>
        /// Zoekt de categorieID op van de categorie bepaald door de gegeven 
        /// <paramref name="groepId"/> en <paramref name="code"/>.
        /// </summary>
        /// <param name="groepId">ID van groep waaraan de gezochte categorie gekoppeld is</param>
        /// <param name="code">Code van de te zoeken categorie</param>
        /// <returns>Het categorieID als de categorie gevonden is, anders 0.</returns>
        public int CategorieIDOphalen(int groepId, string code)
        {
            return CategorieOpzoeken(groepId, code).ID;
        }

        /// <summary>
        /// Maakt een lijst met alle deelgemeentes uit de database; nuttig voor autocompletion
        /// in de ui.
        /// </summary>
        /// <returns>Lijst met alle beschikbare deelgemeentes</returns>
        public IEnumerable<WoonPlaatsInfo> GemeentesOphalen()
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Maakt een lijst met alle landen uit de database.
        /// </summary>
        /// <returns>Lijst met alle beschikbare landen</returns>
        public IEnumerable<LandInfo> LandenOphalen()
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt alle straten op uit een gegeven <paramref name="postNr"/>, waarvan de naam begint
        /// met het gegeven <paramref name="straatBegin"/>.
        /// </summary>
        /// <param name="straatBegin">Eerste letters van de te zoeken straatnamen</param>
        /// <param name="postNr">Postnummer waarin te zoeken</param>
        /// <returns>Gegevens van de gevonden straten</returns>
        public IEnumerable<StraatInfo> StratenOphalen(string straatBegin, int postNr)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
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
        public IEnumerable<StraatInfo> StratenOphalenMeerderePostNrs(string straatBegin, IEnumerable<int> postNrs)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Eens de gebruiker alle informatie heeft ingegeven, wordt de gewenste afdelingsverdeling naar de server gestuurd.
        /// <para />
        /// Dit in de vorm van een lijst van afdelingsjaardetails, met volgende info:
        ///		AFDELINGID van de afdelingen die geactiveerd zullen worden
        ///		Geboortejaren voor elk van die afdelingen
        /// </summary>
        /// <param name="teActiveren">Lijst van de afdelingen die geactiveerd moeten worden in het nieuwe werkJaar</param>
        /// <param name="groepId">ID van de groep voor wie een nieuw groepswerkjaar aangemaakt moet worden</param>
        public void JaarovergangUitvoeren(IEnumerable<AfdelingsJaarDetail> teActiveren, int groepId)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Berekent wat het nieuwe werkJaar zal zijn als op deze moment de jaarovergang zou gebeuren.
        /// </summary>
        /// <returns>Een jaartal</returns>
        public int NieuwWerkJaarOphalen(int groepId)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Deze method geeft gewoon de gebruikersnaam weer waaronder je de service aanroept.  Vooral om de
        /// authenticate te testen.
        /// </summary>
        /// <returns>Gebruikersnaam waarmee aangemeld</returns>
        public string WieBenIk()
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Deze method geeft weer of we op een liveomgeving werken (<c>true</c>) of niet (<c>false</c>)
        /// </summary>
        /// <returns><c>True</c> als we op een liveomgeving werken, <c>false</c> als we op een testomgeving werken</returns>
        public bool IsLive()
        {
            return _groepenMgr.IsLive();
        }

        /// <summary>
        /// Haalt informatie over alle gebruikersrechten van de gegeven groep op.
        /// </summary>
        /// <param name="groepId">ID van de groep waarvan de gebruikersrechten op te vragen zijn</param>
        /// <returns>Lijstje met details van de gebruikersrechten</returns>
        public IEnumerable<GebruikersDetail> GebruikersOphalen(int groepId)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Stelt afdelingsjaren voor voor het volgende werkjaar, gegeven de <paramref name="afdelingsIDs"/> van de
        /// afdelingen die je volgend werkjaar wilt hebben.
        /// </summary>
        /// <param name="afdelingsIDs">ID's van de afdelingen die je graag wilt activeren</param>
        /// <param name="groepId">ID van je groep</param>
        /// <returns>Een voorstel voor de afdelingsjaren, in de vorm van een lijstje AfdelingDetails.</returns>
        public IList<AfdelingDetail> NieuweAfdelingsJarenVoorstellen(int[] afdelingsIDs, int groepId)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }
    }
}
