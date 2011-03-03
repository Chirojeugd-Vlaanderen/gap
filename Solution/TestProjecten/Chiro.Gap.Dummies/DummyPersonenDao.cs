// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
	/// <summary>
	/// Dummy PersonenDao, die niets implementeert
	/// </summary>
	public class DummyPersonenDao : DummyDao<Persoon>, IPersonenDao
	{
		#region IPersonenDao Members

		public IList<Persoon> LijstOphalen(IList<int> personenIDs)
		{
			throw new NotImplementedException();
		}

		public IList<Persoon> HuisGenotenOphalen(int gelieerdePersoonID)
		{
			throw new NotImplementedException();
		}

		public Persoon CorresponderendePersoonOphalen(int gelieerdePersoonID)
		{
			throw new NotImplementedException();
		}

		public IList<Persoon> Ophalen(IEnumerable<int> ids, bool metGelieerdePersonen, string login, params Expression<Func<Persoon, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Persoon> OphalenViaGelieerdePersoon(IEnumerable<int> gelieerdePersoonIDs, params Expression<Func<Persoon, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public void DubbelVerwijderen(int origineelID, int dubbelID)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<TweeInts> DubbelsZoekenOpBasisVanAd()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Persoon> ZoekenOpNaam(string naam, string voornaam)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Persoon> ZoekenOpAd(int adNummer)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
