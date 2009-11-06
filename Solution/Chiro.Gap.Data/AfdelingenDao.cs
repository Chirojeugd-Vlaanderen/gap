using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using System.Data.Objects;
using Chiro.Cdf.Data.Entity;

namespace Chiro.Gap.Data.Ef
{
	public class AfdelingenDao : Dao<Afdeling, ChiroGroepEntities>, IAfdelingenDao
	{
		/// <summary>
		/// Afdeling ophalen op basis van ID.
		/// </summary>
		/// <param name="afdelingID">ID van gewenste afdeling</param>
		/// <returns>Afdeling en gekoppelde groep</returns>
		public override Afdeling Ophalen(int afdelingID)
		{
			Afdeling resultaat;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.Afdeling.MergeOption = MergeOption.NoTracking;

				resultaat = (
				    from Afdeling afd
				    in db.Afdeling.Include("Groep")
				    where afd.ID == afdelingID
				    select afd).FirstOrDefault();

				if (resultaat != null)
				{
					// Je zou verwachten dat de Include hierboven
					// meteen ook de groep ophaalt, maar dat blijkt
					// niet het geval.

					resultaat.GroepReference.Load();
				}
				return resultaat;
			}
		}
	}
}
