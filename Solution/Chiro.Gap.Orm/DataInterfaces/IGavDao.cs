// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
	/// <summary>
	/// Interface voor een gegevenstoegangsobject voor GAV's
	/// </summary>
	public interface IGavDao : IDao<Gav>
	{
		/// <summary>
		/// Haalt GAV-object op op basis van login
		/// </summary>
		/// <param name="login">De gebruikersnaam van de bezoeker</param>
		/// <returns>GAV horende bij gegeven login</returns>
		Gav Ophalen(string login);

        /// <summary>
        /// Kijkt of er al een GAV met gegven <paramref name="login"/> bestaat,
        /// en retourneert desgevallend diens ID.
        /// </summary>
        /// <param name="login">login op te zoeken ID</param>
        /// <returns>ID van de gevonden GAV, of 0 indien niets gevonden</returns>
	    int IdOphalen(string login);
	}
}
