// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Chiro.Cdf.Data;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Exceptions;
using Chiro.Gap.ServiceContracts.DataContracts;

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
		/// Maakt een gelieerde persoom <paramref name="gp"/> lid in groepswerkjaar <paramref name="gwj"/>,
		/// met lidtype <paramref name="type"/>
		/// </summary>
		/// <param name="gp">Lid te maken gelieerde persoon</param>
		/// <param name="gwj">Groepswerkjaar waarin de gelieerde persoon lid moet worden</param>
		/// <param name="type">LidType.Kind of LidType.Leiding</param>
		/// <remarks>Deze method kent geen afdelingen toe.  Ze test ook niet
		/// of het groepswerkjaar we het recentste is.  (Voor de unit tests moeten
		/// we ook leden kunnen maken in oude groepswerkjaren.)</remarks>
		/// <returns>Het aangepaste Lid-object</returns>
		private Lid LidMaken(GelieerdePersoon gp, GroepsWerkJaar gwj, LidType type)
		{
			Lid lid = null;

			switch (type)
			{
				case LidType.Kind:
					lid = new Kind();
					break;
				case LidType.Leiding:
					lid = new Leiding();
					break;
				default:
					lid = new Lid();
					break;
			}

			if (!_autorisatieMgr.IsGavGelieerdePersoon(gp.ID) || !_autorisatieMgr.IsGavGroepsWerkJaar(gwj.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			if (gp.Groep.ID != gwj.Groep.ID)
			{
				throw new FoutNummerException(
					FoutNummer.GroepsWerkJaarNietVanGroep, 
					Properties.Resources.GroepsWerkJaarNietVanGroep);
			}

			// Geboortedatum is verplicht als je lid wilt worden
			if (gp.Persoon.GeboorteDatum == null)
			{
				throw new InvalidOperationException(Properties.Resources.GeboorteDatumOntbreekt);
			}

			// GroepsWerkJaar en GelieerdePersoon invullen
			lid.GroepsWerkJaar = gwj;
			lid.GelieerdePersoon = gp;
			gp.Lid.Add(lid);
			gwj.Lid.Add(lid);
			lid.EindeInstapPeriode = DateTime.Today.AddDays(Properties.Settings.Default.DagenInstapPeriode);

			return lid;
		}

		/// <summary>
		/// Maakt gelieerde persoon een kind (lid) voor het recentste werkjaar
		/// </summary>
		/// <param name="gp">Gelieerde persoon</param>
		/// <returns>Nieuw kindobject, niet gepersisteerd</returns>
		public Kind KindMaken(GelieerdePersoon gp)
		{
			Debug.Assert(gp.Persoon != null);	// nodig om geboortedatum te kunnen bepalen

			if (!_autorisatieMgr.IsGavGelieerdePersoon(gp.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			if (gp.Groep == null)
			{
				_daos.GelieerdePersoonDao.GroepLaden(gp);
			}

			// Door het groepswerkjaar van de persoon op te halen is de link tussen groepswerkjaar en persoon zeker in orde
			var gwj = _daos.GroepsWerkJaarDao.RecentsteOphalen(
				gp.Groep.ID, 
				grwj => grwj.AfdelingsJaar.First().Afdeling,
				grwj => grwj.Groep);

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
			// LidMaken doet de nodige checks ivm GAV-schap, enz.

			Kind k = LidMaken(gp, gwj, LidType.Kind) as Kind;
			Debug.Assert(k != null);

			// Probeer nu afdeling te vinden.
			if (gwj.AfdelingsJaar.Count == 0)
			{
				throw new InvalidOperationException("Je kan geen lid maken als de groep geen afdelingen heeft in het huidige werkjaar!");
			}

			// Afdeling automatisch bepalen
			var geboortejaar = gp.Persoon.GeboorteDatum.Value.Year;
			geboortejaar -= gp.ChiroLeefTijd;	 // aanpassen aan Chiroleeftijd

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
					  where a.Geslacht == gp.Persoon.Geslacht || a.Geslacht == GeslachtsType.Gemengd
					  select a).FirstOrDefault();
			}
			if (aj == null)
			{
				aj = afdelingsjaren.First();
			}

			k.AfdelingsJaar = aj;
			aj.Kind.Add(k);

			return k;
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

			return LeidingMaken(gp, gwj);
		}

		/// <summary>
		/// Maakt gelieerde persoon leiding voor het gegeven werkjaar.
		/// </summary>
		/// <param name="gp">Gelieerde persoon</param>
		/// <param name="gwj">Groepswerkjaar waarin leiding te maken</param>
		/// <returns>Nieuw leidingsobject; niet gepersisteerd</returns>
		/// <remarks>Deze method mag niet geexposed worden via de services, omdat
		/// een gebruiker uiteraard enkel in het huidige groepswerkjaar leden
		/// kan maken.</remarks>
		public Leiding LeidingMaken(GelieerdePersoon gp, GroepsWerkJaar gwj)
		{
			// LidMaken doet de nodige checks ivm GAV-schap enz.
			
			return LidMaken(gp, gwj, LidType.Leiding) as Leiding;
		}

		/// <summary>
		/// Zet kinderen en leiding op non-actief. Geen van beide kunnen ooit verwijderd worden!!!
		/// </summary>
		/// <param name="lid"></param>
		/// <remarks>Het <paramref name="lid"/> moet via het groepswerkjaar gekoppeld
		/// aan zijn groep.  Als het om leiding gaat, moeten ook de afdelingen gekoppeld zijn.</remarks>
		public void NonActiefMaken(Lid lid)
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
				throw new InvalidOperationException("Een lid non-actief maken mag enkel als het een lid uit het huidige werkjaar is.");
			}

			lid.NonActief = true;
			_daos.LedenDao.Bewaren(lid);
		}

		/// <summary>
		/// Haal een pagina op met leden van een groepswerkjaar.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <returns>Lijst met alle leden uit het gevraagde groepswerkjaar.</returns>
		public IList<Lid> PaginaOphalen(int groepsWerkJaarID)
		{
			if (!_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			var list = _daos.LedenDao.AllesOphalen(groepsWerkJaarID);
			list = list.OrderBy(e => e.GelieerdePersoon.Persoon.Naam).ThenBy(e => e.GelieerdePersoon.Persoon.VoorNaam).ToList();
			return list;
		}

		/// <summary>
		/// Haalt een 'pagina' op met leden uit een bepaald GroepsWerkJaar
		/// </summary>
		/// <param name="groepsWerkJaarID">ID gevraagde GroepsWerkJaar</param>
		/// <param name="afdelingsID">ID gevraagde afdeling</param>
		/// <returns>De 'pagina' (collectie) met leden</returns>
		public IList<Lid> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID)
		{
			if (!_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			
			return _daos.LedenDao.PaginaOphalenVolgensAfdeling(groepsWerkJaarID, afdelingsID);
		}

		/// <summary>
		/// Haalt een 'pagina' op met leden uit een bepaald GroepsWerkJaar
		/// </summary>
		/// <param name="groepsWerkJaarID">ID gevraagde GroepsWerkJaar</param>
		/// <param name="functieID">ID gevraagde functie</param>
		/// <returns></returns>
		public IList<Lid> PaginaOphalenVolgensFunctie(int groepsWerkJaarID, int functieID)
		{
			if (!_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			return _daos.LedenDao.PaginaOphalenVolgensFunctie(groepsWerkJaarID, functieID);
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

			if (lid is Kind)
			{
				try
				{
					return _daos.KindDao.Bewaren((Kind)lid);
				}
				catch (KeyViolationException<Kind>)
				{
					throw new BestaatAlException<Kind>(lid as Kind);
				}
			}
			else if (lid is Leiding)
			{
				try
				{
					return _daos.LeidingDao.Bewaren((Leiding)lid);
				}
				catch (Exception)
				{
					throw new BestaatAlException<Leiding>(lid as Leiding);
				}
			}
			else
			{
				throw new NotSupportedException(Properties.Resources.OngeldigLidType);
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
		public void InfoOvernemen(LidInfo lidInfo, Lid lid)
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
				leiding.DubbelpuntAbonnement = lidInfo.Dubbelpunt;
				leiding.NonActief = lidInfo.NonActief;
			}
		}

		/// <summary>
		/// Geeft een lijst terug van alle afdelingen waaraan het lid gegeven gekoppeld is.
		/// </summary>
		/// <returns>Lijst met afdelingen</returns>
		/// <remarks>Een kind is hoogstens aan 1 afdeling gekoppeld</remarks>
		public static IList<int> AfdelingIdLijstGet(Lid l)
		{
			IList<int> result = new List<int>();
			if (l is Kind)
			{
				if ((l as Kind).AfdelingsJaar != null)
				{
					result.Add((l as Kind).AfdelingsJaar.Afdeling.ID);
				}
			}
			else if (l is Leiding)
			{
				foreach (AfdelingsJaar aj in (l as Leiding).AfdelingsJaar)
				{
					result.Add(aj.Afdeling.ID);
				}
			}
			else
			{
				Debug.Assert(false, "Lid moet kind of leiding zijn.");
			}

			return result;
		}
	}
}
