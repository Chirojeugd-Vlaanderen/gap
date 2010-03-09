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
	/// Interface voor een gegevenstoegangsobject voor Groepen
	/// </summary>
	public interface IGroepenDao : IDao<Groep>
	{
		/// <summary>
		/// Bepaalt het recentste GroepsWerkJaar van een gegeven Groep.
		/// Voor een actieve groep is dit steeds het huidige 
		/// GroepsWerkJaar; er kan pas een GroepsWerkJaar gemaakt 
		/// worden als het nieuw werkjaar begonnen is.
		/// </summary>
		/// <param name="groepID">ID van Groep waarvoor werkjaar bepaald 
		/// moet worden</param>
		/// <returns>Het relevante GroepsWerkJaarobject</returns>
		GroepsWerkJaar RecentsteGroepsWerkJaarGet(int groepID);

		Groep OphalenMetAfdelingen(int groepID);

		IList<OfficieleAfdeling> OphalenOfficieleAfdelingen();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="groepID">ID van de groep waar het over gaat</param>
		/// <returns></returns>
		Groep OphalenMetGroepsWerkJaren(int groepID);
	}
}
