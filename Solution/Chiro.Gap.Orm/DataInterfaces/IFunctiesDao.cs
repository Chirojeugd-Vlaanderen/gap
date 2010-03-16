// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
	/// <summary>
	/// Interface voor data access object voor functies
	/// </summary>
	/// <remarks>Probeer met een functie ALTIJD ZIJN GROEP mee op te halen.  Want een functie met groep null,
	/// is een nationaal gedefinieerde functie.</remarks>
	public interface IFunctiesDao : IDao<Functie>
	{
		/// <summary>
		/// Haalt een gepredefinieerde functie op
		/// </summary>
		/// <param name="f">GepredefinieerdeFunctieType dat de op te halen functie bepaalt</param>
		/// <returns>De gevraagde gepredefinieerde functie</returns>
		Functie Ophalen(GepredefinieerdeFunctieType f);

		/// <summary>
		/// Bepaalt het aantal leden uit de groep bepaald door <paramref name="groepID"/> de functie
		/// hebben bepaad door <paramref name="functieID"/>
		/// </summary>
		/// <param name="groepID">ID van een groep</param>
		/// <param name="functieID">ID van een functie</param>
		/// <returns>Aantal leden uit de groep bepaald door <paramref name="groepID"/> de functie
		/// hebben bepaad door <paramref name="functieID"/></returns>
		int AantalLeden(int groepID, int functieID);

		/// <summary>
		/// Bepaalt het aantal leden uit de groep met ID <paramref name="groepID"/> met 
		/// gepredefinieerde functie <paramref name="f"/>.
		/// </summary>
		/// <param name="groepID">ID van een groep</param>
		/// <param name="f">Gepredefinieerde functie</param>
		/// <returns>Aantal leden uit de groep met ID <paramref name="groepID"/> met 
		/// gepredefinieerde functie <paramref name="f"/></returns>
		int AantalLeden(int groepID, GepredefinieerdeFunctieType f);

		/// <summary>
		/// Haalt de nationaal bepaalde functies op
		/// </summary>
		/// <returns>De rij met nationaal bepaalde functies</returns>
		IEnumerable<Functie> NationaalBepaaldeFunctiesOphalen();
	}
}
