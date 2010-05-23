using System;
using System.Collections.Generic;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
	public class DummyFunctiesDao : DummyDao<Functie>, IFunctiesDao
	{
		#region IFunctiesDao Members

		public Functie Ophalen(NationaleFunctie f)
		{
			throw new NotImplementedException();
		}

		public int AantalLeden(int groepID, int functieID)
		{
			throw new NotImplementedException();
		}

		public int AantalLeden(int groepID, NationaleFunctie f)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Voor het DummyFunctiesDao zijn er 2 nationaal bepaalde functies:
		/// contactpersoon en financieel verantwoordelijke
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Functie> NationaalBepaaldeFunctiesOphalen()
		{
			return new Functie[]
			{
				new Functie { 
					ID = (int)NationaleFunctie.ContactPersoon,
					Code = "GG1",
					MaxAantal = 1,
					MinAantal = 1,
					IsNationaal = true},
				new Functie {
					ID = (int)NationaleFunctie.FinancieelVerantwoordelijke,
					Code = "FI",
					MaxAantal = 1,
					MinAantal = 1,
					IsNationaal = true}
			};
		}

		public IEnumerable<Functie> FunctiesPerGroepOphalen(int groepID)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
