// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Gegevenstoegangsobject voor leden
	/// </summary>
	public class LedenDao : Dao<Lid, ChiroGroepEntities>, ILedenDao
	{
		/// <summary>
		/// Instantieert een gegevenstoegangsobject voor leden
		/// </summary>
		public LedenDao()
		{
			connectedEntities = new Expression<Func<Lid, object>>[] 
            { 
				e => e.GroepsWerkJaar.WithoutUpdate(), 
				e => e.GelieerdePersoon.Persoon.WithoutUpdate()
            };
		}

		/// <summary>
		/// Zoekt lid op op basis van GelieerdePersoonID en GroepsWerkJaarID
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van gelieerde persoon</param>
		/// <param name="groepsWerkJaarID">ID van groepswerkjaar</param>
		/// <returns>Lidobject indien gevonden, anders null</returns>
		public Lid Ophalen(int gelieerdePersoonID, int groepsWerkJaarID)
		{
			return Ophalen(gelieerdePersoonID, groepsWerkJaarID, getConnectedEntities());
		}

		/// <summary>
		/// Haalt lid met gerelateerde entity's op, op basis van 
		/// GelieerdePersoonID en GroepsWerkJaarID
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van gelieerde persoon</param>
		/// <param name="groepsWerkJaarID">ID van groepswerkjaar</param>
		/// <param name="paths">Lambda-expressies die de extra op te halen
		/// informatie definieren</param>
		/// <returns>Lidobject indien gevonden, anders null</returns>
		public Lid Ophalen(int gelieerdePersoonID, int groepsWerkJaarID, params Expression<Func<Lid, object>>[] paths)
		{
			using (var db = new ChiroGroepEntities())
			{
				var query = (from l in db.Lid
							 where l.GelieerdePersoon.ID == gelieerdePersoonID
							 && l.GroepsWerkJaar.ID == groepsWerkJaarID
							 select l) as ObjectQuery<Lid>;
				return (IncludesToepassen(query, paths).FirstOrDefault());
			}
		}

		/// <summary>
		/// Een lijst ophalen van alle leden voor het opgegeven groepswerkjaar
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <returns>Een lijst alle leden voor het opgegeven groepswerkjaar</returns>
		public IList<Lid> AllesOphalen(int groepsWerkJaarID)
		{
			IList<Lid> lijst;

			using (var db = new ChiroGroepEntities())
			{
				db.Lid.MergeOption = MergeOption.NoTracking;

				var kinderen = (
					from l in db.Lid.OfType<Kind>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
					orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
					select l).ToList<Kind>();

				var leiding = (
					from l in db.Lid.OfType<Leiding>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
					orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
					select l).ToList<Leiding>();

				lijst = new List<Lid>();
				foreach (Lid lid in kinderen)
				{
					lijst.Add(lid);
				}
				foreach (Lid lid in leiding)
				{
					lijst.Add(lid);
				}
			}

			return lijst;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="groepsWerkJaarID"></param>
		/// <returns></returns>
		/// <remarks>
		/// Pagineren gebeurt per werkjaar.
		/// De parameters pagina, paginaGrootte en aantalTotaal zijn niet meer nodig.
		/// </remarks>
		public IList<Lid> PaginaOphalen(int groepsWerkJaarID)
		{
			IList<Lid> lijst = new List<Lid>();

			using (var db = new ChiroGroepEntities())
			{
				db.Lid.MergeOption = MergeOption.NoTracking;

				var kinderen = (
					from l in db.Lid.OfType<Kind>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
					orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
					select l).ToList<Kind>();

				var leiding = (
					from l in db.Lid.OfType<Leiding>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
					orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
					select l).ToList<Leiding>();

				// TODO: onderstaande moet eleganter kunnen.
				foreach (Lid lid in kinderen)
				{
					lijst.Add(lid);
				}
				foreach (Lid lid in leiding)
				{
					lijst.Add(lid);
				}
			}

			return lijst;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="groepsWerkJaarID"></param>
		/// <param name="afdelingsID"></param>
		/// <returns></returns>
		/// <remarks>
		/// Pagineren gebeurt per werkjaar.
		/// </remarks>
		public IList<Lid> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID)
		{
			IList<Lid> lijst;

			using (var db = new ChiroGroepEntities())
			{
				db.Lid.MergeOption = MergeOption.NoTracking;

				var kinderen = (
					from l in db.Lid.OfType<Kind>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
						&&
					  l.AfdelingsJaar.Afdeling.ID == afdelingsID
					orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
					select l).ToList<Kind>();

				var leiding = (
					from l in db.Lid.OfType<Leiding>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
						&&
					  l.AfdelingsJaar.Any(x => x.Afdeling.ID == afdelingsID)
					orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
					select l).ToList<Leiding>();

				lijst = new List<Lid>();
				foreach (Kind lid in kinderen)
				{
					lijst.Add(lid);
				}
				foreach (Leiding lid in leiding)
				{
					lijst.Add(lid);
				}
			}

			return lijst;
		}

		/// <summary>
		/// Haalt GEEN afdeling mee op (nakijken of dit ook effectief niet nodig is?
		/// </summary>
		/// <param name="groepsWerkJaarID"></param>
		/// <param name="functieID"></param>
		/// <returns></returns>
		public IList<Lid> PaginaOphalenVolgensFunctie(int groepsWerkJaarID, int functieID)
		{
			IList<Lid> lijst;

			using (var db = new ChiroGroepEntities())
			{
				db.Lid.MergeOption = MergeOption.NoTracking;

				var kinderen = (
					from l in db.Lid.OfType<Kind>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
						&&
					  l.Functie.Any(e => e.ID == functieID)
					orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
					select l).ToList();

				var leiding = (
					from l in db.Lid.OfType<Leiding>().Include("GroepsWerkJaar").Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling").Include("Functie")
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
						&&
					  l.Functie.Any(e => e.ID == functieID)
					orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
					select l).ToList();

				lijst = new List<Lid>();
				foreach (Kind lid in kinderen)
				{
					lijst.Add(lid);
				}
				foreach (Leiding lid in leiding)
				{
					lijst.Add(lid);
				}
			}

			return lijst;
		}

		/// <summary>
		/// Lid met afdelingsjaren, afdelingen en gelieerdepersoon
		/// </summary>
		/// <param name="lidID">ID van het lid waarvan we gegevens willen opvragen</param>
		/// <returns>Een lid met afdelingsjaren, afdelingen en gelieerdepersoon</returns>
		public Lid OphalenMetDetails(int lidID)
		{
			using (var db = new ChiroGroepEntities())
			{
				db.Lid.MergeOption = MergeOption.NoTracking;

				var lid = (
			from t in db.Lid.Include("GelieerdePersoon.Persoon").Include("GroepsWerkJaar.AfdelingsJaar.Afdeling").Include("Functie")
			where t.ID == lidID
			select t).FirstOrDefault<Lid>();

				if (lid is Kind)
				{
					return (
						from t in db.Lid.OfType<Kind>().Include("GelieerdePersoon.Persoon").Include("GroepsWerkJaar.AfdelingsJaar.Afdeling").Include("AfdelingsJaar.Afdeling").Include("Functie")
						where t.ID == lidID
						select t).FirstOrDefault<Kind>();
				}
				else if (lid is Leiding)
				{
					return (
						from t in db.Lid.OfType<Leiding>().Include("GelieerdePersoon.Persoon").Include("GroepsWerkJaar.AfdelingsJaar.Afdeling").Include("AfdelingsJaar.Afdeling").Include("Functie")
						where t.ID == lidID
						select t).FirstOrDefault<Leiding>();
				}
				return lid;
			}
		}

		/// <summary>
		/// Haalt de leden op die in het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// de functie bepaald door <paramref name="functieID"/> hebben
		/// </summary>
		/// <param name="functieID">ID van een functie</param>
		/// <param name="groepsWerkJaarID">ID van een groepswerkjaar</param>
		/// <param name="paths">Bepaalt de mee op te halen gekoppelde entiteiten</param>
		/// <returns>Lijst leden die in het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// de functie bepaald door <paramref name="functieID"/> hebben.</returns>
		public IList<Lid> OphalenUitFunctie(int functieID, int groepsWerkJaarID, params Expression<Func<Lid, object>>[] paths)
		{
			IList<Lid> result;
			using (var db = new ChiroGroepEntities())
			{
				var query = (from lid in db.Lid
							 where lid.Functie.Any(fnc => fnc.ID == functieID) &&
							lid.GroepsWerkJaar.ID == groepsWerkJaarID
							 select lid) as ObjectQuery<Lid>;
				result = (IncludesToepassen(query, paths)).ToList();
			}

			return Utility.DetachObjectGraph(result);
		}

		/// <summary>
		/// Haalt de leden op die in het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// de gepredefinieerde functie met type <paramref name="f"/> hebben.
		/// </summary>
		/// <param name="f">Type gepredefinieerde functie</param>
		/// <param name="groepsWerkJaarID">ID van een groepswerkjaar</param>
		/// <param name="paths">Bepaalt de mee op te halen gekoppelde entiteiten</param>
		/// <returns>Lijst leden die in het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// de gepredefinieerde functie met type <paramref name="f"/> hebben.</returns>
		public IList<Lid> OphalenUitFunctie(NationaleFunctie f, int groepsWerkJaarID, params Expression<Func<Lid, object>>[] paths)
		{
			return OphalenUitFunctie((int)f, groepsWerkJaarID, paths);
		}

		/// <summary>
		/// Geeft <c>true</c> indien het lid met <paramref name="lidID"/> leiding is, anders <c>false</c>
		/// </summary>
		/// <param name="lidID">ID van lid waarvoor na te gaan of het al dan niet leiding is</param>
		/// <returns><c>true</c> indien het lid met <paramref name="lidID"/> leiding is, anders <c>false</c></returns>
		public bool IsLeiding(int lidID)
		{
			using (var db = new ChiroGroepEntities())
			{
				Lid l = (from ld in db.Lid
						 where ld.ID == lidID
						 select ld).FirstOrDefault();
				return (l is Leiding);
			}
		}

		/// <summary>
		/// Haalt het lid op bepaald door <paramref name="gelieerdePersoonID"/> en
		/// <paramref name="groepsWerkJaarID"/>, inclusief persoon, afdelingen, functies, groepswerkjaar
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van de gelieerde persoon waarvoor het lidobject gevraagd is.</param>
		/// <param name="groepsWerkJaarID">ID van groepswerkjaar in hetwelke het lidobject gevraagd is</param>
		/// <returns>
		/// Het lid bepaald door <paramref name="gelieerdePersoonID"/> en
		/// <paramref name="groepsWerkJaarID"/>, inclusief persoon, afdelingen, functies, groepswerkjaar
		/// </returns>
		public Lid OphalenViaPersoon(int gelieerdePersoonID, int groepsWerkJaarID)
		{
			using (var db = new ChiroGroepEntities())
			{
				db.Lid.MergeOption = MergeOption.NoTracking;

				var lid = (
					from t in db.Lid
					where t.GelieerdePersoon.ID == gelieerdePersoonID
							&&
							t.GroepsWerkJaar.ID == groepsWerkJaarID
					select t).FirstOrDefault<Lid>();

				if (lid != null)
				{
					int lidID = lid.ID;

					if (lid is Kind)
					{
						return (
							from t in db.Lid.OfType<Kind>()
								.Include("GelieerdePersoon.Persoon")
								.Include("GroepsWerkJaar")
								.Include("AfdelingsJaar.Afdeling")
								.Include(knd => knd.Functie)
							where t.ID == lidID
							select t).FirstOrDefault<Kind>();
					}
					else if (lid is Leiding)
					{
						return (
							from t in db.Lid.OfType<Leiding>()
								.Include("GelieerdePersoon.Persoon")
								.Include("GroepsWerkJaar")
								.Include("AfdelingsJaar.Afdeling")
								.Include(leid => leid.Functie)
							where t.ID == lidID
							select t).FirstOrDefault<Leiding>();
					}
				}
				return lid;
			}
		}
	}
}
