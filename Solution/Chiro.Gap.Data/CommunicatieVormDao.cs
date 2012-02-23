// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
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

		/// <summary>
		/// Zoekt alle communicatievormen van een persoon (dus over de verschillende gelieerde
		/// personen heen).  Inclusief communicatietype.
		/// </summary>
		/// <param name="persoonID">ID van de persoon van dewelke we geinteresseerd zijn in de communicatie</param>
		/// <returns>Rij communicatievormen</returns>
		public IEnumerable<CommunicatieVorm> ZoekenOpPersoon(int persoonID)
		{
			IEnumerable<CommunicatieVorm> resultaat;

			using (var db = new ChiroGroepEntities())
			{
				resultaat = (from commVorm in db.CommunicatieVorm.Include(cv => cv.CommunicatieType)
				             where commVorm.GelieerdePersoon.Persoon.ID == persoonID
				             select commVorm).ToArray();
			}

			Utility.DetachObjectGraph(resultaat);

			return resultaat;
		}
	}
}
