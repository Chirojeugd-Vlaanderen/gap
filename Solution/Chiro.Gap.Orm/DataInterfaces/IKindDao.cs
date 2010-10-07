// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
	/// <summary>
	/// Interface voor een gegevenstoegangsobject voor Kinderen
	/// </summary>
	public interface IKindDao : IDao<Kind>
	{
		/// <summary>
		/// Haalt alle kinderen op uit een gegeven groepswerkjaar
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <param name="paths">Geeft aan welke entiteiten mee opgehaald moeten worden</param>
		/// <returns>Rij opgehaalde kinderen</returns>
		IEnumerable<Kind> OphalenUitGroepsWerkJaar(int groepsWerkJaarID, Expression<Func<Kind, object>>[] paths);

		/// <summary>
		/// Haalt alle kinderen op uit afdelingsjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// en <paramref name="afdelingID"/>.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van groepswerkjaar van afdelingsjaar</param>
		/// <param name="afdelingID">ID van afdeling van afdelingsjaar</param>
		/// <param name="paths">Bepaalt de mee op te halen entiteiten</param>
		/// <returns>Alle kinderen van het gevraagde afdelngsjaar</returns>
		IEnumerable<Kind> OphalenUitAfdelingsJaar(int groepsWerkJaarID, int afdelingID, Expression<Func<Kind, object>>[] paths);

		/// <summary>
		/// Haalt alle kinderen op uit groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// met functie bepaald door <paramref name="functieID"/>.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van groepswerkjaar</param>
		/// <param name="functieID">ID van functie</param>
		/// <param name="paths">Bepaalt de mee op te halen entiteiten</param>
		/// <returns>Alle kinderen van het gevraagde afdelngsjaar</returns>
		IEnumerable<Kind> OphalenUitFunctie(int groepsWerkJaarID, int functieID, Expression<Func<Kind, object>>[] paths);

		/// <summary>
		/// Haalt alle probeerleden (type Kind) op van de groep met ID <paramref name="groepID"/>
		/// </summary>
		/// <param name="groepID">ID van groep met op te halen probeerleden</param>
		/// <param name="paths">bepaalt de op te halen gekoppelde entiteiten</param>
		/// <returns>Lijst met info over de probeerleden</returns>
		IEnumerable<Kind> ProbeerLedenOphalen(int groepID, Expression<Func<Kind, object>>[] paths);
	}
}
