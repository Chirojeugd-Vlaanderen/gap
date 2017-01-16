/*
   Copyright 2015,2016 Chirojeugd-Vlaanderen vzw

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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.FixAnomalies.Properties;
using Chiro.Gap.Workers;

namespace Chiro.Gap.FixAnomalies
{
    partial class Program
    {
        static void Main(string[] args)
        {
            var tefixen = TeFixenGet(args);
            if (tefixen == TeFixen.Geen)
            {
                Console.WriteLine(Resources.Usage, Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location));
                Console.WriteLine(Resources.Program_Main_Druk_ENTER_);
                Console.ReadLine();
                return;
            }

            // DIT IS LELIJK, EN KAN BEST WAT OPKUIS GEBRUIKEN.

            string apiKey = Properties.Settings.Default.ApiKey;
            string siteKey = Properties.Settings.Default.SiteKey;

            // Dependency injection gebeurt hier overal manueel
            // TODO: Chiro.Gap.Ioc gebruiken.

            var serviceHelper = new ServiceHelper(new ChannelFactoryChannelProvider());
            var cacherij = new VeelGebruikt(serviceHelper);
            var ledenManager = new LedenManager();
            var groepsWerkJarenManager = new GroepsWerkJarenManager(cacherij);
            var abonnementenManager = new AbonnementenManager();

            // Ik geef null als authenticator, want als deze hacks proberen op te zoeken
            // wie ik ben, dan is er iets mis. De bedoeling is dat ze dingen in de database
            // onafhankelijk van wie ze runt. (Maar diegene die ze runt moet uiteraard wel
            // voldoende rechten hebben op de database.)
            var authenticatieManager = new AuthenticatieManager(cacherij, null);
            var autorisatieManager = new AutorisatieManager(authenticatieManager);

            var helper = new Chiro.Gap.ServiceContracts.Mappers.MappingHelper(ledenManager, groepsWerkJarenManager, abonnementenManager, authenticatieManager, autorisatieManager);

            // Mappings naar civi
            helper.MappingsDefinieren();
            // Mappings naar 'kipsync' (nu civisync)
            Chiro.Gap.Sync.MappingHelper.MappingsDefinieren();

            // TODO: via command line opties verbositeit van dit script bepalen.

            if ((tefixen & TeFixen.Bivakken) != 0) BivakAangiftesFixen(serviceHelper, apiKey, siteKey);
            if ((tefixen & TeFixen.Leden) != 0) LidRelatiesFixen(serviceHelper, apiKey, siteKey);
            if ((tefixen & TeFixen.Dubbelpunt) != 0) DubbelpuntFixen(serviceHelper, apiKey, siteKey);

            // Onderstaande is nog niet af, maar kunnen we afwerken wanneer nodig.
            // Zie #4586.
            //LoonVerliesFixen(serviceHelper, apiKey, siteKey);
        }

        /// <summary>
        /// Levert alles uit lijst <paramref name="a"/> op dat niet voorkomt in lijst <paramref name="b"/>.
        /// Alle items van het resultaat worden afgedrukt op de console.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a">gesorteerde lijst</param>
        /// <param name="b">gesorteerde lijst</param>
        /// <returns>Alles uit <paramref name="a"/> dat niet voorkomt in <paramref name="b"/>.</returns>
        /// <remarks>Alle lijsten worden geacht gesorteerd en distinct te zijn.</remarks>
        static List<T> AEnNietB<T>(IList<T> a, IList<T> b) where T:IComparable
        {
            int aCounter = 0;
            int bCounter = 0;
            var result = new List<T>();

            int nA = a.Count();
            int nB = b.Count();

            while (aCounter < nA && bCounter < nB)
            {
                while (bCounter < nB && b[bCounter].CompareTo(a[aCounter]) < 0)
                {
                    ++bCounter;
                }
                if (bCounter < nB && b[bCounter].CompareTo(a[aCounter]) != 0)
                {
                    result.Add(a[aCounter]);
                    Console.WriteLine(a[aCounter]);
                }
                ++aCounter;
            }
            return result;
        }

        public static TeFixen TeFixenGet(string[] args)
        {
            TeFixen result = TeFixen.Geen;
            foreach (string s in args)
            {
                switch (s.ToLower())
                {
                    case "-b":
                        result |= TeFixen.Bivakken;
                        break;
                    case "-l":
                        result |= TeFixen.Leden;
                        break;
                    case "-d":
                        result |= TeFixen.Dubbelpunt;
                        break;
                    case "-a":
                        result |= TeFixen.Alles;
                        break;
                }
            }
            return result;
        }
    }
}
