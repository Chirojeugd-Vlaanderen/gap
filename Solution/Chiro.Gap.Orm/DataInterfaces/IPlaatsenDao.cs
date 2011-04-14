using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
	/// <summary>
	/// Interface voor data access wat betreft bivakplaatsen.
	/// </summary>
	public interface IPlaatsenDao:IDao<Plaats>
	{
		/// <summary>
		/// Zoekt naar een plaats, op basis van <paramref name="groepID"/>, <paramref name="plaatsNaam"/>
		/// en <paramref name="adresID"/>
		/// </summary>
		/// <param name="groepID">ID van groep die de plaats gemaakt moet hebben</param>
		/// <param name="plaatsNaam">naam van de plaats</param>
		/// <param name="adresID">ID van het adres van de plaats</param>
		/// <param name="paths">Bepaalt wat er allemaal mee opgehaald moet worden</param>
		/// <returns>De gevonden groep, of <c>null</c> als niets werd gevonden</returns>
		Plaats Zoeken(int groepID, string plaatsNaam, int adresID, params Expression<Func<Plaats, object>>[] paths);
	}
}
