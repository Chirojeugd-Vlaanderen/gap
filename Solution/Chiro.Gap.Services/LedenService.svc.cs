// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
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

        private readonly GelieerdePersonenManager _gelieerdePersonenMgr;
        private readonly LedenManager _ledenMgr;
        private readonly FunctiesManager _functiesMgr;
        private readonly AfdelingsJaarManager _afdelingsJaarMgr;
        private readonly GroepsWerkJaarManager _groepwsWjMgr;
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
            GelieerdePersonenManager gpm,
            LedenManager lm,
            FunctiesManager fm,
            AfdelingsJaarManager ajm,
            GroepsWerkJaarManager gwjm,
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
		/// Genereert de lijst van inteschrijven leden met de informatie die ze zouden krijgen als ze automagisch zouden worden ingeschreven, gebaseerd op een lijst van in te schrijven gelieerde personen.
		/// </summary>
		/// <param name="gelieerdePersoonIDs">Lijst van gelieerde persoonIDs waarover we inforamtie willen</param>
		/// <param name="foutBerichten">Als er sommige personen geen lid gemaakt werden, bevat foutBerichten een string waarin wat uitleg staat.</param>
		/// <returns>De LidIDs van de personen die lid zijn gemaakt</returns>
		public IEnumerable<InTeSchrijvenLid> VoorstelTotInschrijvenGenereren(IEnumerable<int> gelieerdePersoonIDs, out string foutBerichten)
		{
			try
			{
				var foutBerichtenBuilder = new StringBuilder();

				// Haal meteen alle gelieerde personen op, gecombineerd met hun groep
				var gelieerdePersonen = _gelieerdePersonenMgr.Ophalen(gelieerdePersoonIDs, PersoonsExtras.Groep);

				// Mogelijk horen de gelieerde personen tot verschillende groepen.  Dat kan, als de GAV GAV is van
				// al die groepen. Als hij geen GAV is van de IDs, dan werd er al een exception gethrowd natuurlijk.
				var groepen = (from gp in gelieerdePersonen select gp.Groep).Distinct();

				var voorgesteldelijst = new List<InTeSchrijvenLid>();

				foreach (var g in groepen)
				{
					// Per groep lid maken.
					// Zoek eerst recentste groepswerkjaar.
					var gwj = _groepwsWjMgr.RecentsteOphalen(g.ID, GroepsWerkJaarExtras.Afdelingen | GroepsWerkJaarExtras.Groep);

					foreach (var gp in g.GelieerdePersoon)
					{
						try
						{
							var l = _ledenMgr.OphalenViaPersoon(gp.ID, gwj.ID);

							if (l != null) // uitgeschreven
							{
								if (!l.NonActief)
								{
									foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.IsNogIngeschreven, gp.Persoon.VolledigeNaam));
									continue;
								}
							}
							else // nieuw lid
							{
								l = _ledenMgr.AutomagischInschrijven(gp, gwj, false);
							}

							if (l != null)
							{
								voorgesteldelijst.Add(Mapper.Map<Lid, InTeSchrijvenLid>(l));
							}
						}
						catch (GapException ex)
						{
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
				foutBerichten = null;
				return null;
			}
		}

        /// <summary>
        /// Gegeven een lijst van IDs van gelieerde personen.
        /// Haal al die gelieerde personen op en probeer ze in het huidige werkjaar lid te maken.
        /// !Niet gebruiken voor automatische jaarovergang, hiervoor is er 'JaarOvergang' in de groepenservice.
        /// <para />
        /// Gaat een gelieerde persoon ophalen en maakt die lid op de plaats die overeenkomt met hun leeftijd in het huidige werkjaar.
        /// </summary>
		/// <param name="lidInformatie">Lijst van informatie over wie lid moet worden</param>
        /// <param name="foutBerichten">Als er sommige personen geen lid gemaakt werden, bevat foutBerichten een
        /// string waarin wat uitleg staat. </param>
        /// <returns>De LidID's van de personen die lid zijn gemaakt</returns>
        /// <remarks>
        /// Iedereen die kan lid gemaakt worden, wordt lid, zelfs als dit voor andere personen niet lukt. Voor die personen worden dan foutberichten
        /// teruggegeven.
        /// </remarks>
        /// <throws>NotSupportedException</throws> // TODO handle
		public IEnumerable<int> Inschrijven(IEnumerable<InTeSchrijvenLid> lidInformatie, out string foutBerichten)
        {
			// TODO hier zat ik
            // TODO (#1053): beter systeem vinden voor deze feedback.
            try
            {
                var lidIDs = new List<int>();
                var foutBerichtenBuilder = new StringBuilder();

                // Haal meteen alle gelieerde personen op, gecombineerd met hun groep
                var gelieerdePersonen = _gelieerdePersonenMgr.Ophalen(lidInformatie.Select(e => e.GelieerdePersoonID), PersoonsExtras.Groep);

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
                        try
                        {
                            var l = _ledenMgr.OphalenViaPersoon(gp.ID, gwj.ID);

                            if (l != null) // uitgeschreven
                            {
                                if (!l.NonActief)
                                {
                                    foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.IsNogIngeschreven, gp.Persoon.VolledigeNaam));
                                    continue;
                                }
                                l.NonActief = false;
                            }
                            else // nieuw lid
                            {
                            	GelieerdePersoon gp1 = gp;
                            	l = _ledenMgr.InschrijvenVolgensVoorstel(gp, gwj, false, Mapper.Map<InTeSchrijvenLid, LidVoorstel>(lidInformatie.Where(e => e.GelieerdePersoonID == gp1.ID).First()));
                            }

                        	// Bewaar leden 1 voor 1, en niet allemaal tegelijk, om te vermijden dat 1 dubbel lid
                            // verhindert dat de rest bewaard wordt.
                            if (l != null)
                            {
                                l = _ledenMgr.Bewaren(l, LidExtras.Afdelingen | LidExtras.Persoon);
                                lidIDs.Add(l.ID);
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
                foutBerichten = null;
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
            // TODO (#1053): beter systeem vinden voor deze feedback

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

                        _ledenMgr.Bewaren(l, LidExtras.Functies);
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
        /// 'Togglet' het vlagje 'lidgeld betaald' van het lid met LidID <paramref name="id"/>.  Geeft als resultaat
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
                _ledenMgr.Bewaren(lid, LidExtras.Geen);
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
        public int TypeToggle(int id)
        {
            LidType oorspronkelijkType = LidType.Alles;

            try
            {
                Lid l = _ledenMgr.Ophalen(
                    id,
                    LidExtras.Afdelingen | LidExtras.Functies | LidExtras.Groep | LidExtras.Persoon | LidExtras.AlleAfdelingen);

                oorspronkelijkType = l.Type;
                int gpID = l.GelieerdePersoon.ID;

                _ledenMgr.TypeToggle(l);

                return gpID;
            }
            catch (GeenGavException ex)
            {
                FoutAfhandelaar.FoutAfhandelen(ex);
                return 0;
            }
            catch (InvalidOperationException ex)
            {
                // TODO (#761): Ervoor zorgen dat LidMaken/LeidingMaken duidelijkere exceptions throwen.
                // Als workaround een andere exception laten afhandelen

                FoutNummer nr;

                switch (oorspronkelijkType)
                {
                    case LidType.Kind:
                        // Fout bij omschakelen kind -> leiding: geef leidingfout
                        nr = FoutNummer.AlgemeneLeidingFout;
                        break;
                    case LidType.Leiding:
                        // Fout bij omschakelen leiding -> kind: geef kindfout
                        nr = FoutNummer.AlgemeneKindFout;
                        break;
                    default:
                        nr = FoutNummer.AlgemeneFout;
                        break;
                }

                FoutAfhandelaar.FoutAfhandelen(new FoutNummerException(nr, ex.Message));
                return 0;
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
                    DateTime.Today, GroepsWerkJaarManager.EindDatum(l.GroepsWerkJaar));

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