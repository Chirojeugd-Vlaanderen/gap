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
using System.Linq;
using Chiro.Cdf.Poco;
using Chiro.Cdf.ServiceHelper;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.Gap.FixAnomalies.Properties;
using Chiro.Gap.Poco.Context;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Sync;
using Chiro.Gap.SyncInterfaces;

namespace Chiro.Gap.FixAnomalies
{
    partial class Program
    {
        private static void LidRelatiesFixen(ServiceHelper serviceHelper, string apiKey, string siteKey)
        {
            Console.WriteLine(Resources.Program_Main_Opvragen_actieve_lidrelaties_CiviCRM_);
            
            // Vermijd dat de CiviCRM-API over zijn memory limit gaat, door de leden op te
            // halen in blokken.

            var civiLeden = new List<string>();
            int offset = 0;
            bool finished = false;

            while (!finished)
            {
                var request = new BaseRequest
                {
                    ApiOptions = new ApiOptions {Limit = Properties.Settings.Default.LedenBlokGrootte, Offset = offset}
                };
                var civiResult =
                    serviceHelper.CallService<ICiviCrmApi, ApiResultStrings>(
                        svc => svc.ChiroDiagnosticsActieveLidRelaties(apiKey, siteKey, request));
                if (civiResult.IsError != 0)
                {
                    throw new ApplicationException(civiResult.ErrorMessage);
                }
                civiLeden.AddRange(from v in civiResult.Values select v[0]);
                offset += civiResult.Count;
                finished = civiResult.Count < Properties.Settings.Default.LedenBlokGrootte;
            }
            Console.WriteLine(Resources.Program_Main_Dat_zijn_er__0__, civiLeden.Count);

            int werkjaar = HuidigWerkJaar();

            Console.WriteLine(Resources.Program_Main_Opvragen_actieve_leden_GAP__);
            var gapLeden = AlleActieveLeden();
            Console.WriteLine(Resources.Program_Main_Dat_zijn_er__0__, gapLeden.Count());

            var teBewarenLeden = OntbrekendInCiviZoeken(civiLeden, gapLeden);
            Console.WriteLine(Resources.Program_Main__0__leden_uit_GAP_niet_teruggevonden_in_CiviCRM_, teBewarenLeden.Count);

            // TODO: command line switch om deze vraag te vermijden.
            Console.Write(Resources.Program_Main_Meteen_syncen__);
            string input = Console.ReadLine();

            if (input.ToUpper() == "J" || input.ToUpper() == "Y")
            {
                LedenNaarCivi(teBewarenLeden, serviceHelper);
            }

            var uitTeSchrijvenLeden = TeVeelInCiviZoeken(civiLeden, gapLeden);
            Console.WriteLine(Resources.Program_Main__0__leden_uit_CiviCRM_niet_teruggevonden_in_GAP_, uitTeSchrijvenLeden.Count);

            // TODO: command line switch om deze vraag te vermijden.
            Console.Write(Resources.Program_Main_Uitschrijven_uit_Civi__);
            string input2 = Console.ReadLine();

            if (input2.ToUpper() == "J" || input2.ToUpper() == "Y")
            {
                LedenUitschrijvenCivi(uitTeSchrijvenLeden, serviceHelper);
            }
        }

        private static int HuidigWerkJaar()
        {
            DateTime vandaag = DateTime.Now;
            int werkjaar = vandaag.Month >= 9 ? vandaag.Year : vandaag.Year - 1;
            return werkjaar;
        }

        private static void LedenNaarCivi(List<LidInfo> teSyncen, ServiceHelper serviceHelper)
        {
            int counter = 0;
            var sync = new LedenSync(serviceHelper);
            using (var context = new ChiroGroepEntities())
            {
                var repositoryProvider = new RepositoryProvider(context);
                var ledenRepo = repositoryProvider.RepositoryGet<Lid>();
                foreach (var l in teSyncen)
                {
                    sync.Bewaren(ledenRepo.ByID(l.LidId));
                    Console.Write("{0} ", ++counter);
                }
            }
        }

        private static void LedenUitschrijvenCivi(List<UitschrijfInfo> teSyncen, ServiceHelper serviceHelper)
        {
            int counter = 0;
            var sync = new LedenSync(serviceHelper);
            foreach (var l in teSyncen)
            {
                sync.Uitschrijven(l);
                Console.Write("{0} ", ++counter);
            }
        }

        private static List<LidInfo> OntbrekendInCiviZoeken(IList<string> civiLeden, IList<LidInfo> gapLeden)
        {
            int civiCounter = 0;
            int gapCounter = 0;
            var teSyncen = new List<LidInfo>();
            int aantalciviLeden = civiLeden.Count;

            // Normaal zijn de leden uit het GAP hetzelfde gesorteerd als die uit Civi.
            // Overloop de GAP-leden, en kijk of ze ook in de Civi-leden voorkomen.

            Console.WriteLine(Resources.Program_OntbrekendInCiviZoeken_Opzoeken_leden_in_GAP_maar_niet_in_CiviCRM_);
            while (gapCounter < gapLeden.Count && civiCounter < aantalciviLeden)
            {
                while (civiCounter < aantalciviLeden && String.Compare(gapLeden[gapCounter].StamNrAdNr, civiLeden[civiCounter], StringComparison.OrdinalIgnoreCase) > 0)
                {
                    ++civiCounter;
                }
                if (civiCounter < aantalciviLeden && String.Compare(gapLeden[gapCounter].StamNrAdNr, civiLeden[civiCounter], StringComparison.OrdinalIgnoreCase) != 0)
                {
                    teSyncen.Add(gapLeden[gapCounter]);
                    Console.WriteLine(gapLeden[gapCounter].StamNrAdNr);
                }
                ++gapCounter;
            }
            return teSyncen;
        }

        private static List<UitschrijfInfo> TeVeelInCiviZoeken(IList<string> civiLeden, IList<LidInfo> gapLeden)
        {
            int civiCounter = 0;
            int gapCounter = 0;
            var teSyncen = new List<UitschrijfInfo>();

            // Normaal zijn de leden uit het GAP hetzelfde gesorteerd als die uit Civi.
            // Overloop de GAP-leden, en kijk of ze ook in de Civi-leden voorkomen.

            Console.WriteLine(Resources.Program_TeVeelInCiviZoeken_Opzoeken_leden_in_CiviCRM_maar_niet_in_GAP_);
            while (gapCounter < gapLeden.Count && civiCounter < civiLeden.Count)
            {
                while (gapCounter < gapLeden.Count && String.Compare(gapLeden[gapCounter].StamNrAdNr, civiLeden[civiCounter], StringComparison.OrdinalIgnoreCase) < 0)
                {
                    ++gapCounter;
                }
                if (gapCounter < gapLeden.Count && String.Compare(gapLeden[gapCounter].StamNrAdNr, civiLeden[civiCounter], StringComparison.OrdinalIgnoreCase) != 0)
                {
                    // Splits output van Civi in stamnummer en AD-nummer.
                    string[] components = civiLeden[civiCounter].Split(';');

                    // Nationale ploegen zitten nog niet in GAP (#4055). We negeren hen op basis van de lengte van
                    // hun stamnummer. (Oh dear.)
                    if (components[0].Length == 8)
                    {
                        // Construeer een fake lid, om dat zodatdelijk naar Civi te syncen.
                        // Ik gebruik niet het echte lid, want het is niet gezegd dat dat bestaat. En leden uit Civi
                        // verwijderen die misschien al DP februari hebben gekregen, lijkt me niet zo'n goed idee.

                        var l = new UitschrijfInfo
                        {
                            AdNummer = int.Parse(components[1]),
                            StamNummer = components[0],
                            WerkJaar = HuidigWerkJaar(),
                            UitschrijfDatum = DateTime.Now
                        };
                        teSyncen.Add(l);
                        Console.WriteLine(civiLeden[civiCounter]);
                    }
                }
                ++civiCounter;
            }
            return teSyncen;
        }

        /// <summary>
        /// Levert een lijst op van alle actieve stamnummer-adnummer-combinaties van de
        /// actieve leden. Zal gebruikt worden voor monitoring. (#4326, #4268)
        /// </summary>
        /// <returns>Lijst van alle stamnummer-adnummer-combinaties van de actieve
        /// leden.</returns>
        public static LidInfo[] AlleActieveLeden()
        {
            // Dit zou beter gebeuren met dependency injection. Maar het is en blijft een hack.
            using (var context = new ChiroGroepEntities())
            {
                var repositoryProvider = new RepositoryProvider(context);
                var repo = repositoryProvider.RepositoryGet<ActiefLid>();

                var result = (from l in repo.GetAll()
                    select new LidInfo
                    {
                        StamNrAdNr = String.Format("{0};{1}", l.Code.Trim(), l.AdNummer),
                        LidId = l.LidID
                    });
                return result.OrderBy(r => r.StamNrAdNr).ToArray();
            }
        }
    }
}
