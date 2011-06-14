// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Dummies
{
	public class DummyAfdelingenDao : IAfdelingenDao
	{
		#region IDao<Afdeling> Members

		public Afdeling Ophalen(int id)
		{
			throw new NotImplementedException();
		}

		public Afdeling Ophalen(int id, params Expression<Func<Afdeling, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<Afdeling> Ophalen(IEnumerable<int> ids)
		{
			throw new NotImplementedException();
		}

		public IList<Afdeling> Ophalen(IEnumerable<int> ids, params Expression<Func<Afdeling, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<Afdeling> PaginaOphalen(int id, Expression<Func<Afdeling, int>> f, int pagina, int paginaGrootte, out int aantalTotaal, params Expression<Func<Afdeling, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<Afdeling> AllesOphalen()
		{
			throw new NotImplementedException();
		}

		public Afdeling Bewaren(Afdeling entiteit)
		{
			throw new NotImplementedException();
		}

		public Afdeling Bewaren(Afdeling entiteit, params Expression<Func<Afdeling, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Afdeling> Bewaren(IEnumerable<Afdeling> es, params Expression<Func<Afdeling, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public Expression<Func<Afdeling, object>>[] GetConnectedEntities()
		{
			throw new NotImplementedException();
		}

		#endregion


		#region IAfdelingenDao Members

		public IList<Afdeling> OngebruikteOphalen(int groepsWerkJaarID)
		{
			throw new NotImplementedException();
		}

		public IList<OfficieleAfdeling> OfficieleAfdelingenOphalen()
		{
			throw new NotImplementedException();
		}

		public OfficieleAfdeling OfficieleAfdelingOphalen(int officieleAfdelingID)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
