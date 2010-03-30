// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Gegevenstoegangsobject voor AfdelingsJaren
	/// </summary>
	public class AfdelingsJaarDao : Dao<AfdelingsJaar, ChiroGroepEntities>, IAfdelingsJarenDao
	{
		/// <summary>
		/// Instantieert een gegevenstoegangsobject voor AfdelingsJaren
		/// </summary>
		public AfdelingsJaarDao()
			: base()
		{
			connectedEntities = new System.Linq.Expressions.Expression<Func<AfdelingsJaar, object>>[3]
			{
				aj => aj.Afdeling.WithoutUpdate(),
				aj => aj.GroepsWerkJaar.WithoutUpdate(),
				aj => aj.OfficieleAfdeling.WithoutUpdate()
			};
		}

		/// <summary>
		/// Afdelingsjaar ophalen op basis van ID's van de
		/// afdeling en het groepswerkjaar.  Samen met afdelingsjaar
		/// wordt GroepsWerkJaar, OfficieleAfdeling en Afdeling teruggegeven.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <param name="afdelingID">ID van de afdeling</param>
		/// <returns>Het gevraagde afdelingsjaar, of null indien niet
		/// gevonden.</returns>
		/// <remarks>Dit heeft enkel zin als de afdeling bepaald door
		/// AfdelingID een afdeling is van de groep bepaald door het
		/// gevraagde groepswerkjaar.</remarks>
		public AfdelingsJaar Ophalen(int groepsWerkJaarID, int afdelingID)
		{
			return Ophalen(groepsWerkJaarID, afdelingID, connectedEntities);
		}

		/// <summary>
		/// Afdelingsjaar ophalen op basis van ID's van de
		/// afdeling en het groepswerkjaar.  Samen met afdelingsjaar
		/// wordt GroepsWerkJaar, OfficieleAfdeling en Afdeling teruggegeven.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <param name="afdelingID">ID van de afdeling</param>
		/// <param name="paths">Bepaalt welke gerelateerde entity's mee opgehaald moeten worden</param>
		/// <returns>Het gevraagde afdelingsjaar, of null indien niet
		/// gevonden.</returns>
		/// <remarks>Dit heeft enkel zin als de afdeling bepaald door
		/// AfdelingID een afdeling is van de groep bepaald door het
		/// gevraagde groepswerkjaar.</remarks>
		public AfdelingsJaar Ophalen(
			int groepsWerkJaarID,
			int afdelingID,
			params Expression<Func<AfdelingsJaar, object>>[] paths)
		{
			AfdelingsJaar resultaat = null;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.AfdelingsJaar.MergeOption = MergeOption.NoTracking;

				var query = (
					from AfdelingsJaar aj
					in db.AfdelingsJaar
					where aj.GroepsWerkJaar.ID == groepsWerkJaarID
					&& aj.Afdeling.ID == afdelingID
					select aj) as ObjectQuery<AfdelingsJaar>;

				resultaat = IncludesToepassen(query, paths).FirstOrDefault();
			}

			return Utility.DetachObjectGraph(resultaat);
		}

	}
}
