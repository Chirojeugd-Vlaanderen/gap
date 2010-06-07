// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using Chiro.Gap.Orm;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Een autorisatiemanager bepaalt de rechten van een gebruiker op entiteiten.
	/// </summary>
	public interface IAutorisatieManager
	{
		/// <summary>
		/// Verwijdert uit een lijst van GelieerdePersoonID's de ID's
		/// van GelieerdePersonen voor wie de aangemelde gebruiker geen GAV is.
		/// </summary>
		/// <param name="gelieerdePersonenIDs">Lijst met ID's van gelieerde personen</param>
		/// <returns>Enkel de ID's van die personen voor wie de gebruiker GAV is</returns>
		IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs);

		/// <summary>
		/// Verwijdert uit een lijst van PersoonID's de ID's
		/// van personen voor wie de aangemelde gebruiker geen GAV is.
		/// </summary>
		/// <param name="personenIDs">Lijst met ID's van personen</param>
		/// <returns>Enkel de ID's van die personen voor wie de gebruiker GAV is</returns>
		IList<int> EnkelMijnPersonen(IEnumerable<int> personenIDs);

		/// <summary>
		/// Ophalen van HUIDIGE gekoppelde groepen voor een aangemelde GAV
		/// </summary>
		/// <returns>ID's van gekoppelde groepen</returns>
		IEnumerable<Groep> GekoppeldeGroepenGet();

		/// <summary>
		/// Geeft true als (en slechts als) de ingelogde user correspondeert
		/// met een GAV van een groep gelieerd aan de gelieerde
		/// persoon met gegeven ID
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van te checken gelieerde persoon</param>
		/// <returns><c>True</c> als de user de persoonsgegevens mag zien/bewerken</returns>
		bool IsGavGelieerdePersoon(int gelieerdePersoonID);

		/// <summary>
		/// IsGav geeft true als de aangelogde user
		/// gav is voor de groep met gegeven ID
		/// </summary>
		/// <param name="groepID">ID van de groep</param>
		/// <returns><c>True</c> (enkel) als user GAV is</returns>
		bool IsGavGroep(int groepID);

		/// <summary>
		/// Geeft true alss de aangemelde user correspondeert
		/// met een GAV van de groep van een GroepsWerkJaar
		/// </summary>
		/// <param name="groepsWerkJaarID">ID gevraagde groepswerkjaar</param>
		/// <returns><c>True</c> als aangemelde gebruiker GAV is</returns>
		bool IsGavGroepsWerkJaar(int groepsWerkJaarID);

		/// <summary>
		/// Geeft true als (en slechts als) de ingelogde user correspondeert
		/// met een GAV van een groep gelieerd aan de
		/// persoon met gegeven ID
		/// </summary>
		/// <param name="persoonID">ID van te checken Persoon</param>
		/// <returns><c>True</c> als de user de persoonsgegevens mag zien/bewerken</returns>
		bool IsGavPersoon(int persoonID);

		/// <summary>
		/// Controleert of een afdeling gekoppeld is aan een groep waarvan
		/// de gebruiker GAV is.
		/// </summary>
		/// <param name="afdelingsID">ID gevraagde afdeling</param>
		/// <returns><c>True</c> als de gebruiker GAV is van de groep van de
		/// afdeling</returns>
		bool IsGavAfdeling(int afdelingsID);

		/// <summary>
		/// Controleert of een afdelingsjaar gekoppeld is aan een groep waarvan
		/// de gebruiker GAV is.
		/// </summary>
		/// <param name="afdelingsJaarID">ID gevraagde afdelingsJaar</param>
		/// <returns><c>True</c> als de gebruiker GAV is van de groep van het
		/// afdelingsjaar</returns>
		bool IsGavAfdelingsJaar(int afdelingsJaarID);

		/// <summary>
		/// Controleert of een lid lid is van een groep waarvan de gebruiker
		/// GAV is.
		/// </summary>
		/// <param name="lidID">ID van het betreffende lid</param>
		/// <returns><c>True</c> als het een lid van een eigen groep is</returns>
		bool IsGavLid(int lidID);

		/// <summary>
		/// Controleert of de huidig aangelogde gebruiker momenteel
		/// GAV is van de groep gekoppeld aan een zekere categorie.
		/// </summary>
		/// <param name="categorieID">ID van de categorie</param>
		/// <returns><c>True</c> indien GAV</returns>
		bool IsGavCategorie(int categorieID);

		/// <summary>
		/// Controleert of de huidig aangelogde gebruiker momenteel
		/// GAV is van de groep gekoppeld aan een zekere communicatievorm.
		/// </summary>
		/// <param name="commvormID">ID van de communicatievorm</param>
		/// <returns><c>True</c> indien GAV</returns>
		bool IsGavCommVorm(int commvormID);

		/// <summary>
		/// Geeft <c>true</c> als de functie met ID <paramref name="functieID"/> nationaal gedefinieerd is
		/// of gekoppeld is aan een groep waar de aangelogde gebruiker momenteel GAV van is.  Anders
		/// <c>false</c>.
		/// </summary>
		/// <param name="functieID">ID van de functie</param>
		/// <returns><c>true</c> als de functie met ID <paramref name="functieID"/> nationaal gedefinieerd is
		/// of gekoppeld is aan een groep waar de aangelogde gebruiker momenteel GAV van is.  Anders
		/// <c>false</c>.</returns>
		bool IsGavFunctie(int functieID);

		/// <summary>
		/// Geeft <c>true</c> als het persoonsAdres met ID <paramref name="persoonsAdresID"/> gekoppeld is aan een persoon
		/// waarop de aangelogde gebruiker momenteel GAV-rechten op heeft.  Anders
		/// <c>false</c>.
		/// </summary>
		/// <param name="persoonsAdresID">ID van de functie</param>
		/// <returns><c>true</c> als het persoonsAdres met ID <paramref name="persoonsAdresID"/> gekoppeld is aan een persoon
		/// waarop de aangelogde gebruiker momenteel GAV-rechten op heeft.  Anders
		/// <c>false</c>.</returns>
		bool IsGavPersoonsAdres(int persoonsAdresID);

		/// <summary>
		/// Geeft true als de aangelogde user
		/// gav is voor de groep met gegeven ID, en 'superrechten' heeft
		/// (zoals het verwijderen van leden uit vorig werkjaar, het 
		/// verwijderen van leden waarvan de probeerperiode voorbij is,...)
		/// </summary>
		/// <param name="groepID">ID van de groep</param>
		/// <returns><c>True</c> (enkel) als user supergav is</returns>
		bool IsSuperGavGroep(int groepID);

		/// <summary>
		/// Bepaalt de gebruikersnaam van de huidig aangemelde gebruiker.
		/// (lege string indien niet van toepassing)
		/// </summary>
		/// <returns>Username aangemelde gebruiker</returns>
		string GebruikersNaamGet();
	}
}
