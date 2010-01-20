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

		/// <summary>
		/// Haalt de afdelingen van een groep op die niet gebruikt zijn in een gegeven 
		/// groepswerkjaar, op basis van een <paramref name="groepsWerkJaarID"/>
		/// </summary>
		/// <param name="groepswerkjaarID">ID van het groepswerkjaar waarvoor de niet-gebruikte afdelingen
		/// opgezocht moeten worden.</param>
		/// <returns>de ongebruikte afdelingen van een groep in het gegeven groepswerkjaar</returns>
		public IList<Afdeling> OngebruikteOphalen(int groepsWerkJaarID)
		{
			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.Afdeling.MergeOption = MergeOption.NoTracking;

				return (from afdeling in db.Afdeling
					     where afdeling.Groep.GroepsWerkJaar.Any(gwj => gwj.ID == groepsWerkJaarID)
					     && !afdeling.AfdelingsJaar.Any(afdj => afdj.GroepsWerkJaar.ID == groepsWerkJaarID)
					     select afdeling).ToList();
			}

		}
	}
}
