using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{

	// TODO: Straat en subgemeente zouden standaard mee opgehaald moeten worden.

	/// <summary>
	/// Data Access Object voor adressen.    
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
		/// Haalt adres op, op basis van de adresgegevens
		/// </summary>
		/// <param name="straatNaam">naam van de straat</param>
		/// <param name="huisNr">huisnummer</param>
		/// <param name="bus">bus</param>
		/// <param name="postNr">postnummer</param>
		/// <param name="postCode">postcode (nt relevant in Belgie)</param>
		/// <param name="gemeenteNaam">naam (sub)gemeente</param>
		/// <param name="metBewoners">indien true, worden ook de PersoonsAdressen
		/// opgehaald.  (ALLE persoonsadressen gekoppeld aan het adres; niet
		/// zomaar over de lijn sturen dus)</param>
		/// <returns>gevraagd adresobject</returns>
		Adres Ophalen(string straatNaam, int? huisNr, string bus, int postNr, string postCode, string gemeenteNaam, bool metBewoners);
	}
}
