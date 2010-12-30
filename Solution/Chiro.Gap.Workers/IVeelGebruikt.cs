using System.Collections.Generic;

using Chiro.Gap.Orm;

namespace Chiro.Gap.Workers
{
	public interface IVeelGebruikt
	{
		/// <summary>
		/// Verwijdert het recentste groepswerkjaar van groep met ID <paramref name="groepID"/>
		/// uit de cache.
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan groepswerkjaarcache te resetten</param>
		void GroepsWerkJaarResetten(int groepID);

		/// <summary>
		/// Haalt van de groep met gegeven <paramref name="groepID"/> het recentste
		/// groepswerkjaar op, inclusief de groep zelf.
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan groepswerkjaar gevraagd</param>
		/// <returns>Het groepswerkjaar, met daaraan gekoppeld de groep</returns>
		GroepsWerkJaar GroepsWerkJaarOphalen(int groepID);

		/// <summary>
		/// Haalt alle nationale functies op
		/// </summary>
		/// <returns>Lijstje nationale functies</returns>
		IEnumerable<Functie> NationaleFunctiesOphalen();
	}
}