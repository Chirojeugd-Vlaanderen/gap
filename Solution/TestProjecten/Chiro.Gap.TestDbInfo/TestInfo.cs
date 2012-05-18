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
        public const int GELIEERDEPERSOONID = 225183;
        public const int GELIEERDEPERSOON2ID = 225184;
        public const int GELIEERDEPERSOON3ID = 225185;
        public const int GELIEERDEPERSOON4ID = 225186;
        public const int GELIEERDEPERSOON5ID = 225187;
	    public const char GP13LETTER = 'B'; // 1ste letter familienaam gelieerde persoon 1 en 3
        public const int MINAANTALGELPERS = 5;
        public const int LID3ID = 421600;
        public const int LID4ID = 421601;
        public const int LID5ID = 421603;
        public const int CATEGORIEID = 887;
        public const string CATEGORIECODE = "last";
        public const int CATEGORIE2ID = 888;
        public const int CATEGORIE3ID = 889;
        public const int FUNCTIEID = 409;
        public const int AANTALINCATEGORIE = 1;
        public const int AFDELING1ID = 5866;
        public const int OFFICIELEAFDELINGID = 1;
        public const int AFDELINGSJAAR1ID = 23209;
        public const int AFDELING1VAN = 2003;
        public const int AFDELING1TOT = 2004;
        public const int AFDELING2ID = 5867;
        public const int AFDELINGSJAAR2ID = 23210;
        public const int AFDELING2VAN = 2003;
        public const int AFDELING2TOT = 2004;
        public const int AFDELING3ID = 5868;
        public const int ADRESID = 53;
        public const int GROEPSWERKJAARID = 5033;
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
