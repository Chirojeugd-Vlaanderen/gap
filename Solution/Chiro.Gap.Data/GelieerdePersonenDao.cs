using System;
using System.Collections.Generic;
using System.Data;
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
	public class GelieerdePersonenDao : Dao<GelieerdePersoon, ChiroGroepEntities>, IGelieerdePersonenDao
	{
		public GelieerdePersonenDao()
		{
			connectedEntities = new Expression<Func<GelieerdePersoon, object>>[2] { 
                                        e => e.Persoon, 
                                        e => e.Groep };
		}

		public IList<GelieerdePersoon> AllenOphalen(int groepID)
		{
			List<GelieerdePersoon> result;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;
				// direct gedetachte gelieerde personen ophalen

				result = (
				    from gp in db.GelieerdePersoon.Include("Persoon").Include("Groep")
				    where gp.Groep.ID == groepID
				    select gp).ToList<GelieerdePersoon>();
			}
			return result;
		}

		//todo na deze aanroep is result.Persoon toch nog == null!!?
		//Johan: probeer eens om MergeOption op MergeOption.NoTracking te zetten
		public override GelieerdePersoon Ophalen(int id)
		{
			GelieerdePersoon result;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				result = (
				    from t in db.GelieerdePersoon.Include("Persoon").Include("Groep")
				    where t.ID == id
				    select t).FirstOrDefault<GelieerdePersoon>();
				db.Detach(result);
			}

			return result;
		}

		public IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
		{
			int wj;
			IList<GelieerdePersoon> lijst;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;

				wj = (
				    from w in db.GroepsWerkJaar
				    where w.Groep.ID == groepID
				    orderby w.WerkJaar descending
				    select w).FirstOrDefault<GroepsWerkJaar>().WerkJaar;

				aantalTotaal = (
				    from gp in db.GelieerdePersoon
				    where gp.Groep.ID == groepID
				    select gp).Count();

				//TODO zoeken hoe je kan bepalen of hij alleen de leden includes als die aan 
				//bepaalde voorwaarden voldoen, maar wel alle gelieerdepersonen
				lijst = (
				    from gp in db.GelieerdePersoon.Include("Persoon").Include("Lid.GroepsWerkJaar").Include("Groep")
				    where gp.Groep.ID == groepID
				    orderby gp.Persoon.Naam, gp.Persoon.VoorNaam
				    select gp).Skip((pagina - 1) * paginaGrootte).Take(paginaGrootte).ToList();

				// work around: alle leden verwijderen behalve het (eventuele) lid in het huidige werkjaar
				foreach (GelieerdePersoon gp in lijst)
				{
					IList<Lid> verkeerdeLeden = (
					    from Lid l in gp.Lid
					    where l.GroepsWerkJaar.WerkJaar != wj
					    select l).ToList();

					foreach (Lid verkeerdLid in verkeerdeLeden)
					{
						gp.Lid.Remove(verkeerdLid);
					}
				}
				/*                foreach (GelieerdePersoon gp in lijst) {
						    Lid huidigLid = gp.Lid.FirstOrDefault(lid => lid.GroepsWerkJaar.WerkJaar == wj);
						    gp.Lid.Clear();
						    if (huidigLid != null)
						    {
							gp.Lid.Add(huidigLid);
						    }
						} */

				/* Dit hieronder werkt ook nog niet ...
				 * 
				 * var tmpLijst = (
				    from gp in db.GelieerdePersoon.Include("Persoon")
				    where gp.Groep.ID == groepID
				    orderby gp.Persoon.Naam, gp.Persoon.VoorNaam
				    select new { gp, gp.Persoon, huidigLid = gp.Lid.FirstOrDefault(lid => lid.GroepsWerkJaar.WerkJaar == wj) }
				    ).Skip((pagina - 1) * paginaGrootte).Take(paginaGrootte).ToList();

				lijst = new List<GelieerdePersoon>();
				foreach (var tmp in tmpLijst)
				{
				    GelieerdePersoon gp = tmp.gp;
				    if (tmp.huidigLid != null)
				    {
					gp.Lid.Add(tmp.huidigLid);
				    }
				    lijst.Add(gp);
				}*/
			}

			return lijst;   // met wat change komt de relevante lidinfo mee.
		}

        public IList<GelieerdePersoon> PaginaOphalenMetLidInfoVolgensCategorie(int groepID, int pagina, int paginaGrootte, out int aantalTotaal, int categorieID)
        {
            int wj;
            IList<GelieerdePersoon> lijst;

            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;

                wj = (
                    from w in db.GroepsWerkJaar
                    where w.Groep.ID == groepID
                    orderby w.WerkJaar descending
                    select w).FirstOrDefault<GroepsWerkJaar>().WerkJaar;

                aantalTotaal = (
                    from gp in db.GelieerdePersoon
                    where gp.Groep.ID == groepID
                    select gp).Count();

                //TODO zoeken hoe je kan bepalen of hij alleen de leden includes als die aan 
                //bepaalde voorwaarden voldoen, maar wel alle gelieerdepersonen
                lijst = (
                    from gp in db.GelieerdePersoon.Include("Persoon").Include("Lid.GroepsWerkJaar").Include("Groep")
                    where gp.Groep.ID == groepID
                    orderby gp.Persoon.Naam, gp.Persoon.VoorNaam
                    select gp).Skip((pagina - 1) * paginaGrootte).Take(paginaGrootte).ToList();

                // work around: alle leden verwijderen behalve het (eventuele) lid in het huidige werkjaar
                for (int i=0; i<lijst.Count; i++)
                {
                    GelieerdePersoon gp = lijst.ElementAt(i);
                    bool found = false;
                    foreach (Categorie c in gp.Categorie)
                    {
                        if (c.ID == categorieID)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        lijst.RemoveAt(i);
                        i--;
                        continue;
                    }

                    IList<Lid> verkeerdeLeden = (
                        from Lid l in gp.Lid
                        where l.GroepsWerkJaar.WerkJaar != wj
                        select l).ToList();

                    foreach (Lid verkeerdLid in verkeerdeLeden)
                    {
                        gp.Lid.Remove(verkeerdLid);
                    }
                }
            }

            return lijst;   // met wat change komt de relevante lidinfo mee.
        }

		public IList<GelieerdePersoon> Ophalen(IEnumerable<int> gelieerdePersonenIDs)
		{
			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;

				return (
				    from gp in db.GelieerdePersoon.Include("Groep").Include("Persoon").Where(Utility.BuildContainsExpression<GelieerdePersoon, int>(gp => gp.ID, gelieerdePersonenIDs))
				    select gp).ToList();
			}
		}

		public GelieerdePersoon DetailsOphalen(int gelieerdePersoonID)
		{
			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;
				return (
				    from gp in db.GelieerdePersoon.Include("Persoon").Include("Communicatie.CommunicatieType").Include("Persoon.PersoonsAdres.Adres.Straat").Include("Persoon.PersoonsAdres.Adres.Subgemeente").Include("Groep").Include("Categorie")
				    where gp.ID == gelieerdePersoonID
				    select gp).FirstOrDefault();
			}

		}

		public GelieerdePersoon GroepLaden(GelieerdePersoon p)
		{
			Debug.Assert(p != null);

			if (p.Groep == null)
			{
				Groep g;

				Debug.Assert(p.ID != 0);
				// Voor een nieuwe gelieerde persoon (p.ID == 0) moet 
				// de groepsproperty altijd gezet zijn, anders kan hij
				// niet bewaard worden.  Aangezien g.Groep == null,
				// kan het dus niet om een nieuwe persoon gaan.

				using (ChiroGroepEntities db = new ChiroGroepEntities())
				{
					db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;

					g = (from gp in db.GelieerdePersoon
					     where gp.ID == p.ID
					     select gp.Groep).FirstOrDefault();
				}
				p.Groep = g;
				g.GelieerdePersoon.Add(p);  // nog niet zeker of dit gaat werken...
			}

			return p;
		}

		public IList<GelieerdePersoon> ZoekenOpNaam(int groepID, string zoekStringNaam)
		{
			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;
				return (
				    from gp in db.GelieerdePersoon.Include("Persoon")
					.Include("Communicatie")
					.Include("Persoon.PersoonsAdres.Adres.Straat")
				    where (gp.Persoon.VoorNaam + " " + gp.Persoon.Naam + " " + gp.Persoon.VoorNaam)
					.Contains(zoekStringNaam)
				    select gp).ToList();
			}
		}

		/*public IList<GelieerdePersoon> OphalenUitCategorie(int categorieID)
		{
		    using (ChiroGroepEntities db = new ChiroGroepEntities())
		    {
			db.Categorie.MergeOption = MergeOption.NoTracking;

			var query
			    = from c in db.Categorie.Include("GelieerdePersoon.Persoon")
			      where c.ID == categorieID
			      select c;

			Categorie cat = query.FirstOrDefault();

			if (cat == null)
			{
			    // categorie niet gevonden
			    return new List<GelieerdePersoon>();
			}
			else
			{
			    return cat.GelieerdePersoon.ToList();
			}
		    }
		}*/

		public IEnumerable<CommunicatieType> ophalenCommunicatieTypes()
		{
			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.CommunicatieType.MergeOption = MergeOption.NoTracking;
				return (
				    from gp in db.CommunicatieType
				    select gp).ToList();
			}
		}
	}
}
