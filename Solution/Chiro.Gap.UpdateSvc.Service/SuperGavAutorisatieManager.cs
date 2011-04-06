using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Gap.Orm;
using Chiro.Gap.Workers;

namespace Chiro.Gap.UpdateSvc.Service
{
	/// <summary>
	/// Autorisatiemanager voor de synchronisatie kipadmin->gap, die
	/// eigenlijk alleen maar zegt: 'Ik ben supergav!'
	/// </summary>
	public class SuperGavAutorisatieManager: IAutorisatieManager
	{
		public IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs)
		{
			throw new NotImplementedException();
		}

		public IList<int> EnkelMijnPersonen(IEnumerable<int> personenIDs)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Groep> MijnGroepenOphalen()
		{
			throw new NotImplementedException();
		}

		public bool IsGavGelieerdePersoon(int gelieerdePersoonID)
		{
			throw new NotImplementedException();
		}

		public bool IsGavGroep(int groepID)
		{
			throw new NotImplementedException();
		}

		public bool IsGavGroepen(IEnumerable<int> groepIDs)
		{
			throw new NotImplementedException();
		}

		public bool IsGavGroepsWerkJaar(int groepsWerkJaarID)
		{
			throw new NotImplementedException();
		}

		public bool IsGavPersoon(int persoonID)
		{
			throw new NotImplementedException();
		}

		public bool IsGavAfdeling(int afdelingsID)
		{
			throw new NotImplementedException();
		}

		public bool IsGavAfdelingsJaar(int afdelingsJaarID)
		{
			throw new NotImplementedException();
		}

		public bool IsGavLid(int lidID)
		{
			throw new NotImplementedException();
		}

		public bool IsGavCategorie(int categorieID)
		{
			throw new NotImplementedException();
		}

		public bool IsGavCommVorm(int commvormID)
		{
			throw new NotImplementedException();
		}

		public bool IsGavFunctie(int functieID)
		{
			throw new NotImplementedException();
		}

		public bool IsGavPersoonsAdres(int persoonsAdresID)
		{
			throw new NotImplementedException();
		}

		public bool IsGavPersoonsAdressen(IEnumerable<int> persoonsAdresIDs)
		{
			throw new NotImplementedException();
		}

		public bool IsGavUitstap(int uitstapID)
		{
			throw new NotImplementedException();
		}

		public bool IsSuperGav()
		{
			return true;
		}

		public string GebruikersNaamGet()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<int> MijnGroepIDsOphalen()
		{
			throw new NotImplementedException();
		}
	}
}
