// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
    public interface IDeelnemersDao : IDao<Deelnemer>
    {
        /// <summary>
        /// Verwijdert de gegeven <paramref name="deelnemer"/> uit de database.
        /// </summary>
        /// <param name="deelnemer">Te verwijderen deelnemer</param>
        void Verwijderen(Deelnemer deelnemer);
    }
}
