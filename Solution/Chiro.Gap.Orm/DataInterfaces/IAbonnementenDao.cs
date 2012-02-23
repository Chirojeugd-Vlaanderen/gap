// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
    /// <summary>
    /// Gegevenstoegangsobject voor abonnementen
    /// </summary>
    public interface IAbonnementenDao : IDao<Abonnement>
    {
        /// <summary>
        /// Haalt alle abonnementen op uit een gegeven groepswerkjaar, inclusief personen, voorkeursadressen, 
        /// groepswerkjaar en groep.
        /// </summary>
        /// <param name="gwjID">ID van het gegeven groepswerkjaar</param>
        /// <returns>
        /// Alle abonnementen op uit een gegeven groepswerkjaar, inclusief personen, voorkeursadressen, 
        /// groepswerkjaar en groep.
        /// </returns>
        IEnumerable<Abonnement> OphalenUitGroepsWerkJaar(int gwjID);
    }
}
