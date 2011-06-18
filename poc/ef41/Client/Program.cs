using System;

namespace Client
{
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

                Console.Write("Deelnemers toegevoegd.  Ophalen...\n");

                foreach (var dnl in client.DeelnemersOphalen(cursusID))
                {
                    Console.WriteLine(dnl);
                }
                Console.ReadLine();
            }
        }
    }
}
