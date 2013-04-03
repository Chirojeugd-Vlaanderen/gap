/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
﻿using System;
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
        private static ISessionFactory _sessionFactory = null;

        /// <summary>
        /// De SessionFactory is statisch, en moet 1 keer aangemaakt worden.
        /// De SessionFactory zal o.a. zorgen voor caching.
        /// </summary>
        public static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    Configuration cfg = new Configuration();
                    
                    // Niet zeker of de twee volgende lijnen nodig zijn.
                    cfg.Configure();

                    // Onderstaande voegt alle xml-files die geembed zijn
                    // in de assembly toe aan de configuratie.  Je zou
                    // de configuratiebestanden ook kunnen toevoegen
                    // via cfg.AddXmlFile()

                    // cfg.AddAssembly(Assembly.GetCallingAssembly());

                    cfg.AddXmlFile("Persoon.hbm.xml");
                    cfg.AddXmlFile("CommunicatieVorm.hbm.xml");

                    _sessionFactory = cfg.BuildSessionFactory();
                }
                return _sessionFactory;
            }
        }

        /// <summary>
        /// Maakt een nieuwe 'NHibernatesessie' aan, nu via de
        /// Session Factory (eigenlijk is er dus geen aparte
        /// method meer voor nodig).
        /// </summary>
        /// <returns>NHibernatesessie</returns>
        static ISession NieuweSessie()
        {
            return SessionFactory.OpenSession();
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
                Console.WriteLine("Persoon aangemaakt en bewaard.");
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

                    Console.WriteLine("Persoon hernoemd");
                }
            }
        }
        
        static void Main(string[] args)
        {
            // log4net moet expliciet geconfigureerd worden,
            // zie http://www.hibernate.org/364.html

            log4net.Config.XmlConfigurator.Configure();

            //MaakEnBewaarPersoon();
            //WijzigPersoon();
            //Console.ReadLine();

            HalloPersonen();

            Console.ReadLine();
        }
    }
}
