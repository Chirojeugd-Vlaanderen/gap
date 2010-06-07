// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Gegevenstoegangsobject voor autorisatie-info
	/// </summary>
	public class AutorisatieDao : Dao<GebruikersRecht, ChiroGroepEntities>, IAutorisatieDao
	{
		#region IAuthorisatieDao Members

		/// <summary>
		/// Haalt de rechten op die de gebruiker met de opgegeven <paramref name="login"/> heeft of had
		/// voor de groep met de opgegeven <paramref name="groepID"/>
		/// </summary>
		/// <param name="login">De gebruikersnaam van de bezoeker</param>
		/// <param name="groepID">ID van de groep die de bezoeker wil bekijken/bewerken</param>
		/// <returns>
		/// Een GebruikersRecht-object waarmee we kunnen nagaan welke rechten de gebruiker heeft of had
		/// m.b.t. de groep waar het over gaat
		/// </returns>
		public GebruikersRecht RechtenMbtGroepGet(string login, int groepID)
		{
			GebruikersRecht resultaat;

			using (var db = new ChiroGroepEntities())
			{
				db.GebruikersRecht.MergeOption = MergeOption.NoTracking;

				// LET OP: Hier mag geen rekening gehouden worden
				// met de vervaldatum; we willen ook rechten kunnen
				// opvragen die vervallen zijn.
				//
				// Naar de vervaldatum kijken moet gebeuren in de
				// businesslaag.

				var query
					= from r in db.GebruikersRecht
					  where r.Groep.ID == groepID && r.Gav.Login == login
					  select r;

				resultaat = query.FirstOrDefault();
			}

			return resultaat;
		}

		/// <summary>
		/// Haalt de rechten op die de gebruiker met de opgegeven <paramref name="login"/> heeft of had
		/// voor de gelieerde persoon met de opgegeven <paramref name="gelieerdePersoonID"/>
		/// </summary>
		/// <param name="login"></param>
		/// <param name="gelieerdePersoonID"></param>
		/// <returns>
		/// Een GebruikersRecht-object waarmee we kunnen nagaan welke rechten de gebruiker heeft of had
		/// m.b.t. de gelieerde persoon waar het over gaat
		/// </returns>
		public GebruikersRecht RechtenMbtGelieerdePersoonGet(string login, int gelieerdePersoonID)
		{
			GebruikersRecht resultaat;

			using (var db = new ChiroGroepEntities())
			{
				db.GebruikersRecht.MergeOption = MergeOption.NoTracking;

				// LET OP: Hier mag geen rekening gehouden worden
				// met de vervaldatum; we willen ook rechten kunnen
				// opvragen die vervallen zijn.
				//
				// Naar de vervaldatum kijken moet gebeuren in de
				// businesslaag.

				var query
					= from GebruikersRecht r in db.GebruikersRecht
					  where r.Gav.Login == login && r.Groep.GelieerdePersoon.Any(gp => gp.ID == gelieerdePersoonID)
					  select r;

				resultaat = query.FirstOrDefault();
			}

			return resultaat;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="login"></param>
		/// <param name="groepID"></param>
		/// <returns></returns>
		public bool IsGavGroep(string login, int groepID)
		{
			using (var db = new ChiroGroepEntities())
			{
				var query1
					= from r in db.Groep
					  where r.ID == groepID
					  select r;

				if (query1.Count() == 0)
				{
					return true;
				}

				var query2
					= from r in db.GebruikersRecht
					  where r.Groep.ID == groepID && r.Gav.Login == login
					  && (r.VervalDatum == null || r.VervalDatum > DateTime.Now)
					  select r;

				return query2.Count() > 0;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="login"></param>
		/// <param name="gelieerdePersoonID"></param>
		/// <returns></returns>
		public bool IsGavGelieerdePersoon(string login, int gelieerdePersoonID)
		{
			bool resultaat;

			using (var db = new ChiroGroepEntities())
			{
				// Dit is nodig om bijvoorbeeld een nieuwe persoon te maken
				var query1
					= from r in db.GelieerdePersoon
					  where r.ID == gelieerdePersoonID
					  select r;

				if (query1.Count() == 0)
				{
					// Geen persoon gevonden => geen GAV.
					return false;
				}

				var query
					= from GebruikersRecht r in db.GebruikersRecht
					  where r.Gav.Login == login && r.Groep.GelieerdePersoon.Any(gp => gp.ID == gelieerdePersoonID)
					  && (r.VervalDatum == null || r.VervalDatum >= DateTime.Now)
					  select r;

				resultaat = query.Count() > 0;
			}

			return resultaat;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="login"></param>
		/// <returns></returns>
		public IEnumerable<Groep> GekoppeldeGroepenGet(string login)
		{
			using (var db = new ChiroGroepEntities())
			{
				// Hier kan het geen kwaad om wel rekening te houden
				// met de vervaldatum; dit is een specifieke query.
				//
				// In RechtenMbtGelieerdePersoonGet en RechtenMbtGroepGet
				// mogen we dat niet, omdat dit 'generieke' vragen zijn;
				// daar willen we ook vervallen rechten kunnen opvragen.

				return
					(from r in db.GebruikersRecht
					 where r.Gav.Login == login && (r.VervalDatum == null || r.VervalDatum > DateTime.Now)
					 select r.Groep).ToList();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gelieerdePersonenIDs"></param>
		/// <param name="login"></param>
		/// <returns></returns>
		public IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs, string login)
		{
			List<int> resultaat;

			using (var db = new ChiroGroepEntities())
			{
				var query
					= db.GelieerdePersoon
					.Where(Utility.BuildContainsExpression<GelieerdePersoon, int>(gp => gp.ID, gelieerdePersonenIDs))
					.Where(gp => gp.Groep.GebruikersRecht.Any(r => r.Gav.Login == login && (r.VervalDatum == null || r.VervalDatum > DateTime.Now)))
					.Select(gp => gp.ID);

				resultaat = query.ToList();
			}

			return resultaat;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="personenIDs"></param>
		/// <param name="login"></param>
		/// <returns></returns>
		public IList<int> EnkelMijnPersonen(IEnumerable<int> personenIDs, string login)
		{
			List<int> resultaat;

			using (var db = new ChiroGroepEntities())
			{
				var query
					= db.GelieerdePersoon
					.Where(Utility.BuildContainsExpression<GelieerdePersoon, int>(gp => gp.Persoon.ID, personenIDs))
					.Where(gp => gp.Groep.GebruikersRecht.Any(r => r.Gav.Login == login && (r.VervalDatum == null || r.VervalDatum > DateTime.Now)))
					.Select(gp => gp.Persoon.ID);

				resultaat = query.ToList();
			}

			return resultaat;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="login"></param>
		/// <param name="persoonID"></param>
		/// <returns></returns>
		public bool IsGavPersoon(string login, int persoonID)
		{
			bool resultaat;

			using (var db = new ChiroGroepEntities())
			{
				// Dit is nodig om bijvoorbeeld een nieuwe persoon te maken
				// TODO: nakijken of je niet beter gewoon test of persoonID == 0.
				// Ik ben er niet meer van overtuigd of je true moet krijgen als je een onbestaande
				// persoonID meegeeft.  Bovendien doe je nu 2 query's op de DB, terwijl het ook met
				// 1 query kan.
				var query1
					= from r in db.Persoon
					  where r.ID == persoonID
					  select r;

				if (query1.Count() == 0)
				{
					return true;
				}

				var query2
					= from GebruikersRecht r in db.GebruikersRecht
					  where r.Gav.Login == login && r.Groep.GelieerdePersoon.Any(gp => gp.Persoon.ID == persoonID)
					  && (r.VervalDatum == null || r.VervalDatum >= DateTime.Now)
					  select r;

				resultaat = query2.Count() > 0;
			}

			return resultaat;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="login"></param>
		/// <param name="groepsWerkJaarID"></param>
		/// <returns></returns>
		public bool IsGavGroepsWerkJaar(string login, int groepsWerkJaarID)
		{
			bool resultaat;

			using (var db = new ChiroGroepEntities())
			{
				// Dit is nodig om bijvoorbeeld een nieuwe persoon te maken
				var query1
					= from r in db.GroepsWerkJaar
					  where r.ID == groepsWerkJaarID
					  select r;

				if (query1.Count() == 0)
				{
					return true;
				}

				var query
					= from GebruikersRecht r in db.GebruikersRecht
					  where r.Gav.Login == login && r.Groep.GroepsWerkJaar.Any(gwj => gwj.ID == groepsWerkJaarID)
					  && (r.VervalDatum == null || r.VervalDatum > DateTime.Now)
					  select r;

				resultaat = query.Count() > 0;
			}
			return resultaat;
		}

		/// <summary>
		/// Controleert of een gebruiker *nu* GAV is van de groep
		/// horende bij de gegeven afdeling
		/// </summary>
		/// <param name="login">De gebruikersnaam van de bezoeker</param>
		/// <param name="afdelingsID">ID van de gegeven afdeling</param>
		/// <returns><c>True</c> als de bezoeker Gav is voor de bedoelde afdeling,
		/// <c>false</c> als dat niet het geval is</returns>
		public bool IsGavAfdeling(string login, int afdelingsID)
		{
			bool resultaat;

			using (var db = new ChiroGroepEntities())
			{
				// Dit is nodig om bijvoorbeeld een nieuwe persoon te maken
				var query1
					= from r in db.Afdeling
					  where r.ID == afdelingsID
					  select r;

				if (query1.Count() == 0)
				{
					return true;
				}

				var query =
					from r in db.GebruikersRecht
					where r.Gav.Login == login
					&& r.Groep.Afdeling.Any(afd => afd.ID == afdelingsID)
					&& (r.VervalDatum == null || r.VervalDatum > DateTime.Now)
					select r;

				resultaat = query.Count() > 0;
			}
			return resultaat;
		}

		/// <summary>
		/// Controleert of een gebruiker *nu* GAV is van de groep
		/// horende bij het gegeven afdelingsJaar
		/// </summary>
		/// <param name="login">De gebruikersnaam van de bezoeker</param>
		/// <param name="afdelingsJaarID">ID van het gegeven afdelingsJaar</param>
		/// <returns><c>true</c> als de bezoeker Gav is voor het bedoelde afdelingsJaar,
		/// <c>false</c> als dat niet het geval is</returns>
		public bool IsGavAfdelingsJaar(string login, int afdelingsJaarID)
		{
			bool resultaat;

			using (var db = new ChiroGroepEntities())
			{
				if (afdelingsJaarID == 0)
				{
					return true;	// altijd GAV van een nieuw afdelingsjaar.
				}
				else
				{
					var query =
						from aj in db.AfdelingsJaar
						where aj.ID == afdelingsJaarID
						&& aj.GroepsWerkJaar.Groep.GebruikersRecht.Any(
							gr => gr.Gav.Login == login && (gr.VervalDatum == null || gr.VervalDatum > DateTime.Now))
						select aj;

					resultaat = query.Count() > 0;
				}
			}
			return resultaat;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="login"></param>
		/// <param name="lidID"></param>
		/// <returns></returns>
		public bool IsGavLid(string login, int lidID)
		{
			bool resultaat;

			using (var db = new ChiroGroepEntities())
			{
				// Dit is nodig om bijvoorbeeld een nieuwe persoon te maken
				var query1
					= from r in db.Lid
					  where r.ID == lidID
					  select r;

				if (query1.Count() == 0)
				{
					return true;
				}

				var query =
					from l in db.Lid
					where l.ID == lidID
					&& l.GroepsWerkJaar.Groep.GebruikersRecht.Any(r => r.Gav.Login == login && (r.VervalDatum == null || r.VervalDatum > DateTime.Now))
					select l;

				resultaat = query.Count() > 0;
			}
			return resultaat;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="categorieID"></param>
		/// <param name="login"></param>
		/// <returns></returns>
		public bool IsGavCategorie(int categorieID, string login)
		{
			bool resultaat;

			using (var db = new ChiroGroepEntities())
			{
				// Dit is nodig om bijvoorbeeld een nieuwe persoon te maken
				var query1
					= from r in db.Categorie
					  where r.ID == categorieID
					  select r;

				if (query1.Count() == 0)
				{
					return true;
				}

				var query =
					from l in db.Categorie
					where l.ID == categorieID
					&& l.Groep.GebruikersRecht.Any(r => r.Gav.Login == login && (r.VervalDatum == null || r.VervalDatum > DateTime.Now))
					select l;

				resultaat = query.Count() > 0;
			}
			return resultaat;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="commvormID"></param>
		/// <param name="login"></param>
		/// <returns></returns>
		public bool IsGavCommVorm(int commvormID, string login)
		{
			bool resultaat;

			using (var db = new ChiroGroepEntities())
			{
				// Dit is nodig om bijvoorbeeld een nieuwe persoon te maken
				var query1
					= from r in db.CommunicatieVorm
					  where r.ID == commvormID
					  select r;

				if (query1.Count() == 0)
				{
					return true;
				}

				var query =
					from l in db.CommunicatieVorm
					where l.ID == commvormID
					&& l.GelieerdePersoon.Groep.GebruikersRecht.Any(r => r.Gav.Login == login && (r.VervalDatum == null || r.VervalDatum > DateTime.Now))
					select l;

				resultaat = query.Count() > 0;
			}
			return resultaat;
		}

		/// <summary>
		/// Geeft <c>true</c> als het persoonsAdres met ID <paramref name="persoonsAdresID"/> gekoppeld is aan een persoon
		/// waarop de gebruiker met login <paramref name="login"/> momenteel GAV-rechten op heeft.  Anders
		/// <c>false</c>.
		/// </summary>
		/// <param name="persoonsAdresID">ID van de functie</param>
		/// <param name="login">Gebruikersnaam</param>
		/// <returns><c>true</c> als het persoonsAdres met ID <paramref name="persoonsAdresID"/> gekoppeld is aan een persoon
		/// waarop de gebruiker met login <paramref name="login"/> momenteel GAV-rechten op heeft.  Anders
		/// <c>false</c>.</returns>
		public bool IsGavPersoonsAdres(int persoonsAdresID, string login)
		{
			using (var db = new ChiroGroepEntities())
			{
				var query = from gp in db.GelieerdePersoon
				            where gp.Persoon.PersoonsAdres.Any(pa => pa.ID == persoonsAdresID) &&
				                  gp.Groep.GebruikersRecht.Any(
				                  	r => r.Gav.Login == login && (r.VervalDatum == null || r.VervalDatum > DateTime.Now))
				            select gp;

				return query.Count() > 0;
			}
		}

		#endregion
	}
}
