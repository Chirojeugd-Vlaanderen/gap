using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	public class CommunicatieVormDao : Dao<CommunicatieVorm, ChiroGroepEntities>, ICommunicatieVormDao
	{
		public IList<CommunicatieVorm> ZoekenOpNummer(string zoekString)
		{
			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.CommunicatieVorm.MergeOption = MergeOption.NoTracking;

				return (
				    from cv in db.CommunicatieVorm
				    where cv.Nummer == zoekString
				    select cv).ToList();
			}
		}
	}
}
