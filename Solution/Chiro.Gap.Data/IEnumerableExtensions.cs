using System.Collections.Generic;
using System;
using System.Linq;

namespace Chiro.Gap.Data
{
	public static class IEnumerableExtensions
	{
		public static IEnumerable<T> Paging<T>(this IEnumerable<T> data, int pagina, int paginaGrootte)
		{
			return data.Skip((pagina - 1)*paginaGrootte)
				.Take(paginaGrootte);
		}

	}
}