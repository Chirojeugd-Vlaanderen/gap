// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
    /// <summary>
    /// Interface voor de repository voor landen
    /// </summary>
    public interface ILandenDao : IDao<Land>
    {
        /// <summary>
        /// Haalt een land op, op basis van zijn naam
        /// </summary>
        /// <param name="landNaam">Naam van het land</param>
        /// <returns>Opgehaald land</returns>
        Land Ophalen(string landNaam);
    }
}
