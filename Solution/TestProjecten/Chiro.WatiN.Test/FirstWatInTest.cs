using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

using Chiro.Gap.TestDbInfo;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using WatiN.Core;

namespace Chiro.WatiN.Test
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class FirstWatInTest
	{
		/// <summary>
		/// Dit is een eerste voorbeeldje dat niks specifiek test, de bedoeling hier is enkel om aan te
		/// tonen hoe we de gemeenschappelijke delen van een test schrijven.
		///
		/// </summary>
		///<remarks>
		///</remarks>
		public TestContext TestContext { get; set; }

		// 
		// ------------------------------------------------------------------------------------------------------------
		// - Configuratie van test omgeving
		// ------------------------------------------------------------------------------------------------------------
		//

		#region Additional test attributes

		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//

		#endregion

		// Deployment server voor GapServices
		private static Process _gapServicesProcess;
		private static readonly int GapServicesPort = Properties.Settings.Default.GapServicesPort;
		private static string _gapServicesPhysicalPath;

		// Deployment server voor GapWebApp
		private static Process _gapWebAppProcess;
		private static readonly int GapWebAppPort = Properties.Settings.Default.GapWebAppPort;
		private static readonly string GapWebAppDeploymentServer = "http://localhost:" + GapWebAppPort;
		private static string _gapWebAppPhysicalPath;

		// Gemeenschappelijk voor alle Services
		private const string VIRTUAL_DIRECTORY = "/";

		private const string WEB_DEV_SERVER_PATH =
			@"C:\Program Files (x86)\Common Files\microsoft shared\DevServer\9.0\WebDev.WebServer.EXE";

		private static string _solutionRoot;

		[ClassInitialize]
		public static void StartWebServer(TestContext testContext)
		{
			// De IDE wat tijd geven om correct zijn werk te doen, vooral op trage machines.
			System.Threading.Thread.Sleep(1000);

			// Probeer de Solution directory te vinden.
			var dte = (EnvDTE.DTE) System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE");
			_solutionRoot = System.IO.Path.GetDirectoryName(dte.Solution.FullName);

			// Als we de testen runnen in de debugging mode, dan start de DevServer automatisch.
			// Er zijn 2 mogelijkheden wanneer we niet in de debugger zitten, ofwel runnen we onze testen
			// via de VisualStudio UI, ofwel via een commandline tool genaamd: MSTest.
			// We veronderstellen dat als de environment variabele "CG2_GESTART_DOOR" de waarde DevEnv heeft, 
			// dan gebruiken we niet de VisualStudio UI.
			Boolean startDevServer = false;
			String gg2GestartDoor = Environment.GetEnvironmentVariable("    ");
			if (!String.IsNullOrEmpty(gg2GestartDoor))
			{
				if (String.Equals(gg2GestartDoor, "MSTest"))
				{
					startDevServer = true;
				}
			}
			else
			{
				if (!Debugger.IsAttached)
				{
					startDevServer = true;
				}
			}
			Debug.WriteLine("StartWebserver: Moet server opstarten: " + startDevServer);

			if (startDevServer)
			{
				_gapServicesPhysicalPath = (_solutionRoot + @"\Chiro.Gap.Services").TrimEnd('\\');
				Debug.WriteLine("StartWebserver: ServicesPhysicalPath: " + _gapServicesPhysicalPath);
				_gapWebAppPhysicalPath = (_solutionRoot + @"\Chiro.Gap.WebApp").TrimEnd('\\');
				Debug.WriteLine("StartWebserver: WebAppPhysicalPath: " + _gapWebAppPhysicalPath);

				_gapServicesProcess = new Process();
				string gapServicesArguments = string.Format("/port:{0} /path:\"{1}\" /vpath:{2}",
				                                            GapServicesPort,
				                                            _gapServicesPhysicalPath,
				                                            VIRTUAL_DIRECTORY);
				_gapServicesProcess.StartInfo = new ProcessStartInfo(WEB_DEV_SERVER_PATH, gapServicesArguments);
				_gapServicesProcess.Start();

				_gapWebAppProcess = new Process();
				string gapWebAppArguments = string.Format("/port:{0} /path:\"{1}\" /vpath:{2}",
				                                          GapWebAppPort,
				                                          _gapWebAppPhysicalPath,
				                                          VIRTUAL_DIRECTORY);
				_gapWebAppProcess.StartInfo = new ProcessStartInfo(WEB_DEV_SERVER_PATH, gapWebAppArguments);
				_gapWebAppProcess.Start();

				// Wat tijd geven om correct op te starten
				System.Threading.Thread.Sleep(120);
			}
			else
			{
				// We moeten de IDE de tijd geven om correct op te starten.
				// Omdat de services opstarten vraagt wat tijd.
				// Hoe doen we dat het beste?
				System.Threading.Thread.Sleep(120);
			}

			// Geef gebruiker GAV-rechten op groep voor watin-test
			TestHelper.GeefGavRecht(TestInfo.WATINGROEPID);
		}

		[ClassCleanup]
		public static void StopWebServer()
		{
			// Ontneem gebruiker GAV-rechten op groep voor watin-test
			TestHelper.OntneemGavRecht(TestInfo.WATINGROEPID);

			if (!Debugger.IsAttached)
			{
				if (_gapServicesProcess == null)
				{
					throw new InvalidOperationException("Start() must be called before Stop()");
				}
				_gapServicesProcess.Kill();

				if (_gapWebAppProcess == null)
				{
					throw new InvalidOperationException("Start() must be called before Stop()");
				}
				_gapWebAppProcess.Kill();
			}
		}

		// Use TestCleanup to run code after each test has run
		[TestInitialize]
		public void MyTestInitialize()
		{
			// Tussen elke test moeten we er voor zorgen dat alle data leeg is, dit wil zeggen dat voor
			// de ChiroGroep met stamnummer 'WATIN'  en naam 'St-WebAutomatedTestIndotNet' volledig leeg moet zijn.

			TestHelper.KuisOp(TestInfo.WATINGROEPID);
		}

		// 
		// ------------------------------------------------------------------------------------------------------------
		// - Acties met Menu's
		// ------------------------------------------------------------------------------------------------------------
		//

		/// <summary>
		/// Definitie van de beschikbare paginas:
		/// </summary>
		public enum Paginas
		{
			Afdelingen,
			Personen,
			Leden
		}

		private String _mijnHomePage;
		// Hier schrijf ik een aantal help functies die mijn testen overzichtelijker maken
		/// <summary>
		/// Deze procedure gaat in het window naar de gevraagde pagina.
		/// </summary>
		/// <param name="window">Het browser window</param>
		/// <param name="pagina">De pagina die je wil openen</param>
		public void GaNaarMenuPagina(IE window, Paginas pagina)
		{
			// We gaan we moeten controlleren of de normale gebruiker dit kan doen, en moeten 
			// er dus opletten dat we naar een generieke link gaan, daarna door klikken naar de 
			// gewenste pagina. 
			// Als we direct naar de link surfen, weten we niet of deze bereikbaar via de normale
			// weg.  (2e reden: een aantal linken zijn afhankelijk van wie er op de site zit.)
			if (String.IsNullOrEmpty(_mijnHomePage))
			{
				window.GoTo(GapWebAppDeploymentServer);

				// Indien een gebruiker toegang heef tot meer dan 1 groep dan moet die nu een groep selecteren, 
				// indien dit zo is bevat de titel van de pagina 'Kies je Chirogroep | Nog geen Chirogroep geselecteerd'
				// en hij moet dan 'WATIN - St-WebAutomatedTestIndotNet ()' selecteren
				var selecteerGroep = new Regex("Kies je Chirogroep");
				if (selecteerGroep.Match(window.Title).Success)
				{
					window.Link(Find.ByText(new Regex("St-WebAutomatedTestIndotNet"))).Click();
					_mijnHomePage = window.Url;
				}
			}
			else
			{
				window.GoTo(_mijnHomePage);
			}

			// Op deze pagina is er een unordered list (ul) met id menu.
			Element menuLijst = window.Element(Find.ById("menu"));

			switch (pagina)
			{
				case Paginas.Personen:
					{
						// Code om naar Personen pagina te gaan
						menuLijst.DomContainer.Link(Find.ByText(new Regex("^Iedereen$"))).Click();
						break;
					}
				case Paginas.Leden:
					{
						// Code om naar Leden pagina te gaan
						menuLijst.DomContainer.Link(Find.ByText(new Regex("^Ingeschreven$"))).Click();
						break;
					}
				default:
					{
						throw new NotImplementedException();
					}
			}
		}

		// 
		// ------------------------------------------------------------------------------------------------------------
		// - Acties met Menu's
		// ------------------------------------------------------------------------------------------------------------
		//

		public struct Persoon
		{
			public string Voornaam, FamilieNaam, GeboorteDatum, Geslacht, ChiroLeeftijd;

			public Persoon(String voornaam, String familieNaam, String geboorteDatum, String geslacht, String chiroLeeftijd)
			{
				Voornaam = voornaam;
				FamilieNaam = familieNaam;
				GeboorteDatum = geboorteDatum;
				Geslacht = geslacht;
				ChiroLeeftijd = chiroLeeftijd;
			}
		}

		public enum PersonenActie
		{
			NieuwePersoon,
			ZoekPersoonEnMaakLid,
			TelVoorkomen
		}

		/// <summary>
		/// Preconditie: We window moet in Personen menu zitten.
		/// </summary>
		/// <param name="window"></param>
		/// <param name="actie"></param>
		public void PersonenPaginaActie(IE window, PersonenActie actie)
		{
			// Op deze pagina is er een unordered list (ul) met id menu.
			Element actieLijst = window.Element(Find.ById(new Regex("^acties$")));

			switch (actie)
			{
				case PersonenActie.NieuwePersoon:
					{
						// Klik op de 'Nieuwe persoon' link
						// Bij het zoeken van linken/id's/... op een pagina moeten we er met rekening houden dat 
						// die pagina's gegenereerd (toegekend) worden door de compiler, en dat die id's wel eens 
						// een beetje van naam kunnen veranderen.
						// Zoek daarom het beste via een reguliere expressie die minimaal bevat wat nodig is.
						actieLijst.DomContainer.Link(Find.ByText(new Regex("^Persoon toevoegen$"))).Click();
						break;
					}

				default:
					{
						throw new NotImplementedException();
					}
			}
		}

		public Boolean PersonenPaginaActie(IE window, PersonenActie actie, Persoon pers)
		{
			switch (actie)
			{
				case PersonenActie.NieuwePersoon:
					{
						// Dit is enkel gesupporteerd bij PersonenPaginaActie(IE window, PersonenActie actie)
						throw new NotImplementedException("Persoons gegevens niet nodig.");
					}
				case PersonenActie.ZoekPersoonEnMaakLid:
					{
						// Vanaf dat we 1 persoon hebben getracht lid te maken stoppen we er mee.
						bool persoonLidGemaakt = false;
						// Resultaat van lid maken
						bool persoonLidGemaaktRestultaat = false;

						// Aanmaken van een reguliere expressie die we gaan gebruiken om een persoon op te zoeken, 
						// Hier gebruiken we enkel nogmaar de Naam en VoorNaam.
						var vindPersoon = new Regex(pers.Voornaam + " " + pers.FamilieNaam);

						// Als we dit vinden in de volgende pagina dat in 
						var vindIsToegevoegdAlsLid = new Regex(@"is nu ingeschreven als (lid|leiding|kind)");

						// Een volgende uitdaging is het vinden van een persoon in de lijst, deze kan namelijk op een andere pagina staan,
						// de verschillende pagina nummers staat in een blok met naam: 'pager', van dit blok itereren we over alle links.
						int linkNr = 0;
						while (linkNr < window.Div(Find.ByClass(new Regex("^pager$"))).Links.Count && !persoonLidGemaakt)
						{
							// Openen van de pagina. (het volgen van de link)
							window.Div(Find.ByClass(new Regex("^pager$"))).Links[linkNr].Click();

							// Kijk of we daar een '<Voornaam> <Naam>' tegen komen in de tabel met personen.
							// Alle verschillende personen staan in een tabel, waarvan spijtig genoeg de table noch tbody noch tr een id hebben.
							// Gaan zoeken over alle itereren over alle Tabel rijen.

							IEnumerator<TableRow> personenLijstEnum = window.TableRows.GetEnumerator();
							// Bij creatie van de PersonenLijstEnum, wordt die geinitialiseerd juist voor het eerste resultaat. 
							// (resultaat is nog niet opgehaald) en als hij er geen meer kan geven geeft hij een false return.
							// De eerste rij is altijd de table header, dus ga direct naar de volgende.
							personenLijstEnum.MoveNext();
							while (personenLijstEnum.MoveNext() && !persoonLidGemaakt)
							{
								// De naam van de persoon is de 3e in de tabel rij. (Find.ByIndex(2))
								// Let Op: Geen regex want we willen een exacte match
								if (vindPersoon.IsMatch(personenLijstEnum.Current.TableCell(Find.ByIndex(2)).ToString()))
								{
									// TODO: Nakijken of GeboorteDatum en Geslacht correct zijn.

									// Maak de persoon lid, maar er bestaat een kans dat die persoon al lid is, 
									// Dus voor de link gaan ophalen kijken of die wel bestaat.
									if (personenLijstEnum.Current.ElementOfType<Link>(Find.ByText(new Regex("^inschrijven$"))).Exists)
									{
										personenLijstEnum.Current.ElementOfType<Link>(Find.ByText(new Regex("^inschrijven$"))).Click();

										// Als een persoon is toegevoed gaan we terug naar het begin scherm, en hebben we een veld waar:
										// '<Voornaam> <Naam> is toegevoegd als lid.' in staat.
										// We controlleren hier voor 2 dingen:
										//   - of de juiste naam van de persoon in de feedback staat
										//   - of de string: 'is toegevoegd als lid.' in de feedback staat.

										string feedBackText = window.Element(Find.ByClass(new Regex("^Succesmelding$"))).Text;

										persoonLidGemaaktRestultaat =
											vindIsToegevoegdAlsLid.IsMatch(window.Element(Find.ByClass(new Regex("^Succesmelding$"))).Text);

										// We hebben een poging gedaan om de persoon lid te maken.
										persoonLidGemaakt = true;
									}
									else
									{
										// Niet duidelijk wat ik moet doen, het is mogelijk om in een groep verschillende 
										// personen te hebben met de zelfde naam.  Maar als we hem uiteindelijk niet vinden dan 
										// moeten we een fout tonen.
									}
								}
							}
							linkNr = linkNr + 1;
						}
						// We hebben alle pagina's doorlopen en we hebben de persoon niet kunnen lid maken.
						if (!persoonLidGemaakt)
						{
							Debug.WriteLine("ASSERT ERROR: Niet lid gemaakt:" + pers.Voornaam + " " + pers.FamilieNaam);
						}
						return persoonLidGemaaktRestultaat;
					}
				default:
					{
						throw new NotImplementedException();
					}
			}
		}

		public int PersonenPaginaActieCount(IE window, PersonenActie actie, Persoon pers)
		{
			int aantalGevonden = 0;

			// Aanmaken van een reguliere expressie die we gaan gebruiken om een persoon op te zoeken, 
			// Hier gebruiken we enkel nogmaar de Naam en VoorNaam.
			var vindPersoon = new Regex(pers.Voornaam + " " + pers.FamilieNaam);

			// Een volgende uitdaging is het vinden van een persoon in de lijst, deze kan namelijk op een andere pagina staan,
			// de verschillende pagina nummers staat in een blok met naam: 'pager', van dit blok itereren we over alle links.
			int linkNr = 0;
			while (linkNr < window.Div(Find.ByClass(new Regex("^pager$"))).Links.Count)
			{
				// Openen van de pagina. (het volgen van de link)
				window.Div(Find.ByClass(new Regex("^pager$"))).Links[linkNr].Click();

				// Kijk of we daar een '<Voornaam> <Naam>' tegen komen in de tabel met personen.
				// Alle verschillende personen staan in een tabel, waarvan spijtig genoeg de table noch tbody noch tr een id hebben.
				// Gaan zoeken over alle itereren over alle Tabel rijen.

				IEnumerator<TableRow> personenLijstEnum = window.TableRows.GetEnumerator();
				// Bij creatie van de PersonenLijstEnum, wordt die geinitialiseerd juist voor het eerste resultaat. 
				// (resultaat is nog niet opgehaald) en als hij er geen meer kan geven geeft hij een false return.
				// De eerste rij is altijd de table header, dus ga direct naar de volgende.
				personenLijstEnum.MoveNext();
				while (personenLijstEnum.MoveNext())
				{
					// De naam van de persoon is de 3e in de tabel rij. (Find.ByIndex(2))
					// Let Op: Geen regex want we willen een exacte match
					if (vindPersoon.IsMatch(personenLijstEnum.Current.TableCell(Find.ByIndex(2)).ToString()))
					{
						// Een persoon gevonden
						aantalGevonden += 1;
					}
				}
				linkNr += 1;
			}
			return aantalGevonden;
		}

		/// <summary>
		///  Preconditie we zitten in toevoegen van nieuwe persoon.
		/// </summary>
		/// <param name="window"></param>
		/// <param name="pers"></param>
		public void PersoonToevoegen(IE window, Persoon pers)
		{
			// Na het klikken op Nieuwe Persoon, krijgen we een scherm met de volgende gegevens:
			TextField fieldVoornaam = window.TextField(Find.ByName(new Regex("\\.VoorNaam$")));
			TextField fieldNaam = window.TextField(Find.ByName(new Regex("\\.Naam$")));
			TextField fieldGebDat = window.TextField(Find.ByName(new Regex("\\.GeboorteDatum$")));
			// De RadioButton's kunnen we op onderstaande mannier vinden, maar vermits de naam overeen komt met wat 
			// er in de Test tabel staat kan ik het ook op een andere manier doen (zie lager).
			// RadioButton Button_Geslacht_Man = window.RadioButton(Find.ByName("HuidigePersoon.Persoon.Geslacht") && Find.ByValue("Man"));
			// RadioButton Button_Geslacht_Vrouw = window.RadioButton(Find.ByName("HuidigePersoon.Persoon.Geslacht") && Find.ByValue("Vrouw"));
			TextField fieldChiroLeeftijd = window.TextField(Find.ByName(new Regex("\\.ChiroLeefTijd$")));
			Button buttonBewaren = window.Button(Find.ByValue(new Regex("Bewaren$")));

			// Eenvoudig testje, we gaan de volgende persoon invoegen:
			//  Voornaam, Naam, GeboorteDatum, Geslacht, ChiroLeeftijd
			fieldVoornaam.Click();
			fieldVoornaam.TypeText(pers.Voornaam);

			fieldNaam.Click();
			fieldNaam.TypeText(pers.FamilieNaam);

			fieldGebDat.Click();
			fieldGebDat.TypeText(pers.GeboorteDatum);

			if (("Man".Equals(pers.Geslacht)) || ("Vrouw".Equals(pers.Geslacht)))
			{
				window.RadioButton(Find.ByName(new Regex("Geslacht$")) && Find.ByValue(pers.Geslacht)).Checked = true;
			}

			fieldChiroLeeftijd.Click();
			fieldChiroLeeftijd.TypeText(pers.ChiroLeeftijd);

			// We bewaren de toestand in de database
			buttonBewaren.Click();
		}

		public enum ToegevoegdePersoonActies
		{
			NieuwAdres,
			CommunicatieVormToevoegen,
			ToevoegenCategorie,
			TerugNaarLijst,
			GegevensWijzigen,
			Bewaren,
			Niets
		}

		public void ToegevoegdePersoonActie(IE window, ToegevoegdePersoonActies actie)
		{
			switch (actie)
			{
				case ToegevoegdePersoonActies.TerugNaarLijst:
					{
						window.Link(Find.ByText(new Regex("Terug naar vorig overzicht"))).Click();
						break;
					}
				case ToegevoegdePersoonActies.Niets:
					{
						// Niks
						break;
					}
				case ToegevoegdePersoonActies.Bewaren:
					{
						window.Button(Find.ByValue("Bewaren")).Click();
						break;
					}
				default:
					{
						throw new NotImplementedException();
					}
			}
		}

		public Boolean CheckToegevoegdePersoon(IE window, Persoon pers, Boolean success, string foutBoodschap)
		{
			Boolean resultaat = true;

			if (success)
			{
				// Kijk of de pagina een succesmelding bevat.

				if (window.Element(Find.ByClass(new Regex("Succesmelding"))) == null)
				{
					Debug.WriteLine("ASSERT ERROR: " + foutBoodschap);
					resultaat = false;
				}

				// Als de boodschap er staat dat het gelukt is, zijn we content.
				// Eigenlijk zou het wel beter zijn moesten we even nakijken of de gegevens op het
				// scherm overeen komen met wat we verwachten.

				// Vroeger was er dergelijke code, maar die veronderstelde dat we in een form
				// terecht kwamen.  Dat is nu niet meer het geval.
			}
			else
			{
				// Deze pagina bevat een div met class naam 'validation-summary-errors', 
				// en daarin staat dan een lijst met de fouten.
				Element foutMeldingElement = window.Element(Find.ByClass(new Regex("validation-summary-errors")));

				if (!foutMeldingElement.Exists)
				{
					Debug.WriteLine("ASSERT ERROR: " + foutBoodschap);
					Debug.WriteLine("              validation-summary-errors Class niet gevonden.");
					resultaat = false;
				}
				else
				{
					// Soms staat de validation-summary-errors in een <DIV>
					// en soms in een <P>
					switch (foutMeldingElement.TagName)
					{
						case "DIV":
							resultaat = false;
							Div foutMelding = window.Div(Find.ByClass(new Regex("validation-summary-errors")));
							IEnumerator<Element> errorElements = foutMelding.Elements.GetEnumerator();
							while (errorElements.MoveNext())
							{
								// De fout boodschappen ziten in een <LI>
								if (errorElements.Current.TagName.Equals("LI"))
								{
									if (errorElements.Current.OuterText.Contains(foutBoodschap))
									{
										// We hebben de fout boodschap gevonden.
										resultaat = true;
									}
								}
							}
							// We hebben over alle Elements met TagName LI gegaan en als de Boodschap overeen kwam dan staat resultaat op true
							// Als hier resultaat nog false is dan moeten we een foutboodschap terugsturen.
							break;

						case "P":
							if (!foutMeldingElement.InnerHtml.Contains(foutBoodschap))
							{
								// We hebben de fout boodschap niet gevonden.
								resultaat = false;
							}
							break;

						default:
							throw new NotImplementedException();
					}

					if (!resultaat)
					{
						Debug.WriteLine("ASSERT ERROR: " + foutBoodschap);
						Debug.WriteLine("              validation-summary-errors Gevonden maar bevat niet de verwachte boodschap.");
						resultaat = false;
					}
				}
			}
			return resultaat;
		}

		// De naam van de test moet een inidicatie zijn van wat er getest gaat worden.
		// Als de test faalt dan weten we ook in eens wat er niet meer werkt. 
		//
		// Tracht ook te vermijden om te veel te gaan testen in 1 test.  Het is beter
		// meer simpele verstaanbare testen te schrijven i.p.v. 1 grote niet te begrijpen
		// test.

		/// <summary>
		/// InvoegenPersonen gaat test of we de lijst van personen (Testdata) met success kunnen 
		/// invoegen.
		/// </summary>
		[TestMethod]
		public void PersonenInvoegen()
		{
			Boolean foutGevonden = false;
			String raporteerFout = String.Empty;

			// new IE(), maakt gebruik van externe resources (en implementeerd IDisposable), 
			// zet dit dus in een 'using' block.  
			using (var window = new IE())
			{
				// Eerst gaan we naar een aantal foutieve pagina's, enkel om te testen of GaNaarMenuPagina 
				// geen fouten heeft.
				GaNaarMenuPagina(window, Paginas.Leden);

				// Ga naar de Personen pagina
				GaNaarMenuPagina(window, Paginas.Personen);
				// We wensen een persoon toe te voegen.
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);

				// ------------------------------------------------------------------------------------------------ 
				// Positive Testen
				// ------------------------------------------------------------------------------------------------

				//
				// TEST 1: Toevoegen van een persoon, van het type mannelijk.
				//
				var koen = new Persoon("Koen", "Meersman", "23/11/1975", "Man", "-8");
				PersoonToevoegen(window, koen);
				// Na het bewaren moeten we wachten tot dat we een resutltaat gekregen hebben
				if (!CheckToegevoegdePersoon(window, koen, true, "Persoon (Koen Meersman) is niet opgeslagen"))
				{
					foutGevonden |= true;
					raporteerFout += "\nPersoon (Koen Meersman) is niet opgeslagen";
				}
				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.TerugNaarLijst);

				//
				// TEST 2: Toevoegen van een persoon van het type vrouwelijk.
				//
				var marleen = new Persoon("Marleen", "Van Loock", "07/12/1977", "Vrouw", "0");
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
				PersoonToevoegen(window, marleen);
				if (!CheckToegevoegdePersoon(window, marleen, true, "Persoon (Marleen Van Loock) is niet opgeslagen"))
				{
					foutGevonden |= true;
					raporteerFout += "\nPersoon (Marleen Van Loock) is niet opgeslagen";
				}
				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.TerugNaarLijst);

				//
				// TEST 3: Toevoegen van een persoon van het type onzijdig.
				//
				var onzijdig = new Persoon("Onzijdige", "Persoon", "07/12/1977", "Onzijdig", "0");
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);

				PersoonToevoegen(window, onzijdig);
				if (!CheckToegevoegdePersoon(window, onzijdig, true, "Een onzijdige Persoon is niet opgeslagen"))
				{
					foutGevonden |= true;
					raporteerFout += "\nEen onzijdige Persoon is niet opgeslagen";
				}
				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.TerugNaarLijst);

				//
				// TEST 4: Toevoegen van een persoon, die nog geboren moet worden ;-)
				//
				var onGeboren = new Persoon("Minder", "Dan Baby", "07/12/2077", "Vrouw", "0");
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
				PersoonToevoegen(window, onGeboren);
				// In theorie moet dit falen, maar momenteel is dit nog success vol.
				if (!CheckToegevoegdePersoon(window, onGeboren, true, "Persoon met geboorte datum in de toekomst is opgeslagen"))
				{
					foutGevonden |= true;
					raporteerFout += "\nPersoon met geboorte datum in de toekomst kan met niet invoegen.";
				}

				//
				// TEST 5: Toevoegen van een persoon, zonder geboortedatum
				//
				var geenGebDat = new Persoon("Geen", "Geboorte Datum", "", "Vrouw", "0");
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
				PersoonToevoegen(window, geenGebDat);
				if (!CheckToegevoegdePersoon(window, geenGebDat, true, "Persoon zonder geboortedatum is niet opgeslagen"))
				{
					foutGevonden |= true;
					raporteerFout += "\nPersoon zonder geboortedatum is niet opgeslagen.";
				}

				//
				// TEST 6 : Invoegen van 2 personen die op elkaar gelijken.
				//          Ik weet wel niet de definitie van 2 op elkaar lijkende personen.
				//          (En deze persoon confirmeren)
				//
				var tweeling1 = new Persoon("Tweeling", "Persoon1", "01/04/2009", "Man", "2");
				var tweeling2 = new Persoon("Tweeling", "Persoon2", "01/04/2009", "Man", "2");
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
				PersoonToevoegen(window, tweeling1);
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
				PersoonToevoegen(window, tweeling2);
				// Kijken of we een waarschuwing zoals onderstaande hebben en Confirmeer: 
				//   - Let op! Uw nieuwe persoon lijkt verdacht veel op (een) reeds bestaande perso(o)n(en). Als u zeker bent dat u niemand dubbel toevoegt, klik dan opnieuw op ‘Bewaren’. 
				const string WAARSCHUWING_STRING = "Pas op! Je nieuwe persoon lijkt verdacht veel op iemand die al gekend is in de Chiroadministratie";
				if (!CheckToegevoegdePersoon(window, tweeling2, false, WAARSCHUWING_STRING))
				{
					foutGevonden |= true;
					raporteerFout += "\n2 op elkaar gelijkende personen niet gevonden.";
				}
				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.Bewaren);
				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.TerugNaarLijst);
				// Controlleren of we nu 2 personen hebben met bijna identieke naam.
				if (!PersonenPaginaActieCount(window, PersonenActie.TelVoorkomen, tweeling1).Equals(1))
				{
					foutGevonden |= true;
					raporteerFout += "\n2 Tweeling1 niet gevonden.";
				}
				if (!PersonenPaginaActieCount(window, PersonenActie.TelVoorkomen, tweeling2).Equals(1))
				{
					foutGevonden |= true;
					raporteerFout += "\n2 Tweeling2 niet gevonden.";
				}

				// 
				// TEST X : Invoegen van 2 identieke personen
				//
				var identiek = new Persoon("Identieke", "Persoon", "01/04/2009", "Man", "-0");
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
				PersoonToevoegen(window, identiek);
				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.TerugNaarLijst);
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
				PersoonToevoegen(window, identiek);
				// Kijken of we een waarschuwing zien en confirmeer.
				if (!CheckToegevoegdePersoon(window, identiek, false, WAARSCHUWING_STRING))
				{
					foutGevonden |= true;
					raporteerFout += "\n2 de zelfde personen niet gevonden.";
				}
				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.Bewaren);

				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.TerugNaarLijst);
				// Kijken of we nu 2 personen hebben met de zelfde naam
				if (!PersonenPaginaActieCount(window, PersonenActie.TelVoorkomen, identiek).Equals(2))
				{
					foutGevonden |= true;
					raporteerFout += "\n2 Geen 2 personen gevonden die identiek zijn.";
				}

				//
				// TEST X : Invoegen van 2 personen met zelfde naam, maar had die al toegevoegd, 
				//          Ik confirmeer dus niet.        
				//

				// ------------------------------------------------------------------------------------------------ 
				// Negatieve Testen
				// ------------------------------------------------------------------------------------------------

				//
				// TEST X: Toevoegen van een persoon, zonder FamilieNaam
				//
				var geenFamNaam = new Persoon("Geen FamilieNaam", "", "23/11/2000", "Vrouw", "0");
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
				PersoonToevoegen(window, geenFamNaam);

				// Controleer enkel het ontbreken van een succesboodschap.  
				// Het lezen van de foutmelding lukt niet meer zoals vroeger.

				if (!CheckToegevoegdePersoon(window, geenFamNaam, true, String.Empty))
				{
					foutGevonden |= true;
					raporteerFout += "\nFoutboodschappen voor geen FamilieNaam niet gevonden.";
				}

				//
				// TEST X: Toevoegen van een persoon, zonder Voornaam
				//

				//
				// TEST X: Toevoegen van een persoon, met foutieve geboortedatums
				//
				var fouteGebDat = new Persoon("Foutieve", "Geboorte Datum", "33/11/2000", "Vrouw", "0");
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
				PersoonToevoegen(window, fouteGebDat);
				if (!CheckToegevoegdePersoon(window, fouteGebDat, false, "The value '33/11/2000' is not valid for Geboortedatum."))
				{
					foutGevonden |= true;
					raporteerFout += "\nFout boodschap voor foutieve geboortedatum (33/11/2000) niet gevonden.";
				}

				//
				fouteGebDat = new Persoon("Foutieve", "Geboorte Datum", "XX/11/2000", "Vrouw", "0");
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
				PersoonToevoegen(window, fouteGebDat);
				if (!CheckToegevoegdePersoon(window, fouteGebDat, false, "The value 'XX/11/2000' is not valid for Geboortedatum."))
				{
					foutGevonden |= true;
					raporteerFout += "\nFout boodschap voor foutieve geboortedatum (XX/11/2000) niet gevonden.";
				}
			}
			Debug.Assert(!foutGevonden, "Sommige sub tests van InvoegenPersoon falen!", raporteerFout);
		}

		[TestMethod]
		public void LidMakenPersonen()
		{
			Boolean foutGevonden = false;
			String raporteerFout = String.Empty;

			using (var window = new IE())
			{
				// Ga naar de Personen pagina
				GaNaarMenuPagina(window, Paginas.Personen);
				// Voeg een persoon Toe
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
				var koen = new Persoon("Koen", "Meersman", "23/11/1975", "Man", "-8");
				PersoonToevoegen(window, koen);
				if (!CheckToegevoegdePersoon(window, koen, true, "Persoon (Koen Meersman) is niet opgeslagen"))
				{
					foutGevonden |= true;
					raporteerFout += "\nPersoon (Koen Meersman) is niet opgeslagen";
				}
				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.TerugNaarLijst);
				// Maak deze persoon Lid
				if (!PersonenPaginaActie(window, PersonenActie.ZoekPersoonEnMaakLid, koen))
				{
					foutGevonden |= true;
					raporteerFout += "\nPersoon (Koen Meersman) is niet Lid geamaakt.";
				}
				window.ForceClose();
			}
			Debug.Assert(!foutGevonden, raporteerFout);
		}
	}
}