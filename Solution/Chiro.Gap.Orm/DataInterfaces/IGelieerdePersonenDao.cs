using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
	/// <summary>
	/// CRUD-operaties op GelieerdePersoon
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
		/// <param name="zoekStringNaam">zoekstring voor naam</param>
		/// <returns>Lijst met gevonden gelieerde personen</returns>
		IList<GelieerdePersoon> ZoekenOpNaam(int groepID, string zoekStringNaam);

		/// <summary>
		/// Zoekt naar gelieerde personen van een bepaalde groep (met ID <paramref name="groepID"/> met naam 
		/// en voornaam gelijkaardig aan <paramref name="naam"/> en <paramref name="voornaam"/>.
		/// (inclusief communicatie en adressen)
		/// </summary>
		/// <param name="groepID">GroepID dat bepaalt in welke gelieerde personen gezocht mag worden</param>
		/// <param name="naam">te zoeken naam (ongeveer)</param>
		/// <param name="voornaam">te zoeken voornaam (ongeveer)</param>
		/// <returns>lijst met gevonden matches</returns>
		/// <remarks>standaard wordt de persoonsinfo 'geincludet'</remarks>
		IList<GelieerdePersoon> ZoekenOpNaamOngeveer(int groepID, string naam, string voornaam);

		/// <summary>
		/// Zoekt naar gelieerde personen van een bepaalde groep (met ID <paramref name="groepID"/> met naam 
		/// en voornaam gelijkaardig aan <paramref name="naam"/> en <paramref name="voornaam"/>.
		/// (inclusief communicatie en adressen)
		/// </summary>
		/// <param name="groepID">GroepID dat bepaalt in welke gelieerde personen gezocht mag worden</param>
		/// <param name="naam">te zoeken naam (ongeveer)</param>
		/// <param name="voornaam">te zoeken voornaam (ongeveer)</param>
		/// <param name="paths">expressies die aangeven welke dependencies mee opgehaald moeten worden</param>
		/// <returns>lijst met gevonden matches</returns>
		IList<GelieerdePersoon> ZoekenOpNaamOngeveer(int groepID, string naam, string voornaam, params Expression<Func<GelieerdePersoon, object>>[] paths);


		/// <summary>
		/// Haalt de persoonsgegevens van alle gelieerde personen van een groep op.
		/// </summary>
		/// <param name="GroepID">ID van de groep</param>
		/// <returns>Lijst van gelieerde personen</returns>
		IList<GelieerdePersoon> AllenOphalen(int GroepID);

		/// <summary>
		/// Haalt een 'pagina' persoonsgegevens van de gelieerde personen van een groep op, inclusief
		/// eventueel lidobject in het recentste werkjaar.
		/// </summary>
		/// <param name="groepID">ID van de groep</param>
		/// <param name="pagina">paginanummer (1 of groter)</param>
		/// <param name="paginaGrootte">aantal records op een pagina</param>
		/// <param name="aantalTotaal">outputparameter voor totaal aantal
		/// personen gelieerd aan de groep.</param>
		/// <returns>Lijst met gelieerde personen</returns>
		IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalTotaal);

		/// <summary>
		/// Haalt een 'pagina' persoonsgegevens van de gelieerde personen van een groep op behorende tot de gegeven categorie
		/// inclusief eventueel lidobject in het recentste werkjaar.
		/// </summary>
		/// <param name="pagina">paginanummer (1 of groter)</param>
		/// <param name="paginaGrootte">aantal records op een pagina</param>
		/// <param name="aantalTotaal">outputparameter voor totaal aantal
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
		/// Laadt groepsgegevens in GelieerdePersoonsobject
		/// </summary>
		/// <param name="p">gelieerde persoon</param>
		/// <returns>referentie naar p, nadat groepsgegevens
		/// geladen zijn</returns>
		GelieerdePersoon GroepLaden(GelieerdePersoon p);

		/*/// <summary>
		/// Geeft de gelieerde personen uit een categorie (inc. persoonsinfo)
		/// TODO: Misschien moet deze functie een lijst van PersoonsInfo ophalen.
		/// </summary>
		/// <param name="categorieID">ID van een categorie</param>
		/// <returns>lijst van gelieerde personen met persoonsinfo in de categorie</returns>
		IList<GelieerdePersoon> OphalenUitCategorie(int categorieID);*/

		/// <summary>
		/// Haalt een lijst op met alle communicatietypes
		/// </summary>
		IEnumerable<CommunicatieType> ophalenCommunicatieTypes();
	}
}
