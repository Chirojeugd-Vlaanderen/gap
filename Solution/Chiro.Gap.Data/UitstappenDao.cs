using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Diagnostics;
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
		/// <param name="inschrijvenMogelijk">Als dit <c>true</c> is, worden enkel de uitstappen van het
		/// huidige werkjaar van de groep opgehaald.</param>
		/// <returns>Details van uitstappen</returns>
		public IEnumerable<Uitstap> OphalenVanGroep(int groepID, bool inschrijvenMogelijk)
		{
			IEnumerable<Uitstap> resultaat;
			using (var db = new ChiroGroepEntities())
			{
				if (!inschrijvenMogelijk)
				{
					// Alle uitstappen ophalen
					resultaat = (from u in db.Uitstap
					             where u.GroepsWerkJaar.Groep.ID == groepID
					             select u).ToArray();
				}
				else
				{
					// Enkel uitstappen van recentste groepswerkjaar
					var groep = db.Groep.Include(g => g.GroepsWerkJaar).Where(grp => grp.ID == groepID).FirstOrDefault();
					Debug.Assert(groep != null);
					var groepsWerkJaar = groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar).FirstOrDefault();
					Debug.Assert(groepsWerkJaar != null);
					groepsWerkJaar.Uitstap.Load();
					resultaat = groepsWerkJaar.Uitstap.ToArray();
				}
			}

			return Utility.DetachObjectGraph(resultaat);
		}

		/// <summary>
		/// Haalt een uitstap op
		/// </summary>
		/// <param name="id">ID op te halen uitstap</param>
		/// <param name="paths">Gekoppelde entiteiten</param>
		/// <returns>Opgehaalde uitstap</returns>
		/// <remarks>Als de plaats is gekoppeld, wordt het adres mee opgehaald als Belgisch
		/// of buitenlands adres.</remarks>
		public override Uitstap Ophalen(int id, params System.Linq.Expressions.Expression<Func<Uitstap, object>>[] paths)
		{
			Uitstap resultaat;

			using (var db = new ChiroGroepEntities())
			{

				// Haal uitstap op met gekoppelde objecten

				var query = (from a in db.Uitstap
					     where a.ID == id
					     select a) as ObjectQuery<Uitstap>;

				query = IncludesToepassen(query, paths);
				resultaat = query.FirstOrDefault();

				// Als er een adres is, koppel de relevante 'onderdelen'

				if (resultaat.Plaats != null && resultaat.Plaats.Adres != null)
				{
					if (resultaat.Plaats.Adres is BelgischAdres)
					{
						((BelgischAdres)resultaat.Plaats.Adres).StraatNaamReference.Load();
						((BelgischAdres)resultaat.Plaats.Adres).WoonPlaatsReference.Load();
					}
					else if (resultaat.Plaats.Adres is BuitenLandsAdres)
					{
						((BuitenLandsAdres)resultaat.Plaats.Adres).LandReference.Load();
					}
				}

			}

			return Utility.DetachObjectGraph(resultaat);
		}
	}
}
