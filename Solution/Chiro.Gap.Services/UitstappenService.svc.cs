// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

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
using Chiro.Gap.Services.Properties;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.Services
{
    /// <summary>
    /// Service methods m.b.t. uitstappen
    /// </summary>
    public class UitstappenService : IUitstappenService, IDisposable
    {
        // Repositories, verantwoordelijk voor data access
        private readonly IRepository<GroepsWerkJaar> _groepsWerkJaarRepo;
        private readonly IRepository<Groep> _groepenRepo;
        private readonly IRepository<Adres> _adressenRepo;
        private readonly IRepository<Uitstap> _uitstappenRepo;
        private readonly IRepository<GelieerdePersoon> _gelieerdePersonenRepo;
        private readonly IRepository<Deelnemer> _deelnemersRepo;

        // Managers voor niet-triviale businesslogica

        private readonly IAutorisatieManager _autorisatieMgr;
        private readonly IUitstappenManager _uitstappenMgr;
        private readonly IAdressenManager _adressenMgr;

                private readonly GavChecker _gav;

        /// <summary>
        /// Constructor, verantwoordelijk voor dependency injection
        /// </summary>
        /// <param name="repositoryProvider">De repositoryprovider zal de nodige repositories opleveren</param>
        /// <param name="autorisatieManager">Businesslogica m.b.t. autorisatie</param>
        /// <param name="uitstappenManager">Businesslogica m.b.t. uitstappen</param>
        /// <param name="adressenManager">Businesslogica m.b.t. adressen</param>
        public UitstappenService(IRepositoryProvider repositoryProvider,
                                 IAutorisatieManager autorisatieManager,
                                 IUitstappenManager uitstappenManager,
                                 IAdressenManager adressenManager)
        {
            _groepsWerkJaarRepo = repositoryProvider.RepositoryGet<GroepsWerkJaar>();
            _groepenRepo = repositoryProvider.RepositoryGet<Groep>();
            _adressenRepo = repositoryProvider.RepositoryGet<Adres>();
            _uitstappenRepo = repositoryProvider.RepositoryGet<Uitstap>();
            _gelieerdePersonenRepo = repositoryProvider.RepositoryGet<GelieerdePersoon>();
            _deelnemersRepo = repositoryProvider.RepositoryGet<Deelnemer>();

            _autorisatieMgr = autorisatieManager;
            _uitstappenMgr = uitstappenManager;
            _adressenMgr = adressenManager;

            _gav = new GavChecker(_autorisatieMgr);
        }

        public GavChecker Gav
        {
            get { return _gav; }
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
                    _groepsWerkJaarRepo.Dispose();

                }
                disposed = true;
            }
        }

        ~UitstappenService()
        {
            Dispose(false);
        }

        #endregion

        /// <summary>
        /// Bewaart een uitstap voor de groep met gegeven <paramref name="groepId"/>
        /// </summary>
        /// <param name="groepId">ID van de groep horende bij de uitstap.
        ///  Is eigenlijk enkel relevant als het om een nieuwe uitstap gaat.</param>
        /// <param name="info">Details over de uitstap.  Als <c>uitstap.ID</c> <c>0</c> is,
        ///  dan wordt een nieuwe uitstap gemaakt.  Anders wordt de bestaande overschreven.</param>
        /// <returns>ID van de uitstap</returns>
        public int Bewaren(int groepId, UitstapInfo info)
        {
            // Als de uitstap een ID heeft, moet een bestaande uitstap opgehaald worden.
            // Anders maken we een nieuwe.

            Uitstap uitstap;

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
                uitstap = Mapper.Map<UitstapInfo, Uitstap>(info);
                _uitstappenMgr.Koppelen(uitstap, groepsWerkJaar);
            }
            else
            {
                // Haal origineel op, gekoppeld aan groepswerkjaar
                uitstap = (from u in groepsWerkJaar.Uitstap
                           where u.ID == info.ID
                           select u).First();

                // overschrijf met gegevens uit 'info'
                Mapper.Map(info, uitstap);
            }

            _context.SaveChanges();
            return uitstap.ID;
        }

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

            return Mapper.Map<IEnumerable<Uitstap>, IEnumerable<UitstapInfo>>(resultaat);
        }

        /// <summary>
        /// Haalt details over uitstap met gegeven <paramref name="uitstapId"/> op.
        /// </summary>
        /// <param name="uitstapId">ID van de uitstap</param>
        /// <returns>Details over de uitstap</returns>
        public UitstapOverzicht DetailsOphalen(int uitstapId)
        {
            var uitstap = _uitstappenRepo.ByID(uitstapId);
            Gav.Check(uitstap);
            return Mapper.Map<Uitstap, UitstapOverzicht>(uitstap);
        }

        /// <summary>
        /// Bewaart de plaats voor een uitstap
        /// </summary>
        /// <param name="uitstapId">ID van de uitstap</param>
        /// <param name="plaatsNaam">Naam van de plaats</param>
        /// <param name="adresInfo">Adres van de plaats</param>
        public void PlaatsBewaren(int uitstapId, string plaatsNaam, AdresInfo adresInfo)
        {
            var uitstap = _uitstappenRepo.ByID(uitstapId);
            Gav.Check(uitstap);

            uitstap.Plaats.Naam = plaatsNaam;
            uitstap.Plaats.Adres = _adressenMgr.ZoekenOfMaken(adresInfo, _adressenRepo.Select());

            _context.SaveChanges();
        }

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
        public UitstapInfo Inschrijven(IList<int> gelieerdePersoonIDs, int geselecteerdeUitstapId,
                                       bool logistiekDeelnemer)
        {
            var uitstap = _uitstappenRepo.ByID(geselecteerdeUitstapId);
            Gav.Check(uitstap);

            var alleGpIDs = gelieerdePersoonIDs.Distinct().ToList();
            var mijnGpIDs = _autorisatieMgr.EnkelMijnGelieerdePersonen(alleGpIDs);

            if (alleGpIDs.Count() != mijnGpIDs.Count())
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var gelieerdePersonen = _gelieerdePersonenRepo.Select().Where(gp => gelieerdePersoonIDs.Contains(gp.ID)).ToList();

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

            _context.SaveChanges();
            return Mapper.Map<Uitstap, UitstapInfo>(uitstap);
        }

        /// <summary>
        /// Haalt informatie over alle deelnemers van de uitstap met gegeven <paramref name="uitstapId"/> op.
        /// </summary>
        /// <param name="uitstapId">ID van de relevante uitstap</param>
        /// <returns>Informatie over alle deelnemers van de uitstap met gegeven <paramref name="uitstapId"/></returns>
        public IEnumerable<DeelnemerDetail> DeelnemersOphalen(int uitstapId)
        {
            var uitstap = _uitstappenRepo.ByID(uitstapId);
            Gav.Check(uitstap);

            return Mapper.Map<IEnumerable<Deelnemer>, IEnumerable<DeelnemerDetail>>(uitstap.Deelnemer);
        }

        /// <summary>
        /// Verwijdert een uitstap met als ID <paramref name="uitstapId"/>
        /// </summary>
        /// <param name="uitstapId">ID van de te verwijderen uitstap</param>
        /// <returns>Verwijderd de uitstap en toont daarna het overzicht scherm</returns>
        public void UitstapVerwijderen(int uitstapId)
        {
            var uitstap = _uitstappenRepo.ByID(uitstapId);
            Gav.Check(uitstap);

            if (uitstap != null)
            {
                _uitstappenRepo.Delete(uitstap);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Stelt de deelnemer met gegeven <paramref name="deelnemerId" /> in als contactpersoon voor de uitstap
        /// waaraan hij deelneemt
        /// </summary>
        /// <param name="deelnemerId">ID van de als contact in te stellen deelnemer</param>
        /// <returns>De ID van de uitstap, ter controle, en misschien handig voor feedback</returns>
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

            return deelnemer.Uitstap.ID;
        }

        /// <summary>
        /// Schrijft de deelnemer met gegeven <paramref name="deelnemerId"/> uit voor zijn uitstap.
        /// </summary>
        /// <param name="deelnemerId">ID uit te schrijven deelnemer</param>
        /// <returns>ID van de uitstap, ter controle, en handig voor feedback</returns>
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
            _context.SaveChanges();

            return uitstap.ID;
        }

        /// <summary>
        /// Haalt informatie over de deelnemer met ID <paramref name="deelnemerId"/> op.
        /// </summary>
        /// <param name="deelnemerId">ID van de relevante deelnemer</param>
        /// <returns>Informatie over de deelnemer met ID <paramref name="deelnemerId"/></returns>
        public DeelnemerDetail DeelnemerOphalen(int deelnemerId)
        {
            var resultaat = _deelnemersRepo.Select().FirstOrDefault(dln => dln.ID == deelnemerId);
            return Mapper.Map<Deelnemer, DeelnemerDetail>(resultaat);
        }

        /// <summary>
        /// Updatet een deelnemer op basis van de info in <paramref name="info"/>
        /// </summary>
        /// <param name="info">Info nodig voor de update</param>
        public void DeelnemerBewaren(DeelnemerInfo info)
        {
            var deelnemer = _deelnemersRepo.ByID(info.DeelnemerID);
            Gav.Check(deelnemer);

            // Oorspronkelijke deelnemer ophalen
            deelnemer = _deelnemersRepo.Select().First(dln => dln.ID == info.DeelnemerID);

            // Nieuwe waarden invullen en opslaan
            Mapper.Map(info, deelnemer);
            _context.SaveChanges();
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
    }
}
