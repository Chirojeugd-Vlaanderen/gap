using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CgDal;
using System.Data.Objects.DataClasses;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            ChiroGroepServiceReference.CgServiceClient service = new ChiroGroepServiceReference.CgServiceClient();

            // Gegevens persoon p zullen gewijzigd worden, persoon q is kloon.
            Persoon p = service.PersoonGet(1894);
            Persoon q = p.CloneSerializing();

            // Haal tegelijk kopie van persoon p op, zodat we een exceptie
            // kunnen veroorzaken.

            Persoon a = service.PersoonGet(1894);
            Persoon b = a.CloneSerializing();


            String nieuweVoornaam;

            Console.WriteLine(service.Hello());
            Console.WriteLine("Persoon opgehaald: " + p.VoorNaam + ' ' + p.Naam);

            Console.WriteLine("Geef nieuwe voornaam in:");
            nieuweVoornaam = Console.ReadLine();

            p.VoorNaam = nieuweVoornaam;

            service.PersoonUpdaten(p, q);

            // Veroorzaak exceptie door gewijzigd persoon te overschrijven.

            a.VoorNaam = "Marsipulami.";
            service.PersoonUpdaten(a, b);

        }
    }
}
