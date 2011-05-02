using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
	/// <summary>
	/// Interface voor data access voor uitstappen
	/// </summary>
	public interface IUitstappenDao: IDao<Uitstap>
	{
		/// <summary>
		/// Haalt alle uitstappen van een gegeven groep op.
		/// </summary>
		/// <param name="groepID">ID van de groep</param>
		/// <param name="inschrijvenMogelijk">Als dit <c>true</c> is, worden enkel de uitstappen van het
		/// huidige werkjaar van de groep opgehaald.</param>
		/// <returns>Details van uitstappen</returns>
		IEnumerable<Uitstap> OphalenVanGroep(int groepID, bool inschrijvenMogelijk);
	}
}
