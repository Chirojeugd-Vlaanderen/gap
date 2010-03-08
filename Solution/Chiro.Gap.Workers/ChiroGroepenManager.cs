// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Worker die alle businesslogica i.v.m. Chirogroepen bevat
	/// </summary>
	public class ChiroGroepenManager
	{
		private IDao<ChiroGroep> _dao = null;

		/// <summary>
		/// Creëert een ChiroGroepenManager
		/// </summary>
		/// <param name="dao">Repository voor Chirogroepen</param>
		public ChiroGroepenManager(IDao<ChiroGroep> dao)
		{
			_dao = dao;
		}

		// Nog geen interessante functionaliteit
	}
}
