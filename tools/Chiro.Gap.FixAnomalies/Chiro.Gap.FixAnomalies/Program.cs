/*
   Copyright 2015 Chirojeugd-Vlaanderen vzw

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

using Chiro.Cdf.Poco;
using Chiro.Cdf.ServiceHelper;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.Gap.Poco.Context;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiro.Gap.FixAnomalies
{
    class Program
    {
        static void Main(string[] args)
        {
            string apiKey = Properties.Settings.Default.ApiKey;
            string siteKey = Properties.Settings.Default.SiteKey;

            var teSyncen = new List<LidInfo>();

            // In het GAP (versie 2.2 alleszins) gebruiken we dependency injection
            // om de ChannelProvider te injecteren. Voor dit voorbeeld doen we het
            // manueel.
            var serviceHelper = new ServiceHelper(new ChannelFactoryChannelProvider());

            var civiResult =
                serviceHelper.CallService<ICiviCrmApi, ApiResultStrings>(
                    svc => svc.ChiroDiagnosticsActieveLidRelaties(apiKey, siteKey));
            if (civiResult.IsError != 0)
            {
                throw new ApplicationException(civiResult.ErrorMessage);
            }

            DateTime vandaag = DateTime.Now;
            int werkjaar = vandaag.Month >= 9 ? vandaag.Year : vandaag.Year - 1;

            var gapLeden = AlleLeden(werkjaar);

            int civiCounter = 0;
            int gapCounter = 0;

            // Normaal zijn de leden uit het GAP hetzelfde gesorteerd als die uit Civi.
            // Overloop de GAP-leden, en kijk of ze ook in de Civi-leden voorkomen.

            while (gapCounter < gapLeden.Length && civiCounter < civiResult.Count)
            {
                while (civiCounter < civiResult.Count && String.Compare(gapLeden[gapCounter].StamNrAdNr, civiResult.Values[civiCounter].First(), true) > 0)
                {
                    ++civiCounter;
                }
                if (civiCounter < civiResult.Count && gapLeden[gapCounter].StamNrAdNr != civiResult.Values[civiCounter].First())
                {
                    teSyncen.Add(gapLeden[gapCounter]);
                    Console.WriteLine(gapLeden[gapCounter].StamNrAdNr);
                }
                ++gapCounter;
            }
            
            Console.WriteLine("Het zijn er {0}.", teSyncen.Count);
            Console.Write("Meteen syncen?");
            string input = Console.ReadLine();
            if (input.ToUpper() == "J" || input.ToUpper() == "Y")
            {
                var sync = new LedenSync(serviceHelper);
                using (var context = new ChiroGroepEntities())
                {
                    var repositoryProvider = new RepositoryProvider(context);
                    var ledenRepo = repositoryProvider.RepositoryGet<Lid>();
                    foreach (var l in teSyncen)
                    {
                        sync.Bewaren(ledenRepo.ByID(l.LidId));
                    }
                }
            }
        }

        /// <summary>
        /// Levert een lijst op van alle stamnummer-adnummer-combinaties van het huidige
        /// werkjaar. Zal gebruikt worden voor monitoring. (#4326, #4268)
        /// </summary>
        /// <returns>Lijst van alle stamnummer-adnummer-combinaties van het huidige
        /// werkjaar.</returns>
        /// <remarks>
        /// Deze functie hoort niet echt thuis in iets dat 'GapUpdater' heet. Misschien
        /// is dit eerder een GapWorker. Of misschien moet deze klasse opgesplitst worden.
        /// </remarks>
        public static LidInfo[] AlleLeden(int werkjaar)
        {
            // Dit zou beter gebeuren met dependency injection. Maar het is en blijft een hack.
            using (var context = new ChiroGroepEntities())
            {
                var repositoryProvider = new RepositoryProvider(context);
                var ledenRepo = repositoryProvider.RepositoryGet<Lid>();

                var alles = from ld in ledenRepo.Select()
                            where ld.GroepsWerkJaar.WerkJaar == werkjaar && !ld.NonActief
                            select new LidInfo { StamNrAdNr = ld.GroepsWerkJaar.Groep.Code.Trim() + ";" + ld.GelieerdePersoon.Persoon.AdNummer, LidId = ld.ID };
                return alles.OrderBy(info => info.StamNrAdNr).ToArray();
            }
        }
    }
}
