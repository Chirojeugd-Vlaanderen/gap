using System.Collections.Generic;

using Chiro.Gap.Workers;
using Chiro.Gap.Orm;

namespace Chiro.Gap.Dummies
{
	/// <summary>
	/// Autorisatiemanager die er steeds van uitgaat dat
	/// de gebruiker geen rechten heeft.
	/// (nuttig voor authorisatietests..)
	/// </summary>
	public class AutMgrNooitGav : IAutorisatieManager
	{
		#region IAutorisatieManager Members

		public IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs)
		{
			return new List<int>();
		}

		public IList<int> EnkelMijnPersonen(IEnumerable<int> personenIDs)
		{
			return new List<int>();
		}

		public IEnumerable<int> EnkelMijnLeden(IEnumerable<int> lidIDs)
		{
			return new List<int>();
		}

		public IEnumerable<Groep> GekoppeldeGroepenGet()
		{
			return new List<Groep>();
		}

		public bool IsGavGelieerdePersoon(int gelieerdePersoonID)
		{
			return false;
		}

		public bool IsGavGroep(int groepID)
		{
			return false;
		}

		public bool IsGavGroepsWerkJaar(int groepsWerkJaarID)
		{
			return false;
		}

		public bool IsGavPersoon(int persoonID)
		{
			return false;
		}

		public string GebruikersNaamGet()
		{
			return "Paria";
		}

		public bool IsGavAfdeling(int afdelingsID)
		{
			return false;
		}

		public bool IsGavLid(int lidID)
		{
			return false;
		}

		public bool IsGavCategorie(int categorieID)
		{
			return false;
		}

		public bool IsGavCommVorm(int commvormID)
		{
			return false;
		}

		public bool IsGavUitstap(int uitstapID)
		{
			return false;
		}

		public bool IsSuperGav()
		{
			return false;
		}

		public bool IsGavFunctie(int functieID)
		{
			return false;
		}

		public bool IsGavAfdelingsJaar(int afdelingsJaarID)
		{
			return false;
		}

		public bool IsGavPersoonsAdres(int persoonsAdresID)
		{
			return false;
		}


		public IEnumerable<Groep> MijnGroepenOphalen()
		{
			return new List<Groep>();
		}

		public bool IsGavGroepen(IEnumerable<int> groepIDs)
		{
			return false;
		}

		public IEnumerable<int> MijnGroepIDsOphalen()
		{
			return new List<int>();
		}

		public bool IsGavPlaats(int plaatsID)
		{
			return false;
		}

	    public bool IsGavDeelnemer(int deelnemerID)
	    {
	        return false;
	    }

	    public bool IsGavPersoonsAdressen(IEnumerable<int> persoonsAdresIDs)
		{
			return false;
		}

		#endregion
	}
}