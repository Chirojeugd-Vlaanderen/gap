// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Worker die alle businesslogica i.v.m. leden bevat
	/// </summary>
	public class LedenManager
	{
		private readonly LedenDaoCollectie _daos;
		private readonly IAutorisatieManager _autorisatieMgr;

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
			lid.EindeInstapPeriode = DateTime.Today.AddDays(Properties.Settings.Default.DagenInstapPeriode);
		}

		/// <summary>
		/// Doet alle nodige checks om na te kijken dat de gegeven gelieerde persoon lid gemaakt kan worden in het
		/// gegeven groepswerkjaar. In dat geval wordt er true teruggeven
		/// </summary>
		/// <param name="gp">De gelieerde persoon die je lid wilt maken</param>
		/// <param name="gwj">Het groepswerkjaar waarin je die persoon lid wilt maken</param>
		/// <param name="gpMetDetails">De gelieerde persoon aangevuld met details</param>
		/// <returns><c>True</c> als de persoon lid kan worden</returns>
		private bool KanLidMaken(GelieerdePersoon gp, GroepsWerkJaar gwj, out GelieerdePersoon gpMetDetails)
		{
			if (!_autorisatieMgr.IsGavGelieerdePersoon(gp.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			if (!_autorisatieMgr.IsGavGroepsWerkJaar(gwj.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			// De persoonsgegevens zijn nodig om de afdeling te bepalen.  Haal ze op als ze ontbreken.
			gpMetDetails = gp;
			if (gp.Persoon == null)
			{
				var gpm = Factory.Maak<GelieerdePersonenManager>();
				gpMetDetails = gpm.DetailsOphalen(gp.ID);
			}

			// CONTROLES
			// Lid bestaat al
			var x = (from l in gpMetDetails.Lid
					 where l.GroepsWerkJaar.ID == gwj.ID
					 select l).FirstOrDefault();

			if (x != null) // was dus al lid
			{
				throw new BestaatAlException<Lid>(x);
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
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			if (gp.Groep == null)
			{
				_daos.GelieerdePersoonDao.GroepLaden(gp);
			}

			// Door het groepswerkjaar van de persoon op te halen is de link tussen groepswerkjaar en persoon zeker in orde
			var gwj = _daos.GroepsWerkJaarDao.RecentsteOphalen(gp.Groep.ID, grwj => grwj.AfdelingsJaar.First().Afdeling);

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
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			GelieerdePersoon gpMetDetails;
			if (!KanLidMaken(gp, gwj, out gpMetDetails))
			{
				throw new InvalidOperationException("De gegeven persoon kan geen lid worden in het huidige werkjaar.");
			}

			// Er zijn afdelingen
			if (gwj.AfdelingsJaar.Count == 0)
			{
				throw new InvalidOperationException("Je kan geen lid maken als de groep geen afdelingen heeft in het huidige werkjaar!");
			}

			// Geboortedatum is verplicht als je lid wilt worden
			if (gpMetDetails.Persoon.GeboorteDatum == null)
			{
				throw new InvalidOperationException("Je kan geen lid maken als je de geboortedatum van de persoon niet hebt ingegeven");
			}

			// Afdeling automatisch bepalen
			var geboortejaar = gpMetDetails.Persoon.GeboorteDatum.Value.Year;
			geboortejaar -= gpMetDetails.ChiroLeefTijd;	 // aanpassen aan Chiroleeftijd

			// Relevante afdelingsjaren opzoeken
			var afdelingsjaren =
				(from a in gwj.AfdelingsJaar
				 where a.GeboorteJaarVan <= geboortejaar && geboortejaar <= a.GeboorteJaarTot
				 select a).ToList();

			if (afdelingsjaren.Count == 0)
			{
				throw new InvalidOperationException("Er is geen afdeling in jullie groep voor die leeftijd. Je maakt er best eerst een aan (of controleert de leeftijd van het kind).");
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

			var kind = new Kind();
			kind.AfdelingsJaar = aj;
			aj.Kind.Add(kind);

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
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			if (gp.Groep == null)
			{
				_daos.GelieerdePersoonDao.GroepLaden(gp);
			}

			var gwj = _daos.GroepsWerkJaarDao.RecentsteOphalen(gp.Groep.ID);

			GelieerdePersoon gpMetDetails;
			if (!KanLidMaken(gp, gwj, out gpMetDetails))
			{
				throw new InvalidOperationException("De gegeven persoon kan geen lid worden in het huidige werkjaar.");
			}

			var leiding = new Leiding();

			LidMaken(leiding, gpMetDetails, gwj);
			return leiding;
		}

		/// <summary>
		/// Verwijdert lid uit database
		/// Dit kan enkel als het een lid uit het huidige werkjaar is en als de instapperiode niet verstreken is voor een kind
		/// Leiding kan niet verwijderd worden.  Persisteert.
		/// </summary>
		/// <param name="lid">Te verwijderen lid</param>
		/// <remarks>Het <paramref name="lid"/> moet via het groepswerkjaar gekoppeld
		/// aan zijn groep.  Als het om leiding gaat, moeten ook de afdelingen gekoppeld zijn.</remarks>
		public void Verwijderen(Lid lid)
		{
			Debug.Assert(lid.GroepsWerkJaar != null);
			Debug.Assert(lid.GroepsWerkJaar.Groep != null);

			if (!_autorisatieMgr.IsGavLid(lid.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			// checks:
			if (lid.GroepsWerkJaar.ID != _daos.GroepsWerkJaarDao.RecentsteOphalen(lid.GroepsWerkJaar.Groep.ID).ID)
			{
				throw new InvalidOperationException("Een lid verwijderen mag enkel als het een lid uit het huidige werkjaar is.");
			}
			if (lid is Kind && DateTime.Compare(lid.EindeInstapPeriode.Value, DateTime.Today) < 0)
			{
				throw new InvalidOperationException("Een kind verwijderen kan niet meer nadat de instapperiode verstreken is.");
			}

			lid.TeVerwijderen = true;
			if (lid is Leiding)
			{
				var leiding = lid as Leiding;

				foreach (AfdelingsJaar aj in leiding.AfdelingsJaar)
				{
					aj.TeVerwijderen = true;
				}
				_daos.LeidingDao.Bewaren(leiding, ld => ld.AfdelingsJaar);
			}
			else
			{
				_daos.LedenDao.Bewaren(lid);
			}
		}

		/// <summary>
		/// Haal een pagina op met leden van een groepswerkjaar.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <param name="paginas">Totaal aantal groepswerkjaren van de groep</param>
		/// <returns>Lijst met alle leden uit het gevraagde groepswerkjaar.</returns>
		public IList<Lid> PaginaOphalen(int groepsWerkJaarID, out int paginas)
		{
			if (!_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			var gwj = _daos.GroepsWerkJaarDao.Ophalen(groepsWerkJaarID, grwj => grwj.Groep);
			paginas = _daos.GroepenDao.OphalenMetGroepsWerkJaren(gwj.Groep.ID).GroepsWerkJaar.Count;
			var list = _daos.LedenDao.AllesOphalen(groepsWerkJaarID);
			list = list.OrderBy(e => e.GelieerdePersoon.Persoon.Naam).ThenBy(e => e.GelieerdePersoon.Persoon.VoorNaam).ToList();
			return list;
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
			var gwj = _daos.GroepsWerkJaarDao.Ophalen(groepsWerkJaarID, grwj => grwj.Groep);
			paginas = _daos.GroepenDao.OphalenMetGroepsWerkJaren(gwj.Groep.ID).GroepsWerkJaar.Count;
			if (_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
			{
				return _daos.LedenDao.PaginaOphalenVolgensAfdeling(groepsWerkJaarID, afdelingsID);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// TODO: documenteren
		/// </summary>
		/// <param name="lid">Het <paramref name="lid"/> dat bewaard moet worden</param>
		/// <returns></returns>
		public Lid LidBewaren(Lid lid)
		{
			if (!_autorisatieMgr.IsGavLid(lid.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			Debug.Assert(lid is Kind || lid is Leiding, "Lid moet ofwel Kind ofwel Leiding zijn!");
			if (lid is Kind)
			{
				return _daos.KindDao.Bewaren((Kind)lid);
			}
			if (lid is Leiding)
			{
				return _daos.LeidingDao.Bewaren((Leiding)lid);
			}
			// Hier mag de code nooit komen (zie assert)
			return null;
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
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			if (_daos.LedenDao.IsLeiding(lidID))
			{
				// Leiding: ga via LeidingDAO.

				var paths = new List<Expression<Func<Leiding, object>>>();
				paths.Add(ld => ld.GroepsWerkJaar);

				if ((extras & LidExtras.Persoon) != 0)
				{
					paths.Add(ld => ld.GelieerdePersoon.Persoon);
				}
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
				if ((extras & LidExtras.AlleAfdelingen) != 0)
				{
					paths.Add(ld => ld.GroepsWerkJaar.AfdelingsJaar.First().Afdeling);
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
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			if (!_autorisatieMgr.IsGavFunctie(functieID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			var paths = new List<Expression<Func<Lid, object>>>();

			if ((extras & LidExtras.Persoon) != 0)
			{
				paths.Add(ld => ld.GelieerdePersoon.Persoon);
			}
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

		/// <summary>
		/// Haalt het lid op van een gelieerdepersoon in een groepswerkjaar als het bestaat, met gelinkte informatie
		/// van afdelingsjaren en afdelingen
		/// </summary>
		/// <param name="gelieerdePersoonID">De ID van de gelieerde persoon</param>
		/// <param name="groepsWerkJaarID">De ID van het groepswerkjaar</param>
		/// <returns>Een Lid-object</returns>
		public Lid OphalenViaPersoon(int gelieerdePersoonID, int groepsWerkJaarID)
		{
			if (!_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			if (!_autorisatieMgr.IsGavGelieerdePersoon(gelieerdePersoonID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			return _daos.LedenDao.OphalenViaPersoon(gelieerdePersoonID, groepsWerkJaarID);
		}

		/// <summary>
		/// Neemt de info uit <paramref name="lidInfo"/> over in <paramref name="lid"/>
		/// </summary>
		/// <param name="lidInfo">LidInfo om over te nemen in <paramref name="lid"/></param>
		/// <param name="lid">Lid dat <paramref name="lidInfo"/> moet krijgen</param>
		public void InfoOvernemen(ServiceContracts.LidInfo lidInfo, Lid lid)
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
				var kind = (Kind)lid;
				kind.LidgeldBetaald = lidInfo.LidgeldBetaald;
				kind.NonActief = lidInfo.NonActief;
			}
			else
			{
				var leiding = (Leiding)lid;
				leiding.DubbelPuntAbonnement = lidInfo.DubbelPunt;
				leiding.NonActief = lidInfo.NonActief;
			}
		}
	}
}
