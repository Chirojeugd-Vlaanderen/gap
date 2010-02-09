namespace Chiro.Gap.TestDbInfo
{
	/// <summary>
	/// Info over testgegevens in de database voor unit testing.
	/// </summary>
	public static class TestInfo
	{
		#region ID's gegenereerd door script
		// te plakken uit output van 
		// cg2_database_action.sh -a creatie -t test

		public const int GROEPID = 37;
		public const int GELIEERDEPERSOONID = 4002;
		public const int GELIEERDEPERSOON2ID = 4003;
		public const int GELIEERDEPERSOON3ID = 4004;
		public const int MINAANTALGELPERS = 3;
		public const int LID3ID = 9;
		public const int CATEGORIEID = 9;
		public const string CATEGORIECODE = "last";
		public const int CATEGORIE2ID = 10;
		public const int AANTALINCATEGORIE = 1;
		public const int AFDELINGID = 13;
		public const int OFFICIELEAFDELINGID = 1;
		public const int AFDELINGSJAARID = 13;
		public const int AFDELING2ID = 14;
		public const int AFDELING2VAN = 2001;
		public const int AFDELING2TOT = 1998;
		public const int GROEPSWERKJAARID = 40;
		public const string ZOEKNAAM = "Bosmans";
		public const string GAV1 = "Yvonne";
		public const string GAV2 = "Yvette";
		public const int WATINGROEPID = 38;
		#endregion

		#region Zelf in te stellen:
		/// <summary>
		/// Code van een onbestaande categorie in de testgroep, om te testen
		/// </summary>
		public const string ONBESTAANDECATEGORIECODE1 = "BROES";

		/// <summary>
		/// Code van een onbestaande categorie in de testgroep, om te testen
		/// </summary>
		public const string ONBESTAANDECATEGORIECODE2 = "HALLO";

		/// <summary>
		/// Willekeurige onbestaande categorienaam, om te testen
		/// </summary>
		public const string CATEGORIENAAM = "Testkookies";

		/// <summary>
		/// Gebruikersnaam voor een nieuwe GAV
		/// </summary>
		public const string NIEUWEGAV = "nietgebruiken";

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
		/// String voor autosuggestie straat mee te testen
		/// (onregelmatigheid in casing is bewust)
		/// </summary>
		public const string TEZOEKENSTRAAT = "GRote ste";

		/// <summary>
		/// postnummer om bovenstaande straat in te zoeken
		/// </summary>
		public const int POSTNR = 2560;

		/// <summary>
		/// Ongeveer de voornaam van diezelfde bestaande persoon in de testgroep
		/// </summary>
		public const string ZOEKVOORNAAMONGEVEER = "Joss";

		#endregion;
	}

}
