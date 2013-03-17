using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;

namespace Chiro.Gap.WorkerInterfaces
{
	public interface IChiroGroepenManager
	{
		/// <summary>
		/// Maakt een nieuwe afdeling voor een Chirogroep, zonder te persisteren
		/// </summary>
		/// <param name="groep">
		/// Chirogroep waarvoor afdeling moet worden gemaakt, met daaraan gekoppeld
		/// de bestaande afdelingen
		/// </param>
		/// <param name="naam">
		/// Naam van de afdeling
		/// </param>
		/// <param name="afkorting">
		/// Handige afkorting voor in schemaatjes
		/// </param>
		/// <returns>
		/// De toegevoegde (maar nog niet gepersisteerde) afdeling
		/// </returns>
		/// <exception cref="GeenGavException">
		/// Komt voor als de gebruiker geen GAV is voor de opgegeven <paramref name="groep"/>
		/// </exception>
		Afdeling AfdelingToevoegen(ChiroGroep groep, string naam, string afkorting);
	}
}