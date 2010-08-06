// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using Chiro.Cdf.Data;
using Chiro.Gap.Orm;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Worker die alle businesslogica i.v.m. Chirogroepen bevat
	/// </summary>
	public class ChiroGroepenManager
	{
		private IDao<ChiroGroep> _dao;

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
