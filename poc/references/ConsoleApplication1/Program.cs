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

            var lijst = service.PersoonsAdressenGet(1893);
            var eerste = lijst.First();

            Console.WriteLine("PersoonID: " + eerste.PersoonID);
            Console.WriteLine("AdresID:" + eerste.AdresID);

            Console.ReadLine();

        }
    }
}
