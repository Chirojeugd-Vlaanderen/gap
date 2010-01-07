namespace Chiro.Gap.Data.Test
{
	/// <summary>
	/// Gegevens voor unit testing, zodat de automatische tests de
	/// `manuele' tests niet in de weg lopen.
	/// </summary>
	/// <remarks>Deze ID's moeten overeenkomen met de DB.  Het zou niet slecht zijn moest het
	/// script om de database te genereren telkens dezelfde testdata aanmaken, waaraan dan deze
	/// ID's aangepast kunnen worden.</remarks>
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
		/// ID van een derde gelieerde persoon van de testgroep; deze persoon is lid.
		/// </summary>
		public const int GELIEERDEPERSOON3ID = 1580;

		/// <summary>
		/// Ondergrens voor het totaal aantal gelieerde personen in een groep.
		/// </summary>
		public const int MINAANTALGELPERS = 3;

		/// <summary>
		/// LidID van een leider;
		/// </summary>
		public const int LID3ID = 607;


		/// <summary>
		/// ID van categorie van testgroep1
		/// </summary>
		/// <remarks>Enkel gelieerde Persoon 1 moet in deze categorie zitten</remarks>
		public const int CATEGORIEID = 288;

		/// <summary>
		/// code van bovenvermelde categorie
		/// </summary>
		public const string CATEGORIECODE = "last";

		/// <summary>
		/// ID van tweede testcategorie
		/// </summary>
		/// <remarks>Deze categorie bevat zowel gelieerde personen 1 en 2</remarks>
		public const int CATEGORIE2ID = 289;

		/// <summary>
		/// Totaal aantal personen in deze categorie
		/// </summary>
		public const int AANTALINCATEGORIE = 1;

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
		/// Ongeveer de voornaam van diezelfde bestaande persoon in de testgroep
		/// </summary>
		public const string ZOEKVOORNAAMONGEVEER = "Joss";

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

		/// <summary>
		/// String voor autosuggestie straat mee te testen
		/// (onregelmatigheid in casing is bewust)
		/// </summary>
		public const string TEZOEKENSTRAAT = "GRote ste";

		/// <summary>
		/// postnummer om bovenstaande straat in te zoeken
		/// </summary>
		public const int POSTNR = 2560;
	}

}