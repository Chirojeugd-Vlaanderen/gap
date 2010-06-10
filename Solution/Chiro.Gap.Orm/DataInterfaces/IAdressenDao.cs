// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
	// TODO: Straat en subgemeente zouden standaard mee opgehaald moeten worden.

	/// <summary>
	/// Interface voor een data access object voor adressen
	/// </summary>
	public interface IAdressenDao : IDao<Adres>
	{
		/// <summary>
		/// Haalt een adres op, samen met de gekoppelde personen
		/// </summary>
		/// <param name="adresID">ID op te halen adres</param>
		/// <param name="user">Als user gegeven, worden enkel de
		/// personen gekoppeld waar die user bekijkrechten op heeft.
		/// Als user de lege string is, worden alle bewoners meegeleverd.
		/// </param>
		/// <returns>Adresobject met gekoppelde personen</returns>
		Adres BewonersOphalen(int adresID, string user);

		/// <summary>
		/// Haalt het adres met ID <paramref name="adresID"/> op, inclusief de bewoners uit de groep met ID
		/// <paramref name="groepID"/>
		/// </summary>
		/// <param name="adresID">ID van het op te halen adres</param>
		/// <param name="groepID">ID van de groep waaruit bewoners moeten worden gehaald</param>
		/// <returns>Het gevraagde adres met de relevante bewoners.</returns>
		Adres BewonersOphalen(int adresID, int groepID);

		/// <summary>
		/// Haalt adres op, op basis van de adresgegevens
		/// </summary>
		/// <param name="straatNaam">Naam van de straat</param>
		/// <param name="huisNr">Het huisnummer</param>
		/// <param name="bus">Het eventuele busnummer</param>
		/// <param name="postNr">Het postnummer</param>
		/// <param name="postCode">De postcode (niet relevant in België)</param>
		/// <param name="woonPlaatsNaam">Naam van de woonplaats</param>
		/// <param name="metBewoners">Indien <c>true</c>, worden ook de PersoonsAdressen
		/// opgehaald.  (ALLE persoonsadressen gekoppeld aan het adres; niet
		/// zomaar over de lijn sturen dus)</param>
		/// <returns>Gevraagd adresobject</returns>
		Adres Ophalen(
			string straatNaam, int? huisNr, string bus, 
			int postNr, string postCode, string woonPlaatsNaam, 
			bool metBewoners);
	}
}
