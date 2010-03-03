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
	/// Data access object voor functies
	/// </summary>
	/// <remarks>Probeer met een functie ALTIJD ZIJN GROEP mee op te halen.  Want een functie met groep null,
	/// is een nationaal gedefinieerde functie.</remarks>
	public class FunctiesDao: Dao<Functie, ChiroGroepEntities>, IFunctiesDao
	{
		#region IFunctiesDao Members

		/// <summary>
		/// Standaard wordt de link met de Groep vastgelegd in ConnectedEntities
		/// </summary>
		public FunctiesDao()
		{
			connectedEntities = new System.Linq.Expressions.Expression<Func<Functie, object>>[]
            {
				fnc => fnc.Groep
            };
		}

		/// <summary>
		/// Haalt een gepredefinieerde functie op
		/// </summary>
		/// <param name="f">GepredefinieerdeFunctieType dat de op te halen functie bepaalt</param>
		/// <returns>De gevraagde gepredefinieerde functie</returns>
		public Functie Ophalen(GepredefinieerdeFunctieType f)
		{
			return Ophalen((int)f);
		}

		/// <summary>
		/// Bepaalt het aantal leden uit de groep bepaald door <paramref name="groepID"/> de functie
		/// hebben bepaad door <paramref name="functieID"/>
		/// </summary>
		/// <param name="groepID">ID van een groep</param>
		/// <param name="functieID">ID van een functie</param>
		/// <returns>antal leden uit de groep bepaald door <paramref name="groepID"/> de functie
		/// hebben bepaad door <paramref name="functieID"/></returns>
		public int AantalLeden(int groepID, int functieID)
		{
			using (var db = new ChiroGroepEntities())
			{
				return (from ld in db.Lid
					where ld.GelieerdePersoon.Groep.ID == groepID 
						&& ld.Functie.Any(fnc => fnc.ID == functieID)
					select ld).Count();

			}
		}

		#endregion
	}
}
