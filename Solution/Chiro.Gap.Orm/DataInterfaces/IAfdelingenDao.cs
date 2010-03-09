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
	/// Interface voor een data access object voor afdelingen
	/// </summary>
	public interface IAfdelingenDao : IDao<Afdeling>
	{
		/// <summary>
		/// Haalt de afdelingen van een groep op die niet gebruikt zijn in een gegeven 
		/// groepswerkjaar, op basis van een <paramref name="groepsWerkJaarID"/>
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar waarvoor de niet-gebruikte afdelingen
		/// opgezocht moeten worden.</param>
		/// <returns>De ongebruikte afdelingen van een groep in het gegeven groepswerkjaar</returns>
		IList<Afdeling> OngebruikteOphalen(int groepsWerkJaarID);
	}
}
