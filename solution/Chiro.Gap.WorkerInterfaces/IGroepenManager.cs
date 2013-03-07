using Chiro.Cdf.Poco;
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
        /// Zoekt in de eigen functies de gegeven <paramref name="groep"/> en in de nationale functies een
        /// functie met gegeven <paramref name="code"/>.
        /// </summary>
        /// <param name="groep">Groep waarvoor functie gezocht moet worden</param>
        /// <param name="code">Code van de te zoeken functie</param>
        /// <param name="functieRepo"></param>
        Functie FunctieZoeken(Groep groep, string code, IRepository<Functie> functieRepo);

        /// <summary>
        /// Converteert een <paramref name="lidType"/> naar een niveau, gegeven het niveau van de
        /// groep (<paramref name="groepsNiveau"/>)
        /// </summary>
        /// <param name="lidType">Leden, Leiding of allebei</param>
        /// <param name="groepsNiveau">Plaatselijke groep, gewestploeg, verbondsploeg, satelliet</param>
        /// <returns>Niveau van het <paramref name="lidType"/> voor een groep met gegeven <paramref name="groepsNiveau"/></returns>
        Niveau LidTypeNaarMiveau(LidType lidType, Niveau groepsNiveau);
        
        /// <summary>
        /// Bepaalt het huidige groepswerkjaar van de gegeven <paramref name="groep"/>
        /// </summary>
        /// <param name="groep">De groep waarvoor het huidige werkjaar gevraagd is</param>
        /// <returns>Huidige groepswerkjaar van de groep</returns>
        GroepsWerkJaar HuidigWerkJaar(Groep groep);

        /// <summary>
        /// Geeft <c>true</c> als we in de live-omgeving werken
        /// </summary>
        /// <returns><c>true</c> als we in de live-omgeving werken</returns>
        bool IsLive();
    }
}