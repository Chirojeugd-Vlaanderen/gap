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
            ChiroGroepServiceReference.CgServiceClient service = new ChiroGroepServiceReference.CgServiceClient();

            var p = service.PersoonGet(1893);

            Console.WriteLine(service.Hello());
            Console.WriteLine(p.VoorNaam + ' ' + p.Naam);

            Console.ReadLine();
        }
    }
}
