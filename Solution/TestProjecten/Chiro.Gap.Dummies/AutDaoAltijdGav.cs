using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
	/// <summary>
	/// Deze klasse kan gebruikt worden als IAuthorisatieDao om te testen.
	/// Ze geeft altijd true op IsGav-vragen.
	/// </summary>
	public class AutDaoAltijdGav : DummyDao<GebruikersRecht>, IAutorisatieDao
	{
		#region IAuthorisatieDao Members

		public GebruikersRecht RechtenMbtGroepGet(string login, int groepID)
		{
			throw new NotImplementedException();
		}

		public GebruikersRecht RechtenMbtGelieerdePersoonGet(string login, int GelieerdePersoonID)
		{
			throw new NotImplementedException();
		}

		public bool IsGavGroep(string login, int groepID)
		{
			return true;
		}

		/// <summary>
		/// Positieve ID: je bent GAV (want je bent altijd GAV).  Bij een negatieve ID, weten
		/// we dat het ID ongeldig is.  Dan is het resultaat altijd false.
		/// </summary>
		/// <param name="login"></param>
		/// <param name="gelieerdePersoonID"></param>
		/// <returns></returns>
		public bool IsGavGelieerdePersoon(string login, int gelieerdePersoonID)
		{
			return (gelieerdePersoonID >= 0);
		}

		public bool IsGavPersoon(string login, int persoonID)
		{
			throw new NotImplementedException();
		}

		public bool IsGavGroepsWerkJaar(string login, int groepsWerkJaarID)
		{
			return true;
		}

		public IEnumerable<Groep> GekoppeldeGroepenGet(string login)
		{
			throw new NotImplementedException();
		}

		public IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs, string login)
		{
			return gelieerdePersonenIDs.ToList();
		}

		public IList<int> EnkelMijnPersonen(IEnumerable<int> personenIDs, string p)
		{
			throw new NotImplementedException();
		}

		#endregion

		public bool IsGavAfdeling(string login, int afdelingsID)
		{
			return true;
		}

		public bool IsGavLid(string login, int lidID)
		{
			return true;
		}

		public bool IsGavCategorie(int categorieID, string login)
		{
			return true;
		}

		public bool IsGavCommVorm(int commvormID, string login)
		{
			return true;
		}

		public bool IsGavAfdelingsJaar(string login, int afdelingsJaarID)
		{
			return true;
		}
	}
}
