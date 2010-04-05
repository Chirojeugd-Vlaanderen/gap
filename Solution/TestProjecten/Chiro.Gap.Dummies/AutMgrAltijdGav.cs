using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Workers;

namespace Chiro.Gap.Dummies
{
	/// <summary>
	/// Autorisatiemanager die altijd alle rechten toekent
	/// (nuttig voor testen van niet-autorisatiegebonden 
	/// business logica.)
	/// </summary>
	public class AutMgrAltijdGav : IAutorisatieManager
	{
		#region IAutorisatieManager Members

		public IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs)
		{
			return gelieerdePersonenIDs.ToList();
		}

		public IList<int> EnkelMijnPersonen(IEnumerable<int> personenIDs)
		{
			return personenIDs.ToList();
		}

		public IEnumerable<Chiro.Gap.Orm.Groep> GekoppeldeGroepenGet()
		{
			throw new NotImplementedException();
		}

		public bool IsGavGelieerdePersoon(int gelieerdePersoonID)
		{
			return true;
		}

		public bool IsGavGroep(int groepID)
		{
			return true;
		}

		public bool IsGavGroepsWerkJaar(int groepsWerkJaarID)
		{
			return true;
		}

		public bool IsGavPersoon(int persoonID)
		{
			return true;
		}

		public string GebruikersNaamGet()
		{
			throw new NotImplementedException();
		}

		public bool IsGavAfdeling(int afdelingsID)
		{
			return true;
		}

		public bool IsGavLid(int lidID)
		{
			return true;
		}

		public bool IsGavCategorie(int categorieID)
		{
			return true;
		}

		public bool IsGavCommVorm(int commvormID)
		{
			return true;
		}

		public bool IsSuperGavGroep(int groepID)
		{
			return true;
		}

		public bool IsGavFunctie(int functieID)
		{
			return true;
		}

		#endregion
	}
}
