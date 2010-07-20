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

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Gegevenstoegangsobject voor gelieerde personen
	/// </summary>
	public class GelieerdePersonenDao : Dao<GelieerdePersoon, ChiroGroepEntities>, IGelieerdePersonenDao
	{
		/// <summary>
		/// Instantieert een gegevenstoegangsobject voor gelieerde personen
		/// </summary>
		public GelieerdePersonenDao()
		{
			connectedEntities = new Expression<Func<GelieerdePersoon, object>>[] 
			{ 
				e => e.Persoon, 
				e => e.Groep.WithoutUpdate()
			};
		}

		/// <summary>
		/// Haalt alle gelieerde personen van een groep op, inclusief de gerelateerde entity's gegeven
		/// in <paramref name="paths"/>
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan we de gelieerde personen willen opvragen</param>
		/// <param name="paths">Een array van lambda-expressions die de mee op te halen gerelateerde entity's
		/// bepaalt</param>
		/// <returns>De gevraagde lijst gelieerde personen</returns>
		public IList<GelieerdePersoon> AllenOphalen(
			int groepID,
			params Expression<Func<GelieerdePersoon, object>>[] paths)
		{
			IList<GelieerdePersoon> result;

			using (var db = new ChiroGroepEntities())
			{
				// Eerst alles 'gewoon' ophalen'.  Op die manier komen straatnamen en woonplaatsen
				// niet dubbel over voor alle adressen en standaardadres.

				var query = (from gp in db.GelieerdePersoon
							 where gp.Groep.ID == groepID
							 select gp) as ObjectQuery<GelieerdePersoon>;

				result = (IncludesToepassen(query, paths)).ToList();
			}

			// Dan detachen

			result = Utility.DetachObjectGraph(result);

			return result;
		}

		/// <summary>
		/// Haal een pagina op met gelieerde personen van een groep, inclusief hun categorieën en relevante 
		/// lidinfo voor het recentste werkjaar.
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan gelieerde personen op te halen zijn</param>
		/// <param name="pagina">Gevraagde pagina</param>
		/// <param name="paginaGrootte">Aantal personen per pagina</param>
		/// <param name="aantalTotaal">Out-parameter die weergeeft hoeveel gelieerde personen er in totaal 
		/// zijn. </param>
		/// <returns>De gevraagde lijst gelieerde personen</returns>
		public IList<GelieerdePersoon> PaginaOphalenMetLidInfo(
			int groepID,
			int pagina,
			int paginaGrootte,
			out int aantalTotaal)
		{
			int groepsWerkJaarID;

			using (var db = new ChiroGroepEntities())
			{
				// Vind het huidig groepsWerkJaarID
				groepsWerkJaarID = (from w in db.GroepsWerkJaar
									where w.Groep.ID == groepID
									orderby w.WerkJaar descending
									select w.ID).FirstOrDefault();
			}

			return PaginaOphalenMetLidInfo(
				groepID,
				groepsWerkJaarID,
				pagina,
				paginaGrootte,
				out aantalTotaal);
		}

		/// <summary>
		/// Haal een pagina op met gelieerde personen van een groep, inclusief hun categorieën en relevante 
		/// lidinfo voor het gegeven werkjaar.
		/// </summary>
		/// <param name="groepID">ID van de groep waarvan gelieerde personen op te halen zijn</param>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar waarvoor we geinteresseerd zijn in de
		/// lidinfo.</param>
		/// <param name="pagina">Gevraagde pagina</param>
		/// <param name="paginaGrootte">Aantal personen per pagina</param>
		/// <param name="aantalTotaal">Out-parameter die weergeeft hoeveel gelieerde personen er in totaal 
		/// zijn. </param>
		/// <returns>De gevraagde lijst gelieerde personen</returns>
		public IList<GelieerdePersoon> PaginaOphalenMetLidInfo(
			int groepID,
			int groepsWerkJaarID,
			int pagina,
			int paginaGrootte,
			out int aantalTotaal)
		{
			IList<GelieerdePersoon> lijst;

			using (var db = new ChiroGroepEntities())
			{
				// Haal de gelieerde personen op van de gevraagde groep
				var gpQuery = (from grp in db.Groep
							   .Include("GelieerdePersoon.Persoon")
							   .Include("GelieerdePersoon.Categorie")
							   where grp.ID == groepID
							   select grp).FirstOrDefault().GelieerdePersoon;

				// Selecteer gewenste pagina, en bepaal totaal aantal personen
				lijst = gpQuery
						.OrderBy(gp => String.Format(
							"{0} {1}",
							gp.Persoon.Naam,
							gp.Persoon.VoorNaam))
						.Skip((pagina - 1) * paginaGrootte)
						.Take(paginaGrootte).ToList();
				aantalTotaal = gpQuery.Count();

				// De gelieerde personen in 'lijst' zijn geattacht aan de objectcontext.  Als nu
				// ook de gewenste lidobjecten van het huidige werkjaar opgevraagd worden, dan zal
				// entity framework die automagisch koppelen aan de gelieerde personen in 'lijst'.

				IList<int> relevanteGpIDs = (from gp in lijst select gp.ID).ToList();
				var huidigeLedenUitLijst = (from ld in db.Lid.Include("GelieerdePersoon")
								.Where(Utility.BuildContainsExpression<Lid, int>(
									l => l.GelieerdePersoon.ID,
									relevanteGpIDs))
											where ld.GroepsWerkJaar.ID == groepsWerkJaarID
											select ld).ToList();
			}

			Utility.DetachObjectGraph(lijst);
			return lijst;
		}

		/// <summary>
		/// Haal een pagina op met gelieerde personen uit een categorie, inclusief lidinfo voor het huidige
		/// werkjaar.
		/// </summary>
		/// <param name="categorieID">ID van de gevraagde categorie</param>
		/// <param name="pagina">Gevraagde pagina</param>
		/// <param name="paginaGrootte">Grootte van de pagina</param>
		/// <param name="paths">Geeft aan welke gekoppelde entiteiten mee opgehaald moeten worden</param>
		/// <param name="metHuidigLidInfo">Als <c>true</c> worden ook eventuele lidobjecten *van dit werkjaar* 
		/// mee opgehaald.</param>
		/// <param name="aantalTotaal">Outputparameter die het totaal aantal personen in de categorie weergeeft</param>
		/// <returns>Lijst gelieerde personen</returns>
		public IList<GelieerdePersoon> PaginaOphalenUitCategorie(
			int categorieID, 
			int pagina, 
			int paginaGrootte,
			bool metHuidigLidInfo,
			out int aantalTotaal,
			params Expression<Func<GelieerdePersoon, object>>[] paths)
		{
			Groep g;
			IList<GelieerdePersoon> lijst;

			using (var db = new ChiroGroepEntities())
			{
				// Haal alle personen in de gevraagde categorie op
				
				// Met de oorspronkelijke query kreeg ik het niet geregeld:
                                //                              
				//  var query = (from c in db.Categorie.Include(cat => cat.GelieerdePersoon.First().Persoon)
				//                         where c.ID == categorieID
				//                         select c).FirstOrDefault().GelieerdePersoon;


				var query = from gp in db.GelieerdePersoon.Include(gp => gp.Categorie)
				            where gp.Categorie.Any(cat => cat.ID == categorieID)
				            select gp;

				var queryMetExtras = IncludesToepassen(
					query as ObjectQuery<GelieerdePersoon>,
					paths);

				// Pas Extra's toe

				// Sorteer ze en bepaal totaal aantal personen
				lijst = queryMetExtras.OrderBy(e => e.Persoon.Naam)
						  .Skip((pagina - 1) * paginaGrootte).Take(paginaGrootte)
						  .ToList();
				aantalTotaal = query.Count();

				// haal indien gevraagd huidige lidobjecten mee op

				if (metHuidigLidInfo)
				{
					// Haal de groep van de gevraagde categorie op
					g = (from c in db.Categorie
					     where c.ID == categorieID
					     select c.Groep).FirstOrDefault();

					// Haal het huidige groepswerkjaar van de groep op
					var huidigWj = (
									from w in db.GroepsWerkJaar
									where w.Groep.ID == g.ID
									orderby w.WerkJaar descending
									select w).FirstOrDefault<GroepsWerkJaar>().WerkJaar;

					// Lijst is geattacht aan de objectcontext.  Als we nu ook de lidojecten van de 
					// gelieerdepersonen in de lijst ophalen voor het gegeven werkjaar, dan worden
					// die DDD-gewijze aan de gelieerde personen gekoppeld.

					// Haal de IDs van alle relevante personen op
					IList<int> relevanteGpIDs = (from gp in lijst select gp.ID).ToList();

					// Selecteer nu alle leden van huidig werkjaar met relevant gelieerdePersoonID

					var huidigeLedenUitlijst = (from l in db.Lid.Include(ld => ld.GelieerdePersoon)
					                            	.Where(Utility.BuildContainsExpression<Lid, int>(ld => ld.GelieerdePersoon.ID,
					                            	                                                 relevanteGpIDs))
					                            where l.GroepsWerkJaar.WerkJaar == huidigWj
					                            select l).ToList();

					// !LET OP! Bovenstaande variabele is weliswaar never used, maar is wel nodig
					// om de huidige leden in de objectcontext te laden! Laten staan dus!
				}
			}
			Utility.DetachObjectGraph(lijst);

			return lijst;   // lijst met personen + lidinfo
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gelieerdePersonenIDs"></param>
		/// <returns></returns>
		public override IList<GelieerdePersoon> Ophalen(IEnumerable<int> gelieerdePersonenIDs)
		{
			using (var db = new ChiroGroepEntities())
			{
				db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;

				return (
					from gp in db.GelieerdePersoon.Include("Groep").Include("Persoon").Where(Utility.BuildContainsExpression<GelieerdePersoon, int>(gp => gp.ID, gelieerdePersonenIDs))
					select gp).ToList();
			}
		}

		/// <summary>
		/// Haalt een gelieerde persoon op, inclusief
		///   - persoon
		///   - communicatievormen
		///   - adressen
		///   - groepen
		///   - categorieen
		///   - ALLE lidobjecten van alle groepswerkjaren waarin de persoon actief was,
		///     (inclusief groepswerkjaren)  (@Broes, is dat echt wat je wil? zie changeset [742])
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van de gevraagde gelieerde persoon</param>
		/// <returns>Gelieerde persoon met alle bovenvernoemde details</returns>
		public GelieerdePersoon DetailsOphalen(int gelieerdePersoonID)
		{
			using (var db = new ChiroGroepEntities())
			{
				db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;
				return (
					from gp in db.GelieerdePersoon
						.Include(gp => gp.Persoon)
						.Include(gp => gp.Communicatie.First().CommunicatieType)
						.Include(gp => gp.PersoonsAdres)
						.Include(gp => gp.Persoon.PersoonsAdres.First().Adres.StraatNaam)
						.Include(gp => gp.Persoon.PersoonsAdres.First().Adres.WoonPlaats)
						.Include(gp => gp.Groep)
						.Include(gp => gp.Categorie)
						// FIXME: dit is vermoedelijk niet nodig, maar eens nakijken of het ergens gebruikt wordt?
						.Include(gp => gp.Lid.First().GroepsWerkJaar)
						.Include(gp => gp.Persoon.PersoonsVerzekering.First().VerzekeringsType)
					where gp.ID == gelieerdePersoonID
					select gp).FirstOrDefault();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
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

				using (var db = new ChiroGroepEntities())
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

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerable<CommunicatieType> CommunicatieTypesOphalen()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="groepID"></param>
		/// <param name="zoekStringNaam"></param>
		/// <returns></returns>
		public IList<GelieerdePersoon> ZoekenOpNaam(int groepID, string zoekStringNaam)
		{
			using (var db = new ChiroGroepEntities())
			{
				db.GelieerdePersoon.MergeOption = MergeOption.NoTracking;
				return (
					from gp in db.GelieerdePersoon
						.Include(gp => gp.Persoon)
						.Include(gp => gp.Communicatie)
						.Include(gp => gp.PersoonsAdres)
						.Include(gp => gp.Persoon.PersoonsAdres.First().Adres.StraatNaam)
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
		/// <param name="naam">Te zoeken naam (ongeveer)</param>
		/// <param name="voornaam">Te zoeken voornaam (ongeveer)</param>
		/// <returns>Lijst met gevonden matches</returns>
		/// <remarks>Includeert bij wijze van standaard de persoonsinfo</remarks>
		public IList<GelieerdePersoon> ZoekenOpNaamOngeveer(int groepID, string naam, string voornaam)
		{
			return ZoekenOpNaamOngeveer(groepID, naam, voornaam, gp => gp.Persoon);
		}

		/// <summary>
		/// Zoekt naar gelieerde personen van een bepaalde groep (met ID <paramref name="groepID"/> met naam 
		/// en voornaam gelijkaardig aan <paramref name="naam"/> en <paramref name="voornaam"/>.
		/// (inclusief communicatie en adressen)
		/// </summary>
		/// <param name="groepID">GroepID dat bepaalt in welke gelieerde personen gezocht mag worden</param>
		/// <param name="naam">Te zoeken naam (ongeveer)</param>
		/// <param name="voornaam">Te zoeken voornaam (ongeveer)</param>
		/// <param name="paths">Expressies die aangeven welke dependencies mee opgehaald moeten worden</param>
		/// <returns>Lijst met gevonden matches</returns>
		public IList<GelieerdePersoon> ZoekenOpNaamOngeveer(int groepID, string naam, string voornaam, params Expression<Func<GelieerdePersoon, object>>[] paths)
		{
			using (var db = new ChiroGroepEntities())
			{
				// Ik denk dat sql server user defined functions niet aangeroepen kunnen worden via LINQ to entities,
				// vandaar dat de query in Entity SQL opgesteld wordt.

				// !Herinner van bij de scouts dat die soundex best bij in de tabel terecht komt,
				// en dat daarop een index moet komen te liggen!

				var esqlQuery = "SELECT VALUE gp FROM ChiroGroepEntities.GelieerdePersoon AS gp " +
					"WHERE gp.Groep.ID = @groepid " +
					"AND ChiroGroepModel.Store.ufnSoundEx(gp.Persoon.Naam)=ChiroGroepModel.Store.ufnSoundEx(@naam) " +
					"AND ChiroGroepModel.Store.ufnSoundEx(gp.Persoon.Voornaam)=ChiroGroepModel.Store.ufnSoundEx(@voornaam)";

				// Query casten naar ObjectQuery, om zodadelijk de includes te kunnen toepassen.

				var query = db.CreateQuery<GelieerdePersoon>(esqlQuery) as ObjectQuery<GelieerdePersoon>;

				query.Parameters.Add(new ObjectParameter("groepid", groepID));
				query.Parameters.Add(new ObjectParameter("voornaam", voornaam));
				query.Parameters.Add(new ObjectParameter("naam", naam));

				return IncludesToepassen(query, paths).ToList();
			}
		}

		/*public IList<GelieerdePersoon> OphalenUitCategorie(int categorieID)
		{
		    using (var db = new ChiroGroepEntities())
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

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerable<CommunicatieType> OphalenCommunicatieTypes()
		{
			using (var db = new ChiroGroepEntities())
			{
				db.CommunicatieType.MergeOption = MergeOption.NoTracking;
				return (
					from gp in db.CommunicatieType
					select gp).ToList();
			}
		}

		/// <summary>
		/// Haalt alle gelieerde personen op (incl persoonsinfo) die op een zelfde
		/// adres wonen en gelieerd zijn aan dezelfde groep als de gelieerde persoon met het gegeven ID.
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van gegeven gelieerde
		/// persoon.</param>
		/// <returns>Lijst met GelieerdePersonen (inc. persoonsinfo)</returns>
		/// <remarks>Als de persoon nergens woont, is hij toch zijn eigen
		/// huisgenoot.  Enkel huisgenoten uit dezelfde groep als de gelieerde persoon worden opgeleverd.</remarks>
		public IList<GelieerdePersoon> HuisGenotenOphalenZelfdeGroep(int gelieerdePersoonID)
		{
			using (var db = new ChiroGroepEntities())
			{
				// Zoek eerst de geleerde persoon zelf.

				var zelf = (from gp in db.GelieerdePersoon.Include(gp => gp.Persoon).Include(gp => gp.Groep)
							where gp.ID == gelieerdePersoonID
							select gp).FirstOrDefault();

				if (zelf == null)
				{
					return new List<GelieerdePersoon>();
				}
				else
				{
					var query = (from gp in db.GelieerdePersoon.Include(gp => gp.Persoon)
								 where gp.Groep.ID == zelf.Groep.ID &&
								   gp.Persoon.PersoonsAdres.Any(
									   pa => pa.Adres.PersoonsAdres.Any(pa2 => pa2.Persoon.ID == zelf.Persoon.ID))
								 select gp);

					if (query.FirstOrDefault() == null)
					{
						return new List<GelieerdePersoon> { zelf };
					}
					else
					{
						return query.ToList();
					}
				}
			}
		}
	}
}
