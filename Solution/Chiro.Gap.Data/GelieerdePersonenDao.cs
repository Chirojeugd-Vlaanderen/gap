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
		    from gp in db.GelieerdePersoon.Include("Persoon").Include("Lid.GroepsWerkJaar").Include("Groep").Include("Categorie")
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


		/// <summary>
		/// Haal een pagina op met gelieerde personen uit een categorie, inclusief lidinfo voor het huidige
		/// werkjaar.
		/// </summary>
		/// <param name="pagina">gevraagde pagina</param>
		/// <param name="paginaGrootte">grootte van de pagina</param>
		/// <param name="aantalTotaal">outputparameter die het totaal aantal personen in de categorie weergeeft</param>
		/// <param name="categorieID">ID van de gevraagde categorie</param>
		/// <returns>lijst gelieerde personen</returns>
		public IList<GelieerdePersoon> PaginaOphalenMetLidInfoVolgensCategorie(int categorieID, int pagina, int paginaGrootte, out int aantalTotaal)
		{
			Groep g;
			int huidigWj;

			IList<GelieerdePersoon> lijst;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				//haal de groep van de gevraagde categorie op
				g = (from c in db.Categorie
				     where c.ID == categorieID
				     select c.Groep).FirstOrDefault();

				//haal het huidige groepswerkjaar van de groep op
				huidigWj = (
				    from w in db.GroepsWerkJaar
				    where w.Groep.ID == g.ID
				    orderby w.WerkJaar descending
				    select w).FirstOrDefault<GroepsWerkJaar>().WerkJaar;

				// haal alle personen in de gevraagde categorie op
				var query = (from c in db.Categorie.Include("GelieerdePersoon.Persoon")
					     where c.ID == categorieID
					     select c).FirstOrDefault().GelieerdePersoon;

				//sorteer ze en bepaal totaal aantal personen
				lijst = query.OrderBy(e => e.Persoon.Naam)
					      .Skip((pagina - 1) * paginaGrootte).Take(paginaGrootte)
					      .ToList();
				aantalTotaal = query.Count();

				// lijst is geattacht aan de objectcontext.  Als we nu ook de lidojecten van de 
				// gelieerdepersonen in de lijst ophalen voor het gegeven werkjaar, dan worden
				// die DDD-gewijze aan de gelieerde personen gekoppeld.

				// haal de IDs van alle relevante personen op
				IList<int> relevanteGpIDS = (from gp in lijst select gp.ID).ToList();

				// Selecteer nu alle leden van huidig werkjaar met relevant gelieerdePersoonID

				var huidigeLedenUitlijst = (from l in db.Lid.Include("GelieerdePersoon.Categorie")
								.Where(Utility.BuildContainsExpression<Lid, int>(ld => ld.GelieerdePersoon.ID, relevanteGpIDS))
							    where l.GroepsWerkJaar.WerkJaar == huidigWj
							    select l).ToList();
			}
			Utility.DetachObjectGraph(lijst);

			return lijst;   // lijst met personen + lidinfo
		}

		public override IList<GelieerdePersoon> Ophalen(IEnumerable<int> gelieerdePersonenIDs)
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
				    from gp in db.GelieerdePersoon.Include("Persoon").Include("Communicatie.CommunicatieType").Include("Persoon.PersoonsAdres.Adres.Straat").Include("Persoon.PersoonsAdres.Adres.Subgemeente").Include("Groep").Include("Categorie").Include("Lid.GroepsWerkJaar")
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

		/// <summary>
		/// Zoekt naar gelieerde personen van een bepaalde groep (met ID <paramref name="groepID"/> met naam 
		/// en voornaam gelijkaardig aan <paramref name="naam"/> en <paramref name="voornaam"/>.
		/// (inclusief communicatie en adressen)
		/// </summary>
		/// <param name="groepID">GroepID dat bepaalt in welke gelieerde personen gezocht mag worden</param>
		/// <param name="naam">te zoeken naam (ongeveer)</param>
		/// <param name="voornaam">te zoeken voornaam (ongeveer)</param>
		/// <returns>lijst met gevonden matches</returns>
		public IList<GelieerdePersoon> ZoekenOpNaamOngeveer(int groepID, string naam, string voornaam)
		{
			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				// Ik denk dat sql server user defined functions niet aangeroepen kunnen worden via LINQ to entities,
				// vandaar dat de query in Entity SQL opgesteld wordt.

				// !Herinner van bij de scouts dat die soundex best bij in de tabel terecht komt,
				// en dat daarop een index moet komen te liggen!


				string esqlQuery = "SELECT VALUE gp FROM ChiroGroepEntities.GelieerdePersoon AS gp " +
					"WHERE gp.Groep.ID = @groepid " +
					"AND ChiroGroepModel.Store.ufnSoundEx(gp.Persoon.Naam)=ChiroGroepModel.Store.ufnSoundEx(@naam) " +
					"AND ChiroGroepModel.Store.ufnSoundEx(gp.Persoon.Voornaam)=ChiroGroepModel.Store.ufnSoundEx(@voornaam)";

				var query = db.CreateQuery<GelieerdePersoon>(esqlQuery);

				query.Parameters.Add(new ObjectParameter("groepid", groepID));
				query.Parameters.Add(new ObjectParameter("voornaam", voornaam));
				query.Parameters.Add(new ObjectParameter("naam", naam));

				return query.ToList();
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
