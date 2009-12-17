using System;
using System.Diagnostics;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WatiN.Core;
using WatiN.Core.Logging;


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
        private static int GapServicesPort = 2734;
        private static string GapServicesDeploymentServer = "http://localhost:" + GapServicesPort;
        private static string GapServicesPhysicalPath;

        // Deployment server voor GapWebApp
        private static Process GapWebAppProcess;
        private static int GapWebAppPort = 2780;
        private static string GapWebAppDeploymentServer = "http://localhost:" + GapWebAppPort;
        private static string GapWebAppPhysicalPath;

        // Gemeenschappelijk voor alle Services
        private static string virtualDirectory = "/";
        private const string webDevServerPath = @"C:\Program Files\Common Files\Microsoft Shared\DevServer\9.0\WebDev.WebServer.EXE";
        private static string SolutionRoot;

        [ClassInitialize()]
        public static void StartWebServer(TestContext testContext)
        {
            // Probeer de Solution directory te vinden.
            EnvDTE.DTE dte = (EnvDTE.DTE)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE");
            SolutionRoot = System.IO.Path.GetDirectoryName(dte.Solution.FullName);
            
            // Als we de testen runnen in de debugging mode, dan start de DevServer automatisch.
            // Er zijn 2 mogelijkheden wanneer we niet in de debugger zitten, ofwel runnen we onze testen
            // via de VisualStudio UI, ofwel via een commandline tool genaamd: MSTest.
            // We veronderstellen dat als de environment variabele "CG2_GESTART_DOOR" de waarde DevEnv heeft, 
            // dan gebruiken we niet de VisualStudio UI.
            Boolean StartDevServer = false;
            String Gg2GestartDoor = Environment.GetEnvironmentVariable("CG2_GESTART_DOOR"); 
            if ( ! String.IsNullOrEmpty (Gg2GestartDoor)) 
            { 
                if (String.Equals(Gg2GestartDoor, "MSTest"))
                {
                    StartDevServer = true;
                }
            } else {
                if (!System.Diagnostics.Debugger.IsAttached)
                {
                    StartDevServer = true;
                }
            }

            if (StartDevServer)
            {

                GapServicesPhysicalPath = (SolutionRoot + @"\Chiro.Gap.Services").TrimEnd('\\');
                GapWebAppPhysicalPath = (SolutionRoot + @"\Chiro.Gap.WebApp").TrimEnd('\\');

                GapServicesProcess = new Process();
                string GapServicesArguments = string.Format("/port:{0} /path:\"{1}\" /vpath:{2}", GapServicesPort, GapServicesPhysicalPath, virtualDirectory);
                GapServicesProcess.StartInfo = new ProcessStartInfo(webDevServerPath, GapServicesArguments);
                GapServicesProcess.Start();

                GapWebAppProcess = new Process();
                string GapWebAppArguments = string.Format("/port:{0} /path:\"{1}\" /vpath:{2}", GapWebAppPort, GapWebAppPhysicalPath, virtualDirectory);
                GapWebAppProcess.StartInfo = new ProcessStartInfo(webDevServerPath, GapWebAppArguments);
                GapWebAppProcess.Start();
            }
        }

        [ClassCleanup()]
        public static void StopWebServer()
        {
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
            // Testdata is een lijst van personen, (Voornaam, Naam, Geb, Geslacht, ChiroLeeftijd)
            // die we in deze test gaan invoegen.
            string[,] Testdata = {{"Koen", "Meersman", "23/11/1975", "Man", "23"},
                                  {"Marleen", "Van Loock", "07/12/1977", "Vrouw","0"}};

            // new IE(), maakt gebruik van externe resources (en implementeerd IDisposable), 
            // zet dit dus in een 'using' block.  
            using (IE window = new IE())
            {
                // We gaan we moeten controlleren of de normale gebruiker dit kan doen, en moeten 
                // er dus opletten dat we naar een generieke link gaan, daarna door klikken naar de 
                // gewenste pagina. 
                // Als we direct naar de link surfen, weten we niet of deze bereikbaar via de normale
                // weg.  (2e reden: een aantal linken zijn afhankelijk van wie er op de site zit.)
                window.GoTo(GapWebAppDeploymentServer);

                for (int gegevenslijn = 0; gegevenslijn < Testdata.GetLength(0); gegevenslijn++)
                {
                    // Klik op de 'Nieuwe persoon' link
                    // Bij het zoeken van linken/id's/... op een pagina moeten we er met rekening houden dat 
                    // die pagina's gegenereerd (toegekend) worden door de compiler, en dat die id's wel eens 
                    // een beetje van naam kunnen veranderen.
                    // Zoek daarom het beste via een reguliere expressie die minimaal bevat wat nodig is.
                    Link Personen_Nieuw = window.Link(Find.ByText(new Regex("Nieuwe persoon$")));
                    Personen_Nieuw.Click();

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
                    Field_Voornaam.TypeText(Testdata[gegevenslijn, 0]);

                    Field_Naam.Click();
                    Field_Naam.TypeText(Testdata[gegevenslijn, 1]);

                    Field_GebDat.Click();
                    Field_GebDat.TypeText(Testdata[gegevenslijn, 2]);


                    window.RadioButton(Find.ByName(new Regex("Geslacht$")) && Find.ByValue(Testdata[gegevenslijn, 3])).Checked = true;

                    Field_ChiroLeeftijd.Click();
                    Field_ChiroLeeftijd.TypeText(Testdata[gegevenslijn, 4]);

                    // We bewaren de toestand in de database
                    Button_Bewaren.Click();

                    // Na het bewaren moeten we wachten tot dat we een resutltaat gekregen hebben
                    Assert.IsTrue(String.Equals(window.Element(Find.ByClass(new Regex("Feedback"))).Text, "Wijzigingen zijn opgeslagen"),
                        "Persoon (" + Testdata[gegevenslijn, 1] + " - " + Testdata[gegevenslijn, 2] + ")is niet opgeslagen");

                    // Nu gaan we terug naar de lijst, zodat we klaar zijn voor het toevoegen van de volgende persoon.
                    // Voor de link te vinden, <a href="">DIT IS TEXT</a>
                    window.Link(Find.ByText(new Regex("Terug naar de lijst"))).Click();
                }
            }
        }

        [TestMethod]
        public void LidMakenPersonen()
        {
            // Testdata is een lijst van personen, (Voornaam, Naam) die we in deze test gaan lid maken.
            string[,] Testdata = {{"Koen", "Meersman"},
                                  {"Marleen", "Van Loock"}};

            Regex VindIsToegevoegdAlsLid = new Regex("is toegevoegd als lid.");

            using (IE window = new IE())
            {

                window.GoTo(GapWebAppDeploymentServer);

                for (int gegevenslijn = 0; gegevenslijn < Testdata.GetLength(0); gegevenslijn++)
                {
                    bool PersoonLidGemaakt = false;

                    // Aanmaken van een reguliere expressie die we gaan gebruiken om een persoon op te zoeken
                    Regex VindPersoon = new Regex(Testdata[gegevenslijn, 0] + " " + Testdata[gegevenslijn, 1]);

                    // Een volgende uitdaging is het vinden van een persoon in de lijst, deze kan namelijk op een andere pagina staan,
                    // de verschillende pagina nummers staat in een blok met naam: 'pager', van dit blok itereren we over alle links.
                    int link_nr = 0;
                    while (link_nr < window.Div(Find.ByClass(new Regex("pager"))).Links.Count && !PersoonLidGemaakt)
                    {
                        // Openen van de pagina. (het volgen van de link)
                        window.Div(Find.ByClass(new Regex("pager"))).Links[link_nr].Click();

                        // Kijk of we daar een 'Koen Meersman' tegen komen in de tabel met personen.
                        // Alle verschillende personen staan in een tabel, waarvan spijtig genoeg de table noch tbody noch tr een id hebben.
                        // Gaan zoeken over alle itereren over alle Tabel rijen.

                        IEnumerator<TableRow> PersonenLijstEnum = window.TableRows.GetEnumerator();
                        // Bij creatie van de PersonenLijstEnum, wordt die geinitialiseerd juist voor het eerste resultaat. 
                        // (resultaat is nog niet opgehaald) en als hij er geen meer kan geven geeft hij een false return.
                        while (PersonenLijstEnum.MoveNext() && !PersoonLidGemaakt)
                        {
                            // De naam van de persoon is de 2e in de tabel rij. (Elements[1])
                            // Let Op: Geen regex want we willen een exacte match
                            if (VindPersoon.IsMatch(PersonenLijstEnum.Current.Elements[1].ToString()))
                            {
                                // Maak de persoon lid, maar er bestaat een kans dat die persoon al lid is, 
                                // Dus voor de link gaan ophalen kijken of die wel bestaat.
                                if (PersonenLijstEnum.Current.ElementOfType<Link>(Find.ByText(new Regex("Lid maken"))).Exists)
                                {
                                    PersonenLijstEnum.Current.ElementOfType<Link>(Find.ByText(new Regex("Lid maken"))).Click();

                                    // Als een persoon is toegevoed gaan we terug naar het begin schrem, en hebben we een veld waar:
                                    // '<Voornaam> <Naam> is toegevoegd als lid.' in staat.
                                    Assert.IsTrue(VindIsToegevoegdAlsLid.IsMatch(window.Element(Find.ByClass(new Regex("Feedback"))).Text) &&
                                    VindPersoon.IsMatch(window.Element(Find.ByClass(new Regex("Feedback"))).Text));

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
                        Assert.Fail("Niet lid gemaakt:" + Testdata[gegevenslijn, 0] + " " + Testdata[gegevenslijn, 1]);
                    }
                }
            }
        }
    }
}
