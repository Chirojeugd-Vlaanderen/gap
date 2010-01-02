using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Gap.Fouten.Exceptions;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;


namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Klasse die methods bevat voor de businesslogica m.b.t. categorieen.
	/// </summary>
	public class CategorieenManager
	{
		private ICategorieenDao _dao;
		private IAutorisatieManager _autorisatieMgr;

		/// <summary>
		/// Creeert een CategorieenManager
		/// </summary>
		/// <param name="dao">Repository voor Categorieen</param>
		/// <param name="auMgr">Autorisatiemanager voor Categorieen</param>
		public CategorieenManager(ICategorieenDao dao, IAutorisatieManager auMgr)
		{
			_dao = dao;
			_autorisatieMgr = auMgr;
		}

		#region Proxy naar DAO

		/// <summary>
		/// Een categorie ophalen op basis van zijn <paramref name="categorieID"/>, inclusief groep.
		/// </summary>
		/// <param name="categorieID">ID van op te halen categorie</param>
		/// <returns>opgehaalde categorie, met gekoppelde groep</returns>
		public Categorie Ophalen(int categorieID)
		{
			if (_autorisatieMgr.IsGavCategorie(categorieID))
			{
				return _dao.Ophalen(categorieID);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGroep);
			}
		}

		/// <summary>
		/// Persisteert de <paramref name="categorie"/> in de database.
		/// </summary>
		/// <param name="categorie">Te persisteren categorie</param>
		/// <returns>Gepersisteerde categorie (met correct ID)</returns>
		public Categorie Bewaren(Categorie categorie)
		{
			if (_autorisatieMgr.IsGavCategorie(categorie.ID))
			{
				return _dao.Bewaren(categorie);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavCategorie);
			}
		}

		/// <summary>
		/// Zoekt een categorie op op basis van <paramref name="groepID"/> en
		/// <paramref name="code"/>.
		/// </summary>
		/// <param name="groepID">ID van groep waaraan de te zoeken categorie gekoppeld moet zijn</param>
		/// <param name="code">code van de te zoeken categorie</param>
		/// <returns>de gevonden categorie; <c>null</c> indien niet gevonden</returns>
		public Categorie Ophalen(int groepID, string code)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				return _dao.Ophalen(groepID, code);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGroep);
			}
		}

		public Categorie BewarenMetPersonen(Categorie cat)
		{
			if (_autorisatieMgr.IsGavCategorie(cat.ID))
			{
				// TODO: (bug ) Eigenlijk zou de lambda-expressie hieronder
				// cgrie => cgrie.GelieerdePersoon.First().WithoutUpdate()
				// moeten zijn.  Maar met WithoutUpdate worden blijkbaar de
				// TeVerwijderen categorieen genegeerd.
				// Zie ticket #116.

				return _dao.Bewaren(cat, cgrie => cgrie.GelieerdePersoon.First());
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGroep);
			}
		}

		#endregion
	}
}
