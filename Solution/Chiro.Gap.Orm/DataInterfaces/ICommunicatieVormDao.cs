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
	/// Interface voor een gegevenstoegangsobject voor CommunicatieVormen
	/// </summary>
	public interface ICommunicatieVormDao : IDao<CommunicatieVorm>
	{
		IList<CommunicatieVorm> ZoekenOpNummer(string zoekString);
	}
}
