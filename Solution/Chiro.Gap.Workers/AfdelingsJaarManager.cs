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
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Fouten.Exceptions;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Worker die alle businesslogica i.v.m. afdelingsjaren bevat
	/// </summary>
	public class AfdelingsJaarManager
	{
		private IAfdelingsJarenDao _dao;
		private IAutorisatieManager _autorisatieMgr;

		/// <summary>
		/// Deze constructor laat toe om een alternatieve repository voor
		/// de groepen te gebruiken.  Nuttig voor mocking en testing.
		/// </summary>
		/// <param name="dao">Alternatieve dao</param>
		/// <param name="autorisatieMgr">Alternatieve autorisatiemanager</param>
		public AfdelingsJaarManager(IAfdelingsJarenDao dao, IAutorisatieManager autorisatieMgr)
		{
			_dao = dao;
			_autorisatieMgr = autorisatieMgr;
		}

		/// <summary>
		/// Op basis van een ID een afdelingsjaar ophalen
		/// </summary>
		/// <param name="afdelingsJaarID">De ID van het afdelingsjaar</param>
		/// <returns>Het afdelingsjaar met de opgegeven ID</returns>
		public AfdelingsJaar Ophalen(int afdelingsJaarID)
		{
			AfdelingsJaar aj = _dao.Ophalen(afdelingsJaarID, a => a.Afdeling, a => a.Leiding, a => a.Kind, a => a.OfficieleAfdeling);

			if (_autorisatieMgr.IsGavAfdeling(aj.Afdeling.ID))
			{
				return aj;
			}
			else
			{
				throw new GeenGavException(Resources.GeenGavGroep);
			}
		}

		/// <summary>
		/// Verwijdert AfdelingsJaar uit database
		/// </summary>
		/// <param name="afdelingsJaarID">ID van het AfdelingsJaar</param>
		/// <returns><c>True</c> on successful</returns>
		public bool Verwijderen(int afdelingsJaarID)
		{
			AfdelingsJaar aj = _dao.Ophalen(afdelingsJaarID, a => a.Afdeling, a => a.Leiding, a => a.Kind);

			if (_autorisatieMgr.IsGavAfdeling(aj.Afdeling.ID))
			{
				if (aj.Kind.Count != 0 || aj.Leiding.Count != 0)
				{
					throw new InvalidOperationException("AfdelingsJaar kan niet verwijderd worden omdat er nog leden of leiding in deze afdeling zitten.");
				}
				else
				{
					aj.TeVerwijderen = true;
					_dao.Bewaren(aj);
					return true;
				}
			}
			else
			{
				throw new GeenGavException(Resources.GeenGavGroep);
			}
		}

		/// <summary>
		/// Het opgegeven afdelingsjaar opslaan
		/// </summary>
		/// <param name="aj">Het afdelingsjaar dat opgeslagen moet worden</param>
		public void Bewaren(AfdelingsJaar aj)
		{
			if (_autorisatieMgr.IsGavAfdeling(aj.Afdeling.ID))
			{
				_dao.Bewaren(aj);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavLid);
			}
		}
	}
}
