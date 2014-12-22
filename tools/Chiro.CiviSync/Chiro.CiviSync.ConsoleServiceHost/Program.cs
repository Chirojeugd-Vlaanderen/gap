/*
   Copyright 2013 Chirojeugd-Vlaanderen vzw

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

using System;
using Chiro.Cdf.Ioc;
using Chiro.Cdf.ServiceModel;
using Chiro.CiviSync.Services;

namespace Chiro.CiviSync.ConsoleServiceHost
{
    /// <summary>
    /// Executable die luistert naar messages in de civisyncdev message queue,
    /// en ze dan verwerkt.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Initialiseer dependency injection en automapper
            MappingHelper.MappingsDefinieren();
            Factory.ContainerInit();

            // Creeer en open service host.
            
            var host = new ServiceHost<SyncService>();
            host.Open();

            // Sluit host na enter.
            Console.WriteLine("Hit enter to stop the service.");
            Console.ReadLine();
            host.Close();
        }
    }
}
