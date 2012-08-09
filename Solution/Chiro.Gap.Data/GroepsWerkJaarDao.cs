// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
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
	/// Gegevenstoegangsobject voor GroepsWerkJaren
	/// </summary>
	public class GroepsWerkJaarDao : Dao<GroepsWerkJaar, ChiroGroepEntities>, IGroepsWerkJaarDao
	{
		/// <summary>
		/// Haalt recentste groepswerkjaar op van groep met ID <paramref name="groepID"/>, inclusief 
		/// afdelingsjaren.
		/// </summary>
		/// <param name="groepID">ID van groep waarvan het recentste groepswerkjaar gevraagd is.</param>
		/// <param name="paths">Lambda-expressies die bepalen welke gekoppelde entiteiten mee opgehaald moeten worden.</param>
		/// <returns>Groepswerkjaar van groep met ID <paramref name="groepID"/>, met daaraan gekoppeld de
		/// groep en de afdelingsjaren.</returns>
		public GroepsWerkJaar RecentsteOphalen(int groepID, params Expression<Func<GroepsWerkJaar, object>>[] paths)
		{
			GroepsWerkJaar result;
			using (var db = new ChiroGroepEntities())
			{
				var query = (
					from wj in db.GroepsWerkJaar
					where wj.Groep.ID == groepID
					orderby wj.WerkJaar descending
					select wj) as ObjectQuery<GroepsWerkJaar>;

				query = IncludesToepassen(query, paths);

				result = query.FirstOrDefault();
			}
			result = Utility.DetachObjectGraph(result);

			return result;
		}

		/// <summary>
		/// Haalt het recentste groepswerkjaar op van de groep waar de gelieerde persoon <paramref name="gp"/>
		/// aan gekoppeld is.
		/// </summary>
		/// <param name="gp">Gelieerde persoon met op te halen groepswerkjaar</param>
		/// <param name="paths">Bepaalt de mee op te halen gekoppelde entiteiten</param>
		/// <returns>Het gevraagde groepswerkjaar met de gevraagde gekoppelde entiteiten</returns>
		public GroepsWerkJaar RecentsteOphalen(GelieerdePersoon gp, params Expression<Func<GroepsWerkJaar, object>>[] paths)
		{
			GroepsWerkJaar result;
			using (var db = new ChiroGroepEntities())
			{
				var query = (
					from wj in db.GroepsWerkJaar
					where wj.Groep.GelieerdePersoon.Any(gelp => gelp.ID == gp.ID)
					orderby wj.WerkJaar descending
					select wj) as ObjectQuery<GroepsWerkJaar>;

				query = IncludesToepassen(query, paths);

				result = query.FirstOrDefault();
			}
			result = Utility.DetachObjectGraph(result);

			return result;
		}

		/// <summary>
		/// Kijkt na of het groepswerkjaar met ID <paramref name="groepsWerkJaarID"/> het recentste groepswerkjaar
		/// van zijn groep is.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <returns><c>True</c> alss het groepswerkjaar het recentste is</returns>
		public bool IsRecentste(int groepsWerkJaarID)
		{
			int recentsteID;

			using (var db = new ChiroGroepEntities())
			{
				var gwjQuery = (from g in db.Groep
				             where g.GroepsWerkJaar.Any(gwj => gwj.ID == groepsWerkJaarID)
				             select g.GroepsWerkJaar).FirstOrDefault();

				recentsteID = (from gwj in gwjQuery
				                      orderby gwj.WerkJaar descending
				                      select gwj.ID).FirstOrDefault();
			}
			return recentsteID == groepsWerkJaarID;
		}

        /// <summary>
        /// Bewaart het gegeven <paramref name="groepsWerkJaar"/>, samen met de gekoppelde entiteiten
        /// bepaald door <paramref name="groepsWerkJaarExtras"/>.
        /// </summary>
        /// <param name="groepsWerkJaar">Te bewaren groepswerkjaar</param>
        /// <param name="groepsWerkJaarExtras">Bepaalt welke entiteiten mee bewaard moeten worden</param>
        /// <returns>Bewaarde groepswerkjaar en gekoppelde entiteiten.</returns>
	    public GroepsWerkJaar Bewaren(GroepsWerkJaar groepsWerkJaar, GroepsWerkJaarExtras groepsWerkJaarExtras)
	    {
	        if (groepsWerkJaarExtras.HasFlag(GroepsWerkJaarExtras.AlleAfdelingen))
	        {
	            throw new NotSupportedException();
	        }
            return Bewaren(groepsWerkJaar, ExtrasNaarLambdas(groepsWerkJaarExtras));
	    }

        /// <summary>
        /// Haalt het groepswerkjaar met gegeven <paramref name="groepsWerkJaarID"/> op, samen met de
        /// gekoppelde entiteiten bepaald door <paramref name="extras"/>.
        /// </summary>
        /// <param name="groepsWerkJaarID">ID op te halen groepswerkjaar</param>
        /// <param name="extras">bepaalt de mee op te halen gekoppelde entiteiten</param>
        /// <returns>Het groepswerkjaar met de gevraagde koppelingen</returns>
	    public GroepsWerkJaar Ophalen(int groepsWerkJaarID, GroepsWerkJaarExtras extras)
	    {
            GroepsWerkJaar result;

            using (var db = new ChiroGroepEntities())
            {

                var query = (from g in db.GroepsWerkJaar
                             where g.ID == groepsWerkJaarID
                             select g) as ObjectQuery<GroepsWerkJaar>;

                // eerst de gemakkelijke includes

                var paths = ExtrasNaarLambdas(extras);
                result = IncludesToepassen(query, paths).FirstOrDefault();

                // Nu nog de speciale includes

                if (extras.HasFlag(GroepsWerkJaarExtras.AlleAfdelingen))
                {
                    AlleAfdelingenKoppelen(db, result);
                }
            }

            return Utility.DetachObjectGraph(result);
	    }

        /// <summary>
        /// Haalt het recenste groepswerkjaar van de groep met gegeven <paramref name="groepID"/> op,
        /// samen met de gekoppelde entiteiten bepaald door <paramref name="extras"/>.
        /// </summary>
        /// <param name="groepID">ID van groep van op te halen groepswerkjaar</param>
        /// <param name="extras">bepaalt de mee op te halen gekoppelde entiteiten</param>
        /// <returns>De gevraagde groep met de gevraagde gekoppelde entiteiten</returns>
	    public GroepsWerkJaar RecentsteOphalen(int groepID, GroepsWerkJaarExtras extras)
	    {
            GroepsWerkJaar result;
            using (var db = new ChiroGroepEntities())
            {
                var query = (
                    from wj in db.GroepsWerkJaar
                    where wj.Groep.ID == groepID
                    orderby wj.WerkJaar descending
                    select wj) as ObjectQuery<GroepsWerkJaar>;

                // Eerst doen we de makkelijke dingen:
                query = IncludesToepassen(query, ExtrasNaarLambdas(extras));

                result = query.FirstOrDefault();

                if (extras.HasFlag(GroepsWerkJaarExtras.AlleAfdelingen))
                {
                    AlleAfdelingenKoppelen(db, result);
                }
            }
            result = Utility.DetachObjectGraph(result);

            return result;
	    }

	    /// <summary>
        /// Als <paramref name="groepsWerkJaar"/> een groepswerkjaar van een Chirogroep is, dan haalt deze method
        /// alle afdelingen van die Chirogroep op.
        /// </summary>
        /// <param name="db">Datacontext waaraan <paramref name="groepsWerkJaar"/> gekoppeld moet zijn</param>
        /// <param name="groepsWerkJaar">het groepswerkjaar</param>
        private void AlleAfdelingenKoppelen(ChiroGroepEntities db, GroepsWerkJaar groepsWerkJaar)
        {
            var query = from afd in db.Afdeling.Include(a => a.ChiroGroep)
                        where afd.ChiroGroep.ID == groepsWerkJaar.Groep.ID
                        select afd;

            query.ToArray(); // Instantieer (en koppel)
        }

	    /// <summary>
        /// Converteert de GroepsWerkJaarExtras <paramref name="extras"/> naar lambda-expressies die mee naar 
        /// de data access moeten om de extra's daadwerkelijk op te halen.
        /// </summary>
        /// <param name="extras">
        /// Te converteren groepsextra's
        /// </param>
        /// <returns>
        /// Lambda-expressies geschikt voor onze DAO's
        /// </returns>
        private static Expression<Func<GroepsWerkJaar, object>>[] ExtrasNaarLambdas(GroepsWerkJaarExtras extras)
        {
            var paths = new List<Expression<Func<GroepsWerkJaar, object>>>();

            if (extras.HasFlag(GroepsWerkJaarExtras.Afdelingen))
            {
                paths.Add(gwj => gwj.AfdelingsJaar.First().Afdeling);
                paths.Add(gwj => gwj.AfdelingsJaar.First().OfficieleAfdeling.WithoutUpdate());
            }
            if (extras.HasFlag(GroepsWerkJaarExtras.LidFuncties))
            {
                paths.Add(gwj => gwj.Lid.First().Functie);
                paths.Add(gwj => gwj.AfdelingsJaar.First().Kind.First().Functie);
                paths.Add(gwj => gwj.AfdelingsJaar.First().Leiding.First().Functie);
            }
            else if (extras.HasFlag(GroepsWerkJaarExtras.Leden))
            {
                paths.Add(gwj => gwj.AfdelingsJaar.First().Kind);
                paths.Add(gwj => gwj.AfdelingsJaar.First().Leiding);
            }

            if (extras.HasFlag(GroepsWerkJaarExtras.GroepsFuncties))
            {
                paths.Add(gwj => gwj.Groep.Functie);
            }
            else if (extras.HasFlag(GroepsWerkJaarExtras.Groep) || extras.HasFlag(GroepsWerkJaarExtras.AlleAfdelingen))
            {
                // Ook als we straks alle afdelingen gana ophalen, hebben we de groep nodig.
                paths.Add(gwj => gwj.Groep);
            }

            return paths.ToArray();
        }
	}
}
