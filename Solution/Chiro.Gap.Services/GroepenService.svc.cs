// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using AutoMapper;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.Workers;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Services
{
	// OPM: als je de naam van de class "GroepenService" hier verandert, moet je ook de sectie "Services" in web.config aanpassen.
	public class GroepenService : IGroepenService
	{
		#region Manager Injection

		private readonly GroepenManager _groepenMgr;
		private readonly AfdelingsJaarManager _afdelingsJaarMgr;
		private readonly AdressenManager _adresMgr;
		private readonly GroepsWerkJaarManager _groepsWerkJaarManager;
		private readonly IAutorisatieManager _autorisatieMgr;
		private readonly GelieerdePersonenManager _gelieerdePersonenMgr;
		private readonly CategorieenManager _categorieenMgr;
		private readonly FunctiesManager _functiesMgr;

		public GroepenService(
			GroepenManager gm,
			AfdelingsJaarManager ajm,
			GroepsWerkJaarManager wm,
			GelieerdePersonenManager gpm,
			AdressenManager adresMgr,
			CategorieenManager cm,
			FunctiesManager fm,
			IAutorisatieManager am)
		{
			_groepenMgr = gm;
			_afdelingsJaarMgr = ajm;
			_groepsWerkJaarManager = wm;
			_autorisatieMgr = am;
			_gelieerdePersonenMgr = gpm;
			_adresMgr = adresMgr;
			_categorieenMgr = cm;
			_functiesMgr = fm;
		}

		#endregion

		#region algemene members
		/// <summary>
		/// Ophalen van Groepsinformatie
		/// </summary>
		/// <param name="groepID">GroepID van groep waarvan we de informatie willen opvragen</param>
		/// <returns>
		/// De gevraagde informatie over de groep met id <paramref name="groepID"/>
		/// </returns>
		public GroepInfo InfoOphalen(int groepID)
		{
			var g = _groepenMgr.Ophalen(groepID);
			return Mapper.Map<Groep, GroepInfo>(g);
		}

		/// <summary>
		/// Haalt info op, uitgaande van code (stamnummer)
		/// </summary>
		/// <param name="code">Stamnummer van de groep waarvoor info opgehaald moet worden</param>
		/// <returns>Groepsinformatie voor groep met code <paramref name="code"/></returns>
		public GroepInfo InfoOphalenCode(string code)
		{
			Groep g;

			try
			{
				g = _groepenMgr.Ophalen(code);
			}
			catch (GeenGavException)
			{
				throw new FaultException<GapFault>(new GapFault { FoutNummer = FoutNummer.GeenGav });
			}

			return Mapper.Map<Groep, GroepInfo>(g);
		}

		/// <summary>
		/// Ophalen van gedetailleerde informatie over de groep met ID <paramref name="groepID"/>
		/// </summary>
		/// <param name="groepID">ID van de groep waarvoor de informatie opgehaald moet worden</param>
		/// <returns>Groepsdetails, inclusief categorieen en huidige actieve afdelingen</returns>
		public GroepDetail DetailOphalen(int groepID)
		{
			var resultaat = new GroepDetail();

			var g = _groepenMgr.OphalenMetIndelingen(groepID);
			Mapper.Map(g, resultaat);

			resultaat.Afdelingen = Mapper.Map<IEnumerable<AfdelingsJaar>, List<AfdelingDetail>>(
				_groepsWerkJaarManager.RecentsteOphalen(groepID, GroepsWerkJaarExtras.Afdelingen).AfdelingsJaar);

			return resultaat;
		}

		/// <summary>
		/// Persisteert een groep in de database
		/// </summary>
		/// <param name="g">Te persisteren groep</param>
		/// <remarks>FIXME: gedetailleerde exception</remarks>
		public void Bewaren(GroepInfo g)
		{
			try
			{
				var groep = _groepenMgr.Ophalen(g.ID);

				// Ik gebruik hier geen mapper, omdat de entity Groep (en eender welke entity in het algemeen)
				// heel veel members heeft.  Omdat Automapper.AssertConfigurationIsValid wil gebruiken, zou je
				// dan al die members moeten ignoren.

				groep.ID = g.ID;
				groep.Naam = g.Naam;
				groep.Code = g.StamNummer;

				// TODO: Hier gaat natuurlijk nooit een concurrency exception optreden,
				// aangezien GroepInfo (nog?) geen versiestring bevat.

				_groepenMgr.Bewaren(groep);
			}
			catch (Exception e)
			{
				// TODO: fatsoenlijke exception handling
				throw new FaultException(e.Message, new FaultCode("Optimistic Concurrency Exception"));
			}
		}

		public int RecentsteGroepsWerkJaarIDGet(int groepID)
		{
			return _groepsWerkJaarManager.RecentsteGroepsWerkJaarIDGet(groepID);
		}

		#endregion

		#region ophalen

		/// <summary>
		/// Haalt details over alle officiele afdelingen op.
		/// </summary>
		/// <param name="groepID">ID van een groep, zodat aan de hand van het recenste groepswerkjaar
		/// de standaardgeboortejaren van en tot bepaald kunnen worden</param>
		/// <returns>Rij met details over de officiele afdelingen</returns>

		public IEnumerable<OfficieleAfdelingDetail> OfficieleAfdelingenOphalen(int groepID)
		{
			GroepsWerkJaar gwj = _groepsWerkJaarManager.RecentsteOphalen(groepID);

			Mapper.CreateMap<OfficieleAfdeling, OfficieleAfdelingDetail>()
				.ForMember(
					dst => dst.StandaardGeboorteJaarVan,
					opt => opt.MapFrom(src => gwj.WerkJaar - src.LeefTijdTot))
				.ForMember(
					dst => dst.StandaardGeboorteJaarTot,
					opt => opt.MapFrom(src => gwj.WerkJaar - src.LeefTijdVan));

			return Mapper.Map<IEnumerable<OfficieleAfdeling>, IEnumerable<OfficieleAfdelingDetail>>(
				_afdelingsJaarMgr.OfficieleAfdelingenOphalen());
		}

		public IEnumerable<GroepInfo> MijnGroepenOphalen()
		{
			var result = _autorisatieMgr.MijnGroepenOphalen();
			return Mapper.Map<IEnumerable<Groep>, IEnumerable<GroepInfo>>(result);
		}

		#endregion

		#region afdelingen

		// Bedoeling van het afdelingsgedeelte:
		// er zijn een aantal officiële afdelingen, die een range van leeftijden hebben. Blijven dat altijd dezelfde?
		// Elke Chirogroep heeft elk werkjaar haar eigen afdelingen, die ook een range van leeftijden hebben.
		// 
		// Elke afdeling moet overeenkomen met een officiële afdeling.
		// Er is niet gespecifieerd of het mogelijk is om een eerste-jaar-rakkers en een tweede-jaar-rakkers te hebben
		// 
		// Omdat bovenstaande niet echt duidelijk is en misschien niet altijd voldoende:
		// waarom moet er een mapping zijn met een officiële afdeling? Als dit echt moet, dan is het bovenstaande niet duidelijk,
		// en stel ik het onderstaande voor
		// 
		// Elke afdeling heeft een naam, een afkorting en een boolean NOGINGEBRUIK?
		// Elk afdelingsjaar heeft een afdeling en een interval van leeftijden.
		// Voor elke leeftijd is er een mapping met een officiële afdeling
		// elke leeftijd kan maar op 1 officiële afdeling gemapt worden
		// 
		// Voorbeelden:
		// "de kleintjes" = {minis, speelclub}
		// "de 5de jaars" = {eerste jaar rakkers}
		// "rakwi's" = {tweede jaar speelclub, rakkers}

		/// <summary>
		/// Maakt een nieuwe afdeling voor een gegeven groep
		/// </summary>
		/// <param name="groepID">ID van de groep</param>
		/// <param name="naam">Naam van de afdeling</param>
		/// <param name="afkorting">Afkorting van de afdeling (voor lijsten, overzichten,...)</param>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void AfdelingAanmaken(int groepID, string naam, string afkorting)
		{
			Groep g = _groepenMgr.Ophalen(groepID, GroepsExtras.AlleAfdelingen);
			try
			{
				_groepenMgr.AfdelingToevoegen(g, naam, afkorting);
			}
			catch (BestaatAlException<Afdeling> ex)
			{
				var fault = Mapper.Map<
					BestaatAlException<Afdeling>,
					BestaatAlFault<AfdelingInfo>>(ex);

				throw new FaultException<BestaatAlFault<AfdelingInfo>>(fault);
			}

			_groepenMgr.Bewaren(g, e => e.Afdeling);
		}

		/// <summary>
		/// Maakt/bewerkt een AfdelingsJaar: 
		/// andere OfficieleAfdeling en/of andere leeftijden
		/// </summary>
		/// <param name="detail">AfdelingsJaarDetail met de gegevens over het aan te maken of te wijzigen
		/// afdelingsjaar.  <c>detail.AfdelingsJaarID</c> bepaat of het om een bestaand afdelingsjaar gaat
		/// (ID > 0), of een bestaand (ID == 0)</param>
		public void AfdelingsJaarBewaren(AfdelingsJaarDetail detail)
		{
			AfdelingsJaar afdelingsJaar;

			Afdeling afd = _afdelingsJaarMgr.AfdelingOphalen(detail.AfdelingID);
			OfficieleAfdeling oa = _afdelingsJaarMgr.OfficieleAfdelingOphalen(detail.OfficieleAfdelingID);
			GroepsWerkJaar huidigGwj = _groepsWerkJaarManager.RecentsteOphalen(afd.Groep.ID);


			if (detail.AfdelingsJaarID == 0)
			{
				// nieuw maken.

				afdelingsJaar = _afdelingsJaarMgr.Aanmaken(
					afd,
					oa,
					huidigGwj,
					detail.GeboorteJaarVan, detail.GeboorteJaarTot,
					detail.Geslacht);
			}
			else
			{
				// wijzigen

				afdelingsJaar = _afdelingsJaarMgr.Ophalen(
					detail.AfdelingsJaarID,
					AfdelingsJaarExtras.OfficieleAfdeling | AfdelingsJaarExtras.Afdeling | AfdelingsJaarExtras.GroepsWerkJaar);

				if (afdelingsJaar.GroepsWerkJaar.ID != huidigGwj.ID
					|| afdelingsJaar.Afdeling.ID != detail.AfdelingID)
				{
					throw new NotSupportedException("Afdeling en GroepsWerkJaar mogen niet"
						+ " gewijzigd worden.");
				}

				_afdelingsJaarMgr.Wijzigen(
					afdelingsJaar,
					_afdelingsJaarMgr.OfficieleAfdelingOphalen(detail.OfficieleAfdelingID),
					detail.GeboorteJaarVan,
					detail.GeboorteJaarTot,
					detail.Geslacht,
					detail.VersieString);

			}

			_afdelingsJaarMgr.Bewaren(afdelingsJaar);

			// TODO: Concurrency exception catchen
		}

		/// <summary>
		/// Verwijdert een afdelingsjaar
		/// en controleert of er geen leden in zitten.
		/// </summary>
		/// <param name="afdelingsJaarID">ID van het afdelingsjaar dat verwijderd moet worden</param>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void AfdelingsJaarVerwijderen(int afdelingsJaarID)
		{
			try
			{
				_afdelingsJaarMgr.Verwijderen(afdelingsJaarID);
			}catch(InvalidOperationException)
			{
				/*var afdjaar = _afdelingsJaarMgr.Ophalen(afdelingsJaarID, AfdelingsJaarExtras.Afdeling);
				var afdjaardetail = Mapper.Map<AfdelingsJaar, AfdelingsJaarDetail>(afdjaar);*/
				throw new FaultException<GapFault>(new GapFault { FoutNummer = FoutNummer.AfdelingNietLeeg });
			}
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public AfdelingsJaarDetail AfdelingsJaarOphalen(int afdelingsJaarID)
		{
			AfdelingsJaar aj = _afdelingsJaarMgr.Ophalen(
				afdelingsJaarID,
				AfdelingsJaarExtras.Afdeling | AfdelingsJaarExtras.OfficieleAfdeling);

			return Mapper.Map<AfdelingsJaar, AfdelingsJaarDetail>(aj);
		}

		/// <summary>
		/// Haalt details op van een afdeling, gebaseerd op het <paramref name="afdelingsJaarID"/>
		/// </summary>
		/// <param name="afdelingsJaarID">ID van het AFDELINGSJAAR waarvoor de details opgehaald moeten 
		/// worden.</param>
		/// <returns>De details van de afdeling in het gegeven afdelingsjaar.</returns>
		public AfdelingDetail AfdelingDetailOphalen(int afdelingsJaarID)
		{
			AfdelingsJaar aj = _afdelingsJaarMgr.Ophalen(
				afdelingsJaarID,
				AfdelingsJaarExtras.Afdeling | AfdelingsJaarExtras.OfficieleAfdeling);

			return Mapper.Map<AfdelingsJaar, AfdelingDetail>(aj);
		}


		/// <summary>
		/// Haat een afdeling op, op basis van <paramref name="afdelingID"/>
		/// </summary>
		/// <param name="afdelingID">ID van op te halen afdeling</param>
		/// <returns>Info van de gevraagde afdeling</returns>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public AfdelingInfo AfdelingOphalen(int afdelingID)
		{
			Afdeling a = _afdelingsJaarMgr.AfdelingOphalen(afdelingID);
			return Mapper.Map<Afdeling, AfdelingInfo>(a);
		}

		/// <summary>
		/// Haalt informatie op over alle actieve afdelingen in het groepswerkjaar met 
		/// ID <paramref name="groepsWerkJaarID"/>
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <returns>
		/// Informatie over alle actieve afdelingen in het groepswerkjaar met 
		/// ID <paramref name="groepsWerkJaarID"/>
		/// </returns>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<AfdelingDetail> AfdelingenOphalen(int groepsWerkJaarID)
		{
			var groepswerkjaar = _groepsWerkJaarManager.Ophalen(groepsWerkJaarID, GroepsWerkJaarExtras.Afdelingen | GroepsWerkJaarExtras.Leden);
			return Mapper.Map<IList<AfdelingsJaar>, IList<AfdelingDetail>>(groepswerkjaar.AfdelingsJaar.OrderBy(e => e.GeboorteJaarVan).ToList());
		}

		/// <summary>
		/// Haalt beperkte informatie op over de beschikbare afdelingen van een groep in het huidige
		/// groepswerkjaar.
		/// </summary>
		/// <param name="groepID">ID van de groep waarvoor de afdelingen gevraagd zijn</param>
		/// <returns>Lijst van ActieveAfdelingInfo</returns>
		public IList<ActieveAfdelingInfo> BeschikbareAfdelingenOphalen(int groepID)
		{
			var gwj = _groepsWerkJaarManager.RecentsteOphalen(groepID, GroepsWerkJaarExtras.Afdelingen);
			return Mapper.Map<IEnumerable<AfdelingsJaar>, IList<ActieveAfdelingInfo>>(gwj.AfdelingsJaar);
		}

		/// <summary>
		/// Haalt informatie op over de afdelingen van een groep die niet gebruikt zijn in een gegeven 
		/// groepswerkjaar, op basis van een <paramref name="groepswerkjaarID"/>
		/// </summary>
		/// <param name="groepswerkjaarID">ID van het groepswerkjaar waarvoor de niet-gebruikte afdelingen
		/// opgezocht moeten worden.</param>
		/// <returns>Info over de ongebruikte afdelingen van een groep in het gegeven groepswerkjaar</returns>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<AfdelingInfo> OngebruikteAfdelingenOphalen(int groepswerkjaarID)
		{
			IList<Afdeling> ongebruikteAfdelingen = _groepsWerkJaarManager.OngebruikteAfdelingenOphalen(groepswerkjaarID);
			return Mapper.Map<IList<Afdeling>, IList<AfdelingInfo>>(ongebruikteAfdelingen);
		}

		#endregion

		#region Functies

		/// <summary>
		/// Haalt uit groepswerkjaar met ID <paramref name="groepsWerkJaarID"/> alle beschikbare functies
		/// op voor een lid van type <paramref name="lidType"/>.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar van de gevraagde functies</param>
		/// <param name="lidType"><c>LidType.Kind</c> of <c>LidType.Leiding</c></param>
		/// <returns>De gevraagde lijst afdelingsinfo</returns>
        public IEnumerable<FunctieInfo> FunctiesOphalen(int groepsWerkJaarID, LidType lidType)
		{
			var relevanteFuncties = _functiesMgr.OphalenRelevant(groepsWerkJaarID, lidType);
		    return Mapper.Map<IList<Functie>, IList<FunctieInfo>>(relevanteFuncties);
		}

		/// <summary>
		/// Zoekt naar problemen ivm de maximum- en minimumaantallen van functies voor het
		/// huidige werkjaar.
		/// </summary>
		/// <param name="groepID">ID van de groep waarvoor de functies gecontroleerd moeten worden.</param>
		/// <returns>
		/// Indien er problemen zijn, wordt een rij FunctieProbleemInfo opgeleverd.
		/// </returns>
		public IEnumerable<FunctieProbleemInfo> FunctiesControleren(int groepID)
		{
			GroepsWerkJaar gwj = _groepsWerkJaarManager.RecentsteOphalen(
				groepID, GroepsWerkJaarExtras.GroepsFuncties | GroepsWerkJaarExtras.LidFuncties);

			IEnumerable<Telling> problemen = _functiesMgr.AantallenControleren(gwj);

			// Blijkbaar kan ik hier niet anders dan de functies weer ophalen.

			var resultaat = (from p in problemen
							 let f = _functiesMgr.Ophalen(p.ID)
							 select new FunctieProbleemInfo
										{
											Code = f.Code,
											EffectiefAantal = p.Aantal,
											ID = f.ID,
											MaxAantal = p.Max,
											MinAantal = p.Min,
											Naam = f.Naam
										}).ToList();

			// Ter info: return resultaat.ToArray() werkt niet; problemen met (de)serializeren?

			return resultaat.ToList();
		}

		/// <summary>
		/// Maakt een nieuwe Functie voor de groep met ID <paramref name="groepID"/>
		/// </summary>
		/// <param name="groepID">ID van de groep waarvoor nieuwe functie wordt gemaakt</param>
		/// <param name="naam">Naam voor de nieuwe functie</param>
		/// <param name="code">Code voor de nieuwe functie</param>
		/// <param name="maxAantal">Eventueel het maximumaantal leden met die functie in een werkjaar</param>
		/// <param name="minAantal">Het minimumaantal leden met die functie in een werkjaar</param>
		/// <param name="lidType">Gaat het over een functie voor leden, leiding of beide?</param>
		/// <param name="werkJaarVan">Eventueel het vroegste werkjaar waarvoor de functie beschikbaar moet zijn</param>
		/// <returns></returns>
		public int FunctieToevoegen(int groepID, string naam, string code, int? maxAantal, int minAantal, LidType lidType, int? werkJaarVan)
		{
			Groep g = _groepenMgr.OphalenMetFuncties(groepID);
			try
			{
				_groepenMgr.FunctieToevoegen(g, naam, code, maxAantal, minAantal, lidType, werkJaarVan);
			}
			catch (BestaatAlException<Functie> ex)
			{
				var fault = Mapper.Map<BestaatAlException<Functie>,
						BestaatAlFault<FunctieInfo>>(ex);

				throw new FaultException<BestaatAlFault<FunctieInfo>>(fault);
			}
// ReSharper disable RedundantCatchClause
			catch (Exception)
			{
				// ********************************************************************************
				// * BELANGRIJK: Als je debugger breakt op deze throw, dan is dat geen probleem.  *
				// * Dat wil gewoon zeggen dat er een bestaande Functie gevonden is, met        *
				// * dezelfde code als de nieuwe.  Er gaat een faultexception over de lijn,       *
				// * die door de UI gecatcht moet worden.  Druk gewoon F5 om verder te gaan.      *
				// ********************************************************************************

				throw;
			}
// ReSharper restore RedundantCatchClause

			g = _groepenMgr.Bewaren(g, e => e.Functie);

			return (from ctg in g.Functie
					where ctg.Code == code
					select ctg.ID).FirstOrDefault();
		}

		/// <summary>
		/// Verwijdert de functie met gegeven <paramref name="functieID"/>
		/// </summary>
		/// <param name="functieID">ID van de te verwijderen functie</param>
		/// <param name="forceren">Indien <c>true</c>, worden eventuele personen uit de
		/// te verwijderen functie eerst uit de functie weggehaald.  Indien
		/// <c>false</c> krijg je een exception als de functie niet leeg is.</param>
		public void FunctieVerwijderen(int functieID, bool forceren)
		{
			// Personen moeten mee opgehaald worden; anders werkt 
			// functieenManager.Verwijderen niet.

			Functie f = _functiesMgr.Ophalen(functieID, true);
			
			try
			{
				_functiesMgr.Verwijderen(f, forceren);
			}
			catch (BlokkerendeObjectenException<Lid> ex)
			{
				var fault = Mapper.Map<BlokkerendeObjectenException<Lid>,
					BlokkerendeObjectenFault<PersoonLidInfo>>(ex);

				throw new FaultException<BlokkerendeObjectenFault<PersoonLidInfo>>(fault);
			}
		}


		#endregion

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IEnumerable<WerkJaarInfo> WerkJarenOphalen(int groepsID)
		{
			var werkjaren = (from gwj in _groepenMgr.OphalenMetGroepsWerkJaren(groepsID).GroepsWerkJaar
							 orderby gwj.WerkJaar descending
							 select gwj);

			return Mapper.Map<IEnumerable<GroepsWerkJaar>, IEnumerable<WerkJaarInfo>>(werkjaren);
		}

		#region categorieën

		/// <summary>
		/// Maakt een nieuwe categorie voor de groep met ID <paramref name="groepID"/>
		/// </summary>
		/// <param name="groepID">ID van de groep waarvoor nieuwe categorie wordt gemaakt</param>
		/// <param name="naam">Naam voor de nieuwe categorie</param>
		/// <param name="code">Code voor de nieuwe categorie</param>
		/// <returns></returns>
		public int CategorieToevoegen(int groepID, string naam, string code)
		{
			Groep g = _groepenMgr.OphalenMetCategorieen(groepID);
			try
			{
				_groepenMgr.CategorieToevoegen(g, naam, code);
			}
			catch (BestaatAlException<Categorie> ex)
			{
				var fault = Mapper.Map<BestaatAlException<Categorie>,
						BestaatAlFault<CategorieInfo>>(ex);

				throw new FaultException<BestaatAlFault<CategorieInfo>>(fault);
			}
// ReSharper disable RedundantCatchClause
			catch (Exception)
			{
				// ********************************************************************************
				// * BELANGRIJK: Als je debugger breakt op deze throw, dan is dat geen probleem.  *
				// * Dat wil gewoon zeggen dat er een bestaande categorie gevonden is, met        *
				// * dezelfde code als de nieuwe.  Er gaat een faultexception over de lijn,       *
				// * die door de UI gecatcht moet worden.  Druk gewoon F5 om verder te gaan.      *
				// ********************************************************************************

				throw;
			}
// ReSharper restore RedundantCatchClause

			g = _groepenMgr.Bewaren(g, e => e.Categorie);

			return (from ctg in g.Categorie
					where ctg.Code == code
					select ctg.ID).FirstOrDefault();
		}

		/// <summary>
		/// Verwijdert de categorie met gegeven <paramref name="categorieID"/>
		/// </summary>
		/// <param name="categorieID">ID van de te verwijderen categorie</param>
		/// <param name="forceren">Indien <c>true</c>, worden eventuele personen uit de
		/// te verwijderen categorie eerst uit de categorie weggehaald.  Indien
		/// <c>false</c> krijg je een exception als de categorie niet leeg is.</param>
		public void CategorieVerwijderen(int categorieID, bool forceren)
		{
			// Personen moeten mee opgehaald worden; anders werkt 
			// CategorieenManager.Verwijderen niet.

			Categorie c = _categorieenMgr.Ophalen(categorieID, true);

			try
			{
				_categorieenMgr.Verwijderen(c, forceren);
			}
			catch (BlokkerendeObjectenException<GelieerdePersoon> ex)
			{
				var fault = Mapper.Map<BlokkerendeObjectenException<GelieerdePersoon>,
					BlokkerendeObjectenFault<PersoonDetail>>(ex);

				throw new FaultException<BlokkerendeObjectenFault<PersoonDetail>>(fault);
			}
		}

		/// <summary>
		/// Past de naam van een categorie aan
		/// </summary>
		/// <param name="categorieID">De ID van de categorie waar het over gaat</param>
		/// <param name="nieuwenaam">De nieuwe naam die de categorie moet krijgen</param>
		public void CategorieAanpassen(int categorieID, string nieuwenaam)
		{
			/*Groep g = OphalenMetCategorieen(groepID);
			Categorie c = null;*/
			throw new NotImplementedException();
		}

		/// <summary>
		/// Zoekt de categorieID op van de categorie bepaald door de gegeven 
		/// <paramref name="groepID"/> en <paramref name="code"/>.
		/// </summary>
		/// <param name="groepID">ID van groep waaraan de gezochte categorie gekoppeld is</param>
		/// <param name="code">Code van de te zoeken categorie</param>
		/// <returns>Het categorieID als de categorie gevonden is, anders 0.</returns>
		public int CategorieIDOphalen(int groepID, string code)
		{
			Categorie cat = _categorieenMgr.Ophalen(groepID, code);
			return (cat == null) ? 0 : cat.ID;
		}

		/// <summary>
		/// Zoekt een categorie op, op basis van <paramref name="groepID"/> en
		/// <paramref name="categorieCode"/>
		/// </summary>
		/// <param name="groepID">ID van de groep waaraan de categorie gekoppeld moet zijn.</param>
		/// <param name="categorieCode">Code van de categorie</param>
		/// <returns>De categorie met code <paramref name="categorieCode"/> die van toepassing is op
		/// de groep met ID <paramref name="groepID"/>.</returns>
		public CategorieInfo CategorieOpzoeken(int groepID, string categorieCode)
		{
			Mapper.CreateMap<Categorie, CategorieInfo>();
			return Mapper.Map<Categorie, CategorieInfo>(_categorieenMgr.Ophalen(groepID, categorieCode));
		}

		/// <summary>
		/// Haalt alle categorieeen op van de groep met ID <paramref name="groepID"/>
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan de categorieen zijn gevraagd</param>
		/// <returns>Lijst met categorie-info van de categorieen van de gevraagde groep</returns>
		public IList<CategorieInfo> CategorieenOphalen(int groepID)
		{
			var result = _categorieenMgr.AllesOphalen(groepID);

			return Mapper.Map<IList<Categorie>, IList<CategorieInfo>>(result);
		}


		#endregion categorieën

		#region adressen
		/// <summary>
		/// Maakt een lijst met alle deelgemeentes uit de database; nuttig voor autocompletion
		/// in de ui.
		/// </summary>
		/// <returns>Lijst met alle beschikbare deelgemeentes</returns>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IEnumerable<WoonPlaatsInfo> GemeentesOphalen()
		{
			return Mapper.Map<IEnumerable<WoonPlaats>, IList<WoonPlaatsInfo>>(_adresMgr.GemeentesOphalen());
		}

		/// <summary>
		/// Haalt alle straten op uit een gegeven <paramref name="postNr"/>, waarvan de naam begint
		/// met het gegeven <paramref name="straatBegin"/>.
		/// </summary>
		/// <param name="straatBegin">Eerste letters van de te zoeken straatnamen</param>
		/// <param name="postNr">Postnummer waarin te zoeken</param>
		/// <returns>Gegevens van de gevonden straten</returns>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IEnumerable<StraatInfo> StratenOphalen(String straatBegin, int postNr)
		{
			return Mapper.Map<IList<StraatNaam>, IList<StraatInfo>>(_adresMgr.StratenOphalen(straatBegin, postNr));
		}

		/// <summary>
		/// Haalt alle straten op uit een gegeven rij <paramref name="postNrs"/>, waarvan de naam begint
		/// met het gegeven <paramref name="straatBegin"/>.
		/// </summary>
		/// <param name="straatBegin">Eerste letters van de te zoeken straatnamen</param>
		/// <param name="postNrs">Postnummers waarin te zoeken</param>
		/// <returns>Gegevens van de gevonden straten</returns>
		/// <remarks>Ik had deze functie ook graag StratenOphalen genoemd, maar je mag geen 2 
		/// WCF-functies met dezelfde naam in 1 service hebben.  Spijtig.</remarks>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IEnumerable<StraatInfo> StratenOphalenMeerderePostNrs(String straatBegin, IEnumerable<int> postNrs)
		{
			return Mapper.Map<IList<StraatNaam>, IList<StraatInfo>>(_adresMgr.StratenOphalen(straatBegin, postNrs));
		}


		/// <summary>
		/// Deze method geeft gewoon de gebruikersnaam weer waaronder je de service aanroept.  Vooral om de
		/// authenticate te testen.
		/// </summary>
		/// <returns>Gebruikersnaam waarmee aangemeld</returns>
		public string WieBenIk()
		{
			return _autorisatieMgr.GebruikersNaamGet();
		}

		#endregion
	}
}
