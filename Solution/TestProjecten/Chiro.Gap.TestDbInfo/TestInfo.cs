namespace Chiro.Gap.TestDbInfo
{
	/// <summary>
	/// Info over testgegevens in de database voor unit testing.
	/// </summary>
	public static class TestInfo
	{
		/// <summary>
		/// ID van groep waarop automatisch getest mag worden
		/// </summary>
		public const int GROEPID = 1;

		/// <summary>
		/// ID van een gelieerde persoon van de testgroep
		/// </summary>
		/// <remarks>Deze persoon moet minstens 1 categorie hebben</remarks>
		public const int GELIEERDEPERSOONID = 1;

		/// <summary>
		/// ID van een andere gelieerde persoon van de testgroep
		/// </summary>
		public const int GELIEERDEPERSOON2ID = 2;

		/// <summary>
		/// ID van een derde gelieerde persoon van de testgroep; deze persoon is lid.
		/// </summary>
		public const int GELIEERDEPERSOON3ID = 3;

		/// <summary>
		/// Ondergrens voor het totaal aantal gelieerde personen in een groep.
		/// </summary>
		public const int MINAANTALGELPERS = 3;

		/// <summary>
		/// LidID van een leider;
		/// </summary>
		public const int LID3ID = 1;


		/// <summary>
		/// ID van categorie van testgroep1
		/// </summary>
		/// <remarks>Enkel gelieerde Persoon 1 moet in deze categorie zitten</remarks>
		public const int CATEGORIEID = 1;

		/// <summary>
		/// code van bovenvermelde categorie
		/// </summary>
		public const string CATEGORIECODE = "last";

		/// <summary>
		/// ID van tweede testcategorie
		/// </summary>
		/// <remarks>Deze categorie bevat zowel gelieerde personen 1 en 2</remarks>
		public const int CATEGORIE2ID = 2;

		/// <summary>
		/// Totaal aantal personen in deze categorie
		/// </summary>
		public const int AANTALINCATEGORIE = 2;

		/// <summary>
		/// ID van een afdeling van de testgroep.  Van deze afdeling bestaat een afdelingsjaar.
		/// </summary>
		public const int AFDELINGID = 1;

		/// <summary>
		/// Officiele afdeling horende bij afdeling bepaald door AFDELINGID
		/// </summary>
		public const int OFFICIELEAFDELINGID = 1;

		/// <summary>
		/// Afdelingsjaar voor de testafdeling
		/// </summary>
		public const int AFDELINGSJAARID = 1;

		/// <summary>
		/// ID van een andere afdeling van de testgroep.  Van deze afdeling bestaat
		/// geen afdelingsjaar.
		/// </summary>
		public const int AFDELING2ID = 2;

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
		public const int GROEPSWERKJAARID = 1;

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