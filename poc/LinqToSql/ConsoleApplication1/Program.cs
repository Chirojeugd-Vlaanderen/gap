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

            var persoon = service.PersoonMetAdressenGet(1894);

            Console.WriteLine(String.Format("{0} {1}", persoon.VoorNaam, persoon.Naam));

            if (persoon.PersoonsAdres != null)
            {
                foreach (var persoonsadres in persoon.PersoonsAdres)
                {
                    Console.WriteLine(String.Format("Adres: {0} - huisnummer {1}", persoonsadres.AdresID));
                }
            }

            Console.ReadLine();
        }
    }
}
