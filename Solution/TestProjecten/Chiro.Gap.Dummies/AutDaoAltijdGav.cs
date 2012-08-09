// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

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

		public GebruikersRecht RechtenMbtGelieerdePersoonGet(string login, int gelieerdePersoonID)
		{
			throw new NotImplementedException();
		}

	    public GebruikersRecht GebruikersRechtGelieerdePersoon(int gelieerdePersoonID)
	    {
	        return null;
	    }

	    public IEnumerable<GebruikersRecht> AllesOphalen(int groepID)
	    {
	        throw new NotImplementedException();
	    }

	    public int IdOphalen(int gavID, int groepID)
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

		public IEnumerable<Groep> MijnGroepenOphalen(string login)
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

		public IEnumerable<int> EnkelMijnLeden(IEnumerable<int> lidIDs, string login)
		{
			return lidIDs;
		}

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

		public bool IsGavPersoonsAdres(int persoonsAdresID, string login)
		{
			return true;
		}

		public bool IsGavUitstap(int uitstapID, string login)
		{
			return true;
		}

		public bool IsGavPlaats(int plaatsID, string login)
		{
			return true;
		}

	    public bool IsGavDeelnemer(int deelnemerID, string login)
	    {
	        return true;
	    }

	    public bool IsGavGebruikersRecht(int gebruikersRechtID, string login)
	    {
	        throw new NotImplementedException();
	    }

	    public IEnumerable<int> EnkelMijnAfdelingen(IEnumerable<int> afdelingIDs, string login)
	    {
	        throw new NotImplementedException();
	    }

	    #endregion
	}
}
