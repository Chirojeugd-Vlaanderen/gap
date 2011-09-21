// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
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
		/// <param name="groepID">ID van de groep</param>
		/// <returns>
		/// Een GebruikersRecht-object waarmee we kunnen nagaan welke rechten de gebruiker heeft of had
		/// m.b.t. de groep waar het over gaat
		/// </returns>
		/// <remarks>Let op: de gebruikersrechten kunnen vervallen zijn!</remarks>
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
		/// <param name="login">De gebruikersnaam van de bezoeker</param>
		/// <param name="gelieerdePersoonID">ID van gelieerde persoon</param>
		/// <returns><c>Null</c> indien geen gebruikersrechten gevonden,
		/// anders een GebruikersRecht-object</returns>
		/// <remarks>Let op: de gebruikersrechten kunnen vervallen zijn!</remarks>
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
        /// Als een gelieerde persoon een gebruikersrecht heeft/had voor zijn eigen groep, dan
        /// levert deze call dat gebruikersrecht op.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van een gelieerde persoon</param>
        /// <returns>Gebruikersrecht van de gelieerde persoon met ID <paramref name="gelieerdePersoonID"/>
        /// op zijn eigen groep (if any, anders null)</returns>
	    public GebruikersRecht GebruikersRechtGelieerdePersoon(int gelieerdePersoonID)
        {
            GebruikersRecht resultaat;
	        using (var db = new ChiroGroepEntities())
	        {

                // GavSchap is een tabel met kolommen (GavID, PersoonID).  (GavID, PersoonID) is primary key, en
                // bij gevolg uniek.
                // GavSchap wordt gebruikt om een persoon met een GAV te koppelen.  Dus eigenlijk zou dat een
                // 0..1-to-0..1 relatie moeten zijn.  Maar dat lukt precies niet goed in Entity Framework.

	            var gav = (from gp in db.GelieerdePersoon
	                               where gp.ID == gelieerdePersoonID
	                               select gp.Persoon.Gav.FirstOrDefault()).FirstOrDefault();

                if (gav == null)
                {
                    return null;
                }

   	            var groep = (from gp in db.GelieerdePersoon
	                         where gp.ID == gelieerdePersoonID
	                         select gp.Groep).FirstOrDefault();


	            resultaat = (from gr in db.GebruikersRecht
	                         where gr.Groep.ID == groep.ID && gr.Gav.ID == gav.ID
	                         select gr).FirstOrDefault();
	        }

            return Utility.DetachObjectGraph(resultaat);
        }

	    /// <summary>
		/// Kijkt in de tabel Gebruikersrecht of er een record is dat de  opgegeven 
		/// <paramref name="login"/> aan de opgegeven <paramref name="groepID"/> koppelt.
		/// </summary>
		/// <param name="login">De login van de gebruiker in kwestie</param>
		/// <param name="groepID">De ID van de groep die de gebruiker wil zien en/of bewerken</param>
		/// <returns><c>True</c> als de gebruiker nu GAV is</returns>
		public bool IsGavGroep(string login, int groepID)
		{
			using (var db = new ChiroGroepEntities())
			{
				var query
									= from r in db.GebruikersRecht
									  where r.Groep.ID == groepID && r.Gav.Login == login
									  && (r.VervalDatum == null || r.VervalDatum > DateTime.Now)
									  select r;

				return query.Count() > 0;
			}
		}

		/// <summary>
		/// Kijkt na in de tabel Gebruikersrecht of de persoon gelieerd is aan een groep
		/// waar de gebruiker GAV van is
		/// </summary>
		/// <param name="login">De login van de gebruiker in kwestie</param>
		/// <param name="gelieerdePersoonID">De ID van de gelieerde persoon</param>
		/// <returns><c>True</c> als de bezoeker nu GAV is van de groep waar de persoon aan gelieerd is, 
		/// <c>false</c> in het andere geval</returns>
		public bool IsGavGelieerdePersoon(string login, int gelieerdePersoonID)
		{
			if (gelieerdePersoonID == 0) // is het geval bij een nieuwe gelieerde persoon
			{
				return true;
			}
			else
			{
				using (var db = new ChiroGroepEntities())
				{
					DateTime nu = DateTime.Now;

					var query
						= from GebruikersRecht r in db.GebruikersRecht
						  where r.Gav.Login == login && r.Groep.GelieerdePersoon.Any(gp => gp.ID == gelieerdePersoonID)
						  && (r.VervalDatum == null || r.VervalDatum >= nu)
						  select r;

					return query.Count() > 0;
				}
			}
		}

		/// <summary>
		/// Haalt de groepen op waarvoor de gebruiker met de opgegeven login
		/// *op dit moment* Gebruikersrechten heeft
		/// </summary>
		/// <param name="login">De login van de gebruiker in kwestie</param>
		/// <returns>Lijst met een of meerdere groepen</returns>
		public IEnumerable<Groep> MijnGroepenOphalen(string login)
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
		/// Haalt uit een lijst van ID's gelieerde personen degene die onder de *huidige* gebruikersrechten vallen
		/// van de gebruiker met de opgegeven login
		/// </summary>
		/// <param name="gelieerdePersonenIDs">Een lijst van ID's van gelieerde personen</param>
		/// <param name="login">De login van de gebruiker in kwestie</param>
		/// <returns>Een lijst van ID's van de gelieerde personen die de gebruiker mag bekijken en bewerken</returns>
		public IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs, string login)
		{
			List<int> resultaat;

			using (var db = new ChiroGroepEntities())
			{
				var query
					= db.GelieerdePersoon
					.Where(Utility.BuildContainsExpression<GelieerdePersoon, int>(gp => gp.ID, gelieerdePersonenIDs))
					.Where(gp => gp.Groep.GebruikersRecht.Any(r => r.Gav.Login == login && (r.VervalDatum == null || r.VervalDatum > DateTime.Now)))
					.Select(gp => gp.ID).Distinct();

				resultaat = query.ToList();
			}

			return resultaat;
		}

		/// <summary>
		/// Haalt uit een lijst van ID's personen degene die onder de *huidige* gebruikersrechten vallen
		/// van de gebruiker met de opgegeven login
		/// </summary>
		/// <param name="personenIDs">Een lijst van ID's van personen</param>
		/// <param name="login">De login van de gebruiker in kwestie</param>
		/// <returns>Een lijst van ID's van de personen die de gebruiker mag bekijken en bewerken</returns>
		public IList<int> EnkelMijnPersonen(IEnumerable<int> personenIDs, string login)
		{
			List<int> resultaat;

			using (var db = new ChiroGroepEntities())
			{
				var query
					= db.GelieerdePersoon
					.Where(Utility.BuildContainsExpression<GelieerdePersoon, int>(gp => gp.Persoon.ID, personenIDs))
					.Where(gp => gp.Groep.GebruikersRecht.Any(r => r.Gav.Login == login && (r.VervalDatum == null || r.VervalDatum > DateTime.Now)))
					.Select(gp => gp.Persoon.ID).Distinct();

				resultaat = query.ToList();
			}

			return resultaat;
		}

		/// <summary>
		/// Verwijdert uit een lijst van LidID's de ID's
		/// van leden voor wie de gebruiker met gegeven <paramref name="login"/> geen GAV is.
		/// </summary>
		/// <param name="lidIDs">ID's van leden</param>
		/// <param name="login">login van de gebruiker</param>
		/// <returns>Enkel de ID's van leden waarvoor de gebruiker GAV is.</returns>
		public IEnumerable<int> EnkelMijnLeden(IEnumerable<int> lidIDs, string login)
		{
			using (var db = new ChiroGroepEntities())
			{
				return (db.Lid
					.Where(Utility.BuildContainsExpression<Lid, int>(ld => ld.ID, lidIDs))
					.Where(
						ld =>
						ld.GelieerdePersoon.Groep.GebruikersRecht.Any(
							r => r.Gav.Login == login && (r.VervalDatum == null || r.VervalDatum > DateTime.Now)))
					.Select(ld => ld.ID).Distinct()).ToArray();
			}
		}

		/// <summary>
		/// Gaat na of de gebruiker met de opgegeven login *nu* GAV-rechten heeft 
		/// op de persoon met de opgegeven ID
		/// </summary>
		/// <param name="login">De login van de gebruiker in kwestie</param>
		/// <param name="persoonID">De ID van de persoon over wie het gaat</param>
		/// <returns><c>True</c> als de bezoeker GAV is, 
		/// <c>false</c> in het andere geval</returns>
		public bool IsGavPersoon(string login, int persoonID)
		{
			// (moet deze test misschen eerder in de business dan hier?)
			if (persoonID == 0)  // is het geval bij nieuwe personen
			{
				return true;
			}
			else
			{
				using (var db = new ChiroGroepEntities())
				{
					var query
						= from GebruikersRecht r in db.GebruikersRecht
						  where r.Gav.Login == login && r.Groep.GelieerdePersoon.Any(gp => gp.Persoon.ID == persoonID)
								&& (r.VervalDatum == null || r.VervalDatum >= DateTime.Now)
						  select r;

					return query.Count() > 0;
				}
			}
		}

		/// <summary>
		/// Gaat na of de gebruiker met de opgegeven login *nu* GAV-rechten heeft 
		/// voor het groepswerkjaar met de opgegeven ID
		/// </summary>
		/// <param name="login">De login van de gebruiker in kwestie</param>
		/// <param name="groepsWerkJaarID">De ID van het groepswerkjaar in kwestie</param>
		/// <returns><c>True</c> als de bezoeker GAV is van de gevraagde groep in het
		/// gekoppelde werkjaar, <c>false</c> in het andere geval</returns>
		public bool IsGavGroepsWerkJaar(string login, int groepsWerkJaarID)
		{
			if (groepsWerkJaarID == 0) // is het geval bij een nieuw groepswerkjaar
			{
				return true;
			}
			else
			{
				using (var db = new ChiroGroepEntities())
				{
					var query
						= from GebruikersRecht r in db.GebruikersRecht
						  where r.Gav.Login == login && r.Groep.GroepsWerkJaar.Any(gwj => gwj.ID == groepsWerkJaarID)
								&& (r.VervalDatum == null || r.VervalDatum > DateTime.Now)
						  select r;

					return query.Count() > 0;
				}
			}
		}

		/// <summary>
		/// Gaat na of de gebruiker met de opgegeven login *nu* GAV-rechten heeft 
		/// voor de groep die bij de gegeven afdeling hoort
		/// </summary>
		/// <param name="login">De login van de gebruiker in kwestie</param>
		/// <param name="afdelingsID">ID van de afdeling in kwestie</param>
		/// <returns><c>True</c> als de bezoeker GAV is voor de bedoelde afdeling,
		/// <c>false</c> als dat niet het geval is</returns>
		public bool IsGavAfdeling(string login, int afdelingsID)
		{
			if (afdelingsID == 0) // is het geval bij nieuwe afdelingen
			{
				return true;
			}
			else
			{
				using (var db = new ChiroGroepEntities())
				{
					var query = from a in db.Afdeling
					             where a.ID == afdelingsID
					                   && a.ChiroGroep.GebruikersRecht.Any(
					                   	gr => gr.Gav.Login == login &&
					                   	      (gr.VervalDatum == null || gr.VervalDatum > DateTime.Now))
					             select a;

					return query.FirstOrDefault() != null;
				}
			}
		}

		/// <summary>
		/// Gaat na of de gebruiker met de opgegeven login *nu* GAV-rechten heeft 
		/// voor de groep die bij het gegeven afdelingsJaar hoort
		/// </summary>
		/// <param name="login">De login van de gebruiker in kwestie</param>
		/// <param name="afdelingsJaarID">ID van het gegeven afdelingsJaar</param>
		/// <returns><c>True</c> als de bezoeker GAV is voor het bedoelde afdelingsJaar,
		/// <c>false</c> als dat niet het geval is</returns>
		public bool IsGavAfdelingsJaar(string login, int afdelingsJaarID)
		{
			if (afdelingsJaarID == 0) // is het geval bij een nieuw afdelingsjaar
			{
				return true;
			}
			else
			{
				using (var db = new ChiroGroepEntities())
				{
					var query =
						from aj in db.AfdelingsJaar
						where aj.ID == afdelingsJaarID
							  && aj.GroepsWerkJaar.Groep.GebruikersRecht.Any(
								gr => gr.Gav.Login == login && (gr.VervalDatum == null || gr.VervalDatum > DateTime.Now))
						select aj;

					return query.Count() > 0;
				}
			}
		}

		/// <summary>
		/// Gaat na of de gebruiker met de opgegeven login *nu* GAV-rechten heeft 
		/// voor het gegeven lid
		/// </summary>
		/// <param name="login">De login van de gebruiker in kwestie</param>
		/// <param name="lidID">ID van het lid in kwestie</param>
		/// <returns><c>True</c> als de bezoeker GAV is van de groep waar het lid ingeschreven is, 
		/// <c>false</c> in het andere geval</returns>
		public bool IsGavLid(string login, int lidID)
		{
			if (lidID == 0) // is het geval bij een nieuw lid
			{
				return true;
			}
			else
			{
				using (var db = new ChiroGroepEntities())
				{
					var query =
						from l in db.Lid
						where l.ID == lidID
						&& l.GroepsWerkJaar.Groep.GebruikersRecht.Any(r => r.Gav.Login == login && (r.VervalDatum == null || r.VervalDatum > DateTime.Now))
						select l;

					return query.Count() > 0;
				}
			}
		}

		/// <summary>
		/// Gaat na of de gebruiker met de opgegeven login *nu* GAV-rechten heeft 
		/// op (de groep met) de gegeven categorie
		/// </summary>
		/// <param name="categorieID">ID van de categorie in kwestie</param>
		/// <param name="login">De login van de gebruiker in kwestie</param>
		/// <returns><c>True</c> als de bezoeker GAV is van de groep waar die categorie gebruikt wordt, 
		/// <c>false</c> in het andere geval</returns>
		public bool IsGavCategorie(int categorieID, string login)
		{
			if (categorieID == 0) // is het geval bij een nieuwe categorie
			{
				return true;
			}
			else
			{
				using (var db = new ChiroGroepEntities())
				{
					var query =
						from l in db.Categorie
						where l.ID == categorieID
						&& l.Groep.GebruikersRecht.Any(r => r.Gav.Login == login && (r.VervalDatum == null || r.VervalDatum > DateTime.Now))
						select l;

					return query.Count() > 0;
				}
			}
		}

		/// <summary>
		/// Gaat na of de gebruiker met de opgegeven login *nu* GAV-rechten heeft 
		/// op (de persoon met) de gegeven communicatievorm
		/// </summary>
		/// <param name="commvormID">ID van de communicatievorm in kwestie</param>
		/// <param name="login">De login van de gebruiker in kwestie</param>
		/// <returns><c>True</c> als de bezoeker GAV is van de groep waar die communicatievorm geregistreerd is, 
		/// <c>false</c> in het andere geval</returns>
		public bool IsGavCommVorm(int commvormID, string login)
		{
			if (commvormID == 0) // is het geval bij een nieuwe communicatievorm
			{
				return true;
			}
			else
			{
				using (var db = new ChiroGroepEntities())
				{
					var query =
						from l in db.CommunicatieVorm
						where l.ID == commvormID
						&& l.GelieerdePersoon.Groep.GebruikersRecht.Any(r => r.Gav.Login == login && (r.VervalDatum == null || r.VervalDatum > DateTime.Now))
						select l;

					return query.Count() > 0;
				}
			}
		}

		/// <summary>
		/// Gaat na of de gebruiker met de opgegeven login *nu* GAV-rechten heeft 
		/// op (de persoon met) het gegeven adres
		/// </summary>
		/// <param name="persoonsAdresID">ID van het persoonsadres</param>
		/// <param name="login">De login van de gebruiker in kwestie</param>
		/// <returns><c>True</c> als het persoonsAdres met ID <paramref name="persoonsAdresID"/> gekoppeld is aan een persoon
		/// waarop de gebruiker met login <paramref name="login"/> momenteel GAV-rechten op heeft.  Anders
		/// <c>false</c>.</returns>
		public bool IsGavPersoonsAdres(int persoonsAdresID, string login)
		{
			if (persoonsAdresID == 0) // is het geval bij een nieuw persoonsadres
			{
				return true;
			}
			else
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
		}

		/// <summary>
		/// Geeft <c>true</c> als de uitstap met ID <paramref name="uitstapID"/> gekoppeld is aan een 
		/// groepswerkjaar waarop de gebruiker met login <paramref name="login"/> momenteel GAV-rechten heeft.  Anders
		/// <c>false</c>.
		/// </summary>
		/// <param name="uitstapID">ID van de uitstap</param>
		/// <param name="login">De gebruikersnaam</param>
		/// <returns><c>true</c> als de uitstap met ID <paramref name="uitstapID"/> gekoppeld is aan een 
		/// groepswerkjaar waarop de gebruiker met login <paramref name="login"/> momenteel GAV-rechten heeft.  Anders
		/// <c>false</c>.</returns>
		public bool IsGavUitstap(int uitstapID, string login)
		{
			if (uitstapID == 0) // is het geval bij een nieuwe uitstap
			{
				return true;
			}
			else
			{
				using (var db = new ChiroGroepEntities())
				{
					var query = from u in db.Uitstap
					            where u.ID == uitstapID &&
					                  u.GroepsWerkJaar.Groep.GebruikersRecht.Any(
					                  	r => r.Gav.Login == login && (r.VervalDatum == null || r.VervalDatum > DateTime.Now))
					            select u;

					return query.Count() > 0;
				}
			}
		}

		/// <summary>
		/// Geeft <c>true</c> als de plaats met ID <paramref name="plaatsID"/> gekoppeld is aan een 
		/// groep waarop de gebruiker met login <paramref name="login"/> momenteel GAV-rechten heeft.  Anders
		/// <c>false</c>.
		/// </summary>
		/// <param name="plaatsID">ID van de plaats</param>
		/// <param name="login">De gebruikersnaam</param>
		/// <returns><c>true</c> als de plaats met ID <paramref name="plaatsID"/> gekoppeld is aan een 
		/// groep waarop de gebruiker met login <paramref name="login"/> momenteel GAV-rechten heeft.  Anders
		/// <c>false</c>.</returns>
		public bool IsGavPlaats(int plaatsID, string login)
		{
			if (plaatsID == 0) // is het geval bij een nieuwe plaats
			{
				return true;
			}
			else
			{
				using (var db = new ChiroGroepEntities())
				{
					var query = from p in db.Plaats
						    where p.ID == plaatsID &&
							  p.Groep.GebruikersRecht.Any(
								r => r.Gav.Login == login && (r.VervalDatum == null || r.VervalDatum > DateTime.Now))
						    select p;

					return query.Count() > 0;
				}
			}
		}

        /// <summary>
        /// Controleert of een gebruiker op dit moment GAV-rechten heeft op de deelnemer
        /// met ID <paramref name="deelnemerID"/>
        /// </summary>
        /// <param name="deelnemerID">ID van een (uitstap)deelnemer</param>
        /// <param name="login">login van de gebruiker wiens GAV-schap moet worden getest</param>
        /// <returns><c>true</c> als de gebruiker GAV-rechten heeft voor de gevraagde 
        /// deelnemer, anders <c>false</c></returns>
	    public bool IsGavDeelnemer(int deelnemerID, string login)
	    {
            if (deelnemerID == 0) // is het geval bij een nieuwe deelnemer
            {
                return true;
            }
            else
            {
                using (var db = new ChiroGroepEntities())
                {
                    var query = from d in db.Deelnemer
                                where d.ID == deelnemerID &&
                                  d.GelieerdePersoon.Groep.GebruikersRecht.Any(
                                    r => r.Gav.Login == login && (r.VervalDatum == null || r.VervalDatum > DateTime.Now))
                                select d;

                    return query.Count() > 0;
                }
            }
	    }

	    #endregion
	}
}
