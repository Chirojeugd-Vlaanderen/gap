// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Data.Objects;
using System.Linq;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Gegevenstoegangsobject voor GAV-info
	/// </summary>
	public class GavDao : Dao<Gav, ChiroGroepEntities>, IGavDao
	{
		/// <summary>
		/// Haalt GAV-object op voor een gegeven <paramref name="login"/>, inclusief via gebruikersrecht
		/// gekoppelde groepen
		/// </summary>
		/// <param name="login">Gebruikersnaam op te halen gav</param>
		/// <returns>GAV-object met gekoppelde gebruikersrechten en groepen</returns>
		public Gav Ophalen(string login)
		{
			using (var db = new ChiroGroepEntities())
			{
				db.Gav.MergeOption = MergeOption.NoTracking;

				return (from gav in db.Gav.Include("GebruikersRecht.Groep")
						where gav.Login == login
						select gav).FirstOrDefault();
			}
		}

        /// <summary>
        /// Kijkt of er al een GAV met gegven <paramref name="login"/> bestaat,
        /// en retourneert desgevallend diens ID.
        /// </summary>
        /// <param name="login">Login op te zoeken ID</param>
        /// <returns>ID van de gevonden GAV, of 0 indien niets gevonden</returns>
	    public int IdOphalen(string login)
	    {
            using (var db = new ChiroGroepEntities())
            {
                return (from gav in db.Gav
                        where gav.Login == login
                        select gav.ID).FirstOrDefault();
            }
	    }
	}
}
