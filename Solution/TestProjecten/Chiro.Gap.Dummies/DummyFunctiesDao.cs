using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
	public class DummyFunctiesDao: DummyDao<Functie>, IFunctiesDao
	{
		#region IFunctiesDao Members

		public Functie Ophalen(GepredefinieerdeFunctieType f)
		{
			throw new NotImplementedException();
		}

		public int AantalLeden(int groepID, int functieID)
		{
			throw new NotImplementedException();
		}

		public int AantalLeden(int groepID, GepredefinieerdeFunctieType f)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Voor het DummyFunctiesDao zijn er 2 nationaal bepaalde functies:
		/// contactpersoon en financieel verantwoordelijke
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Chiro.Gap.Orm.Functie> NationaalBepaaldeFunctiesOphalen()
		{
			return new Functie[]
			{
				new Functie { 
					ID = (int)GepredefinieerdeFunctieType.ContactPersoon,
					Code = "GG1",
					MaxAantal = 1,
					MinAantal = 1},
				new Functie {
					ID = (int)GepredefinieerdeFunctieType.FinancieelVerantwoordelijke,
					Code = "FI",
					MaxAantal = 1,
					MinAantal = 1}
			};
		}

		#endregion

	}
}
