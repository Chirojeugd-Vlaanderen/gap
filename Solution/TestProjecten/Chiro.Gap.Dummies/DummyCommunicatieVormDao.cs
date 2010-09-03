using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
	public class DummyCommunicatieVormDao: IDao<CommunicatieVorm>, ICommunicatieVormDao
	{
		public CommunicatieVorm Ophalen(int id)
		{
			throw new NotImplementedException();
		}

		public CommunicatieVorm Ophalen(int id, params Expression<Func<CommunicatieVorm, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<CommunicatieVorm> Ophalen(IEnumerable<int> ids)
		{
			throw new NotImplementedException();
		}

		public IList<CommunicatieVorm> Ophalen(IEnumerable<int> ids, params Expression<Func<CommunicatieVorm, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<CommunicatieVorm> PaginaOphalen(int id, Expression<Func<CommunicatieVorm, int>> f, int pagina, int paginaGrootte, out int aantalTotaal, params Expression<Func<CommunicatieVorm, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<CommunicatieVorm> AllesOphalen()
		{
			throw new NotImplementedException();
		}

		public CommunicatieVorm Bewaren(CommunicatieVorm entiteit)
		{
			throw new NotImplementedException();
		}

		public CommunicatieVorm Bewaren(CommunicatieVorm entiteit, params Expression<Func<CommunicatieVorm, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<CommunicatieVorm> Bewaren(IEnumerable<CommunicatieVorm> es, params Expression<Func<CommunicatieVorm, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public Expression<Func<CommunicatieVorm, object>>[] GetConnectedEntities()
		{
			throw new NotImplementedException();
		}

		public IList<CommunicatieVorm> ZoekenOpNummer(string zoekString)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<CommunicatieVorm> ZoekenOpPersoon(int persoonID)
		{
			throw new NotImplementedException();
		}
	}
}
