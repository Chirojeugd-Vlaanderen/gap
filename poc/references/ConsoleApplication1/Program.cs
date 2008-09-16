using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            ChiroGroepServiceReference.CgServiceClient service = new ChiroGroepServiceReference.CgServiceClient();
            int keuze;

            var lijst = service.PersoonsAdressenGet(1893);
            var eerste = lijst.First();
            var origineel = eerste.CloneSerializing();

            Console.WriteLine("PersoonID: " + eerste.PersoonID);
            Console.WriteLine("AdresID: " + eerste.AdresID);
            Console.WriteLine("AdresType: " + eerste.AdresType.Omschrijving);

            Console.WriteLine("\nWijzig in: (1) thuis, (2) kot, (3) werk, (4) onbekend");
            keuze = int.Parse(Console.ReadLine());

            switch (keuze)
            {
                case 1: eerste.AdresType = service.ThuisAdresType(); break;
                case 2: eerste.AdresType = service.KotAdresType(); break;
                case 3: eerste.AdresType = service.WerkAdresType(); break;
                default: eerste.AdresType = service.OnbekendAdresType(); break;
            };

            EntityReference<CgDal.AdresType> adresTypeReference = new EntityReference<CgDal.AdresType>();
            adresTypeReference.EntityKey = eerste.AdresType.EntityKey;
            eerste.AdresTypeReference = adresTypeReference;


            Console.WriteLine("\nNieuw AdresTypeId: " + eerste.AdresType.AdresTypeID);


            service.PersoonsAdresUpdaten(eerste, origineel);

            Console.ReadLine();
        }
  
    }

}


