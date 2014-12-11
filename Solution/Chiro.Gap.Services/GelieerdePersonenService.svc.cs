/*
 * Copyright 2008-2014 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
#if KIPDORP
using System.Transactions;
#endif

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
    public class GelieerdePersonenService : BaseService, IGelieerdePersonenService, IDisposable
    {
        // Repositories, verantwoordelijk voor data access.

        private readonly IRepositoryProvider _repositoryProvider;
        private readonly IRepository<CommunicatieVorm> _communicatieVormRepo;
        private readonly IRepository<GelieerdePersoon> _gelieerdePersonenRepo;
        private readonly IRepository<Groep> _groepenRepo;
        private readonly IRepository<Categorie> _categorieenRepo;
        private readonly IRepository<CommunicatieType> _communicatieTypesRepo;
        private readonly IRepository<Adres> _adressenRepo;
        private readonly IRepository<PersoonsAdres> _persoonsAdressenRepo;
        private readonly IRepository<StraatNaam> _straatNamenRepo;
        private readonly IRepository<WoonPlaats> _woonPlaatsenRepo;
        private readonly IRepository<Land> _landenRepo;
        private readonly IRepository<AfdelingsJaar> _afdelingsJarenRepo; 

        // Managers voor niet-triviale businesslogica

        private readonly IAutorisatieManager _autorisatieMgr;
        private readonly ICommunicatieVormenManager _communicatieVormenMgr;
        private readonly IGebruikersRechtenManager _gebruikersRechtenMgr;
        private readonly IGelieerdePersonenManager _gelieerdePersonenMgr;
        private readonly IAdressenManager _adressenMgr;
        private readonly IPersonenManager _personenMgr;
        private readonly IGroepenManager _groepenMgr;

        // Sync-interfaces

        private readonly ICommunicatieSync _communicatieSync;
        private readonly IAdressenSync _adressenSync;
        private readonly IPersonenSync _personenSync;
        private readonly ILedenSync _ledenSync;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repositoryProvider">De repositoryprovider levert de te
        /// gebruiken context en repository op.</param>
        /// <param name="autorisatieMgr">Logica m.b.t. autorisatie</param>
        /// <param name="communicatieVormenMgr">Logica m.b.t. communicatievormen</param>
        /// <param name="gebruikersRechtenMgr">Logica m.b.t. gebruikersrechten</param>
        /// <param name="gelieerdePersonenMgr">Logica m.b.t. gelieerde personen</param>
        /// <param name="adressenManager">Logica m.b.t. adressen</param>
        /// <param name="personenManager">Logica m.b.t. personen (geeuw)</param>
        /// <param name="groepenManager">Logica m.b.t. groepen</param>
        /// <param name="ledenManager">Logica m.b.t. leden</param>
        /// <param name="groepsWerkJarenManager">Logica m.b.t. groepswerkjaren</param>
        /// <param name="communicatieSync">Voor synchronisatie van communicatie met Kipadmin</param>
        /// <param name="personenSync">Voor synchronisatie van personen naar Kipadmin</param>
        /// <param name="adressenSync">Voor synchronisatie van adressen naar Kipadmin</param>
        /// <param name="ledenSync">Voor synchronisatie lidgegevens naar Kipadmin</param>
        public GelieerdePersonenService(IRepositoryProvider repositoryProvider, IAutorisatieManager autorisatieMgr,
            ICommunicatieVormenManager communicatieVormenMgr,
            IGebruikersRechtenManager gebruikersRechtenMgr,
            IGelieerdePersonenManager gelieerdePersonenMgr,
            IAdressenManager adressenManager,
            IPersonenManager personenManager,
            IGroepenManager groepenManager,
            ILedenManager ledenManager,
            IGroepsWerkJarenManager groepsWerkJarenManager,
            ICommunicatieSync communicatieSync,
            IPersonenSync personenSync,
            IAdressenSync adressenSync,
            ILedenSync ledenSync): base(ledenManager, groepsWerkJarenManager)
        {
            _repositoryProvider = repositoryProvider;

            _communicatieVormRepo = repositoryProvider.RepositoryGet<CommunicatieVorm>();
            _gelieerdePersonenRepo = repositoryProvider.RepositoryGet<GelieerdePersoon>();
            repositoryProvider.RepositoryGet<GroepsWerkJaar>();
            _groepenRepo = repositoryProvider.RepositoryGet<Groep>();
            _categorieenRepo = repositoryProvider.RepositoryGet<Categorie>();
            _communicatieTypesRepo = repositoryProvider.RepositoryGet<CommunicatieType>();

            _adressenRepo = repositoryProvider.RepositoryGet<Adres>();
            _persoonsAdressenRepo = repositoryProvider.RepositoryGet<PersoonsAdres>();
            _straatNamenRepo = repositoryProvider.RepositoryGet<StraatNaam>();
            _woonPlaatsenRepo = repositoryProvider.RepositoryGet<WoonPlaats>();
            _landenRepo = repositoryProvider.RepositoryGet<Land>();
            _afdelingsJarenRepo = repositoryProvider.RepositoryGet<AfdelingsJaar>();

            _autorisatieMgr = autorisatieMgr;
            _communicatieVormenMgr = communicatieVormenMgr;
            _gebruikersRechtenMgr = gebruikersRechtenMgr;
            _gelieerdePersonenMgr = gelieerdePersonenMgr;
            _adressenMgr = adressenManager;
            _personenMgr = personenManager;
            _groepenMgr = groepenManager;

            _communicatieSync = communicatieSync;
            _adressenSync = adressenSync;
            _personenSync = personenSync;
            _ledenSync = ledenSync;
        }

        #region Disposable etc

        private bool disposed;

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
                    _repositoryProvider.Dispose();
                }
                disposed = true;
            }
        }

        ~GelieerdePersonenService()
        {
            Dispose(false);
        }

        #endregion

        #region Ophalen

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
        /// Haalt de persoonsgegevens op van alle personen van een groep
        /// </summary>
        /// <param name="groepID">ID van de betreffende groep</param>
        /// <returns>Lijst van gelieerde personen met persoonsinfo</returns>
        public IList<PersoonDetail> DetailsOphalen(int groepID)
        {
            var groep = _groepenRepo.ByID(groepID, "GelieerdePersoon.Persoon", "GelieerdePersoon.PersoonsAdres",
                "GelieerdePersoon.Categorie");

            if (!_autorisatieMgr.IsGav(groep))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var gelieerdePersonen = from gp in groep.GelieerdePersoon
                                    select gp;
            // Als we hier crashen, controleer dan of Persoon de kolommen SeNaam en SeVoornaam heeft.
            
            var result = Mapper.Map<IEnumerable<GelieerdePersoon>, List<PersoonDetail>>(gelieerdePersonen);

            return result;
        }

        /// <summary>
        /// Haalt persoonsgegevens op van gelieerde personen van een groep die tot de gegeven categorie behoren,
        /// inclusief eventueel lidobject voor het recentste werkJaar.
        /// </summary>
        /// <param name="categorieID">ID van de gevraagde categorie</param>
        /// <param name="aantalTotaal">Outputparameter; geeft het totaal aantal personen weer in de lijst</param>
        /// <returns>Lijst van gelieerde personen met persoonsinfo</returns>
        public IList<PersoonDetail> OphalenUitCategorieMetLidInfo(int categorieID, out int aantalTotaal)
        {
            var categorie = _categorieenRepo.ByID(categorieID);

            if (!_autorisatieMgr.IsGav(categorie))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var gelieerdePersonen = from gp in categorie.GelieerdePersoon
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
        public IList<PersoonInfo> InfosOphalen(IList<int> gelieerdePersoonIDs)
        {
            var p = _gelieerdePersonenRepo.ByIDs(gelieerdePersoonIDs);

            if (!_autorisatieMgr.IsGav(p))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            return Mapper.Map<IList<GelieerdePersoon>, List<PersoonInfo>>(p);
        }

        /// <summary>
        /// Haalt persoonsgegevens op voor een gegeven gelieerde persoon.
        /// </summary>
        /// <param name="gelieerdePersoonID">GelieerdePersoonID van op te halen persoon</param>
        /// <returns>PersoonInfo voor de persoon met gegeven <paramref name="gelieerdePersoonIDs" /></returns>
        public PersoonInfo InfoOphalen(int gelieerdePersoonID)
        {
            return InfosOphalen(new List<int> { gelieerdePersoonID }).FirstOrDefault();
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
            else if (gelieerdePersoon.Persoon.Gav.Any())
            {
                // Als er geen gebruikersrecht is op eigen groep, maar wel een gebruiker, dan mappen we de gebruiker
                // naar GebruikersInfo
                result.GebruikersInfo = Mapper.Map<Gav, GebruikersInfo>(gelieerdePersoon.Persoon.Gav.FirstOrDefault());
            }

            return result;
        }

        /// <summary>
        /// Haalt gegevens op van alle personen uit categorie met ID <paramref name="categorieID"/>
        /// </summary>
        /// <param name="categorieID">Indien verschillend van 0, worden alle personen uit de categore met
        /// gegeven CategoreID opgehaald.  Anders alle personen tout court.</param>
        /// <returns>Lijst 'PersoonLidInfo'-objecten van alle gelieerde personen uit de categorie</returns>
        public IList<PersoonLidInfo> AllenOphalenUitCategorie(int categorieID)
        {
            var categorie = _categorieenRepo.ByID(categorieID);

            if (!_autorisatieMgr.IsGav(categorie))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var gelieerdePersonen = from gp in categorie.GelieerdePersoon
                                    select gp;

            var result = Mapper.Map<IEnumerable<GelieerdePersoon>, List<PersoonLidInfo>>(gelieerdePersonen);

            return result;
        }

        /// <summary>
        /// Haalt gegevens op van alle personen uit groep met ID <paramref name="groepID"/>.
        /// </summary>
        /// <param name="groepID">ID van de groep waaruit de personen gehaald moeten worden</param>
        /// <returns>'PersoonLidInfo'-objecten van alle gelieerde personen uit de groep.</returns>
        public IList<PersoonLidInfo> AllenOphalenUitGroep(int groepID)
        {
            var groep = _groepenRepo.ByID(groepID);

            if (!_autorisatieMgr.IsGav(groep))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var gelieerdePersonen = from gp in groep.GelieerdePersoon
                                    select gp;

            var result = Mapper.Map<IEnumerable<GelieerdePersoon>, List<PersoonLidInfo>>(gelieerdePersonen);

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

        public IList<PersoonDetail> PaginaOphalen(int groepID, int pageSize, int page)
        {
            var groep = _groepenRepo.ByID(groepID);

            if (!_autorisatieMgr.IsGav(groep))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var gelieerdePersonen = (from gp in groep.GelieerdePersoon
                                     select gp).Skip((page - 1)*pageSize).Take(pageSize);

            var result = Mapper.Map<IEnumerable<GelieerdePersoon>, List<PersoonDetail>>(gelieerdePersonen);

            return result;
        }

        /// <summary>
        /// Haalt PersoonID op van een gelieerde persoon
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van de gelieerde persoon</param>
        /// <returns>PersoonID van de persoon gekoppeld aan de gelieerde persoon bepaald door <paramref name="gelieerdePersoonID"/></returns>
        /// <remarks>Eigenlijk is dit een domme method, maar ze wordt gemakshalve nog gebruikt.</remarks>
        public int PersoonIDGet(int gelieerdePersoonID)
        {
            var gelieerdePersoon = _gelieerdePersonenRepo.ByID(gelieerdePersoonID);

            if (!_autorisatieMgr.IsGav(gelieerdePersoon))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            return gelieerdePersoon.Persoon.ID;
        }

        /// <summary>
        /// Zoekt naar gelieerde personen van een bepaalde groep (met ID <paramref name="groepID"/> waarbij
        /// naam of voornaam begint met <paramref name="teZoeken"/>
        /// </summary>
        /// <param name="groepID">GroepID dat bepaalt in welke gelieerde personen gezocht mag worden</param>
        /// <param name="teZoeken">Te zoeken voor- of achternaam</param>
        /// <returns>Lijst met gevonden matches</returns>
        /// <remarks>Deze method levert enkel naam, voornaam en gelieerdePersoonID op!</remarks>
        public IEnumerable<PersoonInfo> ZoekenOpNaamVoornaamBegin(int groepID, string teZoeken)
        {
            var groep = _groepenRepo.ByID(groepID);

            if (!_autorisatieMgr.IsGav(groep))
            {
                throw FaultExceptionHelper.GeenGav();
            }

	    if (String.IsNullOrEmpty(teZoeken))
	    {
		    return new List<PersoonInfo>();
	    }

            var personen = (from gp in groep.GelieerdePersoon
                            where String.Format("{0} {1}", gp.Persoon.Naam, gp.Persoon.VoorNaam).StartsWith(teZoeken, StringComparison.InvariantCultureIgnoreCase)
                            || String.Format("{1} {0}", gp.Persoon.Naam, gp.Persoon.VoorNaam).StartsWith(teZoeken, StringComparison.InvariantCultureIgnoreCase)
                            select gp).ToList();

            return Mapper.Map<IList<GelieerdePersoon>, List<PersoonInfo>>(personen);
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
            var groep = _groepenRepo.ByID(groepID);

            if (!_autorisatieMgr.IsGav(groep))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var adres = _adressenRepo.ByID(adresID);

            var personen = (from pa in adres.PersoonsAdres
                                    where pa.Persoon.GelieerdePersoon.Any(gp => Equals(gp.Groep, groep))
                                    select pa.Persoon).ToList();

            // Belangrijk: selecteer alleen de gelieerde personen uit de gevraagde groep!
            var gelieerdePersonen = personen.SelectMany(persoon => persoon.GelieerdePersoon, (persoon, gelieerdePersoon) => gelieerdePersoon)
                                            .Where(gelieerdePersoon => Equals(gelieerdePersoon.Groep, groep)).ToList();

            var resultaat = new GezinInfo();

            Mapper.Map(adres, resultaat);
            resultaat.Bewoners = Mapper.Map<IList<GelieerdePersoon>, IList<BewonersInfo>>(gelieerdePersonen);

            return resultaat;
        }

        /// <summary>
        /// Gegeven een gelieerde persoon met gegeven <paramref name="gelieerdePersoonID"/>, haal al diens
        /// huisgenoten uit zijn eigen groep op.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van GelieerdePersoon</param>
        /// <returns>Lijst met Personen uit dezelfde groep die huisgenoot zijn van gegeven
        /// persoon</returns>
        /// <remarks>Parameters: GELIEERDEpersoonID, returns PERSONEN</remarks>
        public List<BewonersInfo> HuisGenotenOphalenZelfdeGroep(int gelieerdePersoonID)
        {
            var gelieerdePersoon = _gelieerdePersonenRepo.ByID(gelieerdePersoonID);

            if (!_autorisatieMgr.IsGav(gelieerdePersoon))
            {
                throw FaultExceptionHelper.GeenGav();
            }
            
	    // Hieronder stond eerst tussen 'PersoonsAdres.' en 'SelectMany...' 
	    // nog ".Select(pa => pa.Persoon)" nog tussen,
            // waardoor twee keer dezelfde persoon weergegegeven werd.
            var huisGenoten =
                (from gp in
                     gelieerdePersoon.Persoon.PersoonsAdres.Select(pa => pa.Adres).SelectMany(p => p.PersoonsAdres).Select(pa => pa.Persoon).SelectMany(p => p.GelieerdePersoon)
                 where Equals(gp.Groep, gelieerdePersoon.Groep)
                 select gp).Distinct().ToList();

            if (!huisGenoten.Any())
            {
                // Als er nog geen adressen zijn, dan zijn er ook geen huisgenoten.
                // In dat geval leveren we gewoon de originele gelieerde persoon op.

                huisGenoten = new List<GelieerdePersoon> { gelieerdePersoon };
            }

            return Mapper.Map<IList<GelieerdePersoon>, List<BewonersInfo>>(huisGenoten);
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
            var communicatieVorm = _communicatieVormRepo.ByID(commvormID);
            if (!_autorisatieMgr.IsGav(communicatieVorm))
            {
                throw FaultExceptionHelper.GeenGav();
            }
            return Mapper.Map<CommunicatieVorm, CommunicatieDetail>(communicatieVorm);
        }

        /// <summary>
        /// Wijzigt het nummer van de communicatievorm met gegeven <paramref name="ID"/>
        /// naar <paramref name="waarde"/>.
        /// </summary>
        /// <param name="ID">CommunicatieVormID</param>
        /// <param name="waarde">Nieuw nummer</param>
        public void NummerCommunicatieVormWijzigen(int ID, string waarde)
        {
            var communicatieVorm = _communicatieVormRepo.ByID(ID);
            if (!_autorisatieMgr.IsGav(communicatieVorm))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            if (!_communicatieVormenMgr.IsGeldig(waarde, communicatieVorm.CommunicatieType))
            {
                throw new FoutNummerException(FoutNummer.ValidatieFout, string.Format(Resources.OngeldigeCommunicatie,
                                                           waarde,
                                                           communicatieVorm.CommunicatieType.Omschrijving));
            }

            string origineelNummer = communicatieVorm.Nummer;
            communicatieVorm.Nummer = waarde;

            // Niet vergeten te bewaren en te syncen
#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
                _communicatieVormRepo.SaveChanges();
                if (communicatieVorm.GelieerdePersoon.Persoon.InSync)
                {
                    _communicatieSync.Bijwerken(communicatieVorm, origineelNummer);
                }
#if KIPDORP
                tx.Complete();
            }
#endif
        }

        /// <summary>
        /// Schrijft een communicatievorm in of uit voor de snelleberichtgenlijsten
        /// </summary>
        /// <param name="communicatieVormID">ID in/uit te schrijven communicatievorm</param>
        /// <param name="inschrijven"><c>true</c> voor inschrijven, <c>false</c> voor uitschrijven.</param>
        public void SnelleBerichtenInschrijven(int communicatieVormID, bool inschrijven)
        {
            var communicatieVorm = _communicatieVormRepo.ByID(communicatieVormID);
            if (!_autorisatieMgr.IsGav(communicatieVorm))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            communicatieVorm.IsVoorOptIn = inschrijven;
#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
                _communicatieVormRepo.SaveChanges();
                if (communicatieVorm.GelieerdePersoon.Persoon.InSync)
                {
                    // het nummer veranderde niet.
                    _communicatieSync.Bijwerken(communicatieVorm, communicatieVorm.Nummer);
                }
#if KIPDORP
            tx.Complete();
            }
#endif
        }

        #endregion

        #region aanmaken (wordt niet gesynct)

        /// <summary>
        /// Maakt een nieuwe persoon aan, met adres, e-mailadres en telefoonnummer, en maakt de persoon
        /// desgevallend ook lid.
        /// </summary>
        /// <param name="details">details voor de aanmaak van de persoon</param>
        /// <param name="groepID">ID van de groep waaraan de persoon gekoppeld moet worden</param>
        /// <param name="forceer">Als <c>true</c>, doe dan ook verder als er al een gelijkaardige persoon bestaat</param>
        /// <returns>ID en GelieerdePersoonID van de nieuwe persoon</returns>
        public IDPersEnGP Nieuw(NieuwePersoonDetails details, int groepID, bool forceer)
        {
            // We weten dat we hier met een volledig nieuw persoon te maken hebben. 
            // We moeten dus niet syncen tot op het moment dat we weten dat hij/zij lid 
            // moet worden.

            Lid lid = null;
            GelieerdePersoon gelieerdePersoon;
            Adres adres = null;

            var problemen = new Dictionary<string, FoutBericht>();

            var groep = _groepenRepo.ByID(groepID);

            if (!_autorisatieMgr.IsGav(groep))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            // Persoon maken

            if (details.PersoonInfo.GelieerdePersoonID != 0 || details.PersoonInfo.AdNummer != null)
            {
                throw FaultExceptionHelper.FoutNummer(FoutNummer.AlgemeneFout, Resources.IdsNietWijzigen);
            }

            var nieuwePersoon = new Persoon
                                {
                                    AdNummer = null,    // een nieuwe persoon heeft geen AD-nummer
                                    VoorNaam = details.PersoonInfo.VoorNaam,
                                    Naam = details.PersoonInfo.Naam,
                                    Geslacht = details.PersoonInfo.Geslacht,
                                    GeboorteDatum = details.PersoonInfo.GeboorteDatum
                                };


            try
            {
                gelieerdePersoon = _gelieerdePersonenMgr.Toevoegen(nieuwePersoon, groep, 0, forceer);
                gelieerdePersoon.ChiroLeefTijd = details.PersoonInfo.ChiroLeefTijd;
            }
            catch (BlokkerendeObjectenException<GelieerdePersoon> ex)
            {
                // als er een gelijkaardige persoon bestaat, en we forceren niet, dan throwen we deze

                throw FaultExceptionHelper.Blokkerend(
                    Mapper.Map<IList<GelieerdePersoon>, List<PersoonDetail>>(ex.Objecten), ex.Message);
            }

            // Telefoonnummer en e-mailadres koppelen

            if (details.EMail != null)
            {
                var eMail = new CommunicatieVorm()
                            {
                                ID = 0, // nieuw e-mailadres
                                CommunicatieType = _communicatieTypesRepo.ByID((int) CommunicatieTypeEnum.Email),
                                IsGezinsgebonden = details.EMail.IsGezinsGebonden,
                                IsVoorOptIn = details.EMail.IsVoorOptIn,
                                Nota = details.EMail.Nota,
                                Nummer = details.EMail.Nummer,
                                Voorkeur = details.EMail.Voorkeur
                            };

                // Communicatie koppelen is een beetje een gedoe, omdat je die voorkeuren hebt, en het concept
                // 'gezinsgebonden' dat eigenlijk niet helemaal klopt. Al die brol handelen we af in de manager.

                try
                {
                    _communicatieVormenMgr.Koppelen(gelieerdePersoon, eMail);
                }
                catch (FoutNummerException ex)
                {
                    if (ex.FoutNummer == FoutNummer.ValidatieFout)
                    {
                        problemen.Add("EMail.Nummer", new FoutBericht
                                                      {
                                                          FoutNummer = FoutNummer.ValidatieFout,
                                                          Bericht =
                                                              string.Format(Resources.OngeldigeCommunicatie,
                                                                  details.EMail.Nummer,
                                                                  eMail.CommunicatieType.Omschrijving)
                                                      });
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            if (details.TelefoonNummer != null)
            {
                var telefoonNummer = new CommunicatieVorm()
                                     {
                                         ID = 0, // nieuw e-mailadres
                                         CommunicatieType =
                                             _communicatieTypesRepo.ByID((int) CommunicatieTypeEnum.TelefoonNummer),
                                         IsGezinsgebonden = details.TelefoonNummer.IsGezinsGebonden,
                                         IsVoorOptIn = details.TelefoonNummer.IsVoorOptIn,
                                         Nota = details.TelefoonNummer.Nota,
                                         Nummer = details.TelefoonNummer.Nummer,
                                         Voorkeur = details.TelefoonNummer.Voorkeur
                                     };


                try
                {
                    _communicatieVormenMgr.Koppelen(gelieerdePersoon, telefoonNummer);
                }
                catch (FoutNummerException ex)
                {
                    if (ex.FoutNummer == FoutNummer.ValidatieFout)
                    {
                        problemen.Add("TelefoonNummer.Nummer", new FoutBericht
                                                               {
                                                                   FoutNummer = FoutNummer.ValidatieFout,
                                                                   Bericht =
                                                                       string.Format(Resources.OngeldigeCommunicatie,
                                                                           details.TelefoonNummer.Nummer,
                                                                           telefoonNummer.CommunicatieType.Omschrijving)
                                                               });
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // adres koppelen

            if (details.Adres != null)
            {
                try
                {
                    adres = _adressenMgr.ZoekenOfMaken(details.Adres, _adressenRepo.Select(), _straatNamenRepo.Select(),
                        _woonPlaatsenRepo.Select(), _landenRepo.Select());
                }
                catch (OngeldigObjectException ex)
                {
                    adres = null;
                    foreach (KeyValuePair<string, FoutBericht> kvp in ex.Berichten)
                    {
                        problemen.Add(kvp.Key, kvp.Value);
                    }
                }

                if (adres != null)
                {
                    _gelieerdePersonenMgr.AdresToevoegen(new List<GelieerdePersoon> {gelieerdePersoon}, adres,
                        details.AdresType, true);
                }
            }

            // Zo nodig lid maken.

            if (details.InschrijvenAls != LidType.Geen)
            {
                if (groep.StopDatum != null && groep.StopDatum < DateTime.Now)
                {
                    problemen.Add("InschrijvenAls",
                        new FoutBericht {FoutNummer = FoutNummer.GroepInactief, Bericht = Resources.GroepInactief});
                }
                else
                {
                    var gwj = _groepenMgr.HuidigWerkJaar(groep);
                    var lidVoorstel = new LidVoorstel
                                      {
                                          AfdelingsJaren =
                                              _afdelingsJarenRepo.ByIDs(details.AfdelingsJaarIDs),
                                          LeidingMaken = details.InschrijvenAls == LidType.Leiding,
                                          GelieerdePersoon = gelieerdePersoon,
                                          GroepsWerkJaar = gwj
                                      };

                    // Een nieuwe persoon kan nog niet ingeschreven zijn. Gemakkelijk.

                    try
                    {
                        lid = _ledenMgr.NieuwInschrijven(lidVoorstel, false);
                        gelieerdePersoon.Persoon.InSync = true;
                    }
                    catch (FoutNummerException ex)
                    {
                        switch (ex.FoutNummer)
                        {
                            case FoutNummer.LidTypeVerkeerd:
                            case FoutNummer.LidTeJong:
                            case FoutNummer.AfdelingKindVerplicht:
                            case FoutNummer.LeidingTeJong:
                                // TODO: backendinformatie naar frontend om rechtsteeks te tonen:
                                // geen goed idee.
                                problemen.Add("InschrijvenAls",
                                    new FoutBericht {Bericht = ex.Message, FoutNummer = ex.FoutNummer});
                                lid = null;
                                break;
                            case FoutNummer.GeboorteDatumOntbreekt:
                                problemen.Add("NieuwePersoon.GeboorteDatum",
                                    new FoutBericht
                                    {
                                        Bericht = Resources.GeboorteDatumOntbreekt,
                                        FoutNummer = ex.FoutNummer
                                    });
                                break;
                            case FoutNummer.OnbekendGeslacht:
                                problemen.Add("NieuwePersoon.Geslacht",
                                    new FoutBericht {Bericht = ex.Message, FoutNummer = ex.FoutNummer});
                                lid = null;
                                break;
                            case FoutNummer.AdresOntbreekt:
                                problemen.Add("PostNr", new FoutBericht {Bericht = Properties.Resources.AdresOntbreekt});
                                lid = null;
                                break;
                            case FoutNummer.TelefoonNummerOntbreekt:
                                problemen.Add("TelefoonNummer.Nummer", new FoutBericht {Bericht = Properties.Resources.WaaromTelefoonNummer});
                                lid = null;
                                break;
                            case FoutNummer.EMailVerplicht:
                                problemen.Add("Email.Nummer", new FoutBericht {Bericht = Properties.Resources.WaaromEmail});
                                lid = null;
                                break;
                            default:
                                throw;
                        }
                    }
                }
            }

            if (problemen.Count > 0)
            {
                throw FaultExceptionHelper.Ongeldig(problemen);
            }

            if (lid == null)
            {
                _gelieerdePersonenRepo.SaveChanges();
            }
            else
            {
#if KIPDORP
                using (var tx = new TransactionScope())
                {
#endif
                    // eerst savechanges, en dan lid bewaren
                    // op die manier hebben we het gapID van de gelieerde persoon.

                    _gelieerdePersonenRepo.SaveChanges();
                    _ledenSync.Bewaren(lid);
#if KIPDORP
                tx.Complete();
                }
#endif
            }
            return new IDPersEnGP {GelieerdePersoonID = gelieerdePersoon.ID, PersoonID = gelieerdePersoon.Persoon.ID};
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

            return new IDPersEnGP { GelieerdePersoonID = resultaat.ID, PersoonID = resultaat.Persoon.ID };
        }

        #endregion

        #region categorieen (worden niet gesynct)

        /// <summary>
        /// Voegt een collectie gelieerde personen op basis van hun ID toe aan een collectie categorieën
        /// </summary>
        /// <param name="gelieerdepersonenIDs">ID's van de gelieerde personen</param>
        /// <param name="categorieIDs">ID's van de categorieën waaraan ze toegevoegd moeten worden</param>
        public void CategorieKoppelen(IList<int> gelieerdepersonenIDs, IList<int> categorieIDs)
        {
            var gelieerdePersonen = _gelieerdePersonenRepo.ByIDs(gelieerdepersonenIDs);

            var groepen = gelieerdePersonen.Select(gp => gp.Groep).Distinct().ToList();

            if (!_autorisatieMgr.IsGav(groepen))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            if (groepen.Count > 1)
            {
                // Een categorie is gekoppeld aan 1 groep. Als de personen uit meerdere groepen komen,
                // dan is er zeker een persoon waaraan de categorie niet gekoppeld kan worden.
                // (pigeon hole princplie)
                throw FaultExceptionHelper.FoutNummer(FoutNummer.CategorieNietVanGroep,
                                                      Properties.Resources.FouteCategorieVoorGroep);
            }

            var categorieen = (from c in groepen.First().Categorie
                               where categorieIDs.Contains(c.ID)
                               select c).ToList();

            if (categorieen.Count != categorieIDs.Count)
            {
                // Categorie niet gevonden -> vermoedelijk niet gekoppeld aan groep
                throw FaultExceptionHelper.FoutNummer(FoutNummer.CategorieNietVanGroep,
                                                      Properties.Resources.FouteCategorieVoorGroep);
            }

            foreach (var c in categorieen)
            {
                foreach (var gp in gelieerdePersonen)
                {
                    c.GelieerdePersoon.Add(gp);
                }
            }

            _gelieerdePersonenRepo.SaveChanges();
        }

        /// <summary>
        /// Haalt een collectie gelieerde personen uit de opgegeven categorie
        /// </summary>
        /// <param name="gelieerdepersonenIDs">ID's van de gelieerde personen over wie het gaat</param>
        /// <param name="categorieID">ID van de categorie waaruit ze verwijderd moeten worden</param>
        public void UitCategorieVerwijderen(IList<int> gelieerdepersonenIDs, int categorieID)
        {
            var categorie = _categorieenRepo.ByID(categorieID);

            if (!_autorisatieMgr.IsGav(categorie))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var gelieerdePersonen = (from gp in categorie.GelieerdePersoon
                                     where gelieerdepersonenIDs.Contains(gp.ID)
                                     select gp).ToList();

            if (gelieerdePersonen.Count != gelieerdepersonenIDs.Count)
            {
                throw FaultExceptionHelper.FoutNummer(FoutNummer.CategorieNietGekoppeld,
                                                      Properties.Resources.CategorieNietGekoppeld);
            }

            foreach (var gp in gelieerdePersonen)
            {
                categorie.GelieerdePersoon.Remove(gp);
            }

            _categorieenRepo.SaveChanges();
        }

        #endregion

        #region te syncen updates

        /// <summary>
        /// Updatet een bestaand persoon op basis van <paramref name="wijzigingen"/>
        /// </summary>
        /// <param name="wijzigingen">De velden die niet <c>null</c> zijn, bevatten de toe te passen wijzigingen.
        /// </param>
        /// <returns>GelieerdePersoonID van de bewaarde persoon</returns>
        /// <remarks>We hebben hier een issue als er informatie verwijderd moet worden. Ik zeg maar iets, geboorte-
        /// of sterfdatum. Misschien moet dit toch maar aangepast worden zodanig dat alles wordt bewaard, i.e.
        /// als een value <c>null</c> is, wordt de oorspronkelijke waarde overschreven door <c>null</c>.</remarks>
        public int Wijzigen(PersoonsWijziging wijzigingen)
        {
            var gp = _gelieerdePersonenRepo.ByID(wijzigingen.GelieerdePersoonID);

            if (gp == null || !_autorisatieMgr.IsGav(gp))
            {
                throw FaultExceptionHelper.GeenGav();
            }
            if (wijzigingen.ChiroLeefTijd.HasValue)
            {
                gp.ChiroLeefTijd = wijzigingen.ChiroLeefTijd.Value;
            }
            if (wijzigingen.GeboorteDatum.HasValue)
            {
                gp.Persoon.GeboorteDatum = wijzigingen.GeboorteDatum;
            }
            if (wijzigingen.Geslacht.HasValue)
            {
                gp.Persoon.Geslacht = wijzigingen.Geslacht.Value;
            }
            if (wijzigingen.Naam != null)
            {
                gp.Persoon.Naam = wijzigingen.Naam;
            }
            if (wijzigingen.SterfDatum != null)
            {
                gp.Persoon.SterfDatum = wijzigingen.SterfDatum;
            }
            if (wijzigingen.VersieString != null)
            {
                gp.Persoon.VersieString = wijzigingen.VersieString;
            }
            if (wijzigingen.VoorNaam != null)
            {
                gp.Persoon.VoorNaam = wijzigingen.VoorNaam;
            }

#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
            _gelieerdePersonenRepo.SaveChanges();
            if (gp.Persoon.InSync)
            {
                _personenSync.Bewaren(gp, false, false);
            }
#if KIPDORP   
            tx.Complete();
            }
#endif
            return gp.ID;
        }

        /// <summary>
        /// Verhuist gelieerde personen van een oud naar een nieuw adres
        /// (De koppelingen Persoon-Oudadres worden aangepast 
        /// naar Persoon-NieuwAdres.)
        /// </summary>
        /// <param name="gelieerdePersoonIDs">ID's van te verhuizen *GELIEERDE* Personen </param>
        /// <param name="nieuwAdresInfo">AdresInfo-object met nieuwe adresgegevens</param>
        /// <param name="oudAdresID">ID van het oude adres</param>
        /// <remarks>De ID van <paramref name="nieuwAdresInfo"/> wordt genegeerd.  Het adresID wordt altijd
        /// opnieuw opgezocht in de bestaande adressen.  Bestaat het adres nog niet,
        /// dan krijgt het adres een nieuw ID.</remarks>
        public void GelieerdePersonenVerhuizen(IEnumerable<int> gelieerdePersoonIDs, PersoonsAdresInfo nieuwAdresInfo, int oudAdresID)
        {
            var oudAdres = _adressenRepo.ByID(oudAdresID);
            Adres nieuwAdres;
            IList<PersoonsAdres> persoonsAdressen;

            try
            {
                nieuwAdres = _adressenMgr.ZoekenOfMaken(nieuwAdresInfo, _adressenRepo.Select(),
                                                        _straatNamenRepo.Select(), _woonPlaatsenRepo.Select(),
                                                        _landenRepo.Select());
            }
            catch (OngeldigObjectException ex)
            {
                throw FaultExceptionHelper.Ongeldig(ex.Berichten);
            }

            if (nieuwAdres.ID == oudAdresID)
            {
                if (nieuwAdres is BelgischAdres)
                {
                    // Vang situatie op dat enkel gemeente verandert (en postnummer hetzelfde blijft). ZoekenOfMaken
                    // levert dan het bestaande adres op, en we moeten dan vermijden dat er met de wijziging niets
                    // gebeurt.

                    var adres = nieuwAdres as BelgischAdres;

                    var nieuweWoonPlaats = (from wp in _woonPlaatsenRepo.Select()
                        where wp.PostNummer == nieuwAdresInfo.PostNr && wp.Naam == nieuwAdresInfo.WoonPlaatsNaam
                        select wp).FirstOrDefault();

                    adres.WoonPlaats = nieuweWoonPlaats;
                    _adressenRepo.SaveChanges();
                    return;
                }
                // Buitenlandse adressen worden enkel als gelijk beschouwd als alle velden overeenkomen, dus inclusief
                // woonplaats. Als we hier terechtkomen, is bron- en doeladres dus sowieso identiek, en hoeven we
                // niets meer te doen.

                return; 
            }

            var verhuizers = (from pa in oudAdres.PersoonsAdres
                              where pa.Persoon.GelieerdePersoon.Any(gp => gelieerdePersoonIDs.Contains(gp.ID))
                              select pa.Persoon).ToList();

            // een beetje dom. Ik selecteer nu de personen via de persoonsadressen, en straks in
            // personenMgr.Verhuizen worden van die personen opnieuw de persoonsadressen
            // opgezocht. Misschien nog wel eens te herwerken.

            if (!_autorisatieMgr.IsGav(verhuizers))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            try
            {
                persoonsAdressen = _personenMgr.Verhuizen(verhuizers, oudAdres, nieuwAdres, nieuwAdresInfo.AdresType);
            }
            catch (BlokkerendeObjectenException<PersoonsAdres> ex)
            {
                throw FaultExceptionHelper.Blokkerend(
                    Mapper.Map<IList<PersoonsAdres>, List<PersoonsAdresInfo2>>(ex.Objecten),
                    Resources.WoontDaarAl);

                // Dit kan nog wel wat verfijnd worden.
            }

            // de persoonsadressen gekoppeld aan een gelieerde persoon, zijn de voorkeursadresen van die gelieerde persoon.
            var teSyncen = (from pa in persoonsAdressen
                            where pa.GelieerdePersoon.Any(gp => gp.Persoon.InSync)
                            select pa).ToList();


#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
            _adressenSync.StandaardAdressenBewaren(teSyncen);

            _adressenRepo.SaveChanges();

#if KIPDORP
            tx.Complete();
            }
#endif
        }

        /// <summary>
        /// Voegt een adres toe aan een verzameling *GELIEERDE* personen
        /// </summary>
        /// <param name="gelieerdePersonenIDs">ID's van de gelieerde personen
        /// waaraan het nieuwe adres toegevoegd moet worden.</param>
        /// <param name="adr">Toe te voegen adres</param>
        /// <param name="voorkeur"><c>True</c> als het nieuwe adres het voorkeursadres moet worden.</param>
        public void AdresToevoegenGelieerdePersonen(IList<int> gelieerdePersonenIDs, PersoonsAdresInfo adr, bool voorkeur)
        {
            Adres adres;
            IList<PersoonsAdres> nieuwePersoonsAdressen;

            var gelieerdePersonen = _gelieerdePersonenRepo.ByIDs(gelieerdePersonenIDs);

            if (!_autorisatieMgr.IsGav(gelieerdePersonen))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            try
            {
                adres = _adressenMgr.ZoekenOfMaken(adr, _adressenRepo.Select(), _straatNamenRepo.Select(),
                                                   _woonPlaatsenRepo.Select(),
                                                   _landenRepo.Select());
            }
            catch (OngeldigObjectException ex)
            {
                // fout in straatnaam, postnr of gemeente
                throw FaultExceptionHelper.Ongeldig(ex.Berichten);
            }

            try
            {
                nieuwePersoonsAdressen = _gelieerdePersonenMgr.AdresToevoegen(gelieerdePersonen, adres, adr.AdresType, voorkeur);
            }
            catch (BlokkerendeObjectenException<PersoonsAdres> ex)
            {
                // persoon blabla woont al op dat adres
                throw FaultExceptionHelper.Blokkerend(
                    Mapper.Map<IList<PersoonsAdres>, List<PersoonsAdresInfo2>>(ex.Objecten), ex.Message);
            }

            var teSyncen = (from pa in nieuwePersoonsAdressen
                            where pa.GelieerdePersoon.Any(gp => gp.Persoon.InSync)
                            select pa).ToList();
                            
#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
                if (teSyncen.Any())
                {
                    _adressenSync.StandaardAdressenBewaren(teSyncen);
                }
                _gelieerdePersonenRepo.SaveChanges();
#if KIPDORP
                tx.Complete();
            }
#endif          
        }

        /// <summary>
        /// Verwijdert een adres van een verzameling personen
        /// </summary>
        /// <param name="personenIDs">ID's van de personen over wie het gaat</param>
        /// <param name="adresID">ID van het adres dat losgekoppeld moet worden</param>
        public void AdresVerwijderenVanPersonen(IList<int> personenIDs, int adresID)
        {
            var adres = _adressenRepo.ByID(adresID);
            var teSyncen = new List<PersoonsAdres>();

            var teVerwijderen = (from pa in adres.PersoonsAdres
                                 where personenIDs.Contains(pa.Persoon.ID)
                                 select pa).ToList();

            if (!_autorisatieMgr.IsGav(teVerwijderen))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            // Van welke gelieerde personen is het te verwijderen adres het voorkeursadres?
            var thuisLozen = teVerwijderen.SelectMany(pa => pa.GelieerdePersoon).ToList();

            foreach (var gp in thuisLozen)
            {
                // kies een willekeurig nieuw adres. (null als er geen meer is)

                var oudVoorkeurAdres = gp.PersoonsAdres;
                var nieuwVoorkeurAdres = (from pa in gp.Persoon.PersoonsAdres
                                          where !Equals(pa, oudVoorkeurAdres)
                                          select pa).FirstOrDefault();

                gp.PersoonsAdres = nieuwVoorkeurAdres;
                if (gp.Persoon.InSync)
                {
                    if (nieuwVoorkeurAdres != null)
                    {
                        teSyncen.Add(nieuwVoorkeurAdres);
                    }
                    else
                    {
                        // TODO (#1647): feit dat voorkeursadres vervalt syncen naar Kipadmin
                    }
                }
            }

#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
                if (teSyncen.Any())
                {
                    _adressenSync.StandaardAdressenBewaren(teSyncen);
                }
                _persoonsAdressenRepo.Delete(teVerwijderen);
                _persoonsAdressenRepo.SaveChanges();
#if KIPDORP
                tx.Complete();
            }
#endif
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
            var gelieerdePersoon = _gelieerdePersonenRepo.ByID(gelieerdePersoonID);

            if (!_autorisatieMgr.IsGav(gelieerdePersoon))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var persoonsAdres = (from pa in gelieerdePersoon.Persoon.PersoonsAdres
                                 where pa.ID == persoonsAdresID
                                 select pa).FirstOrDefault();

            if (persoonsAdres == null)
            {
                throw FaultExceptionHelper.FoutNummer(FoutNummer.AdresNietGekoppeld, Resources.AdresNietGekoppeld);
            }

            gelieerdePersoon.PersoonsAdres = persoonsAdres;

#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
                if (gelieerdePersoon.Persoon.InSync)
                {
                    _adressenSync.StandaardAdressenBewaren(new List<PersoonsAdres>{persoonsAdres});
                }
                _gelieerdePersonenRepo.SaveChanges();
#if KIPDORP
                tx.Complete();
            }
#endif
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

            List<CommunicatieVorm> gekoppeld;

            try
            {
                // Communicatievormen koppelen is een beetje een gedoe, omdat je die voorkeuren hebt,
                // en het concept 'gezinsgebonden' (wat eigenlijk niet helemaal klopt)
                // Al die brol handelen we af in de manager.

                gekoppeld = _communicatieVormenMgr.Koppelen(gelieerdePersoon, communicatieVorm);
            }
            catch (FoutNummerException ex)
            {
                if (ex.FoutNummer == FoutNummer.ValidatieFout)
                {
                    throw FaultExceptionHelper.FoutNummer(ex.FoutNummer, ex.Message);
                }
                // Van validatieexcpetion maken we een faultexception.
                // Eender welke andere exception throwen we opnieuw.
                throw;
            }
            var tesyncen = (from cv in gekoppeld
                            where
                                cv.GelieerdePersoon.Persoon.InSync
                            select cv).ToList();

#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
                    _gelieerdePersonenRepo.SaveChanges();
                    // TODO (#1409): welke communicatievorm de voorkeur heeft, gaat verloren bij de sync
                    // naar Kipadmin. 
                    foreach (var cv in tesyncen)
                    {
                        _communicatieSync.Toevoegen(cv);
                    }
#if KIPDORP   
                    tx.Complete();
            }
#endif

        }

        /// <summary>
        /// Verwijdert een communicatievorm van een gelieerde persoon
        /// </summary>
        /// <param name="commvormID">ID van de communicatievorm</param>
        /// <returns>De ID van de gelieerdepersoon die bij de commvorm hoort</returns>
        public int CommunicatieVormVerwijderen(int commvormID)
        {
            var communicatieVorm = _communicatieVormRepo.ByID(commvormID);
            int gelieerdePersoonID = communicatieVorm.GelieerdePersoon.ID;

            if (!_autorisatieMgr.IsGav(communicatieVorm))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            if (communicatieVorm.Voorkeur)
            {
                // We verwijderen de voorkeurscommunicatie. Zoek een andere om voorkeur te maken.
                var nieuweVoorkeur = (from cv in communicatieVorm.GelieerdePersoon.Communicatie
                                      where
                                          !Equals(cv, communicatieVorm) &&
                                          Equals(cv.CommunicatieType, communicatieVorm.CommunicatieType)
                                      select cv).FirstOrDefault();
                if (nieuweVoorkeur != null)
                {
                    nieuweVoorkeur.Voorkeur = true;
                    // TODO: syncen naar Kipadmin
                }

            }

#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
                if (communicatieVorm.GelieerdePersoon.Persoon.InSync)
                {
                    _communicatieSync.Verwijderen(communicatieVorm);
                }
                _communicatieVormRepo.Delete(communicatieVorm);
                _communicatieVormRepo.SaveChanges();
#if KIPDORP
                tx.Complete();
            }
#endif

            return gelieerdePersoonID;
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
                if (communicatieVorm.GelieerdePersoon.Persoon.InSync)
                {
                    _communicatieSync.Bijwerken(communicatieVorm, origineelNummer);
                }
#if KIPDORP
            tx.Complete();
            }
#endif
        }

        #endregion
    }
}

