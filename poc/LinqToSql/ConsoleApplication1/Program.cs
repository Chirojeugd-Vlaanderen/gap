using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CgDal;

namespace ConsoleApplication1
{
    class Program
    {
        static readonly int testPersoon = 1894;
        static readonly int testGroep = 301;

        static readonly PersonenServiceReference.PersonenServiceClient service
            = new ConsoleApplication1.PersonenServiceReference.PersonenServiceClient();

        /// <summary>
        /// Toont naam, voornaam en adresID's van een persoon op de stdout
        /// </summary>
        /// <param name="p">Persoon waarvan gegevens gevraagd</param>
        static void PersoonTonen(Persoon p)
        {
            Console.WriteLine(String.Format("{2} - {0} {1}", p.VoorNaam, p.Naam, p.PersoonID));
            foreach (PersoonsAdres pa in p.PersoonsAdres)
            {
                Console.WriteLine(String.Format("Adres: {0}", pa.AdresID));
                //Console.WriteLine(String.Format("\tHuisNr: {0}"
                //    , pa.Adres.HuisNr));
            }
        }

        /// <summary>
        /// Test: wijzigen van de voornaam van een persoon.
        /// </summary>
        static void PersoonWijzigenExperiment()
        {
            Persoon p = service.PersoonGet(testPersoon);
            string nieuweVoornaam;

            PersoonTonen(p);

            Console.Write("Nieuwe voornaam: ");
            nieuweVoornaam = Console.ReadLine();

            p.VoorNaam = nieuweVoornaam;
            p.Status = EntityStatus.Gewijzigd;
            service.PersoonUpdaten(ref p);

            Persoon q = service.PersoonGet(testPersoon);

            PersoonTonen(q);

        }

        /// <summary>
        /// Test: toevoegen van een nieuwe persoon, en opnieuw verwijderen
        /// </summary>
        static void PersoonToevoegenVerwijderenExperiment()
        {
            Persoon p = new Persoon();
            
            p.Naam = "Bosmans";
            p.VoorNaam = "Jos";
            p.Status = EntityStatus.Nieuw;
            service.PersoonUpdaten(ref p);

            PersoonTonen(p);

            Persoon q = service.PersoonGet(p.PersoonID);
            q.Status = EntityStatus.Verwijderd;
            service.PersoonUpdaten(ref q);

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
        /// Doet een en ander met personen.
        /// (Wordt niet gebruikt in de huidige applicatie, maar staat
        /// er nog voor als het nog eens nodig is.)
        /// </summary>
        static void AdressenExperiment()
        {
            bool einde = false;
            int keuze, adres;
            string voornaam;
            PersoonsAdres persoonsAdres;

            do
            {
                Persoon persoon = service.PersoonMetAdressenGet(testPersoon);
                PersoonTonen(persoon);

                Console.WriteLine("(1) Adres toekennen, (2) Toegekenning verwijderen, (0) einde: ");
                keuze = int.Parse(Console.ReadLine());

                if (keuze != 0)
                {
                    switch (keuze)
                    {
                        case 1:
                            Console.WriteLine("AdresId: ");
                            adres = int.Parse(Console.ReadLine());

                            persoonsAdres = new PersoonsAdres();
                            persoonsAdres.AdresID = adres;
                            persoonsAdres.AdresTypeID = 1;
                            persoonsAdres.IsStandaard = false;
                            persoonsAdres.PersoonID = persoon.PersoonID;
                            persoonsAdres.Status = EntityStatus.Nieuw;


                            // Om onderstaande method op te roepen, moet ik CgDal referencen!

                            persoon.PersoonsAdres.Add(persoonsAdres);
                            persoonsAdres.Status = EntityStatus.Nieuw;
                            break;
                        case 2:
                            Console.WriteLine("AdresId: ");
                            adres = int.Parse(Console.ReadLine());

                            persoonsAdres = persoon.PersoonsAdres.SingleOrDefault<PersoonsAdres>(
                                a => a.AdresID == adres);
                            persoonsAdres.Status = EntityStatus.Verwijderd;
                            break;
                        default:
                            Console.WriteLine("Huh?");
                            break;
                    }

                    persoon.Status = EntityStatus.Gewijzigd;
                    service.PersoonUpdaten(ref persoon);
                }
                else
                {
                    einde = true;
                }
            }
            while (!einde);
        }

        /// <summary>
        /// LijstExperiment - haalt lijst over, en drukt af
        /// </summary>
        static void LijstExperiment()
        {
            var lijst = service.GelieerdePersonenInfoGet(310,1,50); // eerste pagina, paginagrootte = 50

            foreach (vPersoonsInfo i in lijst)
            {
                Console.WriteLine(i.VoorNaam + ' ' + i.Naam + ';' + i.Categorieen + ';' + i.StraatNaam + ' ' + i.HuisNr
                    + ';' + i.PostNr + ' ' + i.SubGemeente + ';' + i.TelefoonNummer + ';' + i.EMail);
            }
        }

        static void Main(string[] Arguments)
        {
            // PersoonWijzigenExperiment();
            // PersoonToevoegenVerwijderenExperiment();

            AdressenExperiment();

            // LijstExperiment();

            Console.ReadLine();
        }
    }
}
