using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Fouten.Exceptions;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Workers
{
	public class GelieerdePersonenManager
	{
		private IGelieerdePersonenDao _dao;
		private IGroepenDao _groepenDao;
		private ICategorieenDao _categorieenDao;
		private IAutorisatieManager _autorisatieMgr;

		public GelieerdePersonenManager(IGelieerdePersonenDao dao, IGroepenDao groepenDao
		    , ICategorieenDao categorieenDao, IAutorisatieManager autorisatieMgr, IDao<CommunicatieType> typedao, IDao<CommunicatieVorm> commdao)
		{
			_dao = dao;
			_groepenDao = groepenDao;
			_categorieenDao = categorieenDao;
			_autorisatieMgr = autorisatieMgr;
		}

		#region proxy naar data access

		// Wel altijd rekening houden met authorisatie!

		/// <summary>
		/// Haalt gelieerde persoon met gekoppelde persoon op.
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van de gelieerde persoon.</param>
		/// <returns>Gelieerde persoon, met gekoppeld persoonsobject.</returns>
		/// <remarks>De groepsinfo wordt niet mee opgehaald, omdat we die in de
		/// meeste gevallen niet nodig zullen hebben.</remarks>
		public GelieerdePersoon Ophalen(int gelieerdePersoonID)
		{
			if (_autorisatieMgr.IsGavGelieerdePersoon(gelieerdePersoonID))
			{
				// ENKEL informatie gelieerde persoon en persoon ophalen.
				// Als je ook informatie wil van de groep, maak dan een aparte
				// method, of gebruik een lambda-expressie
				//
				// In de meeste gevallen zullen we bij het opvragen van een
				// gelieerde persoon de groepsinfo niet nodig hebben, aangezien
				// er aan die groepsinfo toch niets zal wijzigen.  Vandaar dat
				// die groepsinfo hier niet mee komt.

				return _dao.Ophalen(gelieerdePersoonID, foo => foo.Persoon);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
			}
		}

		public GelieerdePersoon OphalenMetCommVormen(int gelieerdePersoonID)
		{
			if (_autorisatieMgr.IsGavGelieerdePersoon(gelieerdePersoonID))
			{
				return _dao.Ophalen(gelieerdePersoonID, foo => foo.Persoon, foo => foo.Groep, foo => foo.Communicatie);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
			}
		}

		/// <summary>
		/// Haalt een lijst op van gelieerde personen.
		/// </summary>
		/// <param name="gelieerdePersonenIDs">ID's van de op te vragen
		/// gelieerde personen.</param>
		/// <returns>Lijst met gelieerde personen</returns>
		public IList<GelieerdePersoon> Ophalen(IEnumerable<int> gelieerdePersonenIDs)
		{
			return _dao.Ophalen(_autorisatieMgr.EnkelMijnGelieerdePersonen(gelieerdePersonenIDs));
		}

		/// <summary>
		/// Haalt gelieerde persoon op met persoonsgegevens, adressen en
		/// communicatievormen.
		/// </summary>
		/// <param name="gelieerdePersoonID">ID gevraagde gelieerde persoon</param>
		/// <returns>GelieerdePersoon met persoonsgegevens, adressen, categorieen en 
		/// communicatievormen.</returns>
		public GelieerdePersoon DetailsOphalen(int gelieerdePersoonID)
		{
			if (_autorisatieMgr.IsGavGelieerdePersoon(gelieerdePersoonID))
			{
				return _dao.DetailsOphalen(gelieerdePersoonID);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
			}
		}

		/// <summary>
		/// Bewaart een gelieerde persoon, zonder koppelingen
		/// </summary>
		/// <param name="p">Te bewaren gelieerde persoon</param>
		/// <returns>De bewaarde gelieerde persoon</returns>
		public GelieerdePersoon Bewaren(GelieerdePersoon p)
		{
			if (_autorisatieMgr.IsGavGelieerdePersoon(p.ID))
			{
				// Hier mapping gebruiken om te vermijden dat het AD-nummer
				// overschreven wordt, lijkt me wat overkill.  Ik vergelijk
				// hiet nieuwe AD-nummer gewoon met het bestaande.

				GelieerdePersoon origineel = _dao.Ophalen(p.ID, foo => foo.Persoon);
				if (origineel == null || origineel.Persoon.AdNummer == p.Persoon.AdNummer)
				{
					return _dao.Bewaren(p);
				}
				else
				{
					throw new InvalidOperationException(Properties.Resources.AdNummerNietWijzigen);
				}
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
			}
		}

		public GelieerdePersoon BewarenMetCommVormen(GelieerdePersoon p)
		{
			if (_autorisatieMgr.IsGavGelieerdePersoon(p.ID))
			{
				// Hier mapping gebruiken om te vermijden dat het AD-nummer
				// overschreven wordt, lijkt me wat overkill.  Ik vergelijk
				// hiet nieuwe AD-nummer gewoon met het bestaande.

				GelieerdePersoon origineel = _dao.Ophalen(p.ID, e => e.Persoon, e => e.Communicatie.First().CommunicatieType);
				if (origineel.Persoon.AdNummer == p.Persoon.AdNummer)
				{
					return _dao.Bewaren(p, e => e.Persoon, e => e.Communicatie.First().CommunicatieType);
				}
				else
				{
					throw new InvalidOperationException(Properties.Resources.AdNummerNietWijzigen);
				}
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
			}
		}

		/// <summary>
		/// Haal een lijst op met alle gelieerde personen van een groep, inclusief persoons- en lidinfo.
		/// </summary>
		/// <param name="groepID">GroepID van gevraagde groep</param>
		/// <returns>Lijst met alle gelieerde personen, inclusief persoons- en lidinfo</returns>
		/// <remarks>Opgelet! Dit kan een zware query zijn!</remarks>
		public IList<GelieerdePersoon> AllenOphalen(int groepID)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				return _dao.AllenOphalen(groepID);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGroep);
			}
		}

		/// <summary>
		/// Haal een pagina op met gelieerde personen van een groep.
		/// </summary>
		/// <param name="groepID">GroepID gevraagde groep</param>
		/// <param name="pagina">paginanummer (>=1)</param>
		/// <param name="paginaGrootte">grootte van de pagina's</param>
		/// <param name="aantalTotaal">totaal aantal personen in de groep</param>
		/// <returns>Lijst met een pagina aan gelieerde personen.</returns>
		public IList<GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				IList<GelieerdePersoon> list = _dao.PaginaOphalen(groepID, e => e.Groep.ID, pagina, paginaGrootte, out aantalTotaal);
				list.OrderBy(e => e.Persoon.Naam).ThenBy(e => e.Persoon.VoorNaam);
				return list;
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGroep);
			}
		}

		/// <summary>
		/// Haalt een pagina op met gelieerde personen van een groep,
		/// inclusief eventuele lidobjecten voor deze groep
		/// </summary>
		/// <param name="groepID">GroepID gevraagde groep</param>
		/// <param name="pagina">paginanummer (>=1)</param>
		/// <param name="paginaGrootte">aantal personen per pagina</param>
		/// <param name="aantalTotaal">output parameter voor totaal aantal
		/// personen in de groep</param>
		/// <returns>Lijst met GelieerdePersonen</returns>
		public IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				return _dao.PaginaOphalenMetLidInfo(groepID, pagina, paginaGrootte, out aantalTotaal);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGroep);
			}
		}

		/// <summary>
		/// Haalt een pagina op met gelieerde personen van een groep die tot de categorie behoren,
		/// inclusief eventuele lidobjecten voor deze groep
		/// </summary>
		/// <param name="categorieID">ID gevraagde categorie</param>
		/// <param name="pagina">paginanummer (minstens 1)</param>
		/// <param name="paginaGrootte">aantal personen per pagina</param>
		/// <param name="aantalTotaal">output parameter voor totaal aantal
		/// personen in de groep</param>
		/// <returns>Lijst met GelieerdePersonen</returns>
		public IList<GelieerdePersoon> PaginaOphalenMetLidInfoVolgensCategorie(int categorieID, int pagina, int paginaGrootte, out int aantalTotaal)
		{
			if (_autorisatieMgr.IsGavCategorie(categorieID))
			{
				return _dao.PaginaOphalenMetLidInfoVolgensCategorie(categorieID, pagina, paginaGrootte, out aantalTotaal);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavCategorie);
			}
		}

		/// <summary>
		/// Koppelt het relevante groepsobject aan de gegeven
		/// gelieerde persoon.
		/// </summary>
		/// <param name="gp">gelieerde persoon</param>
		/// <returns>diezelfde gelieerde persoon, met zijn groep eraan
		/// gekoppeld.</returns>
		public GelieerdePersoon GroepLaden(GelieerdePersoon gp)
		{
			if (_autorisatieMgr.IsGavGelieerdePersoon(gp.ID))
			{
				return _dao.GroepLaden(gp);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
			}
		}

		/// <summary>
		/// Zoekt naar gelieerde personen van een bepaalde groep (met ID <paramref name="groepID"/> met naam 
		/// en voornaam gelijkaardig aan <paramref name="naam"/> en <paramref name="voornaam"/>.
		/// (inclusief communicatie en adressen)
		/// </summary>
		/// <param name="groepID">GroepID dat bepaalt in welke gelieerde personen gezocht mag worden</param>
		/// <param name="naam">te zoeken naam (ongeveer)</param>
		/// <param name="voornaam">te zoeken voornaam (ongeveer)</param>
		/// <returns>lijst met gevonden matches</returns>
		public IList<GelieerdePersoon> ZoekenOpNaamOngeveer(int groepID, string naam, string voornaam)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				return _dao.ZoekenOpNaamOngeveer(groepID, naam, voornaam);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGroep);
			}
		}

		#endregion


		/// <summary>
		/// Maak een GelieerdePersoon voor gegeven persoon en groep
		/// </summary>
		/// <param name="persoon">te lieren persoon</param>
		/// <param name="groep">groep waaraan te lieren</param>
		/// <param name="chiroLeeftijd">chiroleeftijd gelieerde persoon</param>
		/// <returns>een nieuwe GelieerdePersoon</returns>
		public GelieerdePersoon Koppelen(Persoon persoon, Groep groep, int chiroLeeftijd)
		{
			if (_autorisatieMgr.IsGavGroep(groep.ID))
			{
				GelieerdePersoon resultaat = new GelieerdePersoon();

				resultaat.Persoon = persoon;
				resultaat.Groep = groep;
				resultaat.ChiroLeefTijd = chiroLeeftijd;

				persoon.GelieerdePersoon.Add(resultaat);
				groep.GelieerdePersoon.Add(resultaat);

				return resultaat;
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGroep);
			}
		}

		/// <summary>
		/// Maakt GelieerdePersoon, gekoppelde Persoon, Adressen en Communicatie allemaal
		/// te verwijderen.  Persisteert niet!
		/// </summary>
		/// <param name="gp">Te verwijderen gelieerde persoon</param>
		/// <remarks>Deze wijziging wordt nog niet gepersisteerd in de database! Hiervoor
		/// moet eerst 'Bewaren' aangeroepen worden!</remarks>
		public void VolledigVerwijderen(GelieerdePersoon gp)
		{
			if (_autorisatieMgr.IsGavGelieerdePersoon(gp.ID))
			{
				gp.TeVerwijderen = true;
				gp.Persoon.TeVerwijderen = true;

				foreach (PersoonsAdres pa in gp.Persoon.PersoonsAdres)
				{
					pa.TeVerwijderen = true;
				}

				foreach (CommunicatieVorm cv in gp.Communicatie)
				{
					cv.TeVerwijderen = true;
				}
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
			}
		}


		public void Bewaren(IList<GelieerdePersoon> personenLijst)
		{
			throw new NotImplementedException();
		}

		public bool IsLid(int gelieerdePersoonID)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Koppelt een gelieerde persoon aan een categorie, en persisteert dan de aanpassingen
		/// </summary>
		/// <param name="gelieerdePersonen">te koppelen gelieerde persoon</param>
		/// <param name="categorie">te koppelen categorie</param>
		public void CategorieKoppelen(IList<GelieerdePersoon> gelieerdePersonen, Categorie c)
		{
			// Heeft de gebruiker rechten voor de groep en de categorie?
			foreach (GelieerdePersoon x in gelieerdePersonen)
			{
				if (!_autorisatieMgr.IsGavGelieerdePersoon(x.ID))
				{
					throw new GeenGavException(Properties.Resources.GeenGavCategoriePersoon);
				};
			}

			if (!_autorisatieMgr.IsGavCategorie(c.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGavCategoriePersoon);
			}

			foreach (GelieerdePersoon x in gelieerdePersonen)
			{
				if (!x.Groep.Equals(c.Groep))
				{
					throw new FoutieveGroepException(Properties.Resources.FoutieveGroepCategorie);
				}
				x.Categorie.Add(c);
				c.GelieerdePersoon.Add(x);
			}
		}

		/// <summary>
		/// Verwijdert de gelieerde personen uit de categorie
		/// </summary>
		/// <remarks>De methode is reentrant, als er bepaalde personen niet gelinkt zijn aan de categorie, 
		/// gebeurd er niets met die personen, ook geen error.
		/// </remarks>
		/// <param name="gelieerdePersonenIDs">gelieerde persoon IDs</param>
		/// <param name="categorie">te verwijderen categorie MET gelinkte gelieerdepersonen </param>
		public void CategorieLoskoppelen(IList<int> gelieerdePersonenIDs, Categorie categorie)
		{
			// Heeft de gebruiker rechten voor de groep en de categorie?
			foreach (int x in gelieerdePersonenIDs)
			{
				if (!_autorisatieMgr.IsGavGelieerdePersoon(x))
				{
					throw new GeenGavException(Properties.Resources.GeenGavCategoriePersoon);
				};
			}

			if (!_autorisatieMgr.IsGavCategorie(categorie.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGavCategoriePersoon);
			}

			IList<GelieerdePersoon> gel = 
					(from gp in categorie.GelieerdePersoon
					 where gelieerdePersonenIDs.Contains(gp.ID)
					 select gp).ToList();

			foreach(GelieerdePersoon gp in gel)
			{
				gp.TeVerwijderen = true;
			}
		}

		public Categorie OphalenCategorie(int catID)
		{
			// Heeft de gebruiker rechten voor de groep en de categorie?

			if (!_autorisatieMgr.IsGavCategorie(catID))
			{
				throw new GeenGavException(Properties.Resources.GeenGavCategoriePersoon);
			};

			return _categorieenDao.Ophalen(catID);
		}

		public IEnumerable<Categorie> CategorieenOphalen(int groepID)
		{
			return _categorieenDao.OphalenVanGroep(groepID);
		}

		/// <summary>
		/// Zoekt naar gelieerde personen die gelijkaardig zijn aan een gegeven
		/// <paramref name="persoon"/>.
		/// </summary>
		/// <param name="persoon">Persoon waarmee vergeleken moet worden</param>
		/// <param name="groepID">ID van groep waarin te zoeken</param>
		/// <returns>Lijstje met gelijkaardige personen</returns>
		public IList<GelieerdePersoon> ZoekGelijkaardig(Persoon persoon, int groepID)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				// Momenteel wordt er enkel op 'naam ongeveer' gezocht, maar
				// ik kan me voorstellen dat deze functie in de toekomst wat
				// gesofisticeerder wordt.

				return _dao.ZoekenOpNaamOngeveer(groepID, persoon.Naam, persoon.VoorNaam);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGroep);
			}
		}
	}
}
