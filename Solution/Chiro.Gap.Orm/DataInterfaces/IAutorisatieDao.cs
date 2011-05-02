// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
	/// <summary>
	/// Interface voor een gegevenstoegangsobject voor GebruikersRechten
	/// </summary>
	public interface IAutorisatieDao : IDao<GebruikersRecht>
	{
		#region Alle records in GebruikersRecht doorzoeken
		
		/// <summary>
		/// Haalt de rechten op die de gebruiker met de opgegeven <paramref name="login"/> heeft of had
		/// voor de groep met de opgegeven <paramref name="groepID"/>
		/// </summary>
		/// <param name="login">De gebruikersnaam van de bezoeker</param>
		/// <param name="groepID">ID van de groep</param>
		/// <returns>
		/// Een GebruikersRecht-object waarmee we kunnen nagaan welke rechten de gebruiker heeft of had
		/// m.b.t. de groep waar het over gaat
		/// </returns>
		/// <remarks>Let op: de gebruikersrechten kunnen vervallen zijn!</remarks>
		GebruikersRecht RechtenMbtGroepGet(string login, int groepID);

		/// <summary>
		/// Haalt rechten op die een gebruiker heeft op een gelieerde persoon.
		/// (Dit komt erop neer dat de gelieerde persoon gelieerd is aan een
		/// groep waar de gebruiker GAV van is.)
		/// </summary>
		/// <param name="login">De login van de gebruiker in kwestie</param>
		/// <param name="gelieerdePersoonID">ID van gelieerde persoon</param>
		/// <returns><c>Null</c> indien geen gebruikersrechten gevonden,
		/// anders een GebruikersRecht-object</returns>
		/// <remarks>Let op: de gebruikersrechten kunnen vervallen zijn!</remarks>
		GebruikersRecht RechtenMbtGelieerdePersoonGet(string login, int gelieerdePersoonID);
		#endregion

		#region Enkel de niet-vervallen gebruikersrechten

		/// <summary>
		/// Controleert of een gebruiker *nu* GAV is van een
		/// gegeven groep
		/// </summary>
		/// <param name="login">De login van de gebruiker in kwestie</param>
		/// <param name="groepID">De ID van de groep die de gebruiker wil zien en/of bewerken</param>
		/// <returns><c>True</c> als de gebruiker nu GAV is</returns>
		bool IsGavGroep(string login, int groepID);

		/// <summary>
		/// Controleert of een gebruiker *nu* GAV is van 
		/// de groep van een gelieerde persoon
		/// </summary>
		/// <param name="login">De gebruikersnaam van de bezoeker</param>
		/// <param name="gelieerdePersoonID">ID van de gelieerde persoon</param>
		/// <returns><c>True</c> als gebruiker nu GAV is</returns>
		bool IsGavGelieerdePersoon(string login, int gelieerdePersoonID);

		/// <summary>
		/// Controleert of een gebruiker *nu* GAV is van een
		/// groep waaraan de gegeven persoon gelieerd is.
		/// </summary>
		/// <param name="login">De gebruikersnaam van de bezoeker</param>
		/// <param name="persoonID">ID van persoon</param>
		/// <returns><c>True</c> als de gebruiker (op dit moment) geautoriseerd is
		/// om persoonsgegevens te zien/wijzigen</returns>
		bool IsGavPersoon(string login, int persoonID);

		/// <summary>
		/// Controleert of een gebruiker *nu* GAV is van de groep
		/// horende bij gegeven GroepsWerkJaar
		/// </summary>
		/// <param name="login">De gebruikersnaam van de bezoeker</param>
		/// <param name="groepsWerkJaarID">ID van gevraagde GroepsWerkJaar</param>
		/// <returns><c>True</c> als de gebruiker GAV is</returns>
		bool IsGavGroepsWerkJaar(string login, int groepsWerkJaarID);

		/// <summary>
		/// Controleert of een gebruiker *nu* GAV is van de groep
		/// horende bij de gegeven afdeling
		/// </summary>
		/// <param name="login">De gebruikersnaam van de bezoeker</param>
		/// <param name="afdelingsID">ID van de gegeven afdeling</param>
		/// <returns><c>True</c> als de bezoeker Gav is voor de bedoelde afdeling,
		/// <c>false</c> als dat niet het geval is</returns>
		bool IsGavAfdeling(string login, int afdelingsID);

		/// <summary>
		/// Controleert of een gebruiker *nu* GAV is van de groep
		/// horende bij het gegeven afdelingsJaar
		/// </summary>
		/// <param name="login">De gebruikersnaam van de bezoeker</param>
		/// <param name="afdelingsJaarID">ID van het gegeven afdelingsJaar</param>
		/// <returns><c>true</c> als de bezoeker Gav is voor het bedoelde afdelingsJaar,
		/// <c>false</c> als dat niet het geval is</returns>
		bool IsGavAfdelingsJaar(string login, int afdelingsJaarID);

		/// <summary>
		/// Controleert of een gebruiker *nu* GAV is van de groep
		/// horende bij het gegeven lid
		/// </summary>
		/// <param name="login">De gebruikersnaam van de bezoeker</param>
		/// <param name="lidID">ID van een zeker lid</param>
		/// <returns><c>True</c> als de bezoeker Gav is voor het bedoelde Lid,
		/// <c>false</c> als dat niet het geval is</returns>
		bool IsGavLid(string login, int lidID);

		/// <summary>
		/// Haalt de groepen op waarvoor de gebruiker met de opgegeven login
		/// *op dit moment* Gebruikersrechten heeft
		/// </summary>
		/// <param name="login">De login van de gebruiker in kwestie</param>
		/// <returns>Lijst met een of meerdere groepen</returns>
		IEnumerable<Groep> MijnGroepenOphalen(string login);

		/// <summary>
		/// Haalt uit een lijst van ID's gelieerde personen degene die onder de *huidige* gebruikersrechten vallen
		/// van de gebruiker met de opgegeven login
		/// </summary>
		/// <param name="gelieerdePersonenIDs">Een lijst van ID's van gelieerde personen</param>
		/// <param name="login">De login van de gebruiker in kwestie</param>
		/// <returns>Een lijst van ID's van de gelieerde personen die de gebruiker mag bekijken en bewerken</returns>
		IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs, string login);

		/// <summary>
		/// Haalt uit een lijst van ID's personen degene die onder de *huidige* gebruikersrechten vallen
		/// van de gebruiker met de opgegeven login
		/// </summary>
		/// <param name="personenIDs">Lijst met PersonenID's</param>
		/// <param name="login">De login van de gebruiker in kwestie</param>
		/// <returns>Een lijst van ID's van de personen die de gebruiker mag bekijken en bewerken</returns>
		IList<int> EnkelMijnPersonen(IEnumerable<int> personenIDs, string login);

		/// <summary>
		/// Verwijdert uit een lijst van LidID's de ID's
		/// van leden voor wie de gebruiker met gegeven <paramref name="login"/> geen GAV is.
		/// </summary>
		/// <param name="lidIDs">ID's van leden</param>
		/// <param name="login">login van de gebruiker</param>
		/// <returns>Enkel de ID's van leden waarvoor de gebruiker GAV is.</returns>
		IEnumerable<int> EnkelMijnLeden(IEnumerable<int> lidIDs, string login);

		/// <summary>
		/// Controleert of een gegeven gebruiker GAV is van de groep
		/// horend bij een zekere categorie.
		/// </summary>
		/// <param name="categorieID">ID van de betreffende categorie</param>
		/// <param name="login">De gebruikersnaam van de bezoeker</param>
		/// <returns><c>True</c> als de bezoeker Gav is voor de bedoelde categorie,
		/// <c>false</c> als dat niet het geval is</returns>
		bool IsGavCategorie(int categorieID, string login);

		/// <summary>
		/// Controleert of een gegeven gebruiker GAV is van de groep
		/// horend bij een zekere commvorm.
		/// </summary>
		/// <param name="commvormID">ID van de betreffende commvorm</param>
		/// <param name="login">De gebruikersnaam van de bezoeker</param>
		/// <returns><c>True</c> als de bezoeker Gav is voor de bedoelde communicatievorm,
		/// <c>false</c> als dat niet het geval is</returns>
		bool IsGavCommVorm(int commvormID, string login);

		/// <summary>
		/// Geeft <c>true</c> als het persoonsAdres met ID <paramref name="persoonsAdresID"/> gekoppeld is aan een persoon
		/// waarop de gebruiker met login <paramref name="login"/> momenteel GAV-rechten op heeft.  Anders
		/// <c>false</c>.
		/// </summary>
		/// <param name="persoonsAdresID">ID van de functie</param>
		/// <param name="login">De gebruikersnaam</param>
		/// <returns><c>true</c> als het persoonsAdres met ID <paramref name="persoonsAdresID"/> gekoppeld is aan een persoon
		/// waarop de gebruiker met login <paramref name="login"/> momenteel GAV-rechten op heeft.  Anders
		/// <c>false</c>.</returns>
		bool IsGavPersoonsAdres(int persoonsAdresID, string login);

		/// <summary>
		/// Geeft <c>true</c> als de uitstap met ID <paramref name="uitstapID"/> gekoppeld is aan een 
		/// groepswerkjaar waarop de gebruiker met login <paramref name="login"/> momenteel GAV-rechten heeft.  Anders
		/// <c>false</c>.
		/// </summary>
		/// <param name="uitstapID">ID van de uitstap</param>
		/// <param name="login">De gebruikersnaam</param>
		/// <returns><c>true</c> als het persoonsAdres met ID <paramref name="uitstapID"/> gekoppeld is aan een 
		/// groepswerkjaar waarop de gebruiker met login <paramref name="login"/> momenteel GAV-rechten heeft.  Anders
		/// <c>false</c>.</returns>
		bool IsGavUitstap(int uitstapID, string login);

		/// <summary>
		/// Controleert of de gebruiker met gegeven <paramref name="login"/> op dit moment GAV-rechten heeft op de plaats
		/// met id <paramref name="plaatsID"/>.
		/// </summary>
		/// <param name="login">login van de gebruiker wiens GAV-schap moet worden getest</param>
		/// <param name="plaatsID">ID van een bivakplaats</param>
		/// <returns><c>true</c> als de aangemelde user nu GAV is voor de plaats, anders <c>false</c>. </returns>
		bool IsGavPlaats(int plaatsID, string login);

		#endregion
	}
}
