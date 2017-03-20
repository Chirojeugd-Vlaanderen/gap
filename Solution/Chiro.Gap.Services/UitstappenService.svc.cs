/*
 * Copyright 2008-2015 the GAP developers. See the NOTICE file at the
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.Services.Properties;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;

#if KIPDORP
using System.Transactions;
#endif

namespace Chiro.Gap.Services
{
    /// <summary>
    /// Service methods m.b.t. uitstappen
    /// </summary>
    public class UitstappenService : BaseService, IUitstappenService, IDisposable
    {
        // Repositories, verantwoordelijk voor data access
        private readonly IRepositoryProvider _repositoryProvider;
        private readonly IRepository<GroepsWerkJaar> _groepsWerkJaarRepo;
        private readonly IRepository<Groep> _groepenRepo;
        private readonly IRepository<Adres> _adressenRepo;
        private readonly IRepository<Uitstap> _uitstappenRepo;
        private readonly IRepository<GelieerdePersoon> _gelieerdePersonenRepo;
        private readonly IRepository<Deelnemer> _deelnemersRepo;
        private readonly IRepository<StraatNaam> _straatNamenRepo;
        private readonly IRepository<WoonPlaats> _woonPlaatsenRepo;
        private readonly IRepository<Land> _landenRepo;

        // Managers voor niet-triviale businesslogica

        private readonly IUitstappenManager _uitstappenMgr;
        private readonly IAdressenManager _adressenMgr;

        // Sync

        private readonly IBivakSync _bivakSync;

        // rariteit

        private readonly GavChecker _gav;

        /// <summary>
        /// Constructor, verantwoordelijk voor dependency injection
        /// </summary>
        /// <param name="repositoryProvider">De repositoryprovider zal de nodige repositories opleveren</param>
        /// <param name="autorisatieManager">Businesslogica m.b.t. autorisatie</param>
        /// <param name="uitstappenManager">Businesslogica m.b.t. uitstappen</param>
        /// <param name="adressenManager">Businesslogica m.b.t. adressen</param>
        /// <param name="ledenManager">Businesslogica m.b.t. leden</param>
        /// <param name="groepsWerkJarenManager">Businesslogica m.b.t. groepswerkjaren</param>
        /// <param name="authenticatieManager">Businesslogica m.b.t. authenticatie</param>
        /// <param name="abonnementenManager">Businesslogica m.b.t. abonnementen</param>
        /// <param name="bivakSync"></param>
        public UitstappenService(IRepositoryProvider repositoryProvider,
                                 IAutorisatieManager autorisatieManager,
                                 IUitstappenManager uitstappenManager,
                                 IAdressenManager adressenManager,
                                 ILedenManager ledenManager,
                                 IGroepsWerkJarenManager groepsWerkJarenManager,
                                 IAuthenticatieManager authenticatieManager,
                                 IAbonnementenManager abonnementenManager,
                                 IBivakSync bivakSync): base(ledenManager, groepsWerkJarenManager, authenticatieManager, autorisatieManager, abonnementenManager)
        {
            _repositoryProvider = repositoryProvider;
            _groepsWerkJaarRepo = repositoryProvider.RepositoryGet<GroepsWerkJaar>();
            _groepenRepo = repositoryProvider.RepositoryGet<Groep>();
            _adressenRepo = repositoryProvider.RepositoryGet<Adres>();
            _uitstappenRepo = repositoryProvider.RepositoryGet<Uitstap>();
            _gelieerdePersonenRepo = repositoryProvider.RepositoryGet<GelieerdePersoon>();
            _deelnemersRepo = repositoryProvider.RepositoryGet<Deelnemer>();
            _straatNamenRepo = repositoryProvider.RepositoryGet<StraatNaam>();
            _woonPlaatsenRepo = repositoryProvider.RepositoryGet<WoonPlaats>();
            _landenRepo = repositoryProvider.RepositoryGet<Land>();

            _uitstappenMgr = uitstappenManager;
            _adressenMgr = adressenManager;

            _bivakSync = bivakSync;

            _gav = new GavChecker(_autorisatieMgr);
        }

        public GavChecker Gav
        {
            get { return _gav; }
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

        ~UitstappenService()
        {
            Dispose(false);
        }

        #endregion


        #region ophalen
        /// <summary>
        /// Haalt alle uitstappen van een gegeven groep op.
        /// </summary>
        /// <param name="groepId">ID van de groep</param>
        /// <param name="inschrijvenMogelijk">Als deze <c>true</c> is, worden enkel de uitstappen opgehaald
        /// waarvoor je nog kunt inschrijven.  In praktijk zijn dit de uitstappen van het huidige werkjaar.
        /// </param>
        /// <returns>Details van uitstappen</returns>
        /// <remarks>We laten toe om inschrijvingen te doen voor uitstappen uit het verleden, om als dat
        /// nodig is achteraf fouten in de administratie recht te zetten.</remarks>
        // applying PrincipalPermission at class level doesn't seem to work for a WCF service.
        [PrincipalPermission(SecurityAction.Demand, Role = @"GapServiceConsumers")]
        public IEnumerable<UitstapInfo> OphalenVanGroep(int groepId, bool inschrijvenMogelijk)
        {
            IEnumerable<Uitstap> resultaat;

            var groep = (from g in _groepenRepo.Select()
                         where g.ID == groepId
                         select g).FirstOrDefault();

            if (!_autorisatieMgr.IsGav(groep))
            {
                throw FaultExceptionHelper.GeenGav();
            }


            if (inschrijvenMogelijk)
            {
                // Enkel uitstappen van recentste groepswerkjaar
                var groepsWerkJaar = _groepsWerkJaarRepo.Select()
                                                        .Where(gwj => gwj.Groep.ID == groepId)
                                                        .OrderByDescending(gwj => gwj.WerkJaar)
                                                        .First();

                resultaat = groepsWerkJaar.Uitstap.ToArray();
            }
            else
            {
                // Alle uitstappen ophalen
                resultaat = _uitstappenRepo.Select().Where(u => u.GroepsWerkJaar.Groep.ID == groepId).ToList();
            }

            return _mappingHelper.Map<IEnumerable<Uitstap>, IEnumerable<UitstapInfo>>(resultaat);
        }

        /// <summary>
        /// Haalt details over uitstap met gegeven <paramref name="uitstapId"/> op.
        /// </summary>
        /// <param name="uitstapId">ID van de uitstap</param>
        /// <returns>Details over de uitstap</returns>
        // applying PrincipalPermission at class level doesn't seem to work for a WCF service.
        [PrincipalPermission(SecurityAction.Demand, Role = @"GapServiceConsumers")]
        public UitstapOverzicht DetailsOphalen(int uitstapId)
        {
            var uitstap = _uitstappenRepo.ByID(uitstapId);
            Gav.Check(uitstap);
            return _mappingHelper.Map<Uitstap, UitstapOverzicht>(uitstap);
        }

        /// <summary>
        /// Haalt informatie over de deelnemer met ID <paramref name="deelnemerId"/> op.
        /// </summary>
        /// <param name="deelnemerId">ID van de relevante deelnemer</param>
        /// <returns>Informatie over de deelnemer met ID <paramref name="deelnemerId"/></returns>
        // applying PrincipalPermission at class level doesn't seem to work for a WCF service.
        [PrincipalPermission(SecurityAction.Demand, Role = @"GapServiceConsumers")]
        public DeelnemerDetail DeelnemerOphalen(int deelnemerId)
        {
            var resultaat = _deelnemersRepo.Select().FirstOrDefault(dln => dln.ID == deelnemerId);
            Gav.Check(resultaat.GelieerdePersoon);
			      Gav.Check(resultaat.Uitstap);
            return _mappingHelper.Map<Deelnemer, DeelnemerDetail>(resultaat);
        }

        /// <summary>
        /// Haalt informatie over alle deelnemers van de uitstap met gegeven <paramref name="uitstapId"/> op.
        /// </summary>
        /// <param name="uitstapId">ID van de relevante uitstap</param>
        /// <returns>Informatie over alle deelnemers van de uitstap met gegeven <paramref name="uitstapId"/></returns>
        // applying PrincipalPermission at class level doesn't seem to work for a WCF service.
        [PrincipalPermission(SecurityAction.Demand, Role = @"GapServiceConsumers")]
        public IEnumerable<DeelnemerDetail> DeelnemersOphalen(int uitstapId)
        {
            var uitstap = _uitstappenRepo.ByID(uitstapId);
            Gav.Check(uitstap);

            return _mappingHelper.Map<IEnumerable<Deelnemer>, IEnumerable<DeelnemerDetail>>(uitstap.Deelnemer);
        }

        /// <summary>
        /// Haalt informatie over de bivakaangifte op van de groep <paramref name="groepId"/> voor diens recentste
        /// werkJaar.
        /// </summary>
        /// <param name="groepId">
        /// De groep waarvan info wordt gevraagd
        /// </param>
        /// <returns>
        /// Een lijstje met de geregistreerde bivakken en feedback over wat er op dit moment moet gebeuren
        /// voor de bivakaangifte
        /// </returns>
        // applying PrincipalPermission at class level doesn't seem to work for a WCF service.
        [PrincipalPermission(SecurityAction.Demand, Role = @"GapServiceConsumers")]
        public BivakAangifteLijstInfo BivakStatusOphalen(int groepId)
        {
            var resultaat = new BivakAangifteLijstInfo();

            var gwjQuery = _groepsWerkJaarRepo.Select();

            var groepsWerkJaar =
                gwjQuery.Where(gwj => gwj.Groep.ID == groepId).OrderByDescending(gwj => gwj.WerkJaar).FirstOrDefault();

            if (groepsWerkJaar == null || !_autorisatieMgr.IsGav(groepsWerkJaar))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            if (!_uitstappenMgr.BivakAangifteVanBelang(groepsWerkJaar))
            {
                resultaat.AlgemeneStatus = BivakAangifteStatus.NogNietVanBelang;
            }
            else
            {
                resultaat.Bivakinfos = (from u in groepsWerkJaar.Uitstap
                                        where u.IsBivak
                                        select
                                            new BivakAangifteInfo
                                            {
                                                ID = u.ID,
                                                Omschrijving = u.Naam,
                                                Status = _uitstappenMgr.StatusBepalen(u)
                                            }).ToList();

                if (resultaat.Bivakinfos.FirstOrDefault() == null)
                {
                    resultaat.AlgemeneStatus = BivakAangifteStatus.Ontbrekend;
                }
                else if (resultaat.Bivakinfos.Any(bi => bi.Status != BivakAangifteStatus.Ok))
                {
                    resultaat.AlgemeneStatus = BivakAangifteStatus.Ontbrekend;
                }
                else
                {
                    resultaat.AlgemeneStatus = BivakAangifteStatus.Ok;
                }
            }

            return resultaat;
        }
        #endregion

        #region te syncen wijzigingen
        /// <summary>
        /// Bewaart een uitstap voor de groep met gegeven <paramref name="groepId"/>
        /// </summary>
        /// <param name="groepId">ID van de groep horende bij de uitstap.
        ///  Is eigenlijk enkel relevant als het om een nieuwe uitstap gaat.</param>
        /// <param name="info">Details over de uitstap.  Als <c>uitstap.ID</c> <c>0</c> is,
        ///  dan wordt een nieuwe uitstap gemaakt.  Anders wordt de bestaande overschreven.</param>
        /// <returns>ID van de uitstap</returns>
        /// <remark>De contactdeelnemer zit niet bij in <paramref name="info"/>, daar wordt dus
        /// niets mee gedaan.</remark>
        // applying PrincipalPermission at class level doesn't seem to work for a WCF service.
        [PrincipalPermission(SecurityAction.Demand, Role = @"GapServiceConsumers")]
        public int Bewaren(int groepId, UitstapInfo info)
        {
            // Als de uitstap een ID heeft, moet een bestaande uitstap opgehaald worden.
            // Anders maken we een nieuwe.

            Uitstap uitstap;
            bool wasBivak = false;

            var groepsWerkJaar = _groepsWerkJaarRepo.Select()
                                                    .Where(gwj => gwj.Groep.ID == groepId)
                                                    .OrderByDescending(gwj => gwj.WerkJaar)
                                                    .First();

            if (!_autorisatieMgr.IsGav(groepsWerkJaar.Groep))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            if (info.ID == 0)
            {
                // Nieuwe uitstap
                uitstap = _mappingHelper.Map<UitstapInfo, Uitstap>(info);
                uitstap.GroepsWerkJaar = groepsWerkJaar;
                groepsWerkJaar.Uitstap.Add(uitstap);
            }
            else
            {
                // Haal origineel op, gekoppeld aan groepswerkjaar
                uitstap = (from u in groepsWerkJaar.Uitstap
                           where u.ID == info.ID
                           select u).First();
                wasBivak = uitstap.IsBivak;

                // overschrijf met gegevens uit 'info'
                _mappingHelper.Map(info, uitstap);
            }

#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
                _groepsWerkJaarRepo.SaveChanges();

                if (uitstap.IsBivak)
                {
                    _bivakSync.Bewaren(uitstap);
                }
                else if (wasBivak)
                {
                    _bivakSync.Verwijderen(uitstap.ID);
                }

#if KIPDORP
                tx.Complete();
            }
#endif
            return uitstap.ID;
        }

        /// <summary>
        /// Bewaart de plaats voor een uitstap
        /// </summary>
        /// <param name="uitstapId">ID van de uitstap</param>
        /// <param name="plaatsNaam">Naam van de plaats</param>
        /// <param name="adresInfo">Adres van de plaats</param>
        // applying PrincipalPermission at class level doesn't seem to work for a WCF service.
        [PrincipalPermission(SecurityAction.Demand, Role = @"GapServiceConsumers")]
        public void PlaatsBewaren(int uitstapId, string plaatsNaam, AdresInfo adresInfo)
        {
            var uitstap = _uitstappenRepo.ByID(uitstapId);
            Gav.Check(uitstap);
            var groep = uitstap.GroepsWerkJaar.Groep;

            // zoek of maak adres
            var adres = _adressenMgr.ZoekenOfMaken(adresInfo, _adressenRepo.Select(), _straatNamenRepo.Select(), _woonPlaatsenRepo.Select(), _landenRepo.Select());

            // zoek plaats
            var plaats = (from p in adres.BivakPlaats
                          where String.Equals(p.Naam, plaatsNaam, StringComparison.OrdinalIgnoreCase)
                          && Equals(p.Groep, groep)
                          select p).FirstOrDefault();

            if (plaats == null)
            {
                // als niet gevonden: maak
                plaats = new Plaats {Naam = plaatsNaam, Adres = adres, Groep = groep};
                groep.BivakPlaats.Add(plaats);
                adres.BivakPlaats.Add(plaats);
            }

            // koppelen
            uitstap.Plaats = plaats;
            plaats.Uitstap.Add(uitstap);

#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
                if (uitstap.IsBivak)
                {
                    _bivakSync.Bewaren(uitstap);
                }
                _uitstappenRepo.SaveChanges();
#if KIPDORP
                tx.Complete();
            }
#endif
        }

        /// <summary>
        /// Verwijdert een uitstap met als ID <paramref name="uitstapId"/>
        /// </summary>
        /// <param name="uitstapId">ID van de te verwijderen uitstap</param>
        /// <returns>Verwijderd de uitstap en toont daarna het overzicht scherm</returns>
        // applying PrincipalPermission at class level doesn't seem to work for a WCF service.
        [PrincipalPermission(SecurityAction.Demand, Role = @"GapServiceConsumers")]
        public void UitstapVerwijderen(int uitstapId)
        {
            var uitstap = _uitstappenRepo.ByID(uitstapId);
            Gav.Check(uitstap);

            if (uitstap != null)
            {
                _uitstappenRepo.Delete(uitstap);
#if KIPDORP
                using (var tx = new TransactionScope())
                {
#endif
                    if (uitstap.IsBivak)
                    {
                        _bivakSync.Verwijderen(uitstap.ID);
                    }
                    _uitstappenRepo.SaveChanges();
#if KIPDORP
                    tx.Complete();
                }
#endif
            }
        }

        /// <summary>
        /// Stelt de deelnemer met gegeven <paramref name="deelnemerId" /> in als contactpersoon voor de uitstap
        /// waaraan hij deelneemt
        /// </summary>
        /// <param name="deelnemerId">ID van de als contact in te stellen deelnemer</param>
        /// <returns>De ID van de uitstap, ter controle, en misschien handig voor feedback</returns>
        // applying PrincipalPermission at class level doesn't seem to work for a WCF service.
        [PrincipalPermission(SecurityAction.Demand, Role = @"GapServiceConsumers")]
        public int ContactInstellen(int deelnemerId)
        {
            var deelnemer = _deelnemersRepo.ByID(deelnemerId);
            Gav.Check(deelnemer);

            Debug.Assert(deelnemer.Uitstap != null);

            if (deelnemer.UitstapWaarvoorVerantwoordelijk.FirstOrDefault() == null)
            {
                // Een deelnemer kan alleen contact zijn voor zijn eigen uitstap.  Is de deelnemer
                // al contact voor een uitstap, dan volgt daaruit dat hij al contact is voor zijn
                // eigen uitstap.
                var vorigeVerantwoordelijke = deelnemer.Uitstap.ContactDeelnemer;

                if (vorigeVerantwoordelijke != null)
                {
                    vorigeVerantwoordelijke.UitstapWaarvoorVerantwoordelijk = null;
                }

                deelnemer.Uitstap.ContactDeelnemer = deelnemer;
                deelnemer.UitstapWaarvoorVerantwoordelijk.Add(deelnemer.Uitstap);
            }

#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
                // Let op: eerst kipsync, en dan SaveChanges. Want het kan zijn dat
                // KipSync de vlag 'AdNrInAanvraag' voor de contactpersoon zet.

                if (deelnemer.Uitstap.IsBivak)
                {
                    deelnemer.GelieerdePersoon.Persoon.InSync = true;
                    _bivakSync.Bewaren(deelnemer.Uitstap);
                }
                _deelnemersRepo.SaveChanges();
#if KIPDORP
                tx.Complete();
            }
#endif
            return deelnemer.Uitstap.ID;
        }
        #endregion

        #region wijzigingen niet te syncen
        /// <summary>
        /// Schrijft de gelieerde personen met ID's <paramref name="gelieerdePersoonIDs"/> in voor de
        /// uitstap met ID <paramref name="geselecteerdeUitstapId" />.  Als
        /// <paramref name="logistiekDeelnemer" /> <c>true</c> is, wordt er ingeschreven als
        /// logistiek deelnemer.
        /// </summary>
        /// <param name="gelieerdePersoonIDs">ID's van in te schrijven gelieerde personen</param>
        /// <param name="geselecteerdeUitstapId">ID van uitstap waarvoor in te schrijven</param>
        /// <param name="logistiekDeelnemer">Bepaalt of al dan niet ingeschreven wordt als
        /// logistieker</param>
        /// <returns>De basisgegevens van de uitstap, zodat die in de feedback gebruikt kan worden</returns>
        // applying PrincipalPermission at class level doesn't seem to work for a WCF service.
        [PrincipalPermission(SecurityAction.Demand, Role = @"GapServiceConsumers")]
        public UitstapInfo Inschrijven(IList<int> gelieerdePersoonIDs, int geselecteerdeUitstapId,
                                       bool logistiekDeelnemer)
        {
            var uitstap = _uitstappenRepo.ByID(geselecteerdeUitstapId);
            Gav.Check(uitstap);

            var gelieerdePersonen = _gelieerdePersonenRepo.ByIDs(gelieerdePersoonIDs);

            if (!_autorisatieMgr.IsGav(gelieerdePersonen))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var groepen = (from gp in gelieerdePersonen select gp.Groep).Distinct().ToList();

            Debug.Assert(groepen.Any()); // De gelieerde personen moeten aan een groep gekoppeld zijn.
            Debug.Assert(uitstap.GroepsWerkJaar != null);
            Debug.Assert(uitstap.GroepsWerkJaar.Groep != null);

            // Als er meer dan 1 groep is, dan is er minstens een groep verschillend van de groep
            // van de uitstap (duivenkotenprincipe));););
            if (groepen.Count() > 1 || groepen.First().ID != uitstap.GroepsWerkJaar.Groep.ID)
            {
                throw new FoutNummerException(
                    FoutNummer.UitstapNietVanGroep,
                    Resources.FoutieveGroepUitstap);
            }

            // Koppel enkel de gelieerde personen die nog niet aan de uitstap gekoppeld zijn
            foreach (var gp in gelieerdePersonen.Where(gp => gp.Deelnemer.All(d => d.Uitstap.ID != uitstap.ID)))
            {
                var deelnemer = new Deelnemer
                    {
                        GelieerdePersoon = gp,
                        Uitstap = uitstap,
                        HeeftBetaald = false,
                        IsLogistieker = logistiekDeelnemer,
                        MedischeFicheOk = false
                    };

                // Moet dat nu nog alle twee gebeuren?
                gp.Deelnemer.Add(deelnemer);
                uitstap.Deelnemer.Add(deelnemer);
            }

            _gelieerdePersonenRepo.SaveChanges();
            return _mappingHelper.Map<Uitstap, UitstapInfo>(uitstap);
        }

        /// <summary>
        /// Schrijft de deelnemer met gegeven <paramref name="deelnemerId"/> uit voor zijn uitstap.
        /// </summary>
        /// <param name="deelnemerId">ID uit te schrijven deelnemer</param>
        /// <returns>ID van de uitstap, ter controle, en handig voor feedback</returns>
        // applying PrincipalPermission at class level doesn't seem to work for a WCF service.
        [PrincipalPermission(SecurityAction.Demand, Role = @"GapServiceConsumers")]
        public int Uitschrijven(int deelnemerId)
        {
            var deelnemer = _deelnemersRepo.ByID(deelnemerId);
            Gav.Check(deelnemer);
            var uitstap= deelnemer.Uitstap;

            // Als die deelnemer contactpersoon is, moet eerst de contactpersoon van de uitstap verwijderd worden.
            if (Equals(uitstap.ContactDeelnemer, deelnemer))
            {
                uitstap.ContactDeelnemer = null;
            }

            uitstap.Deelnemer.Remove(deelnemer);
            deelnemer.GelieerdePersoon.Deelnemer.Remove(deelnemer);
            _deelnemersRepo.Delete(deelnemer);

            _deelnemersRepo.SaveChanges();

            return uitstap.ID;
        }

        /// <summary>
        /// Updatet een deelnemer op basis van de info in <paramref name="info"/>
        /// </summary>
        /// <param name="info">Info nodig voor de update</param>
        // applying PrincipalPermission at class level doesn't seem to work for a WCF service.
        [PrincipalPermission(SecurityAction.Demand, Role = @"GapServiceConsumers")]
        public void DeelnemerBewaren(DeelnemerInfo info)
        {
            var deelnemer = _deelnemersRepo.ByID(info.DeelnemerID);
            Gav.Check(deelnemer);

            // Oorspronkelijke deelnemer ophalen
            deelnemer = _deelnemersRepo.Select().First(dln => dln.ID == info.DeelnemerID);

            // Nieuwe waarden invullen en opslaan
            _mappingHelper.Map(info, deelnemer);
            _deelnemersRepo.SaveChanges();
        }
        #endregion
    }
}
