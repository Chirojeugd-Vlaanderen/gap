﻿// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using AutoMapper;
using Chiro.Cdf.Poco;


using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.Services
{
    // OPM: als je de naam van de class "LedenService" hier verandert, moet je ook de sectie "Services" in web.config aanpassen.

    // *BELANGRIJK*: Als het debuggen hier stopt owv een autorisatiefout, kijk dan na of de gebruiker waarmee
    // je aangemeld bent, op je lokale computer in de groep CgUsers zit.

    /// <summary>
    /// Service voor operaties op leden en leiding
    /// </summary>
    public class LedenService : ILedenService, IDisposable
    {

        // Repositories, verantwoordelijk voor data access.

        private readonly IRepository<Lid> _ledenRepo;
        private readonly IRepository<VerzekeringsType> _verzekerRepo;
        private readonly IRepository<GelieerdePersoon> _gelieerdePersonenRepo;
        private readonly IRepository<AfdelingsJaar> _afdelingsJaarRepo;
        private readonly IRepository<Functie> _functiesRepo;
        private readonly IRepository<GroepsWerkJaar> _groepsWerkJarenRepo;

        // Managers voor niet-triviale businesslogica

        private readonly IAutorisatieManager _autorisatieMgr;
        private readonly IVerzekeringenManager _verzekeringenMgr;
        private readonly ILedenManager _ledenMgr;
        private readonly IGroepsWerkJarenManager _groepsWerkJarenMgr;
        private readonly IGroepenManager _groepenMgr;
        private readonly IFunctiesManager _functiesMgr;

        // Sync

        private readonly ILedenSync _ledenSync;

        private readonly GavChecker _gav;

        /// <summary>
        /// Nieuwe ledenservice
        /// </summary>
        /// <param name="autorisatieMgr">Verantwoordelijke voor autorisatie</param>
        /// <param name="verzekeringenMgr">Businesslogica aangaande verzekeringen</param>
        /// <param name="ledenMgr">Businesslogica aangaande leden</param>
        /// <param name="groepsWerkJarenMgr">Businesslogica wat betreft groepswerkjaren</param>
        /// <param name="groepenMgr">Businesslogica m.b.t. groepen</param>
        /// <param name="functiesMgr">Businesslogica m.b.t. functies</param>
        /// <param name="repositoryProvider">De repository provider levert alle nodige repository's op.</param>
        /// <param name="ledenSync">Voor synchronisatie lidgegevens met Kipadmin</param>
        public LedenService(IAutorisatieManager autorisatieMgr,
                            IVerzekeringenManager verzekeringenMgr,
                            ILedenManager ledenMgr, IGroepsWerkJarenManager groepsWerkJarenMgr,
                            IGroepenManager groepenMgr, IFunctiesManager functiesMgr,
                            IRepositoryProvider repositoryProvider, ILedenSync ledenSync)
        {
            _ledenRepo = repositoryProvider.RepositoryGet<Lid>();
            _afdelingsJaarRepo = repositoryProvider.RepositoryGet<AfdelingsJaar>();
            _functiesRepo = repositoryProvider.RepositoryGet<Functie>();
            _groepsWerkJarenRepo = repositoryProvider.RepositoryGet<GroepsWerkJaar>();
            _verzekerRepo = repositoryProvider.RepositoryGet<VerzekeringsType>();
            _gelieerdePersonenRepo = repositoryProvider.RepositoryGet<GelieerdePersoon>();

            _verzekeringenMgr = verzekeringenMgr;
            _ledenMgr = ledenMgr;
            _groepsWerkJarenMgr = groepsWerkJarenMgr;
            _autorisatieMgr = autorisatieMgr;
            _groepenMgr = groepenMgr;
            _functiesMgr = functiesMgr;

            _ledenSync = ledenSync;

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
                    _ledenRepo.Dispose();
                    _verzekerRepo.Dispose();
                    _gelieerdePersonenRepo.Dispose();
                    _afdelingsJaarRepo.Dispose();
                    _functiesRepo.Dispose();
                    _groepsWerkJarenRepo.Dispose();
                }
                disposed = true;
            }
        }

        ~LedenService()
        {
            Dispose(false);
        }

        #endregion

 
        /// <summary>
        /// Genereert de lijst van inteschrijven leden met de informatie die ze zouden krijgen als ze automagisch zouden worden ingeschreven, gebaseerd op een lijst van in te schrijven gelieerde personen.
        /// </summary>
        /// <param name="gelieerdePersoonIds">Lijst van gelieerde persoonIds waarover we inforamtie willen</param>
        /// <param name="foutBerichten">Als er sommige personen geen lid gemaakt werden, bevat foutBerichten een string waarin wat uitleg staat.</param>
        /// <returns>De LidIds van de personen die lid zijn gemaakt</returns>
        public List<InTeSchrijvenLid> VoorstelTotInschrijvenGenereren(IList<int> gelieerdePersoonIds, out string foutBerichten)
        {
            // TODO (#195): van onderstaande logica moet wel wat verhuizen naar de workers!
            // TODO (#1053): beter systeem bedenken voor feedback dan via foutBerichten.
            //      Foutberichten bevat nu een string die gewoon in de UI wordt geplakt, en dat breekt de seperation of concerns.

            foutBerichten = string.Empty;
            var foutBerichtenBuilder = new StringBuilder();

            var gelieerdePersonen = _gelieerdePersonenRepo.ByIDs(gelieerdePersoonIds);

            if (!_autorisatieMgr.IsGav(gelieerdePersonen) || gelieerdePersoonIds.Count != gelieerdePersonen.Count)
            {
                throw FaultExceptionHelper.GeenGav();
            }


            // We gaan ervan uit dat alle gelieerde personen op dit moment tot dezelfde groep behoren.
            // Maar in de toekomst is dat misschien niet meer zo. Dus laten we onderstaande constructie
            // maar staan.
            var groepen = (from gp in gelieerdePersonen select gp.Groep).Distinct();

            var voorgesteldelijst = new List<InTeSchrijvenLid>();

            foreach (var g in groepen)
            {
                var gwj = _groepenMgr.HuidigWerkJaar(g);

                foreach (var gp in gelieerdePersonen.Where(gelp => gelp.Groep.ID == g.ID).OrderByDescending(gp => gp.GebDatumMetChiroLeefTijd))
                {
                    if (_ledenMgr.IsActiefLid(gp))
                    {
                        foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.IsNogIngeschreven,
                                                                      gp.Persoon.VolledigeNaam));
                    }
                    else
                    {
                        try
                        {
                            var voorstel = _ledenMgr.InschrijvingVoorstellen(gp, gwj, true);

                            voorgesteldelijst.Add(new InTeSchrijvenLid
                                                      {
                                                          AfdelingsJaarIrrelevant = voorstel.AfdelingsJarenIrrelevant,
                                                          AfdelingsJaarIDs = voorstel.AfdelingsJaarIDs,
                                                          GelieerdePersoonID = gp.ID,
                                                          LeidingMaken = voorstel.LeidingMaken,
                                                          VolledigeNaam = gp.Persoon.VolledigeNaam
                                                      });
                        }
                        catch (GapException ex)
                        {
                            // TODO (#95): ex.Message, wat een bericht voor de programmeur moet zijn, wordt meegegeven met 'foutberichten', 
                            //      waarvan de inhoud rechtstreeks op de GUI getoond zal worden. Dit breekt de seperation of concerns.

                            foutBerichtenBuilder.AppendLine(String.Format("Fout voor {0}: {1}", gp.Persoon.VolledigeNaam,
                                                                          ex.Message));
                        }
                    }
                }
            }

            foutBerichten = foutBerichtenBuilder.ToString();

            return voorgesteldelijst;
        }

        /// <summary>
        /// Probeert de opgegeven personen in te schrijven met de meegegeven informatie. Als dit niet mogelijk blijkt te zijn, wordt er niemand ingeschreven.
        /// </summary>
        /// <param name="lidInformatie">Lijst van informatie over wie lid moet worden</param>
        /// <param name="foutBerichten">Als er sommige personen geen lid konden worden gemaakt, bevat foutBerichten een string waarin wat uitleg staat. </param>
        /// <returns>De LidIds van de personen die lid zijn gemaakt</returns>
        public IEnumerable<int> Inschrijven(InTeSchrijvenLid[] lidInformatie, out string foutBerichten)
        {
            foutBerichten = String.Empty;

            var teSyncen = new List<Lid>();

            var lidIDs = new List<int>();
            var foutBerichtenBuilder = new StringBuilder();

            // Haal meteen alle gelieerde personen op, samen met alle info die nodig is om het lid
            // over te zetten naar Kipadmin: groep, persoon, voorkeursadres

            var gelieerdePersonen = _gelieerdePersonenRepo.ByIDs(lidInformatie.Select(e => e.GelieerdePersoonID));

            if (!_autorisatieMgr.IsGav(gelieerdePersonen) || lidInformatie.Count() != gelieerdePersonen.Count)
            {
                throw FaultExceptionHelper.GeenGav();
            }


            // Mogelijk horen de gelieerde personen tot verschillende groepen.  Dat kan, als de GAV GAV is van
            // al die groepen. Als hij geen GAV is van de IDs, dan werd er al een exception gethrowd natuurlijk.
            var groepen = (from gp in gelieerdePersonen select gp.Groep).Distinct();

            foreach (var g in groepen)
            {
                // Per groep lid maken.
                // Zoek eerst recentste groepswerkjaar.
                var gwj = _groepenMgr.HuidigWerkJaar(g);

                foreach (var gp in gelieerdePersonen.Where(gelp => gelp.Groep.ID == g.ID))
                {
                    // TODO: Dit is te veel business. Bekijken of een lid al ingeschreven is, moet in de workers gebeuren.

                    // Behandel leden 1 voor 1 zodat een probleem met 1 lid niet verhindert dat de rest bewaard wordt.

                    // Kijk of het lid al bestaat (eventueel niet-actief).  In de meeste gevallen zal dit geen
                    // resultaat opleveren.  Als er toch al een lid is, worden persoon, voorkeursadres, officiele afdeling,
                    // functies ook opgehaald, omdat een eventueel geheractiveerd lid opnieuw naar Kipadmin zal moeten.

                    var l = gp.Lid.FirstOrDefault(ld => ld.GroepsWerkJaar.ID == gwj.ID);

                    if (l != null) // al ingeschreven
                    {
                        if (l.UitschrijfDatum == null)
                        {
                            // Al ingeschreven als actief lid; we doen er verder niets mee.
                            // (Behalve een foutbericht meegeven, wat ook niet echt correct is, zie #1053)

                            foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.IsNogIngeschreven,
                                                                          gp.Persoon.VolledigeNaam));
                        }
                        else
                        {
                            l.UitschrijfDatum = null;
                            l.NonActief = false;
                            teSyncen.Add(l);
                        }
                    }
                    else // nieuw lid
                    {
                        try
                        {
                            l = _ledenMgr.NieuwInschrijven(gp, gwj, false,
                                                           Mapper.Map<InTeSchrijvenLid, LidVoorstel>(
                                                               lidInformatie.First(e => e.GelieerdePersoonID == gp.ID)));
                            teSyncen.Add(l);
                        }
                        catch (BestaatAlException<Kind>)
                        {
                            foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.WasAlLid,
                                                                          gp.Persoon.VolledigeNaam));
                        }
                        catch (BestaatAlException<Leiding>)
                        {
                            foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.WasAlLeiding,
                                                                          gp.Persoon.VolledigeNaam));
                        }
                        catch (GapException ex)
                        {
                            foutBerichtenBuilder.AppendLine(String.Format("Fout voor {0}: {1}", gp.Persoon.VolledigeNaam,
                                                                          ex.Message));
                        }
                    }
                }
            }

            foutBerichten = foutBerichtenBuilder.ToString();

#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
                _ledenSync.Bewaren(teSyncen);
                _gelieerdePersonenRepo.SaveChanges();
#if KIPDORP
                tx.Commit();
            }
#endif
            

            return lidIDs;
        }

        /// <summary>
        /// Schrijft de leden met gegeven <paramref name="gelieerdePersoonIDs"/> uit voor het huidige
        /// werkjaar. We gaan ervan uit dat ze allemaal ingeschreven zijn.
        /// </summary>
        /// <param name="gelieerdePersoonIDs">Id's van de gelieerde personen</param>
        /// <param name="foutBerichten">Als voor sommige personen die actie mislukte, bevat foutBerichten een
        ///     string waarin wat uitleg staat.</param>
        public void Uitschrijven(IList<int> gelieerdePersoonIDs, out string foutBerichten)
        {
            // Deze code is tamelijk rommelig; gebruik ze niet als referentie-implementatie
            // (Ik ben er ook niet van overtuigd of het werken met 'foutBerichten' wel in orde is.)
            var teSyncen = new List<Lid>();

            var foutBerichtenBuilder = new StringBuilder();

            var gelieerdePersonen = _gelieerdePersonenRepo.ByIDs(gelieerdePersoonIDs);

            if (!_autorisatieMgr.IsGav(gelieerdePersonen) || gelieerdePersoonIDs.Count() != gelieerdePersonen.Count)
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var groepen = (from gp in gelieerdePersonen select gp.Groep).Distinct();

            foreach (var g in groepen)
            {
                var gwj = _groepenMgr.HuidigWerkJaar(g);

                // Handel per groep de uitschrijvingen af, zodat we per groep kunnen
                // controleren of de persoon wel ingeschreven is in het recentste groepswerkjaar.

                foreach (var gp in gelieerdePersonen.Where(gp => Equals(gp.Groep, g)))
                {
                    var lid = gp.Lid.FirstOrDefault(e => e.GroepsWerkJaar.ID == gwj.ID);

                    if (lid == null)
                    {
                        foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.IsNogNietIngeschreven,
                                                                      gp.Persoon.VolledigeNaam));
                        continue;
                    }
                    if (lid.NonActief)
                    {
                        foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.IsAlUitgeschreven,
                                                                      gp.Persoon.VolledigeNaam));
                        continue;
                    }

                    lid.UitschrijfDatum = DateTime.Now;
                    lid.Functie.Clear();

                    if (lid.EindeInstapPeriode > lid.UitschrijfDatum || lid.Niveau > Niveau.Groep)
                    {
                        teSyncen.Add(lid);
                    }
                }
            }

            foutBerichten = foutBerichtenBuilder.ToString();

#if KIPDORP
			using (var tx = new TransactionScope())
			{
#endif
                _gelieerdePersonenRepo.SaveChanges();
                foreach (var l in teSyncen)
                {
                    _ledenSync.Verwijderen(l);
                }
#if KIPDORP
				tx.Complete();
			}
#endif
        }

        /// <summary>
        /// Vervangt de functies van het lid bepaald door <paramref name="lidId"/> door de functies
        /// met Id's <paramref name="functieIds"/>
        /// </summary>
        /// <param name="lidId">Id van lid met te vervangen functies</param>
        /// <param name="functieIds">Ids van nieuwe functies voor het lid</param>
        public void FunctiesVervangen(int lidId, IEnumerable<int> functieIds)
        {
            var lid = _ledenRepo.ByID(lidId);
            Gav.Check(lid);

            var functies = _functiesRepo.ByIDs(functieIds);

            // TODO: optimaliseren. Tien tegen 1 zijn al die functies aan dezelfde groep gekoppeld.
            // In dat geval volstaat het om 1 check te doen, ipv een check per functie.
            // TIP: controleer GAV-schap functies.SelectMany(fn=>fn.Groep).Distinct().
            foreach (var functie in functies)
            {
                Gav.Check(functie);
            }

            try
            {
                _functiesMgr.Vervangen(lid, functies);
            }
            catch (FoutNummerException ex)
            {
                switch (ex.FoutNummer)
                {
                    case FoutNummer.GroepsWerkJaarNietBeschikbaar:
                    case FoutNummer.FunctieNietVanGroep:
                    case FoutNummer.FunctieNietBeschikbaar: 
                    case FoutNummer.LidTypeVerkeerd:
                        // Deze exceptions verwachten we; we sturen een foutnummerfault
                        throw FaultExceptionHelper.FoutNummer(ex.FoutNummer, ex.Message);
                    default:
                        // Onverwachte exception gooien we opnieuw op.
                        throw;
                }
            }

#if KIPDORP
			using (var tx = new TransactionScope())
			{
#endif
                _ledenRepo.SaveChanges();
                _ledenSync.FunctiesUpdaten(lid);
#if KIPDORP
				tx.Complete();
			}
#endif
           
        }

        /// <summary>
        /// Haalt summiere info van een lid met gegeven <paramref name="lidId"/> op.
        /// (naam van lid, afdeling, en lidtype)
        /// </summary>
        /// <param name="lidId">Id van het lid waarin we geinteresseerd zijn</param>
        /// <returns>naam, afdeling en lidtype van het gegeven lid</returns>
        public LidAfdelingInfo AfdelingenOphalen(int lidId)
        {
            var lid = _ledenRepo.ByID(lidId);
            Gav.Check(lid); // throwt als je geen rechten hebt.

            return Mapper.Map<Lid, LidAfdelingInfo>(lid);
        }

        /// <summary>
        /// Vervangt de afdelingen van het lid met Id <paramref name="lidId"/> door de afdelingen
        /// met AFDELINGSJAARIds gegeven door <paramref name="afdelingsJaarIds"/>.
        /// </summary>
        /// <param name="lidId">Lid dat nieuwe afdelingen moest krijgen</param>
        /// <param name="afdelingsJaarIds">Id's van de te koppelen afdelingsjaren</param>
        /// <returns>De GelieerdePersoonId van het lid</returns>
        public int AfdelingenVervangen(int lidId, IEnumerable<int> afdelingsJaarIds)
        {
            var lid = _ledenRepo.ByID(lidId);
            Gav.Check(lid);

            AfdelingenVervangenBulk(new List<int> { lidId }, afdelingsJaarIds);

            return lid.GelieerdePersoon.ID;
        }

        /// <summary>
        /// Vervangt de afdelingen van de leden met gegeven <paramref name="lidIds"/> door de afdelingen
        /// met AFDELINGSJAARIds gegeven door <paramref name="afdelingsJaarIds"/>.
        /// </summary>
        /// <param name="lidIds">Id's van leden die nieuwe afdelingen moeten krijgen</param>
        /// <param name="afdelingsJaarIds">Id's van de te koppelen afdelingsjaren</param>
        public void AfdelingenVervangenBulk(IEnumerable<int> lidIds, IEnumerable<int> afdelingsJaarIds)
        {
            var afdelingsJaren = (from g in afdelingsJaarIds select _afdelingsJaarRepo.ByID(g)).ToList();
            foreach (var afdelingsJaar in afdelingsJaren)
            {
                Gav.Check(afdelingsJaar);
            }

            foreach (var lidId in lidIds)
            {
                var lid = _ledenRepo.ByID(lidId);
                Gav.Check(lid);

                var kind = lid as Kind;
                if (kind != null)
                {
                    if (afdelingsJaren.Count != 1)
                    {
                        FaultExceptionHelper.FoutNummer(FoutNummer.AlgemeneKindFout, Properties.Resources.KindInEenAfdelingsJaar);
                    }
                    kind.AfdelingsJaar = afdelingsJaren.First();
                }
                else
                {
                    var leiding = lid as Leiding;
                    if (leiding != null)
                    {
                        leiding.AfdelingsJaar = afdelingsJaren;
                    }
                }
            }

            _ledenRepo.SaveChanges();
        }

        /// <summary>
        /// Verzekert lid met Id <paramref name="lidId"/> tegen loonverlies
        /// </summary>
        /// <param name="lidId">Id van te verzekeren lid</param>
        /// <returns>GelieerdePersoonId van het verzekerde lid</returns>
        /// <remarks>Dit is nogal een specifieke method.  In ons domain model is gegeven dat verzekeringen gekoppeld zijn aan
        /// personen, voor een bepaalde periode.  Maar in eerste instantie zal alleen de verzekering loonverlies gebruikt worden,
        /// die per definitie enkel voor leden bestaat.</remarks>
        public int LoonVerliesVerzekeren(int lidId)
        {
            var lid = _ledenRepo.ByID(lidId);
            Gav.Check(lid);

            var verzekeringstype = (from g in _verzekerRepo.Select() where g.ID == (int)Verzekering.LoonVerlies select g).First();

            try
            {
                _verzekeringenMgr.Verzekeren(lid, verzekeringstype, DateTime.Today,
                                             _groepsWerkJarenMgr.EindDatum(lid.GroepsWerkJaar));
            }
            catch (FoutNummerException ex)
            {
                throw FaultExceptionHelper.FoutNummer(ex.FoutNummer, ex.Message);
            }
            catch (BlokkerendeObjectenException<PersoonsVerzekering>)
            {
                // TODO: beter faultcontract. (VerzkeringsInfo?)
                throw FaultExceptionHelper.BestaatAl("Verzekering");
            }

            _ledenRepo.SaveChanges();
            return lid.GelieerdePersoon.ID;
        }

        /// <summary>
        /// Haalt actief lid op, inclusief gelieerde persoon, persoon, groep, afdelingen en functies
        /// </summary>
        /// <param name="lidId">Id op te halen lid</param>
        /// <returns>Lidinfo; bevat info over gelieerde persoon, persoon, groep, afdelingen en functies</returns>
        public PersoonLidInfo DetailsOphalen(int lidId)
        {
            var lid = _ledenRepo.ByID(lidId);
            Gav.Check(lid);
            if (lid.NonActief)
            {
                FaultExceptionHelper.GeenGav();
            }
            return Mapper.Map<Lid, PersoonLidInfo>(lid);
        }

        /// <summary></summary>
        /// <param name="filter">De niet-nulle properties van de filter
        /// bepalen waarop gezocht moet worden</param>
        /// <param name="metAdressen">Indien <c>true</c>, worden de
        /// adressen mee opgehaald. (Adressen ophalen vertraagt aanzienlijk.)
        /// </param>
        /// <returns>Lijst met info over gevonden leden</returns>
        /// <remarks>
        /// Er worden enkel actieve leden opgehaald.
        /// Let er ook op dat je in de filter iets opgeeft als LidType
        /// (Kind, Leiding of Alles), want anders krijg je niets terug.
        /// </remarks>
        public IList<LidOverzicht> Zoeken(LidFilter filter, bool metAdressen)
        {
            var leden = (from ld in _ledenRepo.Select()
                         where
                             ld.UitschrijfDatum == null &&
                             (filter.GroepID == null || ld.GroepsWerkJaar.Groep.ID == filter.GroepID) &&
                             (filter.GroepsWerkJaarID == null || ld.GroepsWerkJaar.ID == filter.GroepsWerkJaarID) &&
                             (filter.FunctieID == null || ld.Functie.Select(e => e.ID == filter.FunctieID).Any()) &&
                             (filter.ProbeerPeriodeNa == null || !ld.EindeInstapPeriode.HasValue ||
                              filter.ProbeerPeriodeNa < ld.EindeInstapPeriode.Value) &&
                             (filter.HeeftVoorkeurAdres == null || ld.GelieerdePersoon.PersoonsAdres != null) &&
                             (filter.HeeftTelefoonNummer == null ||
                              ld.GelieerdePersoon.Communicatie.Select(
                                  e => e.CommunicatieType.ID == (int) CommunicatieTypeEnum.TelefoonNummer).Any() ==
                              filter.HeeftTelefoonNummer) &&
                             (filter.HeeftEmailAdres == null ||
                              ld.GelieerdePersoon.Communicatie.Select(
                                  e => e.CommunicatieType.ID == (int) CommunicatieTypeEnum.Email).Any() ==
                              filter.HeeftEmailAdres)
                         select ld).ToList();

            if (filter.AfdelingID != null)
            {
                leden = leden.Where(e => e.AfdelingIds.Contains(filter.AfdelingID.Value)).ToList();
            }

            leden =
                leden.Where(e => (filter.LidType != LidType.Kind || e.Type == LidType.Kind) &&
                            (filter.LidType != LidType.Leiding || e.Type == LidType.Leiding)).ToList();

            if (metAdressen)
            {
                return Mapper.Map<IList<Lid>, IList<LidOverzicht>>(leden);
            }
            var list = Mapper.Map<IList<Lid>, IList<KleinLidOverzicht>>(leden);
            return Mapper.Map<IList<KleinLidOverzicht>, IList<LidOverzicht>>(list);
        }

        /// <summary>
        /// Togglet het vlagje 'lidgeld betaald' van het lid met LidId <paramref name="lidId"/>.  Geeft als resultaat
        /// het GelieerdePersoonId.  (Niet proper, maar wel interessant voor redirect.)
        /// </summary>
        /// <param name="lidId">Id van lid met te togglen lidgeld</param>
        /// <returns>GelieerdePersoonId van lid</returns>
        public int LidGeldToggle(int lidId)
        {
            var lid = _ledenRepo.ByID(lidId);
            Gav.Check(lid);
            lid.LidgeldBetaald = !lid.LidgeldBetaald;
            _ledenRepo.SaveChanges();
            return lid.GelieerdePersoon.ID;
        }

        /// <summary>
        /// Verandert een kind in leiding of vice versa
        /// </summary>
        /// <param name="lidId">Id van lid met te togglen lidtype</param>
        /// <returns>GelieerdePersoonId van lid</returns>
        public int TypeToggle(int lidId)
        {
            var lid = _ledenRepo.ByID(lidId);
            Gav.Check(lid);

            var voorstel = new LidVoorstel
                         {
                             AfdelingsJaarIDs = null,
                             AfdelingsJarenIrrelevant = true,
                             LeidingMaken = lid is Kind
                         };
            try
            {
                return _ledenMgr.Wijzigen(lid, voorstel).GelieerdePersoon.ID;
            }
            catch (FoutNummerException e)
            {
                if (e.FoutNummer == FoutNummer.AfdelingNietBeschikbaar)
                {
                    // Deze exception treedt normaal gezien enkel op als er geen afdelingen zijn,
                    // of als het lid te jong is (maar dat moeten we nog wel implementeren,
                    // zie #1326).
                    //
                    // Omdat we weten dat de exception mogelijk optreedt, handelen we ze af.
                    throw;
                }

                throw;
            }
        }
    }
}