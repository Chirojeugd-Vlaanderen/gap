using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface IGroepenManager
    {
        /// <summary>
        /// Maakt een nieuwe categorie, en koppelt die aan een bestaande groep (met daaraan
        /// gekoppeld zijn categorieën)
        /// </summary>
        /// <param name="g">
        /// Groep waarvoor de categorie gemaakt wordt.  Als bestaande categorieën
        /// gekoppeld zijn, wordt op dubbels gecontroleerd
        /// </param>
        /// <param name="categorieNaam">
        /// Naam voor de nieuwe categorie
        /// </param>
        /// <param name="categorieCode">
        /// Code voor de nieuwe categorie
        /// </param>
        /// <returns>
        /// De toegevoegde categorie
        /// </returns>
        Categorie CategorieToevoegen(Groep g, string categorieNaam, string categorieCode);

        /// <summary>
        /// Maakt een nieuwe (groepseigen) functie voor groep <paramref name="g"/>.  Persisteert niet.
        /// </summary>
        /// <param name="g">
        /// Groep waarvoor de functie gemaakt wordt, inclusief minstens het recentste werkJaar
        /// </param>
        /// <param name="naam">
        /// Naam van de functie
        /// </param>
        /// <param name="code">
        /// Code van de functie
        /// </param>
        /// <param name="maxAantal">
        /// Maximumaantal leden in de categorie.  Onbeperkt indien null.
        /// </param>
        /// <param name="minAantal">
        /// Minimumaantal leden in de categorie.
        /// </param>
        /// <param name="lidType">
        /// LidType waarvoor de functie van toepassing is
        /// </param>
        /// <returns>
        /// De nieuwe (gekoppelde) functie
        /// </returns>
        Functie FunctieToevoegen(
            Groep g, 
            string naam, 
            string code, 
            int? maxAantal, 
            int minAantal, 
            LidType lidType);

    }
}