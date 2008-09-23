using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            PersonenServiceReference.PersonenServiceClient service = new ConsoleApplication1.PersonenServiceReference.PersonenServiceClient();
            string NieuweVoorNaam;

            var persoon = service.PersoonGet(1894);

            Console.WriteLine(String.Format("{0} {1}", persoon.VoorNaam, persoon.Naam));
            Console.WriteLine("Nieuwe voornaam:");
            
            NieuweVoorNaam = Console.ReadLine();
            persoon.VoorNaam = NieuweVoorNaam;
            persoon.Status = PersonenServiceReference.EntityStatus.Gewijzigd;
            service.PersoonUpdaten(persoon);

            Console.WriteLine("Opnieuw ophalen:");
            persoon = service.PersoonGet(1894);
            Console.WriteLine(String.Format("{0} {1}", persoon.VoorNaam, persoon.Naam));

            Console.ReadLine();
        }
    }
}
