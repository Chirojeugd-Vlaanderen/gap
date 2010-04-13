// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		/// Haalt rechten op die een gebruiker heeft op een groep.
		/// </summary>
		/// <param name="login">De gebruikersnaam van de bezoeker</param>
		/// <param name="groepID">ID van de groep</param>
		/// <returns><c>Null</c> als geen gebruikersrechten gevonden,
		/// anders een GebruikersRecht-object</returns>
		/// <remarks>Let op: de gebruikersrechten kunnen vervallen zijn!</remarks>
		GebruikersRecht RechtenMbtGroepGet(string login, int groepID);

		/// <summary>
		/// Haalt rechten op die een gebruiker heeft op een gelieerde persoon.
		/// (Dit komt erop neer dat de gelieerde persoon gelieerd is aan een
		/// groep waar de gebruiker GAV van is.)
		/// </summary>
		/// <param name="login">De gebruikersnaam van de bezoeker</param>
		/// <param name="GelieerdePersoonID">ID van gelieerde persoon</param>
		/// <returns><c>Null</c> indien geen gebruikersrechten gevonden,
		/// anders een GebruikersRecht-object</returns>
		/// <remarks>Let op: de gebruikersrechten kunnen vervallen zijn!</remarks>
		GebruikersRecht RechtenMbtGelieerdePersoonGet(string login, int GelieerdePersoonID);
		#endregion

		#region Enkel de niet-vervallen gebruikersrechten

		/// <summary>
		/// Controleert of een gebruiker *nu* GAV is van een
		/// gegeven groep
		/// </summary>
		/// <param name="login">De gebruikersnaam </param>
		/// <param name="groepID">ID van de groep</param>
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
		/// Haalt lijst groepen op waaraan de GAV met gegeven
		/// login MOMENTEEL gekoppeld is
		/// </summary>
		/// <param name="login">Gebruikersnaam van de GAV</param>
		/// <returns>Lijst met groepen</returns>
		IEnumerable<Groep> GekoppeldeGroepenGet(string login);

		/// <summary>
		/// Verwijdert uit een lijst met GelieerdePersonenID's de ID's
		/// waarvan een gegeven gebruiker geen GAV is
		/// </summary>
		/// <param name="gelieerdePersonenIDs">Lijst met GelieerdePersonenID's</param>
		/// <param name="login">De gebruikersnaam van de bezoeker</param>
		/// <returns>Een lijst met de ID's van GelieerdePersonen waar de gebruiker
		/// GAV over is. (hoeveel indirectie kan er in 1 zin?)</returns>
		IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs, string login);

		/// <summary>
		/// Verwijdert uit een lijst met PersonenID's de ID's
		/// waarvan een gegeven gebruiker geen GAV is
		/// </summary>
		/// <param name="personenIDs">Lijst met PersonenID's</param>
		/// <param name="p"></param>
		/// <returns>Een lijst met de ID's van Personen waar de gebruiker
		/// GAV over is. (hoeveel indirectie kan er in 1 zin?)</returns>
		IList<int> EnkelMijnPersonen(IEnumerable<int> personenIDs, string p); // TODO: parameter p documenteren (#340)

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

		#endregion
	}
}
