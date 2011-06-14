// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
	/// <summary>
	/// Interface voor een gegevenstoegangsobject voor Subgemeenten
	/// </summary>
	public interface ISubgemeenteDao : IDao<WoonPlaats>
	{
		/// <summary>
		/// Haalt subgemeente op op basis van naam en postnummer
		/// </summary>
		/// <param name="naam">Naam van de subgemeente</param>
		/// <param name="postNr">Postnummer van de subgemeente</param>
		/// <returns>Relevante subgemeente</returns>
		WoonPlaats Ophalen(string naam, int postNr);
	}
}
