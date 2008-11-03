using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CgDal;

namespace ConsoleApplication1
{
    class Program
    {
        static readonly int testPersoonID = 1894;
        static readonly int testGroepID = 310;

        static PersonenServiceReference.PersonenServiceClient service
        {
            get
            {
                return new ConsoleApplication1.PersonenServiceReference.PersonenServiceClient();
            }
        }


        /// <summary>
        /// Toont naam, voornaam en adresID's van een persoon op de stdout
        /// </summary>
        /// <param name="p">Persoon waarvan gegevens gevraagd</param>
        static void PersoonTonen(Persoon p)
        {
            Console.WriteLine(String.Format("{2} - {0} {1}", p.VoorNaam
                , p.Naam, p.PersoonID));
            foreach (PersoonsAdres pa in p.PersoonsAdres)
            {
                Console.WriteLine(String.Format("Adres: {0}", pa.AdresID));
                //Console.WriteLine(String.Format("\tHuisNr: {0}"
                //    , pa.Adres.HuisNr));
            }
            foreach (CommunicatieVorm cv in p.CommunicatieVorms)
            {
                Console.WriteLine(cv.Nummer);
            }
        }


        // Hieronder een hele hoop experimenten, waarvan er 1 opgeroepen
        // wordt in main().

        /// <summary>
        /// Experiment met manipulaties van telefoonnrs.
        /// </summary>
        static void TelefoonNrExperiment()
        {
            Persoon p = service.PersoonMetDetailsGet(testPersoonID);

            Console.WriteLine("Oorspronkelijke persoon:");
            PersoonTonen(p);

            // Laatste nummer wijzigen
            p.CommunicatieVorms[p.CommunicatieVorms.Count - 1].Nummer = "015/339126";   //nummer wijzigen

            CommunicatieVorm nieuwNr = new CommunicatieVorm();
            nieuwNr.CommunicatieTypeID = 1; // telefoonnr.
            nieuwNr.Nummer = String.Format("Test {0}", DateTime.Now.TimeOfDay);

            p.CommunicatieVorms.Add(nieuwNr);

            service.PersoonUpdaten(p);

            Persoon q = service.PersoonMetDetailsGet(testPersoonID);

            Console.WriteLine("\nNr gewijzigd en toegevoegd:");
            PersoonTonen(q);

            // eerste nummer verwijderen
            q.CommunicatieVorms[0].TeVerwijderen = true;
            service.PersoonUpdaten(q);

            Persoon r = service.PersoonMetDetailsGet(testPersoonID);

            Console.WriteLine("\nEerste nr verwijderd:");
            PersoonTonen(r);
        }


        /// <summary>
        /// Test: wijzigen van de voornaam van een persoon.
        /// </summary>
        static void PersoonWijzigenExperiment()
        {
            Persoon np = new Persoon();

            //np.VoorNaam = "Jos";
            //np.Naam = "Bosmans";
            //np.GeslachtS = GeslachtsSoort.Man;
            //service.PersoonUpdaten(np);


            Persoon p = service.PersoonGet(testPersoonID);
            Persoon q = service.PersoonGet(testPersoonID);
            string nieuweVoornaam;

            PersoonTonen(p);

            Console.Write("Nieuwe voornaam: ");
            nieuweVoornaam = Console.ReadLine();

            p.VoorNaam = nieuweVoornaam;
            q.VoorNaam = "blabla";

            //service.PersoonUpdaten(q);
            service.PersoonUpdaten(p);




        }

        /// <summary>
        /// Test: toevoegen van een nieuwe persoon, en opnieuw verwijderen
        /// </summary>
        static void PersoonToevoegenVerwijderenExperiment()
        {
            Persoon p = new Persoon();

            p.Naam = "Bosmans";
            p.VoorNaam = "Jos";
            p.PersoonID = service.PersoonUpdaten(p);

            PersoonTonen(p);

            Persoon q = service.PersoonGet(p.PersoonID);
            service.PersoonUpdaten(q);

            Persoon r = service.PersoonGet(p.PersoonID);

            if (r == null)
            {
                Console.WriteLine(String.Format(
                    "Persoon met id {0} bestaat niet meer.", p.PersoonID));
            }
            else
            {
                PersoonTonen(r);
            }
        }


        /// <summary>
        /// LijstExperiment - haalt lijst over, en drukt af
        /// </summary>
        static void LijstExperiment()
        {
            var lijst = service.GelieerdePersonenInfoGet(testGroepID); // tweede pagina, paginagrootte = 50

            foreach (vPersoonsInfo i in lijst)
            {
                Console.WriteLine(String.Format("{0} {1}; {2} {3}; {4} {5}; {6}; {7}; {8}"
                    , i.VoorNaam, i.Naam, i.StraatNaam
                    , i.HuisNr, i.PostNr, i.SubGemeente, i.TelefoonNummer
                    , i.EMail, i.Categorieen));
            }
        }

        /// <summary>
        /// Wat is het probleem:
        /// 
        /// Er is een 1-op-1-relatie van Groep naar ChiroGroep.
        /// De foreign key wijst van ChiroGroep naar Groep.
        /// 
        /// Een ChiroGroep ophalen, incl. zijn groepsgegevens, werkt niet.
        /// De groepsgegevens komen niet mee over de lijn.
        /// 
        /// Een Groep ophalen, incl. gegevens van ChiroGroep werkt daarentegen
        /// wel.
        /// </summary>
        static void Rariteit()
        {
            using (GroepenServiceReference.GroepenServiceClient service
                = new ConsoleApplication1.GroepenServiceReference.GroepenServiceClient())
            {
                ChiroGroep cg = service.ChiroGroepGet(testGroepID);
                Groep g = service.ChiroGroepGroepGet(testGroepID);

                // werkt wel:
                Console.WriteLine(String.Format("ID ChiroGroep: {0}", g.ChiroGroep.chiroGroepID));

                if (cg.Groep == null)
                {
                    Console.WriteLine("Groepsgegevens niet doorgekomen!");
                }
                else
                {
                    // werkt niet:
                    Console.WriteLine(String.Format("ID Groep: {0}", cg.Groep.GroepID));
                }
            }
        }

        static void Main(string[] Arguments)
        {
            // PersoonWijzigenExperiment();
            // PersoonToevoegenVerwijderenExperiment();
            // AdressenExperiment();
            // LijstExperiment();

            // Rariteit();

            TelefoonNrExperiment();

            Console.ReadLine();
        }
    }
}
