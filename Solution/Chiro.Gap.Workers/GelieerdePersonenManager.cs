// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

using Chiro.Cdf.Data;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Worker die alle businesslogica i.v.m. gelieerde personen bevat
	/// </summary>
	public class GelieerdePersonenManager
	{
		private readonly IGelieerdePersonenDao _dao;
		private readonly IGroepenDao _groepenDao;
		private readonly ICategorieenDao _categorieenDao;
		private readonly IAutorisatieManager _autorisatieMgr;

		/// <summary>
		/// Creëert een GelieerdePersonenManager
		/// </summary>
		/// <param name="dao">Repository voor gelieerde personen</param>
		/// <param name="groepenDao">Repository voor groepen</param>
		/// <param name="categorieenDao">Repository voor categorieën</param>
		/// <param name="autorisatieMgr">Worker die autorisatie regelt</param>
		/// <param name="typedao">Repository voor communicatietypes</param>
		/// <param name="commdao">Repository voor communicatievormen</param>
		public GelieerdePersonenManager(
			IGelieerdePersonenDao dao,
			IGroepenDao groepenDao,
			ICategorieenDao categorieenDao,
			IAutorisatieManager autorisatieMgr,
			IDao<CommunicatieType> typedao,
			IDao<CommunicatieVorm> commdao)
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
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Een gelieerde persoon ophalen met al zijn/haar communicatievormen
		/// </summary>
		/// <param name="gelieerdePersoonID">De ID van de gelieerde persoon</param>
		/// <returns>Een gelieerde persoon met al zijn/haar communicatievormen</returns>
		public GelieerdePersoon OphalenMetCommVormen(int gelieerdePersoonID)
		{
			if (_autorisatieMgr.IsGavGelieerdePersoon(gelieerdePersoonID))
			{
				return _dao.Ophalen(gelieerdePersoonID, foo => foo.Persoon, foo => foo.Groep, foo => foo.Communicatie);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
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
		/// <returns>GelieerdePersoon met persoonsgegevens, adressen, categorieën, lidgegevens en 
		/// communicatievormen.</returns>
		public GelieerdePersoon DetailsOphalen(int gelieerdePersoonID)
		{
			if (_autorisatieMgr.IsGavGelieerdePersoon(gelieerdePersoonID))
			{
				return _dao.DetailsOphalen(gelieerdePersoonID);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
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

				// Er mag niet gepoterd worden met PersoonID en AdNummer

				var origineel = _dao.Ophalen(p.ID, foo => foo.Persoon);

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
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// TODO: documenteren
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
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
				throw new GeenGavException(Properties.Resources.GeenGav);
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
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Haal een pagina op met gelieerde personen van een groep.
		/// </summary>
		/// <param name="groepID">GroepID gevraagde groep</param>
		/// <param name="pagina">Paginanummer (>=1)</param>
		/// <param name="paginaGrootte">Grootte van de pagina's</param>
		/// <param name="aantalTotaal">Totaal aantal personen in de groep</param>
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
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Haalt een pagina op met gelieerde personen van een groep,
		/// inclusief eventuele lidobjecten voor deze groep
		/// </summary>
		/// <param name="groepID">GroepID gevraagde groep</param>
		/// <param name="pagina">Paginanummer (>=1)</param>
		/// <param name="paginaGrootte">Aantal personen per pagina</param>
		/// <param name="aantalTotaal">Outputparameter voor totaal aantal
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
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Haalt een pagina op met gelieerde personen van een groep die tot de categorie behoren,
		/// inclusief eventuele lidobjecten voor deze groep
		/// </summary>
		/// <param name="categorieID">ID gevraagde categorie</param>
		/// <param name="pagina">Paginanummer (minstens 1)</param>
		/// <param name="paginaGrootte">Aantal personen per pagina</param>
		/// <param name="aantalTotaal">Outputparameter voor totaal aantal
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
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Koppelt het relevante groepsobject aan de gegeven
		/// gelieerde persoon.
		/// </summary>
		/// <param name="gp">Gelieerde persoon</param>
		/// <returns>Diezelfde gelieerde persoon, met zijn of haar groep 
		/// eraan gekoppeld.</returns>
		public GelieerdePersoon GroepLaden(GelieerdePersoon gp)
		{
			if (_autorisatieMgr.IsGavGelieerdePersoon(gp.ID))
			{
				return _dao.GroepLaden(gp);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Zoekt naar gelieerde personen van een bepaalde groep (met ID <paramref name="groepID"/> met naam 
		/// en voornaam gelijkaardig aan <paramref name="naam"/> en <paramref name="voornaam"/>.
		/// (inclusief communicatie en adressen)
		/// </summary>
		/// <param name="groepID">GroepID dat bepaalt in welke gelieerde personen gezocht mag worden</param>
		/// <param name="naam">Te zoeken naam (ongeveer)</param>
		/// <param name="voornaam">Te zoeken voornaam (ongeveer)</param>
		/// <returns>Lijst met gevonden matches</returns>
		public IList<GelieerdePersoon> ZoekenOpNaamOngeveer(int groepID, string naam, string voornaam)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				return _dao.ZoekenOpNaamOngeveer(groepID, naam, voornaam);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		#endregion

		/// <summary>
		/// Maak een GelieerdePersoon voor gegeven persoon en groep
		/// </summary>
		/// <param name="persoon">Te liëren persoon</param>
		/// <param name="groep">Groep waaraan te liëren</param>
		/// <param name="chiroLeeftijd">Chiroleeftijd gelieerde persoon</param>
		/// <returns>Een nieuwe GelieerdePersoon</returns>
		public GelieerdePersoon Koppelen(Persoon persoon, Groep groep, int chiroLeeftijd)
		{
			if (_autorisatieMgr.IsGavGroep(groep.ID))
			{
				var resultaat = new GelieerdePersoon();

				resultaat.Persoon = persoon;
				resultaat.Groep = groep;
				resultaat.ChiroLeefTijd = chiroLeeftijd;

				persoon.GelieerdePersoon.Add(resultaat);
				groep.GelieerdePersoon.Add(resultaat);

				return resultaat;
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Maakt GelieerdePersoon, gekoppelde Persoon, Adressen en Communicatie allemaal
		/// te verwijderen.  Persisteert!
		/// </summary>
		/// <param name="gp">Te verwijderen gelieerde persoon</param>
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

				_dao.Bewaren(gp, gpers => gpers.Persoon.PersoonsAdres, gpers => gpers.Communicatie);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// TODO: documenteren
		/// </summary>
		/// <param name="personenLijst"></param>
		public void Bewaren(IList<GelieerdePersoon> personenLijst)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// TODO: documenteren
		/// </summary>
		/// <param name="gelieerdePersoonID"></param>
		/// <returns></returns>
		public bool IsLid(int gelieerdePersoonID)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Koppelt een gelieerde persoon aan een categorie, en persisteert dan de aanpassingen
		/// </summary>
		/// <param name="gelieerdePersonen">Te koppelen gelieerde persoon</param>
		/// <param name="c">Te koppelen categorie</param>
		public void CategorieKoppelen(IList<GelieerdePersoon> gelieerdePersonen, Categorie c)
		{
			// Heeft de gebruiker rechten voor de groep en de categorie?
			if (gelieerdePersonen.Any(x => !_autorisatieMgr.IsGavGelieerdePersoon(x.ID)))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			if (!_autorisatieMgr.IsGavCategorie(c.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			foreach (GelieerdePersoon x in gelieerdePersonen)
			{
				if (!x.Groep.Equals(c.Groep))
				{
					throw new GapException(
						FoutNummers.CategorieNietVanGroep, 
						Properties.Resources.FoutieveGroepCategorie);
				}
				x.Categorie.Add(c);
				c.GelieerdePersoon.Add(x);
			}
		}

		/// <summary>
		/// Verwijdert de gelieerde personen uit de categorie, en persisteert
		/// </summary>
		/// <remarks>De methode is reentrant, als er bepaalde personen niet gelinkt zijn aan de categorie, 
		/// gebeurt er niets met die personen, ook geen error.
		/// </remarks>
		/// <param name="gelieerdePersonenIDs">Gelieerde persoon IDs</param>
		/// <param name="categorie">Te verwijderen categorie MET gelinkte gelieerdepersonen </param>
		/// <returns>Een kloon van de categorie, waaruit de gevraagde personen verwijderd zijn</returns>
		public Categorie CategorieLoskoppelen(IList<int> gelieerdePersonenIDs, Categorie categorie)
		{
			// Heeft de gebruiker rechten voor de groep en de categorie?
			if (gelieerdePersonenIDs.Any(x => !_autorisatieMgr.IsGavGelieerdePersoon(x)))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			if (!_autorisatieMgr.IsGavCategorie(categorie.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			IList<GelieerdePersoon> gel =
					(from gp in categorie.GelieerdePersoon
					 where gelieerdePersonenIDs.Contains(gp.ID)
					 select gp).ToList();

			foreach (GelieerdePersoon gp in gel)
			{
				gp.TeVerwijderen = true;
			}

			return _categorieenDao.Bewaren(categorie, cat => cat.GelieerdePersoon);
		}

		/// <summary>
		/// Een categorie ophalen op basis van de opgegeven ID
		/// </summary>
		/// <param name="catID">De ID van de categorie die je nodig hebt</param>
		/// <returns>De categorie met de opgegeven ID</returns>
		public Categorie OphalenCategorie(int catID)
		{
			// Heeft de gebruiker rechten voor de groep en de categorie?

			if (!_autorisatieMgr.IsGavCategorie(catID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			return _categorieenDao.Ophalen(catID);
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
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}
	}
}
