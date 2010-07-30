// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Gegevenstoegangsobject voor communicatievormen
	/// </summary>
	public class CommunicatieVormDao : Dao<CommunicatieVorm, ChiroGroepEntities>, ICommunicatieVormDao
	{
		/// <summary>
		/// Zoekt een lijst van communicatievormen waarbij de waarde (het 'nummer',
		/// maar dat kan ook bv. een mailadres zijn) overeenkomt met de zoekterm
		/// </summary>
		/// <param name="zoekString">De zoekterm</param>
		/// <returns>Een lijst van communicatievormen die de zoekterm als waarde hebben</returns>
		public IList<CommunicatieVorm> ZoekenOpNummer(string zoekString)
		{
			using (var db = new ChiroGroepEntities())
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
