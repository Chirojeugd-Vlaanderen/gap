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

using Chiro.Cdf.Data;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Fouten;
using Chiro.Gap.Fouten.Exceptions;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Worker die alle businesslogica i.v.m. groepen bevat (dat is breder dan 'Chirogroepen', bv. satellieten)
	/// </summary>
	public class GroepenManager
	{
		private IGroepenDao _groepenDao;
		private IAfdelingsJarenDao _afdJrDao;
		private IAfdelingenDao _afdelingenDao;
		private IAutorisatieManager _autorisatieMgr;
		private ICategorieenDao _categorieenDao;
		private IGelieerdePersonenDao _gelPersDao;

		/// <summary>
		/// Standaardconstructor voor GroepenManagers
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
				throw new GeenGavException(GeenGavFoutCode.Groep, Resources.GeenGavGroep);
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
				throw new GeenGavException(GeenGavFoutCode.GeenSuperGav, Properties.Resources.GeenSuperGav);
			};
		}

		/// <summary>
		/// Maakt een nieuwe afdeling voor een groep, zonder te persisteren
		/// </summary>
		/// <param name="groep">Groep waarvoor afdeling moet worden gemaakt</param>
		/// <param name="naam">Naam van de afdeling</param>
		/// <param name="afkorting">Handige afkorting voor in schemaatjes</param>
		/// <returns>De toegevoegde (maar nog niet gepersisteerde) afdeling</returns>
		public Afdeling AfdelingToevoegen(Groep groep, string naam, string afkorting)
		{
			if (_autorisatieMgr.IsGavGroep(groep.ID))
			{
				Afdeling a = new Afdeling
				{
					Afkorting = afkorting,
					Naam = naam
				};

				a.Groep = groep;
				groep.Afdeling.Add(a);

				return a;
			}
			else
			{
				throw new GeenGavException(GeenGavFoutCode.Groep, Resources.GeenGavGroep);
			}
		}

		/// <summary>
		/// Haat een afdeling op, op basis van <paramref name="afdelingID"/>
		/// </summary>
		/// <param name="afdelingID">ID van op te halen afdeling</param>
		/// <returns>De gevraagde afdeling</returns>
		public Afdeling AfdelingOphalen(int afdelingID)
		{
			if (_autorisatieMgr.IsGavAfdeling(afdelingID))
			{
				return _afdelingenDao.Ophalen(afdelingID);
			}
			else
			{
				throw new GeenGavException(GeenGavFoutCode.Afdeling, Resources.GeenGavAfdeling);
			}
		}

		/// <summary>
		/// Maakt een afdelingsjaar voor een groep en een afdeling
		/// </summary>
		/// <param name="a">Afdeling voor nieuw afdelingsjaar</param>
		/// <param name="oa">Te koppelen officiële afdeling</param>
		/// /// <param name="gwj">Groepswerkjaar (koppelt de afdeling aan een groep en een werkjaar)</param>
		/// <param name="geboorteJaarBegin">Geboortejaar van</param>
		/// <param name="geboorteJaarEind">Geboortejaar tot</param>
		/// <returns>Het aangemaakte afdelingsjaar</returns>
		public AfdelingsJaar AfdelingsJaarMaken(Afdeling a, OfficieleAfdeling oa, GroepsWerkJaar gwj, int geboorteJaarBegin, int geboorteJaarEind)
		{
			if (!_autorisatieMgr.IsGavAfdeling(a.ID))
			{
				throw new GeenGavException(GeenGavFoutCode.Afdeling, Resources.GeenGavAfdeling);
			}

			// Leden moeten minstens in het 1ste leerjaar zitten, alvorens we inschrijven.
			// De maximumleeftijd is arbitrair nattevingerwerk. :-)
			if  (!(gwj.WerkJaar - geboorteJaarEind >= Properties.Settings.Default.MinLidLeefTijd) 
				|| !(gwj.WerkJaar - geboorteJaarBegin <= Properties.Settings.Default.MaxLidLeefTijd)
				|| !(geboorteJaarBegin <= geboorteJaarEind))
			{
				throw new InvalidOperationException("Ongeldige geboortejaren voor het afdelingsjaar");
			}

			AfdelingsJaar afdelingsJaar = new AfdelingsJaar();

			// TODO check if no conflicts with existing afdelingsjaar

			afdelingsJaar.OfficieleAfdeling = oa;
			afdelingsJaar.Afdeling = a;
			afdelingsJaar.GroepsWerkJaar = gwj;
			afdelingsJaar.GeboorteJaarVan = geboorteJaarBegin;
			afdelingsJaar.GeboorteJaarTot = geboorteJaarEind;

			a.AfdelingsJaar.Add(afdelingsJaar);
			oa.AfdelingsJaar.Add(afdelingsJaar);
			gwj.AfdelingsJaar.Add(afdelingsJaar);

			return afdelingsJaar;
		}

		/// <summary>
		/// Haalt lijst officiële afdelingen op.
		/// </summary>
		/// <returns>Lijst officiële afdelingen</returns>
		public IList<OfficieleAfdeling> OfficieleAfdelingenOphalen()
		{
			// Iedereen heeft het recht deze op te halen.
			return _groepenDao.OphalenOfficieleAfdelingen();
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
				throw new GeenGavException(GeenGavFoutCode.Groep, Resources.GeenGavGroep);
			}
		}

		/// <summary>
		/// Haalt een groepsobject op zonder gerelateerde entiteiten
		/// </summary>
		/// <param name="groepID">ID van de op te halen groep</param>
		/// <returns>De groep met de opgegeven ID <paramref name="groepID"/></returns>
		public Groep Ophalen(int groepID)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				return _groepenDao.Ophalen(groepID);
			}
			else
			{
				throw new GeenGavException(GeenGavFoutCode.Groep, Resources.GeenGavGroep);
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
				throw new GeenGavException(GeenGavFoutCode.Groep, Resources.GeenGavGroep);
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
				throw new GeenGavException(GeenGavFoutCode.Groep, Resources.GeenGavGroep);
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
				throw new GeenGavException(GeenGavFoutCode.Groep, Resources.GeenGavGroep);
			}
			else
			{
				// Is er al een categorie met die code?
				Categorie bestaande = (from ctg in g.Categorie
									   where String.Compare(ctg.Code, categorieCode) == 0
									   select ctg).FirstOrDefault();

				if (bestaande != null)
				{
					throw new BlokkerendeObjectenException<BestaatAlFoutCode, Categorie>(
						BestaatAlFoutCode.CategorieCodeBestaatAl,
						new Categorie[] {bestaande}
						);
				}
				else
				{
					// Is er al een categorie met die beschrijving?
					bestaande = (from ctg in g.Categorie
								 where String.Compare(ctg.Naam, categorieNaam) == 0
								 select ctg).FirstOrDefault();

					if (bestaande != null)
					{
						throw new BlokkerendeObjectenException<BestaatAlFoutCode, Categorie>(
							BestaatAlFoutCode.CategorieNaamBestaatAl,
							new Categorie[] { bestaande }
							);
					}
					else
					{
						Categorie c = new Categorie();
						c.Naam = categorieNaam;
						c.Code = categorieCode;
						c.Groep = g;
						g.Categorie.Add(c);
						return c;
					}
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
				throw new GeenGavException(GeenGavFoutCode.Groep, Resources.GeenGavGroep);
			}
			else
			{
				// Controleer op dubbele code

				var bestaande = (from fun in g.Functie
								 where String.Compare(g.Code, code) == 0
								 select fun).FirstOrDefault();

				if (bestaande != null && bestaande.TeVerwijderen)
				{
					throw new InvalidOperationException(
						"Er bestaat al een functie met die code, gemarkeerd als TeVerwijderen");
				}
				else if (bestaande != null)
				{
					throw new BlokkerendeObjectenException<BestaatAlFoutCode, Functie>(
						BestaatAlFoutCode.FunctieCodeBestaatAl, new Functie[] { bestaande });
				}

				// Hetzelfde voor dubbele naam

				bestaande = (from fun in g.Functie
							 where String.Compare(g.Naam, naam) == 0
							 select fun).FirstOrDefault();

				if (bestaande != null && bestaande.TeVerwijderen)
				{
					throw new InvalidOperationException(
						"Er bestaat al een functie met die naam, gemarkeerd als TeVerwijderen");
				}
				else if (bestaande != null)
				{
					throw new BlokkerendeObjectenException<BestaatAlFoutCode, Functie>(
						BestaatAlFoutCode.FunctieNaamBestaatAl, new Functie[] { bestaande });
				}

				// Zonder problemen hier geraakt.  Dan kunnen we verder.

				Functie f = new Functie
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
			GroepsWerkJaar resultaat = new GroepsWerkJaar
			{
				Groep = groep,
				WerkJaar = werkJaar
			};
			groep.GroepsWerkJaar.Add(resultaat);
			return resultaat;
		}
	}
}
