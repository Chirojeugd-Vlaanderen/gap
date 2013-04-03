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
using CgDal;
using System.Data.Objects.DataClasses;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            ChiroGroepServiceReference.CgServiceClient service = new ChiroGroepServiceReference.CgServiceClient();

            // Gegevens persoon p zullen gewijzigd worden, persoon q is kloon.
            Persoon p = service.PersoonGet(1894);
            Persoon q = p.CloneSerializing();

            // Haal tegelijk kopie van persoon p op, zodat we een exceptie
            // kunnen veroorzaken.

            Persoon a = service.PersoonGet(1894);
            Persoon b = a.CloneSerializing();


            String nieuweVoornaam;

            Console.WriteLine(service.Hello());
            Console.WriteLine("Persoon opgehaald: " + p.VoorNaam + ' ' + p.Naam);

            Console.WriteLine("Geef nieuwe voornaam in:");
            nieuweVoornaam = Console.ReadLine();

            p.VoorNaam = nieuweVoornaam;

            service.PersoonUpdaten(p, q);

            // Veroorzaak exceptie door gewijzigd persoon te overschrijven.

            a.VoorNaam = "Marsipulami.";
            service.PersoonUpdaten(a, b);

        }
    }
}
