// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WorkerInterfaces;

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

		public IEnumerable<int> EnkelMijnLeden(IEnumerable<int> lidIDs)
		{
			return lidIDs;
		}

	    public IEnumerable<int> EnkelMijnAfdelingen(IEnumerable<int> afdelingIDs)
	    {
	        return afdelingIDs;
	    }

	    public IEnumerable<Groep> MijnGroepenOphalen()
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

		public bool IsGavUitstap(int uitstapID)
		{
			return true;
		}

		public bool IsSuperGav()
		{
			return true;
		}

		public bool IsGavFunctie(int functieID)
		{
			return true;
		}

		public bool IsGavAfdelingsJaar(int afdelingsJaarID)
		{
			return true;
		}

		public bool IsGavPersoonsAdres(int persoonsAdresID)
		{
			return true;
		}


		public bool IsGavGroepen(IEnumerable<int> groepIDs)
		{
			return true;
		}

		public IEnumerable<int> MijnGroepIDsOphalen()
		{
			throw new NotImplementedException();
		}

		public bool IsGavPlaats(int plaatsID)
		{
			return true;
		}

	    public bool IsGavDeelnemer(int deelnemerID)
	    {
	        return true;
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
	        return true;
	    }

	    public bool IsGav(CommunicatieVorm communicatieVorm)
	    {
	        return true;
	    }

	    public bool IsGavPersoonsAdressen(IEnumerable<int> persoonsAdresIDs)
		{
			return true;
		}

		#endregion
	}
}
