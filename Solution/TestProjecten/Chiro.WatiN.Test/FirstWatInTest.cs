using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using WatiN.Core;
using WatiN.Core.Logging;

using Chiro.Gap.TestDbInfo;
using Chiro.WatiN.Test;

namespace Chiro.WatiN.Test
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class FirstWatInTest
	{
		public FirstWatInTest()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		private TestContext testContextInstance;

		/// <summary>
		/// Dit is een eerste voorbeeldje dat niks specifiek test, de bedoeling hier is enkel om aan te
		/// tonen hoe we de gemeenschappelijke delen van een test schrijven.
		///
		///</summary>
		///<remarks>
		///</remarks>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

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
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion
		// Deployment server voor GapServices
		private static Process GapServicesProcess;
		private static int GapServicesPort = Properties.Settings.Default.GapServicesPort;
		private static string GapServicesDeploymentServer = "http://localhost:" + GapServicesPort;
		private static string GapServicesPhysicalPath;

		// Deployment server voor GapWebApp
		private static Process GapWebAppProcess;
		private static int GapWebAppPort = Properties.Settings.Default.GapWebAppPort;
		private static string GapWebAppDeploymentServer = "http://localhost:" + GapWebAppPort;
		private static string GapWebAppPhysicalPath;

		// Gemeenschappelijk voor alle Services
		private static string virtualDirectory = "/";
		private const string webDevServerPath = @"C:\Program Files\Common Files\Microsoft Shared\DevServer\9.0\WebDev.WebServer.EXE";
		private static string SolutionRoot;

		[ClassInitialize()]
		public static void StartWebServer(TestContext testContext)
		{
			// De IDE wat tijd geven om correct zijn werk te doen, vooral op trage machines.
			System.Threading.Thread.Sleep(1000);

			// Probeer de Solution directory te vinden.
			EnvDTE.DTE dte = (EnvDTE.DTE)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE");
			SolutionRoot = System.IO.Path.GetDirectoryName(dte.Solution.FullName);

			// Als we de testen runnen in de debugging mode, dan start de DevServer automatisch.
			// Er zijn 2 mogelijkheden wanneer we niet in de debugger zitten, ofwel runnen we onze testen
			// via de VisualStudio UI, ofwel via een commandline tool genaamd: MSTest.
			// We veronderstellen dat als de environment variabele "CG2_GESTART_DOOR" de waarde DevEnv heeft, 
			// dan gebruiken we niet de VisualStudio UI.
			Boolean StartDevServer = false;
			String Gg2GestartDoor = Environment.GetEnvironmentVariable("    ");
			if (!String.IsNullOrEmpty(Gg2GestartDoor))
			{
				if (String.Equals(Gg2GestartDoor, "MSTest"))
				{
					StartDevServer = true;
				}
			}
			else
			{
				if (!System.Diagnostics.Debugger.IsAttached)
				{
					StartDevServer = true;
				}
			}
			Debug.WriteLine("StartWebserver: Moet server opstarten: " + StartDevServer.ToString());

			if (StartDevServer)
			{

				GapServicesPhysicalPath = (SolutionRoot + @"\Chiro.Gap.Services").TrimEnd('\\');
				Debug.WriteLine("StartWebserver: ServicesPhysicalPath: " + GapServicesPhysicalPath.ToString());
				GapWebAppPhysicalPath = (SolutionRoot + @"\Chiro.Gap.WebApp").TrimEnd('\\');
				Debug.WriteLine("StartWebserver: WebAppPhysicalPath: " + GapWebAppPhysicalPath.ToString());

				GapServicesProcess = new Process();
				string GapServicesArguments = string.Format("/port:{0} /path:\"{1}\" /vpath:{2}", GapServicesPort, GapServicesPhysicalPath, virtualDirectory);
				GapServicesProcess.StartInfo = new ProcessStartInfo(webDevServerPath, GapServicesArguments);
				GapServicesProcess.Start();

				GapWebAppProcess = new Process();
				string GapWebAppArguments = string.Format("/port:{0} /path:\"{1}\" /vpath:{2}", GapWebAppPort, GapWebAppPhysicalPath, virtualDirectory);
				GapWebAppProcess.StartInfo = new ProcessStartInfo(webDevServerPath, GapWebAppArguments);
				GapWebAppProcess.Start();

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

		[ClassCleanup()]
		public static void StopWebServer()
		{
			// Ontneem gebruiker GAV-rechten op groep voor watin-test
			TestHelper.OntneemGavRecht(TestInfo.WATINGROEPID);

			if (!System.Diagnostics.Debugger.IsAttached)
			{
				if (GapServicesProcess == null)
				{
					throw new InvalidOperationException("Start() must be called before Stop()");
				}
				GapServicesProcess.Kill();

				if (GapWebAppProcess == null)
				{
					throw new InvalidOperationException("Start() must be called before Stop()");
				}
				GapWebAppProcess.Kill();
			}
		}

		// Use TestCleanup to run code after each test has run
		[TestInitialize()]
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

		String MijnHomePage = null;
		// Hier schrijf ik een aantal help functies die mijn testen overzichtelijker maken
		/// <summary>
		/// Deze procedure gaat in het window naar de gevraagde pagina.
		/// </summary>
		/// <param name="window">Het browser window</param>
		/// <param name="Pagina">De pagina die je wil openen</param>
		public void GaNaarMenuPagina(IE window, Paginas pagina)
		{
			// We gaan we moeten controlleren of de normale gebruiker dit kan doen, en moeten 
			// er dus opletten dat we naar een generieke link gaan, daarna door klikken naar de 
			// gewenste pagina. 
			// Als we direct naar de link surfen, weten we niet of deze bereikbaar via de normale
			// weg.  (2e reden: een aantal linken zijn afhankelijk van wie er op de site zit.)
			if (String.IsNullOrEmpty(MijnHomePage))
			{
				window.GoTo(GapWebAppDeploymentServer);

				// Indien een gebruiker toegang heef tot meer dan 1 groep dan moet die nu een groep selecteren, 
				// indien dit zo is bevat de titel van de pagina 'Kies je Chirogroep | Nog geen Chirogroep geselecteerd'
				// en hij moet dan 'WATIN - St-WebAutomatedTestIndotNet ()' selecteren
				Regex SelecteerGroep = new Regex("Kies je Chirogroep");
				if (SelecteerGroep.Match(window.Title).Success)
				{
					window.Link(Find.ByText(new Regex("St-WebAutomatedTestIndotNet"))).Click();
					MijnHomePage = window.Url;
				}
			}
			else
			{
				window.GoTo(MijnHomePage);
			}

			// Op deze pagina is er een unordered list (ul) met id menu.
			Element MenuLijst = window.Element(Find.ById("menu"));

			switch (pagina)
			{
				case Paginas.Personen:
					{
						// Code om naar Personen pagina te gaan
						MenuLijst.DomContainer.Link(Find.ByText(new Regex("^Personen$"))).Click();
						break;
					}
				case Paginas.Leden:
					{
						// Code om naar Leden pagina te gaan
						MenuLijst.DomContainer.Link(Find.ByText(new Regex("^Leden$"))).Click();
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

			public Persoon(String Voornaam, String FamilieNaam, String GeboorteDatum, String Geslacht, String ChiroLeeftijd)
			{
				this.Voornaam = Voornaam;
				this.FamilieNaam = FamilieNaam;
				this.GeboorteDatum = GeboorteDatum;
				this.Geslacht = Geslacht;
				this.ChiroLeeftijd = ChiroLeeftijd;
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
			Element ActieLijst = window.Element(Find.ById(new Regex("^acties$")));

			switch (actie)
			{
				case PersonenActie.NieuwePersoon:
					{
						// Klik op de 'Nieuwe persoon' link
						// Bij het zoeken van linken/id's/... op een pagina moeten we er met rekening houden dat 
						// die pagina's gegenereerd (toegekend) worden door de compiler, en dat die id's wel eens 
						// een beetje van naam kunnen veranderen.
						// Zoek daarom het beste via een reguliere expressie die minimaal bevat wat nodig is.
						ActieLijst.DomContainer.Link(Find.ByText(new Regex("^Nieuwe persoon$"))).Click();
						break;
					}

				default:
					{
						throw new NotImplementedException();
					}
			}
		}

		public Boolean PersonenPaginaActie(IE window, PersonenActie actie, Persoon Pers)
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
						bool PersoonLidGemaakt = false;
						// Resultaat van lid maken
						bool PersoonLidGemaaktRestultaat = false;

						// Aanmaken van een reguliere expressie die we gaan gebruiken om een persoon op te zoeken, 
						// Hier gebruiken we enkel nogmaar de Naam en VoorNaam.
						Regex VindPersoon = new Regex(Pers.Voornaam + " " + Pers.FamilieNaam);

						// Als we dit vinden in de volgende pagina dat in 
						Regex VindIsToegevoegdAlsLid = new Regex(@"is nu ingeschreven als (lid|leiding|kind)");

						// Een volgende uitdaging is het vinden van een persoon in de lijst, deze kan namelijk op een andere pagina staan,
						// de verschillende pagina nummers staat in een blok met naam: 'pager', van dit blok itereren we over alle links.
						int link_nr = 0;
						while (link_nr < window.Div(Find.ByClass(new Regex("^pager$"))).Links.Count && !PersoonLidGemaakt)
						{
							// Openen van de pagina. (het volgen van de link)
							window.Div(Find.ByClass(new Regex("^pager$"))).Links[link_nr].Click();

							// Kijk of we daar een '<Voornaam> <Naam>' tegen komen in de tabel met personen.
							// Alle verschillende personen staan in een tabel, waarvan spijtig genoeg de table noch tbody noch tr een id hebben.
							// Gaan zoeken over alle itereren over alle Tabel rijen.

							IEnumerator<TableRow> PersonenLijstEnum = window.TableRows.GetEnumerator();
							// Bij creatie van de PersonenLijstEnum, wordt die geinitialiseerd juist voor het eerste resultaat. 
							// (resultaat is nog niet opgehaald) en als hij er geen meer kan geven geeft hij een false return.
							// De eerste rij is altijd de table header, dus ga direct naar de volgende.
							PersonenLijstEnum.MoveNext();
							while (PersonenLijstEnum.MoveNext() && !PersoonLidGemaakt)
							{
								// De naam van de persoon is de 3e in de tabel rij. (Find.ByIndex(2))
								// Let Op: Geen regex want we willen een exacte match
								if (VindPersoon.IsMatch(PersonenLijstEnum.Current.TableCell(Find.ByIndex(2)).ToString()))
								{
									// TODO: Nakijken of GeboorteDatum en Geslacht correct zijn.

									// Maak de persoon lid, maar er bestaat een kans dat die persoon al lid is, 
									// Dus voor de link gaan ophalen kijken of die wel bestaat.
									if (PersonenLijstEnum.Current.ElementOfType<Link>(Find.ByText(new Regex("^Leiding maken$"))).Exists)
									{
										PersonenLijstEnum.Current.ElementOfType<Link>(Find.ByText(new Regex("^Leiding maken$"))).Click();

										// Als een persoon is toegevoed gaan we terug naar het begin scherm, en hebben we een veld waar:
										// '<Voornaam> <Naam> is toegevoegd als lid.' in staat.
										// We controlleren hier voor 2 dingen:
										//   - of de juiste naam van de persoon in de feedback staat
										//   - of de string: 'is toegevoegd als lid.' in de feedback staat.

										string feedBackText = window.Element(Find.ByClass(new Regex("^Feedback$"))).Text;

										PersoonLidGemaaktRestultaat = VindIsToegevoegdAlsLid.IsMatch(window.Element(Find.ByClass(new Regex("^Feedback$"))).Text);

										// We hebben een poging gedaan om de persoon lid te maken.
										PersoonLidGemaakt = true;
									}
									else
									{
										// Niet duidelijk wat ik moet doen, het is mogelijk om in een groep verschillende 
										// personen te hebben met de zelfde naam.  Maar als we hem uiteindelijk niet vinden dan 
										// moeten we een fout tonen.
									}
								}
							}
							link_nr = link_nr + 1;
						}
						// We hebben alle pagina's doorlopen en we hebben de persoon niet kunnen lid maken.
						if (!PersoonLidGemaakt)
						{
							Debug.WriteLine("ASSERT ERROR: Niet lid gemaakt:" + Pers.Voornaam + " " + Pers.FamilieNaam);
						}
						return PersoonLidGemaaktRestultaat;
					}
				default:
					{
						throw new NotImplementedException();
					}
			}
		}

		public int PersonenPaginaActieCount(IE window, PersonenActie actie, Persoon Pers)
		{
			int AantalGevonden = 0;

			// Aanmaken van een reguliere expressie die we gaan gebruiken om een persoon op te zoeken, 
			// Hier gebruiken we enkel nogmaar de Naam en VoorNaam.
			Regex VindPersoon = new Regex(Pers.Voornaam + " " + Pers.FamilieNaam);

			// Een volgende uitdaging is het vinden van een persoon in de lijst, deze kan namelijk op een andere pagina staan,
			// de verschillende pagina nummers staat in een blok met naam: 'pager', van dit blok itereren we over alle links.
			int link_nr = 0;
			while (link_nr < window.Div(Find.ByClass(new Regex("^pager$"))).Links.Count)
			{
				// Openen van de pagina. (het volgen van de link)
				window.Div(Find.ByClass(new Regex("^pager$"))).Links[link_nr].Click();

				// Kijk of we daar een '<Voornaam> <Naam>' tegen komen in de tabel met personen.
				// Alle verschillende personen staan in een tabel, waarvan spijtig genoeg de table noch tbody noch tr een id hebben.
				// Gaan zoeken over alle itereren over alle Tabel rijen.

				IEnumerator<TableRow> PersonenLijstEnum = window.TableRows.GetEnumerator();
				// Bij creatie van de PersonenLijstEnum, wordt die geinitialiseerd juist voor het eerste resultaat. 
				// (resultaat is nog niet opgehaald) en als hij er geen meer kan geven geeft hij een false return.
				// De eerste rij is altijd de table header, dus ga direct naar de volgende.
				PersonenLijstEnum.MoveNext();
				while (PersonenLijstEnum.MoveNext())
				{
					// De naam van de persoon is de 3e in de tabel rij. (Find.ByIndex(2))
					// Let Op: Geen regex want we willen een exacte match
					if (VindPersoon.IsMatch(PersonenLijstEnum.Current.TableCell(Find.ByIndex(2)).ToString()))
					{
						// Een persoon gevonden
						AantalGevonden += 1;
					}
				}
				link_nr += 1;
			}
			return AantalGevonden;
		}


		/// <summary>
		///  Preconditie we zitten in toevoegen van nieuwe persoon.
		/// </summary>
		/// <param name="window"></param>
		/// <param name="Voornaam"></param>
		/// <param name="Familienaam"></param>
		/// <param name="Geboortedatum"></param>
		/// <param name="Geslacht">Het geslacht, geldig zijn: 'Man' en 'Vrouw' alle andere waarden resulteren in geen selectie.</param>
		/// <param name="ChiroLeeftijd"></param>
		public void PersoonVoegToe(IE window, Persoon Pers)
		{
			// Na het klikken op Nieuwe Persoon, krijgen we een scherm met de volgende gegevens:
			TextField Field_Voornaam = window.TextField(Find.ByName(new Regex("\\.VoorNaam$")));
			TextField Field_Naam = window.TextField(Find.ByName(new Regex("\\.Naam$")));
			TextField Field_GebDat = window.TextField(Find.ByName(new Regex("\\.GeboorteDatum$")));
			// De RadioButton's kunnen we op onderstaande mannier vinden, maar vermits de naam overeen komt met wat 
			// er in de Test tabel staat kan ik het ook op een andere manier doen (zie lager).
			// RadioButton Button_Geslacht_Man = window.RadioButton(Find.ByName("HuidigePersoon.Persoon.Geslacht") && Find.ByValue("Man"));
			// RadioButton Button_Geslacht_Vrouw = window.RadioButton(Find.ByName("HuidigePersoon.Persoon.Geslacht") && Find.ByValue("Vrouw"));
			TextField Field_ChiroLeeftijd = window.TextField(Find.ByName(new Regex("\\.ChiroLeefTijd$")));
			Button Button_Bewaren = window.Button(Find.ByValue(new Regex("Bewaren$")));

			// Eenvoudig testje, we gaan de volgende persoon invoegen:
			//  Voornaam, Naam, GeboorteDatum, Geslacht, ChiroLeeftijd
			Field_Voornaam.Click();
			Field_Voornaam.TypeText(Pers.Voornaam);

			Field_Naam.Click();
			Field_Naam.TypeText(Pers.FamilieNaam);

			Field_GebDat.Click();
			Field_GebDat.TypeText(Pers.GeboorteDatum);

			if (("Man".Equals(Pers.Geslacht)) || ("Vrouw".Equals(Pers.Geslacht)))
			{
				window.RadioButton(Find.ByName(new Regex("Geslacht$")) && Find.ByValue(Pers.Geslacht)).Checked = true;
			}

			Field_ChiroLeeftijd.Click();
			Field_ChiroLeeftijd.TypeText(Pers.ChiroLeeftijd);

			// We bewaren de toestand in de database
			Button_Bewaren.Click();
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
						window.Link(Find.ByText(new Regex("Terug naar de lijst"))).Click();
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

		public Boolean CheckToegevoegdePersoon(IE window, Persoon Pers, Boolean success, string FoutBoodschap)
		{
			Boolean resultaat = true;

			if (success)
			{
				// Kijk of de pagina de string: "Wijzigingen zijn opgeslagen" bevat.
				if (!string.Equals(window.Element(Find.ByClass(new Regex("Feedback"))).Text, "Wijzigingen zijn opgeslagen"))
				{
					Debug.WriteLine("ASSERT ERROR: " + FoutBoodschap);
					resultaat = false;
				}
				else
				{
					// in een fieldset geven we de ingevoegde gegevens ter bevestiging.
					// Kijk Voornaam correct is.
					if (!window.TextField(Find.ById(new Regex("^HuidigePersoon_Persoon_VoorNaam$"))).Value.Equals(Pers.Voornaam))
					{
						Debug.WriteLine("ASSERT ERROR: Voornaam is: "
						    + window.TextField(Find.ById(new Regex("^HuidigePersoon_Persoon_VoorNaam$"))).Value
						    + " en we verwachten: " + Pers.Voornaam);
						resultaat = false;
					}

					// Kijk Naam correct is.
					if (!window.TextField(Find.ById(new Regex("^HuidigePersoon_Persoon_Naam$"))).Value.Equals(Pers.FamilieNaam))
					{
						Debug.WriteLine("ASSERT ERROR: Naam is: "
						    + window.TextField(Find.ById(new Regex("^HuidigePersoon_Persoon_Naam$"))).Value
						    + " en we verwachten: " + Pers.FamilieNaam);
						resultaat = false;
					}

					// Kijk GeboorteDatum correct is.
					if (!"".Equals(Pers.GeboorteDatum))
					{
						String BewaardeGebDat = window.TextField(Find.ById(new Regex("^HuidigePersoon_Persoon_GeboorteDatum$"))).Value;
						// De Datums op de webpagina's bevatten ook een uur
						Regex Verwacht = new Regex("[1-9]{0,1}[0-9]/[0-9]{2}/[0-9]{4} 0:00:00");
						if (!Verwacht.IsMatch(Pers.GeboorteDatum + " 0:00:00"))
						{
							Debug.WriteLine("ASSERT ERROR: GeboorteDatum is: "
							    + window.TextField(Find.ById(new Regex("^HuidigePersoon_Persoon_GeboorteDatum$"))).Value
							    + " en we verwachten: " + Pers.GeboorteDatum);
							resultaat = false;
						}
					}
					// Kijk Geslacht correct is.
					String BewaardGeslacht = window.TextField(Find.ById(new Regex("^HuidigePersoon_Persoon_Geslacht$"))).Value;
					switch (Pers.Geslacht)
					{
						case "Man":
						case "Vrouw":
							{
								// Dit is een geldige waarde, en dat moet overeenkomen
								if (!BewaardGeslacht.Equals(Pers.Geslacht))
								{
									Debug.WriteLine("ASSERT ERROR: Geslacht is: "
								   + BewaardGeslacht + " en we verwachten: " + Pers.Geslacht);
									resultaat = false;
								}
								break;
							}
						case "Onzijdig":
							{
								if (!BewaardGeslacht.Equals("Onbekend"))
								{
									Debug.WriteLine("ASSERT ERROR: Geslacht is: "
								+ BewaardGeslacht + " en we verwachten: " + Pers.Geslacht);
									resultaat = false;
								}
								break;
							}

						default:
							{
								throw new NotImplementedException();
							}

					}

					// Kijk ChiroLeeftijd correct is.
					if (!window.TextField(Find.ById(new Regex("^HuidigePersoon_ChiroLeefTijd$"))).Value.Equals(Pers.ChiroLeeftijd))
					{
						Debug.WriteLine("ASSERT ERROR: ChiroLeeftijd is: "
						    + window.TextField(Find.ById(new Regex("^HuidigePersoon_ChiroLeefTijd$"))).Value
						    + " en we verwachten: " + Pers.ChiroLeeftijd);
						resultaat = false;
					}
				}
			}
			else
			{
				// Deze pagina bevat een div met class naam 'validation-summary-errors', 
				// en daarin staat dan een lijst met de fouten.
				Element FoutMeldingElement = window.Element(Find.ByClass(new Regex("validation-summary-errors")));

				if (!FoutMeldingElement.Exists)
				{
					Debug.WriteLine("ASSERT ERROR: " + FoutBoodschap);
					Debug.WriteLine("              validation-summary-errors Class niet gevonden.");
					resultaat = false;
				}
				else
				{
					// Soms staat de validation-summary-errors in een <DIV>
					// en soms in een <P>
					switch (FoutMeldingElement.TagName)
					{
						case "DIV":
							resultaat = false;
							Div FoutMelding = window.Div(Find.ByClass(new Regex("validation-summary-errors")));
							IEnumerator<Element> ErrorElements = FoutMelding.Elements.GetEnumerator();
							while (ErrorElements.MoveNext())
							{
								// De fout boodschappen ziten in een <LI>
								if (ErrorElements.Current.TagName.Equals("LI"))
								{
									if (ErrorElements.Current.OuterText.Equals(FoutBoodschap))
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
							if (!FoutMeldingElement.InnerHtml.Equals(FoutBoodschap))
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
						Debug.WriteLine("ASSERT ERROR: " + FoutBoodschap);
						Debug.WriteLine("              validation-summary-errors Gevonden maar bevat niet de verwachte boodschap.");
						resultaat = false;
					}
				}
			}
			return resultaat;
		}

		[TestMethod]
		// De naam van de test moet een inidicatie zijn van wat er getest gaat worden.
		// Als de test faalt dan weten we ook in eens wat er niet meer werkt. 
		//
		// Tracht ook te vermijden om te veel te gaan testen in 1 test.  Het is beter
		// meer simpele verstaanbare testen te schrijven i.p.v. 1 grote niet te begrijpen
		// test.

		///<summary>
		/// InvoegenPersonen gaat test of we de lijst van personen (Testdata) met success kunnen 
		/// invoegen.
		/// </summary>
		/// 
		public void InvoegenPersonen()
		{

			Boolean FoutGevonden = false;
			String RaporteerFout = String.Empty;

			// new IE(), maakt gebruik van externe resources (en implementeerd IDisposable), 
			// zet dit dus in een 'using' block.  
			using (IE window = new IE())
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
				Persoon Koen = new Persoon("Koen", "Meersman", "23/11/1975", "Man", "-8");
				PersoonVoegToe(window, Koen);
				// Na het bewaren moeten we wachten tot dat we een resutltaat gekregen hebben
				if (!CheckToegevoegdePersoon(window, Koen, true, "Persoon (Koen Meersman) is niet opgeslagen"))
				{
					FoutGevonden |= true;
					RaporteerFout += "\nPersoon (Koen Meersman) is niet opgeslagen";
				}
				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.TerugNaarLijst);

				//
				// TEST 2: Toevoegen van een persoon van het type vrouwelijk.
				//
				Persoon Marleen = new Persoon("Marleen", "Van Loock", "07/12/1977", "Vrouw", "0");
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
				PersoonVoegToe(window, Marleen);
				if (!CheckToegevoegdePersoon(window, Marleen, true, "Persoon (Marleen Van Loock) is niet opgeslagen"))
				{
					FoutGevonden |= true;
					RaporteerFout += "\nPersoon (Marleen Van Loock) is niet opgeslagen";
				}
				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.TerugNaarLijst);

				//
				// TEST 3: Toevoegen van een persoon van het type onzijdig.
				//
				Persoon Onzijdig = new Persoon("Onzijdige", "Persoon", "07/12/1977", "Onzijdig", "0");
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);

				PersoonVoegToe(window, Onzijdig);
				if (!CheckToegevoegdePersoon(window, Onzijdig, true, "Een onzijdige Persoon is niet opgeslagen"))
				{
					FoutGevonden |= true;
					RaporteerFout += "\nEen onzijdige Persoon is niet opgeslagen";
				}
				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.TerugNaarLijst);

				//
				// TEST 4: Toevoegen van een persoon, die nog geboren moet worden ;-)
				//
				Persoon OnGeboren = new Persoon("Minder", "Dan Baby", "07/12/2077", "Vrouw", "0");
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
				PersoonVoegToe(window, OnGeboren);
				// In theorie moet dit falen, maar momenteel is dit nog success vol.
				if (!CheckToegevoegdePersoon(window, OnGeboren, true, "Persoon met geboorte datum in de toekomst is opgeslagen"))
				{
					FoutGevonden |= true;
					RaporteerFout += "\nPersoon met geboorte datum in de toekomst kan met niet invoegen.";
				}
				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.TerugNaarLijst);

				//
				// TEST 5: Toevoegen van een persoon, zonder geboortedatum
				//
				Persoon GeenGebDat = new Persoon("Geen", "Geboorte Datum", "", "Vrouw", "0");
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
				PersoonVoegToe(window, GeenGebDat);
				if (!CheckToegevoegdePersoon(window, GeenGebDat, true, "Persoon zonder geboortedatum is niet opgeslagen"))
				{
					FoutGevonden |= true;
					RaporteerFout += "\nPersoon zonder geboortedatum is niet opgeslagen.";
				}
				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.TerugNaarLijst);

				//
				// TEST X : Invoegen van 2 personen die op elkaar gelijken.
				//          Ik weet wel niet de definitie van 2 op elkaar lijkende personen.
				//          (En deze persoon confirmeren)
				//
				Persoon Tweeling1 = new Persoon("Tweeling", "Persoon1", "01/04/2009", "Man", "+2");
				Persoon Tweeling2 = new Persoon("Tweeling", "Persoon2", "01/04/2009", "Man", "+2");
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
				PersoonVoegToe(window, Tweeling1);
				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.TerugNaarLijst);
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
				PersoonVoegToe(window, Tweeling2);
				// Kijken of we een waarschuwing zoals onderstaande hebben en Confirmeer: 
				//   - Let op! Uw nieuwe persoon lijkt verdacht veel op (een) reeds bestaande perso(o)n(en). Als u zeker bent dat u niemand dubbel toevoegt, klik dan opnieuw op ‘Bewaren’. 
				String WaarschuwingString = "Let op! Uw nieuwe persoon lijkt verdacht veel op (een) reeds bestaande perso(o)n(en). "
				+ "Als u zeker bent dat u niemand dubbel toevoegt, klik dan opnieuw op ‘Bewaren’. ";
				if (!CheckToegevoegdePersoon(window, Tweeling2, false, WaarschuwingString))
				{
					FoutGevonden |= true;
					RaporteerFout += "\n2 op elkaar gelijkende personen niet gevonden.";
				}
				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.Bewaren);
				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.TerugNaarLijst);
				// Controlleren of we nu 2 personen hebben met bijna identieke naam.
				if (!PersonenPaginaActieCount(window, PersonenActie.TelVoorkomen, Tweeling1).Equals(1))
				{
					FoutGevonden |= true;
					RaporteerFout += "\n2 Tweeling1 niet gevonden.";
				}
				if (!PersonenPaginaActieCount(window, PersonenActie.TelVoorkomen, Tweeling2).Equals(1))
				{
					FoutGevonden |= true;
					RaporteerFout += "\n2 Tweeling2 niet gevonden.";
				}

				// 
				// TEST X : Invoegen van 2 identieke personen
				//
				Persoon Identiek = new Persoon("Identieke", "Persoon", "01/04/2009", "Man", "-0");
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
				PersoonVoegToe(window, Identiek);
				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.TerugNaarLijst);
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
				PersoonVoegToe(window, Identiek);
				// Kijken of we een waarschuwing zien en confirmeer.
				if (!CheckToegevoegdePersoon(window, Identiek, false, WaarschuwingString))
				{
					FoutGevonden |= true;
					RaporteerFout += "\n2 de zelfde personen niet gevonden.";
				}
				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.Bewaren);

				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.TerugNaarLijst);
				// Kijken of we nu 2 personen hebben met de zelfde naam
				if (!PersonenPaginaActieCount(window, PersonenActie.TelVoorkomen, Identiek).Equals(2))
				{
					FoutGevonden |= true;
					RaporteerFout += "\n2 Geen 2 personen gevonden die identiek zijn.";
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
				Persoon GeenFamNaam = new Persoon("Geen FamilieNaam", "", "23/11/2000", "Vrouw", "0");
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
				PersoonVoegToe(window, GeenFamNaam);
				// Kijken voor de volgende foutboodschappen:
				//  - 'Familienaam' moet minstens 2 tekens bevatten.
				//  - 'Familienaam' is een verplicht veld.
				if (!CheckToegevoegdePersoon(window, GeenFamNaam, false, "'Familienaam' moet minstens 2 tekens bevatten.")
				    & !CheckToegevoegdePersoon(window, GeenFamNaam, false, "'Familienaam' is een verplicht veld."))
				{
					FoutGevonden |= true;
					RaporteerFout += "\nFoutboodschappen voor geen FamilieNaam niet gevonden.";
				}
				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.TerugNaarLijst);

				//
				// TEST X: Toevoegen van een persoon, zonder Voornaam
				//

				//
				// TEST X: Toevoegen van een persoon, met foutieve geboortedatums
				//
				Persoon FouteGebDat = new Persoon("Foutieve", "Geboorte Datum", "33/11/2000", "Vrouw", "0");
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
				PersoonVoegToe(window, FouteGebDat);
				if (!CheckToegevoegdePersoon(window, FouteGebDat, false, "The value '33/11/2000' is not valid for Geboortedatum."))
				{
					FoutGevonden |= true;
					RaporteerFout += "\nFout boodschap voor foutieve geboortedatum (33/11/2000) niet gevonden.";
				}
				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.TerugNaarLijst);

				//
				FouteGebDat = new Persoon("Foutieve", "Geboorte Datum", "XX/11/2000", "Vrouw", "0");
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
				PersoonVoegToe(window, FouteGebDat);
				if (!CheckToegevoegdePersoon(window, FouteGebDat, false, "The value 'XX/11/2000' is not valid for Geboortedatum."))
				{
					FoutGevonden |= true;
					RaporteerFout += "\nFout boodschap voor foutieve geboortedatum (XX/11/2000) niet gevonden.";
				}
				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.TerugNaarLijst);

			}
			Debug.Assert(!FoutGevonden, "Sommige sub tests van InvoegenPersoon falen!", RaporteerFout);
		}

		[TestMethod]
		public void LidMakenPersonen()
		{
			Boolean FoutGevonden = false;
			String RaporteerFout = String.Empty;

			using (IE window = new IE())
			{
				// Ga naar de Personen pagina
				GaNaarMenuPagina(window, Paginas.Personen);
				// Voeg een persoon Toe
				PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
				Persoon Koen = new Persoon("Koen", "Meersman", "23/11/1975", "Man", "-8");
				PersoonVoegToe(window, Koen);
				if (!CheckToegevoegdePersoon(window, Koen, true, "Persoon (Koen Meersman) is niet opgeslagen"))
				{
					FoutGevonden |= true;
					RaporteerFout += "\nPersoon (Koen Meersman) is niet opgeslagen";
				}
				ToegevoegdePersoonActie(window, ToegevoegdePersoonActies.TerugNaarLijst);
				// Maak deze persoon Lid
				if (!PersonenPaginaActie(window, PersonenActie.ZoekPersoonEnMaakLid, Koen))
				{
					FoutGevonden |= true;
					RaporteerFout += "\nPersoon (Koen Meersman) is niet Lid geamaakt.";
				}
				window.ForceClose();
			}
			Debug.Assert(!FoutGevonden, RaporteerFout);
		}
	}
}
