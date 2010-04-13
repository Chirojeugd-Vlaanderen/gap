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
		Groep OphalenMetAfdelingen(int groepID);

		/// <summary>
		/// Haalt alle officiele afdelingen op
		/// </summary>
		/// <returns>Lijst officiele afdelingen</returns>
		IList<OfficieleAfdeling> OfficieleAfdelingenOphalen();

		/// <summary>
		/// Haalt de officiele afdeling met ID <paramref name="officieleAfdelingID"/> op.
		/// </summary>
		/// <param name="officieleAfdelingID">ID van de op te halen officiele afdeling.</param>
		/// <returns>Officiele afdeling met ID <paramref name="officieleAfdelingID"/></returns>
		OfficieleAfdeling OfficieleAfdelingOphalen(int officieleAfdelingID);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="groepID">ID van de groep waar het over gaat</param>
		/// <returns></returns>
		Groep OphalenMetGroepsWerkJaren(int groepID);
	}
}
