using Chiro.Gap.Orm;

namespace Chiro.Gap.WorkerInterfaces
{
	public interface IChiroGroepenManager
	{
		/// <summary>
		/// Haalt een groepsobject op
		/// </summary>
		/// <param name="groepID">
		/// ID van de op te halen groep
		/// </param>
		/// <param name="extras">
		/// Geeft aan of er gekoppelde entiteiten mee opgehaald moeten worden.
		/// </param>
		/// <returns>
		/// De Chirogroep met de opgegeven ID <paramref name="groepID"/>
		/// </returns>
		/// <exception cref="GeenGavException">
		/// Komt voor als de gebruiker geen GAV is voor de groep met de opgegeven <paramref name="groepID"/>
		/// </exception>
		ChiroGroep Ophalen(int groepID, ChiroGroepsExtras extras);

		/// <summary>
		/// Bewaart de Chirogroep <paramref name="chiroGroep"/>, met daaraan gekoppeld de
		/// gegeven <paramref name="extras"/>.
		/// </summary>
		/// <param name="chiroGroep">
		/// Te bewaren Chirogroep
		/// </param>
		/// <param name="extras">
		/// Bepaalt de mee te bewaren gekoppelde entiteiten
		/// </param>
		/// <returns>
		/// De Chirogroep met - als alles goed ging - de bijgewerkte waarden
		/// </returns>
		/// <exception cref="GeenGavException">
		/// Komt voor als de gebruiker geen GAV is voor de opgegeven <paramref name="chiroGroep"/>
		/// </exception>
		ChiroGroep Bewaren(ChiroGroep chiroGroep, ChiroGroepsExtras extras);

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