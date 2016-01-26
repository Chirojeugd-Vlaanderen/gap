/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
﻿namespace Chiro.Gap.TestDbInfo
{
	/// <summary>
	/// Info over testgegevens in de database voor unit testing.
	/// </summary>
	public static class TestInfo
	{
		#region ID's gegenereerd door script
		// te plakken uit output van trunk/db/TestSetup/4_unit_tests.sql
        public const int GROEP_ID = 1065; // tst/0001   St.-Unittestius
        public const int GELIEERDE_PERSOON_ID = 225183; // Persoon Jos Bosmans
        public const int GELIEERDE_PERSOON2_ID = 225184; // Persoon Irène Bosmans
        public const int GELIEERDE_PERSOON3_ID = 225185; // Persoon Eugène Bosmans
        public const int GELIEERDE_PERSOON4_ID = 225186; // Persoon Yvonne Bosmans
        public const int GELIEERDE_PERSOON5_ID = 225187; // Persoon Benjamin De Kleine
        public const int MIN_AANTAL_GEL_PERS = 5;
        public const int LID3_ID = 421600;
        public const int LID4_ID = 421601;
        public const int LID5_ID = 421603;
        public const int CATEGORIE_ID = 887;
        public const string CATEGORIE_CODE = "last";
        public const int CATEGORIE2_ID = 888;
        public const int CATEGORIE3_ID = 889;
        public const int FUNCTIE_ID = 409;
        public const int AANTAL_IN_CATEGORIE = 1;
        public const int AFDELING1_ID = 5866;
        public const int OFFICIELE_AFDELING_ID = 1;
        public const int AFDELINGS_JAAR1_ID = 23209;
        public const int AFDELING1_VAN = 2003;
        public const int AFDELING1_TOT = 2004;
        public const int AFDELING2_ID = 5867;
        public const int AFDELINGS_JAAR2_ID = 23210;
        public const int AFDELING2_VAN = 2003;
        public const int AFDELING2_TOT = 2004;
        public const int AFDELING3_ID = 5868;
        public const int ADRES_ID = 53;
        public const int GROEPS_WERKJAAR_ID = 5033;
        public const string ZOEK_NAAM = "Bosmans";
        public const char GP_1_3_LETTER = 'B'; // 1ste letter familienaam gelieerde persoon
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
		public static readonly string[] ONBESTAANDE_CATEGORIE_CODES = {
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
		public const string NIEUWE_PERSOON_VOORNAAM = "Clement";

		/// <summary>
		/// Voornaam voor persoon in `verwijdertest'
		/// </summary>
		public const string TE_VERWIJDEREN_VOORNAAM = "Sylvain";

		/// <summary>
		/// Voornaam voor persoon in DubbelePersonenTest
		/// </summary>
		public const string DUBBELE_PERSOON_VOORNAAM = "Dave";

		/// <summary>
		/// Nog een voornaam voor persoon in DubbelePersonenTest
		/// </summary>
		public const string DUBBELE_PERSOON_VOORNAAM_2 = "Eddy";

		/// <summary>
		/// Een willekeurig AD-nummer
		/// </summary>
		public const int DUBBELE_PERSOON_AD = 123456;


		/// <summary>
		/// String voor autosuggestie straat mee te testen
		/// (onregelmatigheid in casing is bewust)
		/// </summary>
		public const string TE_ZOEKEN_STRAAT = "GRote ste";

		/// <summary>
		/// postnummer om bovenstaande straat in te zoeken
		/// </summary>
		public const int POSTNR = 2560;

		/// <summary>
		/// Ongeveer de voornaam van diezelfde bestaande persoon in de testgroep
		/// </summary>
		public const string ZOEK_VOORNAAM_ONGEVEER = "Joss";

		/// <summary>
		/// 'Geboortejaar tot' voor nieuw afdelingsjaar afdeling 3
		/// </summary>
		public const int AFDELING3_TOT = 2000;

		/// <summary>
		/// 'Geboortejaar van' voor nieuw afdelingsjaar afdeling 3
		/// </summary>
		public const int AFDELING3_VAN = 1998;

		/// <summary>
		/// Een ongeldig telefoonnummer
		/// </summary>
		public const string ONGELDIG_TELEFOON_NR = "00bla";

		/// <summary>
		/// Een geldig telefoonnummer
		/// </summary>
		public const string GELDIG_TELEFOON_NR = "03-484 53 32";

		/// <summary>
		/// Een ongeldig GelieerdePersoonID
		/// </summary>
		public const int ONBESTAANDE_GELIEERDE_PERSOON_ID = -1;
		#endregion;
	}

}
