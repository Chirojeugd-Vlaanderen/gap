namespace Chiro.Gap.TestDbInfo
{
	/// <summary>
	/// Info over testgegevens in de database voor unit testing.
	/// </summary>
	public static class TestInfo
	{
		#region ID's gegenereerd door script
		// te plakken uit output van trunk/db/TestSetup/4_unit_tests.sql
		// 
        public const int GROEPID = 1065;
        public const int GELIEERDEPERSOONID = 227268;
        public const int GELIEERDEPERSOON2ID = 227269;
        public const int GELIEERDEPERSOON3ID = 227270;
        public const int GELIEERDEPERSOON4ID = 227271;
        public const int GELIEERDEPERSOON5ID = 227272;
        public const int MINAANTALGELPERS = 5;
        public const int LID3ID = 424454;
        public const int LID4ID = 424455;
        public const int LID5ID = 424457;
        public const int CATEGORIEID = 920;
        public const string CATEGORIECODE = "last";
        public const int CATEGORIE2ID = 921;
        public const int CATEGORIE3ID = 922;
        public const int FUNCTIEID = 421;
        public const int AANTALINCATEGORIE = 1;
        public const int AFDELING1ID = 5872;
        public const int OFFICIELEAFDELINGID = 1;
        public const int AFDELINGSJAAR1ID = 23266;
        public const int AFDELING1VAN = 2003 ;
        public const int AFDELING1TOT = 2004 ;
        public const int AFDELING2ID = 5873;
        public const int AFDELINGSJAAR2ID = 23267;
        public const int AFDELING2VAN = 2003 ;
        public const int AFDELING2TOT = 2004 ;
        public const int AFDELING3ID = 5874;
        public const int ADRESID = 53;
        public const int GROEPSWERKJAARID = 5042;
        public const string ZOEKNAAM = "Bosmans";
        public const string GAV1 = "Yvonne";
        public const string GAV2 = "Yvette";

		#endregion

		
		#region Zelf in te stellen:

        /// <summary>
        /// GroepID voor watin-tests
        /// </summary>
        public const int WATINGROEPID = 1056;

		/// <summary>
		/// Onbestaande categoriecode te gebruiken voor toevoegtest
		/// </summary>
		public const string ONBESTAANDENIEUWECATCODE = "BROES";

		/// <summary>
		/// Naam voor de nieuwe categorie
		/// </summary>
		public const string ONBESTAANDENIEUWECATNAAM = "Broezers";

		/// <summary>
		/// Codes van onbestaande categorieën, te gebruiken voor unit tests.
		/// </summary>
		public static readonly string[] ONBESTAANDECATEGORIECODES = {
									"HALLO",
									"BLA2",
									"BLA3"};
									
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
		/// Voornaam voor persoon in DubbelePersonenTest
		/// </summary>
		public const string DUBBELEPERSOONVOORNAAM = "Dave";

		/// <summary>
		/// Nog een voornaam voor persoon in DubbelePersonenTest
		/// </summary>
		public const string DUBBELEPERSOONVOORNAAM2 = "Eddy";

		/// <summary>
		/// Een willekeurig AD-nummer
		/// </summary>
		public const int DUBBELEPERSOONAD = 123456;


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

		/// <summary>
		/// 'Geboortejaar tot' voor nieuw afdelingsjaar afdeling 3
		/// </summary>
		public const int AFDELING3TOT = 2000;

		/// <summary>
		/// 'Geboortejaar van' voor nieuw afdelingsjaar afdeling 3
		/// </summary>
		public const int AFDELING3VAN = 1998;

		/// <summary>
		/// Een ongeldig telefoonnummer
		/// </summary>
		public const string ONGELDIGTELEFOONNR = "00bla";

		/// <summary>
		/// Een geldig telefoonnummer
		/// </summary>
		public const string GELDIGTELEFOONNR = "03-484 53 32";

		/// <summary>
		/// Een ongeldig GelieerdePersoonID
		/// </summary>
		public const int ONBESTAANDEGELIEERDEPERSOONID = -1;
		#endregion;
	}

}
