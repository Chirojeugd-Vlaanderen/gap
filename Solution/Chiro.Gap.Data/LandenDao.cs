// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Linq;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
    /// <summary>
    /// Repository voor landen
    /// </summary>
    class LandenDao : Dao<Land, ChiroGroepEntities>, ILandenDao
    {
        /// <summary>
        /// Haalt een land op, op basis van zijn naam
        /// </summary>
        /// <param name="landNaam">Naam van het land</param>
        /// <returns>Opgehaald land</returns>
        public Land Ophalen(string landNaam)
        {
            Land resultaat = null;

            using (var db = new ChiroGroepEntities())
            {
                resultaat = (
                    from Land l in db.Land
                    where l.Naam == landNaam
                    select l).FirstOrDefault();

                if (resultaat != null)
                {
                    db.Detach(resultaat);
                }
            }

            return resultaat;
        }
    }
}
