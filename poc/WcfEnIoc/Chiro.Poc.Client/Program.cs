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
using Chiro.Poc.Ioc;
using Chiro.Poc.ServiceGedoe;
using Chiro.Poc.WcfService.ServiceContracts;

namespace Chiro.Poc.Client
{
    /// <summary>
    /// Voorbeeldprogramma dat ServiceHelper gebruikt om de webservice met cintract IService1 aan te roepen.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Initialiseer de IOC-container.  In App.Config staat gedefinieerd dat ServiceHelper de OnlineServiceClient
            // moet gebruiken om de service calls uit te voeren.

            Factory.ContainerInit();

            Console.WriteLine(ServiceHelper.CallService<IService1, string>(svc => svc.Hallo()));
            Console.ReadLine();
        }
    }
}
