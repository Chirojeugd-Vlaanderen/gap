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
		/// <param name="paths">Expressies die aangeven welke dependencies mee opgehaald moeten worden</param>
		/// <returns>Lijst van gelieerde personen</returns>
		IList<GelieerdePersoon> AllenOphalen(int groepID, PersoonSorteringsEnum sortering, params Expression<Func<GelieerdePersoon, object>>[] paths);

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
		/// <param name="paths">Geeft aan welke gekoppelde entiteiten mee opgehaald moeten worden</param>
		/// <param name="metHuidigLidInfo">Als <c>true</c> worden ook eventuele lidobjecten *van dit werkjaar* 
		/// mee opgehaald.</param>
		/// <param name="aantalTotaal">Outputparameter die het totaal aantal personen in de categorie weergeeft</param>
		/// <returns>Lijst gelieerde personen</returns>
		IList<GelieerdePersoon> PaginaOphalenUitCategorie(int categorieID, int pagina, int paginaGrootte, PersoonSorteringsEnum sortering, bool metHuidigLidInfo, out int aantalTotaal, params Expression<Func<GelieerdePersoon, object>>[] paths);


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

		/*/// <summary>
		/// Geeft de gelieerde personen uit een categorie (inc. persoonsinfo)
		/// TODO: Misschien moet deze functie een lijst van PersoonsInfo ophalen.
		/// </summary>
		/// <param name="categorieID">ID van een categorie</param>
		/// <returns>Lijst van gelieerde personen met persoonsinfo in de categorie</returns>
		IList<GelieerdePersoon> OphalenUitCategorie(int categorieID);*/

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

	}
}
