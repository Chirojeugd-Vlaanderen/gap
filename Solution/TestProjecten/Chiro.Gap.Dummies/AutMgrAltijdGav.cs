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

		public string GebruikersNaamGet()
		{
			throw new NotImplementedException();
		}

		public bool IsSuperGav()
		{
			return true;
		}

		public IEnumerable<int> MijnGroepIDsOphalen()
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

	    public bool IsGav(Groep groep)
	    {
	        return true;
	    }

	    public bool IsGav(CommunicatieVorm communicatieVorm)
	    {
	        return true;
	    }

	    public bool IsGav(GroepsWerkJaar groepsWerkJaar)
	    {
	        return true;
	    }

	    public bool IsGav(GelieerdePersoon gelieerdePersoon)
	    {
	        return true;
	    }

	    public bool IsGav(Deelnemer gelieerdePersoon)
	    {
	        return true;
	    }

	    public bool IsGav(Plaats gelieerdePersoon)
	    {
            return true;
	    }

	    public bool IsGav(Uitstap gelieerdePersoon)
	    {
            return true;
	    }

	    public bool IsGav(GebruikersRecht gelieerdePersoon)
	    {
            return true;
	    }

        public bool IsGav(Lid gelieerdePersoon)
        {
            return true;
        }

        public bool IsGav(Afdeling gelieerdePersoon)
        {
            return true;
        }

        public bool IsGav(Categorie gelieerdePersoon)
        {
            return true;
        }

		#endregion
	}
}
