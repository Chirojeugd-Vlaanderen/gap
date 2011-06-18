using System;

namespace Client
{
    // Dit is het project dat je moet starten!

    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new ServiceReference1.CursusServiceClient())
            {

                // Om de service aan te roepen, heb ik gewoon een servicereference gegenereerd.
                // Dit moet nog iets beters worden.  ServiceHelper zou een mogelijkheid zijn,
                // maar dan is IOC moeilijk.

                int cursusID = client.CursusMaken("Iekaa bla bla", DateTime.Today, DateTime.Today);
                Console.WriteLine("Cursus aangemaakt met id {0}", cursusID);

                client.DeelnemerInschrijven(cursusID, "Jos");
                client.DeelnemerInschrijven(cursusID, "Charel");
                client.DeelnemerInschrijven(cursusID, "Rita");

                Console.WriteLine("Deelnemers toegevoegd.  Ophalen...");

                foreach (var dnl in client.DeelnemersOphalen(cursusID))
                {
                    Console.WriteLine(dnl);
                }

                client.DeelnemerVerwijderen(cursusID, "Charel");
                Console.WriteLine("Charel verwijderd.  Opnieuw ophalen");

                foreach (var dnl in client.DeelnemersOphalen(cursusID))
                {
                    Console.WriteLine(dnl);
                }

                Console.ReadLine();
            }
        }
    }
}
