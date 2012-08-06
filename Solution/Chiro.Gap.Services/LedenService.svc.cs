// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AutoMapper;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Services
{
    // OPM: als je de naam van de class "LedenService" hier verandert, moet je ook de sectie "Services" in web.config aanpassen.

    // *BELANGRIJK*: Als het debuggen hier stopt owv een autorisatiefout, kijk dan na of de gebruiker waarmee
    // je aangemeld bent, op je lokale computer in de groep CgUsers zit.

    /// <summary>
    /// Service voor operaties op leden en leiding
    /// </summary>
    public class LedenService : ILedenService
    {
        #region Manager Injection

        private readonly IGelieerdePersonenManager _gelieerdePersonenMgr;
        private readonly ILedenManager _ledenMgr;
        private readonly FunctiesManager _functiesMgr;
        private readonly IAfdelingsJaarManager _afdelingsJaarMgr;
        private readonly IGroepsWerkJaarManager _groepwsWjMgr;
        private readonly VerzekeringenManager _verzekeringenMgr;

        /// <summary>
        /// Constructor met via IoC toegekende workers
        /// </summary>
        /// <param name="gpm">De worker voor GelieerdePersonen</param>
        /// <param name="lm">De worker voor Leden</param>
        /// <param name="fm">De worker voor Functies</param>
        /// <param name="ajm">De worker voor AfdelingsJaren</param>
        /// <param name="gwjm">De worker voor GroepsWerkJaren</param>
        /// <param name="vrzm">De worker voor Verzekeringen</param>
        public LedenService(
            IGelieerdePersonenManager gpm,
            ILedenManager lm,
            FunctiesManager fm,
            IAfdelingsJaarManager ajm,
            IGroepsWerkJaarManager gwjm,
            VerzekeringenManager vrzm)
        {
            _gelieerdePersonenMgr = gpm;
            _ledenMgr = lm;
            _functiesMgr = fm;
            _afdelingsJaarMgr = ajm;
            _groepwsWjMgr = gwjm;
            _verzekeringenMgr = vrzm;
        }

        #endregion

        #region leden managen

        /// <summary>
        /// Genereert de lijst van in te schrijven leden met de informatie die ze zouden krijgen als ze automagisch 
        /// zouden worden ingeschreven, gebaseerd op een lijst van in te schrijven gelieerde persoon IDs.
        /// </summary>
        /// <param name="gelieerdePersoonIDs">Lijst van gelieerde persoonIDs waarover we inforamtie willen</param>
        /// <param name="foutBerichten">Als er sommige personen geen lid gemaakt werden, bevat foutBerichten een string waarin wat uitleg staat.</param>
        /// <returns>Een lijst met inschrijvingsvoorstellen, per groep gesorteerd op geboortejaar met Chiroleeftijd</returns>
        /// <remarks>We gaan er voorlopig van uit dat alle gelieerde personen aan dezelfde groep gekoppeld zijn. Anders 
        /// zal GelieerdePersonenManager.Ophalen crashen (er staat daar een assert)</remarks>
        public IEnumerable<InTeSchrijvenLid> VoorstelTotInschrijvenGenereren(IEnumerable<int> gelieerdePersoonIDs, out string foutBerichten)
        {
            foutBerichten = string.Empty; 
            
            try
            {
                // TODO: dit lijkt op businesslogica; ik denk dat er wel wat van onderstaande code naar de workers moet.

                var foutBerichtenBuilder = new StringBuilder();

                // Haal meteen alle gelieerde personen op, gecombineerd met hun groep en eventueel huidig lid
                var gelieerdePersonen = _gelieerdePersonenMgr.Ophalen(gelieerdePersoonIDs, PersoonsExtras.Groep|PersoonsExtras.LedenDitWerkJaar);

                // We gaan ervan uit dat alle gelieerde personen op dit moment tot dezelfde groep behoren.
                // Maar in de toekomst is dat misschien niet meer zo. Dus laten we onderstaande constructie
                // maar staan.
                var groepen = (from gp in gelieerdePersonen select gp.Groep).Distinct();

                var voorgesteldelijst = new List<InTeSchrijvenLid>();

                foreach (var g in groepen)
                {
                    // Per groep lid maken.
                    // Zoek eerst recentste groepswerkjaar.
                    var gwj = _groepwsWjMgr.RecentsteOphalen(g.ID, GroepsWerkJaarExtras.Afdelingen | GroepsWerkJaarExtras.Groep);

                    foreach (var gp in g.GelieerdePersoon.OrderByDescending(gp=>gp.GebDatumMetChiroLeefTijd))
                    {
                        try
                        {
                            var bestaandLid = gp.Lid.FirstOrDefault();

                            if (bestaandLid != null && !bestaandLid.NonActief) // bestaat al als actief lid
                            {
                                    foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.IsNogIngeschreven, gp.Persoon.VolledigeNaam));
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
                            //TODO (#95): ex.Message, wat een bericht voor de programmeur moet zijn, wordt meegegeven met 'foutberichten',
                            //waarvan de inhoud rechtstreeks op de GUI getoond zal worden. Dit breekt de seperation of concerns.

                            foutBerichtenBuilder.AppendLine(String.Format("Fout voor {0}: {1}", gp.Persoon.VolledigeNaam, ex.Message));
                        }
                    }
                }

                foutBerichten = foutBerichtenBuilder.ToString();

                return voorgesteldelijst;
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                
                return null; // fake code analysis :-)
            }
        }

        /// <summary>
        /// Gegeven een lijst van IDs van gelieerde personen.
        /// Haal al die gelieerde personen op en probeer ze in het huidige werkJaar lid te maken.
        /// <para />
        /// Gaat een gelieerde persoon ophalen en maakt die lid op de plaats die overeenkomt met hun leeftijd in het huidige werkJaar.
        /// </summary>
        /// <param name="lidInformatie">Lijst van informatie over wie lid moet worden</param>
        /// <param name="foutBerichten">Als er sommige personen geen lid gemaakt werden, bevat foutBerichten een
        /// string waarin wat uitleg staat. </param>
        /// <returns>De LidID's van de personen die lid zijn gemaakt</returns>
        /// <remarks>
        /// Iedereen die kan lid gemaakt worden, wordt lid, zelfs als dit voor andere personen niet lukt. Voor die personen worden dan foutberichten
        /// teruggegeven.
        /// </remarks>
        public IEnumerable<int> Inschrijven(InTeSchrijvenLid[] lidInformatie, out string foutBerichten)
        {
            foutBerichten = String.Empty;

            // TODO (#1053): systeem foutBerichten vervangen door iets beters voor feedback.
            // (want op deze manier wordt er output voor de UI in de backend gegenereerd, dat breekt 
            // seperation of concerns)


            // Als lidInformatie null is, gaan we dat niet negeren. Dat wil zeggen dat er ergens
            // anders in de code iets serieus is misgelopen.  Om dit soort van problemen op te kunnen
            // sporen, gebruiken we best een assertion. 

            Debug.Assert(lidInformatie != null);

            try
            {
                var lidIDs = new List<int>();
                var foutBerichtenBuilder = new StringBuilder();

                // Haal meteen alle gelieerde personen op, samen met alle info die nodig is om het lid
                // over te zetten naar Kipadmin: groep, persoon, voorkeursadres

                var gelieerdePersonen = _gelieerdePersonenMgr.Ophalen(
                    lidInformatie.Select(e => e.GelieerdePersoonID),
                    PersoonsExtras.Groep | PersoonsExtras.KipIdentificatie | PersoonsExtras.LedenDitWerkJaar);

                // Mogelijk horen de gelieerde personen tot verschillende groepen.  Dat kan, als de GAV GAV is van
                // al die groepen. Als hij geen GAV is van de IDs, dan werd er al een exception gethrowd natuurlijk.
                var groepen = (from gp in gelieerdePersonen select gp.Groep).Distinct();

                foreach (var g in groepen)
                {
                    // Per groep lid maken.
                    // Zoek eerst recentste groepswerkjaar.
                    var gwj = _groepwsWjMgr.RecentsteOphalen(g.ID, GroepsWerkJaarExtras.Afdelingen | GroepsWerkJaarExtras.Groep);

                    foreach (var gp in g.GelieerdePersoon)
                    {
                        // Bewaar leden 1 voor 1, en niet allemaal tegelijk, om te vermijden dat 1 dubbel lid
                        // verhindert dat de rest bewaard wordt.

                        try
                        {
                            // Kijk of het lid al bestaat (eventueel niet-actief).  In de meeste gevallen zal dit geen
                            // resultaat opleveren.  Als er toch al een lid is, worden persoon, voorkeursadres, officiele afdeling,
                            // functies ook opgehaald, omdat een eventueel geheractiveerd lid opnieuw naar Kipadmin zal moeten.

                            var l = gp.Lid.FirstOrDefault();    // We hadden daarnet met de gelieerde persoon zijn huidig
                                                                // lidobject mee opgehaald (if any)

                           // TODO (#195, #691): Dit is businesslogica, en hoort dus thuis in de workers.

                            if (l != null) // al ingeschreven
                            {
                                if (!l.NonActief)
                                {
                                    // Al ingeschreven als actief lid; we doen er verder niets mee.
                                    // (Behalve een foutbericht meegeven, wat ook niet echt correct is, zie #1053)

                                    foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.IsNogIngeschreven, gp.Persoon.VolledigeNaam));
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
                                _ledenMgr.Wijzigen(l, Mapper.Map<InTeSchrijvenLid, LidVoorstel>(lidInformatie.First(e => e.GelieerdePersoonID == gp1.ID)));

                                // 'Wijzigen' persisteert zelf
                            }
                            else // nieuw lid
                            {
                                var gp1 = gp;
                                l = _ledenMgr.NieuwInschrijven(gp, gwj, false, Mapper.Map<InTeSchrijvenLid, LidVoorstel>(lidInformatie.First(e => e.GelieerdePersoonID == gp1.ID)));

                                // InschrijvenVolgensVoorstel persisteert niet.  Dat doen we hier.

                                if (l != null)
                                {
                                    l = _ledenMgr.Bewaren(l, LidExtras.Afdelingen | LidExtras.Persoon, true);
                                    lidIDs.Add(l.ID);
                                }

                            }

                        }
                        catch (BestaatAlException<Kind>)
                        {
                            foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.WasAlLid, gp.Persoon.VolledigeNaam));
                        }
                        catch (BestaatAlException<Leiding>)
                        {
                            foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.WasAlLeiding, gp.Persoon.VolledigeNaam));
                        }
                        catch (GapException ex)
                        {
                            foutBerichtenBuilder.AppendLine(String.Format("Fout voor {0}: {1}", gp.Persoon.VolledigeNaam, ex.Message));
                        }
                    }
                }

                foutBerichten = foutBerichtenBuilder.ToString();

                return lidIDs;
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        /// <summary>
        /// Maakt lid met gegeven ID nonactief
        /// </summary>
        /// <param name="gelieerdePersoonIDs">ID's van de gelieerde personen</param>
        /// <param name="foutBerichten">Als voor sommige personen die actie mislukte, bevat foutBerichten een
        /// string waarin wat uitleg staat.</param>
        public void Uitschrijven(IEnumerable<int> gelieerdePersoonIDs, out string foutBerichten)
        {
            // TODO (#1053): beter systeem bedenken voor feedback dan via foutBerichten.
            // Foutberichten bevat nu een string die gewoon in de UI wordt geplakt, en dat breekt
            // de seperation of concerns.
            try
            {
                var foutBerichtenBuilder = new StringBuilder();

                var gelieerdePersonen = _gelieerdePersonenMgr.Ophalen(gelieerdePersoonIDs, PersoonsExtras.Groep);
                var groepen = (from gp in gelieerdePersonen select gp.Groep).Distinct();

                foreach (var g in groepen)
                {
                    var gwj = _groepwsWjMgr.RecentsteOphalen(g.ID, GroepsWerkJaarExtras.Afdelingen | GroepsWerkJaarExtras.Groep);

                    foreach (var gp in g.GelieerdePersoon)
                    {
                        // TODO (#195): onderstaande logica verhuizen naar de workers
                        var l = _ledenMgr.OphalenViaPersoon(gp.ID, gwj.ID);

                        if (l == null)
                        {
                            foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.IsNogNietIngeschreven, gp.Persoon.VolledigeNaam));
                            continue;
                        }
                        if (l.NonActief)
                        {
                            foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.IsAlUitgeschreven, gp.Persoon.VolledigeNaam));
                            continue;
                        }

                        l.NonActief = true;

                        foreach (var fn in l.Functie)
                        {
                            fn.TeVerwijderen = true;
                        }

                        _ledenMgr.Bewaren(l, LidExtras.Functies, true);
                    }
                }

                foutBerichten = foutBerichtenBuilder.ToString();
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                foutBerichten = null;
            }
        }

        /// <summary>
        /// Togglet het vlagje 'lidgeld betaald' van het lid met LidID <paramref name="id"/>.  Geeft als resultaat
        /// het GelieerdePersoonID.  (Niet proper, maar wel interessant voor redirect.)
        /// </summary>
        /// <param name="id">ID van lid met te togglen lidgeld</param>
        /// <returns>GelieerdePersoonID van lid</returns>
        public int LidGeldToggle(int id)
        {
            try
            {
                var lid = _ledenMgr.Ophalen(id, LidExtras.Persoon);

                // Eigenlijk heeft het weinig zin om dat nullable te maken...
                lid.LidgeldBetaald = (lid.LidgeldBetaald == null || lid.LidgeldBetaald == false);
                _ledenMgr.Bewaren(lid, LidExtras.Geen, false);
                return lid.GelieerdePersoon.ID;
            }
            catch (GeenGavException ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return 0;
            }
        }

        /// <summary>
        /// Verandert een kind in leiding of vice versa
        /// </summary>
        /// <param name="id">ID van lid met te togglen lidtype</param>
        /// <returns>GelieerdePersoonID van lid</returns>
        /// <remarks>Bij het omschakelen van leiding naar lid, wordt - als er geen geschikte afdeling is
        /// gevonden - een afdeling gegokt.</remarks>
        public int TypeToggle(int id)
        {
            var lid = _ledenMgr.Ophalen(id, LidExtras.Persoon|LidExtras.Groep|LidExtras.AlleAfdelingen);

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
                if (e.FoutNummer==FoutNummer.AfdelingNietBeschikbaar)
                {
                    // Deze exception treedt normaal gezien enkel op als er geen afdelingen zijn,
                    // of als het lid te jong is (maar dat moeten we nog wel implementeren,
                    // zie #1326).
                    //
                    // Omdat we weten dat de exception mogelijk optreedt, handelen we ze af.
                    FoutAfhandelaar.FoutAfhandelen(e);
                }

                // Als we een onverwachte exception hebben, dan is er waarschijnlijk een bug die
                // we nog niet opmerkten/fixten.  Opdat dit soort situaties opgemerkt zouden
                // worden, throwen we de exception gewoon opnieuw.

                throw;
            }
        }

        #endregion

        #region verzekeren

        /// <summary>
        /// Verzekert lid met ID <paramref name="lidID"/> tegen loonverlies
        /// </summary>
        /// <param name="lidID">ID van te verzekeren lid</param>
        /// <returns>GelieerdePersoonID van het verzekerde lid</returns>
        /// <remarks>Dit is nogal een specifieke method.  In ons domain model is gegeven dat verzekeringen gekoppeld zijn aan
        /// personen, voor een bepaalde periode.  Maar in eerste instantie zal alleen de verzekering loonverlies gebruikt worden,
        /// die per definitie enkel voor leden bestaat.</remarks>
        public int LoonVerliesVerzekeren(int lidID)
        {
            try
            {
                Lid l = _ledenMgr.Ophalen(lidID, LidExtras.Verzekeringen | LidExtras.Groep);
                VerzekeringsType verz = _verzekeringenMgr.Ophalen(Verzekering.LoonVerlies);

                var verzekering = _verzekeringenMgr.Verzekeren(
                    l,
                    verz,
                    DateTime.Today, _groepwsWjMgr.EindDatum(l.GroepsWerkJaar));

                _verzekeringenMgr.PersoonsVerzekeringBewaren(verzekering, l.GroepsWerkJaar);

                return l.GelieerdePersoon.ID;
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return 0;
            }
        }

        #endregion

        #region vervangen

        /// <summary>
        /// Vervangt de functies van het lid bepaald door <paramref name="lidID"/> door de functies
        /// met ID's <paramref name="functieIDs"/>
        /// </summary>
        /// <param name="lidID">ID van lid met te vervangen functies</param>
        /// <param name="functieIDs">IDs van nieuwe functies voor het lid</param>
        public void FunctiesVervangen(int lidID, IEnumerable<int> functieIDs)
        {
            try
            {
                Lid lid = _ledenMgr.Ophalen(lidID, LidExtras.Groep | LidExtras.Functies);
                IList<Functie> functies;

                if (functieIDs != null && functieIDs.Count() > 0)
                {
                    functies = _functiesMgr.Ophalen(functieIDs);
                }
                else
                {
                    functies = new List<Functie>();
                }

                // Probleem is hier dat de functies en de groepen daaraan gekoppeld uit 'functies'
                // mogelijk dezelfde zijn als de functies en de groep van 'lid', hoewel het verschillende
                // objecten zijn.
                //
                // Laat ons dus hopen dat volgende call hierop geen problemen geeft:

                _functiesMgr.Vervangen(lid, functies);
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
            }
        }

        /// <summary>
        /// Vervangt de afdelingen van het lid met ID <paramref name="lidID"/> door de afdelingen
        /// met AFDELINGSJAARIDs gegeven door <paramref name="afdelingsJaarIDs"/>.
        /// </summary>
        /// <param name="lidID">Lid dat nieuwe afdelingen moest krijgen</param>
        /// <param name="afdelingsJaarIDs">ID's van de te koppelen afdelingsjaren</param>
        /// <returns>De GelieerdePersoonID van het lid</returns>
        public int AfdelingenVervangen(int lidID, IEnumerable<int> afdelingsJaarIDs)
        {
            try
            {
                Lid l = _ledenMgr.Ophalen(
                lidID,
                LidExtras.Groep | LidExtras.Afdelingen | LidExtras.AlleAfdelingen | LidExtras.Persoon);

                var afdelingsjaren = from aj in l.GroepsWerkJaar.AfdelingsJaar
                                     where afdelingsJaarIDs.Contains(aj.ID)
                                     select aj;

                if (afdelingsJaarIDs.Count() != afdelingsjaren.Count())
                {
                    // waarschijnlijk afdelingsjaren die niet gekoppeld zijn aan het groepswerkjaar.
                    // Dat wil zeggen dat de user aan het prutsen is.

                    throw new InvalidOperationException(Properties.Resources.AccessDenied);
                }
                _afdelingsJaarMgr.Vervangen(l, afdelingsjaren);

                return l.GelieerdePersoon.ID;
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return 0;
            }
        }

        /// <summary>
        /// Vervangt de afdelingen van de leden met gegeven <paramref name="lidIDs"/> door de afdelingen
        /// met AFDELINGSJAARIDs gegeven door <paramref name="afdelingsJaarIDs"/>.
        /// </summary>
        /// <param name="lidIDs">ID's van leden die nieuwe afdelingen moeten krijgen</param>
        /// <param name="afdelingsJaarIDs">ID's van de te koppelen afdelingsjaren</param>
        public void AfdelingenVervangenBulk(IEnumerable<int> lidIDs, IEnumerable<int> afdelingsJaarIDs)
        {
            IEnumerable<Lid> leden;

            try
            {
                leden = _ledenMgr.Ophalen(
                    lidIDs,
                        LidExtras.Groep | LidExtras.Afdelingen | LidExtras.AlleAfdelingen | LidExtras.Persoon);
            }
            catch (GeenGavException ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                throw;
            }

            // Selecteer gevraagde afdelingsjaren uit afdelingsjaren van groepen van leden

            var afdelingsJaren = afdelingsJaarIDs == null ? new List<AfdelingsJaar>() :
                leden.Select(ld => ld.GroepsWerkJaar.Groep).SelectMany(grp => grp.GroepsWerkJaar).SelectMany(
                    gwj => gwj.AfdelingsJaar).Where(aj => afdelingsJaarIDs.Contains(aj.ID)).Distinct();

            // Als het aantal gevonden afdelingsjaren al niet klopt met het aantal afdelingsjaarIDs, dan
            // is de user aan het prutsen.

            if (afdelingsJaarIDs != null && afdelingsJaren.Count() != afdelingsJaarIDs.Count())
            {
                throw new InvalidOperationException(Properties.Resources.AccessDenied);
            }

            _afdelingsJaarMgr.Vervangen(leden, afdelingsJaren);
        }

        #endregion

        #region Ophalen

        /// <summary>
        /// Haalt lid op, inclusief gelieerde persoon, persoon, groep, afdelingen en functies
        /// </summary>
        /// <param name="lidID">ID op te halen lid</param>
        /// <returns>Lidinfo; bevat info over gelieerde persoon, persoon, groep, afdelingen 
        /// en functies </returns>
        public PersoonLidInfo DetailsOphalen(int lidID)
        {
            try
            {
                return Mapper.Map<Lid, PersoonLidInfo>(_ledenMgr.Ophalen(
                                lidID,
                                LidExtras.Groep | LidExtras.Afdelingen | LidExtras.Functies | LidExtras.Persoon));
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        /// <summary>
        /// Haalt de ID's van de groepswerkjaren van een lid op. (??)
        /// </summary>
        /// <param name="lidID">ID van het lid waarin we geinteresseerd zijn</param>
        /// <returns>Een LidAfdelingInfo-object</returns>
        public LidAfdelingInfo AfdelingenOphalen(int lidID)
        {
            try
            {
                var resultaat = new LidAfdelingInfo();

                Lid l = _ledenMgr.Ophalen(lidID, LidExtras.Afdelingen | LidExtras.Persoon);
                resultaat.VolledigeNaam = String.Format(
                    "{0} {1}",
                    l.GelieerdePersoon.Persoon.VoorNaam,
                    l.GelieerdePersoon.Persoon.Naam);
                resultaat.Type = l.Type;

                if (l is Kind)
                {
                    resultaat.AfdelingsJaarIDs = new List<int> { (l as Kind).AfdelingsJaar.ID };
                }
                else if (l is Leiding)
                {
                    resultaat.AfdelingsJaarIDs = (from aj in (l as Leiding).AfdelingsJaar
                                                  select aj.ID).ToList();
                }

                return resultaat;
            }
            catch (Exception ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return null;
            }
        }

        /// <summary>
        /// Zoekt leden op, op basis van de gegeven <paramref name="filter"/>.
        /// </summary>
        /// <param name="filter">De niet-nulle properties van de filter
        /// bepalen waarop gezocht moet worden</param>
        /// <param name="metAdressen">Indien <c>true</c>, worden de
        /// adressen mee opgehaald. (Adressen ophalen vertraagt aanzienlijk.)
        /// </param>
        /// <returns>Lijst met info over gevonden leden</returns>
        /// <remarks>
        /// Er worden enkel actieve leden opgehaald
        /// </remarks>
        public IList<LidOverzicht> Zoeken(LidFilter filter, bool metAdressen)
        {
            IEnumerable<Lid> gevonden = null;

            LidExtras extras = LidExtras.Persoon |
                               LidExtras.Afdelingen |
                               LidExtras.Functies |
                               LidExtras.Communicatie;

            if (metAdressen)
            {
                extras |= LidExtras.VoorkeurAdres;
            }

            try
            {
                gevonden = _ledenMgr.Zoeken(filter, extras);
            }
            catch (GeenGavException ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
            }

            // Onverwachte exceptions mogen gerust gethrowd worden, zo vallen ze op bij
            // het debuggen.

            return Mapper.Map<IEnumerable<Lid>, IList<LidOverzicht>>(gevonden);
        }

        #endregion
    }
}
