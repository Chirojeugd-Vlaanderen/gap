// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Chiro.Cdf.Data;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Orm.DataInterfaces
{
	/// <summary>
	/// Interface voor een gegevenstoegangsobject voor GelieerdePersonen
	/// </summary>
	/// <remarks>
	/// Met een GelieerdePersoon moet vrijwel altijd het geassocieerde
	/// persoonsobject meekomen, anders heeft het weinig zin.
	/// </remarks>
	public interface IGelieerdePersonenDao : IDao<GelieerdePersoon>
	{
		/// <summary>
		/// Gelieerde personen opzoeken op (fragment van) naam.
		/// Gelieerde persoon, persoonsgegevens, adressen en communicatie
		/// worden opgehaald.
		/// </summary>
		/// <param name="groepID">ID van groep</param>
		/// <param name="zoekStringNaam">Zoekstring voor naam</param>
		/// <returns>Lijst met gevonden gelieerde personen</returns>
		IList<GelieerdePersoon> ZoekenOpNaam(int groepID, string zoekStringNaam);

		/// <summary>
		/// Gelieerde personen opzoeken op (exacte) naam en voornaam.
		/// Gelieerde persoon, persoonsgegevens, adressen en communicatie
		/// worden opgehaald.
		/// </summary>
		/// <param name="groepID">ID van groep</param>
		/// <param name="naam">Exacte naam om op te zoeken</param>
		/// <param name="voornaam">Exacte voornaam om op te zoeken</param>
		/// <returns>Lijst met gevonden gelieerde personen</returns>
		IEnumerable<GelieerdePersoon> ZoekenOpNaam(int groepID, string naam, string voornaam);


		/// <summary>
		/// Zoekt naar gelieerde personen van een bepaalde groep (met ID <paramref name="groepID"/> met naam 
		/// en voornaam gelijkaardig aan <paramref name="naam"/> en <paramref name="voornaam"/>.
		/// (inclusief communicatie en adressen)
		/// </summary>
		/// <param name="groepID">GroepID dat bepaalt in welke gelieerde personen gezocht mag worden</param>
		/// <param name="naam">Te zoeken naam (ongeveer)</param>
		/// <param name="voornaam">Te zoeken voornaam (ongeveer)</param>
		/// <returns>Lijst met gevonden matches</returns>
		/// <remarks>Standaard wordt de persoonsinfo 'geïncludet'</remarks>
		IList<GelieerdePersoon> ZoekenOpNaamOngeveer(int groepID, string naam, string voornaam);

		/// <summary>
		/// Zoekt naar gelieerde personen van een bepaalde groep (met ID <paramref name="groepID"/> met naam 
		/// en voornaam gelijkaardig aan <paramref name="naam"/> en <paramref name="voornaam"/>.
		/// (inclusief communicatie en adressen)
		/// </summary>
		/// <param name="groepID">GroepID dat bepaalt in welke gelieerde personen gezocht mag worden</param>
		/// <param name="naam">Te zoeken naam (ongeveer)</param>
		/// <param name="voornaam">Te zoeken voornaam (ongeveer)</param>
		/// <param name="paths">Expressies die aangeven welke dependencies mee opgehaald moeten worden</param>
		/// <returns>Lijst met gevonden matches</returns>
		IList<GelieerdePersoon> ZoekenOpNaamOngeveer(int groepID, string naam, string voornaam, params Expression<Func<GelieerdePersoon, object>>[] paths);

		/// <summary>
		/// Haalt de persoonsgegevens van alle gelieerde personen van een groep op.
		/// </summary>
		/// <param name="groepID">ID van de groep</param>
		/// <param name="sortering">Geeft aan hoe de pagina gesorteerd moet worden</param>
		/// <param name="extras">Bepaalt welke dependencies mee opgehaald moeten worden</param>
		/// <returns>Lijst van gelieerde personen</returns>
		IList<GelieerdePersoon> AllenOphalen(int groepID, PersoonSorteringsEnum sortering, PersoonsExtras extras);

		/// <summary>
		/// Haalt een gelieerde persoon op op basis van <paramref name="persoonID"/> en <paramref name="groepID"/>
		/// </summary>
		/// <param name="persoonID">ID van de *persoon* waarvoor de gelieerde persoon opgehaald moet worden</param>
		/// <param name="groepID">ID van de groep waaraan de gelieerde persoon gelieerd moet zijn</param>
		/// <param name="metVoorkeurAdres">Indien <c>true</c>, wordt ook het voorkeursadres opgehaald.</param>
		/// <param name="paths">bepaalt op te halen gekoppelde entiteiten</param>
		/// <returns>De gevraagde gelieerde persoon</returns>
		/// <remarks>Het ophalen van adressen kan niet beschreven worden met de lambda-
		/// expressies in <paramref name="paths"/>, o.w.v. de verschillen tussen Belgische en buitenlandse
		/// adressen.</remarks>
		GelieerdePersoon Ophalen(
			int persoonID, 
			int groepID, 
			bool metVoorkeurAdres, 
			params Expression<Func<GelieerdePersoon, object>>[] paths);


		/// <summary>
		/// Haalt een gelieerde persoon op op basis van <paramref name="persoonID"/> en <paramref name="groepID"/>
		/// </summary>
		/// <param name="persoonID">ID van de *persoon* waarvoor de gelieerde persoon opgehaald moet worden</param>
		/// <param name="groepID">ID van de groep waaraan de gelieerde persoon gelieerd moet zijn</param>
		/// <param name="paths">bepaalt op te halen gekoppelde entiteiten</param>
		/// <returns>De gevraagde gelieerde persoon</returns>
		/// <remarks>Adressen worden niet opgehaald, want adressen kunnen niet beschreven worden met de lambda-
		/// expressies in <paramref name="paths"/>.</remarks>
		GelieerdePersoon Ophalen(int persoonID, int groepID, params Expression<Func<GelieerdePersoon, object>>[] paths);

		/// <summary>
		/// Haalt een aantal gelieerde personen op, samen met de gekoppelde entiteiten bepaald door
		/// <paramref name="extras"/>
		/// </summary>
		/// <param name="gelieerdePersoonIDs">ID's op te halen gelieerde personen</param>
		/// <param name="extras">bepaalt de extra op te halen entiteiten</param>
		/// <returns>De gevraagde gelieerde personen.</returns>
		IEnumerable<GelieerdePersoon> Ophalen(IList<int> gelieerdePersoonIDs, PersoonsExtras extras);


		/// <summary>
		/// Haalt een 'pagina' persoonsgegevens van de gelieerde personen van een groep op, inclusief
		/// eventueel lidobject in het recentste werkjaar.
		/// </summary>
		/// <param name="groepID">ID van de groep</param>
		/// <param name="pagina">Paginanummer (1 of groter)</param>
		/// <param name="paginaGrootte">Aantal records op een pagina</param>
		/// <param name="sortering">Geeft aan hoe de pagina gesorteerd moet worden</param>
		/// <param name="aantalTotaal">Outputparameter voor totaal aantal
		/// personen gelieerd aan de groep.</param>
		/// <returns>Lijst met gelieerde personen</returns>
		IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, PersoonSorteringsEnum sortering, out int aantalTotaal);

		/// <summary>
		/// Haal een pagina op met gelieerde personen uit een categorie, inclusief lidinfo voor het huidige
		/// werkjaar.
		/// </summary>
		/// <param name="categorieID">ID van de gevraagde categorie</param>
		/// <param name="pagina">Paginanummer (1 of groter)</param>
		/// <param name="paginaGrootte">Aantal records op een pagina</param>
		/// <param name="sortering">Geeft aan hoe de pagina gesorteerd moet worden</param>
		/// <param name="extras">Geeft aan welke gekoppelde entiteiten mee opgehaald moeten worden</param>
		/// <param name="metHuidigLidInfo">Als <c>true</c> worden ook eventuele lidobjecten *van dit werkjaar* 
		/// mee opgehaald.</param>
		/// <param name="aantalTotaal">Outputparameter die het totaal aantal personen in de categorie weergeeft</param>
		/// <returns>Lijst gelieerde personen</returns>
		IList<GelieerdePersoon> PaginaOphalenUitCategorie(int categorieID, int pagina, int paginaGrootte, PersoonSorteringsEnum sortering, bool metHuidigLidInfo, out int aantalTotaal, PersoonsExtras extras);


		/// <summary>
		/// Haalt een gelieerde persoon op, inclusief
		///   - persoon
		///   - communicatievormen
		///   - adressen
		///   - groepen
		///   - categorieen
		///   - lidobjecten in het huidige werkjaar
		///   - afdelingen en functies van die lidobjecen
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van de gevraagde gelieerde persoon</param>
		/// <returns>Gelieerde persoon met alle bovenvernoemde details</returns>
		GelieerdePersoon DetailsOphalen(int gelieerdePersoonID);

		/// <summary>
		/// Laadt groepsgegevens in GelieerdePersoonsobject
		/// </summary>
		/// <param name="p">Gelieerde persoon</param>
		/// <returns>Referentie naar p, nadat groepsgegevens
		/// geladen zijn</returns>
		GelieerdePersoon GroepLaden(GelieerdePersoon p);

		/// <summary>
		/// Haalt een lijst op met alle communicatietypes
		/// </summary>
		/// <returns>Een lijst met alle communicatietypes</returns>
		IEnumerable<CommunicatieType> CommunicatieTypesOphalen();

		/// <summary>
		/// Haalt alle personen op die op een zelfde
		/// adres wonen als de gelieerde persoon met het gegeven ID.
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van gegeven gelieerde
		/// persoon.</param>
		/// <returns>Lijst met GelieerdePersonen (inc. persoonsinfo)</returns>
		/// <remarks>Als de persoon nergens woont, is hij toch zijn eigen
		/// huisgenoot.  Enkel huisgenoten uit dezelfde groep als de gelieerde persoon worden opgeleverd.</remarks>
		IList<GelieerdePersoon> HuisGenotenOphalenZelfdeGroep(int gelieerdePersoonID);

		/// <summary>
		/// Haalt alle gelieerde personen op waarvan de persoon wel een dubbelpuntabonnement maar
		/// geen AD-nummer heeft.  In theorie kan dit gebeuren tussen het zenden van de message en het
		/// verwerken door kipsync.  In praktijk zijn er blijkbaar een aantal nieuwe dubbelpuntabonnementen
		/// niet goed overgezet.
		/// </summary>
		/// <returns>Gelieerde personen zonder AD-nummer met dubbelpuntabonnement.  Als een persoon gelieerd
		/// is aan meerdere groepen, dan zal die in deze lijst meerdere keren voorkomen.
		/// Inclusief persoonsinfo</returns>
		IEnumerable<GelieerdePersoon> DubbelPuntZonderAdOphalen();

		/// <summary>
		/// Bewaart een gelieerde persoon samen met eventueel gekoppelde entiteiten
		/// </summary>
		/// <param name="gelieerdePersoon">Te bewaren gelieerde persoon</param>
		/// <param name="extras">bepaalt de gekoppelde entiteiten</param>
		/// <returns>de bewaarde gelieerde persoon</returns>
		GelieerdePersoon Bewaren(GelieerdePersoon gelieerdePersoon, PersoonsExtras extras);
	}
}
