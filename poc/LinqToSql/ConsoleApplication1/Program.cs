using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CgDal;

namespace ConsoleApplication1
{
    class Program
    {
        /// <summary>
        /// PersonenExperiment; doet een en ander met personen.
        /// (Wordt niet gebruikt in de huidige applicatie, maar staat
        /// er nog voor als het nog eens nodig is.)
        /// </summary>
        static void PersonenExperiment()
        {
            bool einde = false;
            int keuze, adres;
            string voornaam;
            PersoonsAdres persoonsAdres;

            PersonenServiceReference.PersonenServiceClient service = new ConsoleApplication1.PersonenServiceReference.PersonenServiceClient();

            do
            {
                Persoon persoon = service.PersoonMetAdressenGet(1894);
                Console.WriteLine(String.Format("{0} {1}", persoon.VoorNaam, persoon.Naam));

                foreach (PersoonsAdres persoonsadres in persoon.PersoonsAdres)
                {
                    Console.WriteLine(String.Format("Adres: {0}", persoonsadres.AdresID));

                    // Als ik nu persoonsadres.Adres.HuisNr wil accessen, krijg ik toch
                    // een null pointer dereference exception, wat ik niet goed begrijp.
                    
                }
                Console.WriteLine("(1) Adres toekennen, (2) Toegekenning verwijderen, (3) Voornaam Wijzigen, (0) einde: ");
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
                        case 3:
                            Console.WriteLine("Nieuwe Voornaam: ");
                            voornaam = Console.ReadLine();
                            persoon.VoorNaam = voornaam;
                            break;
                        default:
                            Console.WriteLine("Huh?");
                            break;
                    }

                    persoon.Status = EntityStatus.Gewijzigd;
                    service.PersoonUpdaten(persoon);
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
            PersonenServiceReference.PersonenServiceClient service = new ConsoleApplication1.PersonenServiceReference.PersonenServiceClient();

            var lijst = service.GelieerdePersonenInfoGet(310,1,50); // eerste pagina, paginagrootte = 50

            foreach (vPersoonsInfo i in lijst)
            {
                Console.WriteLine(i.VoorNaam + ' ' + i.Naam + ';' + i.Categorieen + ';' + i.StraatNaam + ' ' + i.HuisNr
                    + ';' + i.PostNr + ' ' + i.SubGemeente + ';' + i.TelefoonNummer + ';' + i.EMail);
            }
            Console.ReadLine();
        }

        static void Main(string[] Arguments)
        {
            LijstExperiment();
        }
    }
}
