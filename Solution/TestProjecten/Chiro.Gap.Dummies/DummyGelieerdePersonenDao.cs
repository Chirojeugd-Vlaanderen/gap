using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

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

		public IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
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

		public IList<GelieerdePersoon> ZoekenOpNaamOngeveer(int groepID, string naam, string voornaam)
		{
			throw new NotImplementedException();
		}

		public IList<GelieerdePersoon> ZoekenOpNaamOngeveer(int groepID, string naam, string voornaam, params System.Linq.Expressions.Expression<Func<GelieerdePersoon, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<GelieerdePersoon> AllenOphalen(int GroepID, params System.Linq.Expressions.Expression<Func<GelieerdePersoon, object>>[] paths)
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
