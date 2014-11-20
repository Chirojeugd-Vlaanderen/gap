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
using System.Text;

#if KIPDORP
using System.Transactions; // NIET VERWIJDEREN!!
#endif

using AutoMapper;
using Chiro.Cdf.Poco;

using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.Validatie;
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

        private readonly IRepositoryProvider _repositoryProvider;
        private readonly IRepository<Lid> _ledenRepo;
        private readonly IRepository<VerzekeringsType> _verzekerRepo;
        private readonly IRepository<GelieerdePersoon> _gelieerdePersonenRepo;
        private readonly IRepository<AfdelingsJaar> _afdelingsJaarRepo;
        private readonly IRepository<Functie> _functiesRepo;
        private readonly IRepository<GroepsWerkJaar> _groepsWerkJarenRepo;
        private readonly IRepository<Kind> _kinderenRepo;
        private readonly IRepository<Leiding> _leidingRepo;
        private readonly IRepository<BuitenLandsAdres> _buitenlandseAdressenRepo;
        private readonly IRepository<BelgischAdres> _belgischeAdressenRepo;
        private readonly IRepository<Groep> _groepenRepo;
        private readonly IRepository<Afdeling> _afdelingenRepo; 

        // Managers voor niet-triviale businesslogica

        private readonly IAutorisatieManager _autorisatieMgr;
        private readonly IVerzekeringenManager _verzekeringenMgr;
        private readonly ILedenManager _ledenMgr;
        private readonly IGroepsWerkJarenManager _groepsWerkJarenMgr;
        private readonly IGroepenManager _groepenMgr;
        private readonly IFunctiesManager _functiesMgr;

        // Sync

        private readonly ILedenSync _ledenSync;
        private readonly IVerzekeringenSync _verzekeringenSync;

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
        /// <param name="verzekeringenSync">Voor synchronisatie verzekeringsgegevens naar Kipadmin</param>
        public LedenService(IAutorisatieManager autorisatieMgr,
                            IVerzekeringenManager verzekeringenMgr,
                            ILedenManager ledenMgr, IGroepsWerkJarenManager groepsWerkJarenMgr,
                            IGroepenManager groepenMgr, IFunctiesManager functiesMgr,
                            IRepositoryProvider repositoryProvider, ILedenSync ledenSync,
                            IVerzekeringenSync verzekeringenSync)
        {
            _repositoryProvider = repositoryProvider;

            _ledenRepo = repositoryProvider.RepositoryGet<Lid>();
            _afdelingsJaarRepo = repositoryProvider.RepositoryGet<AfdelingsJaar>();
            _functiesRepo = repositoryProvider.RepositoryGet<Functie>();
            _groepsWerkJarenRepo = repositoryProvider.RepositoryGet<GroepsWerkJaar>();
            _verzekerRepo = repositoryProvider.RepositoryGet<VerzekeringsType>();
            _gelieerdePersonenRepo = repositoryProvider.RepositoryGet<GelieerdePersoon>();
            _kinderenRepo = repositoryProvider.RepositoryGet<Kind>();
            _leidingRepo = repositoryProvider.RepositoryGet<Leiding>();
            _buitenlandseAdressenRepo = repositoryProvider.RepositoryGet<BuitenLandsAdres>();
            _belgischeAdressenRepo = repositoryProvider.RepositoryGet<BelgischAdres>();
            _groepenRepo = repositoryProvider.RepositoryGet<Groep>();
            _afdelingenRepo = repositoryProvider.RepositoryGet<Afdeling>();

            _verzekeringenMgr = verzekeringenMgr;
            _ledenMgr = ledenMgr;
            _groepsWerkJarenMgr = groepsWerkJarenMgr;
            _autorisatieMgr = autorisatieMgr;
            _groepenMgr = groepenMgr;
            _functiesMgr = functiesMgr;

            _ledenSync = ledenSync;
            _verzekeringenSync = verzekeringenSync;

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

        ~LedenService()
        {
            Dispose(false);
        }

        #endregion

 
        /// <summary>
        /// Genereert de lijst van inteschrijven leden met de informatie die ze zouden krijgen als ze automagisch zouden worden ingeschreven, gebaseerd op een lijst van in te schrijven gelieerde personen.
        /// </summary>
        /// <param name="gelieerdePersoonIds">Lijst van gelieerde persoonIds waarover we inforamtie willen</param>
        /// <returns>De LidIds van de personen die lid zijn gemaakt</returns>
        public List<InschrijvingsVoorstel> InschrijvingVoorstellen(IList<int> gelieerdePersoonIds)
        {
            // TODO (#195): van onderstaande logica moet wel wat verhuizen naar de workers!
            var gelieerdePersonen = _gelieerdePersonenRepo.ByIDs(gelieerdePersoonIds);

            if (!_autorisatieMgr.IsGav(gelieerdePersonen) || gelieerdePersoonIds.Count != gelieerdePersonen.Count)
            {
                throw FaultExceptionHelper.GeenGav();
            }

            // We gaan ervan uit dat alle gelieerde personen op dit moment tot dezelfde groep behoren.
            // Maar in de toekomst is dat misschien niet meer zo. Dus laten we onderstaande constructie
            // maar staan.
            var groepen = (from gp in gelieerdePersonen select gp.Groep).Distinct().ToList();

            var resultaat = new List<InschrijvingsVoorstel>();

            foreach (var g in groepen)
            {
                var gwj = _groepenMgr.HuidigWerkJaar(g);

                foreach (var gp in gelieerdePersonen.Where(gelp => gelp.Groep.ID == g.ID).OrderByDescending(gp => gp.GebDatumMetChiroLeefTijd))
                {
                    var inschrijvingsVoorstel = new InschrijvingsVoorstel
                    {
                        GelieerdePersoonID = gp.ID,
                        VolledigeNaam = gp.Persoon.VolledigeNaam
                    };

                    try
                    {
                        var voorstel = _ledenMgr.InschrijvingVoorstellen(gp, gwj, true);
                        var validator = new LidVoorstelValidator();

                        inschrijvingsVoorstel.AfdelingsJaarIrrelevant = voorstel.AfdelingsJarenIrrelevant;
                        inschrijvingsVoorstel.AfdelingsJaarIDs =
                            voorstel.AfdelingsJarenIrrelevant
                                ? new int[0]
                                : voorstel.AfdelingsJaren.Select(aj => aj.ID)
                                    .ToArray();
                        inschrijvingsVoorstel.LeidingMaken = voorstel.LeidingMaken;
                        inschrijvingsVoorstel.VolledigeNaam = gp.Persoon.VolledigeNaam;
                        inschrijvingsVoorstel.FoutNummer = validator.FoutNummer(voorstel);
                    }
                    catch (FoutNummerException ex)
                    {
                        inschrijvingsVoorstel.FoutNummer = ex.FoutNummer;
                    }

                    if (inschrijvingsVoorstel.FoutNummer == null)
                    {
                        // Gelukte inschrijvingen achteraan toevoegen aan lijst.
                        resultaat.Add(inschrijvingsVoorstel);
                    }
                    else
                    {
                        // Foutberichten vooraan de feedback.
                        resultaat.Insert(0, inschrijvingsVoorstel);
                    }
                }
            }
            return resultaat;
        }

        #region te syncen

        /// <summary>
        /// Probeert de opgegeven personen in te schrijven met de meegegeven informatie.
        /// Je krijgt een lijstje terug met gegevens over de personen die niet ingeschreven konden
        /// worden.
        /// </summary>
        /// <param name="inschrijfInfo">Lijst van informatie over wie lid moet worden</param>
        /// <returns>Lijst met meer informatie over de personen die niet ingeschreven konden worden.</returns>
        public List<InschrijvingsVoorstel> Inschrijven(IList<InschrijvingsVerzoek> inschrijfInfo)
        {
            // TODO: Te veel nesting. Opkuis nodig.

            var probleemGevallen = new List<InschrijvingsVoorstel>();
            var teSyncen = new List<Lid>();

            var gelieerdePersonen = _gelieerdePersonenRepo.ByIDs(inschrijfInfo.Select(e => e.GelieerdePersoonID));

            if (!_autorisatieMgr.IsGav(gelieerdePersonen) || inschrijfInfo.Count() != gelieerdePersonen.Count)
            {
                throw FaultExceptionHelper.GeenGav();
            }

            // Mogelijk horen de gelieerde personen tot verschillende groepen.  Dat kan, als de GAV GAV is van
            // al die groepen. Als hij geen GAV is van de IDs, dan werd er al een exception gethrowd natuurlijk.
            var groepen = (from gp in gelieerdePersonen select gp.Groep).Distinct();

            foreach (var g in groepen)
            {
                bool groepInactief = g.StopDatum != null && g.StopDatum < DateTime.Now;

                // Per groep lid maken.
                // Zoek eerst recentste groepswerkjaar.
                var gwj = _groepenMgr.HuidigWerkJaar(g);

                foreach (var gp in gelieerdePersonen.Where(gelp => gelp.Groep.ID == g.ID).ToList())
                {
                    FoutNummer? foutNummer = groepInactief ? (FoutNummer?) FoutNummer.GroepInactief : null;

                    var info = (from i in inschrijfInfo where i.GelieerdePersoonID == gp.ID select i).First();

                    if (foutNummer == null)
                    {                       
                        var lidVoorstel = new LidVoorstel
                        {
                            AfdelingsJaren = _afdelingsJaarRepo.ByIDs(info.AfdelingsJaarIDs),
                            LeidingMaken = info.LeidingMaken,
                            GelieerdePersoon = gp,
                            GroepsWerkJaar = gwj
                        };

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
                                // (Behalve een foutcode meegeven)

                                foutNummer = FoutNummer.LidWasAlIngeschreven;
                            }
                            else
                            {
                                l.UitschrijfDatum = null;
                                l.NonActief = false;

                                if (lidVoorstel.LeidingMaken != (l.Type == LidType.Leiding))
                                {
                                    // lidtype moet worden veranderd

                                    Lid nieuwLid = null;
                                    try
                                    {
                                        nieuwLid = _ledenMgr.TypeToggle(l);
                                    }
                                    catch (FoutNummerException ex)
                                    {
                                        foutNummer = ex.FoutNummer;
                                    }

                                    if (foutNummer == null)
                                    {
                                        // verwijder bestaande lid
                                        _ledenRepo.Delete(l);

                                        // bewaar nieuw lid (ander type) in l; wordt straks
                                        // toegevoegd aan 'te syncen', waardoor het zal worden
                                        // bewaard en gesynct.

                                        l = nieuwLid;
                                    }
                                }
                                try
                                {
                                    _ledenMgr.AfdelingsJarenVervangen(l, lidVoorstel.AfdelingsJaren);
                                }
                                catch (FoutNummerException ex)
                                {
                                    foutNummer = ex.FoutNummer;
                                }

                                if (foutNummer == null)
                                {
                                    teSyncen.Add(l);
                                }
                            }
                        }
                        else // nieuw lid
                        {
                            try
                            {
                                l = _ledenMgr.NieuwInschrijven(lidVoorstel, false);

                                l.GelieerdePersoon.Persoon.InSync = true;
                                teSyncen.Add(l);
                            }
                            catch (BestaatAlException<Kind>)
                            {
                                foutNummer = FoutNummer.LidWasAlIngeschreven;
                            }
                            catch (BestaatAlException<Leiding>)
                            {
                                foutNummer = FoutNummer.LidWasAlIngeschreven;
                            }
                            catch (FoutNummerException ex)
                            {
                                foutNummer = ex.FoutNummer;
                            }
                            catch (GapException)
                            {
                                foutNummer = FoutNummer.AlgemeneLidFout;
                            }
                        }
                    }
                    if (foutNummer != null)
                    {
                        probleemGevallen.Add(new InschrijvingsVoorstel
                        {
                            GelieerdePersoonID = gp.ID,
                            FoutNummer = foutNummer,
                            VolledigeNaam = gp.Persoon.VolledigeNaam,
                            AfdelingsJaarIDs = info.AfdelingsJaarIDs,
                            AfdelingsJaarIrrelevant = info.AfdelingsJaarIrrelevant,
                            LeidingMaken = info.LeidingMaken
                        });
                    }
                }
            }

#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
                _ledenSync.Bewaren(teSyncen);     // TODO: (#1436) Sync naar Kipadmin
                _gelieerdePersonenRepo.SaveChanges();
#if KIPDORP
                tx.Complete();
            }
#endif
            

            return probleemGevallen;
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
                if (g.StopDatum != null && g.StopDatum < DateTime.Now)
                {
                    throw FaultExceptionHelper.FoutNummer(FoutNummer.GroepInactief, Properties.Resources.GroepInactief);
                }

                var gwj = _groepenMgr.HuidigWerkJaar(g);

                // Handel per groep de uitschrijvingen af, zodat we per groep kunnen
                // controleren of de persoon wel ingeschreven is in het recentste groepswerkjaar.

                Groep g1 = g;
                foreach (var gp in gelieerdePersonen.Where(gp => Equals(gp.Groep, g1)))
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
            // (Als ze al gekoppeld zijn, want de nationale functies hebben geen gekoppelde groep.)
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
                    case FoutNummer.EMailVerplicht:
                    case FoutNummer.ContactMoetNieuwsBriefKrijgen:
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
        /// Vervangt de afdelingen van het lid met Id <paramref name="lidId"/> door de afdelingen
        /// met AFDELINGSJAARIds gegeven door <paramref name="afdelingsJaarIds"/>.
        /// </summary>
        /// <param name="lidId">Lid dat nieuwe afdelingen moest krijgen</param>
        /// <param name="afdelingsJaarIds">Id's van de te koppelen afdelingsjaren</param>
        /// <returns>De GelieerdePersoonId van het lid</returns>
        public int AfdelingenVervangen(int lidId, IList<int> afdelingsJaarIds)
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
        public void AfdelingenVervangenBulk(IList<int> lidIds, IList<int> afdelingsJaarIds)
        {
            // Dit is een beetje een rare functie. Als er meerdere leden zijn, en meerdere afdelingen, dan moeten
            // die leden allemaal kindleden zijn van hetzelfde groepswerkjaar. Anders gaat het mis.

            // TODO: een en ander verhuizen naar workers.

            var leden = _ledenRepo.ByIDs(lidIds);
            List<AfdelingsJaar> afdelingsJaren;

            if (afdelingsJaarIds.Any())
            {
                var gwjs = (from l in leden select l.GroepsWerkJaar).Distinct().ToList();
                if (gwjs.Count() != 1)
                {
                    // Er zijn groepswerkjaren meegegeven. Een afdelingsjaar is steeds gekoppeld
                    // aan precies 1 groepswerkjaar.
                    // De leden komen uit meer dan 1 groepswerkjaar. Het afdelingsjaar kan dus
                    // nooit gekoppeld zijn aan het groepswerkjaar van elk lid. 
                    // (pigeon hole principle)
                    throw FaultExceptionHelper.FoutNummer(FoutNummer.AfdelingNietVanGroep,
                                                          Properties.Resources.OngelidgeAfdelingVoorLid);
                }
                afdelingsJaren = (from aj in gwjs.First().AfdelingsJaar
                                  where afdelingsJaarIds.Contains(aj.ID)
                                  select aj).ToList();

                if (afdelingsJaarIds.Count != afdelingsJaren.Count)
                {
                    // Niet alle afdelingsjaren zijn gevonden in het groepswerkjaar van de leden.
                    throw FaultExceptionHelper.FoutNummer(FoutNummer.AfdelingNietVanGroep,
                                      Properties.Resources.OngelidgeAfdelingVoorLid);

                }
            }
            else
            {
                afdelingsJaren = new List<AfdelingsJaar>();
            }

            if (!_autorisatieMgr.IsGav(leden))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            foreach (var lid in leden)
            {
                try
                {
                    _ledenMgr.AfdelingsJarenVervangen(lid, afdelingsJaren);
                }
                catch (FoutNummerException ex)
                {
                    if (ex.FoutNummer == FoutNummer.AlgemeneKindFout)
                    {
                        throw FaultExceptionHelper.FoutNummer(FoutNummer.AfdelingKindVerplicht, Properties.Resources.KindInEenAfdelingsJaar);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
                foreach (var l in leden)
                {
                    _ledenSync.AfdelingenUpdaten(l);
                }
                _ledenRepo.SaveChanges();
#if KIPDORP
                tx.Complete();
            }
#endif
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
            PersoonsVerzekering persoonsVerzekering;
            var lid = _ledenRepo.ByID(lidId);
            Gav.Check(lid);

            if (lid.GroepsWerkJaar.Groep.StopDatum != null && lid.GroepsWerkJaar.Groep.StopDatum < DateTime.Now)
            {
                throw FaultExceptionHelper.FoutNummer(FoutNummer.GroepInactief, Properties.Resources.GroepInactief);
            }

            var verzekeringstype = (from g in _verzekerRepo.Select() where g.ID == (int)Verzekering.LoonVerlies select g).First();

            try
            {
                persoonsVerzekering = _verzekeringenMgr.Verzekeren(lid, verzekeringstype, DateTime.Today,
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

#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
                _verzekeringenSync.Bewaren(persoonsVerzekering, lid.GroepsWerkJaar);
                _ledenRepo.SaveChanges();
#if KIPDORP
                tx.Complete();
            }
#endif
            return lid.GelieerdePersoon.ID;
        }

        /// <summary>
        /// Verandert een kind in leiding of vice versa
        /// </summary>
        /// <param name="lidId">Id van lid met te togglen lidtype</param>
        /// <returns>Lid-ID van lid</returns>
        public int TypeToggle(int lidId)
        {
            var origineelLid = _ledenRepo.ByID(lidId);
            Lid nieuwLid = null;
            Gav.Check(origineelLid);

            try
            {
                nieuwLid = _ledenMgr.TypeToggle(origineelLid);
            }
            catch (FoutNummerException ex)
            {
                // Dit is misschien wat kort door de bocht:
                throw FaultExceptionHelper.FoutNummer(ex.FoutNummer, ex.Message);
            }           

#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
                _ledenRepo.Delete(origineelLid);
                _ledenSync.TypeUpdaten(nieuwLid);
                _ledenSync.AfdelingenUpdaten(nieuwLid);
                _ledenRepo.SaveChanges();
#if KIPDORP
                tx.Complete();
            }
#endif
            return nieuwLid.ID;
        }


        #endregion

        #region zoeken en ophalen
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
                FaultExceptionHelper.FoutNummer(FoutNummer.LidUitgeschreven, Properties.Resources.LidInactief);
            }
            return Mapper.Map<Lid, PersoonLidInfo>(lid);
        }

        /// <summary>
        /// Haalt persoonsgegevens op voor (actief) lid met gegeven <paramref name="lidID"/>
        /// </summary>
        /// <param name="lidID">ID van een lid</param>
        /// <returns>beperkte informatie over de person</returns>
        public PersoonInfo PersoonOphalen(int lidID)
        {
            var lid = _ledenRepo.ByID(lidID);
            Gav.Check(lid);
            if (lid.NonActief)
            {
                FaultExceptionHelper.FoutNummer(FoutNummer.LidUitgeschreven, Properties.Resources.LidInactief);
            }
            return Mapper.Map<GelieerdePersoon, PersoonInfo>(lid.GelieerdePersoon);
        }

        /// <summary>
        /// Haalt beperkte lidinfo op voor (actief) lid met gegeven <paramref name="lidID"/>
        /// </summary>
        /// <param name="lidID">ID van een lid</param>
        /// <returns>beperkte lidinfo voor lid met gegeven <paramref name="lidID" /></returns>
        public LidInfo LidInfoOphalen(int lidID)
        {
            var lid = _ledenRepo.ByID(lidID);
            Gav.Check(lid);
            if (lid.NonActief)
            {
                FaultExceptionHelper.FoutNummer(FoutNummer.LidUitgeschreven, Properties.Resources.LidInactief);
            }
            return Mapper.Map<Lid, LidInfo>(lid);
        }

        /// <summary>
        /// Zoekt leden op, op basis van de gegeven <paramref name="filter"/>. Levert een lijst van LidOverzicht af.
        /// </summary>
        /// <param name="filter">De niet-nulle properties van de filter
        /// bepalen waarop gezocht moet worden</param>
        /// <param name="metAdressen">Indien <c>true</c>, worden de
        /// adressen mee opgehaald. (Adressen ophalen vertraagt aanzienlijk.)
        /// </param>
        /// <returns>Lijst met LidOverzicht over gevonden leden</returns>
        /// <remarks>
        /// Er worden enkel actieve leden opgehaald.
        /// Let er ook op dat je in de filter iets opgeeft als LidType
        /// (Kind, Leiding of Alles), want anders krijg je niets terug.
        /// </remarks>
        public List<LidOverzicht> LijstZoekenLidOverzicht(LidFilter filter, bool metAdressen)
        {
            // Onderstaande throwt een exception als de filter zaken bevat waar je geen rechten op
            // hebt.
            SecurityCheck(filter);

            List<LidOverzicht> resultaat;

                var leden = Zoeken(filter, metAdressen);

            // mappen
            if (metAdressen)
            {
                resultaat = Mapper.Map<IList<Lid>, List<LidOverzicht>>(leden.ToList());
            }
            else
            {
                // TODO: Waarom wordt er hier twee keer gemapt?
                // Misschien om informatie expliciet niet mee te nemen?

                var list = Mapper.Map<IList<Lid>, List<LidOverzichtZonderAdres>>(leden.ToList());
                resultaat = Mapper.Map<IList<LidOverzichtZonderAdres>, List<LidOverzicht>>(list);
            }

            return resultaat;
        }

        /// <summary>
        /// Zoekt leden op, op basis van de gegeven <paramref name="filter"/>. Levert een lijst van PersoonLidInfo af.
        /// </summary>
        /// <param name="filter">De niet-nulle properties van de filter
        /// bepalen waarop gezocht moet worden</param>
        /// <returns>Lijst met PersoonLidInfo over gevonden leden</returns>
        /// <remarks>
        /// Er worden enkel actieve leden opgehaald.
        /// Let er ook op dat je in de filter iets opgeeft als LidType
        /// (Kind, Leiding of Alles), want anders krijg je niets terug.
        /// </remarks>
        public List<PersoonLidInfo> LijstZoekenPersoonLidInfo(LidFilter filter)
        {
            // Onderstaande throwt een exception als de filter zaken bevat waar je geen rechten op
            // hebt.
            SecurityCheck(filter);

            var leden = Zoeken(filter, true);

            var resultaat = Mapper.Map<IList<Lid>, List<PersoonLidInfo>>(leden.ToList());

            return resultaat;
        }

        #endregion


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

            // Arno: de toggle werkte niet als de status 'null' was, daarom set ik hem hier eerst 
            if (lid.LidgeldBetaald == null)
            {
                lid.LidgeldBetaald = true;
            }
            else
            {
                lid.LidgeldBetaald = !lid.LidgeldBetaald;
            }
            _ledenRepo.SaveChanges();
            return lid.GelieerdePersoon.ID;
        }

        #region Private zaken. Misschien hoort dit eerder thuis in een worker? Geen idee.

        /// <summary>
        /// Controleer of <paramref name="filter"/> geen zaken probeert op te vragen waarvoor
        /// je geen rechten hebt.
        /// </summary>
        /// <param name="filter">Een lidfilter</param>
        private void SecurityCheck(LidFilter filter)
        {
            // Check security
            // We verwachten minstens 1 van volgende zaken:
            // GroepID, GroepsWerkJaarID, AfdelingID, FunctieID
            // Als je recht hebt op groep, groepswerkjaar, afdeling of functie
            // dan krijg je resultaat. Securitygewijze is dat geen probleem, want 
            // alle zoekvoorwaarden worden 'ge-and'.
            // Heb je geen rechten op groep, groepswerjaar, afdeling of functie,
            // dan krijg je een exception (te veel geknoei)

            Groep groep = null;

            if (filter.GroepID != null)
            {
                groep = _groepenRepo.ByID(filter.GroepID.Value);
            }
            else if (filter.GroepsWerkJaarID != null)
            {
                groep = _groepsWerkJarenRepo.ByID(filter.GroepsWerkJaarID.Value, "Groep").Groep;
            }
            else if (filter.AfdelingID != null)
            {
                groep = _afdelingenRepo.ByID(filter.AfdelingID.Value, "Groep").ChiroGroep;
            }
            else if (filter.FunctieID != null)
            {
                groep = _functiesRepo.ByID(filter.FunctieID.Value, "Groep").Groep;
            }

            if (!_autorisatieMgr.IsGav(groep))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        /// <summary>
        /// Zoekt alle leden die voldoen aan de gegeven <paramref name="filter"/>. Als
        /// <paramref name="metAdressen"/> <c>true</c> is, dan worden de adressen eager
        /// geload.
        /// </summary>
        /// <param name="filter">bepaalt de te zoeken leden</param>
        /// <param name="metAdressen">Als <c>true</c>, dan worden de adressen eager geload.</param>
        /// <returns>Enumerable voor de gevonden leden.</returns>
        private IEnumerable<Lid> Zoeken(LidFilter filter, bool metAdressen)
        {
            // Let op. Deze method is zorgvuldig geschreven opdat de lidinfo na 2 of 4 query's 
            // (alnaargelang met of zonder adressen) allemaal geladen zou zijn.
            // Oorspronkelijk waren dat er een 10-tal per lid. (zie #1587)
            // Als je hier iets aanpast, controleer dan de gegenereerde query's. Of als je niet weet hoe daaraan
            // te beginnen, geef mij (johan) een seintje.

            var teLadenDependencies = new List<string>
                                      {
                                          "GelieerdePersoon.Communicatie.CommunicatieType",
                                          "Functie",
                                          "GelieerdePersoon.Persoon",
                                          "AfdelingsJaar.Afdeling"
                                      };
            if (metAdressen)
            {
                teLadenDependencies.Add("GelieerdePersoon.PersoonsAdres.Adres");
            }

            // Het lukt me niet om afdelingen eager te loaden, vermoedelijk omdat
            // die anders gekoppeld zijn aan kinderen en aan leiding. Dus ik doe
            // nu al het werk dubbel, 1 keer voor kinderen, 1 keer voor leiding.
            // Op het einde voeg ik de resultaten samen.

            var kinderen =
                (from ld in
                     _kinderenRepo.Select(teLadenDependencies.ToArray())
                 where
                     ld.UitschrijfDatum == null &&
                     (filter.GroepID == null || ld.GroepsWerkJaar.Groep.ID == filter.GroepID) &&
                     (filter.GroepsWerkJaarID == null || ld.GroepsWerkJaar.ID == filter.GroepsWerkJaarID) &&
                     (filter.FunctieID == null || ld.Functie.Any(e => e.ID == filter.FunctieID)) &&
                     (filter.ProbeerPeriodeNa == null || !ld.EindeInstapPeriode.HasValue ||
                      filter.ProbeerPeriodeNa < ld.EindeInstapPeriode.Value) &&
                     (filter.HeeftVoorkeurAdres == null ||
                      (ld.GelieerdePersoon.PersoonsAdres != null && filter.HeeftVoorkeurAdres == true) ||
                      (ld.GelieerdePersoon.PersoonsAdres == null && filter.HeeftVoorkeurAdres == false)) &&
                     (filter.HeeftTelefoonNummer == null ||
                      ld.GelieerdePersoon.Communicatie.Any(
                          e => e.CommunicatieType.ID == (int)CommunicatieTypeEnum.TelefoonNummer) ==
                      filter.HeeftTelefoonNummer) &&
                     (filter.HeeftEmailAdres == null ||
                      ld.GelieerdePersoon.Communicatie.Any(
                          e => e.CommunicatieType.ID == (int)CommunicatieTypeEnum.Email) ==
                      filter.HeeftEmailAdres) &&
                     (filter.AfdelingID == null || filter.AfdelingID == ld.AfdelingsJaar.Afdeling.ID)
                 select ld).ToList();

            var leiding = (from ld in
                               _leidingRepo.Select(teLadenDependencies.ToArray())
                           where
                               ld.UitschrijfDatum == null &&
                               (filter.GroepID == null || ld.GroepsWerkJaar.Groep.ID == filter.GroepID) &&
                               (filter.GroepsWerkJaarID == null || ld.GroepsWerkJaar.ID == filter.GroepsWerkJaarID) &&
                               (filter.FunctieID == null || ld.Functie.Any(e => e.ID == filter.FunctieID)) &&
                               (filter.ProbeerPeriodeNa == null || !ld.EindeInstapPeriode.HasValue ||
                                filter.ProbeerPeriodeNa < ld.EindeInstapPeriode.Value) &&
                               (filter.HeeftVoorkeurAdres == null ||
                                (ld.GelieerdePersoon.PersoonsAdres != null && filter.HeeftVoorkeurAdres == true) ||
                                (ld.GelieerdePersoon.PersoonsAdres == null && filter.HeeftVoorkeurAdres == false)) &&
                               (filter.HeeftTelefoonNummer == null ||
                                ld.GelieerdePersoon.Communicatie.Any(
                                    e => e.CommunicatieType.ID == (int)CommunicatieTypeEnum.TelefoonNummer) ==
                                filter.HeeftTelefoonNummer) &&
                               (filter.HeeftEmailAdres == null ||
                                ld.GelieerdePersoon.Communicatie.Any(
                                    e => e.CommunicatieType.ID == (int)CommunicatieTypeEnum.Email) ==
                                filter.HeeftEmailAdres) &&
                               (filter.AfdelingID == null ||
                                ld.AfdelingsJaar.Any(aj => aj.Afdeling.ID == filter.AfdelingID))
                           select ld).ToList();

            IEnumerable<Lid> leden;

            // Als lidtype=LidType.Geen, dan gaan we ervan uit dat dit een vergissing is, en de gebruiker
            // LidType.Alles bedoelt.

            if (filter.LidType != LidType.Leiding)
            {
                // Er zijn maar 2 lidtypes: Kind of Leiding.
                // Als de gebruiker niet expliciet naar de leiding vraagt, dan mogen we de kinderen
                // opleveren.

                leden = kinderen;
            }
            else
            {
                leden = new List<Lid>();
            }
            if (filter.LidType != LidType.Kind)
            {
                // Tenzij de user enkel de kinderen vraagt, leveren we de leiding op.

                leden = leden.Union(leiding);
            }


            if (metAdressen)
            {
                // Laad hier meteen alle adressen in 1 query. Entity Framework zal ze koppelen
                // aan de gelieerde personen die we al hebben. En zo vermijden we bij het mappen
                // een adresquery voor iedere gelieerde persoon.

                var gpIDs = (from l in leden select l.GelieerdePersoon.ID).ToList();

                // Resharper melding afzetten hieronder. Het is wel degelijk de bedoeling dat de query's worden 
                // geevalueerd

                // ReSharper disable UnusedVariable
                var buitenlandseAdressen =
                    (from adr in _buitenlandseAdressenRepo.Select("PersoonsAdres.GelieerdePersoon", "Land")
                     where adr.PersoonsAdres.Any(pa => pa.GelieerdePersoon.Any(gp => gpIDs.Contains(gp.ID)))
                     select adr).ToList();
                var belgischeAdressen =
                    (from adr in _belgischeAdressenRepo.Select("PersoonsAdres.GelieerdePersoon", "StraatNaam", "Woonplaats")
                     where adr.PersoonsAdres.Any(pa => pa.GelieerdePersoon.Any(gp => gpIDs.Contains(gp.ID)))
                     select adr).ToList();
                // ReSharper restore UnusedVariable     
            }
            return leden;
        }



        #endregion

    }
}
