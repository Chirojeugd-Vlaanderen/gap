using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.Orm;

namespace Cg2.ServiceContracts
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
        /// <returns>nieuw lidobject</returns>
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

        [OperationContract]
        void BewarenMetAfdelingen(Lid lid);

        [OperationContract]
        void BewarenMetFuncties(Lid lid);

        [OperationContract]
        void BewarenMetVrijeVelden(Lid lid);

        [OperationContract]
        Boolean Verwijderen(int id);


        /// <summary>
        /// Haalt pagina met leden op uit bepaald groepswerkjaar
        /// </summary>
        /// <param name="groepsWerkJaarID">gevraagde groepswerkjaar</param>
        /// <returns>lijst met leden, inclusief info gelieerde personen
        /// en personen</returns>
        [OperationContract]
        IList<LidInfo> PaginaOphalen(int groepsWerkJaarID);

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
    }
}
