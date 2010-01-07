using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Chiro.Gap.Orm;

namespace Chiro.Gap.ServiceContracts
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
		GroepInfo InfoOphalen(int GroepId);

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

		/// <summary>
		/// Haalt de groep met gegeven ID op uit database, eventueel met zijn links naar de entities in de methodenaam
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

		[OperationContract]
		Groep OphalenMetCategorieen(int groepID);

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

		[OperationContract]
		IList<AfdelingInfo> AfdelingenOphalen(int groepswerkjaarID);

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
