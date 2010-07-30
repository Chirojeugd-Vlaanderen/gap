// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace Chiro.Gap.Data
{
	/// <summary>
	/// Static class om functionaliteit van IEnumerables uit te breiden
	/// </summary>
// ReSharper disable InconsistentNaming
	public static class IEnumerableExtensions
// ReSharper restore InconsistentNaming
	{
		/// <summary>
		/// TODO: documenteren
		/// </summary>
		/// <typeparam name="T">Het soort object waarvoor we deze functionaliteit willen gebruiken</typeparam>
		/// <param name="data">De gegevens in een IEnumerable</param>
		/// <param name="pagina">Een paginanummer</param>
		/// <param name="paginaGrootte">Het aantal gegevens per pagina</param>
		/// <returns></returns>
		public static IEnumerable<T> Paging<T>(this IEnumerable<T> data, int pagina, int paginaGrootte)
		{
			return data.Skip((pagina - 1) * paginaGrootte)
				.Take(paginaGrootte);
		}
	}
}