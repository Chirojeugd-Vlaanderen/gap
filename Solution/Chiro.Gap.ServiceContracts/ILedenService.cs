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
        /// <returns>statusboodschap</returns>
        [OperationContract]
        String LidMakenEnBewaren(int gelieerdePersoonID);
        //Kind LidMaken(int gelieerdePersoonID);

        //[OperationContract]
        //Leiding LeidingMaken(int gelieerdePersoonID, IEnumerable<AfdelingsJaar> afdelingen);

        /// <summary>
        /// Om een lid aan te passen of te verwijderen uit het systeem. De gewone bewaar methode slaat altijd alleen
        /// veranderingen op aan het lid object zelf
        /// </summary>
        /// <param name="persoon"></param>
        [OperationContract]
        void Bewaren(Lid lid);

        /// <summary>
        /// Krijgt een lijst met afdelingsIDs en het lid. De bedoeling is dat het lid een afdelingsjaar moet hebben voor die
        /// afdelingsids en dat het voor andere afdelingsids verwijderd moet worden
        /// </summary>
        /// <param name="lidID"></param>
        /// <param name="afdelingsJaarIDs"></param>
        [OperationContract]
        void BewarenMetAfdelingen(int lidID, IList<int> afdelingsIDs);

        [OperationContract]
        void BewarenMetFuncties(Lid lid);

        [OperationContract]
        void BewarenMetVrijeVelden(Lid lid);

        [OperationContract]
        Boolean Verwijderen(int id);


        //TODO: we vragen leden op per groepswerkjaar. Waarom dit verschil met personen? Personen zijn altijd geldig, 
        //maar is dit wel de beste oplossing? Want alle leden zijn personen, maar wat dan als ze weggaan en dan terugkomen? 
        //Moeten ze dan expliciet gedelete worden?...?

        /// <summary>
        /// Haalt pagina met leden op uit bepaald groepswerkjaar
        /// </summary>
        /// <param name="groepsWerkJaarID">gevraagde groepswerkjaar</param>
        /// <returns>lijst met leden, inclusief info gelieerde personen
        /// en personen</returns>
        //[OperationContract]
        //IList<LidInfo> PaginaOphalen(int groepsWerkJaarID);

        /// <summary>
        /// Haalt een pagina met ledengegevens in een bepaald groepswerkjaar
        /// </summary>
        /// <param name="groepID">ID van het betreffende groepwerkjaar</param>
        /// <param name="pagina">paginanummer (1 of hoger)</param>
        /// <param name="paginaGrootte">aantal records per pagina (1 of meer)</param>
        /// <param name="aantalTotaal">outputparameter; geeft het totaal aantal personen weer in de lijst</param>
        /// <returns>lijst van leen met hun relevante informatie</returns>
        [OperationContract]
        IList<LidInfo> PaginaOphalen(int groepsWerkJaarID, int pagina, int paginaGrootte, out int aantalTotaal);

        [OperationContract]
        IList<LidInfo> PaginaOphalenVolgensCategorie(int categorieID, int groepsWerkJaarID, int pagina, int paginaGrootte, out int aantalTotaal);

        [OperationContract]
        IList<LidInfo> PaginaOphalenVoorAfdeling(int groepsWerkJaarID, int afdelingsID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lid"></param>
        [OperationContract]
        void LidOpNonactiefZetten(Lid lid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lid"></param>
        [OperationContract]
        void LidActiveren(Lid lid);

        /// <summary>
        /// Haalt lid inclusief afdelingsjaren, afdelingen en gelieerdepersoon
        /// </summary>
        /// <param name="lidID">ID op te halen lid</param>
        /// <returns>Lidinfo met afdelingsjaren, afdelingen en gelieerdepersoon</returns>
        [OperationContract]
        LidInfo LidOphalenMetAfdelingen(int lidID);
    }
}
