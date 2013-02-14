// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Chiro.Cdf.Poco;


using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.Repositories;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
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

        private readonly ILedenRepository _ledenRepo;
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
        private readonly GavChecker _gav;

        /// <summary>
        /// Nieuwe groepenservice
        /// </summary>
        /// <param name="ledenRepo">Repository voor leden</param>
        /// <param name="autorisatieMgr">Verantwoordelijke voor autorisatie</param>
        /// <param name="ledenMgr">Businesslogica aangaande leden</param>
        /// <param name="groepsWerkJarenMgr">Businesslogica wat betreft groepswerkjaren</param>
        /// <param name="verzekeringenMgr">Businesslogica aangaande verzekeringen</param>
        /// <param name="repositoryProvider">De repository provider levert alle nodige repository's op.</param>
        public LedenService(ILedenRepository ledenRepo, IAutorisatieManager autorisatieMgr,
                              IVerzekeringenManager verzekeringenMgr,
                                ILedenManager ledenMgr, IGroepsWerkJarenManager groepsWerkJarenMgr,
                              IRepositoryProvider repositoryProvider)
        {
            _ledenRepo = ledenRepo;
            _afdelingsJaarRepo = repositoryProvider.RepositoryGet<AfdelingsJaar>();
            _functiesRepo = repositoryProvider.RepositoryGet<Functie>();
            _groepsWerkJarenRepo = repositoryProvider.RepositoryGet<GroepsWerkJaar>();
            _verzekerRepo = repositoryProvider.RepositoryGet<VerzekeringsType>();
            _gelieerdePersonenRepo = repositoryProvider.RepositoryGet<GelieerdePersoon>();

            _verzekeringenMgr = verzekeringenMgr;
            _ledenMgr = ledenMgr;
            _groepsWerkJarenMgr = groepsWerkJarenMgr;
            _autorisatieMgr = autorisatieMgr;
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

        GroepsWerkJaar GetRecentsteGroepsWerkJaarEnCheckGav(int groepId)
        {
            var groepsWerkJaar = _groepsWerkJarenRepo.Select()
                .Where(gwj => gwj.Groep.ID == groepId)
                .OrderByDescending(gwj => gwj.WerkJaar)
                .First();
            Gav.Check(groepsWerkJaar);
            return groepsWerkJaar;
        }

        // TODO (#195): van onderstaande logica moet wel wat verhuizen naar de workers!
        // TODO (#1053): beter systeem bedenken voor feedback dan via foutBerichten.
        //      Foutberichten bevat nu een string die gewoon in de UI wordt geplakt, en dat breekt de seperation of concerns.

        /// <summary>
        /// Genereert de lijst van inteschrijven leden met de informatie die ze zouden krijgen als ze automagisch zouden worden ingeschreven, gebaseerd op een lijst van in te schrijven gelieerde personen.
        /// </summary>
        /// <param name="gelieerdePersoonIds">Lijst van gelieerde persoonIds waarover we inforamtie willen</param>
        /// <param name="foutBerichten">Als er sommige personen geen lid gemaakt werden, bevat foutBerichten een string waarin wat uitleg staat.</param>
        /// <returns>De LidIds van de personen die lid zijn gemaakt</returns>
        public List<InTeSchrijvenLid> VoorstelTotInschrijvenGenereren(IList<int> gelieerdePersoonIds, out string foutBerichten)
        {
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
                var gwj = GetRecentsteGroepsWerkJaarEnCheckGav(g.ID);

                foreach (var gp in g.GelieerdePersoon.OrderByDescending(gp => gp.GebDatumMetChiroLeefTijd))
                {
                    try
                    {
                        var bestaandLid = gp.Lid.FirstOrDefault();

                        if (bestaandLid != null && !bestaandLid.NonActief) // bestaat al als actief lid
                        {
                            foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.IsNogIngeschreven,
                                                                          gp.Persoon.VolledigeNaam));
                            continue;
                        }
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
                var gwj = GetRecentsteGroepsWerkJaarEnCheckGav(g.ID);

                foreach (var gp in g.GelieerdePersoon)
                {
                    // Behandel leden 1 voor 1 zodat een probleem met 1 lid niet verhindert dat de rest bewaard wordt.

                    try
                    {
                        // Kijk of het lid al bestaat (eventueel niet-actief).  In de meeste gevallen zal dit geen
                        // resultaat opleveren.  Als er toch al een lid is, worden persoon, voorkeursadres, officiele afdeling,
                        // functies ook opgehaald, omdat een eventueel geheractiveerd lid opnieuw naar Kipadmin zal moeten.

                        var l = gp.Lid.FirstOrDefault();
                            // We hadden daarnet met de gelieerde persoon zijn huidig lidobject mee opgehaald (if any)

                        if (l != null) // al ingeschreven
                        {
                            if (!l.NonActief)
                            {
                                // Al ingeschreven als actief lid; we doen er verder niets mee.
                                // (Behalve een foutbericht meegeven, wat ook niet echt correct is, zie #1053)

                                foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.IsNogIngeschreven,
                                                                              gp.Persoon.VolledigeNaam));
                                continue;
                            }

                            // We hebben al een lid, dat ooit uitgeschreven was.  Als we straks LedenManager.Wijzigen
                            // gaan gebruiken om dat lid terug in te schrijven, gaat die method nakijken of afdelingen 
                            // in lidInformatie wel gekoppeld zijn aan het groepswerkjaar waaraan het lid gekoppeld is.  Maar die
                            // laatste koppeling hebben we niet mee opgehaald.

                            // Daaorm een beetje foefelare. We hebben het groepswerkjaar wel, in gwj.  Daar hangen alle afdelingen
                            // aan vast. Als we nu het gevonden lid koppelen aan dat groepswerkjaar, dan loopt het hopelijk goed
                            // af.

                            // Proper is dat niet, maar ik doe het toch, omdat ik weet dat het hier geen kwaad kan,
                            // en omdat er geen tijd is voor een mooie oplossing.  Er zal waarschijnlijk eerst een 
                            // refactoring van de backend nodig zijn (#1250).

                            l.GroepsWerkJaar = gwj;
                            gwj.Lid.Add(l);

                            var gp1 = gp;
                            _ledenMgr.Wijzigen(l,
                                               Mapper.Map<InTeSchrijvenLid, LidVoorstel>(
                                                   lidInformatie.First(e => e.GelieerdePersoonID == gp1.ID)));

                            // 'Wijzigen' persisteert zelf
                        }
                        else // nieuw lid
                        {
                            var gp1 = gp;
                            l = _ledenMgr.NieuwInschrijven(gp, gwj, false,
                                                           Mapper.Map<InTeSchrijvenLid, LidVoorstel>(
                                                               lidInformatie.First(e => e.GelieerdePersoonID == gp1.ID)));

                            // InschrijvenVolgensVoorstel persisteert niet.  Dat doen we hier.

                            if (l != null)
                            {
                                lidIDs.Add(l.ID);
                            }
                        }
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

            foutBerichten = foutBerichtenBuilder.ToString();

            _gelieerdePersonenRepo.SaveChanges();

            return lidIDs;
        }

        /// <summary>
        /// Maakt lid met gegeven Id nonactief
        /// </summary>
        /// <param name="gelieerdePersoonIds">Id's van de gelieerde personen</param>
        /// <param name="foutBerichten">Als voor sommige personen die actie mislukte, bevat foutBerichten een
        ///     string waarin wat uitleg staat.</param>
        public void Uitschrijven(IList<int> gelieerdePersoonIds, out string foutBerichten)
        {
            var foutBerichtenBuilder = new StringBuilder();

            var gelieerdePersonen = _gelieerdePersonenRepo.ByIDs(gelieerdePersoonIds);

            if (!_autorisatieMgr.IsGav(gelieerdePersonen) || gelieerdePersoonIds.Count() != gelieerdePersonen.Count)
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var groepen = (from gp in gelieerdePersonen select gp.Groep).Distinct();

            foreach (var g in groepen)
            {
                var gwj = GetRecentsteGroepsWerkJaarEnCheckGav(g.ID);

                foreach (var gp in g.GelieerdePersoon)
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
                }
            }

            foutBerichten = foutBerichtenBuilder.ToString();

            _gelieerdePersonenRepo.SaveChanges();
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
            var functies = (from g in functieIds select _functiesRepo.ByID(g)).ToList();
            foreach (var functie in functies)
            {
                Gav.Check(functie);
            }

            lid.Functie = functies;

            _ledenRepo.SaveChanges();
        }

        /// <summary>
        /// Haalt de Id's van de afdelingsjaren van een lid op.
        /// </summary>
        /// <param name="lidId">Id van het lid waarin we geinteresseerd zijn</param>
        /// <returns>Een LidAfdelingInfo-object</returns>
        public LidAfdelingInfo AfdelingenOphalen(int lidId)
        {
            var lid = _ledenRepo.ByID(lidId);
            Gav.Check(lid);

            var info = new LidAfdelingInfo
                {
                    Type = lid.Type,
                    VolledigeNaam = lid.GelieerdePersoon.Persoon.VolledigeNaam,
                    AfdelingsJaarIDs =
                        (from g in _afdelingsJaarRepo.Select()
                         where g.Kind.Contains(lid) || g.Leiding.Contains(lid)
                         select g.ID).ToList()
                };

            return info;
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
                _verzekeringenMgr.Verzekeren(lid, verzekeringstype, DateTime.Today, _groepsWerkJarenMgr.EindDatum(lid.GroepsWerkJaar));
            }
            catch (Exception ex)
            {
                throw FaultExceptionHelper.Afhandelen(ex);
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
            var leden = (from g in _ledenRepo.Select()
                         where (filter.GroepID == null || g.GroepsWerkJaar.Groep.ID == filter.GroepID) &&
                             (filter.GroepsWerkJaarID == null || g.GroepsWerkJaar.ID == filter.GroepsWerkJaarID) &&
                             (filter.FunctieID == null || g.Functie.Select(e => e.ID == filter.FunctieID).Any()) &&
                             (filter.ProbeerPeriodeNa == null || !g.EindeInstapPeriode.HasValue || filter.ProbeerPeriodeNa < g.EindeInstapPeriode.Value) &&
                             (filter.HeeftVoorkeurAdres == null || g.GelieerdePersoon.PersoonsAdres != null) &&
                             (filter.HeeftTelefoonNummer == null || g.GelieerdePersoon.Communicatie.Select(e => e.CommunicatieType.ID == (int)CommunicatieTypeEnum.TelefoonNummer).Any() == filter.HeeftTelefoonNummer) &&
                             (filter.HeeftEmailAdres == null || g.GelieerdePersoon.Communicatie.Select(e => e.CommunicatieType.ID == (int)CommunicatieTypeEnum.Email).Any() == filter.HeeftEmailAdres)
                         select g).ToList();

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
