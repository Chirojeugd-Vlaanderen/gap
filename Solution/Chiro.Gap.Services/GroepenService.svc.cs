/*
 * Copyright 2008-2016 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Bijgewerkte authenticatie Copyright 2014 Johan Vervloet
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
using AutoMapper;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.Services.Properties;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.Validatie;
using Chiro.Gap.WorkerInterfaces;
#if KIPDORP
using System.Transactions;
#endif

namespace Chiro.Gap.Services
{
	// OPM: als je de naam van de class "GroepenService" hier verandert, moet je ook de sectie "Services" in web.config aanpassen.

	// *BELANGRIJK*: Als het debuggen hier stopt owv een autorisatiefout, kijk dan na of de gebruiker waarmee
	// je aangemeld bent, op je lokale computer in de groep CgUsers zit.

	/// <summary>
	/// Service voor operaties op groepsniveau
	/// </summary>
	public class GroepenService : BaseService, IGroepenService, IDisposable
	{
		// Repositories, verantwoordelijk voor data access.

		private readonly IRepositoryProvider _repositoryProvider;
		private readonly IRepository<StraatNaam> _straatRepo;
		private readonly IRepository<WoonPlaats> _woonplaatsRepo;
		private readonly IRepository<Adres> _adresRepo; 
		private readonly IRepository<Land> _landRepo;
		private readonly IRepository<Groep> _groepenRepo;
		private readonly IRepository<Categorie> _categorieenRepo;
		private readonly IRepository<Afdeling> _afdelingenRepo;
		private readonly IRepository<OfficieleAfdeling> _officieleAfdelingenRepo;
		private readonly IRepository<AfdelingsJaar> _afdelingsJaarRepo;
		private readonly IRepository<Functie> _functiesRepo;
		private readonly IRepository<GroepsWerkJaar> _groepsWerkJarenRepo;
		private readonly IRepository<Lid> _ledenRepo;

		// Managers voor niet-triviale businesslogica

		private readonly IAfdelingsJaarManager _afdelingsJaarMgr;
		private readonly IGroepenManager _groepenMgr;
		private readonly IChiroGroepenManager _chiroGroepenMgr;
		private readonly IAdressenManager _adressenMgr;

		private readonly IFunctiesManager _functiesMgr;

		private readonly GavChecker _gav;

		// Sync

		private readonly IGroepenSync _groepenSync;

	    // Cache

		private readonly IVeelGebruikt _veelGebruikt;

		/// <summary>
		/// Nieuwe groepenservice
		/// </summary>
		/// <param name="afdelingsJaarMgr">Verantwoordelijk voor authenticatie</param>
		/// <param name="authenticatieMgr">Verantwoordelijk voor authenticatie</param>
		/// <param name="autorisatieMgr">Verantwoordelijke voor autorisatie</param>
		/// <param name="groepenMgr">Businesslogica aangaande groepen</param>
		/// <param name="chiroGroepenMgr">Businesslogica aangaande chirogroepen</param>
		/// <param name="groepsWerkJarenMgr">Businesslogica wat betreft groepswerkjaren</param>
		/// <param name="functiesMgr">Businesslogica aangaande functies</param>
		/// <param name="jaarOvergangMgr">Businesslogica aangaande de jaarovergang</param>
		/// <param name="adressenMgr">Businesslogica wat betreft adressen</param>
        /// <param name="ledenMgr">Businesslogica m.b.t. de leden</param>
		/// <param name="abonnementenMgr">Businesslogica wat betreft abonnementen.</param>
		/// <param name="repositoryProvider">De repository provider levert alle nodige repository's op.</param>
		/// <param name="groepenSync">Synchronisatie met Kipadmin</param>
		/// <param name="abonnementenSync">Abonnementensync met Mailchimp.</param>
		/// <param name="veelGebruikt">Cache</param>
		public GroepenService(IAfdelingsJaarManager afdelingsJaarMgr, IAuthenticatieManager authenticatieMgr,
			IAutorisatieManager autorisatieMgr,
			IGroepenManager groepenMgr, IJaarOvergangManager jaarOvergangMgr,
			IChiroGroepenManager chiroGroepenMgr, IGroepsWerkJarenManager groepsWerkJarenMgr,
			IFunctiesManager functiesMgr, IAdressenManager adressenMgr, ILedenManager ledenMgr,
			IAbonnementenManager abonnementenMgr,
			IRepositoryProvider repositoryProvider, IGroepenSync groepenSync, IAbonnementenSync abonnementenSync,
			IVeelGebruikt veelGebruikt) : base(ledenMgr, groepsWerkJarenMgr, authenticatieMgr, autorisatieMgr, abonnementenMgr)
		{
			_repositoryProvider = repositoryProvider;
			_straatRepo = repositoryProvider.RepositoryGet<StraatNaam>();
			_woonplaatsRepo = repositoryProvider.RepositoryGet<WoonPlaats>();
			_adresRepo = repositoryProvider.RepositoryGet<Adres>();
			_landRepo = repositoryProvider.RepositoryGet<Land>();
			_categorieenRepo = repositoryProvider.RepositoryGet<Categorie>();
			_groepenRepo = repositoryProvider.RepositoryGet<Groep>();
			_afdelingsJaarRepo = repositoryProvider.RepositoryGet<AfdelingsJaar>();
			_afdelingenRepo = repositoryProvider.RepositoryGet<Afdeling>();
			_officieleAfdelingenRepo = repositoryProvider.RepositoryGet<OfficieleAfdeling>();
			_functiesRepo = repositoryProvider.RepositoryGet<Functie>();
			_groepsWerkJarenRepo = repositoryProvider.RepositoryGet<GroepsWerkJaar>();
			_ledenRepo = repositoryProvider.RepositoryGet<Lid>();

			// De bedoeling is dat alle repositories dezelfde hash code delen.
			// Ik test er twee. Als dat goed is, zal het overal wel goed zijn.
			Debug.Assert(_straatRepo == null || _woonplaatsRepo == null ||
						 _straatRepo.ContextHash == _woonplaatsRepo.ContextHash);
			// (checks op null zijn van belang voor bij unit tests)

			_groepenMgr = groepenMgr;
			_chiroGroepenMgr = chiroGroepenMgr;
			_functiesMgr = functiesMgr;
			_afdelingsJaarMgr = afdelingsJaarMgr;
			_adressenMgr = adressenMgr;
			_groepenSync = groepenSync;

		    _veelGebruikt = veelGebruikt;

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

		~GroepenService()
		{
			Dispose(false);
		}

		#endregion

		#region ophalen

		/// <summary>
		/// Ophalen van Groepsinformatie
		/// </summary>
		/// <param name="groepId">groepID van groep waarvan we de informatie willen opvragen</param>
		/// <returns>
		/// De gevraagde informatie over de groep met id <paramref name="groepId"/>
		/// </returns>
		public GroepInfo InfoOphalen(int groepId)
		{
			var groep = GetGroepEnCheckGav(groepId);
			return Mapper.Map<Groep, GroepInfo>(groep);
		}

		/// <summary>
		/// Haalt info op, uitgaande van code (stamnummer)
		/// </summary>
		/// <param name="code">Stamnummer van de groep waarvoor info opgehaald moet worden</param>
		/// <returns>Groepsinformatie voor groep met code <paramref name="code"/></returns>
		public GroepInfo InfoOphalenCode(string code)
		{
			var groep = (from g in _groepenRepo.Select()
						 where String.Compare(g.Code, code, StringComparison.OrdinalIgnoreCase) == 0
						 select g).FirstOrDefault();
			
			Gav.Check(groep);

			return Mapper.Map<Groep, GroepInfo>(groep);
		}

		/// <summary>
		/// Ophalen van gedetailleerde informatie over de groep met ID <paramref name="groepId"/>
		/// </summary>
		/// <param name="groepId">ID van de groep waarvoor de informatie opgehaald moet worden</param>
		/// <returns>Groepsdetails, inclusief categorieen en huidige actieve afdelingen</returns>
		public GroepDetail DetailOphalen(int groepId)
		{
			var groepsWerkJaar = _groepenMgr.HuidigWerkJaar(_groepenRepo.ByID(groepId));
			if (!_autorisatieMgr.IsGav(groepsWerkJaar))
			{
				throw FaultExceptionHelper.GeenGav();
			}

			var resultaat = Mapper.Map<Groep, GroepDetail>(groepsWerkJaar.Groep);
			Mapper.Map(groepsWerkJaar.AfdelingsJaar, resultaat.Afdelingen);
			return resultaat;
		}

		/// <summary>
		/// Haalt de groepen op waarvoor de gebruiker (GAV-)rechten heeft
		/// </summary>
		/// <returns>De (informatie over de) groepen van de gebruiker</returns>
		public IEnumerable<GroepInfo> MijnGroepenOphalen()
		{
			IEnumerable<Groep> groepen = new List<Groep>();
            int? mijnAdNr = _authenticatieMgr.AdNummerGet();
            if (mijnAdNr == null)
			{
                throw FaultExceptionHelper.GeenGav();
            }
            groepen = from g in _groepenRepo.Select()
                        where
                            g.GebruikersRechtV2.Any(
                                gr => gr.Persoon.AdNummer == mijnAdNr 
                                    && (gr.VervalDatum == null || gr.VervalDatum > DateTime.Now))
                        select g;

            return Mapper.Map<IEnumerable<Groep>, IEnumerable<GroepInfo>>(groepen);

            // ** CRASH ** CRASH ** CRASH ** CRASH ** CRASH ** CRASH ** CRASH ** CRASH
            // Als we hier crashen, zou het kunnnen dat de database niet beschikbaar is.
            // Werk je op de algemene dev-db, check dan de connectie met devsrv1.
            // Werk je op een eigen database, dan moeten je connectionstrings aangepast zijn.
            // en uiteraard moet je database service gestart zijn :-P
		}

		/// <summary>
		/// Haalt informatie op over alle werkjaren waarin een groep actief was/is.
		/// </summary>
		/// <param name="groepId">ID van de groep</param>
		/// <returns>Info over alle werkjaren waarin een groep actief was/is.</returns>
		public IEnumerable<WerkJaarInfo> WerkJarenOphalen(int groepId)
		{
			var groepsWerkJaren = _groepsWerkJarenRepo.Select()
								  .Where(gwj => gwj.Groep.ID == groepId)
								  .OrderByDescending(gwj => gwj.WerkJaar).ToList();
			if (groepsWerkJaren.Count > 0)
			{
				Gav.Check(groepsWerkJaren.First());
			}

			return Mapper.Map<IEnumerable<GroepsWerkJaar>, IEnumerable<WerkJaarInfo>>(groepsWerkJaren);
		}

		/// <summary>
		/// Haalt groepswerkjaarId van het recentst gemaakte groepswerkjaar
		/// voor een gegeven groep op.
		/// </summary>
		/// <param name="groepId">groepID van groep</param>
		/// <returns>ID van het recentste GroepsWerkJaar</returns>
		public int RecentsteGroepsWerkJaarIDGet(int groepId)
		{
			var groepsWerkJaar = _groepenMgr.HuidigWerkJaar(_groepenRepo.ByID(groepId));
			if (!_autorisatieMgr.IsGav(groepsWerkJaar))
			{
				throw FaultExceptionHelper.GeenGav();
			}
			return groepsWerkJaar.ID;
		}

		/// <summary>
		/// Haalt gedetailleerde gegevens op van het recentst gemaakte groepswerkjaar
		/// voor een gegeven groep op.
		/// </summary>
		/// <param name="groepId">groepID van groep</param>
		/// <returns>
		/// De details van het recentste groepswerkjaar
		/// </returns>
		public GroepsWerkJaarDetail RecentsteGroepsWerkJaarOphalen(int groepId)
		{
			var groepsWerkJaar = _groepenMgr.HuidigWerkJaar(_groepenRepo.ByID(groepId));
			if (!_autorisatieMgr.IsGav(groepsWerkJaar))
			{
				throw FaultExceptionHelper.GeenGav();
			}

			var result = Mapper.Map<GroepsWerkJaar, GroepsWerkJaarDetail>(groepsWerkJaar);
		    result.Status = _groepsWerkJarenMgr.StatusBepalen(groepsWerkJaar);
			return result;
		}

	    /// <summary>
		/// Controleert de verplicht in te vullen lidgegevens.
		/// </summary>
		/// <param name="groepId">ID van de groep waarvan de leden te controleren zijn</param>
		/// <returns>Een rij LedenProbleemInfo.  Leeg bij gebrek aan problemen.</returns>
		public IEnumerable<LedenProbleemInfo> LedenControleren(int groepId)
		{
			var resultaat = new List<LedenProbleemInfo>();

			var groepsWerkJaar = _groepenMgr.HuidigWerkJaar(_groepenRepo.ByID(groepId));
			if (!_autorisatieMgr.IsGav(groepsWerkJaar))
			{
				throw FaultExceptionHelper.GeenGav();
			}

			var aantalLedenZonderAdres = (from ld in _ledenRepo.Select("GelieerdePersoon.PersoonsAdres")
										  where ld.GroepsWerkJaar.ID == groepsWerkJaar.ID
												&& ld.UitschrijfDatum == null &&
												ld.GelieerdePersoon.PersoonsAdres == null
										  select ld).Count();

			if (aantalLedenZonderAdres > 0)
			{
				resultaat.Add(new LedenProbleemInfo
				{
					Probleem = LidProbleem.AdresOntbreekt,
					Aantal = aantalLedenZonderAdres
				});
			}

			var aantalLedenZonderTelefoonNr = (from ld in _ledenRepo.Select("GelieerdePersoon.Communicatie")
											   where ld.GroepsWerkJaar.ID == groepsWerkJaar.ID
												&& ld.GelieerdePersoon.Communicatie.All(cmm => cmm.CommunicatieType.ID != (int)CommunicatieTypeEnum.TelefoonNummer)
												&& ld.UitschrijfDatum == null
											   select ld).Count();

			if (aantalLedenZonderTelefoonNr > 0)
			{
				resultaat.Add(new LedenProbleemInfo
				{
					Probleem = LidProbleem.TelefoonNummerOntbreekt,
					Aantal = aantalLedenZonderTelefoonNr
				});
			}

			var aantalLeidingZonderEmail = (from ld in _ledenRepo.Select("GelieerdePersoon.Communicatie")
											where ld.GroepsWerkJaar.ID == groepsWerkJaar.ID
												&& ld is Leiding 
												&& ld.GelieerdePersoon.Communicatie.All(cmm => cmm.CommunicatieType.ID != (int)CommunicatieTypeEnum.Email)
												&& ld.UitschrijfDatum == null
											select ld).Count();

			if (aantalLeidingZonderEmail > 0)
			{
				resultaat.Add(new LedenProbleemInfo
								  {
									  Probleem = LidProbleem.EmailOntbreekt,
									  Aantal = aantalLeidingZonderEmail
								  });
			}

			return resultaat;
		}


		/// <summary>
		/// Gets the Groep by ID and checks gav.
		/// </summary>
		/// <param name="groepId">The groep id.</param>
		/// <returns>Groep.</returns>
		Groep GetGroepEnCheckGav(int groepId)
		{
			var groep = _groepenRepo.ByID(groepId);
			Gav.Check(groep);
			return groep;
		}


		/// <summary>
		/// Deze method geeft gewoon de gebruikersnaam weer waaronder je de service aanroept.  Vooral om de
		/// authenticate te testen.
		/// </summary>
		/// <returns>Gebruikersnaam waarmee aangemeld</returns>
		public string WieBenIk()
		{
			return _authenticatieMgr.GebruikersNaamGet();
		}

		/// <summary>
		/// Deze method geeft weer of we op een liveomgeving werken (<c>true</c>) of niet (<c>false</c>)
		/// </summary>
		/// <returns><c>True</c> als we op een liveomgeving werken, <c>false</c> als we op een testomgeving werken</returns>
		public bool IsLive()
		{
			return _groepenMgr.IsLive();
		}

		/// <summary>
		/// Haalt informatie over alle gebruikersrechten van de gegeven groep op.
		/// </summary>
		/// <param name="groepId">ID van de groep waarvan de gebruikersrechten op te vragen zijn</param>
		/// <returns>Lijstje met details van de gebruikersrechten</returns>
		public IEnumerable<GebruikersDetail> GebruikersOphalen(int groepId)
		{
			var groep = GetGroepEnCheckGav(groepId);
            return Mapper.Map<IEnumerable<GebruikersRechtV2>, IEnumerable<GebruikersDetail>>(groep.GebruikersRechtV2);
		}

		#endregion

		#region te syncen wijzigingen

		/// <summary>
		/// Persisteert een groep in de database
		/// Momenteel ondersteunen we enkel het wijzigen van groepsnaam
		/// en stamnummer. (En dat stamnummer wijzigen, mag dan nog enkel
		/// als we super-gav zijn.)
		/// </summary>
		/// <param name="groepInfo">Te persisteren groep</param>
		public void Bewaren(GroepInfo groepInfo)
		{
			var groep = GetGroepEnCheckGav(groepInfo.ID);

			if (String.Compare(groepInfo.StamNummer, groep.Code, StringComparison.OrdinalIgnoreCase) != 0 && !_autorisatieMgr.IsSuperGav())
			{
				throw FaultExceptionHelper.GeenGav();
			}

			// Naam hier maar fixen (zie #1349)

			if (groepInfo.Naam == null)
			{
				// TODO (#1420): Validator maken in Chiro.Gap.Validatie
				throw FaultExceptionHelper.FoutNummer(FoutNummer.ValidatieFout, Resources.OngeldigeGroepsNaam);
			}

			groep.Naam = groepInfo.Naam.Trim();

			// misschien moet dit ook via de validator?
			if (groep.Naam.StartsWith("chiro ", StringComparison.CurrentCultureIgnoreCase))
			{
				groep.Naam = groep.Naam.Substring(6).Trim();
			}

			groep.Code = groepInfo.StamNummer;

#if KIPDORP
			using (var tx = new TransactionScope())
			{
#endif
				_groepenSync.Bewaren(groep);
				_groepenRepo.SaveChanges();
#if KIPDORP
				tx.Complete();
			}
#endif
		}

		#endregion

		#region beheer afdelingen (wordt niet gesynct)

		/// <summary>
		/// Maakt een nieuwe afdeling voor een gegeven ChiroGroep
		/// </summary>
		/// <param name="chirogroepId">ID van de groep</param>
		/// <param name="naam">Naam van de afdeling</param>
		/// <param name="afkorting">Afkorting van de afdeling (voor lijsten, overzichten,...)</param>
		public void AfdelingAanmaken(int chirogroepId, string naam, string afkorting)
		{
			var g = GetGroepEnCheckGav(chirogroepId);
			if (!(g is ChiroGroep))
			{
				FaultExceptionHelper.GeenGav();
			}
			try
			{
				_chiroGroepenMgr.AfdelingToevoegen((ChiroGroep)g, naam, afkorting);
			}
			catch (BestaatAlException<Afdeling> ex)
			{
				throw FaultExceptionHelper.BestaatAl(Mapper.Map<Afdeling, AfdelingInfo>(ex.Bestaande));
			}

			_groepenRepo.SaveChanges();
		}

		/// <summary>
		/// Bewaart een afdeling met de nieuwe informatie.
		/// </summary>
		/// <param name="info">De afdelingsinfo die opgeslagen moet worden</param>
		public void AfdelingBewaren(AfdelingInfo info)
		{
			var afdeling = _afdelingenRepo.ByID(info.ID);
			Gav.Check(afdeling);
			Debug.Assert(afdeling != null, "ai != null");
			afdeling.Naam = info.Naam;
			afdeling.Afkorting = info.Afkorting;
			try
			{
				_afdelingenRepo.SaveChanges();
			}
			catch (Exception)
			{
				// In principe moeten we een specifiekere exception catchen.
				// In dit geval zal die specifieke exceptie van entity framework komen,
				// en daar willen we dan weer typisch niet naar refereren.

				// De SaveChanges van de repository zou exceptions zoals de
				// DbUpdateException moeten catchen, en dan generieke fouten throwen.
				// (zie #1588)

				// Naam of code is niet uniek. Zoek op.

				var query = from afd in afdeling.ChiroGroep.Afdeling
							where !Equals(afd, afdeling)
								  && (afd.Afkorting == afdeling.Afkorting || afd.Naam == afdeling.Naam)
							select afd;

				var dubbel = query.FirstOrDefault();

				if (dubbel == null)
				{
					throw;
				}

				var result = Mapper.Map<Afdeling, AfdelingInfo>(dubbel);

				throw FaultExceptionHelper.BestaatAl(result);
			}
		}

		/// <summary>
		/// Uitgebreide info ophalen over het afdelingsjaar met de opgegeven ID
		/// </summary>
		/// <param name="afdelingsJaarId">De ID van het afdelingsjaar in kwestie</param>
		/// <returns>Uitgebreide info over het afdelingsjaar met de opgegeven ID</returns>
		public AfdelingsJaarDetail AfdelingsJaarOphalen(int afdelingsJaarId)
		{
			var afd = _afdelingsJaarRepo.ByID(afdelingsJaarId);
			Gav.Check(afd);
			return Mapper.Map<AfdelingsJaar, AfdelingsJaarDetail>(afd);
		}

		/// <summary>
		/// Maakt/bewerkt een AfdelingsJaar: 
		/// andere OfficieleAfdeling en/of andere leeftijden
		/// </summary>
		/// <param name="detail">AfdelingsJaarDetail met de gegevens over het aan te maken of te wijzigen
		/// afdelingsjaar.  <c>aj.AfdelingsJaarID</c> bepaat of het om een bestaand afdelingsjaar gaat
		/// (ID > 0), of een bestaand (ID == 0)</param>
		public void AfdelingsJaarBewaren(AfdelingsJaarDetail detail)
		{
			var afdeling = _afdelingenRepo.ByID(detail.AfdelingID);
			Gav.Check(afdeling); // throws als geen GAV van afdeling

			var officieleAfdeling = _officieleAfdelingenRepo.ByID(detail.OfficieleAfdelingID);

			Debug.Assert(afdeling != null, "afdeling != null");
			var huidigGwj = _groepenMgr.HuidigWerkJaar(afdeling.ChiroGroep);

			if (detail.AfdelingsJaarID == 0)
			{
				try
				{
					var afdelingsJaar = _afdelingsJaarMgr.Aanmaken(
						afdeling,
						officieleAfdeling,
						huidigGwj,
						detail.GeboorteJaarVan,
						detail.GeboorteJaarTot,
						detail.Geslacht);
					huidigGwj.AfdelingsJaar.Add(afdelingsJaar);
				}
				catch (FoutNummerException ex)
				{
					throw FaultExceptionHelper.FoutNummer(ex.FoutNummer, ex.Message);
				}
			}
			else
			{
				// wijzigen
				var afdelingsJaar = _afdelingsJaarRepo.ByID(detail.AfdelingsJaarID);
				Gav.Check(afdelingsJaar);

				Debug.Assert(afdelingsJaar != null, "afdelingsJaar != null");
				if (afdelingsJaar.GroepsWerkJaar.ID != huidigGwj.ID || afdelingsJaar.Afdeling.ID != detail.AfdelingID)
				{
					throw new NotSupportedException("Afdeling en Groepswerkjaar mogen niet gewijzigd worden.");
				}

				afdelingsJaar.OfficieleAfdeling = officieleAfdeling;
				afdelingsJaar.GeboorteJaarVan = detail.GeboorteJaarVan;
				afdelingsJaar.GeboorteJaarTot = detail.GeboorteJaarTot;
				afdelingsJaar.Geslacht = detail.Geslacht;
				afdelingsJaar.VersieString = detail.VersieString;

				FoutNummer? foutNummer = new AfdelingsJaarValidator().FoutNummer(afdelingsJaar);

				if (foutNummer != null)
				{
					throw FaultExceptionHelper.FoutNummer(foutNummer.Value, Resources.OngeldigAfdelingsJaar);
				}

			}

			_afdelingenRepo.SaveChanges();
		}

		/// <summary>
		/// Verwijdert een afdelingsjaar 
		/// en controleert of er geen leden in zitten.
		/// </summary>
		/// <param name="afdelingsJaarId">ID van het afdelingsjaar waarover het gaat</param>
		public void AfdelingsJaarVerwijderen(int afdelingsJaarId)
		{
			var afdelingsJaar = _afdelingsJaarRepo.ByID(afdelingsJaarId);
			Gav.Check(afdelingsJaar);

			try
			{
				_afdelingsJaarRepo.Delete(afdelingsJaar);
				_afdelingsJaarRepo.SaveChanges();
			}
			catch (Exception)
			{
				throw FaultExceptionHelper.FoutNummer(FoutNummer.AfdelingNietLeeg, Resources.AfdelingNietLeeg);
			}
		}

		/// <summary>
		/// Verwijdert een afdeling
		/// </summary>
		/// <param name="afdelingId">ID van de afdeling waarover het gaat</param>
		public void AfdelingVerwijderen(int afdelingId)
		{
			var afdeling = _afdelingenRepo.ByID(afdelingId);
			Gav.Check(afdeling);

			try{
				_afdelingenRepo.Delete(afdeling);
				_afdelingenRepo.SaveChanges();
			} catch(Exception){
				throw FaultExceptionHelper.FoutNummer(FoutNummer.AfdelingNietLeeg, Resources.AfdelingNietLeeg);
			}
		}

		/// <summary>
		/// Haalt details over alle officiele afdelingen op.
		/// </summary>
		/// <returns>Rij met details over de officiele afdelingen</returns>
		public IEnumerable<OfficieleAfdelingDetail> OfficieleAfdelingenOphalen()
		{
			return Mapper.Map<IEnumerable<OfficieleAfdeling>, IEnumerable<OfficieleAfdelingDetail>>(_officieleAfdelingenRepo.GetAll());
		}

		/// <summary>
		/// Haat een afdeling op, op basis van <paramref name="afdelingId"/>
		/// </summary>
		/// <param name="afdelingId">ID van op te halen afdeling</param>
		/// <returns>Info van de gevraagde afdeling</returns>
		public AfdelingInfo AfdelingOphalen(int afdelingId)
		{
			var afdeling = _afdelingenRepo.ByID(afdelingId);
			Gav.Check(afdeling);
			return Mapper.Map<Afdeling, AfdelingInfo>(afdeling);
		}

		/// <summary>
		/// Haalt details op van een afdelingsjaar, gebaseerd op het <paramref name="afdelingsJaarId"/>
		/// </summary>
		/// <param name="afdelingsJaarId">ID van het AFDELINGSJAAR waarvoor de details opgehaald moeten 
		/// worden.</param>
		/// <returns>De details van de afdeling in het gegeven afdelingsjaar.</returns>
		public AfdelingDetail AfdelingDetailOphalen(int afdelingsJaarId)
		{
			var afdelingsJaar = _afdelingsJaarRepo.ByID(afdelingsJaarId);
			Gav.Check(afdelingsJaar);
			Debug.Assert(afdelingsJaar != null, "afdelingsJaar != null");
			return Mapper.Map<AfdelingsJaar, AfdelingDetail>(afdelingsJaar);
		}

		/// <summary>
		/// Haalt details op over alle actieve afdelingen in het groepswerkjaar met 
		/// ID <paramref name="groepswerkjaarId"/>
		/// </summary>
		/// <param name="groepswerkjaarId">ID van het groepswerkjaar</param>
		/// <returns>
		/// Informatie over alle actieve afdelingen in het groepswerkjaar met 
		/// ID <paramref name="groepswerkjaarId"/>
		/// </returns>
		public List<AfdelingDetail> ActieveAfdelingenOphalen(int groepswerkjaarId)
		{
			var gwj = _groepsWerkJarenRepo.ByID(groepswerkjaarId);
			Gav.Check(gwj);
			Debug.Assert(gwj != null, "gwj != null");
			return Mapper.Map<IEnumerable<AfdelingsJaar>, List<AfdelingDetail>>(gwj.AfdelingsJaar);
		}

		/// <summary>
		/// Haalt beperkte informatie op over alle afdelingen van een groep
		/// (zowel actief als inactief)
		/// </summary>
		/// <param name="groepId">ID van de groep waarvoor de afdelingen gevraagd zijn</param>
		/// <returns>Lijst met AfdelingInfo</returns>
		public IList<AfdelingInfo> AlleAfdelingenOphalen(int groepId)
		{
			var groep = _groepenRepo.ByID(groepId);
			if (!_autorisatieMgr.IsGav(groep))
			{
				throw FaultExceptionHelper.GeenGav();
			}

			if (!(groep is ChiroGroep))
			{
				// een kadergroep heeft geen afdelingen.
				return new List<AfdelingInfo>();
			}

			return Mapper.Map<IEnumerable<Afdeling>, IList<AfdelingInfo>>(((ChiroGroep)groep).Afdeling);
		}

		/// <summary>
		/// Haalt informatie op over de beschikbare afdelingsjaren en hun gelinkte afdelingen van een groep in het huidige
		/// groepswerkjaar.
		/// </summary>
		/// <param name="groepId">ID van de groep waarvoor de info gevraagd is</param>
		/// <returns>Lijst van AfdelingInfo</returns>
		public List<ActieveAfdelingInfo> HuidigeAfdelingsJarenOphalen(int groepId)
		{
			var gwj = _groepenMgr.HuidigWerkJaar(_groepenRepo.ByID(groepId));
			if (!_autorisatieMgr.IsGav(gwj))
			{
				throw FaultExceptionHelper.GeenGav();
			}

			Debug.Assert(gwj != null, "gwj != null");
			return Mapper.Map<IEnumerable<AfdelingsJaar>, List<ActieveAfdelingInfo>>(gwj.AfdelingsJaar);
		}

		/// <summary>
		/// Haalt informatie op over de afdelingen van een groep die niet gebruikt zijn in een gegeven 
		/// groepswerkjaar, op basis van een <paramref name="groepswerkjaarId"/> (die dus geen afdelingsjaar hebben in het huidige werkjaar)
		/// </summary>
		/// <param name="groepswerkjaarId">ID van het groepswerkjaar waarvoor de niet-gebruikte afdelingen
		/// opgezocht moeten worden.</param>
		/// <returns>Info de ongebruikte afdelingen van een groep in het gegeven groepswerkjaar</returns>
		public List<AfdelingInfo> OngebruikteAfdelingenOphalen(int groepswerkjaarId)
		{
			var gwj = _groepsWerkJarenRepo.ByID(groepswerkjaarId);
			Gav.Check(gwj);
			Debug.Assert(gwj != null, "gwj != null");

			var chiroGroep = (gwj.Groep as ChiroGroep);
			if (chiroGroep == null)
			{
				return new List<AfdelingInfo>();    // lege lijst
			}

			var ongebruikteAfdelingen = (from afd in (chiroGroep).Afdeling
										 where !afd.AfdelingsJaar.Any(aj => Equals(aj.GroepsWerkJaar, gwj))
										 select afd).ToList();
			return Mapper.Map<List<Afdeling>, List<AfdelingInfo>>(ongebruikteAfdelingen);
		}

		#endregion

		#region beheer functies (wordt niet gesynct)

		/// <summary>
		/// Haalt uit groepswerkjaar met ID <paramref name="groepswerkjaarId"/> alle beschikbare functies
		/// op voor een lid van type <paramref name="lidType"/>.
		/// </summary>
		/// <param name="groepswerkjaarId">ID van het groepswerkjaar van de gevraagde functies</param>
		/// <param name="lidType"><c>LidType.Kind</c> of <c>LidType.Leiding</c></param>
		/// <returns>De gevraagde lijst afdelingsinfo</returns>
		public IEnumerable<FunctieDetail> FunctiesOphalen(int groepswerkjaarId, LidType lidType)
		{
			var gwj = _groepsWerkJarenRepo.ByID(groepswerkjaarId);
			Gav.Check(gwj); // throwt als ik geen GAV ben voor dit groepswerkjaar.

			// Indien kadergroep => niveau van groep
			//  Anders => meegeven lidType omgezet naar niveau (1 bit geshift)
			var lidNiveau = (gwj.Groep.Niveau == Niveau.Groep ? ((int)lidType)<<1 : (int)gwj.Groep.Niveau);

			var nationaleFuncties = (from f in _functiesRepo.Select()
                                     where f.IsNationaal && ((f.NiveauInt & lidNiveau) != 0) &&
                                      (f.WerkJaarVan == null || f.WerkJaarVan <= gwj.WerkJaar) && // Toegevoegd om functies te verbergen als ze niet meer actief zijn voor dit werkjaar
                                      (f.WerkJaarTot == null || gwj.WerkJaar <= f.WerkJaarTot)   // Fixes bug #1809
									 select f).ToList();

			var eigenRelevanteFuncties = (from f in gwj.Groep.Functie
										  where
											  f.WerkJaarVan <= gwj.WerkJaar &&
											  (f.WerkJaarTot == null || gwj.WerkJaar <= f.WerkJaarTot) &&
											  ((f.NiveauInt & lidNiveau) != 0)
										  select f).ToList();

			return Mapper.Map<IEnumerable<Functie>, IEnumerable<FunctieDetail>>(nationaleFuncties.Union(eigenRelevanteFuncties));
		}

		/// <summary>
		/// Zoekt naar problemen ivm de maximum- en minimumaantallen van functies voor het
		/// huidige werkJaar.
		/// </summary>
		/// <param name="groepId">ID van de groep waarvoor de functies gecontroleerd moeten worden.</param>
		/// <returns>
		/// Een rij FunctieProbleemInfo.  Als er geen problemen zijn, is deze leeg.
		/// </returns>
		public IEnumerable<FunctieProbleemInfo> FunctiesControleren(int groepId)
		{
			var groepsWerkJaar = _groepenMgr.HuidigWerkJaar(_groepenRepo.ByID(groepId));
			if (!_autorisatieMgr.IsGav(groepsWerkJaar))
			{
				throw FaultExceptionHelper.GeenGav();
			}

			var nationaleFuncties = (from f in _functiesRepo.Select()
									 where f.IsNationaal
									 select f).ToList();
			var eigenFuncties = groepsWerkJaar.Groep.Functie.ToList();
			var relevanteFuncties = eigenFuncties.Union(nationaleFuncties).ToList();


			var problemen = _functiesMgr.AantallenControleren(groepsWerkJaar,
															  relevanteFuncties);
			var resultaat = (from f in relevanteFuncties
							 join p in problemen on f.ID equals p.ID
							 select new FunctieProbleemInfo
										{
											Code = f.Code,
											EffectiefAantal = p.Aantal,
											ID = f.ID,
											Naam = f.Naam,
											MaxAantal = p.Max,
											MinAantal = p.Min
										}).ToList();
			return resultaat;
		}


		/// <summary>
		/// Voegt een functie toe aan de groep
		/// </summary>
		/// <param name="groepId">De groep waaraan het wordt toegevoegd</param>
		/// <param name="naam">De naam van de nieuwe functie</param>
		/// <param name="code">Code voor de nieuwe functie</param>
		/// <param name="maxAantal">Eventueel het maximumaantal leden met die functie in een werkJaar</param>
		/// <param name="minAantal">Het minimumaantal leden met die functie in een werkJaar</param>
		/// <param name="lidType">Gaat het over een functie voor leden, leiding of beide?</param>
		/// <param name="werkJaarVan">Eventueel het vroegste werkJaar waarvoor de functie beschikbaar moet zijn</param>
		/// <returns>De ID van de aangemaakte Functie</returns>
		public int FunctieToevoegen(int groepId, string naam, string code, int? maxAantal, int minAantal, LidType lidType, int? werkJaarVan)
		{
			var groep = GetGroepEnCheckGav(groepId);

			// Deze variabele vullen we als de functie al bestaat
			Functie bestaandeFunctie = null;

			// Bestaat er al een eigen of nationale functie met dezelfde code?
			bestaandeFunctie = _groepenMgr.FunctieZoekenOpCode(groep, code, _functiesRepo);
			if (bestaandeFunctie != null)
			{
				throw FaultExceptionHelper.BestaatAl(Mapper.Map<Functie, FunctieInfo>(
					bestaandeFunctie));
			}

			// bestaat er al een functie met dezelfde naam?
			bestaandeFunctie = _groepenMgr.FunctieZoekenOpNaam(groep, naam, _functiesRepo);
			if (bestaandeFunctie != null)
			{
				throw FaultExceptionHelper.BestaatAl(Mapper.Map<Functie, FunctieInfo>(
					bestaandeFunctie));
			}

			var recentsteWerkJaar = _groepenMgr.HuidigWerkJaar(groep);
			if (recentsteWerkJaar == null)
			{
				throw FaultExceptionHelper.FoutNummer(FoutNummer.GroepsWerkJaarNietBeschikbaar,
												Resources.GeenWerkJaar);
			}

			var f = new Functie
			{
				Code = code,
				Groep = groep,
				MaxAantal = maxAantal,
				MinAantal = minAantal,
				Niveau = _groepenMgr.LidTypeNaarMiveau(lidType, groep.Niveau),
				Naam = naam,
				WerkJaarTot = null,
				WerkJaarVan = recentsteWerkJaar.WerkJaar,
				IsNationaal = false
			};

			groep.Functie.Add(f);

			_groepenRepo.SaveChanges();

			return f.ID;
		}

		/// <summary>
		/// Verwijdert de functie met gegeven <paramref name="functieId"/>
		/// </summary>
		/// <param name="functieId">ID van de te verwijderen functie</param>
		/// <param name="forceren">Indien <c>true</c>, worden eventuele personen uit de
		/// te verwijderen functie eerst uit de functie weggehaald.  Indien
		/// <c>false</c> krijg je een exception als de functie niet leeg is.</param>
		public void FunctieVerwijderen(int functieId, bool forceren)
		{
			var functie = _functiesRepo.ByID(functieId);
			Gav.Check(functie);
			Debug.Assert(functie != null, "functie != null");

			try
			{
				var feedback = _functiesMgr.Verwijderen(functie, forceren);
				// als feedback null is, kan de functie daadwerkelijk verwijderd worden.
				// TODO: Dit is verwarrend!
				// TODO: Als er echt een object verwijderd moet worden (ipv een link), dan kan/mag dat eigenlijk niet via de workers
				if (feedback == null)
				{
					_functiesRepo.Delete(functie);
				}
			}
			catch (BlokkerendeObjectenException<Lid> ex)
			{
				// Er waren leden dit werkjaar met de gegeven functie:
				// stuur faultexception

				// TODO: PersoonLidInfo bevat wat te veel bloat voor deze stomme exceptie.
				// een gelieerdepersoonID en een naam zou al genoeg zijn.

				throw FaultExceptionHelper.Blokkerend(Mapper.Map<IList<Lid>, List<PersoonLidInfo>>(ex.Objecten), ex.Message);
			}
			_functiesRepo.SaveChanges();
		}

		/// <summary>
		/// Overschrijft een functie, op basis van de informatie uit <paramref name="detail" />
		/// </summary>
		/// <param name="detail">Te bewaren functie-informatie.</param>
		/// <remarks>Het veld <c>ID</c> van <paramref name="detail" /> bepaalt welke functie
		/// overschreven zal worden.</remarks>
		public void FunctieBewerken(FunctieDetail detail)
		{
			var functie = _functiesRepo.ByID(detail.ID);
			Gav.Check(functie);

			if (functie.IsNationaal)
			{
				throw FaultExceptionHelper.FoutNummer(FoutNummer.AlgemeneFout,
					Resources.NationaleFunctieNietBewerken);
			}

			if (String.Compare(detail.Code, functie.Code, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				// Code verandert. Controleer of ze nog niet bestaat
				var bestaandeFunctie = _groepenMgr.FunctieZoekenOpCode(functie.Groep, detail.Code, _functiesRepo);
				if (bestaandeFunctie != null)
				{
					throw FaultExceptionHelper.BestaatAl(Mapper.Map<Functie, FunctieInfo>(
						bestaandeFunctie));
				}
			}

			if (String.Compare(detail.Naam, functie.Naam, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				// Naam verandert. Controleer of ze nog niet bestaat
				var bestaandeFunctie = _groepenMgr.FunctieZoekenOpNaam(functie.Groep, detail.Naam, _functiesRepo);
				if (bestaandeFunctie != null)
				{
					throw FaultExceptionHelper.BestaatAl(Mapper.Map<Functie, FunctieInfo>(
						bestaandeFunctie));
				}
			}

			// Ik gebruik hier geen mappers, om te vermijden dat
			// er zaken overschreven worden die eigenlijk niet mogen overschreven worden.

			functie.Naam = detail.Naam;
			functie.Code = detail.Code;
			functie.Niveau = _groepenMgr.LidTypeNaarMiveau(detail.Type, functie.Groep.Niveau);
			functie.MaxAantal = detail.MaxAantal;
			functie.MinAantal = detail.MinAantal;

			_functiesRepo.SaveChanges();

		}

		/// <summary>
		/// Haalt functie met gegeven <paramref name="functieId"/> op
		/// </summary>
		/// <param name="functieId"></param>
		/// <returns></returns>
		public FunctieDetail FunctieOphalen(int functieId)
		{
			var functie = _functiesRepo.ByID(functieId);
			Gav.Check(functie);
			return Mapper.Map<Functie, FunctieDetail>(functie);
		}
	    #endregion

		#region beheer categorieen (wordt niet gesynct)

		/// <summary>
		/// Voegt een categorie toe aan de groep
		/// </summary>
		/// <param name="groepId">De groep waaraan het wordt toegevoegd</param>
		/// <param name="naam">De naam van de nieuwe categorie</param>
		/// <param name="code">Code voor de nieuwe categorie</param>
		/// <returns>De ID van de aangemaakte categorie</returns>
		public int CategorieToevoegen(int groepId, string naam, string code)
		{
			var groep = GetGroepEnCheckGav(groepId);

			var bestaandeCategorie = (from c in groep.Categorie
									  where String.Compare(c.Code, code, StringComparison.OrdinalIgnoreCase) == 0
									  select c).FirstOrDefault();

			if (bestaandeCategorie != null)
			{
				var info = Mapper.Map<Categorie, CategorieInfo>(bestaandeCategorie);
				throw FaultExceptionHelper.BestaatAl(info);
			}

			var nieuweCategorie = new Categorie { Code = code, Naam = naam };
			groep.Categorie.Add(nieuweCategorie);
			_groepenRepo.SaveChanges();

			return nieuweCategorie.ID;
		}

		/// <summary>
		/// Verwijdert de gegeven categorie
		/// </summary>
		/// <param name="categorieId">De ID van de te verwijderen categorie</param>
		/// <param name="forceren">Indien <c>true</c>, worden eventuele personen uit de
		/// te verwijderen categorie eerst uit de categorie weggehaald.  Indien
		/// <c>false</c> krijg je een exception als de categorie niet leeg is.</param>
		public void CategorieVerwijderen(int categorieId, bool forceren)
		{
			var categorie = _categorieenRepo.ByID(categorieId);
			Gav.Check(categorie);
			Debug.Assert(categorie != null, "categorie != null");
			if (forceren)
			{
				categorie.GelieerdePersoon.Clear();
			}
			else if (categorie.GelieerdePersoon.Any())
			{
				throw FaultExceptionHelper.Blokkerend(Mapper.Map<IEnumerable<GelieerdePersoon>, List<PersoonDetail>>(categorie.GelieerdePersoon),
													  Resources.CategorieNietLeeg);
			}
			_categorieenRepo.Delete(categorie);
			_categorieenRepo.SaveChanges();
		}

		/// <summary>
		/// Het veranderen van de naam van een categorie
		/// </summary>
		/// <param name="categorieId">De ID van de categorie</param>
		/// <param name="nieuwenaam">De nieuwe naam van de categorie</param>
		/// <exception cref="FoutNummerException">Gegooid als de naam leeg is of null is</exception>
		public void CategorieAanpassen(int categorieId, string nieuwenaam)
		{
			var categorie = _categorieenRepo.ByID(categorieId);
			Gav.Check(categorie);
			Debug.Assert(categorie != null, "categorie != null");

			if (string.IsNullOrEmpty(nieuwenaam))
			{
				throw FaultExceptionHelper.FoutNummer(FoutNummer.ValidatieFout, Resources.OngeldigeCategorieNaam);
			}
			bool bestaatal = (from g in _categorieenRepo.Select()
							  where String.Compare(g.Naam, nieuwenaam, StringComparison.OrdinalIgnoreCase) == 0
							  select g).Any();
			if (bestaatal)
			{
				throw FaultExceptionHelper.BestaatAl(nieuwenaam);
			}
			categorie.Naam = nieuwenaam;

			_categorieenRepo.SaveChanges();
		}

		/// <summary>
		/// Zoekt een categorie op, op basis van <paramref name="groepId"/> en
		/// <paramref name="code"/>
		/// </summary>
		/// <param name="groepId">ID van de groep waaraan de categorie gekoppeld moet zijn.</param>
		/// <param name="code">Code van de categorie</param>
		/// <returns>De categorie met code <paramref name="code"/> die van toepassing is op
		/// de groep met ID <paramref name="groepId"/>.</returns>
		public CategorieInfo CategorieOpzoeken(int groepId, string code)
		{
			var groep = GetGroepEnCheckGav(groepId);
			var categorie = groep.Categorie.FirstOrDefault(e => String.Compare(e.Code, code, StringComparison.InvariantCultureIgnoreCase) == 0);
			if (categorie == null)
			{
				throw FaultExceptionHelper.GeenGav();
			}
			return Mapper.Map<Categorie, CategorieInfo>(categorie);
		}

		/// <summary>
		/// Haalt alle categorieeen op van de groep met ID <paramref name="groepId"/>
		/// </summary>
		/// <param name="groepId">ID van de groep waarvan de categorieen zijn gevraagd</param>
		/// <returns>Lijst met categorie-info van de categorieen van de gevraagde groep</returns>
		public IList<CategorieInfo> CategorieenOphalen(int groepId)
		{
			var groep = GetGroepEnCheckGav(groepId);
			return Mapper.Map<IEnumerable<Categorie>, IList<CategorieInfo>>(groep.Categorie);
		}

		/// <summary>
		/// Zoekt de categorieID op van de categorie bepaald door de gegeven 
		/// <paramref name="groepId"/> en <paramref name="code"/>.
		/// </summary>
		/// <param name="groepId">ID van groep waaraan de gezochte categorie gekoppeld is</param>
		/// <param name="code">Code van de te zoeken categorie</param>
		/// <returns>Het categorieID als de categorie gevonden is, anders 0.</returns>
		public int CategorieIDOphalen(int groepId, string code)
		{
			return CategorieOpzoeken(groepId, code).ID;
		}

		/// <summary>
		/// Stelt het groepsadres in.
		/// </summary>
		/// <param name="groepID">ID van groep waarvan adres in te stellen.</param>
		/// <param name="adresInfo">Nieuw adres van de groep.</param>
		public void AdresInstellen(int groepID, AdresInfo adresInfo)
		{
			Adres adres;
			var groep = _groepenRepo.ByID(groepID);

			if (!_autorisatieMgr.IsGav(groep))
			{
				throw new GeenGavException(Resources.GeenGav);
			}

			// zoek of maak adres

			try
			{
				adres = _adressenMgr.ZoekenOfMaken(adresInfo, _adresRepo.Select(), _straatRepo.Select(),
					_woonplaatsRepo.Select(), _landRepo.Select());
			}
			catch (OngeldigObjectException ex)
			{
				throw FaultExceptionHelper.Ongeldig(ex.Berichten);
			}

			// koppelen

			groep.Adres = adres;        

#if KIPDORP
			using (var tx = new TransactionScope())
			{
#endif
				_groepenSync.Bewaren(groep);
				_groepenRepo.SaveChanges();
#if KIPDORP
				tx.Complete();
			}
#endif
		}

		#endregion

		#region adresgegevens ophalen

		/// <summary>
		/// Maakt een lijst met alle deelgemeentes uit de database; nuttig voor autocompletion
		/// in de ui.
		/// </summary>
		/// <returns>Lijst met alle beschikbare deelgemeentes</returns>
		public IEnumerable<WoonPlaatsInfo> GemeentesOphalen()
		{
			return Mapper.Map<IEnumerable<WoonPlaats>, IEnumerable<WoonPlaatsInfo>>(_woonplaatsRepo.GetAll());
		}

		/// <summary>
		/// Maakt een lijst met alle landen uit de database.
		/// </summary>
		/// <returns>Lijst met alle beschikbare landen</returns>
		public List<LandInfo> LandenOphalen()
		{
			var alleLanden = _landRepo.GetAll();
			return Mapper.Map<IEnumerable<Land>, List<LandInfo>>(alleLanden);
		}

		/// <summary>
		/// Haalt alle straten op uit een gegeven <paramref name="postNr"/>, waarvan de naam begint
		/// met het gegeven <paramref name="straatStukje"/> en maximum 20 (dit is een setting) antwoorden
		/// </summary>
		/// <param name="straatStukje">Enkele letters van de te zoeken straatnamen</param>
		/// <param name="postNr">Postnummer waarin we zoeken</param>
		/// <returns>Gegevens van de gevonden straten</returns>
		public IEnumerable<StraatInfo> StratenOphalen(string straatStukje, int postNr)
		{
			var straatNaams =
				_straatRepo.Select().Where(e =>
										   e.PostNummer == postNr
										   && e.Naam.Contains(straatStukje))
						   .Take(Settings.Default.AantalStraatSuggesties)
						   .ToList();

			var straatInfos = Mapper.Map<IEnumerable<StraatNaam>, IEnumerable<StraatInfo>>(straatNaams);

			return straatInfos;
		}

		// IndexOf(string value,int startIndex,StringComparison comparisonType


		/// <summary>
		/// Haalt alle straten op uit een gegeven rij <paramref name="postNrs"/>, waarvan de naam begint
		/// met het gegeven <paramref name="straatBegin"/>.
		/// </summary>
		/// <param name="straatBegin">Eerste letters van de te zoeken straatnamen</param>
		/// <param name="postNrs">Postnummers waarin te zoeken</param>
		/// <returns>Gegevens van de gevonden straten</returns>
		/// <remarks>Ik had deze functie ook graag StratenOphalen genoemd, maar je mag geen 2 
		/// WCF-functies met dezelfde naam in 1 service hebben.  Spijtig.</remarks>
		public IEnumerable<StraatInfo> StratenOphalenMeerderePostNrs(string straatBegin, IEnumerable<int> postNrs)
		{
			var straatNaams =
				_straatRepo.Select().Where(
					e =>
					postNrs.Contains(e.PostNummer) && e.Naam.StartsWith(straatBegin, StringComparison.OrdinalIgnoreCase));
			var straatInfos = Mapper.Map<IEnumerable<StraatNaam>, IEnumerable<StraatInfo>>(straatNaams);
			return straatInfos;
		}

		#endregion

		#region Jaarovergang (wordt niet gesynct; pas als leden worden ingeschreven)

		/// <summary>
		/// Berekent wat het nieuwe werkJaar zal zijn als op deze moment de jaarovergang zou gebeuren.
		/// </summary>
		/// <returns>Een jaartal</returns>
		public int NieuwWerkJaarOphalen(int groepId)
		{
			GetGroepEnCheckGav(groepId);
			return _groepsWerkJarenMgr.NieuweWerkJaar(groepId);
		}

		///  <summary>
		///  Eens de gebruiker alle informatie heeft ingegeven, wordt de gewenste afdelingsverdeling naar de server gestuurd.
		///  <para />
		///  Dit in de vorm van een lijst van afdelingsjaardetails, met volgende info:
		/// 		AFDELINGID van de afdelingen die geactiveerd zullen worden
		/// 		Geboortejaren, geslacht en officiele afdeling voor elk van die afdelingen
		///  </summary>
		/// <param name="teActiveren">Lijst van de afdelingen die geactiveerd moeten worden in het nieuwe werkJaar</param>
		/// <param name="groepID">ID van de groep voor wie een nieuw groepswerkjaar aangemaakt moet worden</param>
		public void JaarOvergangUitvoeren(IEnumerable<AfdelingsJaarDetail> teActiveren, int groepID)
		{
			// LET OP: De IEnumerable hierboven kan niet zomaar vervangen worden door IList.
			// Een IEnumerable<AfdelingsDetail> kan toegekend worden aan een 
			// IEnumerable<AfdelingsJaarDetail>. Maar omgekeerd werkt het niet.

			var groep = _groepenRepo.ByID(groepID);

			if (!_autorisatieMgr.IsGav(groep))
			{
				throw FaultExceptionHelper.GeenGav();
			}

			var vorigGwj = _groepenMgr.HuidigWerkJaar(groep);

			if (!_groepsWerkJarenMgr.OvergangMogelijk(DateTime.Today, vorigGwj.WerkJaar))
			{
				throw FaultExceptionHelper.FoutNummer(FoutNummer.OvergangTeVroeg, Resources.OvergangTeVroeg);
			}

		    var nieuwGwj = new GroepsWerkJaar
		    {
		        WerkJaar = _groepsWerkJarenMgr.NieuweWerkJaar(groepID),
		        Groep = groep,
		        Datum = DateTime.Today
		    };

			groep.GroepsWerkJaar.Add(nieuwGwj);

			foreach (var afdelingsJaarDetail in teActiveren)
			{
				Debug.Assert(groep is ChiroGroep);

				var detail = afdelingsJaarDetail;

				var afd = (from a in ((ChiroGroep)groep).Afdeling
						   where a.ID == detail.AfdelingID
						   select a).FirstOrDefault();

				var offAfd = _officieleAfdelingenRepo.ByID(afdelingsJaarDetail.OfficieleAfdelingID);

				if (afd == null || offAfd == null)
				{
					// gepruts => geen GAV
					throw FaultExceptionHelper.GeenGav();
				}

				var nieuwAfdelingsJaar = new AfdelingsJaar
											 {
												 Afdeling = afd,
												 GeboorteJaarVan = afdelingsJaarDetail.GeboorteJaarVan,
												 GeboorteJaarTot = afdelingsJaarDetail.GeboorteJaarTot,
												 Geslacht = afdelingsJaarDetail.Geslacht,
												 OfficieleAfdeling = offAfd,
												 GroepsWerkJaar = nieuwGwj
											 };

				FoutNummer? foutNummer = new AfdelingsJaarValidator().FoutNummer(nieuwAfdelingsJaar);

				if (foutNummer == FoutNummer.OngeldigeGeboorteJarenVoorAfdeling || foutNummer == FoutNummer.ChronologieFout)
				{
					throw FaultExceptionHelper.FoutNummer(FoutNummer.OngeldigeGeboorteJarenVoorAfdeling,
									  String.Format(
										  Resources.OngeldigeGeborteJarenAfdelingsJaar, afd.Naam));
				}
				if (foutNummer != null)
				{
					throw FaultExceptionHelper.FoutNummer(FoutNummer.ValidatieFout,
														  Resources.OngeldigAfdelingsJaar);
				}

				nieuwGwj.AfdelingsJaar.Add(nieuwAfdelingsJaar);
			}

#if KIPDORP
			using (var tx = new TransactionScope())
			{
#endif
				_groepenRepo.SaveChanges();
		        _groepenSync.WerkjaarAfsluiten(vorigGwj);

#if KIPDORP
				tx.Complete();
			}
#endif
			_veelGebruikt.WerkJaarInvalideren(groep);
		}

		/// <summary>
		/// Stelt afdelingsjaren voor voor het volgende werkjaar, gegeven de <paramref name="afdelingsIDs"/> van de
		/// afdelingen die je volgend werkjaar wilt hebben.
		/// </summary>
		/// <param name="afdelingsIDs">ID's van de afdelingen die je graag wilt activeren</param>
		/// <param name="groepId">ID van je groep</param>
		/// <returns>Een voorstel voor de afdelingsjaren, in de vorm van een lijstje AfdelingDetails.</returns>
		public IList<AfdelingDetail> NieuweAfdelingsJarenVoorstellen(int[] afdelingsIDs, int groepId)
		{
			var groepsWerkJaar = _groepenMgr.HuidigWerkJaar(_groepenRepo.ByID(groepId));
			if (!_autorisatieMgr.IsGav(groepsWerkJaar))
			{
				throw FaultExceptionHelper.GeenGav();
			}

			var nieuwWerkJaar = NieuwWerkJaarOphalen(groepId);
			var groep = groepsWerkJaar.Groep as ChiroGroep;
			var ribbels = _officieleAfdelingenRepo.ByID((int)NationaleAfdeling.Ribbels);

			Debug.Assert(groep != null, "groep != null");
			var afdelingen = (from a in groep.Afdeling
							  where afdelingsIDs.Contains(a.ID)
							  select a).ToList();

			var afdelingsJaren = _groepsWerkJarenMgr.AfdelingsJarenVoorstellen(groep, afdelingen, nieuwWerkJaar, ribbels);

			return Mapper.Map<IList<AfdelingsJaar>, IList<AfdelingDetail>>(afdelingsJaren);
		}

        /// <summary>
        /// Verwijdert (zo mogelijk) het groepswerkjaar met gegeven <paramref name="groepsWerkJaarId"/>, en
        /// herstelt de situatie zoals op het einde van vorig groepswerkjaar.
        /// </summary>
        /// <param name="groepsWerkJaarId">ID van groepswerkjaar.</param>
        public void JaarOvergangTerugDraaien(int groepsWerkJaarId)
        {
            var groepsWerkJaar = _groepsWerkJarenRepo.ByID(groepsWerkJaarId);
            if (!_autorisatieMgr.IsGav(groepsWerkJaar))
            {
                throw FaultExceptionHelper.GeenGav();
            }
            var groep = groepsWerkJaar.Groep;
            // Vanaf dat we dit releasen, wordt er in een groepswerkjaar een datum bewaard. Maar opdat
            // alles nog zou werken met bestaande jaarovergangen naar 2015-2016, kiezen we 1 september
            // als die datum niet gegeven zou zijn.
            DateTime werkjaarStart = groepsWerkJaar.Datum ?? new DateTime(groepsWerkJaar.WerkJaar, 9, 1);

            _groepsWerkJarenMgr.Verwijderen(groepsWerkJaar, _ledenRepo, _groepsWerkJarenRepo, _afdelingsJaarRepo);

#if KIPDORP
			using (var tx = new TransactionScope())
			{
#endif
            // Keer terug naar laaste dag vorig groepswerkjaar, zie #5367.
            _groepenSync.WerkjaarTerugDraaien(groep, werkjaarStart.Date.AddDays(-1));
            _groepsWerkJarenRepo.SaveChanges();
            _veelGebruikt.WerkJaarInvalideren(groep);
#if KIPDORP
				tx.Complete();
			}
#endif
        }

        #endregion

    }
}

