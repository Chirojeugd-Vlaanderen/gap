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
	public class GroepenDao : Dao<Groep, ChiroGroepEntities>, IGroepenDao
	{
		/*
		TODO creeren(groep) ?
		*/

		/// <summary>
		/// Haalt recentste groepswerkjaar op van groep met ID <paramref name="groepID"/>, inclusief 
		/// afdelingsjaren.
		/// </summary>
		/// <param name="groepID">ID van groep waarvan het recentste groepswerkjaar gevraagd is.</param>
		/// <returns>Groepswerkjaar van groep met ID <paramref name="groepID"/>, met daaraan gekoppeld de
		/// groep en de afdelingsjaren.</returns>
		public GroepsWerkJaar RecentsteGroepsWerkJaarGet(int groepID)
		{
			GroepsWerkJaar result;
			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.GroepsWerkJaar.MergeOption = MergeOption.NoTracking;

				var query = (
				    from wj in db.GroepsWerkJaar.Include("AfdelingsJaar").Include("Groep")
				    where wj.Groep.ID == groepID
				    orderby wj.WerkJaar descending
				    select wj);

				result = query.FirstOrDefault<GroepsWerkJaar>();
			}
			return result;
		}

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

		/*
        /// <summary>
		/// Creeert een nieuwe afdeling.
		/// </summary>
		/// <param name="groepID">ID van de groep</param>
		/// <param name="naam">naam van de afdeling</param>
		/// <param name="afkorting">afkorting voor de afdeling</param>
		/// <returns>Een relevant afdelingsobject</returns>
		public Afdeling AfdelingCreeren(int groepID, string naam, string afkorting)
		{
		    Afdeling a = new Afdeling();



		    using (ChiroGroepEntities db = new ChiroGroepEntities())
		    {
			var gp = (
			    from g in db.Groep
			    where g.ID == groepID
			    select g
			    ).FirstOrDefault<Groep>();

			a.AfdelingsNaam = naam;
			a.Afkorting = afkorting;
			a.Groep = gp;

			gp.Afdeling.Add(a);

			db.SaveChanges();

			db.Detach(a);
		    }

		    return a;
		}
         */

		/*/// <summary>
		/// Creeert een nieuw afdelingsjaar.
		/// </summary>
		/// <param name="g">Groep voor afdelingsjaar</param>
		/// <param name="a">Afdeling voor afdelingsjaar</param>
		/// <param name="oa">officiële afdeling voor afdelingsjaar</param>
		/// <param name="geboorteJaarVan">begingeboortejaar voor afdeling</param>
		/// <param name="geboorteJaarTot">eindgeboortejaar voor afdeling</param>
		/// <returns>Het nieuwe afdelingsjaar</returns>
		public AfdelingsJaar AfdelingsJaarCreeren(Groep g, Afdeling a, OfficieleAfdeling oa, int geboorteJaarVan, int geboorteJaarTot)
		{
		    AfdelingsJaar afdelingsJaar = new AfdelingsJaar();
		    GroepsWerkJaar huidigWerkJaar = RecentsteGroepsWerkJaarGet(g.ID);

		    // Omdat we met een combinatie van geattachte en nieuwe objecten
		    // zitten, geeft het het minste problemen als de we relaties tussen
		    // de objecten gedetachet leggen, en daarna AttachObjectGraph
		    // aanroepen.

		    // In theorie zouden de eerstvolgende zaken in Business kunnen gebeuren,
		    // en zou daarna het resulterende AfdelingsJaar
		    // doorgespeeld moeten worden aan deze functie, die het dan enkel
		    // nog moet persisteren.

		    // TODO: verifieren of de afdeling bij de groep hoort door
		    // de groep van de afdeling op te halen, ipv alle afdelingen
		    // van de groep.

		    // Groep g heeft niet altijd de afdelingen mee
		    Groep groepMetAfdelingen = OphalenMetAfdelingen(g.ID);

		    // TODO: deze test hoort thuis in business, niet in DAL:

		    if (!groepMetAfdelingen.Afdeling.Contains(a))
		    {
			throw new InvalidOperationException("Afdeling " + a.AfdelingsNaam + " is geen afdeling van Groep " + g.Naam);
		    }

		    // TODO: test of de officiële afdeling bestaat, heb
		    // ik voorlopig even weggelaten.  Als de afdeling niet
		    // bestaat, zal er bij het bewaren toch een exception
		    // optreden, aangezien het niet de bedoeling is dat
		    // een officiële afdeling bijgemaakt wordt.

		    //TODO check if no conflicts with existing afdelingsjaar
		    //TODO: bovenstaande TODO moet ook in business layer gebeuren

		    afdelingsJaar.OfficieleAfdeling = oa;
		    afdelingsJaar.Afdeling = a;
		    afdelingsJaar.GroepsWerkJaar = huidigWerkJaar;
		    afdelingsJaar.GeboorteJaarVan = geboorteJaarVan;
		    afdelingsJaar.GeboorteJaarTot = geboorteJaarTot;

		    a.AfdelingsJaar.Add(afdelingsJaar);
		    oa.AfdelingsJaar.Add(afdelingsJaar);
		    huidigWerkJaar.AfdelingsJaar.Add(afdelingsJaar);

		    using (ChiroGroepEntities db = new ChiroGroepEntities())
		    {
			AfdelingsJaar geattachtAfdJr = db.AttachObjectGraph(afdelingsJaar
			    , aj=>aj.OfficieleAfdeling.WithoutUpdate()
			    , aj=>aj.Afdeling.WithoutUpdate()
			    , aj=>aj.GroepsWerkJaar.WithoutUpdate());

			db.SaveChanges();

			// SaveChanges geeft geattachtAfdJr een ID.  Neem dit
			// id over in het gedetachte afdelingsJaar.

			afdelingsJaar.ID = geattachtAfdJr.ID;
		    }

		    return afdelingsJaar;
		}*/

		public IList<OfficieleAfdeling> OphalenOfficieleAfdelingen()
		{
			IList<OfficieleAfdeling> result;
			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				result = (
				    from d in db.OfficieleAfdeling
				    select d
				).ToList();
			}
			return Utility.DetachObjectGraph(result);
		}

        public Groep OphalenMetGroepsWerkJaren(int groepID)
        {
            Groep result = null;
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.GroepsWerkJaar.MergeOption = MergeOption.NoTracking;

                result = (
                    from g in db.Groep.Include("GroepsWerkJaar")
                    where g.ID == groepID
                    select g
                ).FirstOrDefault();
            }
            return Utility.DetachObjectGraph(result);
        }
    }
}
