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

                int cursusId = client.CursusMaken("Iekaa bla bla", DateTime.Today, DateTime.Today);
                Console.WriteLine("Cursus aangemaakt met id {0}", cursusId);

                client.DeelnemerInschrijven(cursusId, "Jos");
                client.DeelnemerInschrijven(cursusId, "Charel");
                client.DeelnemerInschrijven(cursusId, "Rita");

                Console.WriteLine("Deelnemers toegevoegd.  Ophalen...");

                foreach (var dnl in client.DeelnemersOphalen(cursusId))
                {
                    Console.WriteLine(dnl);
                }

                client.DeelnemerVerwijderen(cursusId, "Charel");
                Console.WriteLine("Charel verwijderd.  Opnieuw ophalen");

                foreach (var dnl in client.DeelnemersOphalen(cursusId))
                {
                    Console.WriteLine(dnl);
                }

                int cursusID2 = client.CursusMaken("Esbee hoezee", DateTime.Today, DateTime.Today);


                client.DeelnemerVerhuizen(cursusId, cursusID2, "Jos");
                Console.WriteLine("Jos verhuisd naar volgende cursus.  Deelnemerslijst 1:");
                foreach (var dnl in client.DeelnemersOphalen(cursusId))
                {
                    Console.WriteLine(dnl);
                }

                Console.WriteLine("Deelnemerslijst 2:");
                foreach (var dnl in client.DeelnemersOphalen(cursusID2))
                {
                    Console.WriteLine(dnl);
                }



                Console.ReadLine();
            }
        }
    }
}
