using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Core.Domain;

namespace ZtommeConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceReference1.GroepenServiceClient service = new ZtommeConsoleApp.ServiceReference1.GroepenServiceClient())
            {
                Groep g = service.Ophalen(310);

                Console.WriteLine(g.Naam);
                Console.ReadLine();
            }
        }
    }
}
