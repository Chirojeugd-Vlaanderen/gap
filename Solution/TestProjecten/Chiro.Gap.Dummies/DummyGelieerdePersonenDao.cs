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

		public IList<GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, PersoonSorteringsEnum sortering,  out int aantalTotaal)
		{
			throw new NotImplementedException();
		}

		public IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, PersoonSorteringsEnum sortering, out int aantalTotaal)
		{
			throw new NotImplementedException();
		}

		public IList<GelieerdePersoon> PaginaOphalenUitCategorie(int categorieID, int pagina, int paginaGrootte, PersoonSorteringsEnum sortering, bool metHuidigLidInfo, out int aantalTotaal, params Expression<Func<GelieerdePersoon, object>>[] paths)
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

		public IList<GelieerdePersoon> ZoekenOpNaamOngeveer(int groepID, string naam, string voornaam, params System.Linq.Expressions.Expression<Func<GelieerdePersoon, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<GelieerdePersoon> AllenOphalen(int GroepID, PersoonSorteringsEnum sortering, params System.Linq.Expressions.Expression<Func<GelieerdePersoon, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<GelieerdePersoon> HuisGenotenOphalenZelfdeGroep(int gelieerdePersoonID)
		{
			throw new NotImplementedException();
		}

		public IList<GelieerdePersoon> PaginaOphalenUitCategorie(int categorieID, int pagina, int paginaGrootte, bool metHuidigLidInfo, out int aantalTotaal, params System.Linq.Expressions.Expression<Func<GelieerdePersoon, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
