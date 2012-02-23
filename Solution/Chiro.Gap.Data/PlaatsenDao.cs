// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
    /// <summary>
    /// Gegevenstoegangsobject voor plaatsen
    /// </summary>
    class PlaatsenDao : Dao<Plaats, ChiroGroepEntities>, IPlaatsenDao
    {
        /// <summary>
        /// Zoekt een plaats op basis van een aantal gegevens
        /// </summary>
        /// <param name="groepID">De ID van de groep die de plaats opzoekt</param>
        /// <param name="plaatsNaam">De naam van de gemeente</param>
        /// <param name="adresID">De ID van het adres</param>
        /// <param name="paths">De extra gegevens die opgehaald moeten worden</param>
        /// <returns>Ofwel <c>null</c>, ofwel de gevonden Plaats</returns>
        public Plaats Zoeken(int groepID, string plaatsNaam, int adresID, params Expression<Func<Plaats, object>>[] paths)
        {
            Plaats result;

            using (var db = new ChiroGroepEntities())
            {
                var query = (from p in db.Plaats
                             where p.Groep.ID == groepID && string.Equals(p.Naam, plaatsNaam) && p.Adres.ID == adresID
                             select p) as ObjectQuery<Plaats>;
                query = IncludesToepassen(query, paths);

                result = query.FirstOrDefault();
            }
            return result == null ? null : Utility.DetachObjectGraph(result);
        }
    }
}
