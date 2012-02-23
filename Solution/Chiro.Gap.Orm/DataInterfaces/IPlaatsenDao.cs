// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Linq.Expressions;

using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
    /// <summary>
    /// Interface voor data access wat betreft bivakplaatsen.
    /// </summary>
    public interface IPlaatsenDao : IDao<Plaats>
    {
        /// <summary>
        /// Zoekt naar een plaats, op basis van <paramref name="groepID"/>, <paramref name="plaatsNaam"/>
        /// en <paramref name="adresID"/>
        /// </summary>
        /// <param name="groepID">ID van groep die de plaats gemaakt moet hebben</param>
        /// <param name="plaatsNaam">Naam van de plaats</param>
        /// <param name="adresID">ID van het adres van de plaats</param>
        /// <param name="paths">Bepaalt wat er allemaal mee opgehaald moet worden</param>
        /// <returns>De gevonden groep, of <c>null</c> als niets werd gevonden</returns>
        Plaats Zoeken(int groepID, string plaatsNaam, int adresID, params Expression<Func<Plaats, object>>[] paths);
    }
}
