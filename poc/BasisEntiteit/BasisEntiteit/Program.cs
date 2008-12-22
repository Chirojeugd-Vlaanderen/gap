using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;
using Core.Domain;

namespace BasisEntiteit
{
    class Program
    {
        static void Main(string[] args)
        {
            ChiroGroepEntities context = new ChiroGroepEntities();

            Groep g = (
                from Groep grp in context.Groep
                where grp.ID == 310
                select grp
                ).First();

            Console.WriteLine(g.Naam);

            g.Naam = "Valentijn";
            context.SaveChanges();

            Console.ReadLine();
        }
    }
}
