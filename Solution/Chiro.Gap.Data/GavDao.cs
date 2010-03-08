// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
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
			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.Gav.MergeOption = MergeOption.NoTracking;

				return (from gav in db.Gav.Include("GebruikersRecht.Groep")
						where gav.Login == login
						select gav).FirstOrDefault();
			}
		}
	}
}
