// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using AutoMapper;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
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
        private readonly IRepository<Functie> _functiesRepo;
        private readonly IRepository<GroepsWerkJaar> _groepsWerkJarenRepo;

        // Managers voor niet-triviale businesslogica
        
        private readonly IAuthenticatieManager _authenticatieMgr;
        private readonly IAutorisatieManager _autorisatieMgr;
        private readonly IGroepenManager _groepenMgr;
        private readonly IGroepsWerkJarenManager _groepsWerkJarenMgr;
        private readonly IFunctiesManager _functiesMgr;
        private readonly IVeelGebruikt _veelGebruikt;

        /// <summary>
        /// Nieuwe groepenservice
        /// </summary>
        /// <param name="authenticatieMgr">Verantwoordelijk voor authenticatie</param>
        /// <param name="autorisatieMgr">Verantwoordelijke voor autorisatie</param>
        /// <param name="groepenMgr">Businesslogica aangaande groepen</param>
        /// <param name="groepsWerkJarenMgr">Businesslogica wat betreft groepswerkjaren</param>
        /// <param name="functiesMgr">Businesslogica aangaande functies</param>
        /// <param name="veelGebruikt">Cache voor veelgebruikte zaken</param>
        /// <param name="repositoryProvider">De repository provider levert alle nodige repository's op.</param>
        public GroepenService(IAuthenticatieManager authenticatieMgr, IAutorisatieManager autorisatieMgr,
                              IGroepenManager groepenMgr, IGroepsWerkJarenManager groepsWerkJarenMgr,
                              IFunctiesManager functiesMgr, IVeelGebruikt veelGebruikt,
                              IRepositoryProvider repositoryProvider)
        {
            _context = repositoryProvider.ContextGet();
            _groepenRepo = repositoryProvider.RepositoryGet<Groep>();
            _functiesRepo = repositoryProvider.RepositoryGet<Functie>();
            _groepsWerkJarenRepo = repositoryProvider.RepositoryGet<GroepsWerkJaar>();

            _groepenMgr = groepenMgr;
            _groepsWerkJarenMgr = groepsWerkJarenMgr;
            _functiesMgr = functiesMgr;
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
        /// <param name="groepID">GroepID van groep waarvan we de informatie willen opvragen</param>
        /// <returns>
        /// De gevraagde informatie over de groep met id <paramref name="groepID"/>
        /// </returns>
        public GroepInfo InfoOphalen(int groepID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt info op, uitgaande van code (stamnummer)
        /// </summary>
        /// <param name="code">Stamnummer van de groep waarvoor info opgehaald moet worden</param>
        /// <returns>Groepsinformatie voor groep met code <paramref name="code"/></returns>
        public GroepInfo InfoOphalenCode(string code)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Ophalen van gedetailleerde informatie over de groep met ID <paramref name="groepID"/>
        /// </summary>
        /// <param name="groepID">ID van de groep waarvoor de informatie opgehaald moet worden</param>
        /// <returns>Groepsdetails, inclusief categorieen en huidige actieve afdelingen</returns>
        public GroepDetail DetailOphalen(int groepID)
        {
            var resultaat = new GroepDetail
                                {
                                    Afdelingen = new List<AfdelingDetail>()
                                };

            var groepsWerkJaar =
                _groepsWerkJarenRepo.Select()
                                    .Where(gwj => gwj.Groep.ID == groepID)
                                    .OrderByDescending(gwj => gwj.WerkJaar)
                                    .FirstOrDefault();

            if (!_autorisatieMgr.IsGav(groepsWerkJaar))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            Debug.Assert(groepsWerkJaar != null);

            Mapper.Map(groepsWerkJaar.Groep, resultaat);
            Mapper.Map(groepsWerkJaar.AfdelingsJaar, resultaat.Afdelingen);

            return resultaat;
        }

        /// <summary>
        /// Haalt de groepen op waarvoor de gebruiker (GAV-)rechten heeft
        /// </summary>
        /// <returns>De (informatie over de) groepen van de gebruiker</returns>
        public IEnumerable<GroepInfo> MijnGroepenOphalen()
        {
            string mijnLogin = _authenticatieMgr.GebruikersNaamGet();
            var groepen = from g in _groepenRepo.Select()
                          where g.GebruikersRecht.Any(gr => gr.Gav.Login == mijnLogin)
                          select g;
            return Mapper.Map<IEnumerable<Groep>, IEnumerable<GroepInfo>>(groepen);
        }

        /// <summary>
        /// Haalt informatie op over alle werkjaren waarin een groep actief was/is.
        /// </summary>
        /// <param name="groepsID">ID van de groep</param>
        /// <returns>Info over alle werkjaren waarin een groep actief was/is.</returns>
        public IEnumerable<WerkJaarInfo> WerkJarenOphalen(int groepsID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Persisteert een groep in de database
        /// </summary>
        /// <param name="groepInfo">Te persisteren groep</param>
        /// <remarks>FIXME: gedetailleerde exception</remarks>
        public void Bewaren(GroepInfo groepInfo)
        {
            var groep = (from g in _groepenRepo.Select()
                         where g.ID == groepInfo.ID
                         select g).FirstOrDefault();

            // Momenteel ondersteunen we enkel het wijzigen van groepsnaam
            // en stamnummer. (En dat stamnummer wijzigen, mag dan nog enkel
            // als we super-gav zijn.)

            if (!_autorisatieMgr.IsGav(groep))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            Debug.Assert(groep != null);

            if (String.Compare(groepInfo.StamNummer, groep.Code, StringComparison.OrdinalIgnoreCase) != 0 && !_autorisatieMgr.IsSuperGav())
            {
                throw FaultExceptionHelper.GeenGav();
            }

            groep.Naam = groepInfo.Naam;
            groep.Code = groepInfo.StamNummer;

            _context.SaveChanges();
        }

        /// <summary>
        /// Haalt GroepsWerkJaarID van het recentst gemaakte groepswerkjaar
        /// voor een gegeven groep op.
        /// </summary>
        /// <param name="groepID">GroepID van groep</param>
        /// <returns>ID van het recentste GroepsWerkJaar</returns>
        public int RecentsteGroepsWerkJaarIDGet(int groepID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt gedetailleerde gegevens op van het recentst gemaakte groepswerkjaar
        /// voor een gegeven groep op.
        /// </summary>
        /// <param name="groepid">GroepID van groep</param>
        /// <returns>
        /// De details van het recentste groepswerkjaar
        /// </returns>
        public GroepsWerkJaarDetail RecentsteGroepsWerkJaarOphalen(int groepid)
        {
            var groepsWerkJaar =
                _groepsWerkJarenRepo.Select()
                                    .Where(gwj => gwj.Groep.ID == groepid)
                                    .OrderByDescending(gwj => gwj.WerkJaar)
                                    .FirstOrDefault();

            // Autorisatie:

            if (!_autorisatieMgr.IsGav(groepsWerkJaar))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var result = Mapper.Map<GroepsWerkJaar, GroepsWerkJaarDetail>(groepsWerkJaar);

            result.Status = _groepsWerkJarenMgr.OvergangMogelijk(DateTime.Now, result.WerkJaar)
                                ? WerkJaarStatus.InOvergang
                                : WerkJaarStatus.Bezig;

            return result;
        }

        /// <summary>
        /// Maakt een nieuwe afdeling voor een gegeven ChiroGroep
        /// </summary>
        /// <param name="chiroGroepID">ID van de groep</param>
        /// <param name="naam">Naam van de afdeling</param>
        /// <param name="afkorting">Afkorting van de afdeling (voor lijsten, overzichten,...)</param>
        public void AfdelingAanmaken(int chiroGroepID, string naam, string afkorting)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Bewaart een afdeling met de nieuwe informatie.
        /// </summary>
        /// <param name="info">De afdelingsinfo die opgeslagen moet worden</param>
        public void AfdelingBewaren(AfdelingInfo info)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Uitgebreide info ophalen over het afdelingsjaar met de opgegeven ID
        /// </summary>
        /// <param name="afdelingsJaarID">De ID van het afdelingsjaar in kwestie</param>
        /// <returns>Uitgebreide info over het afdelingsjaar met de opgegeven ID</returns>
        public AfdelingsJaarDetail AfdelingsJaarOphalen(int afdelingsJaarID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Maakt/bewerkt een AfdelingsJaar: 
        /// andere OfficieleAfdeling en/of andere leeftijden
        /// </summary>
        /// <param name="aj">AfdelingsJaarDetail met de gegevens over het aan te maken of te wijzigen
        /// afdelingsjaar.  <c>aj.AfdelingsJaarID</c> bepaat of het om een bestaand afdelingsjaar gaat
        /// (ID > 0), of een bestaand (ID == 0)</param>
        public void AfdelingsJaarBewaren(AfdelingsJaarDetail aj)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Verwijdert een afdelingsjaar 
        /// en controleert of er geen leden in zitten.
        /// </summary>
        /// <param name="afdelingsJaarID">ID van het afdelingsjaar waarover het gaat</param>
        public void AfdelingsJaarVerwijderen(int afdelingsJaarID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Verwijdert een afdeling
        /// </summary>
        /// <param name="afdelingID">ID van de afdeling waarover het gaat</param>
        public void AfdelingVerwijderen(int afdelingID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt details over alle officiele afdelingen op.
        /// </summary>
        /// <param name="groepID">ID van een groep, zodat aan de hand van het recenste groepswerkjaar
        /// de standaardgeboortejaren van en tot bepaald kunnen worden</param>
        /// <returns>Rij met details over de officiele afdelingen</returns>
        public IEnumerable<OfficieleAfdelingDetail> OfficieleAfdelingenOphalen(int groepID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haat een afdeling op, op basis van <paramref name="afdelingID"/>
        /// </summary>
        /// <param name="afdelingID">ID van op te halen afdeling</param>
        /// <returns>Info van de gevraagde afdeling</returns>
        public AfdelingInfo AfdelingOphalen(int afdelingID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt details op van een afdeling, gebaseerd op het <paramref name="afdelingsJaarID"/>
        /// </summary>
        /// <param name="afdelingsJaarID">ID van het AFDELINGSJAAR waarvoor de details opgehaald moeten 
        /// worden.</param>
        /// <returns>De details van de afdeling in het gegeven afdelingsjaar.</returns>
        public AfdelingDetail AfdelingDetailOphalen(int afdelingsJaarID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt details op over alle actieve afdelingen in het groepswerkjaar met 
        /// ID <paramref name="groepsWerkJaarID"/>
        /// </summary>
        /// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
        /// <returns>
        /// Informatie over alle actieve afdelingen in het groepswerkjaar met 
        /// ID <paramref name="groepsWerkJaarID"/>
        /// </returns>
        public IList<AfdelingDetail> ActieveAfdelingenOphalen(int groepsWerkJaarID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt beperkte informatie op over de beschikbare afdelingen van een groep in het huidige
        /// groepswerkjaar.
        /// </summary>
        /// <param name="groepID">ID van de groep waarvoor de afdelingen gevraagd zijn</param>
        /// <returns>Lijst van ActieveAfdelingInfo</returns>
        public IList<AfdelingInfo> AlleAfdelingenOphalen(int groepID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt informatie op over de beschikbare afdelingsjaren en hun gelinkte afdelingen van een groep in het huidige
        /// groepswerkjaar.
        /// </summary>
        /// <param name="groepID">ID van de groep waarvoor de info gevraagd is</param>
        /// <returns>Lijst van AfdelingInfo</returns>
        public IList<ActieveAfdelingInfo> HuidigeAfdelingsJarenOphalen(int groepID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt informatie op over de afdelingen van een groep die niet gebruikt zijn in een gegeven 
        /// groepswerkjaar, op basis van een <paramref name="groepswerkjaarID"/>
        /// </summary>
        /// <param name="groepswerkjaarID">ID van het groepswerkjaar waarvoor de niet-gebruikte afdelingen
        /// opgezocht moeten worden.</param>
        /// <returns>Info de ongebruikte afdelingen van een groep in het gegeven groepswerkjaar</returns>
        public IList<AfdelingInfo> OngebruikteAfdelingenOphalen(int groepswerkjaarID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt uit groepswerkjaar met ID <paramref name="groepsWerkJaarID"/> alle beschikbare functies
        /// op voor een lid van type <paramref name="lidType"/>.
        /// </summary>
        /// <param name="groepsWerkJaarID">ID van het groepswerkjaar van de gevraagde functies</param>
        /// <param name="lidType"><c>LidType.Kind</c> of <c>LidType.Leiding</c></param>
        /// <returns>De gevraagde lijst afdelingsinfo</returns>
        public IEnumerable<FunctieDetail> FunctiesOphalen(int groepsWerkJaarID, LidType lidType)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Zoekt naar problemen ivm de maximum- en minimumaantallen van functies voor het
        /// huidige werkJaar.
        /// </summary>
        /// <param name="groepID">ID van de groep waarvoor de functies gecontroleerd moeten worden.</param>
        /// <returns>
        /// Een rij FunctieProbleemInfo.  Als er geen problemen zijn, is deze leeg.
        /// </returns>
        public IEnumerable<FunctieProbleemInfo> FunctiesControleren(int groepID)
        {
            var groepsWerkJaar =
                _groepsWerkJarenRepo.Select()
                                    .Where(gwj => gwj.Groep.ID == groepID)
                                    .OrderByDescending(gwj => gwj.WerkJaar)
                                    .FirstOrDefault();

            if (!_autorisatieMgr.IsGav(groepsWerkJaar))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            Debug.Assert(groepsWerkJaar != null);

            var problemen = _functiesMgr.AantallenControleren(
                groepsWerkJaar,
                groepsWerkJaar.Groep.Functie.Union(_veelGebruikt.NationaleFunctiesOphalen(_functiesRepo)));

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
        /// <param name="groepID">ID van de groep waarvan de leden te controleren zijn</param>
        /// <returns>Een rij LedenProbleemInfo.  Leeg bij gebrek aan problemen.</returns>
        public IEnumerable<LedenProbleemInfo> LedenControleren(int groepID)
        {
            var resultaat = new List<LedenProbleemInfo>();
            var groepsWerkJaar =
                _groepsWerkJarenRepo.Select()
                                    .Where(gwj => gwj.Groep.ID == groepID)
                                    .OrderByDescending(gwj => gwj.WerkJaar)
                                    .FirstOrDefault();

            if (!_autorisatieMgr.IsGav(groepsWerkJaar))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            Debug.Assert(groepsWerkJaar != null);

            int aantalLedenZonderAdres = (from ld in groepsWerkJaar.Lid
                                          where ld.GelieerdePersoon.PersoonsAdres == null
                                          // geen voorkeursadres
                                          select ld).Count();

            int aantalLedenZonderTelefoonNr = (from ld in groepsWerkJaar.Lid
                                               where
                                                   !ld.GelieerdePersoon.Communicatie.Any(
                                                       cmm =>
                                                       cmm.CommunicatieType.ID ==
                                                       (int) CommunicatieTypeEnum.TelefoonNummer)
                                               select ld).Count();
            // Hierboven: tel de personen waarbij er geen enkel communicatiemiddel is van het type
            // 'telefoonnummer'. Resharper zal voorstellen om dit te vervangen door
            // alle communicatiemiddelen hebben een type verschillend van telefoonnummer, maar dat
            // is niet equivalent, IHB als er geen communicatietypes zijn.

            int aantalLeidingZonderEmail = (from ld in groepsWerkJaar.Lid
                                               where ld.Type == LidType.Leiding &&
                                                   !ld.GelieerdePersoon.Communicatie.Any(
                                                       cmm =>
                                                       cmm.CommunicatieType.ID ==
                                                       (int) CommunicatieTypeEnum.Email)
                                               select ld).Count();

            if (aantalLedenZonderAdres > 0)
            {
                resultaat.Add(new LedenProbleemInfo
                                  {
                                      Probleem = LidProbleem.AdresOntbreekt,
                                      Aantal = aantalLedenZonderAdres
                                  });
            }

            if (aantalLedenZonderTelefoonNr > 0)
            {
                resultaat.Add(new LedenProbleemInfo
                                  {
                                      Probleem = LidProbleem.TelefoonNummerOntbreekt,
                                      Aantal = aantalLedenZonderTelefoonNr
                                  });
            }

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

        /// <summary>
        /// Voegt een functie toe aan de groep
        /// </summary>
        /// <param name="groepID">De groep waaraan het wordt toegevoegd</param>
        /// <param name="naam">De naam van de nieuwe functie</param>
        /// <param name="code">Code voor de nieuwe functie</param>
        /// <param name="maxAantal">Eventueel het maximumaantal leden met die functie in een werkJaar</param>
        /// <param name="minAantal">Het minimumaantal leden met die functie in een werkJaar</param>
        /// <param name="lidType">Gaat het over een functie voor leden, leiding of beide?</param>
        /// <param name="werkJaarVan">Eventueel het vroegste werkJaar waarvoor de functie beschikbaar moet zijn</param>
        /// <returns>De ID van de aangemaakte Functie</returns>
        public int FunctieToevoegen(int groepID, string naam, string code, int? maxAantal, int minAantal, LidType lidType, int? werkJaarVan)
        {
            // Haal groep op 

            var groep = (from g in _groepenRepo.Select()
                         where g.ID == groepID
                         select g).FirstOrDefault();

            // Heb ik daar rechten op?

            if (groep == null || !_autorisatieMgr.IsGav(groep))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            // Bestaat er al een eigen of nationale functie met dezelfde code?

            var bestaande = _groepenMgr.FunctieZoeken(groep, code, _functiesRepo);

            if (bestaande != null)
            {
                // TODO: Ook hier faultexception door helper laten genereren.
                throw new FaultException<BestaatAlFault<FunctieInfo>>(new BestaatAlFault<FunctieInfo>
                                                                          {
                                                                              Bestaande =
                                                                                  Mapper.Map<Functie, FunctieInfo>(
                                                                                      bestaande)
                                                                          });
            }

            var recentsteWerkJaar = _groepenMgr.RecentsteWerkJaar(groep);

            if (recentsteWerkJaar == null)
            {
                throw new FaultException<FoutNummerFault>(new FoutNummerFault { FoutNummer = FoutNummer.GroepsWerkJaarNietBeschikbaar },
                                                          new FaultReason(Properties.Resources.GeenWerkJaar));
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
        /// Verwijdert de functie met gegeven <paramref name="functieID"/>
        /// </summary>
        /// <param name="functieID">ID van de te verwijderen functie</param>
        /// <param name="forceren">Indien <c>true</c>, worden eventuele personen uit de
        /// te verwijderen functie eerst uit de functie weggehaald.  Indien
        /// <c>false</c> krijg je een exception als de functie niet leeg is.</param>
        public void FunctieVerwijderen(int functieID, bool forceren)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Voegt een categorie toe aan de groep
        /// </summary>
        /// <param name="groepID">De groep waaraan het wordt toegevoegd</param>
        /// <param name="naam">De naam van de nieuwe categorie</param>
        /// <param name="code">Code voor de nieuwe categorie</param>
        /// <returns>De ID van de aangemaakte categorie</returns>
        public int CategorieToevoegen(int groepID, string naam, string code)
        {
            var groep = (from g in _groepenRepo.Select()
                         where g.ID == groepID
                         select g).FirstOrDefault();

            if (!_autorisatieMgr.IsGav(groep))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            Debug.Assert(groep != null);

            // Check of de categorie al bestaat

            var bestaande = (from c in groep.Categorie
                             where String.Compare(c.Code, code, StringComparison.OrdinalIgnoreCase) == 0
                             select c).FirstOrDefault();

            if (bestaande != null)
            {
                var info = Mapper.Map<Categorie, CategorieInfo>(bestaande);
                throw FaultExceptionHelper.BestaatAl(info);
            }

            var nieuwe = new Categorie {Code = code, Naam = naam};
            groep.Categorie.Add(nieuwe);
            _context.SaveChanges();

            return nieuwe.ID;
        }

        /// <summary>
        /// Verwijdert de gegeven categorie
        /// </summary>
        /// <param name="categorieID">De ID van de te verwijderen categorie</param>
        /// <param name="forceren">Indien <c>true</c>, worden eventuele personen uit de
        /// te verwijderen categorie eerst uit de categorie weggehaald.  Indien
        /// <c>false</c> krijg je een exception als de categorie niet leeg is.</param>
        public void CategorieVerwijderen(int categorieID, bool forceren)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Het veranderen van de naam van een categorie
        /// </summary>
        /// <param name="categorieID">De ID van de categorie</param>
        /// <param name="nieuwenaam">De nieuwe naam van de categorie</param>
        /// <exception cref="InvalidOperationException">Gegooid als de naam al bestaat, leeg is of null is</exception>
        public void CategorieAanpassen(int categorieID, string nieuwenaam)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
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
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt alle categorieeen op van de groep met ID <paramref name="groepID"/>
        /// </summary>
        /// <param name="groepID">ID van de groep waarvan de categorieen zijn gevraagd</param>
        /// <returns>Lijst met categorie-info van de categorieen van de gevraagde groep</returns>
        public IList<CategorieInfo> CategorieenOphalen(int groepID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
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
            throw new NotImplementedException(NIEUWEBACKEND.Info);
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
        /// <param name="groepID">ID van de groep voor wie een nieuw groepswerkjaar aangemaakt moet worden</param>
        public void JaarovergangUitvoeren(IEnumerable<AfdelingsJaarDetail> teActiveren, int groepID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Berekent wat het nieuwe werkJaar zal zijn als op deze moment de jaarovergang zou gebeuren.
        /// </summary>
        /// <returns>Een jaartal</returns>
        public int NieuwWerkJaarOphalen(int groepID)
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
        /// <param name="groepID">ID van de groep waarvan de gebruikersrechten op te vragen zijn</param>
        /// <returns>Lijstje met details van de gebruikersrechten</returns>
        public IEnumerable<GebruikersDetail> GebruikersOphalen(int groepID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Stelt afdelingsjaren voor voor het volgende werkjaar, gegeven de <paramref name="afdelingsIDs"/> van de
        /// afdelingen die je volgend werkjaar wilt hebben.
        /// </summary>
        /// <param name="afdelingsIDs">ID's van de afdelingen die je graag wilt activeren</param>
        /// <param name="groepID">ID van je groep</param>
        /// <returns>Een voorstel voor de afdelingsjaren, in de vorm van een lijstje AfdelingDetails.</returns>
        public IList<AfdelingDetail> NieuweAfdelingsJarenVoorstellen(int[] afdelingsIDs, int groepID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }
    }
}
