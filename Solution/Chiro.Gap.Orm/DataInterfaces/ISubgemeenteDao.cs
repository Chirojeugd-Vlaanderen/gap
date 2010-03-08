using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
	/// <summary>
	/// Data Access Object voor subgemeente
	/// </summary>
	public interface ISubgemeenteDao : IDao<Subgemeente>
	{
		/// <summary>
		/// Haalt subgemeente op op basis van naam en postnummer
		/// </summary>
		/// <param name="naam">naam van de subgemeente</param>
		/// <param name="postNr">postnummer van de subgemeente</param>
		/// <returns>relevante subgemeene</returns>
		Subgemeente Ophalen(string naam, int postNr);
	}
}
