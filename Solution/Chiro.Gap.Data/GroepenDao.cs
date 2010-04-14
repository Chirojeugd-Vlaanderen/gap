// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Gegevenstoegangsobject voor groepen
	/// </summary>
	public class GroepenDao : Dao<Groep, ChiroGroepEntities>, IGroepenDao
	{
		/// <summary>
		/// Ophalen van groep, groepswerkjaar, afdeling, afdelingsjaar en officiële afdelingen 
		/// voor gegeven groepswerkjaar.
		/// </summary>
		/// <remarks>Deze functie haande origineel de afdelingen op voor een groep in het
		/// huidige werkjaar, maar 'huidige werkjaar' vind ik precies wat veel business
		/// voor in de DAL.</remarks>
		/// <param name="groepsWerkJaarID">ID van gevraagde groepswerkjaar</param>
		/// <returns>Groep, afdelingsjaar, afdelingen en officiële afdelingen</returns>
		public Groep OphalenMetAfdelingen(int groepsWerkJaarID)
		{
			GroepsWerkJaar groepswj;
			Groep result;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				// Deze functie geeft een circulaire graaf mee, dus gebruiken ik
				// MergeOption.NoTracking ipv Utility.DetachObjectGraph.

				db.AfdelingsJaar.MergeOption = MergeOption.NoTracking;
				db.GroepsWerkJaar.MergeOption = MergeOption.NoTracking;

				var ajQuery = (
					from aj in db.AfdelingsJaar.Include("Afdeling").Include("OfficieleAfdeling")
					where aj.GroepsWerkJaar.ID == groepsWerkJaarID
					select aj);

				// In principe zouden we Afdeling.Groep kunnen includen, 
				// maar aangezien we met NoTracking werken, zou dat resulteren
				// in een kopie van de groep voor elke afdeling, ipv 1
				// GroepsObject met daaraan gekoppeld de gewenste afdelingen.

				// Ik selecteer dus de groep apart, en koppel daarna alle gevonden
				// afdelingen.

				result = (
					from gwj in db.GroepsWerkJaar
					where gwj.ID == groepsWerkJaarID
					select gwj.Groep).FirstOrDefault();

				// Een beetje prutsen om het originele groepswerkjaar
				// aan de groep te koppelen.
				// TODO: Kan dit niet in 1 keer?

				groepswj = (
					from gwj in db.GroepsWerkJaar
					where gwj.ID == groepsWerkJaarID
					select gwj).FirstOrDefault();

				groepswj.Groep = result;
				result.GroepsWerkJaar.Add(groepswj);

				if (result != null)
				{
					// Koppel gevonden afdelingsjaren aan groep en aan groepswerkjaar

					foreach (AfdelingsJaar aj in ajQuery)
					{
						aj.Afdeling.Groep = result;
						result.Afdeling.Add(aj.Afdeling);

						aj.GroepsWerkJaar = result.GroepsWerkJaar.First();
						result.GroepsWerkJaar.First().AfdelingsJaar.Add(aj);
					}
				}
			}
			return result;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="groepID"></param>
		/// <returns></returns>
		public Groep OphalenMetGroepsWerkJaren(int groepID)
		{
			Groep result = null;
			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.GroepsWerkJaar.MergeOption = MergeOption.NoTracking;

				result = (from g in db.Groep.Include("GroepsWerkJaar")
						  where g.ID == groepID
						  select g).FirstOrDefault();
			}
			return Utility.DetachObjectGraph(result);
		}
	}
}
