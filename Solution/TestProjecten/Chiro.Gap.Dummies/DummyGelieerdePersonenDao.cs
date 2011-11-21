// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Dummies
{
	/// <summary>
	/// Dummy GelieerdePersonenDao, die niets implementeert
	/// </summary>
	public class DummyGelieerdePersonenDao : DummyDao<GelieerdePersoon>, IGelieerdePersonenDao
	{
		#region IGelieerdePersonenDao Members

		public IList<GelieerdePersoon> ZoekenOpNaam(int groepID, string zoekStringNaam)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<GelieerdePersoon> ZoekenOpNaam(int groepID, string naam, string voornaam)
		{
			throw new NotImplementedException();
		}

		public IList<GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, PersoonSorteringsEnum sortering,  out int aantalTotaal)
		{
			throw new NotImplementedException();
		}

		public GelieerdePersoon Ophalen(int persoonID, int groepID, bool metVoorkeurAdres, params Expression<Func<GelieerdePersoon, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public GelieerdePersoon Ophalen(int persoonID, int groepID, params Expression<Func<GelieerdePersoon, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<GelieerdePersoon> Ophalen(IList<int> gelieerdePersoonIDs, PersoonsExtras extras)
		{
			throw new NotImplementedException();
		}

		public GelieerdePersoon Ophalen(int gelieerdePersoonID, PersoonsExtras extras)
		{
			throw new NotImplementedException();
		}

		public IList<GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, PersoonSorteringsEnum sortering, PersoonsExtras extras, out int aantalTotaal)
		{
			throw new NotImplementedException();
		}

		public IList<GelieerdePersoon> PaginaOphalenUitCategorie(int categorieID, int pagina, int paginaGrootte, PersoonSorteringsEnum sortering, out int aantalTotaal, PersoonsExtras extras)
		{
			throw new NotImplementedException();
		}

		public GelieerdePersoon DetailsOphalen(int gelieerdePersoonID)
		{
			throw new NotImplementedException();
		}

		public GelieerdePersoon GroepLaden(GelieerdePersoon p)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<CommunicatieType> CommunicatieTypesOphalen()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<CommunicatieType> OphalenCommunicatieTypes()
		{
			throw new NotImplementedException();
		}

		public IList<GelieerdePersoon> ZoekenOpNaamOngeveer(int groepID, string naam, string voornaam)
		{
			throw new NotImplementedException();
		}

		public IList<GelieerdePersoon> ZoekenOpNaamOngeveer(int groepID, string naam, string voornaam, params Expression<Func<GelieerdePersoon, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<GelieerdePersoon> AllenOphalen(int GroepID, PersoonSorteringsEnum sortering, PersoonsExtras extras)
		{
			throw new NotImplementedException();
		}

		public IList<GelieerdePersoon> HuisGenotenOphalenZelfdeGroep(int gelieerdePersoonID)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<GelieerdePersoon> DubbelPuntZonderAdOphalen()
		{
			throw new NotImplementedException();
		}

		public GelieerdePersoon Bewaren(GelieerdePersoon gelieerdePersoon, PersoonsExtras extras)
		{
			throw new NotImplementedException();
		}

	    public IEnumerable<GelieerdePersoon> OphalenOpBasisVanGavs(int groepID)
	    {
	        throw new NotImplementedException();
	    }

	    public IList<GelieerdePersoon> PaginaOphalenUitCategorie(int categorieID, int pagina, int paginaGrootte, bool metHuidigLidInfo, out int aantalTotaal, params Expression<Func<GelieerdePersoon, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
