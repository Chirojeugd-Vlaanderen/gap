// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Gegevenstoegangsobject voor functies
	/// </summary>
	/// <remarks>Probeer met een functie ALTIJD ZIJN GROEP mee op te halen.  Want een functie met groep null,
	/// is een nationaal gedefinieerde functie.</remarks>
	public class FunctiesDao : Dao<Functie, ChiroGroepEntities>, IFunctiesDao
	{
		#region IFunctiesDao Members

		/// <summary>
		/// Instantieert een gegevenstoegangsobject voor functies
		/// </summary>
		/// <remarks>
		/// Standaard wordt de link met de Groep vastgelegd in ConnectedEntities
		/// </remarks>
		public FunctiesDao()
		{
			ConnectedEntities = new System.Linq.Expressions.Expression<Func<Functie, object>>[]
			{
				fnc => fnc.Groep
			};
		}

		/// <summary>
		/// Haalt een nationaal bepaalde functie op
		/// </summary>
		/// <param name="f">Type nationale functie dat de op te halen functie bepaalt</param>
		/// <returns>De gevraagde nationaal bepaalde functie</returns>
		public Functie Ophalen(NationaleFunctie f)
		{
			return Ophalen((int)f);
		}

		/// <summary>
		/// Bepaalt het aantal leden uit de groep bepaald door <paramref name="groepID"/> de functie
		/// hebben bepaad door <paramref name="functieID"/>
		/// </summary>
		/// <param name="groepID">ID van een groep</param>
		/// <param name="functieID">ID van een functie</param>
		/// <returns>Aantal leden uit de groep bepaald door <paramref name="groepID"/> de functie
		/// hebben bepaald door <paramref name="functieID"/></returns>
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

		/// <summary>
		/// Bepaalt het aantal leden uit de groep met ID <paramref name="groepID"/> met 
		/// nationaal bepaalde functie <paramref name="f"/>.
		/// </summary>
		/// <param name="groepID">ID van een groep</param>
		/// <param name="f">Nationaal bepaalde functie</param>
		/// <returns>Aantal leden uit de groep met ID <paramref name="groepID"/> met 
		/// nationaal bepaalde functie <paramref name="f"/></returns>
		public int AantalLeden(int groepID, NationaleFunctie f)
		{
			return AantalLeden(groepID, (int)f);
		}

		/// <summary>
		/// Haalt de nationaal bepaalde functies op
		/// </summary>
		/// <returns>De rij met nationaal bepaalde functies</returns>
		public IEnumerable<Functie> NationaalBepaaldeFunctiesOphalen()
		{
			IEnumerable<Functie> resultaat;

			using (var db = new ChiroGroepEntities())
			{
				resultaat = (from f in db.Functie
							 where f.IsNationaal
							 select f).ToList();
			}
			return Utility.DetachObjectGraph(resultaat);
		}

		/// <summary>
		/// Haalt de functies op die de groep zelf aangemaakt heeft
		/// </summary>
		/// <param name="groepID">De ID van de groep</param>
		/// <returns>De rij met eigen functies van de groep met de opgegeven ID</returns>
		public IEnumerable<Functie> FunctiesPerGroepOphalen(int groepID)
		{
			IEnumerable<Functie> resultaat;

			using (var db = new ChiroGroepEntities())
			{
				resultaat = (from f in db.Functie
							 where f.Groep.ID == groepID
							 select f).ToList();
			}
			return Utility.DetachObjectGraph(resultaat);
		}

		#endregion
	}
}
