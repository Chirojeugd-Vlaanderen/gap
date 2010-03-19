// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Gegevenstoegangsobject voor subgemeenten
	/// </summary>
	public class SubgemeenteDao : Dao<WoonPlaats, ChiroGroepEntities>, ISubgemeenteDao
	{
		/// <summary>
		/// Haalt de subgemeente op op basis van een postnummer en een naam
		/// </summary>
		/// <param name="naam">De naam van de (sub)gemeente</param>
		/// <param name="postNr">Het postnummer van de (sub)gemeente</param>
		/// <returns>De subgemeente die aan de criteria voldoet</returns>
		public WoonPlaats Ophalen(string naam, int postNr)
		{
			WoonPlaats resultaat = null;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				resultaat = (
					from WoonPlaats s in db.WoonPlaats
					where s.Naam == naam && s.PostNummer == postNr
					select s).FirstOrDefault<WoonPlaats>();

				if (resultaat != null)
				{
					db.Detach(resultaat);
				}
			}

			return resultaat;
		}
	}
}
