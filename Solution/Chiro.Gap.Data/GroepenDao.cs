// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Data.Objects;
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
		/// <remarks>Deze functie haalde origineel de afdelingen op voor een groep in het
		/// huidige werkjaar, maar 'huidige werkjaar' vind ik precies wat veel business
		/// voor in de DAL.</remarks>
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

				// In principe zouden we Afdeling.Groep kunnen includen, 
				// maar aangezien we met NoTracking werken, zou dat resulteren
				// in een kopie van de groep voor elke afdeling, ipv 1
				// GroepsObject met daaraan gekoppeld de gewenste afdelingen.

				// Ik selecteer dus de groep apart, en koppel daarna alle gevonden
				// afdelingen.

				// Een beetje prutsen om het originele groepswerkjaar aan de groep te koppelen.
				// TODO: Kan dit niet in 1 keer? => Poging gedaan, nog uitgebreider te controleren
				// Dit is een kortere versie van wat er eerst stond, maar het derde statement maakt duidelijk dat het inderdaad gepruts is :)

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
					aj.Afdeling.Groep = groepswj.Groep;
					groepswj.Groep.Afdeling.Add(aj.Afdeling);

					aj.GroepsWerkJaar = groepswj.Groep.GroepsWerkJaar.First();
					groepswj.Groep.GroepsWerkJaar.First().AfdelingsJaar.Add(aj);
				}
			}

			return groepswj.Groep;
		}

		/// <summary>
		/// Haalt groep op met gegeven stamnummer
		/// </summary>
		/// <param name="code">Stamnummer op te halen groep</param>
		/// <returns>Groep met <paramref name="code"/> als stamnummer</returns>
		public Groep Ophalen(string code)
		{
			using (var db = new ChiroGroepEntities())
			{
				db.Groep.MergeOption = MergeOption.NoTracking;

				return (from g in db.Groep
						where g.Code == code
						select g).FirstOrDefault();
			}
		}
	}
}
