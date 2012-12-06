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

	    public bool IsGavGebruikersRecht(int gebruikersRechtID)
	    {
	        throw new NotImplementedException();
	    }

	    public GebruikersRecht GebruikersRechtToekennen(Gav gav, Groep groep, DateTime vervalDatum)
	    {
	        throw new NotImplementedException();
	    }

	    public GebruikersRecht GebruikersRechtToekennen(string login, int groepID, DateTime vervalDatum)
	    {
	        throw new NotImplementedException();
	    }

	    public bool IsGavAccount(int accountID)
	    {
	        throw new NotImplementedException();
	    }

	    public bool IsGavGebruikersRechten(int[] gebruikersRechtIDs)
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

	    public bool IsGavPersoonsAdressen(IEnumerable<int> persoonsAdresIDs)
		{
			return false;
		}

		#endregion
	}
}