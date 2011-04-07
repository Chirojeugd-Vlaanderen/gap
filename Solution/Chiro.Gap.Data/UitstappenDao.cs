using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Voorziet data access voor uitstappen
	/// </summary>
	public class UitstappenDao: Dao<Uitstap,ChiroGroepEntities>, IUitstappenDao 
	{
		/// <summary>
		/// Haalt alle uitstappen van een gegeven groep op.
		/// </summary>
		/// <param name="groepID">ID van de groep</param>
		/// <returns>Details van uitstappen</returns>
		public IEnumerable<Uitstap> OphalenVanGroep(int groepID)
		{
			IEnumerable<Uitstap> resultaat;
			using (var db = new ChiroGroepEntities())
			{
				resultaat = (from u in db.Uitstap
				             where u.GroepsWerkJaar.Groep.ID == groepID
				             select u).ToArray();
			}

			return Utility.DetachObjectGraph(resultaat);
		}
	}
}
