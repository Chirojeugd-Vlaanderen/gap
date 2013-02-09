using System;
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

		public IEnumerable<int> EnkelMijnLeden(IEnumerable<int> lidIDs)
		{
			return new List<int>();
		}

	    public IEnumerable<int> EnkelMijnAfdelingen(IEnumerable<int> afdelingIDs)
	    {
	        throw new NotImplementedException();
	    }

	    public IEnumerable<Groep> GekoppeldeGroepenGet()
		{
			return new List<Groep>();
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

		public IEnumerable<int> MijnGroepIDsOphalen()
		{
			return new List<int>();
		}
        
	    public GebruikersRecht GebruikersRechtToekennen(Gav gav, Groep groep, DateTime vervalDatum)
	    {
	        throw new NotImplementedException();
	    }

	    public GebruikersRecht GebruikersRechtToekennen(string login, int groepID, DateTime vervalDatum)
	    {
	        throw new NotImplementedException();
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

		#endregion
	}
}