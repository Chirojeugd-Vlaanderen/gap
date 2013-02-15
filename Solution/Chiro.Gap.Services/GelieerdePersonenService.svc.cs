// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;      // laten staan voor live!
using AutoMapper;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.Services.Properties;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.Services
{
    // OPM: als je de naam van de class "GelieerdePersonenService" hier verandert, moet je ook de sectie "Services" in web.config aanpassen.

    // *BELANGRIJK*: Als het debuggen hier stopt owv een autorisatiefout, kijk dan na of de gebruiker waarmee
    // je aangemeld bent, op je lokale computer in de groep CgUsers zit.

    /// <summary>
    /// Service voor operaties op gelieerde personen
    /// </summary>
    public class GelieerdePersonenService : IGelieerdePersonenService, IDisposable
    {
        // Repositories, verantwoordelijk voor data access.

        private readonly IRepository<CommunicatieVorm> _communicatieVormRepo;
        private readonly IRepository<GelieerdePersoon> _gelieerdePersonenRepo;
        private readonly IRepository<GroepsWerkJaar> _groepsWerkJarenRepo;
        private readonly IRepository<Groep> _groepenRepo;

        // Managers voor niet-triviale businesslogica

        private readonly IAutorisatieManager _autorisatieMgr;
        private readonly ICommunicatieVormenManager _communicatieVormenMgr;
        private readonly IGebruikersRechtenManager _gebruikersRechtenMgr;

        // Sync-interfaces

        private readonly ICommunicatieSync _communicatieSync;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repositoryProvider">De repositoryprovider levert de te
        /// gebruiken context en repository op.</param>
        /// <param name="autorisatieMgr">Logica m.b.t. autorisatie</param>
        /// <param name="communicatieVormenMgr">Logica m.b.t. communicatievormen</param>
        /// <param name="gebruikersRechtenMgr">Logica m.b.t. gebruikersrechten</param>
        /// <param name="communicatieSync">Zorgt voor synchronisatie met Kipadmin</param>
        public GelieerdePersonenService(IRepositoryProvider repositoryProvider, IAutorisatieManager autorisatieMgr,
                                        ICommunicatieVormenManager communicatieVormenMgr,
                                        IGebruikersRechtenManager gebruikersRechtenMgr,
                                        ICommunicatieSync communicatieSync)
        {
            _communicatieVormRepo = repositoryProvider.RepositoryGet<CommunicatieVorm>();
            _gelieerdePersonenRepo = repositoryProvider.RepositoryGet<GelieerdePersoon>();
            _groepsWerkJarenRepo = repositoryProvider.RepositoryGet<GroepsWerkJaar>();
            _groepenRepo = repositoryProvider.RepositoryGet<Groep>();

            _autorisatieMgr = autorisatieMgr;
            _communicatieVormenMgr = communicatieVormenMgr;
            _gebruikersRechtenMgr = gebruikersRechtenMgr;

            _communicatieSync = communicatieSync;
        }

        #region Disposable etc

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    _communicatieVormRepo.Dispose();
                    _gelieerdePersonenRepo.Dispose();
                }
                disposed = true;
            }
        }

        ~GelieerdePersonenService()
        {
            Dispose(false);
        }

        #endregion

        /// <summary>
        /// Haalt een persoonsgegevens op van gelieerde personen van een groep,
        /// inclusief eventueel lidobject voor het recentste werkJaar.
        /// </summary>
        /// <param name="selectieGelieerdePersoonIDs">GelieerdePersoonIDs van op te halen personen</param>
        /// <returns>Lijst van gelieerde personen met persoonsinfo</returns>
        public IList<PersoonDetail> OphalenMetLidInfo(IEnumerable<int> selectieGelieerdePersoonIDs)
        {
            var gelieerdePersonen = _gelieerdePersonenRepo.ByIDs(selectieGelieerdePersoonIDs);

            if (gelieerdePersonen.Any(gp => !_autorisatieMgr.IsGav(gp)))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var result = Mapper.Map<IEnumerable<GelieerdePersoon>, List<PersoonDetail>>(gelieerdePersonen);

            return result;
        }

        /// <summary>
        /// Haalt de persoonsgegevens op van gelieerde personen van een groep
        /// wiens familienaam begint met de letter <paramref name="letter"/>.
        /// inclusief eventueel lidobject voor het recentste werkJaar.
        /// </summary>
        /// <param name="groepID">ID van de betreffende groep</param>
        /// <param name="letter">Beginletter van de achternaam</param>
        /// <param name="aantalTotaal">Outputparameter; levert het totaal aantal personen in de groep op</param>
        /// <returns>Lijst van gelieerde personen met persoonsinfo</returns>
        public IList<PersoonDetail> OphalenMetLidInfoViaLetter(int groepID, string letter, out int aantalTotaal)
        {
            var gelieerdePersonen = from gp in _gelieerdePersonenRepo.Select()
                                    where
                                        gp.Groep.ID == groepID &&
                                        String.Compare(gp.Persoon.Naam.Substring(0,1), letter, StringComparison.InvariantCultureIgnoreCase) == 0
                                    select gp;

            var groepsWerkJaar = _groepsWerkJarenRepo.Select()
                                                     .Where(gwj => gwj.Groep.ID == groepID)
                                                     .OrderByDescending(gwj => gwj.WerkJaar)
                                                     .First();

            var result = Mapper.Map<IEnumerable<GelieerdePersoon>, List<PersoonDetail>>(gelieerdePersonen);
            aantalTotaal = result.Count();

            // Bepaal of persoon lid of leiding kan worden.
            // (TODO: Dit moet naar een worker)

            foreach (var detail in result.Where(det => det.GeboorteDatum.HasValue && det.SterfDatum == null))
            {
                Debug.Assert(detail.GeboorteDatum.HasValue); // altijd true, zie boven

                int geboortejaar = detail.GeboorteDatum.Value.Year - detail.ChiroLeefTijd;
                var afd = (from a in groepsWerkJaar.AfdelingsJaar
                           where a.GeboorteJaarTot >= geboortejaar && a.GeboorteJaarVan <= geboortejaar
                           select a).FirstOrDefault();

                detail.KanLidWorden = (afd != null);

                detail.KanLeidingWorden = (detail.GeboorteDatum.Value.Year <
                                           DateTime.Today.Year - Settings.Default.LeidingVanafLeeftijd +
                                           detail.ChiroLeefTijd);
            }
            return result;
        }

        /// <summary>
        /// Haalt een pagina met persoonsgegevens op van gelieerde personen van een groep,
        /// inclusief eventueel lidobject voor het recentste werkJaar.
        /// </summary>
        /// <param name="groepID">ID van de betreffende groep</param>
        /// <param name="pagina">Paginanummer (1 of hoger)</param>
        /// <param name="paginaGrootte">Aantal records per pagina (1 of meer)</param>
        /// <param name="sortering">Geeft aan hoe de pagina gesorteerd moet worden</param>
        /// <param name="aantalTotaal">Outputparameter; geeft het totaal aantal personen weer in de lijst</param>
        /// <returns>Lijst van gelieerde personen met persoonsinfo</returns>
        public IList<PersoonDetail> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, PersoonSorteringsEnum sortering, out int aantalTotaal)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt een pagina met persoonsgegevens op van gelieerde personen van een groep die tot de gegeven categorie behoren,
        /// inclusief eventueel lidobject voor het recentste werkJaar.
        /// </summary>
        /// <param name="categorieID">ID van de gevraagde categorie</param>
        /// <param name="pagina">Paginanummer (1 of hoger)</param>
        /// <param name="paginaGrootte">Aantal records per pagina (1 of meer)</param>
        /// <param name="sortering">Geeft aan hoe de pagina gesorteerd moet worden</param>
        /// <param name="aantalTotaal">Outputparameter; geeft het totaal aantal personen weer in de lijst</param>
        /// <returns>Lijst van gelieerde personen met persoonsinfo</returns>
        public IList<PersoonDetail> PaginaOphalenUitCategorieMetLidInfo(int categorieID, string letter, PersoonSorteringsEnum sortering, out int aantalTotaal)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt persoonsgegevens op voor alle gegeven gelieerde personen.
        /// </summary>
        /// <param name="gelieerdePersoonIDs">GelieerdePersoonIDs van op te halen personen</param>
        /// <returns>List van PersoonInfo overeenkomend met die IDs</returns>
        public IList<PersoonInfo> PersoonInfoOphalen(IList<int> gelieerdePersoonIDs)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt een lijst op van de eerste letters van de achternamen van gelieerde personen van een groep
        /// </summary>
        /// <param name="groepID">De ID van de groep waaruit we de gelieerde persoonsnamen gaan halen</param>
        /// <returns>Lijst met de eerste letter van de namen</returns>
        public IList<string> EersteLetterNamenOphalen(int groepID)
        {
            var groep = _groepenRepo.ByID(groepID);

            if (!_autorisatieMgr.IsGav(groep))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            // ik concatenate hieronder naam en voornaam, om personen op te vangen 
            // zonder familienaam. Blijkbaar zijn er zo. (Als dat maar goed komt...)

            return (from gp in groep.GelieerdePersoon
                          orderby gp.Persoon.Naam + gp.Persoon.VoorNaam
                          select (gp.Persoon.Naam + gp.Persoon.VoorNaam).Substring(0, 1).ToUpper()).Distinct().ToList();
        }

        /// <summary>
        /// Haalt een lijst op van de eerste letters van de achternamen van gelieerde personen van een categorie
        /// </summary>
        /// <param name="categorie">Categorie waaruit we de letters willen halen</param>
        /// <returns>Lijst met de eerste letter van de namen</returns>
        public IList<string> EersteLetterNamenOphalenCategorie(int categorie)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt gelieerd persoon op, incl. persoonsgegevens, communicatievormen en adressen
        /// </summary>
        /// <param name="gelieerdePersoonID">ID op te halen GelieerdePersoon</param>
        /// <returns>GelieerdePersoon met persoonsgegevens, communicatievorm en adressen</returns>
        public PersoonDetail DetailOphalen(int gelieerdePersoonID)
        {
            var p = _gelieerdePersonenRepo.ByID(gelieerdePersoonID);

            if (!_autorisatieMgr.IsGav(p))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            return Mapper.Map<GelieerdePersoon, PersoonDetail>(p);
        }

        /// <summary>
        /// Haalt gelieerde persoon op met ALLE nodige info om het persoons-bewerken scherm te vullen:
        /// persoonsgegevens, categorieen, communicatievormen, lidinfo, afdelingsinfo, adressen
        /// functies
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van de gevraagde gelieerde persoon</param>
        /// <returns>
        /// Gelieerde persoon met ALLE nodige info om het persoons-bewerken scherm te vullen:
        /// persoonsgegevens, categorieen, communicatievormen, lidinfo, afdelingsinfo, adressen
        /// functies
        /// </returns>
        public PersoonLidInfo AlleDetailsOphalen(int gelieerdePersoonID)
        {
            var gelieerdePersoon = _gelieerdePersonenRepo.ByID(gelieerdePersoonID);

            if (gelieerdePersoon == null || !_autorisatieMgr.IsGav(gelieerdePersoon))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var result = Mapper.Map<GelieerdePersoon, PersoonLidInfo>(gelieerdePersoon);

            // Gebruikersrechten kunnen nog niet automatisch gemapt worden.

            // Als er gebruikersrechten zijn op de eigen groep, dan mappen we die gebruikersrechten naar 
            // GebruikersInfo

            var gebruikersRecht = _gebruikersRechtenMgr.GebruikersRechtGet(gelieerdePersoon);

            if (gebruikersRecht != null)
            {
                result.GebruikersInfo = Mapper.Map<Poco.Model.GebruikersRecht, GebruikersInfo>(gebruikersRecht);
            }

            // Als er geen gebruikersrecht is op eigen groep, maar wel een gebruiker, dan mappen we de gebruiker
            // naar GebruikersInfo

            if (gelieerdePersoon.Persoon.Gav.Any())
            {
                result.GebruikersInfo = Mapper.Map<Gav, GebruikersInfo>(gelieerdePersoon.Persoon.Gav.FirstOrDefault());
            }

            return result;
        }

        /// <summary>
        /// Haalt gegevens op van alle personen uit categorie met ID <paramref name="categorieID"/>
        /// </summary>
        /// <param name="categorieID">Indien verschillend van 0, worden alle personen uit de categore met
        /// gegeven CategoreID opgehaald.  Anders alle personen tout court.</param>
        /// <param name="sortering">Geeft aan hoe de pagina gesorteerd moet worden</param>
        /// <returns>Lijst 'PersoonOverzicht'-objecten van alle gelieerde personen uit de categorie</returns>
        public IEnumerable<PersoonOverzicht> AllenOphalenUitCategorie(int categorieID, PersoonSorteringsEnum sortering)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt gegevens op van alle personen uit groep met ID <paramref name="groepID"/>.
        /// </summary>
        /// <param name="groepID">ID van de groep waaruit de personen gehaald moeten worden</param>
        /// <param name="sortering">Geeft aan hoe de pagina gesorteerd moet worden</param>
        /// <returns>Rij 'PersoonOverzicht'-objecten van alle gelieerde personen uit de groep.</returns>
        public IEnumerable<PersoonOverzicht> AllenOphalenUitGroep(int groepID, PersoonSorteringsEnum sortering)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt gegevens op van alle gelieerdepersonen met IDs in <paramref name="gelieerdePersoonIDs"/>.
        /// </summary>
        /// <param name="gelieerdePersoonIDs">IDs van de gelieerdepersonen waarover informatie opgehaald moet worden</param>
        /// <returns>Rij 'PersoonOverzicht'-objecten van alle gelieerde personen uit de groep.</returns>
        public IEnumerable<PersoonOverzicht> AllenOphalenUitLijst(IList<int> gelieerdePersoonIDs)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Updatet een persoon op basis van <paramref name="persoonInfo"/>
        /// </summary>
        /// <param name="persoonInfo">Info over te bewaren persoon</param>
        /// <returns>ID van de bewaarde persoon</returns>
        public int Bewaren(PersoonInfo persoonInfo)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Maakt een nieuwe persoon aan, en koppelt die als gelieerde persoon aan de groep met gegeven
        /// <paramref>groepID</paramref>
        /// </summary>
        /// <param name="info">Informatie om de nieuwe (gelieerde) persoon te construeren</param>
        /// <param name="groepID">ID van de groep waaraan de nieuwe persoon gekoppeld moet worden</param>
        /// <returns>ID's van de bewaarde persoon en gelieerde persoon</returns>
        public IDPersEnGP Aanmaken(PersoonInfo info, int groepID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Maakt een nieuwe persoon aan, en koppelt die als gelieerde persoon aan de groep met gegeven <paramref>groepID</paramref>
        /// </summary>
        /// <param name="info">Informatie om de nieuwe (gelieerde) persoon te construeren</param>
        /// <param name="groepID">ID van de groep waaraan de nieuwe persoon gekoppeld moet worden</param>
        /// <returns>ID's van de bewaarde persoon en gelieerde persoon</returns>
        /// <param name="forceer">Als deze <c>true</c> is, wordt de nieuwe persoon sowieso gemaakt, ook
        /// al lijkt hij op een bestaande gelieerde persoon.  Is <paramref>force</paramref>
        /// <c>false</c>, dan wordt er een exceptie opgegooid als de persoon te hard lijkt op een
        /// bestaande.</param>
        /// <remarks>Adressen, Communicatievormen,... worden niet mee gepersisteerd; enkel de persoonsinfo
        /// en de Chiroleeftijd.  Ik had deze functie ook graag 'aanmaken' genoemd (zie coding guideline
        /// 190), maar dat mag blijkbaar niet bij services.</remarks>
        public IDPersEnGP AanmakenForceer(PersoonInfo info, int groepID, bool forceer)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt PersoonID op van een gelieerde persoon
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van de gelieerde persoon</param>
        /// <returns>PersoonID van de persoon gekoppeld aan de gelieerde persoon bepaald door <paramref name="gelieerdePersoonID"/></returns>
        /// <remarks>Eigenlijk is dit een domme method, maar ze wordt gemakshalve nog gebruikt.</remarks>
        public int PersoonIDGet(int gelieerdePersoonID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Zoekt naar (gelieerde)persoonID's op basis van naam, voornaam en groepid
        /// </summary>
        /// <param name="groepID">ID van de groep met de te vinden persoon</param>
        /// <param name="naam">Familienaam van de te vinden persoon</param>
        /// <param name="voornaam">Voornaam van de te vinden persoon</param>
        /// <returns>GelieerdePersoonID en PersoonID van de gevonden personen, of <c>null</c> als
        /// niet gevonden.</returns>
        /// <remarks>Dit is nogal een domme method, maar ze is nodig om ticket #710 te fixen.</remarks>
        public IDPersEnGP[] Opzoeken(int groepID, string naam, string voornaam)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Zoekt naar gelieerde personen van een bepaalde groep (met ID <paramref name="groepID"/> waarbij
        /// naam of voornaam ongeveer begint met <paramref name="teZoeken"/>
        /// </summary>
        /// <param name="groepID">GroepID dat bepaalt in welke gelieerde personen gezocht mag worden</param>
        /// <param name="teZoeken">Te zoeken voor- of achternaam (ongeveer)</param>
        /// <returns>Lijst met gevonden matches</returns>
        /// <remarks>Deze method levert enkel naam, voornaam en gelieerdePersoonID op!</remarks>
        public IEnumerable<PersoonInfo> ZoekenOpNaamVoornaamBegin(int groepID, string teZoeken)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt adres op, met daaraan gekoppeld de bewoners uit de groep met ID <paramref name="groepID"/>.
        /// </summary>
        /// <param name="adresID">ID op te halen adres</param>
        /// <param name="groepID">ID van de groep</param>
        /// <returns>Adresobject met gekoppelde personen</returns>
        /// <remarks>GelieerdePersoonID's van bewoners worden niet mee opgehaald</remarks>
        public GezinInfo GezinOphalen(int adresID, int groepID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Verhuist gelieerde personen van een oud naar een nieuw adres
        /// (De koppelingen Persoon-Oudadres worden aangepast 
        /// naar Persoon-NieuwAdres.)
        /// </summary>
        /// <param name="gelieerdePersoonIDs">ID's van te verhuizen *GELIEERDE* Personen </param>
        /// <param name="nieuwAdres">AdresInfo-object met nieuwe adresgegevens</param>
        /// <param name="oudAdresID">ID van het oude adres</param>
        /// <remarks>De ID van <paramref name="nieuwAdres"/> wordt genegeerd.  Het adresID wordt altijd
        /// opnieuw opgezocht in de bestaande adressen.  Bestaat het adres nog niet,
        /// dan krijgt het adres een nieuw ID.</remarks>
        public void GelieerdePersonenVerhuizen(IEnumerable<int> gelieerdePersoonIDs, PersoonsAdresInfo nieuwAdres, int oudAdresID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt alle personen op die een adres gemeen hebben met de
        /// Persoon bepaald door gelieerdePersoonID
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van GelieerdePersoon</param>
        /// <returns>Lijst met Personen die huisgenoot zijn van gegeven
        /// persoon</returns>
        /// <remarks>Parameters: GELIEERDEpersoonID, returns PERSONEN</remarks>
        public IList<BewonersInfo> HuisGenotenOphalenZelfdeGroep(int gelieerdePersoonID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Voegt een adres toe aan een verzameling *GELIEERDE* personen
        /// </summary>
        /// <param name="gelieerdePersonenIDs">ID's van de gelieerde personen
        /// waaraan het nieuwe adres toegevoegd moet worden.</param>
        /// <param name="adr">Toe te voegen adres</param>
        /// <param name="voorkeur"><c>True</c> als het nieuwe adres het voorkeursadres moet worden.</param>
        public void AdresToevoegenGelieerdePersonen(List<int> gelieerdePersonenIDs, PersoonsAdresInfo adr, bool voorkeur)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Verwijdert een adres van een verzameling personen
        /// </summary>
        /// <param name="personenIDs">ID's van de personen over wie het gaat</param>
        /// <param name="adresID">ID van het adres dat losgekoppeld moet worden</param>
        public void AdresVerwijderenVanPersonen(IList<int> personenIDs, int adresID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Maakt het PersoonsAdres met ID <paramref name="persoonsAdresID"/> het voorkeursadres van de gelieerde persoon
        /// met ID <paramref name="gelieerdePersoonID"/>
        /// </summary>
        /// <param name="persoonsAdresID">ID van het persoonsadres dat voorkeursadres moet worden</param>
        /// <param name="gelieerdePersoonID">ID van de gelieerde persoon die het gegeven persoonsadres als voorkeur 
        /// moet krijgen.</param>
        /// <remarks>Goed opletten: een PersoonsAdres is gekoppeld aan een persoon; het voorkeursadres is gekoppeld
        /// aan een *gelieerde* persoon.</remarks>
        public void VoorkeursAdresMaken(int persoonsAdresID, int gelieerdePersoonID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Voegt een commvorm toe aan een gelieerde persoon
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van de gelieerde persoon</param>
        /// <param name="commInfo">De communicatievorm die aan die persoon gekoppeld moet worden</param>
        public void CommunicatieVormToevoegen(int gelieerdePersoonID, CommunicatieInfo commInfo)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Verwijdert een communicatievorm van een gelieerde persoon
        /// </summary>
        /// <param name="commvormID">ID van de communicatievorm</param>
        /// <returns>De ID van de gelieerdepersoon die bij de commvorm hoort</returns>
        public int CommunicatieVormVerwijderenVanPersoon(int commvormID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Persisteert de wijzigingen aan een bestaande communicatievorm
        /// </summary>
        /// <param name="c">De aan te passen communicatievorm</param>
        public void CommunicatieVormAanpassen(CommunicatieInfo c)
        {
            var communicatieVorm = (from cv in _communicatieVormRepo.Select()
                                    where cv.ID == c.ID
                                    select cv).FirstOrDefault();

            // TODO: Ik weet eigenlijk nog niet of lazy loading werkt.
            // Mag ik er vanuitgaan dat eender wat ik achteraf nodig heb, bijgeladen
            // wordt?

            // Autorisatie:

            if (communicatieVorm == null || !_autorisatieMgr.IsGav(communicatieVorm))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            // Het origineel nummer gaan we nodig hebben, opdat KipSync zou weten
            // welke communicatievorm te vervangen.

            string origineelNummer = communicatieVorm.Nummer;

            // Worker gebruiken voor bijwerken, owv de IsVoorkeur
            _communicatieVormenMgr.Bijwerken(communicatieVorm, c);

            // Niet vergeten te bewaren en te syncen
#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
            _communicatieVormRepo.SaveChanges();
            _communicatieSync.Bijwerken(communicatieVorm, origineelNummer);
#if KIPDORP
            }
#endif
        }

        /// <summary>
        /// Haalt info over een bepaald communicatietype op, op basis van ID
        /// </summary>
        /// <param name="commTypeID">De ID van het communicatietype</param>
        /// <returns>Info over het gevraagde communicatietype</returns>
        public CommunicatieTypeInfo CommunicatieTypeOphalen(int commTypeID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt een lijst op met alle communicatietypes
        /// </summary>
        /// <returns>Een lijst op met alle communicatietypes</returns>
        public IEnumerable<CommunicatieTypeInfo> CommunicatieTypesOphalen()
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt detail van een communicatievorm op
        /// </summary>
        /// <param name="commvormID">ID van de communicatievorm waarover het gaat</param>
        /// <returns>De communicatievorm met de opgegeven ID</returns>
        public CommunicatieDetail CommunicatieVormOphalen(int commvormID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Voegt een collectie gelieerde personen op basis van hun ID toe aan een collectie categorieën
        /// </summary>
        /// <param name="gelieerdepersonenIDs">ID's van de gelieerde personen</param>
        /// <param name="categorieIDs">ID's van de categorieën waaraan ze toegevoegd moeten worden</param>
        public void CategorieKoppelen(IList<int> gelieerdepersonenIDs, IList<int> categorieIDs)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Haalt een collectie gelieerde personen uit de opgegeven categorie
        /// </summary>
        /// <param name="gelieerdepersonenIDs">ID's van de gelieerde personen over wie het gaat</param>
        /// <param name="categorieID">ID van de categorie waaruit ze verwijderd moeten worden</param>
        public void CategorieVerwijderen(IList<int> gelieerdepersonenIDs, int categorieID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Bestelt Dubbelpunt voor de persoon met GelieerdePersoonID <paramref name="gelieerdePersoonID"/>.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van gelieerde persoon van persoon die Dubbelpunt wil</param>
        public void DubbelPuntBestellen(int gelieerdePersoonID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }
        
    }
}
