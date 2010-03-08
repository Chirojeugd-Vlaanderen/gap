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
	/// Data access voor GAV's
	/// </summary>
	public interface IGavDao : IDao<Gav>
	{
		/// <summary>
		/// Haalt GAV-object op op basis van login
		/// </summary>
		/// <param name="login">De gebruikersnaam</param>
		/// <returns>GAV horende bij gegeven login</returns>
		Gav Ophalen(string login);
	}
}
