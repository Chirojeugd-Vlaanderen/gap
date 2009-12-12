using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

using System.Data.Objects.DataClasses;
using System.Linq.Expressions;

namespace Chiro.Gap.Data.Ef
{
	public class AutorisatieDao : Dao<GebruikersRecht, ChiroGroepEntities>, IAutorisatieDao
	{
		#region IAuthorisatieDao Members

		public GebruikersRecht RechtenMbtGroepGet(string login, int groepID)
		{
			GebruikersRecht resultaat;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
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

		public GebruikersRecht RechtenMbtGelieerdePersoonGet(string login, int gelieerdePersoonID)
		{
			GebruikersRecht resultaat;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
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

		public bool IsGavGroep(string login, int groepID)
		{
			using (ChiroGroepEntities db = new ChiroGroepEntities())
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

		public bool IsGavGelieerdePersoon(string login, int gelieerdePersoonID)
		{
			bool resultaat;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				//Dit is nodig om bijvoorbeeld een nieuwe persoon te maken
				var query1
				    = from r in db.GelieerdePersoon
				      where r.ID == gelieerdePersoonID
				      select r;

				if (query1.Count() == 0)
				{
					return true;
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


		public IEnumerable<Groep> GekoppeldeGroepenGet(string login)
		{
			using (ChiroGroepEntities db = new ChiroGroepEntities())
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

		public IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs, string login)
		{
			List<int> resultaat;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				var query
				    = db.GelieerdePersoon
				    .Where(Utility.BuildContainsExpression<GelieerdePersoon, int>(gp => gp.ID, gelieerdePersonenIDs))
				    .Where(gp => gp.Groep.GebruikersRecht.Any(r => r.Gav.Login == login && (r.VervalDatum == null || r.VervalDatum > DateTime.Now)))
				    .Select(gp => gp.ID);

				resultaat = query.ToList<int>();
			}

			return resultaat;
		}


		public IList<int> EnkelMijnPersonen(IEnumerable<int> personenIDs, string login)
		{
			List<int> resultaat;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				var query
				    = db.GelieerdePersoon
				    .Where(Utility.BuildContainsExpression<GelieerdePersoon, int>(gp => gp.Persoon.ID, personenIDs))
				    .Where(gp => gp.Groep.GebruikersRecht.Any(r => r.Gav.Login == login && (r.VervalDatum == null || r.VervalDatum > DateTime.Now)))
				    .Select(gp => gp.Persoon.ID);

				resultaat = query.ToList<int>();
			}

			return resultaat;
		}

		public bool IsGavPersoon(string login, int persoonID)
		{
			bool resultaat;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				//Dit is nodig om bijvoorbeeld een nieuwe persoon te maken
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

		public bool IsGavGroepsWerkJaar(string login, int groepsWerkJaarID)
		{
			bool resultaat;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				//Dit is nodig om bijvoorbeeld een nieuwe persoon te maken
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

		public bool IsGavAfdeling(string login, int afdelingsID)
		{
			bool resultaat;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				//Dit is nodig om bijvoorbeeld een nieuwe persoon te maken
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

		public bool IsGavLid(string login, int lidID)
		{
			bool resultaat;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				//Dit is nodig om bijvoorbeeld een nieuwe persoon te maken
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

		public bool IsGavCategorie(int categorieID, string login)
		{
			bool resultaat;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				//Dit is nodig om bijvoorbeeld een nieuwe persoon te maken
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

		public bool IsGavCommVorm(int commvormID, string login)
		{
			bool resultaat;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				//Dit is nodig om bijvoorbeeld een nieuwe persoon te maken
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

		#endregion
	}
}
