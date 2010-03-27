// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Fouten.Exceptions;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Worker die alle businesslogica i.v.m. leden bevat
	/// </summary>
	public class LedenManager
	{
		private LedenDaoCollectie _daos;
		private IAutorisatieManager _autorisatieMgr;

		/// <summary>
		/// Maakt een nieuwe ledenmanager aan
		/// </summary>
		/// <param name="daos">Een hele reeks van IDao-objecten, nodig
		/// voor data access.</param>
		/// <param name="autorisatie">Een IAuthorisatieManager, die
		/// de GAV-permissies van de huidige user controleert.</param>
		public LedenManager(LedenDaoCollectie daos, IAutorisatieManager autorisatie)
		{
			_daos = daos;
			_autorisatieMgr = autorisatie;
		}

		/// <summary>
		/// Doet de basic aanpassingen aan het lidobject (alle andere zijn specifiek voor kind of leiding).
		/// </summary>
		/// <param name="lid"></param>
		/// <param name="gp"></param>
		/// <param name="gwj"></param>
		private void LidMaken(Lid lid, GelieerdePersoon gp, GroepsWerkJaar gwj)
		{
			// GroepsWerkJaar en GelieerdePersoon invullen
			lid.GroepsWerkJaar = gwj;
			lid.GelieerdePersoon = gp;
			gp.Lid.Add(lid);
			gwj.Lid.Add(lid);
		}

		/// <summary>
		/// Doet alle nodige checks om na te kijken dat de gegeven gelieerde persoon lid gemaakt kan worden in het
		/// gegeven groepswerkjaar. In dat geval wordt er true teruggeven
		/// </summary>
		/// <param name="gp">De gelieerde persoon die je lid wilt maken</param>
		/// <param name="gwj">Het groepswerkjaar waarin je die persoon lid wilt maken</param>
		/// <param name="gpMetDetails">De gelieerde persoon aangevuld met details</param>
		/// <returns><c>True</c> als de persoon lid kan worden</returns>
		private bool kanLidMaken(GelieerdePersoon gp, GroepsWerkJaar gwj, out GelieerdePersoon gpMetDetails)
		{
			if (!_autorisatieMgr.IsGavGelieerdePersoon(gp.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
			}
			if (!_autorisatieMgr.IsGavGroepsWerkJaar(gwj.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGavGroepsWerkJaar);
			}

			// De persoonsgegevens zijn nodig om de afdeling te bepalen.  Haal ze op als ze ontbreken.
			gpMetDetails = gp;
			if (gp.Persoon == null)
			{
				GelieerdePersonenManager gpm = Factory.Maak<GelieerdePersonenManager>();
				gpMetDetails = gpm.DetailsOphalen(gp.ID);
			}

			// CONTROLES
			// Lid bestaat al
			var x = (from l in gpMetDetails.Lid
					 where l.GroepsWerkJaar.ID == gwj.ID
					 select l).FirstOrDefault();

			if (x != null) // was dus al lid
			{
				throw new BestaatAlException();
			}

			return true;
		}

		/// <summary>
		/// Maakt gelieerde persoon een kind (lid) voor het recentste werkjaar
		/// </summary>
		/// <param name="gp">Gelieerde persoon</param>
		/// <returns>Nieuw kindobject, niet gepersisteerd</returns>
		public Kind KindMaken(GelieerdePersoon gp)
		{
			if (!_autorisatieMgr.IsGavGelieerdePersoon(gp.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
			}

			if (gp.Groep == null)
			{
				_daos.GelieerdePersoonDao.GroepLaden(gp);
			}

			// Door het groepswerkjaar van de persoon op te halen is de link tussen groepswerkjaar en persoon zeker in orde
			GroepsWerkJaar gwj = _daos.GroepenDao.RecentsteGroepsWerkJaarGet(gp.Groep.ID);

			return KindMaken(gp, gwj);
		}

		/// <summary>
		/// Maakt gelieerde persoon een kind (lid) voor het gegeven werkjaar
		/// </summary>
		/// <param name="gp">Gelieerde persoon, gekoppeld aan groep</param>
		/// <param name="gwj">Groepswerkjaar waarin lid te maken</param>
		/// <returns>Nieuw kindobject, niet gepersisteerd</returns>
		/// <remarks>De user zal nooit zelf mogen kiezen in welk groepswerkjaar een kind lid wordt.  Maar 
		/// om testdata voor unit tests op te bouwen, hebben we deze functionaliteit wel nodig.
		/// </remarks>
		public Kind KindMaken(GelieerdePersoon gp, GroepsWerkJaar gwj)
		{
			if (!_autorisatieMgr.IsGavGelieerdePersoon(gp.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
			}

			GroepenManager gm = Factory.Maak<GroepenManager>();

			GelieerdePersoon gpMetDetails;
			if (!kanLidMaken(gp, gwj, out gpMetDetails))
			{
				throw new OngeldigeActieException("De gegeven persoon kan geen lid worden in het huidige werkjaar.");
			}

			// Er zijn afdelingen
			if (gwj.AfdelingsJaar.Count == 0)
			{
				throw new OngeldigeActieException("Je kan geen lid maken als de groep geen afdelingen heeft in het huidige werkjaar!");
			}

			// Geboortedatum is verplicht als je lid wilt worden
			if (gpMetDetails.Persoon.GeboorteDatum == null)
			{
				throw new OngeldigeActieException("Je kan geen lid maken als je de geboortedatum van de persoon niet hebt ingegeven");
			}

			// Afdeling automatisch bepalen
			int geboortejaar = gpMetDetails.Persoon.GeboorteDatum.Value.Year;
			geboortejaar += gpMetDetails.ChiroLeefTijd;	 // aanpassen aan Chiroleeftijd

			int leeftijd = DateTime.Today.Year - gpMetDetails.Persoon.GeboorteDatum.Value.Year;
			if (leeftijd < 6)
			{
				throw new OngeldigeActieException("Deze persoon is te jong om lid te zijn in de chiro.");
			}
			if (leeftijd > 20)
			{
				throw new OngeldigeActieException("Is deze persoon niet te oud om in de chiro te komen?");
			}
			int aangepasteleeftijd = DateTime.Today.Year - geboortejaar;
			if (aangepasteleeftijd < 6 || aangepasteleeftijd > 18)
			{
				throw new OngeldigeActieException("De afdelingen lopen maar van 6 tot 18 jaar.");
			}

			// Relevante afdelingsjaren opzoeken
			List<AfdelingsJaar> afdelingsjaren =
				(from a in gwj.AfdelingsJaar
				 where a.GeboorteJaarVan <= geboortejaar && geboortejaar <= a.GeboorteJaarTot
				 select a).ToList();

			if (afdelingsjaren.Count == 0)
			{
				throw new OngeldigeActieException("Er is geen afdeling in jullie groep voor die leeftijd. Je maakt er best eerst een aan (of controleert de leeftijd van het kind).");
			}

			AfdelingsJaar aj = null;
			if (afdelingsjaren.Count > 1)
			{
				aj = (from a in afdelingsjaren
					  where a.Geslacht == gpMetDetails.Persoon.Geslacht || a.Geslacht == GeslachtsType.Gemengd
					  select a).FirstOrDefault();
			}
			if (aj == null)
			{
				aj = afdelingsjaren.First();
			}

			Kind kind = new Kind();
			kind.AfdelingsJaar = aj;
			aj.Kind.Add(kind);
			kind.EindeInstapPeriode = DateTime.Today.AddDays(Settings.Default.DagenInstapPeriode);

			LidMaken(kind, gpMetDetails, gwj);

			return kind;
		}

		/// <summary>
		/// Maakt de gelieerdepersoon leiding in het huidige werkjaar.
		/// </summary>
		/// <param name="gp">De gelieerde persoon die je leiding wilt maken</param>
		/// <returns>Het leidingsobject van de gelieerde persoon die leiding geworden is</returns>
		public Leiding LeidingMaken(GelieerdePersoon gp)
		{
			if (!_autorisatieMgr.IsGavGelieerdePersoon(gp.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
			}

			GroepsWerkJaarManager mgr = Factory.Maak<GroepsWerkJaarManager>();

			if (gp.Groep == null)
			{
				_daos.GelieerdePersoonDao.GroepLaden(gp);
			}

			GroepsWerkJaar gwj = _daos.GroepenDao.RecentsteGroepsWerkJaarGet(gp.Groep.ID);

			GelieerdePersoon gpMetDetails;
			if (!kanLidMaken(gp, gwj, out gpMetDetails))
			{
				throw new OngeldigeActieException("De gegeven persoon kan geen lid worden in het huidige werkjaar.");
			}

			Leiding leiding = new Leiding();

			LidMaken(leiding, gpMetDetails, gwj);
			return leiding;
		}

		/// <summary>
		/// Verwijdert lid uit database
		/// Dit kan enkel als het een lid uit het huidige werkjaar is en als de instapperiode niet verstreken is voor een kind
		/// Leiding kan niet verwijderd worden.  Persisteert.
		/// </summary>
		/// <param name="id">ID van het lid dat verwijderd moet worden</param>
		/// <returns><c>True</c> on successful</returns>
		public Boolean LidVerwijderen(int id)
		{
			if (!_autorisatieMgr.IsGavLid(id))
			{
				throw new GeenGavException(Properties.Resources.GeenGavLid);
			}

			Lid lid = _daos.LedenDao.OphalenMetDetails(id);

			// checks:
			if (lid is Leiding)
			{
				throw new OngeldigeActieException("Leiding kan niet meer verwijderd worden.");
			}
			if (lid.GroepsWerkJaar != _daos.GroepenDao.RecentsteGroepsWerkJaarGet(lid.GroepsWerkJaar.Groep.ID))
			{
				throw new OngeldigeActieException("Een lid verwijderen mag enkel als het een lid uit het huidige werkjaar is.");
			}
			if (lid is Kind && DateTime.Compare(((Kind)lid).EindeInstapPeriode.Value, DateTime.Today) < 0)
			{
				throw new OngeldigeActieException("Een kind verwijderen kan niet meer nadat de instapperiode verstreken is.");
			}

			lid.TeVerwijderen = true;

			Debug.Assert(lid is Kind || lid is Leiding, "Lid moet ofwel Kind ofwel Leiding zijn!");

			// voor een Kind is _dao.Bewaren(lid) voldoende
			if (lid is Kind)
			{
				_daos.LedenDao.Bewaren(lid);
			}
			return true;
		}

		/// <summary>
		/// Haal een pagina op met leden van een groepswerkjaar.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <param name="paginas">Totaal aantal groepswerkjaren van de groep</param>
		/// <returns>Lijst met alle leden uit het gevraagde groepswerkjaar.</returns>
		public IList<Lid> PaginaOphalen(int groepsWerkJaarID, out int paginas)
		{
			if (_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
			{
				GroepsWerkJaar gwj = _daos.GroepsWerkJaarDao.Ophalen(groepsWerkJaarID, grwj => grwj.Groep);
				paginas = _daos.GroepenDao.OphalenMetGroepsWerkJaren(gwj.Groep.ID).GroepsWerkJaar.Count;
				IList<Lid> list = _daos.LedenDao.AllesOphalen(groepsWerkJaarID);
				list = list.OrderBy(e => e.GelieerdePersoon.Persoon.Naam).ThenBy(e => e.GelieerdePersoon.Persoon.VoorNaam).ToList();
				return list;
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGroepsWerkJaar);
			}
		}

		/// <summary>
		/// Haalt een 'pagina' op met leden uit een bepaald GroepsWerkJaar
		/// </summary>
		/// <param name="groepsWerkJaarID">ID gevraagde GroepsWerkJaar</param>
		/// <param name="afdelingsID">ID gevraagde afdeling</param>
		/// <param name="paginas">De 'pagina' met leden uit een bepaald GroepsWerkJaar</param>
		/// <returns>De 'pagina' (collectie) met leden</returns>
		public IList<Lid> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID, out int paginas)
		{
			GroepsWerkJaar gwj = _daos.GroepsWerkJaarDao.Ophalen(groepsWerkJaarID, grwj => grwj.Groep);
			paginas = _daos.GroepenDao.OphalenMetGroepsWerkJaren(gwj.Groep.ID).GroepsWerkJaar.Count;
			if (_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
			{
				return _daos.LedenDao.PaginaOphalenVolgensAfdeling(groepsWerkJaarID, afdelingsID);
			}
			else
			{
				throw new GeenGavException("Dit GroepsWerkJaar hoort niet bij je groep(en).");
			}
		}

		public Lid LidBewaren(Lid lid)
		{
			if (_autorisatieMgr.IsGavLid(lid.ID))
			{
				Debug.Assert(lid is Kind || lid is Leiding, "Lid moet ofwel Kind ofwel Leiding zijn!");
				if (lid is Kind)
				{
					return _daos.KindDao.Bewaren((Kind)lid);
				}
				else if (lid is Leiding)
				{
					return _daos.LeidingDao.Bewaren((Leiding)lid);
				}
				// Hier mag de code nooit komen (zie assert)
				return null;
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavLid);
			}
		}

		/// <summary>
		/// Haalt lid op, op basis van zijn <paramref name="lidID"/>
		/// </summary>
		/// <param name="lidID">ID gevraagde lid</param>
		/// <param name="extras">Geeft aan welke gekoppelde entiteiten mee opgehaald moeten worden</param>
		/// <returns>Kind of Leiding met persoonsgegevens en <paramref name="extras"/>.</returns>
		public Lid Ophalen(int lidID, LidExtras extras)
		{
			if (!_autorisatieMgr.IsGavLid(lidID))
			{
				throw new GeenGavException(Properties.Resources.GeenGavLid);
			}
			if (_daos.LedenDao.IsLeiding(lidID))
			{
				// Leiding: ga via LeidingDAO.

				var paths = new List<Expression<Func<Leiding, object>>>();
				paths.Add(ld => ld.GelieerdePersoon.Persoon);
				paths.Add(ld => ld.GroepsWerkJaar);

				if ((extras & LidExtras.Groep) != 0)
				{
					paths.Add(ld => ld.GroepsWerkJaar.Groep);
				}
				if ((extras & LidExtras.Afdelingen) != 0)
				{
					paths.Add(ld => ld.AfdelingsJaar.First().Afdeling);
				}
				if ((extras & LidExtras.Functies) != 0)
				{
					paths.Add(ld => ld.Functie);
				}
				if ((extras & LidExtras.AlleAfdelingen) != 0)
				{
					paths.Add(ld => ld.GroepsWerkJaar.AfdelingsJaar.First().Afdeling);
				}

				return _daos.LeidingDao.Ophalen(lidID, paths.ToArray());
			}
			else
			{
				// Nog eens ongeveer hetzelfde voor kinderen.  Waarschijnlijk kan dit
				// properder.
				var paths = new List<Expression<Func<Kind, object>>>();
				paths.Add(ld => ld.GelieerdePersoon.Persoon);
				paths.Add(ld => ld.GroepsWerkJaar);

				if ((extras & LidExtras.Groep) != 0)
				{
					paths.Add(ld => ld.GroepsWerkJaar.Groep);
				}
				if ((extras & LidExtras.Afdelingen) != 0)
				{
					paths.Add(ld => ld.AfdelingsJaar.Afdeling);
				}
				if ((extras & LidExtras.Functies) != 0)
				{
					paths.Add(ld => ld.Functie);
				}

				return _daos.KindDao.Ophalen(lidID, paths.ToArray());
			}
		}

		/// <summary>
		/// Haalt lid en gekoppelde persoon op, op basis van <paramref name="lidID"/>
		/// </summary>
		/// <param name="lidID">ID op te halen lid</param>
		/// <returns>Lid, met daaraan gekoppeld gelieerde persoon en persoon.</returns>
		public Lid Ophalen(int lidID)
		{
			return Ophalen(lidID, LidExtras.Geen);
		}

		/// <summary>
		/// Haalt leden op uit een bepaald groepswerkjaar met een gegeven functie
		/// </summary>
		/// <param name="functieID">ID van de functie</param>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <param name="extras">Geeft aan welke gekoppelde entiteiten mee opgehaald moeten worden
		/// met de leden</param>
		/// <returns>Lijst leden uit het groepswerkjaar met de gegeven functie</returns>
		/// <remarks>Persoonsgegevens worden standaard mee opgehaald met lid.</remarks>
		public IList<Lid> Ophalen(int functieID, int groepsWerkJaarID, LidExtras extras)
		{
			if (!_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
			{
				throw new GeenGavException(Properties.Resources.GeenGavGroepsWerkJaar);
			}
			else if (!_autorisatieMgr.IsGavFunctie(functieID))
			{
				throw new GeenGavException(Properties.Resources.GeenGavFunctie);
			}
			else
			{
				var paths = new List<Expression<Func<Lid, object>>>();

				paths.Add(ld => ld.GelieerdePersoon.Persoon);
				if ((extras & LidExtras.Groep) != 0)
				{
					paths.Add(ld => ld.GroepsWerkJaar.Groep);
				}
				if ((extras & LidExtras.Afdelingen) != 0)
				{
					// Afdelingen: altijd tricky, want verschillend voor leden
					// en leiding.
					throw new NotImplementedException();
				}
				if ((extras & LidExtras.Functies) != 0)
				{
					paths.Add(ld => ld.Functie);
				}

				return _daos.LedenDao.OphalenUitFunctie(
					functieID,
					groepsWerkJaarID,
					paths.ToArray());
			}
		}

		/// <summary>
		/// De afdelingen van het gegeven lid worden aangepast van whatever momenteel zijn afdelingen zijn naar
		/// de gegeven lijst nieuwe afdelingen.
		/// Een kind mag maar 1 afdeling hebben, voor een leider staan daar geen constraints op.
		/// Persisteert.
		/// </summary>
		/// <param name="l">Lid, geladen met groepswerkjaar met afdelingsjaren</param>
		/// <param name="afdelingsIDs">De ids van de AFDELING waarvan het kind lid is</param>
		/// <remarks>Deze functie is niet 'compliant' aan de coding standard, zie 
		/// richtlijnen 85, 83 en 1)</remarks>
		/// <returns>Lidobject met gekoppeld(e) afdelingsja(a)r(en)</returns>
		public Lid AanpassenAfdelingenVanLid(Lid l, IList<int> afdelingsIDs)
		{
			Debug.Assert(l.GroepsWerkJaar != null);
			Debug.Assert(l.GroepsWerkJaar.AfdelingsJaar != null);

			if (l is Kind)
			{
				Kind kind = (Kind)l;
				if (afdelingsIDs.Count != 1)
				{
					throw new OngeldigeActieException("Een kind moet in exact 1 afdeling zitten.");
				}

				if (kind.AfdelingsJaar.Afdeling.ID != afdelingsIDs[0]) // anders verandert er niets
				{
					// Verwijder het kind uit zijn huidige afdelingsjaar
					kind.AfdelingsJaar.TeVerwijderen = true;

					AfdelingsJaar ajnieuw = kind.GroepsWerkJaar.AfdelingsJaar.FirstOrDefault(e => e.Afdeling.ID == afdelingsIDs[0]);
					if (ajnieuw == null)
					{
						throw new OngeldigeActieException("De gekozen afdeling is geen afdeling van de groep in het gekozen werkjaar.");
					}
					ajnieuw.Kind.Add(kind);

					// Pas Chiroleeftijd aan als het kind buiten het veld van de nieuwe afdeling valt
					if (kind.GelieerdePersoon.Persoon.GeboorteDatum.Value.Year + kind.GelieerdePersoon.ChiroLeefTijd < ajnieuw.GeboorteJaarVan)
					{
						kind.GelieerdePersoon.ChiroLeefTijd = ajnieuw.GeboorteJaarVan - kind.GelieerdePersoon.Persoon.GeboorteDatum.Value.Year;
					}
					else if (kind.GelieerdePersoon.Persoon.GeboorteDatum.Value.Year + kind.GelieerdePersoon.ChiroLeefTijd > ajnieuw.GeboorteJaarTot)
					{
						kind.GelieerdePersoon.ChiroLeefTijd = ajnieuw.GeboorteJaarTot - kind.GelieerdePersoon.Persoon.GeboorteDatum.Value.Year;
					}

					kind.AfdelingsJaar = ajnieuw;
					return _daos.KindDao.Bewaren(kind, knd => knd.AfdelingsJaar);
				}
				else
				{
					return kind;
				}
			}
			else
			{
				Leiding leiding = (Leiding)l;

				// Controleer op voorhand of de gevraagde afdelingen wel geactiveerd zijn in het 
				// groepswerkjaar van het lid.
				var geldigeAfdelingIDs = from aj in leiding.GroepsWerkJaar.AfdelingsJaar
										 select aj.Afdeling.ID;

				var nietGevonden = from aid in afdelingsIDs
								   where !(geldigeAfdelingIDs.Contains(aid))
								   select aid;

				if (nietGevonden.Count() > 0)
				{
					throw new GroepsWerkJaarException(Properties.Resources.AfdelingNietBeschikbaar);
				}

				// Verwijder ontbrekende afdelingen;
				var teVerwijderenAfdelingen = from aj in leiding.AfdelingsJaar
											  where !afdelingsIDs.Contains(aj.Afdeling.ID)
											  select aj;

				foreach (var aj in teVerwijderenAfdelingen)
				{
					aj.TeVerwijderen = true;
				}

				// Ken nieuwe afdelingen toe
				var nieuweAfdelingen = from aj in leiding.GroepsWerkJaar.AfdelingsJaar
									   where afdelingsIDs.Contains(aj.Afdeling.ID) && !(leiding.AfdelingsJaar.Contains(aj))
									   select aj;

				foreach (var aj in nieuweAfdelingen)
				{
					leiding.AfdelingsJaar.Add(aj);
					aj.Leiding.Add(leiding);
				}

				return _daos.LeidingDao.Bewaren(leiding, ldng => ldng.AfdelingsJaar);
			}
		}

		/// <summary>
		/// Neemt de info uit <paramref name="lidInfo"/> over in <paramref name="lid"/>
		/// </summary>
		/// <param name="lidinfo">LidInfo om over te nemen in <paramref name="lid"/></param>
		/// <param name="lid">Lid dat <paramref name="lidInfo"/> moet krijgen</param>
		public void InfoOvernemen(Chiro.Gap.ServiceContracts.LidInfo lidInfo, Lid lid)
		{
			Debug.Assert(lid is Leiding || lid is Kind);

			if (lid is Kind && lidInfo.Type == LidType.Leiding)
			{
				throw new NotImplementedException();
			}
			else if (lid is Leiding && lidInfo.Type == LidType.Kind)
			{
				throw new NotImplementedException();
			}

			if (lid is Kind)
			{
				Kind kind = (Kind)lid;
				kind.LidgeldBetaald = lidInfo.LidgeldBetaald;
				kind.NonActief = lidInfo.NonActief;
			}
			else
			{
				Leiding leiding = (Leiding)lid;
				leiding.DubbelPuntAbonnement = lidInfo.DubbelPunt;
				leiding.NonActief = lidInfo.NonActief;
			}
		}
	}
}
