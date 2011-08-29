// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
	/// <summary>
	/// Fake LedenDao voor tests.  Doet niets.
	/// </summary>
	public class DummyLedenDao : DummyDao<Lid>, ILedenDao
	{
		#region ILedenDao Members

		public IList<Lid> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID, LidEigenschap sortering)
		{
			return new List<Lid>();
		}

		public IList<Lid> PaginaOphalenVolgensFunctie(int groepsWerkJaarID, int functieID, LidEigenschap sortering)
		{
			return new List<Lid>();
		}

		public Lid Ophalen(int gelieerdePersoonID, int groepsWerkJaarID)
		{
			return null;
		}

		public IEnumerable<Lid> Ophalen(IEnumerable<int> lidIDs, LidExtras extras)
		{
			throw new NotImplementedException();
		}

		public Lid Ophalen(int lidID, LidExtras extras)
		{
			throw new NotImplementedException();
		}

		public Lid Ophalen(int gelieerdePersoonID, int groepsWerkJaarID, params Expression<Func<Lid, object>>[] paths)
		{
			return null;
		}

		public Lid OphalenMetDetails(int lidID)
		{
			return null;
		}

		public IList<Lid> OphalenUitFunctie(int functieID, int groepsWerkJaarID, params Expression<Func<Lid, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<Lid> OphalenUitFunctie(NationaleFunctie f, int groepsWerkJaarID, params Expression<Func<Lid, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public bool IsLeiding(int lidID)
		{
			throw new NotImplementedException();
		}

		public Lid OphalenViaPersoon(int gelieerdePersoonID, int groepsWerkJaarID)
		{
			throw new NotImplementedException();
		}

	    public IEnumerable<Lid> OphalenUitGroepsWerkJaar(int gwjID, bool ookInactief)
	    {
	        throw new NotImplementedException();
	    }

		#endregion
	}
}
