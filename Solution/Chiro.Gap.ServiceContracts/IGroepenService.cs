using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts.FaultContracts;

namespace Chiro.Gap.ServiceContracts
{
	// NOTE: If you change the interface name "IGroepenService" here, you must also update the reference to "IGroepenService" in Web.config.
	[ServiceContract]
	public interface IGroepenService
	{
		/// <summary>
		/// Ophalen van Groepsinformatie
		/// </summary>
		/// <param name="groepID">GroepID van groep waarvan we de informatie willen opvragen</param>
		/// <param name="extras">Bitset, die aangeeft welke extra informatie er opgehaald moet worden</param>
		/// <returns>
		/// GroepInfo-structuur met de gevraagde informatie over de groep met id <paramref name="groepID"/>
		/// </returns>
		[OperationContract]
		GroepInfo Ophalen(int groepID, GroepsExtras extras);

		/// <summary>
		/// Ophalen van de Groeps info
		/// </summary>
		/// <param name="GroepId"></param>
		/// <returns></returns>
		[OperationContract]
		IEnumerable<GroepInfo> MijnGroepenOphalen();

		[OperationContract]
		IList<GroepsWerkJaar> WerkJarenOphalen(int groepsID);

		/// <summary>
		/// Persisteert een groep in de database
		/// </summary>
		/// <param name="g">Te persisteren groep</param>
		/// <returns>De persoon met eventueel gewijzigde informatie</returns>
		/// <remarks>FIXME: gedetailleerde exception</remarks>
		[OperationContract]
		Groep Bewaren(Groep g);

		#region ophalen

		[OperationContract]
		Groep OphalenMetAdressen(int groepID);

		[OperationContract]
		Groep OphalenMetFuncties(int groepID);

		[OperationContract]
		Groep OphalenMetVrijeVelden(int groepID);


		#endregion

		#region werkjaren

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

		#endregion

		#region afdelingen

		[OperationContract]
		void AfdelingAanmaken(int groepID, string naam, string afkorting);

		[OperationContract]
		AfdelingsJaar AfdelingsJaarOphalen(int afdelingsJaarID);

		/// <summary>
		/// Methode die aan de hand van een groep, een afdelingsjaar van de groep en een officiele afdeling een 
		/// afdelingsjaar maakt voor de gegeven leeftijden. 
		/// </summary>
		/// <param name="g">Deze moet gelinkt zijn met afdelingsjaar, officieleafdeling en afdeling</param>
		/// <param name="aj"></param>
		/// <param name="oa"></param>
		[OperationContract]
		void AfdelingsJaarAanmaken(int groepID, int afdID, int offafdID, int geboortejaarbegin, int geboortejaareind);

		/// <summary>
		/// Bewerkt een AfdelingsJaar: 
		/// andere OfficieleAfdeling en/of andere leeftijden
		/// </summary>
		/// <param name="afdID">AfdelingsJaarID</param>
		/// <param name="offafdID">OfficieleAfdelingsID</param>
		/// <param name="geboortVan">GeboorteJaarVan</param>
		/// <param name="geboortTot">GeboorteJaarTot</param>
		[OperationContract]
		void AfdelingsJaarBewarenMetWijzigingen(int afdID, int offafdID, int geboorteVan, int geboorteTot);

		/// <summary>
		/// Verwijdert een afdelingsjaar 
		/// en controleert of er geen leden in zitten.
		/// </summary>
		/// <param name="afdelingsJaarID"></param>
		[OperationContract]
		void AfdelingsJaarVerwijderen(int afdelingsJaarID);

		[OperationContract]
		IList<OfficieleAfdeling> OfficieleAfdelingenOphalen();

		/// <summary>
		/// Haat een afdeling op, op basis van <paramref name="afdelingID"/>
		/// </summary>
		/// <param name="afdelingID">ID van op te halen afdeling</param>
		/// <returns>de gevraagde afdeling</returns>
		[OperationContract]
		Afdeling AfdelingOphalen(int afdelingID);

		/// <summary>
		/// Haalt informatie op over alle actieve afdelingen in het groepswerkjaar met 
		/// ID <paramref name="groepsWerkJaarID"/>
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <returns>
		/// Informatie over alle actieve afdelingen in het groepswerkjaar met 
		/// ID <paramref name="groepsWerkJaarID"/>
		/// </returns>
		[OperationContract]
		IList<AfdelingInfo> AfdelingenOphalen(int groepsWerkJaarID);

		/// <summary>
		/// Haalt informatie op over de afdelingen van een groep die niet gebruikt zijn in een gegeven 
		/// groepswerkjaar, op basis van een <paramref name="groepsWerkJaarID"/>
		/// </summary>
		/// <param name="groepswerkjaarID">ID van het groepswerkjaar waarvoor de niet-gebruikte afdelingen
		/// opgezocht moeten worden.</param>
		/// <returns>info de ongebruikte afdelingen van een groep in het gegeven groepswerkjaar</returns>
		[OperationContract]
		IList<AfdelingInfo> OngebruikteAfdelingenOphalen(int groepswerkjaarID);

		#endregion

		#region categorieen

		/// <summary>
		/// Voegt een categorie toe aan de groep
		/// </summary>
		/// <param name="groepID">De groep waaraan het wordt toegevoegd</param>
		/// <param name="naam">De naam van de nieuwe categorie</param>
		/// <param name="code">code voor de nieuwe categorie</param>
		/// <returns>De ID van de aangemaakte categorie</returns>
		[OperationContract]
		[FaultContract(typeof(BestaatAlFault))]
		int CategorieToevoegen(int groepID, String naam, String code);

		/// <summary>
		/// Verwijdert de gegeven categorie
		/// </summary>
		/// <param name="categorieID">De ID van de te verwijderen categorie</param>
		[OperationContract]
		void CategorieVerwijderen(int categorieID);

		/// <summary>
		/// Het veranderen van de naam van een categorie
		/// </summary>
		/// <param name="categorieID">De ID van de categorie</param>
		/// <param name="nieuwenaam">De nieuwe naam van de categorie</param>
		/// <exception cref="invalidoperation">Gegooit als de naam al bestaat, leeg is of null is</exception>
		[OperationContract]
		void CategorieAanpassen(int categorieID, string nieuwenaam);

		/// <summary>
		/// Zoekt de categorieID op van de categorie bepaald door de gegeven 
		/// <paramref name="groepID"/> en <paramref name="code"/>.
		/// </summary>
		/// <param name="groepID">ID van groep waaraan de gezochte categorie gekoppeld is</param>
		/// <param name="code">code van de te zoeken categorie</param>
		/// <returns>Het categorieID als de categorie gevonden is, anders 0.</returns>
		int CategorieIDOphalen(int groepID, string code);

		#endregion categorieen

		/*TODO
            bivakorganiseren(g, b)
            stelGAVin
            verwijderGAV (JV: ik zou hier de 'VervalDatum' op nu instellen, zodat geregistreerd blijft dat iemand ooit gav was)
            maaknieuwesatelliet(g, s)
            afdelingenreorganiseren(g) (JV: bedoel je het bewaren van de afdelingen gekoppeld aan de groep?)
            afdelingsjaarverwijderen(g) (JV: kan enkel als er geen leden in dat afdelingsjaar zijn/waren)
         */


		#region adressen
		[OperationContract]
		IEnumerable<GemeenteInfo> GemeentesOphalen();

		/// <summary>
		/// Haalt alle straten op uit een gegeven <paramref name="postNr"/>, waarvan de naam begint
		/// met het gegeven <paramref name="straatBegin"/>.
		/// </summary>
		/// <param name="straatBegin">Eerste letters van de te zoeken straatnamen</param>
		/// <param name="postNr">Postnummer waarin te zoeken</param>
		/// <returns>Gegevens van de gevonden straten</returns>
		[OperationContract]
		IEnumerable<StraatInfo> StratenOphalen(String straatBegin, int postNr);

		/// <summary>
		/// Haalt alle straten op uit een gegeven rij <paramref name="postNrs"/>, waarvan de naam begint
		/// met het gegeven <paramref name="straatBegin"/>.
		/// </summary>
		/// <param name="straatBegin">Eerste letters van de te zoeken straatnamen</param>
		/// <param name="postNrs">Postnummers waarin te zoeken</param>
		/// <returns>Gegevens van de gevonden straten</returns>
		/// <remarks>Ik had deze functie ook graag StratenOphalen genoemd, maar je mag geen 2 
		/// WCF-functies met dezelfde naam in 1 service hebben.  Spijtig.</remarks>
		[OperationContract]
		IEnumerable<StraatInfo> StratenOphalenMeerderePostNrs(String straatBegin, IEnumerable<int> postNrs);

		#endregion
	}
}
