// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace Chiro.Gap.Data
{
	/// <summary>
	/// Static class om functionaliteit van IQueryables uit te breiden
	/// </summary>
	public static class QueryableExtensions
	{
		/// <summary>
		/// Past een queryable op type <typeparamref name="T"/> aan zodanig dat enkel de gevraagde pagina
		/// wordt opgehaald.
		/// </summary>
		/// <typeparam name="T">Het soort object waarvoor we deze functionaliteit willen gebruiken</typeparam>
		/// <param name="data">De qyeryable</param>
		/// <param name="pagina">Een paginanummer</param>
		/// <param name="paginaGrootte">Het aantal gegevens per pagina</param>
		/// <returns>Een aangepaste queryable, die enkel de gevraagde pagina ophaalt</returns>
		public static IQueryable<T> PaginaSelecteren<T>(this IQueryable<T> data, int pagina, int paginaGrootte)
		{
			return data.Skip((pagina - 1) * paginaGrootte)
				.Take(paginaGrootte);
		}
	}
}