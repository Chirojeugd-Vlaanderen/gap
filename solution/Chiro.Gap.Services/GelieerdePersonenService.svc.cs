// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;      // laten staan voor live!
using AutoMapper;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
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
        private readonly IRepository<Categorie> _categorieenRepo;
        private readonly IRepository<CommunicatieType> _communicatieTypesRepo;

        // Managers voor niet-triviale businesslogica

        private readonly IAutorisatieManager _autorisatieMgr;
        private readonly ICommunicatieVormenManager _communicatieVormenMgr;
        private readonly IGebruikersRechtenManager _gebruikersRechtenMgr;
        private readonly IGelieerdePersonenManager _gelieerdePersonenMgr;

        // Sync-interfaces

        private readonly ICommunicatieSync _communicatieSync;
        private readonly IPersonenSync _personenSync;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repositoryProvider">De repositoryprovider levert de te
        /// gebruiken context en repository op.</param>
        /// <param name="autorisatieMgr">Logica m.b.t. autorisatie</param>
        /// <param name="communicatieVormenMgr">Logica m.b.t. communicatievormen</param>
        /// <param name="gebruikersRechtenMgr">Logica m.b.t. gebruikersrechten</param>
        /// <param name="gelieerdePersonenMgr">Logica m.b.t. gelieerde personen</param>
        /// <param name="communicatieSync">Voor synchronisatie van communicatie met Kipadmin</param>
        /// <param name="personenSync">Voor synchronisatie van personen naar Kipadmin</param>
        public GelieerdePersonenService(IRepositoryProvider repositoryProvider, IAutorisatieManager autorisatieMgr,
                                        ICommunicatieVormenManager communicatieVormenMgr,
                                        IGebruikersRechtenManager gebruikersRechtenMgr, IGelieerdePersonenManager gelieerdePersonenMgr,
                                        ICommunicatieSync communicatieSync,
                                        IPersonenSync personenSync)
        {
            _communicatieVormRepo = repositoryProvider.RepositoryGet<CommunicatieVorm>();
            _gelieerdePersonenRepo = repositoryProvider.RepositoryGet<GelieerdePersoon>();
            _groepsWerkJarenRepo = repositoryProvider.RepositoryGet<GroepsWerkJaar>();
            _groepenRepo = repositoryProvider.RepositoryGet<Groep>();
            _categorieenRepo = repositoryProvider.RepositoryGet<Categorie>();
            _communicatieTypesRepo = repositoryProvider.RepositoryGet<CommunicatieType>();

            _autorisatieMgr = autorisatieMgr;
            _communicatieVormenMgr = communicatieVormenMgr;
            _gebruikersRechtenMgr = gebruikersRechtenMgr;
            _gelieerdePersonenMgr = gelieerdePersonenMgr;

            _communicatieSync = communicatieSync;
            _personenSync = personenSync;
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
            var groep = _groepenRepo.ByID(groepID);

            if (!_autorisatieMgr.IsGav(groep))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var gelieerdePersonen = from gp in groep.GelieerdePersoon
                                    where
                                        String.Compare((gp.Persoon.Naam + gp.Persoon.VoorNaam).Substring(0, 1), letter,
                                                       StringComparison.InvariantCultureIgnoreCase) == 0
                                    select gp;
            
            var result = Mapper.Map<IEnumerable<GelieerdePersoon>, List<PersoonDetail>>(gelieerdePersonen);
            aantalTotaal = groep.GelieerdePersoon.Count;

            return result;
        }

        /// <summary>
        /// Haalt persoonsgegevens op van gelieerde personen van een groep die tot de gegeven categorie behoren,
        /// waarvan de naam begint met de gegeven <paramref name="letter"/>
        /// inclusief eventueel lidobject voor het recentste werkJaar.
        /// </summary>
        /// <param name="categorieID">ID van de gevraagde categorie</param>
        /// <param name="letter">letter waarmee de naam moet beginnen</param>
        /// <param name="sortering">Geeft aan hoe de pagina gesorteerd moet worden</param>
        /// <param name="aantalTotaal">Outputparameter; geeft het totaal aantal personen weer in de lijst</param>
        /// <returns>Lijst van gelieerde personen met persoonsinfo</returns>
        public IList<PersoonDetail> OphalenUitCategorieMetLidInfo(int categorieID, string letter, PersoonSorteringsEnum sortering, out int aantalTotaal)
        {
            var categorie = _categorieenRepo.ByID(categorieID);

            if (!_autorisatieMgr.IsGav(categorie))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var gelieerdePersonen = from gp in categorie.GelieerdePersoon
                                    where
                                        String.Compare((gp.Persoon.Naam + gp.Persoon.VoorNaam).Substring(0, 1), letter,
                                                       StringComparison.InvariantCultureIgnoreCase) == 0
                                    select gp;

            var result = Mapper.Map<IEnumerable<GelieerdePersoon>, List<PersoonDetail>>(gelieerdePersonen);
            aantalTotaal = categorie.GelieerdePersoon.Count;

            return result;
        }

        /// <summary>
        /// Haalt persoonsgegevens op voor alle gegeven gelieerde personen.
        /// </summary>
        /// <param name="gelieerdePersoonIDs">GelieerdePersoonIDs van op te halen personen</param>
        /// <returns>List van PersoonInfo overeenkomend met die IDs</returns>
        public IList<PersoonInfo> InfoOphalen(IList<int> gelieerdePersoonIDs)
        {
            var p = _gelieerdePersonenRepo.ByIDs(gelieerdePersoonIDs);

            if (!_autorisatieMgr.IsGav(p))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            return Mapper.Map<IList<GelieerdePersoon>, List<PersoonInfo>>(p);
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
        /// <param name="categorieID">ID van de categorie waaruit we de letters willen halen</param>
        /// <returns>Lijst met de eerste letter van de namen</returns>
        public IList<string> EersteLetterNamenOphalenCategorie(int categorieID)
        {
            var categorie = _categorieenRepo.ByID(categorieID);

            if (!_autorisatieMgr.IsGav(categorie))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            // ik concatenate hieronder naam en voornaam, om personen op te vangen 
            // zonder familienaam. Blijkbaar zijn er zo. (Als dat maar goed komt...)

            return (from gp in categorie.GelieerdePersoon
                    orderby gp.Persoon.Naam + gp.Persoon.VoorNaam
                    select (gp.Persoon.Naam + gp.Persoon.VoorNaam).Substring(0, 1).ToUpper()).Distinct().ToList();
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
        /// <returns>Lijst 'PersoonOverzicht'-objecten van alle gelieerde personen uit de categorie</returns>
        public IList<PersoonOverzicht> AllenOphalenUitCategorie(int categorieID)
        {
            var categorie = _categorieenRepo.ByID(categorieID);

            if (!_autorisatieMgr.IsGav(categorie))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var gelieerdePersonen = from gp in categorie.GelieerdePersoon
                                    select gp;

            var result = Mapper.Map<IEnumerable<GelieerdePersoon>, List<PersoonOverzicht>>(gelieerdePersonen);

            return result;
        }

        /// <summary>
        /// Haalt gegevens op van alle personen uit groep met ID <paramref name="groepID"/>.
        /// </summary>
        /// <param name="groepID">ID van de groep waaruit de personen gehaald moeten worden</param>
        /// <returns>Rij 'PersoonOverzicht'-objecten van alle gelieerde personen uit de groep.</returns>
        public IList<PersoonOverzicht> AllenOphalenUitGroep(int groepID)
        {
            var groep = _groepenRepo.ByID(groepID);

            if (!_autorisatieMgr.IsGav(groep))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var gelieerdePersonen = from gp in groep.GelieerdePersoon
                                    select gp;

            var result = Mapper.Map<IEnumerable<GelieerdePersoon>, List<PersoonOverzicht>>(gelieerdePersonen);

            return result;
        }

        /// <summary>
        /// Haalt gegevens op van alle gelieerdepersonen met IDs in <paramref name="gelieerdePersoonIDs"/>.
        /// </summary>
        /// <param name="gelieerdePersoonIDs">IDs van de gelieerdepersonen waarover informatie opgehaald moet worden</param>
        /// <returns>Rij 'PersoonOverzicht'-objecten van alle gelieerde personen uit de groep.</returns>
        public IEnumerable<PersoonOverzicht> OverzichtOphalen(IList<int> gelieerdePersoonIDs)
        {
            var p = _gelieerdePersonenRepo.ByIDs(gelieerdePersoonIDs);

            if (!_autorisatieMgr.IsGav(p))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            return Mapper.Map<IList<GelieerdePersoon>, List<PersoonOverzicht>>(p);
        }

        /// <summary>
        /// Updatet een bestaand persoon op basis van <paramref name="persoonInfo"/>
        /// </summary>
        /// <param name="persoonInfo">Info over te bewaren persoon</param>
        /// <returns>GelieerdePersoonID van de bewaarde persoon</returns>
        public int Bewaren(PersoonInfo persoonInfo)
        {
            var gp = _gelieerdePersonenRepo.ByID(persoonInfo.GelieerdePersoonID);

            if (gp == null || !_autorisatieMgr.IsGav(gp))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            if (gp.Persoon.AdNummer != null && gp.Persoon.AdNummer != persoonInfo.AdNummer)
            {
                throw FaultExceptionHelper.FoutNummer(FoutNummer.AlgemeneFout, Resources.AdNummerNietWijzigen);
            }

            gp.ChiroLeefTijd = persoonInfo.ChiroLeefTijd;   // Chiroleeftijd vullen we gauw zo in
            Mapper.Map(persoonInfo, gp.Persoon);    // overschrijf persoonsgegevens met info uit persoonInfo


#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
            _gelieerdePersonenRepo.SaveChanges();
            if (gp.Persoon.AdNummer != null || gp.Persoon.AdInAanvraag)
            {
                _personenSync.Bewaren(gp, false, false);
            }
#if KIPDORP    
            }
#endif
            return gp.ID;
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
            return AanmakenForceer(info, groepID, false);
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
            var groep = _groepenRepo.ByID(groepID);

            if (!_autorisatieMgr.IsGav(groep))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var nieuwePersoon = new Persoon
                                    {
                                        AdNummer = null, // nieuwe persoon kan geen ad-nummer hebben
                                        VoorNaam = info.VoorNaam,
                                        Naam = info.Naam,
                                        GeboorteDatum = info.GeboorteDatum,
                                        Geslacht = info.Geslacht
                                    };
            GelieerdePersoon resultaat;

            try
            {
                resultaat = _gelieerdePersonenMgr.Toevoegen(nieuwePersoon, groep, 0, forceer);
            }
            catch (BlokkerendeObjectenException<GelieerdePersoon> ex)
            {
                throw FaultExceptionHelper.Blokkerend(
                    Mapper.Map<IList<GelieerdePersoon>, List<PersoonDetail>>(ex.Objecten), ex.Message);
            }

            _groepenRepo.SaveChanges();

            return new IDPersEnGP {GelieerdePersoonID = resultaat.ID, PersoonID = resultaat.Persoon.ID};
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
            var gelieerdePersoon = _gelieerdePersonenRepo.ByID(gelieerdePersoonID);
            if (!_autorisatieMgr.IsGav(gelieerdePersoon))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var communicatieVorm = new CommunicatieVorm();

            Mapper.Map(commInfo, communicatieVorm);
            communicatieVorm.ID = 0;    // zodat die zeker als nieuw wordt beschouwd
            communicatieVorm.CommunicatieType = _communicatieTypesRepo.ByID(commInfo.CommunicatieTypeID);

            // Communicatievormen koppelen is een beetje een gedoe, omdat je die voorkeuren hebt,
            // en het concept 'gezinsgebonden' (wat eigenlijk niet helemaal klopt)
            // Al die brol handelen we af in de manager.

            var gekoppeld = _communicatieVormenMgr.Koppelen(gelieerdePersoon, communicatieVorm);
            var tesyncen = (from cv in gekoppeld
                            where
                                cv.GelieerdePersoon.Persoon.AdNummer != null || cv.GelieerdePersoon.Persoon.AdInAanvraag
                            select cv).ToList();

#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
                    _communicatieVormRepo.SaveChanges();
                    // TODO (#1409): welke communicatievorm de voorkeur heeft, gaat verloren bij de sync
                    // naar Kipadmin. 
                    foreach (var cv in tesyncen)
                    {
                        _communicatieSync.Toevoegen(cv);
                    }
#if KIPDORP    
            }
#endif

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
            if (communicatieVorm.GelieerdePersoon.Persoon.AdNummer != null ||
                communicatieVorm.GelieerdePersoon.Persoon.AdInAanvraag)
            {
                _communicatieSync.Bijwerken(communicatieVorm, origineelNummer);
            }
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
            // Communicatietypes zijn voor iedereen leesbaar
            // (het gaat hier om 'e-mail','telefoonnr',...
            var communicatietype = _communicatieTypesRepo.ByID(commTypeID);
            return Mapper.Map<CommunicatieType, CommunicatieTypeInfo>(communicatietype);

        }

        /// <summary>
        /// Haalt een lijst op met alle communicatietypes
        /// </summary>
        /// <returns>Een lijst op met alle communicatietypes</returns>
        public IEnumerable<CommunicatieTypeInfo> CommunicatieTypesOphalen()
        {
            var communicatietypes = _communicatieTypesRepo.GetAll();
            return Mapper.Map<IEnumerable<CommunicatieType>, List<CommunicatieTypeInfo>>(communicatietypes);
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
