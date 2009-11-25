using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Orm;
using Chiro.Gap.Fouten.Exceptions;
using Chiro.Cdf.Ioc;
using Chiro.Cdf.Data;
using System.Diagnostics;

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
		/// Haalt een lijst op van gelieerde personen, inclusief gekoppelde groep
		/// </summary>
		/// <param name="gelieerdePersonenIDs">ID's van de op te vragen
		/// gelieerde personen.</param>
		/// <returns>Lijst met gelieerde personen, met gekoppelde groep</returns>
		public IList<GelieerdePersoon> Ophalen(IEnumerable<int> gelieerdePersonenIDs)
		{
			return _dao.Ophalen(
				_autorisatieMgr.EnkelMijnGelieerdePersonen(gelieerdePersonenIDs), 
				gp=>gp.Groep);
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
		/// Haal een lijst op met alle gelieerde personen van een groep.
		/// </summary>
		/// <param name="groepID">GroepID van gevraagde groep</param>
		/// <returns>Lijst met alle gelieerde personen</returns>
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
				return _dao.PaginaOphalen(groepID, pagina, paginaGrootte, out aantalTotaal);
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

		#region Categoriegerelateerd

		// TODO: deze functionaliteit hoort volgens mij thuis in de CategorieenManager.  De namen moeten
		// dan wel aangepast worden.  GelieerdePersonenManager.CategorieKoppelen moet dan worden
		// CategorieenManager.PersonenKoppelen, enz.

		/// <summary>
		/// Voegt een rij <paramref name="gelieerdePersonen"/> toe aan een <paramref name="categorie"/>.
		/// Persisteert niet.
		/// </summary>
		/// <param name="gelieerdePersonen">rij gelieerde personen, met daaraan gekoppeld de groep</param>
		/// <param name="categorie">categorie, met daaraan gekoppeld de groep</param>
		/// <remarks>
		/// De groep van de categorie moet dezelfde zijn als alle groepen van alle gelieerde personen.
		/// </remarks>
		public void CategorieKoppelen(IEnumerable<GelieerdePersoon> gelieerdePersonen, Categorie categorie)
		{
			CategorieKoppelen(gelieerdePersonen, categorie, true);
		}

		/// <summary>
		/// Verwijdert een rij <paramref name="gelieerdePersonen"/> van een <paramref name="categorie"/>.
		/// Persisteert niet.
		/// </summary>
		/// <param name="gelieerdePersonen">rij gelieerde personen, met daaraan gekoppeld de groep</param>
		/// <param name="categorie">categorie, met daaraan gekoppeld de groep</param>
		/// <remarks>
		/// De groep van de categorie moet dezelfde zijn als alle groepen van alle gelieerde personen.
		/// Verder denk ik dat deze functie best protected of zo wordt, want om los te koppelen zet deze
		/// de 'te verwijderen' van een persoon op true.  Dit is verwarrend.
		/// </remarks>
		public void CategorieOntkoppelen(IEnumerable<GelieerdePersoon> gelieerdePersonen, Categorie categorie)
		{
			CategorieKoppelen(gelieerdePersonen, categorie, true);
		}

		/// <summary>
		/// Voegt een rij <paramref name="gelieerdePersonen"/> toe aan een <paramref name="categorie"/>.
		/// Persisteert wel.
		/// </summary>
		/// <param name="gelieerdePersonen">rij gelieerde personen, met daaraan gekoppeld de groep</param>
		/// <param name="categorie">categorie, met daaraan gekoppeld de groep</param>
		/// <remarks>
		/// De groep van de categorie moet dezelfde zijn als alle groepen van alle gelieerde personen.
		/// Verder denk ik dat deze functie best protected of zo wordt, want om los te koppelen zet deze
		/// de 'te verwijderen' van een persoon op true.  Dit is verwarrend.
		/// </remarks>
		public void CategorieOntkoppelenEnBewaren(IEnumerable<GelieerdePersoon> gelieerdePersonen, Categorie categorie)
		{
			var catMgr = Factory.Maak<CategorieenManager>();

			CategorieKoppelen(gelieerdePersonen, categorie, true);
			catMgr.BewarenMetPersonen(categorie);
		}


		/// <summary>
		/// Voegt een rij <paramref name="gelieerdePersonen"/> toe aan een <paramref name="categorie"/>, of
		/// koppelt de personen los.  Persisteert niet.
		/// </summary>
		/// <param name="gelieerdePersonen">rij gelieerde personen, met daaraan gekoppeld de groep</param>
		/// <param name="categorie">categorie, met daaraan gekoppeld de groep</param>
		/// <param name="koppelen">indien true, worden de <paramref name="gelieerdePersonen"/> vastgekoppeld,
		/// anders losgekoppeld.</param>
		/// <remarks>De groep van de categorie moet dezelfde zijn als alle groepen van alle gelieerde personen.
		/// Verder denk ik dat deze functie best protected of zo wordt, want om los te koppelen zet deze
		/// de 'te verwijderen' van een persoon op true.  Dit is verwarrend.</remarks>
		public void CategorieKoppelen(
			IEnumerable<GelieerdePersoon> gelieerdePersonen, 
			Categorie categorie,
			bool koppelen)
		{
			Debug.Assert(categorie.Groep != null);

			// Is de gebruiker GAV van de categorie?

			if (!_autorisatieMgr.IsGavCategorie(categorie.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGavCategorie);
			}

			// Koppel gelieerde personen een voor een.

			foreach (GelieerdePersoon gp in gelieerdePersonen)
			{
				// Controleer eerst of de rechten op de persoon ok zijn, en of de categorie overeenkomt
				// met de groep

				if (!_autorisatieMgr.IsGavGelieerdePersoon(gp.ID))
				{
					throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
				}

				Debug.Assert(gp.Groep != null);
				if (!(gp.Groep.Equals(categorie.Groep)))
				{
					throw new FoutieveGroepException(Properties.Resources.FoutieveGroepCategorie);
				}

				// Ik ga ervan uit dat zo'n entityset zelf checkt of er geen dubbels zijn?
				gp.Categorie.Add(categorie);

				categorie.GelieerdePersoon.Add(gp);
				gp.TeVerwijderen = !koppelen;
			}
		}



		// TODO: onderstaande functionaliteit hoort hier ook zeker niet thuis.  Ofwel naar CategorieenManager
		// ofwel naar Groepenmanager.


		public IEnumerable<Categorie> ophalenCategorieen(int groepID)
		{
			return _categorieenDao.OphalenVanGroep(groepID);
		}

		#endregion
	}
}
