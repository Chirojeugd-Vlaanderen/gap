using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsAppZonderPragmaticSoa
{
    class Program
    {
        static void Main(string[] args)
        {
            using (PersonenServiceReference.PersonenServiceClient service = new ConsAppZonderPragmaticSoa.PersonenServiceReference.PersonenServiceClient())
            {
                PersonenServiceReference.Persoon p = service.PersoonGet(1894);

                p.VoorNaam = "Willy";

                service.PersoonUpdaten(p);
            }
        }
    }
}
