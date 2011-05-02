using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
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
