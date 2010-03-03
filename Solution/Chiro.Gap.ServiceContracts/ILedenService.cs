// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using Chiro.Gap.Orm;

namespace Chiro.Gap.ServiceContracts
{
    // NOTE: If you change the interface name "ILedenService" here, you must also update the reference to "ILedenService" in Web.config.

    [ServiceContract]
    public interface ILedenService
    {
        /// <summary>
        /// Gaat een gelieerde persoon ophalen en maakt die lid in het huidige werkjaar. Hiervoor krijgt het
        /// de eigenschappen die overeenkomen met zijn/haar leeftijd, maar er moet wel de juiste methode worden
        /// aangeroepen om een kind of een leiding te maken
        /// </summary>
        /// <param name="gelieerdePersoonIDs">ID's van de gelieerde persoon</param>
        /// <returns>De ids van de personen die lid zijn gemaakt</returns>
		/// <throws>OngeldigeActieException als om een of andere reden, minstens 1 van de personen geen lid gemaakt 
		/// kan worden. In dat geval wordt geen enkele persoon lid. </throws>
		/// <remarks>De methode is reentrant, dus zal niet klagen als er personen al lid zijn.</remarks>
        [OperationContract]
		IEnumerable<int> LedenMakenEnBewaren(IEnumerable<int> gelieerdePersoonIDs);

		/// <summary>
		/// </summary>
		/// <param name="gelieerdePersoonIDs"></param>
		/// <returns></returns>
		[OperationContract]
		IEnumerable<int> LeidingMakenEnBewaren(IEnumerable<int> gelieerdePersoonIDs);

        /// <summary>
		/// Slaat veranderingen op aan de eigenschappen van het lidobject zelf. Creëert of verwijdert geen leden, en leden
		/// kunnen ook niet van werkjaar of van gelieerdepersoon veranderen.
        /// </summary>
        /// <param name="lid"></param>
		/// <returns></returns>
        [OperationContract]
        LidInfo Bewaren(LidInfo lid);

        /// <summary>
		/// Slaat veranderingen op aan de eigenschappen van het lidobject zelf. Creëert of verwijdert geen leden, en leden
		/// kunnen ook niet van werkjaar of van gelieerdepersoon veranderen. Ook de afdelingen worden aangepast.
        /// </summary>
		/// <param name="lid"></param>
		/// <returns></returns>
        [OperationContract]
        LidInfo BewarenMetAfdelingen(LidInfo lid);

		/// <summary>
		/// </summary>
		/// <param name="lid"></param>
		/// <returns></returns>
        [OperationContract]
		LidInfo BewarenMetFuncties(LidInfo lid);

		/// <summary>
		/// </summary>
		/// <param name="lid"></param>
		/// <returns></returns>
        [OperationContract]
		LidInfo BewarenMetVrijeVelden(LidInfo lid);

        /// <summary>
        /// TODO wat moet deze methode juist doen (nonactief maken of verwijderen als er nog niet lang in)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Boolean Verwijderen(int id);

        // TODO: we vragen leden op per groepswerkjaar. Waarom dit verschil met personen? Personen zijn altijd geldig, 
        // maar is dit wel de beste oplossing? Want alle leden zijn personen, maar wat dan als ze weggaan en dan terugkomen? 
        // Moeten ze dan expliciet gedeletet worden?...?

        /// <summary>
        /// Haalt een pagina met ledengegevens in een bepaald groepswerkjaar
        /// </summary>
        /// <param name="groepID">ID van het betreffende groepwerkjaar</param>
        /// <param name="paginas">Het totaal aantal pagians</param>
        /// <returns>Lijst van leden met hun relevante informatie</returns>
        [OperationContract]
        IList<LidInfo> PaginaOphalen(int groepsWerkJaarID, out int paginas);

		/// <summary>
		/// Haalt een pagina met ledengegevens in een bepaald groepswerkjaar, maar alleen leden uit de gegeven afdeling
		/// </summary>
		/// <param name="groepID">ID van het betreffende groepwerkjaar</param>
		/// <param name="afdelingID">ID van de betreffende afdeling</param>
		/// <param name="paginas">Het totaal aantal pagians</param>
		/// <returns>Lijst van leen met hun relevante informatie</returns>
        [OperationContract]
        IList<LidInfo> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID, out int paginas);

        /// <summary>
        /// Haalt lid inclusief afdelingsjaren, afdelingen en gelieerdepersoon
        /// </summary>
        /// <param name="lidID">ID op te halen lid</param>
        /// <returns>Lidinfo met afdelingsjaren, afdelingen en gelieerdepersoon</returns>
        [OperationContract]
        LidInfo LidOphalenMetAfdelingen(int lidID);
    }
}
