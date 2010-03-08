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
	/// Data Access Object voor straten
	/// </summary>
	public interface IStratenDao : IDao<Straat>
	{
		/// <summary>
		/// Haalt straat op op basis van naam en postnummer
		/// </summary>
		/// <param name="naam">straatnaam</param>
		/// <param name="postNr">postnummer</param>
		/// <returns>relevante straat</returns>
		Straat Ophalen(string naam, int postNr);

		/// <summary>
		/// Haalt alle straten op uit een gegeven <paramref name="postNr"/>, waarvan de naam begint
		/// met het gegeven <paramref name="straatBegin"/>.
		/// </summary>
		/// <param name="straatBegin">Eerste letters van de te zoeken straatnamen</param>
		/// <param name="postNr">Postnummer waarin te zoeken</param>
		/// <returns>Gegevens van de gevonden straten</returns>
		IList<Straat> MogelijkhedenOphalen(string naamBegin, int postNr);

		/// <summary>
		/// Haalt alle straten op uit een gegeven rij <paramref name="postNrs"/>, waarvan de naam begint
		/// met het gegeven <paramref name="straatBegin"/>.
		/// </summary>
		/// <param name="straatBegin">Eerste letters van de te zoeken straatnamen</param>
		/// <param name="postNrs">Postnummers waarin te zoeken</param>
		/// <returns>Gegevens van de gevonden straten</returns>
		IList<Straat> MogelijkhedenOphalen(string naamBegin, IEnumerable<int> postNrs);
	}
}
