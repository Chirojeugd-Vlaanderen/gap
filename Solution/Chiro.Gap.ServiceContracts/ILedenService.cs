using System.Collections.Generic;
using System;
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
        /// de eigenschappen die overeenkomen met zijn leeftijd, maar er moet wel de juiste methode worden
        /// aangeroepen om een kind of een leiding te maken
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van de gelieerde persoon</param>
        /// <returns>De ids van de personen die lid zijn gemaakt</returns>
		/// <throws>OngeldigeActieException als om een of andere reden, minstens 1 van de personen geen lid gemaakt 
		/// kan worden. In dat geval wordt geen enkele persoon lid. </throws>
		/// <remarks>De methode is reentrant, dus zal niet klagen als er personen al lid zijn.</remarks>
        [OperationContract]
		IEnumerable<int> LedenMakenEnBewaren(IEnumerable<int> gelieerdePersoonIDs);
		[OperationContract]
		IEnumerable<int> LeidingMakenEnBewaren(IEnumerable<int> gelieerdePersoonIDs);

        /// <summary>
        /// Om een lid aan te passen of te verwijderen uit het systeem. De gewone bewaar methode slaat altijd alleen
        /// veranderingen op aan het lid object zelf
        /// </summary>
        /// <param name="persoon"></param>
        [OperationContract]
        void Bewaren(LidInfo lid);

        /// <summary>
        /// Krijgt een lijst met afdelingsIDs en het lid. De bedoeling is dat het lid een afdelingsjaar moet hebben voor die
        /// afdelingsids en dat het voor andere afdelingsids verwijderd moet worden
        /// </summary>
        /// <param name="lidID"></param>
        /// <param name="afdelingsJaarIDs"></param>
        [OperationContract]
        void BewarenMetAfdelingen(int lidID, IList<int> afdelingsIDs);

        [OperationContract]
		void BewarenMetFuncties(LidInfo lid);

        [OperationContract]
		void BewarenMetVrijeVelden(LidInfo lid);

        /// <summary>
        /// TODO wat moet deze methode juist doen (nonactief maken of verwijderen als er nog niet lang in)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Boolean Verwijderen(int id);


        //TODO: we vragen leden op per groepswerkjaar. Waarom dit verschil met personen? Personen zijn altijd geldig, 
        //maar is dit wel de beste oplossing? Want alle leden zijn personen, maar wat dan als ze weggaan en dan terugkomen? 
        //Moeten ze dan expliciet gedelete worden?...?

        /// <summary>
        /// Haalt een pagina met ledengegevens in een bepaald groepswerkjaar
        /// </summary>
        /// <param name="groepID">ID van het betreffende groepwerkjaar</param>
        /// <param name="paginas">Het totaal aantal pagians</param>
        /// <returns>lijst van leen met hun relevante informatie</returns>
        [OperationContract]
        IList<LidInfo> PaginaOphalen(int groepsWerkJaarID, out int paginas);

		/// <summary>
		/// Haalt een pagina met ledengegevens in een bepaald groepswerkjaar, maar alleen leden uit de gegeven afdeling
		/// </summary>
		/// <param name="groepID">ID van het betreffende groepwerkjaar</param>
		/// <param name="afdelingID">ID van de betreffende afdeling</param>
		/// <param name="paginas">Het totaal aantal pagians</param>
		/// <returns>lijst van leen met hun relevante informatie</returns>
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
