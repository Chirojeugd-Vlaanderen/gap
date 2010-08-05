// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;

using AutoMapper;

using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.Services.Properties;
using Chiro.Gap.Workers;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Services
{
	// OPM: als je de naam van de class "GelieerdePersonenService" hier verandert, moet je ook de sectie "Services" in web.config aanpassen.

	// *BELANGRIJK*: Als het debuggen hier stopt owv een autorisatiefout, kijk dan na of de gebruiker waarmee
	// je aangemeld bent, op je lokale computer in de groep CgUsers zit.

	/// <summary>
	/// Service voor operaties op gelieerde personen
	/// </summary>
	public class GelieerdePersonenService : IGelieerdePersonenService
	{
		#region Manager Injection

		private readonly GelieerdePersonenManager _gpMgr;
		private readonly PersonenManager _pMgr;
		private readonly LedenManager _lidMgr;
		private readonly AdressenManager _adrMgr;
		private readonly GroepenManager _grMgr;
		private readonly GroepsWerkJaarManager _gwjMgr;
		private readonly CommVormManager _cvMgr;
		private readonly CategorieenManager _catMgr;
		private readonly IAutorisatieManager _auMgr;

		/// <summary>
		/// Constructor met via IoC toegekende workers
		/// </summary>
		/// <param name="gpm">De worker voor GelieerdePersonen</param>
		/// <param name="pm">De worker voor Personen</param>
		/// <param name="adm">De worker voor Adressen</param>
		/// <param name="gm">De worker voor Groepen</param>
		/// <param name="gwjm">De worker voor GroepsWerkJaren</param>
		/// <param name="cvm">De worker voor CommunicatieVormen</param>
		/// <param name="cm">De worker voor Categorieën</param>
		/// <param name="aum">De worker voor Autorisatie</param>
		/// <param name="lm">De worker voor Leden</param>
		public GelieerdePersonenService(
			GelieerdePersonenManager gpm,
			PersonenManager pm,
			AdressenManager adm,
			GroepenManager gm,
			GroepsWerkJaarManager gwjm,
			CommVormManager cvm,
			CategorieenManager cm,
			IAutorisatieManager aum,
			LedenManager lm)
		{
			_gpMgr = gpm;
			_pMgr = pm;
			_auMgr = aum;
			_adrMgr = adm;
			_grMgr = gm;
			_gwjMgr = gwjm;
			_cvMgr = cvm;
			_catMgr = cm;
			_lidMgr = lm;
		}

		#endregion

		#region IGelieerdePersonenService Members

		#region Bewaren
		
		/// <summary>
		/// Updatet een persoon op basis van <paramref name="persoonInfo"/>
		/// </summary>
		/// <param name="persoonInfo">Info over te bewaren persoon</param>
		/// <returns>ID van de bewaarde persoon</returns>
		public int Bewaren(PersoonInfo persoonInfo)
		{
			try
			{
				// Haal eerst gelieerde persoon op.
				var gp = _gpMgr.Ophalen(persoonInfo.GelieerdePersoonID);
				gp.ChiroLeefTijd = persoonInfo.ChiroLeefTijd;

				Mapper.Map(persoonInfo, gp.Persoon);
				// In de hoop dat de members die geen 'Ignore hebben' overschreven worden,
				// en de andere niet.

				_gpMgr.Bewaren(gp, PersoonsExtras.Geen);

				return persoonInfo.GelieerdePersoonID;
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				return 0;
			}
		}

		/// <summary>
		/// Maakt een nieuwe persoon aan, en koppelt die als gelieerde persoon aan de groep met gegeven
		/// <paramref>groepID</paramref>
		/// </summary>
		/// <param name="info">Informatie om de nieuwe (gelieerde) persoon te construeren: Chiroleeftijd, en
		/// de velden van <c>info.Persoon</c></param>
		/// <param name="groepID">ID van de groep waaraan de nieuwe persoon gekoppeld moet worden</param>
		/// <returns>ID's van de bewaarde persoon en gelieerde persoon</returns>
		/// <remarks>Adressen, Communicatievormen,... worden niet mee gepersisteerd; enkel de persoonsinfo
		/// en de Chiroleeftijd.</remarks>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IDPersEnGP Aanmaken(PersoonInfo info, int groepID)
		{
			return GeforceerdAanmaken(info, groepID, false);
		}

		/// <summary>
		/// Maakt een nieuwe persoon aan, en koppelt die als gelieerde persoon aan de groep met gegeven
		/// <paramref>groepID</paramref>
		/// </summary>
		/// <param name="info">Informatie om de nieuwe (gelieerde) persoon te construeren: Chiroleeftijd, en
		/// de velden van <c>info.Persoon</c></param>
		/// <param name="groepID">ID van de groep waaraan de nieuwe persoon gekoppeld moet worden</param>
		/// <returns>ID's van de bewaarde persoon en gelieerde persoon</returns>
		/// <param name="forceer">Als deze <c>true</c> is, wordt de nieuwe persoon sowieso gemaakt, ook
		/// al lijkt hij op een bestaande gelieerde persoon.  Is <paramref>force</paramref>
		/// <c>false</c>, dan wordt er een exceptie opgegooid als de persoon te hard lijkt op een
		/// bestaande.</param>
		/// <remarks>Adressen, Communicatievormen,... worden niet mee gepersisteerd; enkel de persoonsinfo
		/// en de Chiroleeftijd.</remarks>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IDPersEnGP GeforceerdAanmaken(PersoonInfo info, int groepID, bool forceer)
		{
			// Indien 'forceer' niet gezet is, moet een FaultException opgeworpen worden
			// als de  nieuwe persoon te hard lijkt op een bestaande Gelieerde Persoon.

			// FIXME: Deze businesslogica moet in de workers gebeuren, waar dan een exception opgeworpen
			// kan worden, die we hier mappen op een faultcontract.

			var nieuwePersoon = new Persoon
			{
				AdNummer = info.AdNummer,
				VoorNaam = info.VoorNaam,
				Naam = info.Naam,
				GeboorteDatum = info.GeboorteDatum,
				Geslacht = info.Geslacht
			};

			if (!forceer)
			{
				IList<GelieerdePersoon> bestaandePersonen =
					_gpMgr.ZoekGelijkaardig(nieuwePersoon, groepID);

				if (bestaandePersonen.Count > 0)
				{
					var fault = new BlokkerendeObjectenFault<PersoonDetail>
					{
						Objecten = Mapper.Map<IList<GelieerdePersoon>, IList<PersoonDetail>>(bestaandePersonen)
					};

					throw new FaultException<BlokkerendeObjectenFault<PersoonDetail>>(fault);

					// ********************************************************************************
					// * BELANGRIJK: Als je debugger breakt op deze throw, dan is dat geen probleem.  *
					// * Dat wil gewoon zeggen dat er een gelieerde persoon gevonden is die lijkt op  *
					// * de nieuw toe te voegen persoon.  Er gaat een faultexception over de lijn,    *
					// * die door de UI gecatcht moet worden.                                         *
					// ********************************************************************************
				}
			}

			// De parameter 'info' wordt hier eigenlijk niet gebruikt als GelieerdePersoon,
			// maar als datacontract dat de persoonsinfo en de Chiroleeftijd bevat.

			Groep g = _grMgr.Ophalen(groepID);

			// Gebruik de businesslaag om info.Persoon te koppelen aan de opgehaalde groep.

			GelieerdePersoon gelieerd = _gpMgr.Koppelen(nieuwePersoon, g, info.ChiroLeefTijd);
			gelieerd = _gpMgr.Bewaren(gelieerd, PersoonsExtras.Groep);
			return new IDPersEnGP { GelieerdePersoonID = gelieerd.ID, PersoonID = gelieerd.Persoon.ID };
		}
		#endregion

		#region Ophalen

		/// <summary>
		/// Haalt gelieerde personen op die in een bepaalde categorie zitten, met lidinfo, 
		/// volgens de pagineringsparameters,
		/// en telt over hoeveel personen het gaat
		/// </summary>
		/// <param name="categorieID">De ID van de categorie waartoe de gelieerde personen moeten behoren</param>
		/// <param name="pagina">Het volgnummer van de 'pagina' die we willen bekijken</param>
		/// <param name="paginaGrootte">Het aantal personen dat er per pagina weergegeven moet worden</param>
		/// <param name="sortering">De parameter waarop de gegevens gesorteerd moeten worden</param>
		/// <param name="aantalTotaal">Het totaal aantal personen in de opgegeven categorie</param>
		/// <returns>Een lijst van persoonsgegevens</returns>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<PersoonDetail> PaginaOphalenUitCategorieMetLidInfo(int categorieID, int pagina, int paginaGrootte, PersoonSorteringsEnum sortering, out int aantalTotaal)
		{
			try
			{
				var gelieerdePersonen = _gpMgr.PaginaOphalenUitCategorie(
					categorieID,
					pagina,
					paginaGrootte,
					sortering,
					PersoonsExtras.Categorieen,
					true,
					out aantalTotaal);
				return Mapper.Map<IEnumerable<GelieerdePersoon>, IList<PersoonDetail>>(gelieerdePersonen);
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				aantalTotaal = 0;
				return null;
			}
		}
	
		/// <summary>
		/// Haalt gelieerde personen op, met lidinfo, 
		/// volgens de pagineringsparameters,
		/// en telt over hoeveel personen het gaat
		/// </summary>
		/// <param name="groepID">De ID van de groep waartoe de gelieerde personen moeten behoren</param>
		/// <param name="pagina">Het volgnummer van de 'pagina' die we willen bekijken</param>
		/// <param name="paginaGrootte">Het aantal personen dat er per pagina weergegeven moet worden</param>
		/// <param name="sortering">De parameter waarop de gegevens gesorteerd moeten worden</param>
		/// <param name="aantalTotaal">Het totaal aantal personen in de opgegeven categorie</param>
		/// <returns>Een lijst van persoonsgegevens</returns>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<PersoonDetail> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, PersoonSorteringsEnum sortering, out int aantalTotaal)
		{
			try
			{
				var gelieerdePersonen = _gpMgr.PaginaOphalenMetLidInfo(groepID, pagina, paginaGrootte, sortering, out aantalTotaal);
				var result = Mapper.Map<IEnumerable<GelieerdePersoon>, IList<PersoonDetail>>(gelieerdePersonen);

				/*
				 * TODO dit staat mss niet op de beste plek
				 * Ophalen afdelingsjaren in het huidige werkjaar (TODO niet als een vorig werkjaar bekeken wordt)
				 * Voor elk persoonsdetail kijken of iemand die nog geen lid is, in een afdeling zou passen
				 * als dit het geval is, kanlidworden op true zetten.
				 * kanleidingworden wordt true als de persoon de juiste leeftijd heeft
				 * 
				 * TODO dubbele code in detailsophalen
				 */
				GroepsWerkJaar gwj = _gwjMgr.RecentsteOphalen(groepID, GroepsWerkJaarExtras.Afdelingen);
				foreach (var p in result)
				{
					if (p.GeboorteDatum == null)
					{
						continue;
					}
					int geboortejaar = p.GeboorteDatum.Value.Year - p.ChiroLeefTijd;
					var afd = (from a in gwj.AfdelingsJaar
							   where a.GeboorteJaarTot >= geboortejaar && a.GeboorteJaarVan <= geboortejaar
							   select a).FirstOrDefault();
					if (afd != null)
					{
						p.KanLidWorden = true;
					}
					if (p.GeboorteDatum.Value.Year < DateTime.Today.Year - Int32.Parse(Resources.LeidingVanafLeeftijd) + p.ChiroLeefTijd)
					{
						p.KanLeidingWorden = true;
					}
				}
				return result;
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				aantalTotaal = 0;
				return null;
			}
		}

		/// <summary>
		/// Haalt gelieerd persoon op, incl. persoonsgegevens, communicatievormen en adressen
		/// </summary>
		/// <param name="gelieerdePersoonID">ID op te halen GelieerdePersoon</param>
		/// <returns>GelieerdePersoon met persoonsgegevens</returns>
		public PersoonDetail DetailsOphalen(int gelieerdePersoonID)
		{
			try
			{
				return Mapper.Map<GelieerdePersoon, PersoonDetail>(_gpMgr.DetailsOphalen(gelieerdePersoonID));
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				return null;
			}
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
			try
			{
				// TODO: Dit moet properder!

				var gp = _gpMgr.DetailsOphalen(gelieerdePersoonID);

				// TODO: Bovenstaande haalt alle lidobjecten ever van de gelieerde persoon op.  Dat is ofwel een bug
				// in DetailsOphalen, ofwel een bug in deze method.  Ik vermoed het eerste.

				var pl = Mapper.Map<GelieerdePersoon, PersoonLidInfo>(gp);
				var gwj = _gwjMgr.RecentsteOphalen(gp.Groep.ID, GroepsWerkJaarExtras.GroepsFuncties | GroepsWerkJaarExtras.Afdelingen);

				var l = _lidMgr.OphalenViaPersoon(gp.ID, gwj.ID);
				if (l != null)
				{
					// Een beetje foefelare et trukare: het lidobject koppelen aan de opgehaalde
					// gelieerde persoon, zodat we bij het mappen meteen weten of het lid verzekerd is tegen
					// loonverlies

					l.GelieerdePersoon = gp;
					gp.Lid.Add(l);

					var ff = Mapper.Map<Lid, LidInfo>(l);
					pl.LidInfo = ff;
				}

				var p = pl.PersoonDetail;
				if (p.GeboorteDatum != null)
				{
					int geboortejaar = p.GeboorteDatum.Value.Year - p.ChiroLeefTijd;
					var afd = (from a in gwj.AfdelingsJaar
							   where a.GeboorteJaarTot >= geboortejaar && a.GeboorteJaarVan <= geboortejaar
							   select a).FirstOrDefault();
					if (afd != null)
					{
						p.KanLidWorden = true;
					}
					if (geboortejaar < DateTime.Today.Year - Int32.Parse(Resources.LeidingVanafLeeftijd) + p.ChiroLeefTijd)
					{
						p.KanLeidingWorden = true;
					}
				}

				return pl;
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				return null;
			}
		}

		/// <summary>
		/// Haalt gegevens op van alle personen uit categorie met ID <paramref name="categorieID"/>
		/// </summary>
		/// <param name="categorieID">Indien verschillend van 0, worden alle personen uit de categore met
		/// gegeven CategoreID opgehaald.  Anders alle personen tout court.</param>
		/// <param name="sortering">Geeft aan hoe de pagina gesorteerd moet worden</param>
		/// <returns>Lijst 'PersoonOverzicht'-objecten van alle gelieerde personen uit de categorie</returns>
		public IEnumerable<PersoonOverzicht> AllenOphalenUitCategorie(int categorieID, PersoonSorteringsEnum sortering)
		{
			try
			{
				int totaal;

				var gelieerdePersonen = _gpMgr.PaginaOphalenUitCategorie(
					categorieID,
					1,
					int.MaxValue,
					sortering,
					PersoonsExtras.Adressen | PersoonsExtras.Communicatie,
					false,
					out totaal);

				return Mapper.Map<IEnumerable<GelieerdePersoon>, IEnumerable<PersoonOverzicht>>(gelieerdePersonen);
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				return null;
			}
		}

		/// <summary>
		/// Haalt gegevens op van alle personen uit groep met ID <paramref name="groepID"/>.
		/// </summary>
		/// <param name="groepID">ID van de groep waaruit de personen gehaald moeten worden</param>
		/// <param name="sortering">Geeft aan hoe de pagina gesorteerd moet worden</param>
		/// <returns>Rij 'PersoonOverzicht'-objecten van alle gelieerde personen uit de groep.</returns>
		public IEnumerable<PersoonOverzicht> AllenOphalenUitGroep(int groepID, PersoonSorteringsEnum sortering)
		{
			try
			{
				var gelieerdePersonen = _gpMgr.AllenOphalen(groepID, PersoonsExtras.Adressen | PersoonsExtras.Communicatie, sortering);
				return Mapper.Map<IEnumerable<GelieerdePersoon>, IEnumerable<PersoonOverzicht>>(gelieerdePersonen);
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				return null;
			}
		}

		#endregion

		#region Gezinnen en adressen

		/// <summary>
		/// Haalt adres op, met daaraan gekoppeld de bewoners (gelieerde personen) uit de groep met ID <paramref name="groepID"/>.
		/// </summary>
		/// <param name="adresID">ID op te halen adres</param>
		/// <param name="groepID">ID van de groep</param>
		/// <returns>Adresobject met gekoppelde personen</returns>
		/// <remarks>GelieerdePersoonID's van bewoners worden niet mee opgehaald</remarks>
		public GezinInfo GezinOphalen(int adresID, int groepID)
		{
			try
			{
				var adres = _adrMgr.AdresMetBewonersOphalen(adresID, groepID);
				var resultaat = Mapper.Map<Adres, GezinInfo>(adres);

				return resultaat;
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				return null;
			}
		}

		/// <summary>
		/// Verhuist personen van een oud naar een nieuw
		/// adres.
		/// (De koppelingen Persoon-Oudadres worden aangepast 
		/// naar Persoon-NieuwAdres.)
		/// </summary>
		/// <param name="persoonIDs">ID's van te verhuizen Personen (niet gelieerd!)</param>
		/// <param name="naarAdres">AdresInfo-object met nieuwe adresgegevens</param>
		/// <param name="oudAdresID">ID van het oude adres</param>
		/// <remarks>
		/// (1) nieuwAdres.ID wordt genegeerd.  Het adresID wordt altijd
		/// opnieuw opgezocht in de bestaande adressen.  Bestaat het adres nog niet,
		/// dan krijgt het adres een nieuw ID. 
		/// <para/>
		/// (2) Deze functie werkt op PersoonID's en niet op
		/// GelieerdePersoonID's, en bijgevolg hoort dit eerder thuis
		/// in een PersonenService dan in een GelieerdePersonenService.
		/// </remarks>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void PersonenVerhuizen(IEnumerable<int> persoonIDs,
										PersoonsAdresInfo naarAdres,
										int oudAdresID)
		{
			// TODO: Dit lijkt te veel op 'GelieerdePersonenVerhuizen'.  Bovendien wordt deze functie waarschijnlijk
			// niet meer gebruikt; ze mag weg.

			// Zoek adres op in database, of maak een nieuw.
			// (als straat en gemeente gekend)

			try
			{
				Adres nieuwAdres;
				try
				{
					nieuwAdres = _adrMgr.ZoekenOfMaken(
						naarAdres.StraatNaamNaam,
						naarAdres.HuisNr,
						naarAdres.Bus,
						naarAdres.WoonPlaatsNaam,
						naarAdres.PostNr,
						null);	// TODO: buitenlandse adressen (#238)
				}
				catch (OngeldigObjectException ex)
				{
					var fault = Mapper.Map<OngeldigObjectException, OngeldigObjectFault>(ex);

					throw new FaultException<OngeldigObjectFault>(fault);
				}

				// Haal te verhuizen personen op, samen met hun adressen.

				IEnumerable<Persoon> personenLijst = _pMgr.LijstOphalen(persoonIDs, PersoonsExtras.Adressen);

				// Kijk na of het naar-adres toevallig mee opgehaald is.  Zo ja, werken we daarmee verder
				// (iet of wat consistenter)

				PersoonsAdres a = personenLijst.SelectMany(prs => prs.PersoonsAdres)
					.Where(pa => pa.Adres.ID == nieuwAdres.ID).FirstOrDefault();

				if (a != null)
				{
					nieuwAdres = a.Adres;
				}

				// Het oud adres is normaal gezien gekoppeld aan een van de te verhuizen personen.

				Adres oudAdres = personenLijst.SelectMany(prs => prs.PersoonsAdres)
					.Where(pa => pa.Adres.ID == oudAdresID).Select(pa => pa.Adres).FirstOrDefault();

				try
				{
					_pMgr.Verhuizen(personenLijst, oudAdres, nieuwAdres, naarAdres.AdresType);
				}
				catch (BlokkerendeObjectenException<PersoonsAdres> ex)
				{
					var fault = Mapper.Map<BlokkerendeObjectenException<PersoonsAdres>,
						BlokkerendeObjectenFault<PersoonsAdresInfo2>>(ex);

					throw new FaultException<BlokkerendeObjectenFault<PersoonsAdresInfo2>>(fault);
				}

				// Persisteren
				_adrMgr.Bewaren(nieuwAdres);

				// Bij een verhuis, blijven de PersoonsAdresobjecten dezelfde,
				// maar worden ze aan een ander adres gekoppeld.  Een post
				// van het nieuwe adres (met persoonsadressen) koppelt bijgevolg
				// de persoonsobjecten los van het oude adres.
				// Bijgevolg moet het oudeAdres niet gepersisteerd worden.
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
			}
		}

		/// <summary>
		/// Verhuist gelieerde personen van een oud naar een nieuw adres
		/// (De koppelingen Persoon-Oudadres worden aangepast 
		/// naar Persoon-NieuwAdres.)
		/// </summary>
		/// <param name="gelieerdePersoonIDs">ID's van te verhuizen *GELIEERDE* Personen </param>
		/// <param name="naarAdres">AdresInfo-object met nieuwe adresgegevens</param>
		/// <param name="oudAdresID">ID van het oude adres</param>
		/// <remarks>nieuwAdres.ID wordt genegeerd.  Het adresID wordt altijd
		/// opnieuw opgezocht in de bestaande adressen.  Bestaat het adres nog niet,
		/// dan krijgt het adres een nieuw ID.</remarks>
		public void GelieerdePersonenVerhuizen(IEnumerable<int> gelieerdePersoonIDs,
												PersoonsAdresInfo naarAdres,
												int oudAdresID)
		{
			// TODO: Dit lijkt te veel op 'PersonenVerhuizen'.  'PersonenVerhuizen' mag weg.

			try
			{
				// Zoek adres op in database, of maak een nieuw.
				// (als straat en gemeente gekend)
				Adres nieuwAdres;

				try
				{
					nieuwAdres = _adrMgr.ZoekenOfMaken(
						naarAdres.StraatNaamNaam,
						naarAdres.HuisNr,
						naarAdres.Bus,
						naarAdres.WoonPlaatsNaam,
						naarAdres.PostNr,
						null);	// TODO: buitenlandse adressen (#238)
				}
				catch (OngeldigObjectException ex)
				{
					var fault = Mapper.Map<OngeldigObjectException, OngeldigObjectFault>(ex);

					throw new FaultException<OngeldigObjectFault>(fault);
				}

				// Haal te verhuizen personen op, samen met hun adressen.

				IEnumerable<Persoon> personenLijst = _pMgr.LijstOphalenViaGelieerdePersoon(gelieerdePersoonIDs, PersoonsExtras.Adressen);

				// Kijk na of het naar-adres toevallig mee opgehaald is.  Zo ja, werken we daarmee verder
				// (iet of wat consistenter)

				PersoonsAdres a = personenLijst.SelectMany(prs => prs.PersoonsAdres)
					.Where(pa => pa.Adres.ID == nieuwAdres.ID).FirstOrDefault();

				if (a != null)
				{
					nieuwAdres = a.Adres;
				}

				// Het oud adres is normaal gezien gekoppeld aan een van de te verhuizen personen.

				Adres oudAdres = personenLijst.SelectMany(prs => prs.PersoonsAdres)
					.Where(pa => pa.Adres.ID == oudAdresID).Select(pa => pa.Adres).FirstOrDefault();

				try
				{
					_pMgr.Verhuizen(personenLijst, oudAdres, nieuwAdres, naarAdres.AdresType);
				}
				catch (BlokkerendeObjectenException<PersoonsAdres> ex)
				{
					var fault = Mapper.Map<BlokkerendeObjectenException<PersoonsAdres>,
						BlokkerendeObjectenFault<PersoonsAdresInfo2>>(ex);

					throw new FaultException<BlokkerendeObjectenFault<PersoonsAdresInfo2>>(fault);
				}

				// Persisteren
				_adrMgr.Bewaren(nieuwAdres);

				// Bij een verhuis, blijven de PersoonsAdresobjecten dezelfde,
				// maar worden ze aan een ander adres gekoppeld.  Een post
				// van het nieuwe adres (met persoonsadressen) koppelt bijgevolg
				// de persoonsobjecten los van het oude adres.
				// Bijgevolg moet het oudeAdres niet gepersisteerd worden.
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
			}
		}

		/// <summary>
		/// Voegt een adres toe aan een verzameling *GELIEERDE* personen
		/// </summary>
		/// <param name="gelieerdePersonenIDs">ID's van de gelieerde personen
		/// waaraan het nieuwe adres toegevoegd moet worden.</param>
		/// <param name="adr">Toe te voegen adres</param>
		/// <param name="voorkeur"><c>True</c> als het nieuwe adres het voorkeursadres moet worden.</param>
		public void AdresToevoegenGelieerdePersonen(List<int> gelieerdePersonenIDs, PersoonsAdresInfo adr, bool voorkeur)
		{
			try
			{
				// Adres opzoeken in database
				Adres adres;
				try
				{
					adres = _adrMgr.ZoekenOfMaken(adr.StraatNaamNaam, adr.HuisNr, adr.Bus, adr.WoonPlaatsNaam, adr.PostNr, null);
				}
				catch (OngeldigObjectException ex)
				{
					var fault = Mapper.Map<OngeldigObjectException, OngeldigObjectFault>(ex);

					throw new FaultException<OngeldigObjectFault>(fault);
				}

				// Personen ophalen.  Haal ook gelieerde personen uit andere groepen op, omdat daarvan
				// mogelijk ook het voorkeursadres verandert (indien er bijv. geen voorkeursadres is)

				IEnumerable<Persoon> personenLijst = _pMgr.LijstOphalenViaGelieerdePersoon(
					gelieerdePersonenIDs, PersoonsExtras.Adressen | PersoonsExtras.AlleGelieerdePersonen);

				// Voor het adres te koppelen, gebruiken we enkel de gelieerde personen met ID's uit gelieerdePersonenIDs.  
				//   - het nieuwe adres wordt aan persoon gekoppeld, en niet aan gelieerde persoon
				//   - de parameter 'voorkeur' is van toepassing op de gp's met ID's uit gelieerdePersonenIDs

				var gpLijst = from gp in personenLijst.SelectMany(p => p.GelieerdePersoon)
							  where gelieerdePersonenIDs.Contains(gp.ID)
							  select gp;

				try
				{
					_gpMgr.AdresToevoegen(gpLijst, adres, adr.AdresType, voorkeur);
				}
				catch (BlokkerendeObjectenException<PersoonsAdres> ex)
				{
					var fault = Mapper.Map<BlokkerendeObjectenException<PersoonsAdres>, BlokkerendeObjectenFault<PersoonsAdresInfo2>>(ex);

					throw new FaultException<BlokkerendeObjectenFault<PersoonsAdresInfo2>>(fault);
				}

				// persisteren
				_adrMgr.Bewaren(adres);
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
			}
		}

		/// <summary>
		/// Voegt een adres toe aan een verzameling personen
		/// </summary>
		/// <param name="personenIDs">ID's van Personen
		/// waaraan het nieuwe adres toegevoegd moet worden.</param>
		/// <param name="adr">Toe te voegen adres</param>
		/// <remarks>Als het adres het eerste adres van de persoon is, maakt deze code hier geen standaardadres van.
		/// Gebruik liever 'AdresToevoegenGelieerdePersonen'.</remarks>
		[Obsolete]
		public void AdresToevoegenPersonen(List<int> personenIDs, PersoonsAdresInfo adr)
		{
			try
			{
				// Dit gaat sterk lijken op verhuizen.

				// Adres opzoeken in database
				Adres adres;
				try
				{
					adres = _adrMgr.ZoekenOfMaken(adr.StraatNaamNaam, adr.HuisNr, adr.Bus, adr.WoonPlaatsNaam, adr.PostNr, null);
				}
				catch (OngeldigObjectException ex)
				{
					var fault = Mapper.Map<OngeldigObjectException, OngeldigObjectFault>(ex);

					throw new FaultException<OngeldigObjectFault>(fault);
				}

				// Personen ophalen
				IEnumerable<Persoon> personenLijst = _pMgr.LijstOphalen(personenIDs, PersoonsExtras.Adressen);

				// Adres koppelen aan personen

				try
				{
					_pMgr.AdresToevoegen(personenLijst, adres, adr.AdresType);
				}
				catch (BlokkerendeObjectenException<PersoonsAdres> ex)
				{
					var fault = Mapper.Map<BlokkerendeObjectenException<PersoonsAdres>, BlokkerendeObjectenFault<PersoonsAdresInfo2>>(ex);

					throw new FaultException<BlokkerendeObjectenFault<PersoonsAdresInfo2>>(fault);
				}

				// persisteren
				_adrMgr.Bewaren(adres);
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
			}
		}

		/// <summary>
		/// Verwijdert voor de personen met de opgegeven ID's de link met het adres met de opgegeven ID
		/// </summary>
		/// <param name="personenIDs">De ID's van de personen over wie het gaat</param>
		/// <param name="adresID">De ID van het adres in kwestie</param>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void AdresVerwijderenVanPersonen(IList<int> personenIDs, int adresID)
		{
			try
			{
				// Adres ophalen, met bewoners voor GAV
				Adres adr = _adrMgr.AdresMetBewonersOphalen(adresID, _auMgr.MijnGroepIDsOphalen(), true);

				IList<PersoonsAdres> teVerwijderen = (from pa in adr.PersoonsAdres
													  where personenIDs.Contains(pa.Persoon.ID)
													  select pa).ToList();

				_gpMgr.AdresVerwijderen(teVerwijderen);
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
			}
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
			try
			{
				GelieerdePersoon gp = _gpMgr.Ophalen(gelieerdePersoonID, PersoonsExtras.Adressen);

				var voorkeur = (from pa in gp.Persoon.PersoonsAdres
								where pa.ID == persoonsAdresID
								select pa).FirstOrDefault();

				_gpMgr.VoorkeurInstellen(gp, voorkeur);
				_gpMgr.Bewaren(gp, PersoonsExtras.Adressen);
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
			}
		}

		/// <summary>
		/// Haalt de relevante info op van personen die op hetzelfde adres wonen als de gelieerde persoon
		/// met de opgegeven ID
		/// </summary>
		/// <param name="gelieerdePersoonID">De ID van de gelieerde persoon in kwestie</param>
		/// <returns>Een lijst met identificatiegegevens van de huisgenoten van de gelieerde persoon</returns>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<BewonersInfo> HuisGenotenOphalenZelfdeGroep(int gelieerdePersoonID)
		{
			try
			{
				IList<GelieerdePersoon> lijst = _gpMgr.HuisGenotenOphalenZelfdeGroep(gelieerdePersoonID);

				// Opgelet: als de return een exception throwt, dan is er waarschijnljk een ongeldig adrestype
				// mee gemoeid. Eventueel moet hier nog een specifiek catch-block toegevoegd worden.

				return Mapper.Map<IList<GelieerdePersoon>, IList<BewonersInfo>>(lijst);
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				return null;
			}
		}

		#endregion

		#region Communicatie

		/// <summary>
		/// Voegt een commvorm toe aan een gelieerde persoon
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van de gelieerde persoon</param>
		/// <param name="commInfo">De communicatievorm die aan die persoon gekoppeld moet worden</param>
		public void CommunicatieVormToevoegen(int gelieerdePersoonID, CommunicatieInfo commInfo)
		{
			try
			{
				// TODO: Deze method moet nog aangepast worden.  De geijkte manier van werken is:
				// 1. Haal gelieerde persoon op
				// 2. Creer nieuwe communicatievorm
				// 3. Gebruik business om te koppelen
				// 4. Bewaar

				var communicatieVorm = Mapper.Map<CommunicatieInfo, CommunicatieVorm>(commInfo);
				communicatieVorm.CommunicatieType = _cvMgr.CommunicatieTypeOphalen(commInfo.CommunicatieTypeID);

				GelieerdePersoon gp = _gpMgr.OphalenMetCommVormen(gelieerdePersoonID);
				_cvMgr.AanpassingenDoorvoeren(gp, communicatieVorm);
				_gpMgr.Bewaren(gp, PersoonsExtras.Communicatie);
			}
			catch (ValidatieException ex)
			{
				// TODO: specifiekere info bij in de exceptie.  Zie ticket #497.
				// OPM: ex.Message als bericht opgenomen in de FoutNummerFault. Is dat voldoende?
				throw new FaultException<FoutNummerFault>(new FoutNummerFault { FoutNummer = FoutNummer.ValidatieFout, Bericht = ex.Message });
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
			}
		}

		/// <summary>
		/// Verwijdert de link tussen een persoon en de communicatievorm met de opgegeven ID
		/// </summary>
		/// <param name="commvormID">De ID van de communicatievorm die niet langer aan de
		/// persoon in kwestie gelinkt moet zijn</param>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void CommunicatieVormVerwijderenVanPersoon(int commvormID)
		{
			try
			{
				var cv = _cvMgr.OphalenMetGelieerdePersoon(commvormID);
				if (cv == null)
				{
					throw new ArgumentException(Resources.FouteCommunicatieVormVoorPersoonString);
				}
				_cvMgr.CommunicatieVormVerwijderen(cv);	// persisteert
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
			}
		}

		// TODO dit moet gecontroleerd worden!

		/// <summary>
		/// Persisteert de wijzigingen aan een bestaande communicatievorm
		/// </summary>
		/// <param name="v">De aan te passen communicatievorm</param>
		public void CommunicatieVormAanpassen(CommunicatieInfo v)
		{
			try
			{
				var communicatieVorm = Mapper.Map<CommunicatieInfo, CommunicatieVorm>(v);
				communicatieVorm.CommunicatieType = _cvMgr.CommunicatieTypeOphalen(v.CommunicatieTypeID);

				var cv = _cvMgr.OphalenMetGelieerdePersoon(v.ID);
				var gp = cv.GelieerdePersoon;
				_cvMgr.AanpassingenDoorvoeren(gp, communicatieVorm);
				_gpMgr.Bewaren(gp, PersoonsExtras.Communicatie);
			}
			catch (ValidatieException ex)
			{
				// TODO: specifiekere info bij in de exceptie.  Zie ticket #497.
				throw new FaultException<FoutNummerFault>(new FoutNummerFault { FoutNummer = FoutNummer.ValidatieFout, Bericht = ex.Message });
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
			}
		}

		/// <summary>
		/// Haalt detail van een communicatievorm op
		/// </summary>
		/// <param name="commvormID">ID van de communicatievorm waarover het gaat</param>
		/// <returns>De communicatievorm met de opgegeven ID</returns>
		public CommunicatieInfo CommunicatieVormOphalen(int commvormID)
		{
			try
			{
				return Mapper.Map<CommunicatieVorm, CommunicatieInfo>(_cvMgr.Ophalen(commvormID));
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				return null;
			}
		}

		/// <summary>
		/// Haalt info over een bepaald communicatietype op, op basis van ID
		/// </summary>
		/// <param name="commTypeID">De ID van het communicatietype</param>
		/// <returns>Info over het gevraagde communicatietype</returns>
		public CommunicatieTypeInfo CommunicatieTypeOphalen(int commTypeID)
		{
			try
			{
				return Mapper.Map<CommunicatieType, CommunicatieTypeInfo>(
					_cvMgr.CommunicatieTypeOphalen(commTypeID));
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				return null;
			}
		}

		/// <summary>
		/// Een lijst ophalen van beschikbare communicatietypes
		/// </summary>
		/// <returns>Een lijst van beschikbare communicatietypes</returns>
		public IEnumerable<CommunicatieTypeInfo> CommunicatieTypesOphalen()
		{
			try
			{
				return Mapper.Map<IEnumerable<CommunicatieType>, IEnumerable<CommunicatieTypeInfo>>(
					_cvMgr.CommunicatieTypesOphalen());
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				return null;
			}
		}

		#endregion

		#region Categorieën

		/// <summary>
		/// Koppelt een lijst gebruikers aan een categorie
		/// </summary>
		/// <param name="gelieerdepersonenIDs">ID's van de te koppelen gebruikers</param>
		/// <param name="categorieIDs">ID's van de te koppelen categorieën</param>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void CategorieKoppelen(IList<int> gelieerdepersonenIDs, IList<int> categorieIDs)
		{
			try
			{
				IList<GelieerdePersoon> gelpersonen = _gpMgr.Ophalen(gelieerdepersonenIDs);

				foreach (int catID in categorieIDs)
				{
					Categorie categorie = _catMgr.Ophalen(catID);

					// Koppelen
					_gpMgr.CategorieKoppelen(gelpersonen, categorie);

					// Bewaren
					_catMgr.BewarenMetPersonen(categorie);
				}
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
			}
		}

		/// <summary>
		/// Koppelt een lijst gebruikers los van een categorie
		/// </summary>
		/// <param name="gelieerdepersonenIDs">ID's van los te koppelen gebruikers</param>
		/// <param name="categorieID">ID van de categorie</param>
		public void CategorieVerwijderen(IList<int> gelieerdepersonenIDs, int categorieID)
		{
			try
			{
			// Haal personen op met groep
			IList<GelieerdePersoon> gelieerdePersonen = _gpMgr.Ophalen(gelieerdepersonenIDs);

			// Haal categorie op met groep
			Categorie categorie = _catMgr.Ophalen(categorieID);

			// Ontkoppelen en persisteren (verwijderen persisteert altijd meteen)
			_gpMgr.CategorieLoskoppelen(gelieerdepersonenIDs, categorie);
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
			}
		}
		
		/// <summary>
		/// Bestelt Dubbelpunt voor de persoon met GelieerdePersoonID <paramref name="gelieerdePersoonID"/>.
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van gelieerde persoon van persoon die Dubbelpunt wil</param>
		public void DubbelPuntBestellen(int gelieerdePersoonID)
		{
			// TODO: exceptions op databaseniveau catchen

			GelieerdePersoon gp = null;
			try
			{
				gp = _gpMgr.Ophalen(gelieerdePersoonID);
			}
			catch (GeenGavException ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
			}

			Debug.Assert(gp != null);

			gp.Persoon.DubbelPuntAbonnement = true;

			_gpMgr.Bewaren(gp, PersoonsExtras.Geen);
		}

		#endregion categorieën

		/// <summary>
		/// Haalt de PersoonID op van de gelieerde persoon met de opgegeven ID
		/// </summary>
		/// <param name="gelieerdePersoonID">De ID van de gelieerde persoon in kwestie</param>
		/// <returns>De persoonID van de gelieerde persoon in kwestie</returns>
		public int PersoonIDGet(int gelieerdePersoonID)
		{
			try
			{
				// TODO: Heel de gelieerde persoon + persoon ophalen voor enkel 1 ID is nog altijd overkill; zie issue #154
				return _gpMgr.Ophalen(gelieerdePersoonID).Persoon.ID;
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				return 0;
			}
		}

		#endregion
	}
}
