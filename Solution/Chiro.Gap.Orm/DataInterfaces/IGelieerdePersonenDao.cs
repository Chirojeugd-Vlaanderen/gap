// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Chiro.Cdf.Data;

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
		/// <param name="GroepID">ID van de groep</param>
		/// <param name="paths">Expressies die aangeven welke dependencies mee opgehaald moeten worden</param>
		/// <returns>Lijst van gelieerde personen</returns>
		IList<GelieerdePersoon> AllenOphalen(int GroepID, params Expression<Func<GelieerdePersoon, object>>[] paths);

		/// <summary>
		/// Haalt een 'pagina' persoonsgegevens van de gelieerde personen van een groep op, inclusief
		/// eventueel lidobject in het recentste werkjaar.
		/// </summary>
		/// <param name="groepID">ID van de groep</param>
		/// <param name="pagina">Paginanummer (1 of groter)</param>
		/// <param name="paginaGrootte">Aantal records op een pagina</param>
		/// <param name="aantalTotaal">Outputparameter voor totaal aantal
		/// personen gelieerd aan de groep.</param>
		/// <returns>Lijst met gelieerde personen</returns>
		IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalTotaal);

		/// <summary>
		/// Haalt een 'pagina' persoonsgegevens van de gelieerde personen van een groep op behorende tot de gegeven categorie
		/// inclusief eventueel lidobject in het recentste werkjaar.
		/// </summary>
		/// <param name="categorieID">ID van de categorie die we nodig hebben</param>
		/// <param name="pagina">Paginanummer (1 of groter)</param>
		/// <param name="paginaGrootte">Aantal records op een pagina</param>
		/// <param name="aantalTotaal">Outputparameter voor totaal aantal
		/// personen gelieerd aan de groep.</param>
		/// <returns>Lijst met gelieerde personen</returns>
		IList<GelieerdePersoon> PaginaOphalenMetLidInfoVolgensCategorie(int categorieID, int pagina, int paginaGrootte, out int aantalTotaal);

		/// <summary>
		/// Haalt persoonsgegevens van een gelieerd persoon op, incl. adressen en communicatievormen
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van op te halen gelieerde persoon</param>
		/// <returns>Gelieerde persoon met persoonsgegevens, adressen en communicatievormen</returns>
		GelieerdePersoon DetailsOphalen(int gelieerdePersoonID);

		/// <summary>
		/// Haalt een gelieerde persoon op, inclusief Lid/Leidingsobject in het groepswerkjaar bepaald
		/// door <paramref name="groepsWerkJaarID"/>, met
		/// daaraan gekoppeld eventuele afdelingsjaren met afdelingen.
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van op te halen gelieerde persoon</param>
		/// <param name="groepsWerkJaarID">ID van het gevraagde groepswerkjaar</param>
		/// <param name="paths">lambda-expressies die aangeven welke extra info er mee opgehaald moet worden.</param>
		/// <returns>De gelieerde persoon, inclusief Lid/Leidingsobject in het groepswerkjaar bepaald
		/// door <paramref name="groepsWerkJaarID"/>, met
		/// daaraan gekoppeld eventuele afdelingsjaren met afdelingen.</returns>
		/// <remarks>Het gegeven 'enkel lidinfo in een bepaald groepswerkjaar ophalen' kan niet opgegeven worden
		/// via een lambda-expressie, omdat gp=>gp.lid de lidobjecten uit alle werkjaren zou ophalen.</remarks>
		GelieerdePersoon OphalenMetAfdelingen(
			int gelieerdePersoonID,
			int groepsWerkJaarID,
			params Expression<Func<GelieerdePersoon, object>>[] paths);

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
		IEnumerable<CommunicatieType> ophalenCommunicatieTypes();
	}
}
