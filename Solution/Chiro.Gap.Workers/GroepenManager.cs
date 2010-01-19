using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Fouten.Exceptions;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
	public class GroepenManager
	{
		private IGroepenDao _dao;
		private IAfdelingsJarenDao _afdao;
		private IAutorisatieManager _autorisatieMgr;
		private ICategorieenDao _categorieenDao;

		/// <summary>
		/// Deze constructor laat toe om een alternatieve repository voor
		/// de groepen te gebruiken.  Nuttig voor mocking en testing.
		/// </summary>
		/// <param name="dao">Alternatieve dao</param>
		public GroepenManager(IGroepenDao dao, IAfdelingsJarenDao afdao, ICategorieenDao categorieenDao, IAutorisatieManager autorisatieMgr)
		{
			_dao = dao;
			_afdao = afdao;
			_autorisatieMgr = autorisatieMgr;
			_categorieenDao = categorieenDao;
		}


		public Groep OphalenMetGroepsWerkJaren(int groepID)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				return _dao.OphalenMetGroepsWerkJaren(groepID);
			}
			else
			{
				throw new GeenGavException(Resources.GeenGavGroep);
			}
		}


		/// <summary>
		/// Maakt een nieuwe afdeling voor een groep, zonder te persisteren
		/// </summary>
		/// <param name="groep">Groep waarvoor afdeling moet worden gemaakt</param>
		/// <param name="naam">naam van de afdeling</param>
		/// <param name="afkorting">handige afkorting voor in schemaatjes</param>
		public Afdeling AfdelingToevoegen(Groep groep, string naam, string afkorting)
		{
			if (_autorisatieMgr.IsGavGroep(groep.ID))
			{
				Afdeling a = new Afdeling { Afkorting = afkorting, Naam = naam };

				a.Groep = groep;
				groep.Afdeling.Add(a);

				return a;
			}
			else
			{
				throw new GeenGavException(Resources.GeenGavGroep);
			}
		}

		/// <summary>
		/// Maakt een afdelingsjaar voor een groep en een afdeling
		/// </summary>
		/// <param name="g">Groep voor nieuw afdelingsjaar</param>
		/// <param name="a">Afdeling voor nieuw afdelingsjaar</param>
		/// <param name="oa">Te koppelen officiele afdeling</param>
		/// <param name="geboorteJaarBegin">Geboortejaar van</param>
		/// <param name="geboorteJaarEind">Geboortejaar tot</param>
		public AfdelingsJaar AfdelingsJaarMaken(Afdeling a, OfficieleAfdeling oa, GroepsWerkJaar gwj, int geboorteJaarBegin, int geboorteJaarEind)
		{
			if (!_autorisatieMgr.IsGavAfdeling(a.ID))
			{
				throw new GeenGavException(Resources.GeenGavAfdeling);
			}

			if (geboorteJaarBegin < System.DateTime.Today.Year - 20
			    || geboorteJaarBegin > geboorteJaarEind
			    || geboorteJaarEind > System.DateTime.Today.Year - 5)
			{
				throw new InvalidOperationException("Ongeldige geboortejaren voor het afdelingsjaar");
			}

			AfdelingsJaar afdelingsJaar = new AfdelingsJaar();

			//TODO check if no conflicts with existing afdelingsjaar

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
		/// Haalt lijst officiele afdelingen op.
		/// </summary>
		/// <returns>Lijst officiele afdelingen</returns>
		public IList<OfficieleAfdeling> OfficieleAfdelingenOphalen()
		{
			// Iedereen heeft het recht deze op te halen.
			return _dao.OphalenOfficieleAfdelingen();
		}

		/// <summary>
		/// Haalt recentste groepswerkjaar op voor gegeven groep
		/// </summary>
		/// <param name="p">ID van gegeven groep</param>
		/// <returns>Gevraagde groepswerkjaar</returns>
		public GroepsWerkJaar RecentsteGroepsWerkJaarGet(Groep g)
		{
			if (_autorisatieMgr.IsGavGroep(g.ID))
			{
				return _dao.RecentsteGroepsWerkJaarGet(g.ID);
			}
			else
			{
				throw new GeenGavException(Resources.GeenGavGroep);
			}
		}




		// Zit nu in AfdelingsJaarManager.Bewaren
		///// <summary>
		///// Persisteert AfdelingsJaar in de database
		///// </summary>
		///// <param name="a">Te persisteren AfdelingsJaar</param>
		///// <returns>De bewaarde groep</returns>
		//public AfdelingsJaar AfdelingsJaarBewaren(AfdelingsJaar a)
		//{
		//    if (_autorisatieMgr.IsGavGroepsWerkJaar(a.GroepsWerkJaar.ID))
		//    {
		//        return _afdao.Bewaren(a);
		//    }
		//    else
		//    {
		//        throw new GeenGavException(Resources.GeenGavGroep);
		//    }
		//}



		/// <summary>
		/// Persisteert groep in de database
		/// </summary>
		/// <param name="g">Te persisteren groep</param>
		/// <returns>De bewaarde groep</returns>
		public Groep Bewaren(Groep g, params Expression<Func<Groep, object>>[] paths)
		{
			if (_autorisatieMgr.IsGavGroep(g.ID))
			{
				return _dao.Bewaren(g, paths);
			}
			else
			{
				throw new GeenGavException(Resources.GeenGavGroep);
			}
		}


		/// <summary>
		/// Haalt een groepsobject op zonder gerelateerde entiteiten
		/// </summary>
		/// <param name="groepID">ID van de op te halen groep</param>
		/// <returns>groep met ID <paramref name="groepID"/></returns>
		public Groep Ophalen(int groepID)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				return _dao.Ophalen(groepID);
			}
			else
			{
				throw new GeenGavException(Resources.GeenGavGroep);
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
				return _dao.Ophalen(groepID, grp=>grp.Afdeling);
			}
			else
			{
				throw new GeenGavException(Resources.GeenGavGroep);
			}
		}

		/// <summary>
		/// Haalt een groep op, met daaraan gekoppeld al zijn categorieen
		/// </summary>
		/// <param name="groepID">ID van de gevraagde groep</param>
		/// <returns>De gevraagde groep, met daaraan gekoppeld al zijn categorieen</returns>
		public Groep OphalenMetCategorieen(int groepID)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				return _dao.Ophalen(groepID, grp => grp.Categorie);
			}
			else
			{
				throw new GeenGavException(Resources.GeenGavGroep);
			}
		}

		#region categorieen

		/// <summary>
		/// Persisteert wel
		/// </summary>
		/// <param name="c"></param>
		/// <param name="gID"></param>
		public Categorie CategorieToevoegen(Groep g, String cnaam, String code)
		{
			if (!_autorisatieMgr.IsGavGroep(g.ID))
			{
				throw new GeenGavException(Resources.GeenGavGroep);
			}

			Categorie c = new Categorie();
			c.Naam = cnaam;
			c.Code = code;
			c.Groep = g;
			g.Categorie.Add(c);
			return c;

		}

		#endregion categorieen

		/// <summary>
		/// Maakt een nieuw groepswerkjaar voor een gegeven <paramref name="groep" />
		/// </summary>
		/// <param name="groep">groep waarvoor een groepswerkjaar gemaakt moet worden</param>
		/// <param name="werkJaar">int die het werkjaar identificeert (bijv 2009 voor 2009-2010)</param>
		/// <returns>Het gemaakte groepswerkjaar.</returns>
		/// <remarks>Persisteert niet.</remarks>
		public GroepsWerkJaar GroepsWerkJaarMaken(Groep groep, int werkJaar)
		{
			GroepsWerkJaar resultaat = new GroepsWerkJaar { Groep = groep, WerkJaar = werkJaar };
			groep.GroepsWerkJaar.Add(resultaat);
			return resultaat;
		}
	}
}
