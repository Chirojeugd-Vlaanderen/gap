// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Data.Objects;
using System.Diagnostics;
using System.Linq;

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
		/// <remarks>Aan kadergroepen zullen sowieso geen afdelingen gekoppeld zijn.</remarks>
		/// <param name="groepsWerkJaarID">ID van gevraagde groepswerkjaar</param>
		/// <returns>Groep, afdelingsjaar, afdelingen en officiële afdelingen</returns>
		public Groep OphalenMetAfdelingen(int groepsWerkJaarID)
		{
			// TODO ik denk dat deze niet meer nuttig gebruikt wordt behalve in tests
			GroepsWerkJaar groepswj;

			using (var db = new ChiroGroepEntities())
			{
				// Deze functie geeft een circulaire graaf mee, dus gebruiken ik
				// MergeOption.NoTracking ipv Utility.DetachObjectGraph.

				db.AfdelingsJaar.MergeOption = MergeOption.NoTracking;
				db.GroepsWerkJaar.MergeOption = MergeOption.NoTracking;

				var ajQuery = (
					from aj in db.AfdelingsJaar.Include("Afdeling").Include("OfficieleAfdeling")
					where aj.GroepsWerkJaar.ID == groepsWerkJaarID
					select aj);

				// Afdelingen zijn gekoppeld aan Chirogroepen.  Omdat deze method ook
				// moet werken voor kadergroepen, kan ik de groep niet benaderen via de\
				// afdelingen.  (Bovendien zou dat problemen geven t.g.v.
				// MergeOption.NoTracking)

				// Ik selecteer dus de groep apart, en koppel daarna alle gevonden
				// afdelingen.

				// Een beetje prutsen om het originele groepswerkjaar aan de groep te koppelen.

				groepswj = (from gwj in db.GroepsWerkJaar
							where gwj.ID == groepsWerkJaarID
							select gwj).FirstOrDefault();

				groepswj.Groep = (from gwj in db.GroepsWerkJaar
								  where gwj.ID == groepsWerkJaarID
								  select gwj.Groep).FirstOrDefault();

				groepswj.Groep.GroepsWerkJaar.Add(groepswj);

				// Koppel gevonden afdelingsjaren aan groep en aan groepswerkjaar
				foreach (AfdelingsJaar aj in ajQuery)
				{
					// Als er afdelingsjaren zijn, dan MOET dit een Chirogroep zijn
					// (en geen kadergroep)

					Debug.Assert(groepswj.Groep is ChiroGroep);

					aj.Afdeling.ChiroGroep = groepswj.Groep as ChiroGroep;
					(groepswj.Groep as ChiroGroep).Afdeling.Add(aj.Afdeling);

					aj.GroepsWerkJaar = groepswj.Groep.GroepsWerkJaar.First();
					groepswj.Groep.GroepsWerkJaar.First().AfdelingsJaar.Add(aj);
				}
			}

			return groepswj.Groep;
		}

		/// <summary>
		/// Haalt groep op met gegeven stamnummer, inclusief recentste groepswerkjaar
		/// </summary>
		/// <param name="code">Stamnummer op te halen groep</param>
		/// <returns>Groep met <paramref name="code"/> als stamnummer</returns>
		public Groep Ophalen(string code)
		{
			using (var db = new ChiroGroepEntities())
			{
				db.Groep.MergeOption = MergeOption.NoTracking;

			    var groepsWj = (from gwj in db.GroepsWerkJaar.Include(gwj => gwj.Groep)
			                    where gwj.Groep.Code == code
			                    select gwj).OrderByDescending(gwj=>gwj.WerkJaar).FirstOrDefault();

                if (groepsWj != null)
                {
                    return groepsWj.Groep;
                }
                else
                {
                    var groep = (from g in db.Groep
                                 where g.Code == code
                                 select g).FirstOrDefault();
                    return groep;
                }
			}
		}
	}
}
