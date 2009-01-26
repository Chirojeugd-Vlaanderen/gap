using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using System.Reflection;

namespace HelloNhibernate
{
    class Program
    {
        /// <summary>
        /// Maakt een nieuwe 'NHibernatesessie' aan.  Deze manier van werken
        /// is niet geschikt voor een productieomgeving!
        /// </summary>
        /// <returns>NHibernatesessie</returns>
        static ISession NieuweSessie()
        {
            Configuration c = new Configuration();
            c.AddAssembly(Assembly.GetCallingAssembly());
            ISessionFactory f = c.BuildSessionFactory();
            return f.OpenSession();
        }


        /// <summary>
        /// Maakt een nieuwe persoon aan, en bewaart die in de database
        /// </summary>
        static void MaakEnBewaarPersoon()
        {
            Persoon p = new Persoon { Naam = "Kiekeboe", VoorNaam = "Marcel", Geslacht = GeslachtsType.Man };

            using (ISession session = NieuweSessie())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(p);
                    transaction.Commit();
                }
                Console.WriteLine("Persoon 'Johan' aangemaakt en bewaard.");
            }
        }

        /// <summary>
        /// Roept de 'Hallomethod' op van alle personen
        /// </summary>
        static void HalloPersonen()
        {
            using (ISession sessie = NieuweSessie())
            {
                IQuery query = sessie.CreateQuery(
                    "from Persoon as p order by p.Naam asc");

                IList<Persoon> gevonden = query.List<Persoon>();

                Console.WriteLine("\n{0} personen gevonden:", gevonden.Count);

                foreach (Persoon p in gevonden)
                {
                    Console.WriteLine(p.Hallo());
                }
            }
        }

        /// <summary>
        /// Wijzigt een persoon, en bewaart in de database.
        /// </summary>
        static void WijzigPersoon()
        {
            using (ISession sessie = NieuweSessie())
            {
                using (ITransaction transactie = sessie.BeginTransaction())
                {
                    IQuery q = sessie.CreateQuery(
                        "from Persoon where naam = 'Kiekeboe'");

                    Persoon p = q.List<Persoon>()[0];
                    p.VoorNaam = "Konstantinopel";

                    sessie.Flush();
                    transactie.Commit();

                    Console.WriteLine("Persoon hernoemd als 'Johan'");
                }
            }
        }
        
        static void Main(string[] args)
        {
            MaakEnBewaarPersoon();
            WijzigPersoon();
            HalloPersonen();

            Console.ReadLine();
        }
    }
}
