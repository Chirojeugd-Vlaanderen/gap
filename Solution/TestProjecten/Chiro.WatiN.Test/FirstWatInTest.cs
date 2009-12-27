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
        private static int GapWebAppPort = 58895;
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
            String Gg2GestartDoor = Environment.GetEnvironmentVariable("    "); 
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

        /// <summary>
        /// Definitie van de beschikbare paginas:
        /// </summary>
        public enum Paginas
        {
            Afdelingen, 
            Personen, 
            Leden
        }

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
            window.GoTo(GapWebAppDeploymentServer);

            // Op deze pagina is er een unordered list (ul) met id menu.
            Element MenuLijst = window.Element(Find.ById("menu"));
            switch (pagina)
            {
                case Paginas.Afdelingen:
                    {
                        // Code om naar personen pagina te gaan
                        MenuLijst.DomContainer.Link(Find.ByText(new Regex("^Afdelingen$"))).Click();
                        break;
                    }
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

        public enum PersonenActie
        {
            NieuwePersoon,
            ZoekPersoonEnMaakLid
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

        public Boolean PersonenPaginaActie(IE window, PersonenActie actie,
            String Voornaam, String FamilieNaam, String GeboorteDatum, String Geslacht)
        {
            switch (actie)
            {
                case PersonenActie.NieuwePersoon:
                    {
                        // Dit is enkel gesupporteerd bij PersonenPaginaActie(IE window, PersonenActie actie)
                        throw new NotImplementedException ("Persoons gegevens niet nodig.");
                    }
                case PersonenActie.ZoekPersoonEnMaakLid:
                    {
                        // Vanaf dat we 1 persoon hebben getracht lid te maken stoppen we er mee.
                        bool PersoonLidGemaakt = false;
                        // Resultaat van lid maken
                        bool PersoonLidGemaaktRestultaat = false;

                        // Aanmaken van een reguliere expressie die we gaan gebruiken om een persoon op te zoeken, 
                        // Hier gebruiken we enkel nogmaar de Naam en VoorNaam.
                        Regex VindPersoon = new Regex(Voornaam + " " + FamilieNaam);

                        // Als we dit vinden in de volgende pagina dat in 
                        Regex VindIsToegevoegdAlsLid = new Regex("is toegevoegd als lid.");
                        
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
                                    if (PersonenLijstEnum.Current.ElementOfType<Link>(Find.ByText(new Regex("^Lid maken$"))).Exists)
                                    {
                                        PersonenLijstEnum.Current.ElementOfType<Link>(Find.ByText(new Regex("^Lid maken$"))).Click();

                                        // Als een persoon is toegevoed gaan we terug naar het begin scherm, en hebben we een veld waar:
                                        // '<Voornaam> <Naam> is toegevoegd als lid.' in staat.
                                        // We controlleren hier voor 2 dingen:
                                        //   - of de juiste naam van de persoon in de feedback staat
                                        //   - of de string: 'is toegevoegd als lid.' in de feedback staat.
                                        PersoonLidGemaaktRestultaat = (VindIsToegevoegdAlsLid.IsMatch(window.Element(Find.ByClass(new Regex("^Feedback$"))).Text) &&
                                        VindPersoon.IsMatch(window.Element(Find.ByClass(new Regex("^Feedback$"))).Text));

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
                            Debug.WriteLine("ASSERT ERROR: Niet lid gemaakt:" + Voornaam + " " + FamilieNaam);
                        }
                        return PersoonLidGemaaktRestultaat;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
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
        public void PersoonVoegToe(IE window, string VoorNaam, string FamilieNaam, string GeboorteDatum,
            string Geslacht, string ChiroLeeftijd)
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
            Field_Voornaam.TypeText(VoorNaam);

            Field_Naam.Click();
            Field_Naam.TypeText(FamilieNaam);

            Field_GebDat.Click();
            Field_GebDat.TypeText(GeboorteDatum);

            if (("Man".Equals(Geslacht)) || ("Vrouw".Equals(Geslacht))) 
            {
                window.RadioButton(Find.ByName(new Regex("Geslacht$")) && Find.ByValue(Geslacht)).Checked = true;
            }

            Field_ChiroLeeftijd.Click();
            Field_ChiroLeeftijd.TypeText(ChiroLeeftijd);

            // We bewaren de toestand in de database
            Button_Bewaren.Click();
        }

        public enum ToegevoegdePersoonActie
        {
            NieuwAdres,
            CommunicatieVormToevoegen,
            ToevoegenCategorie,
            TerugNaarLijst,
            GegevensWijzigen
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <param name="success"></param>
        /// <param name="FoutBoodschap"></param>
        /// <param name="actie"></param>
        /// <returns></returns>
        public Boolean CheckToegevoegdePersoonEnActie(IE window, Boolean success, string FoutBoodschap, 
            string VoorNaam, string FamilieNaam, string GeboorteDatum, string Geslacht, string ChiroLeeftijd,
            ToegevoegdePersoonActie actie)
        {
            Boolean resultaat = true;
            String PaginaBevatString = string.Empty;
            
            if (success) {
                PaginaBevatString = "Wijzigingen zijn opgeslagen";
            }
            else
            {
                PaginaBevatString = "Wijzigingen zijn niet opgeslagen";
            }

            if (!string.Equals(window.Element(Find.ByClass(new Regex("Feedback"))).Text, PaginaBevatString))
            {
                Debug.WriteLine("ASSERT ERROR: " + FoutBoodschap);
                resultaat = false;
            }
            else
            {
                // in een fieldset geven we de ingevoegde gegevens ter bevestiging.
                // Kijk Voornaam correct is.
                if (!window.TextField(Find.ById(new Regex("^HuidigePersoon_Persoon_VoorNaam$"))).Value.Equals(VoorNaam))
                {
                    Debug.WriteLine("ASSERT ERROR: Voornaam is: "
                        + window.TextField(Find.ById(new Regex("^HuidigePersoon_Persoon_VoorNaam$"))).Value
                        + " en we verwachten: " + VoorNaam);
                    resultaat = false;
                }

                // Kijk Naam correct is.
                if (!window.TextField(Find.ById(new Regex("^HuidigePersoon_Persoon_Naam$"))).Value.Equals(FamilieNaam))
                {
                    Debug.WriteLine("ASSERT ERROR: Naam is: "
                        + window.TextField(Find.ById(new Regex("^HuidigePersoon_Persoon_Naam$"))).Value
                        + " en we verwachten: " + FamilieNaam);
                    resultaat = false;
                }

                // Kijk GeboorteDatum correct is.
                if (!"".Equals(GeboorteDatum)) if (!window.TextField(Find.ById(new Regex("^HuidigePersoon_Persoon_GeboorteDatum$"))).Value.Equals(GeboorteDatum))
                {
                    Debug.WriteLine("ASSERT ERROR: GeboorteDatum is: "
                        + window.TextField(Find.ById(new Regex("^^HuidigePersoon_Persoon_GeboorteDatum$"))).Value
                        + " en we verwachten: " + GeboorteDatum);
                    resultaat = false;
                }

                // Kijk Geslacht correct is.
                if (!window.TextField(Find.ById(new Regex("^HuidigePersoon_Persoon_Geslacht$"))).Value.Equals(Geslacht))
                {
                    Debug.WriteLine("ASSERT ERROR: Geslacht is: "
                        + window.TextField(Find.ById(new Regex("^HuidigePersoon_Persoon_Geslacht$"))).Value
                        + " en we verwachten: " + Geslacht);
                    resultaat = false;
                }

                // Kijk ChiroLeeftijd correct is.
                if (!window.TextField(Find.ById(new Regex("^HuidigePersoon_ChiroLeefTijd$"))).Value.Equals(ChiroLeeftijd))
                {
                    Debug.WriteLine("ASSERT ERROR: ChiroLeeftijd is: "
                        + window.TextField(Find.ById(new Regex("^HuidigePersoon_ChiroLeefTijd$"))).Value
                        + " en we verwachten: " + ChiroLeeftijd);
                    resultaat = false;
                }
            }

            switch (actie)
            {
                case ToegevoegdePersoonActie.TerugNaarLijst:
                    {
                        window.Link(Find.ByText(new Regex("Terug naar de lijst"))).Click();
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException();
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
                GaNaarMenuPagina(window, Paginas.Afdelingen);
                GaNaarMenuPagina(window, Paginas.Leden);

                //
                // TEST 1: Toevoegen van een persoon, van het type mannelijk.
                //

                // Ga naar de Personen pagina
                GaNaarMenuPagina(window, Paginas.Personen);
                // We wensen een persoon toe te voegen.
                PersonenPaginaActie(window, PersonenActie.NieuwePersoon);

                PersoonVoegToe(window, "Koen", "Meersman", "23/11/1975", "Man", "23");
                // Na het bewaren moeten we wachten tot dat we een resutltaat gekregen hebben
                if (!CheckToegevoegdePersoonEnActie(window, true, "Persoon (Koen Meersman) is niet opgeslagen",
                    "Koen", "Meersman", "23/11/1975 0:00:00", "Man", "23",
                    ToegevoegdePersoonActie.TerugNaarLijst))
                {
                    FoutGevonden |= true;
                    RaporteerFout += "\nPersoon (Koen Meersman) is niet opgeslagen";
                }

                //
                // TEST 2: Toevoegen van een persoon van het type vrouwelijk.
                //
                PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
                PersoonVoegToe(window, "Marleen", "Van Loock", "07/12/1977", "Vrouw", "0");
                if (!CheckToegevoegdePersoonEnActie(window, true, "Persoon (Marleen Van Loock) is niet opgeslagen",
                    "Marleen", "Van Loock", "7/12/1977 0:00:00", "Vrouw", "0",
                    ToegevoegdePersoonActie.TerugNaarLijst))
                {
                    FoutGevonden |= true;
                    RaporteerFout += "\nPersoon (Marleen Van Loock) is niet opgeslagen";
                }

                //
                // TEST 3: Toevoegen van een persoon van het type onzijdig.
                //
                PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
                PersoonVoegToe(window, "Onzijdige", "Persoon", "07/12/1977", "onzijdig", "0");
                if (!CheckToegevoegdePersoonEnActie(window, true, "Een onzijdige Persoon is niet opgeslagen",
                    "Onzijdige", "Persoon", "7/12/1977 0:00:00", "Onbekend", "0",
                    ToegevoegdePersoonActie.TerugNaarLijst))
                {
                    FoutGevonden |= true;
                    RaporteerFout += "\nEen onzijdige Persoon is niet opgeslagen";
                }

                //
                // TEST 4: Toevoegen van een persoon, die nog geboren moet worden ;-)
                //
                PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
                PersoonVoegToe(window, "Minder", "Dan Baby", "07/12/2077", "Vrouw", "0");
                if (!CheckToegevoegdePersoonEnActie(window, false, "Persoon met geboorte datum in de toekomst is opgeslagen",
                    "Minder", "Dan Baby", "7/12/2077 0:00:00", "Vrouw", "0",
                    ToegevoegdePersoonActie.TerugNaarLijst))
                {
                    FoutGevonden |= true;
                    RaporteerFout += "\nPersoon met geboorte datum in de toekomst kan met niet invoegen.";
                }

                //
                // TEST 5: Toevoegen van een persoon, zonder geboortedatum
                //
                PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
                PersoonVoegToe(window, "Geen", "Geboorte Datum", "", "Vrouw", "0");
                if (!CheckToegevoegdePersoonEnActie(window, true, "Persoon zonder geboortedatum is niet opgeslagen", 
                    "Geen", "Geboorte Datum", "", "Vrouw", "0",
                    ToegevoegdePersoonActie.TerugNaarLijst))
                {
                    FoutGevonden |= true;
                    RaporteerFout += "\nPersoon zonder geboortedatum is niet opgeslagen.";
                }

                //
                // TEST 6: Toevoegen van een persoon, met foutieve geboortedatums
                //
                PersonenPaginaActie(window, PersonenActie.NieuwePersoon);
                PersoonVoegToe(window, "Foutieve", "Geboorte Datum", "XX/11/2000", "Vrouw", "0");
                if (!CheckToegevoegdePersoonEnActie(window, false, "Persoon met foutieve Geboortedatum is opgeslagen",
                    "Foutieve", "Geboorte Datum", "XX/11/2000 0:00:00", "Vrouw", "0",
                    ToegevoegdePersoonActie.TerugNaarLijst))
                {
                    FoutGevonden |= true;
                    RaporteerFout += "\nPersoon met foutieve GeboorteDatum (XX/11/2000) is opgeslagen.";
                }

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

                PersoonVoegToe(window, "Koen", "Meersman", "23/11/1975", "Man", "23");
                if (!CheckToegevoegdePersoonEnActie(window, true, "Persoon (Koen Meersman) is niet opgeslagen",
                    "Koen", "Meersman", "23/11/1975 0:00:00", "Man", "23",
                    ToegevoegdePersoonActie.TerugNaarLijst))
                {
                    FoutGevonden |= true;
                    RaporteerFout += "\nPersoon (Koen Meersman) is niet opgeslagen";
                }

                // Maak deze persoon Lid
                if (!PersonenPaginaActie(window, PersonenActie.ZoekPersoonEnMaakLid,
                    "Koen", "Meersman", "23/11/1975", "Man"))
                {
                    FoutGevonden |= true;
                    RaporteerFout += "\nPersoon (Koen Meersman) is niet Lid geamaakt.";
                }


            }
            Debug.Assert(!FoutGevonden, RaporteerFout);
        }
    }
}
