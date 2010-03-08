// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	public class LedenDao : Dao<Lid, ChiroGroepEntities>, ILedenDao
	{
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
			return Ophalen(gelieerdePersoonID, groepsWerkJaarID, null);
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
			using (ChiroGroepEntities db = new ChiroGroepEntities())
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

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.Lid.MergeOption = MergeOption.NoTracking;

				var kinderen = (
					from l in db.Lid.OfType<Kind>().Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling")
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
					orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
					select l).ToList<Kind>();

				var leiding = (
					from l in db.Lid.OfType<Leiding>().Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling")
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

		// pagineren gebeurt nu per werkjaar
		// pagina, paginaGrootte en aantalTotaal zijn niet meer nodig
		public IList<Lid> PaginaOphalen(int groepsWerkJaarID)
		{
			IList<Lid> lijst = new List<Lid>();

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.Lid.MergeOption = MergeOption.NoTracking;

				var kinderen = (
					from l in db.Lid.OfType<Kind>().Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling")
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
					orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
					select l).ToList<Kind>();

				var leiding = (
					from l in db.Lid.OfType<Leiding>().Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling")
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

		// pagineren gebeurt nu per werkjaar
		// pagina, paginaGrootte en aantalTotaal zijn niet meer nodig
		public IList<Lid> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID)
		{
			IList<Lid> lijst;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.Lid.MergeOption = MergeOption.NoTracking;

				var kinderen = (
					from l in db.Lid.OfType<Kind>().Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling")
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
						&&
					  l.AfdelingsJaar.Afdeling.ID == afdelingsID
					orderby l.GelieerdePersoon.Persoon.Naam, l.GelieerdePersoon.Persoon.VoorNaam
					select l).ToList<Kind>();

				var leiding = (
					from l in db.Lid.OfType<Leiding>().Include("GelieerdePersoon.Persoon").Include("AfdelingsJaar.Afdeling")
					where l.GroepsWerkJaar.ID == groepsWerkJaarID
						&&
					  l.AfdelingsJaar.Any(x => x.Afdeling.ID == afdelingsID)
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
		/// Lid met afdelingsjaren, afdelingen en gelieerdepersoon
		/// </summary>
		/// <param name="lidID">ID van het lid waarvan we gegevens willen opvragen</param>
		/// <returns>Een lid met afdelingsjaren, afdelingen en gelieerdepersoon</returns>
		public Lid OphalenMetDetails(int lidID)
		{
			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.Lid.MergeOption = MergeOption.NoTracking;

				Lid lid = (
			from t in db.Lid.Include("GelieerdePersoon.Persoon").Include("GroepsWerkJaar.AfdelingsJaar.Afdeling")
			where t.ID == lidID
			select t).FirstOrDefault<Lid>();

				if (lid is Kind)
				{
					return (
			from t in db.Lid.OfType<Kind>().Include("GelieerdePersoon.Persoon").Include("GroepsWerkJaar.AfdelingsJaar.Afdeling").Include("AfdelingsJaar.Afdeling")
			where t.ID == lidID
			select t).FirstOrDefault<Kind>();
				}
				else if (lid is Leiding)
				{
					return (
			from t in db.Lid.OfType<Leiding>().Include("GelieerdePersoon.Persoon").Include("GroepsWerkJaar.AfdelingsJaar.Afdeling").Include("AfdelingsJaar.Afdeling")
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
		/// <param name="paths">bepaalt de mee op te halen gekoppelde entiteiten</param>
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
		/// <param name="f">type gepredefinieerde functie</param>
		/// <param name="groepsWerkJaarID">ID van een groepswerkjaar</param>
		/// <param name="paths">bepaalt de mee op te halen gekoppelde entiteiten</param>
		/// <returns>Lijst leden die in het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// de gepredefinieerde functie met type <paramref name="f"/> hebben.</returns>
		public IList<Lid> OphalenUitFunctie(
			GepredefinieerdeFunctieType f,
			int groepsWerkJaarID,
			params Expression<Func<Lid, object>>[] paths)
		{
			return OphalenUitFunctie((int)f, groepsWerkJaarID, paths);
		}

	}
}
