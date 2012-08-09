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
	/// Gegevenstoegangsobject voor Chirogroepen
	/// </summary>
	public class ChiroGroepenDao : Dao<ChiroGroep, ChiroGroepEntities>, IChiroGroepenDao
	{
		/// <summary>
		/// Haalt de ChiroGroep op met de gegeven ID
		/// </summary>
		/// <param name="id">ID van de Chirogroep in kwestie</param>
		/// <returns>De ChiroGroep met de gegeven ID</returns>
		ChiroGroep IDao<ChiroGroep>.Ophalen(int id)
		{
			ChiroGroep result;

			using (var db = new ChiroGroepEntities())
			{
				result = (
					from g in db.Groep.OfType<ChiroGroep>()
					where g.ID == id
					select g).FirstOrDefault();

				db.Detach(result);
			}
			return result;
		}

		/// <summary>
		/// Haalt alle Chirogroepen op
		/// </summary>
		/// <returns>Een lijst met alle Chirogroepen</returns>
		IList<ChiroGroep> IDao<ChiroGroep>.AllesOphalen()
		{
			List<ChiroGroep> result;

			using (var db = new ChiroGroepEntities())
			{
				db.Groep.MergeOption = MergeOption.NoTracking;

				result = (
					from g in db.Groep.OfType<ChiroGroep>()
					select g).ToList();
			}
			return result;
		}

        /// <summary>
        /// Converteert ChiroGroepsExtras <paramref name="extras"/> naar lambda-expresses voor een
        /// ChiroGroepenDao
        /// </summary>
        /// <param name="extras">
        /// Te converteren Chirogroepsextras
        /// </param>
        /// <returns>
        /// Lambda-expresses voor een KindDao
        /// </returns>
        private static Expression<Func<ChiroGroep, object>>[] ExtrasNaarLambdas(ChiroGroepsExtras extras)
        {
            var paths = new List<Expression<Func<ChiroGroep, object>>>();

            if (extras.HasFlag(ChiroGroepsExtras.GroepsWerkJaren))
            {
                // Withoutupdate-truuk om te vermijden dat groepswerkjaar overschreven
                // wordt.
                paths.Add(gr => gr.GroepsWerkJaar.First().WithoutUpdate());
            }

            if (extras.HasFlag(ChiroGroepsExtras.AlleAfdelingen))
            {
                paths.Add(gr => gr.Afdeling);
            }

            if (extras.HasFlag(ChiroGroepsExtras.Categorieen))
            {
                paths.Add(gr => gr.Categorie);
            }

            if (extras.HasFlag(ChiroGroepsExtras.Functies))
            {
                paths.Add(gr => gr.Functie);
            }

            return paths.ToArray();
        }

        /// <summary>
        /// Bewaart de gegeven <paramref name="chiroGroep"/>, samen met de koppelingen bepaald in
        /// <paramref name="extras"/>
        /// </summary>
        /// <param name="chiroGroep">Te bewaren Chirogroep</param>
        /// <param name="extras">Te bewaren koppelingen</param>
        /// <returns>Bewaarde Chirogroep met koppelingen uit <paramref name="extras"/></returns>
	    public ChiroGroep Bewaren(ChiroGroep chiroGroep, ChiroGroepsExtras extras)
	    {
            if (extras.HasFlag(ChiroGroepsExtras.HuidigWerkJaar))
            {
                throw new NotSupportedException();
            }

            return Bewaren(chiroGroep, ExtrasNaarLambdas(extras));
	    }

        /// <summary>
        /// Haalt de Chirogroep met ID <paramref name="groepID"/> op, samen met de gekoppelde
        /// entiteiten bepaald door <paramref name="extras"/>
        /// </summary>
        /// <param name="groepID">ID op te halen Chirogroep</param>
        /// <param name="extras">Bepaalt mee op te halen entiteiten</param>
        /// <returns>De gevraagde Chirogroep met de gevraagde gekoppelde entiteiten</returns>
        public ChiroGroep Ophalen(int groepID, ChiroGroepsExtras extras)
        {
            ChiroGroep result;

            using (var db = new ChiroGroepEntities())
            {

                var query = (from g in db.Groep.OfType<ChiroGroep>()
                             where g.ID == groepID
                             select g) as ObjectQuery<ChiroGroep>;

                // eerst de gemakkelijke includes

                var paths = ExtrasNaarLambdas(extras);

                result = IncludesToepassen(query, paths).FirstOrDefault();

                // Nu nog de speciale includes

                if (!extras.HasFlag(ChiroGroepsExtras.GroepsWerkJaren) && extras.HasFlag(ChiroGroepsExtras.HuidigWerkJaar))
                {
                    // Als we het huidige groepswerkjaar moeten ophalen, maar niet alle groepswerkjaren,
                    // dan hebben we nog werk.

                    HuidigWerkJaarKoppelen(db, result);
                }

            }

            return Utility.DetachObjectGraph(result);
        }

        /// <summary>
        /// Koppelt het recentste werkjaar aan een <paramref name="chiroGroep"/>, geattacht aan de
        /// datacontext <paramref name="db"/>.
        /// </summary>
        /// <param name="db">Datacontext</param>
        /// <param name="chiroGroep">chirogroep waarvan huidig werkjaar op te vragen is</param>
	    private void HuidigWerkJaarKoppelen(ChiroGroepEntities db, ChiroGroep chiroGroep)
        {
            var query = (from gwj in db.GroepsWerkJaar.Include(gwj => gwj.Groep)
                         where gwj.Groep.ID == chiroGroep.ID
                         orderby gwj.WerkJaar
                         select gwj);
            query.LastOrDefault();  // instantieer
        }
	}
}
