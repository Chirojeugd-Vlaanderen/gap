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


