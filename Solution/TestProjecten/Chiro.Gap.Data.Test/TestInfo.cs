namespace Chiro.Gap.Data.Test
{
	/// <summary>
	/// Gegevens voor unit testing, zodat de automatische tests de
	/// `manuele' tests niet in de weg lopen.
	/// </summary>
	public static class TestInfo
	{
		/// <summary>
		/// ID van groep waarop automatisch getest mag worden
		/// </summary>
		public const int GROEPID = 317;

		/// <summary>
		/// ID van een gelieerde persoon van de testgroep
		/// </summary>
		/// <remarks>Deze persoon moet minstens 1 categorie hebben</remarks>
		public const int GELIEERDEPERSOONID = 917;

		/// <summary>
		/// ID van een andere gelieerde persoon van de testgroep
		/// </summary>
		public const int GELIEERDEPERSOON2ID = 918;

		/// <summary>
		/// ID van categorie van testgroep1
		/// </summary>
		/// <remarks>Gelieerde Persoon 1 moet in deze categorie zitten</remarks>
		public const int CATEGORIEID = 8;

		/// <summary>
		/// ID van een afdeling van de testgroep
		/// </summary>
		public const int AFDELINGID = 37;

		/// <summary>
		/// Officiele afdeling horende bij afdeling bepaald door AFDELINGID
		/// </summary>
		public const int OFFICIELEAFDELINGID = 3;

		/// <summary>
		/// Afdelingsjaar voor de testafdeling
		/// </summary>
		public const int AFDELINGSJAARID = 82;

		/// <summary>
		/// ID van een andere afdeling van de testgroep
		/// </summary>
		public const int AFDELING2ID = 40;

		/// <summary>
		/// `startgeboortejaar' voor afdeling 2
		/// </summary>
		public const int AFDELING2VAN = 2001;

		/// <summary>
		/// `stopgeboortejaar' voor afdeling 2.
		/// </summary>
		public const int AFDELING2TOT = 1998;

		/// <summary>
		/// ID van een groepswerkjaar van de testgroep
		/// </summary>
		public const int GROEPSWERKJAARID = 13;

		/// <summary>
		/// Naam van een bestaande persoon in de testgroep
		/// </summary>
		public const string ZOEKNAAM = "Bosmans";

		/// <summary>
		/// Naam voor nieuwe persoon
		/// </summary>
		public const string NIEUWEPERSOONNAAM = "Perens";

		/// <summary>
		/// Voornaam voor nieuwe persoon
		/// </summary>
		public const string NIEUWEPERSOONVOORNAAM = "Clement";

		/// <summary>
		/// Voornaam voor persoon in `verwijdertest'
		/// </summary>
		public const string TEVERWIJDERENVOORNAAM = "Sylvain";

		/// <summary>
		/// Gebruikersnaam voor een nieuwe GAV
		/// </summary>
		public const string NIEUWEGAV = "nietgebruiken";

		/// <summary>
		/// login van GAV van testgroep 1
		/// </summary>
		public const string GAV1 = "yvonne";

		/// <summary>
		/// login van GAV die geen GAV van testgroep 1 is.
		/// </summary>
		public const string GAV2 = "yvette";
	}

}