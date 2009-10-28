using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Chiro.Gap.Orm;

namespace Cg2.ServiceContracts
{
    // NOTE: If you change the interface name "IGroepenService" here, you must also update the reference to "IGroepenService" in Web.config.
    [ServiceContract]
    public interface IGroepenService
    {
        /// <summary>
        /// Ophalen van de Groeps info
        /// </summary>
        /// <param name="GroepId"></param>
        /// <returns></returns>
        [OperationContract]
        GroepInfo OphalenInfo(int GroepId);

        /// <summary>
        /// Ophalen van de Groeps info
        /// </summary>
        /// <param name="GroepId"></param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<GroepInfo> OphalenMijnGroepen();

        /// <summary>
        /// Persisteert een groep in de database
        /// </summary>
        /// <param name="g">Te persisteren groep</param>
        /// <returns>De persoon met eventueel gewijzigde informatie</returns>
        /// <remarks>FIXME: gedetailleerde exception</remarks>
        [OperationContract]
        Groep Bewaren(Groep g);

        /// <summary>
        /// Haalt groep op uit database
        /// </summary>
        /// <param name="groepID">ID van op te halen groep</param>
        /// <returns>het gevraagde groepsobject, of null indien niet gevonden.
        /// </returns>
        [OperationContract]
        Groep Ophalen(int groepID);

        [OperationContract]
        Groep OphalenMetAdressen(int groepID);

        [OperationContract]
        Groep OphalenMetFuncties(int groepID);

        [OperationContract]
        Groep OphalenMetAfdelingen(int groepID);

        [OperationContract]
        Groep OphalenMetVrijeVelden(int groepID);

        /// <summary>
        /// Haalt GroepsWerkJaarID van het recentst gemaakte groepswerkjaar
        /// voor een gegeven groep op.
        /// </summary>
        /// <param name="groepID">GroepID van groep</param>
        /// <returns>ID van het recentste GroepsWerkJaar</returns>
        [OperationContract]
        int RecentsteGroepsWerkJaarIDGet(int groepID);

        // TODO: Hernoemen naar HuidigWerkJaarOphalen of iets
        // gelijkaardig.  Een groepswerkjaar is een combinatie
        // groep en werkjaar, en ik vermoed dat onderstaande functie
        // bedoeld is om een jaartal te retourneren.
        [OperationContract]
        int HuidigWerkJaarGet(int groepID);

        //alles om gelieerdepersonen op te halen zit in igelieerdepersonenservice

        [OperationContract]
        void AanmakenAfdeling(int groepID, string naam, string afkorting);

        /// <summary>
        /// Methode die aan de hand van een groep, een afdelingsjaar van de groep en een officiele afdeling een 
        /// afdelingsjaar maakt voor de gegeven leeftijden. 
        /// </summary>
        /// <param name="g">Deze moet gelinkt zijn met afdelingsjaar, officieleafdeling en afdeling</param>
        /// <param name="aj"></param>
        /// <param name="oa"></param>
        [OperationContract]
        void AanmakenAfdelingsJaar(Groep g, Afdeling aj, OfficieleAfdeling oa, int geboortejaarbegin, int geboortejaareind);

        [OperationContract]
        IList<OfficieleAfdeling> OphalenOfficieleAfdelingen();
        

        //TODO geef lijst met alle mogelijke afdelingen,
        //zorgen dat ge die lijst kunt aanpassen

        /*
            bivakorganiseren(g, b)
            stelGAVin
            verwijderGAV (JV: ik zou hier de 'VervalDatum' op nu instellen, zodat geregistreerd blijft dat iemand ooit gav was)
            maaknieuwesatelliet(g, s)
            afdelingsjaartoevoegen(g)
            afdelingenreorganiseren(g) (JV: bedoel je het bewaren van de afdelingen gekoppeld aan de groep?)
            afdelingsjaarverwijderen(g) (JV: kan enkel als er geen leden in dat afdelingsjaar zijn/waren)
            afdelingtoevoegen
         */

        #region categorieen
        [OperationContract]
        IList<GelieerdePersoon> PersonenOphalenUitCategorie(int categorieID);

        [OperationContract]
        Groep OphalenMetCategorieen(int groepID);

        //[OperationContract]
        //void CategorieToevoegen(Categorie c, int groepID);
        [OperationContract]
        void CategorieVerwijderen(int categorieID);
        //[OperationContract]
        //void CategorieAanpassen(Categorie c);

        //TODO moet hier eigenlijk een groep worden meegegeven, of kan die worden afgeleid uit de aanroeper?
        [OperationContract]
        void PersonenToevoegenAanCategorie(IList<int> gelieerdePersonenIDs, int categorieID);
        [OperationContract]
        void PersonenVerwijderenUitCategorie(IList<int> gelieerdePersonenIDs, int categorieID);

        #endregion categorieen

    }
}
