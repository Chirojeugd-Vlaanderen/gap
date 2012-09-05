using Chiro.Gap.Domain;
using Chiro.Gap.Orm;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface IGroepenManager
    {
        /// <summary>
        /// Verwijdert alle gelieerde personen van de groep met ID <paramref name="groepID"/>.  Probeert ook
        /// de gekoppelde personen te verwijderen, indien <paramref name="verwijderPersonen"/> <c>true</c> is.
        /// Verwijdert ook mogelijke lidobjecten.
        /// PERSISTEERT!
        /// </summary>
        /// <param name="groepID">
        /// ID van de groep waarvan je de gelieerde personen wilt verwijderen
        /// </param>
        /// <param name="verwijderPersonen">
        /// Indien <c>true</c>, worden ook de personen vewijderd waarvoor
        /// een GelieerdePersoon met de groep bestond.
        /// </param>
        /// <remarks>
        /// Deze functie vereist super-GAV-rechten
        /// </remarks>
        void GelieerdePersonenVerwijderen(int groepID, bool verwijderPersonen);

        /// <summary>
        /// Persisteert groep in de database
        /// </summary>
        /// <param name="g">
        /// Te persisteren groep
        /// </param>
        /// <returns>
        /// De bewaarde groep
        /// </returns>
        Groep Bewaren(Groep g);

        /// <summary>
        /// Haalt een groepsobject op zonder gerelateerde entiteiten
        /// </summary>
        /// <param name="groepID">
        /// ID van de op te halen groep
        /// </param>
        /// <returns>
        /// De groep met de opgegeven ID <paramref name="groepID"/>
        /// </returns>
        Groep Ophalen(int groepID);

        /// <summary>
        /// Haalt de groepen met gegeven <paramref name="groepIDs"/> op, zonder gekoppelde entiteiten.
        /// </summary>
        /// <param name="groepIDs">ID's van op te halen groepen</param>
        /// <returns>De opgehaalde groepen</returns>
        Groep[] Ophalen(int[] groepIDs);

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
        /// De groep met de opgegeven ID <paramref name="groepID"/>
        /// </returns>
        Groep Ophalen(int groepID, GroepsExtras extras);

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

        /// <summary>
        /// Maakt een nieuw groepswerkjaar voor een gegeven <paramref name="groep"/>
        /// </summary>
        /// <param name="groep">
        /// Groep waarvoor een groepswerkjaar gemaakt moet worden
        /// </param>
        /// <param name="werkJaar">
        /// Int die het werkJaar identificeert (bv. 2009 voor 2009-2010)
        /// </param>
        /// <returns>
        /// Het gemaakte groepswerkjaar.
        /// </returns>
        /// <remarks>
        /// Persisteert niet.
        /// </remarks>
        GroepsWerkJaar GroepsWerkJaarMaken(Groep groep, int werkJaar);

        /// <summary>
        /// Haalt groep op met gegeven stamnummer, incl recentse groepswerkjaar
        /// </summary>
        /// <param name="code">
        /// Stamnummer op te halen groep
        /// </param>
        /// <returns>
        /// Groep met <paramref name="code"/> als stamnummer
        /// </returns>
        Groep Ophalen(string code);
    }
}