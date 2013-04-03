using System.Collections.Generic;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WorkerInterfaces;

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

		public string GebruikersNaamGet()
		{
			return "Paria";
		}

		public bool IsSuperGav()
		{
			return false;
		}

		public IEnumerable<Groep> MijnGroepenOphalen()
		{
			return new List<Groep>();
		}

	    public bool IsGav(Groep groep)
	    {
	        return false;
	    }

	    public bool IsGav(CommunicatieVorm communicatieVorm)
	    {
	        return false;
	    }

	    public bool IsGav(GroepsWerkJaar groepsWerkJaar)
	    {
	        return false;
	    }

	    public bool IsGav(GelieerdePersoon gelieerdePersoon)
	    {
	        return false;
	    }

	    public bool IsGav(Deelnemer gelieerdePersoon)
	    {
            return false;
	    }

	    public bool IsGav(Plaats gelieerdePersoon)
	    {
            return false;
	    }

	    public bool IsGav(Uitstap gelieerdePersoon)
	    {
            return false;
	    }

	    public bool IsGav(GebruikersRecht gelieerdePersoon)
	    {
            return false;
	    }

        public bool IsGav(Lid gelieerdePersoon)
        {
            return false;
        }

        public bool IsGav(Afdeling gelieerdePersoon)
        {
            return false;
        }

        public bool IsGav(Categorie gelieerdePersoon)
        {
            return false;
        }

	    public bool IsGav(IList<GelieerdePersoon> gelieerdePersonen)
	    {
	        return false;
	    }

	    public List<GelieerdePersoon> MijnGelieerdePersonen(IList<Persoon> personen)
	    {
	        return new List<GelieerdePersoon>();
	    }

	    public bool IsGav(IList<PersoonsAdres> persoonsAdressen)
	    {
	        return false;
	    }

	    public bool IsGav(IList<Persoon> personen)
	    {
	        return false;
	    }

	    public bool IsGav(List<Groep> groepen)
	    {
	        return false;
	    }

	    #endregion
	}
}