// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Worker die alle businesslogica i.v.m. groepen bevat (dat is breder dan 'Chirogroepen', bv. satellieten)
	/// </summary>
	public class GroepenManager
	{
		private readonly IGroepenDao _groepenDao;
		private readonly IAfdelingsJarenDao _afdJrDao;
		private readonly IAfdelingenDao _afdelingenDao;
		private readonly IAutorisatieManager _autorisatieMgr;
		private readonly ICategorieenDao _categorieenDao;
		private readonly IGelieerdePersonenDao _gelPersDao;

		/// <summary>
		/// De standaardconstructor voor GroepenManagers
		/// </summary>
		/// <param name="grpDao">Repository voor groepen</param>
		/// <param name="afjDao">Repository voor afdelingsjaren</param>
		/// <param name="afdDao">Repository voor afdelingen</param>
		/// <param name="categorieenDao">Repository voor categorieën</param>
		/// <param name="gelPersDao">Repository voor gelieerde personen</param>
		/// <param name="autorisatieMgr">Worker die autorisatie regelt</param>
		public GroepenManager(
			IGroepenDao grpDao,
			IAfdelingsJarenDao afjDao,
			IAfdelingenDao afdDao,
			ICategorieenDao categorieenDao,
			IGelieerdePersonenDao gelPersDao,
			IAutorisatieManager autorisatieMgr)
		{
			_groepenDao = grpDao;
			_afdJrDao = afjDao;
			_afdelingenDao = afdDao;
			_autorisatieMgr = autorisatieMgr;
			_categorieenDao = categorieenDao;
			_gelPersDao = gelPersDao;
		}

		/// <summary>
		/// Haalt een groep op, met daaraan gekoppeld alle groepswerkjaren
		/// </summary>
		/// <param name="groepID">ID van de op te halen groep</param>
		/// <returns>De gevraagde groep, met daaraan gekoppeld al zijn groepswerkjaren</returns>
		public Groep OphalenMetGroepsWerkJaren(int groepID)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				return _groepenDao.OphalenMetGroepsWerkJaren(groepID);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Verwijdert alle gelieerde personen van de groep met ID <paramref name="groepID"/>.  Probeert ook
		/// de gekoppelde personen te verwijderen, indien <paramref name="verwijderPersonen"/> <c>true</c> is.
		/// Verwijdert ook mogelijke lidobjecten.
		/// PERSISTEERT!
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan je de gelieerde personen wilt verwijderen</param>
		/// <param name="verwijderPersonen">Indien <c>true</c>, worden ook de personen vewijderd waarvoor
		/// een GelieerdePersoon met de groep bestond.</param>
		/// <remarks>Deze functie vereist super-GAV-rechten</remarks>
		public void GelieerdePersonenVerwijderen(int groepID, bool verwijderPersonen)
		{
			if (_autorisatieMgr.IsSuperGavGroep(groepID))
			{
				// Alle gelieerde personen van de groep ophalen
				IList<GelieerdePersoon> allePersonen = _gelPersDao.AllenOphalen(
					groepID,
					gp => gp.Lid, gp => gp.Persoon);

				// Alle gelieerde personen als 'te verwijderen' markeren
				foreach (GelieerdePersoon gp in allePersonen)
				{
					gp.TeVerwijderen = true;

					// Alle leden als 'te verwijderen' markeren
					foreach (Lid ld in gp.Lid)
					{
						ld.TeVerwijderen = true;
					}

					// Markeer zo nodig ook de persoon
					if (verwijderPersonen)
					{
						gp.Persoon.TeVerwijderen = true;
					}
				}

				// Persisteer
				_gelPersDao.Bewaren(allePersonen, gp => gp.Lid, gp => gp.Persoon);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Maakt een nieuwe afdeling voor een groep, zonder te persisteren
		/// </summary>
		/// <param name="groep">Groep waarvoor afdeling moet worden gemaakt, met daaraan gekoppeld
		/// de bestaande afdelingen</param>
		/// <param name="naam">Naam van de afdeling</param>
		/// <param name="afkorting">Handige afkorting voor in schemaatjes</param>
		/// <returns>De toegevoegde (maar nog niet gepersisteerde) afdeling</returns>
		public Afdeling AfdelingToevoegen(Groep groep, string naam, string afkorting)
		{
			if (!_autorisatieMgr.IsGavGroep(groep.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		
			// Controleren of de afdeling nog niet bestaat

			var bestaand = from afd in groep.Afdeling
					   where String.Compare(afd.Afkorting, afkorting, true) == 0
					   || String.Compare(afd.Naam, naam, true) == 0
					   select afd;

			if (bestaand.FirstOrDefault() != null)
			{
				throw new BestaatAlException<Afdeling>(bestaand.FirstOrDefault());
			}

			var a = new Afdeling
			{
				Afkorting = afkorting,
				Naam = naam
			};

			a.Groep = groep;
			groep.Afdeling.Add(a);

			return a;
		}

		/// <summary>
		/// Persisteert groep in de database
		/// </summary>
		/// <param name="g">Te persisteren groep</param>
		/// <param name="paths">Expressies die aangeven welke dependencies mee opgehaald moeten worden</param>
		/// <returns>De bewaarde groep</returns>
		public Groep Bewaren(Groep g, params Expression<Func<Groep, object>>[] paths)
		{
			if (_autorisatieMgr.IsGavGroep(g.ID))
			{
				return _groepenDao.Bewaren(g, paths);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Haalt een groepsobject op zonder gerelateerde entiteiten
		/// </summary>
		/// <param name="groepID">ID van de op te halen groep</param>
		/// <returns>De groep met de opgegeven ID <paramref name="groepID"/></returns>
		public Groep Ophalen(int groepID)
		{
			return Ophalen(groepID, GroepsExtras.Geen);
		}

		/// <summary>
		/// Haalt een groepsobject op
		/// </summary>
		/// <param name="groepID">ID van de op te halen groep</param>
		/// <param name="extras">Geeft aan of er gekoppelde entiteiten mee opgehaald moeten worden.</param>
		/// <returns>De groep met de opgegeven ID <paramref name="groepID"/></returns>
		public Groep Ophalen(int groepID, GroepsExtras extras)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				if ((extras | GroepsExtras.AlleAfdelingen) != 0)
				{
					return _groepenDao.Ophalen(groepID, grp => grp.Afdeling);
				}
				else
				{
					return _groepenDao.Ophalen(groepID);
				}
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Haalt een groep op, met daaraan gekoppeld al zijn afdelingen
		/// </summary>
		/// <param name="groepID">ID van de gevraagde groep</param>
		/// <returns>De gevraagde groep, met daaraan gekoppeld al zijn afdelingen</returns>
		public Groep OphalenMetAfdelingen(int groepID)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				return _groepenDao.Ophalen(groepID, grp => grp.Afdeling);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Haalt een groep op, met daaraan gekoppeld al zijn categorieën
		/// </summary>
		/// <param name="groepID">ID van de gevraagde groep</param>
		/// <returns>De gevraagde groep, met daaraan gekoppeld al zijn categorieën</returns>
		public Groep OphalenMetCategorieen(int groepID)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				return _groepenDao.Ophalen(groepID, grp => grp.Categorie);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		#region categorieën

		/// <summary>
		/// Maakt een nieuwe categorie, en koppelt die aan een bestaande groep (met daaraan
		/// gekoppeld zijn categorieën)
		/// </summary>
		/// <param name="g">Groep waarvoor de categorie gemaakt wordt.  Als bestaande categorieën
		/// gekoppeld zijn, wordt op dubbels gecontroleerd</param>
		/// <param name="categorieNaam">Naam voor de nieuwe categorie</param>
		/// <param name="categorieCode">Code voor de nieuwe categorie</param>
		/// <returns>De toegevoegde categorie</returns>
		public Categorie CategorieToevoegen(Groep g, String categorieNaam, String categorieCode)
		{
			if (!_autorisatieMgr.IsGavGroep(g.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			else
			{
				// Is er al een categorie met die code?
				Categorie bestaande = (from ctg in g.Categorie
						       where String.Compare(ctg.Code, categorieCode, true) == 0
							|| String.Compare(ctg.Naam, categorieNaam, true) == 0
						       select ctg).FirstOrDefault();

				if (bestaande != null)
				{
					throw new BestaatAlException<Categorie>(bestaande);
				}
				else
				{
					var c = new Categorie();
					c.Naam = categorieNaam;
					c.Code = categorieCode;
					c.Groep = g;
					g.Categorie.Add(c);
					return c;
				}
			}
		}

		#endregion categorieën

		/// <summary>
		/// Maakt een nieuwe (groepseigen) functie voor groep <paramref name="g"/>.  Persisteert niet.
		/// </summary>
		/// <param name="g">Groep waarvoor de functie gemaakt wordt</param>
		/// <param name="naam">Naam van de functie</param>
		/// <param name="code">Code van de functie</param>
		/// <param name="maxAantal">Maximum aantal leden in de categorie.  Onbeperkt indien null.</param>
		/// <param name="minAantal">Minimum aantal leden in de categorie.</param>
		/// <param name="lidType">LidType waarvoor de functie van toepassing is</param>
		/// <param name="werkJaarVan">Werkjaar vanaf wanneer de categorie gebruikt mag worden.</param>
		/// <returns>De nieuwe (gekoppelde) functie</returns>
		public Functie FunctieToevoegen(
			Groep g,
			string naam,
			string code,
			int? maxAantal,
			int minAantal,
			LidType lidType,
			int? werkJaarVan)
		{
			if (!_autorisatieMgr.IsGavGroep(g.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			else
			{
				// Controleer op dubbele code

				var bestaande = (from fun in g.Functie
								 where String.Compare(g.Code, code) == 0
								 || String.Compare(g.Naam, naam) == 0
								 select fun).FirstOrDefault();

				if (bestaande != null && bestaande.TeVerwijderen)
				{
					throw new InvalidOperationException(
						"Er bestaat al een functie met die code, gemarkeerd als TeVerwijderen");
				}
				else if (bestaande != null)
				{
					throw new BestaatAlException<Functie>(bestaande);
				}

				// Zonder problemen hier geraakt.  Dan kunnen we verder.

				var f = new Functie
				{
					Code = code,
					Groep = g,
					MaxAantal = maxAantal,
					MinAantal = minAantal,
					Type = lidType,
					Naam = naam,
					WerkJaarTot = null,
					WerkJaarVan = werkJaarVan, 
					IsNationaal = false
				};

				g.Functie.Add(f);

				return f;
			}
		}

		/// <summary>
		/// Maakt een nieuw groepswerkjaar voor een gegeven <paramref name="groep" />
		/// </summary>
		/// <param name="groep">Groep waarvoor een groepswerkjaar gemaakt moet worden</param>
		/// <param name="werkJaar">Int die het werkjaar identificeert (bv. 2009 voor 2009-2010)</param>
		/// <returns>Het gemaakte groepswerkjaar.</returns>
		/// <remarks>Persisteert niet.</remarks>
		public GroepsWerkJaar GroepsWerkJaarMaken(Groep groep, int werkJaar)
		{
			var resultaat = new GroepsWerkJaar
			{
				Groep = groep,
				WerkJaar = werkJaar
			};
			groep.GroepsWerkJaar.Add(resultaat);
			return resultaat;
		}

		/// <summary>
		/// Haalt groep op met gegeven stamnummer
		/// </summary>
		/// <param name="code">Stamnummer op te halen groep</param>
		/// <returns>Groep met <paramref name="code"/> als stamnummer</returns>
		public Groep Ophalen(string code)
		{
			var resultaat = _groepenDao.Ophalen(code);

			if (_autorisatieMgr.IsGavGroep(resultaat.ID))
			{
				return resultaat;
			}
			throw new GeenGavException(Properties.Resources.GeenGav);
		}
	}
}
