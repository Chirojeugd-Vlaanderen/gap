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

using Chiro.Cdf.Poco;
using Chiro.Cdf.ServiceHelper;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.Gap.Poco.Context;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Sync;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.Workers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chiro.Gap.FixAnomalies
{
    class Program
    {
        static void Main(string[] args)
        {
            // DIT IS LELIJK, EN KAN BEST WAT OPKUIS GEBRUIKEN.

            string apiKey = Properties.Settings.Default.ApiKey;
            string siteKey = Properties.Settings.Default.SiteKey;

            // Dependency injection gebeurt hier overal manueel
            // TODO: Chiro.Gap.Ioc gebruiken.

            var ledenManager = new LedenManager();
            var groepsWerkJarenManager = new GroepsWerkJarenManager(new VeelGebruikt());
            var abonnementenManager = new AbonnementenManager();

            var helper = new Chiro.Gap.ServiceContracts.Mappers.MappingHelper(ledenManager, groepsWerkJarenManager, abonnementenManager);
            helper.MappingsDefinieren();

            var serviceHelper = new ServiceHelper(new ChannelFactoryChannelProvider());

            // TODO: via command line opties verbositeit van dit script bepalen.
            // TODO: resources gebruiken voor outputtext.

            Console.WriteLine("Opvragen actieve lidrelaties CiviCRM.");

            var civiResult =
                serviceHelper.CallService<ICiviCrmApi, ApiResultStrings>(
                    svc => svc.ChiroDiagnosticsActieveLidRelaties(apiKey, siteKey));
            if (civiResult.IsError != 0)
            {
                throw new ApplicationException(civiResult.ErrorMessage);
            }
            Console.WriteLine("Dat zijn er {0}.", civiResult.Count);

            int werkjaar = HuidigWerkJaar();

            Console.WriteLine("Opvragen leden met AD-nummer in Gap, werkjaar {0}.", werkjaar);
            var gapLeden = AlleLeden(werkjaar);
            Console.WriteLine("Dat zijn er {0}.", gapLeden.Length);

            var teBewarenLeden = OntbrekendInCiviZoeken(civiResult, gapLeden);
            Console.WriteLine("{0} leden uit GAP niet teruggevonden in CiviCRM.", teBewarenLeden.Count);

            // TODO: command line switch om deze vraag te vermijden.
            Console.Write("Meteen syncen? ");
            string input = Console.ReadLine();

            if (input.ToUpper() == "J" || input.ToUpper() == "Y")
            {
                LedenNaarCivi(teBewarenLeden, serviceHelper);
            }

            var uitTeSchrijvenLeden = TeVeelInCiviZoeken(civiResult, gapLeden);
            Console.WriteLine("{0} leden uit CiviCRM niet teruggevonden in GAP.", uitTeSchrijvenLeden.Count);

            // TODO: command line switch om deze vraag te vermijden.
            Console.Write("Uitschrijven uit Civi? ");
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

        private static List<LidInfo> OntbrekendInCiviZoeken(ApiResultStrings civiResult, LidInfo[] gapLeden)
        {
            int civiCounter = 0;
            int gapCounter = 0;
            var teSyncen = new List<LidInfo>();

            // Normaal zijn de leden uit het GAP hetzelfde gesorteerd als die uit Civi.
            // Overloop de GAP-leden, en kijk of ze ook in de Civi-leden voorkomen.

            Console.WriteLine("Opzoeken leden in GAP maar niet in CiviCRM.");
            while (gapCounter < gapLeden.Length && civiCounter < civiResult.Count)
            {
                while (civiCounter < civiResult.Count && String.Compare(gapLeden[gapCounter].StamNrAdNr, civiResult.Values[civiCounter].First(), true) > 0)
                {
                    ++civiCounter;
                }
                if (civiCounter < civiResult.Count && String.Compare(gapLeden[gapCounter].StamNrAdNr, civiResult.Values[civiCounter].First(), true) != 0)
                {
                    teSyncen.Add(gapLeden[gapCounter]);
                    Console.WriteLine(gapLeden[gapCounter].StamNrAdNr);
                }
                ++gapCounter;
            }
            return teSyncen;
        }

        private static List<UitschrijfInfo> TeVeelInCiviZoeken(ApiResultStrings civiResult, LidInfo[] gapLeden)
        {
            int civiCounter = 0;
            int gapCounter = 0;
            var teSyncen = new List<UitschrijfInfo>();

            // Normaal zijn de leden uit het GAP hetzelfde gesorteerd als die uit Civi.
            // Overloop de GAP-leden, en kijk of ze ook in de Civi-leden voorkomen.

            Console.WriteLine("Opzoeken leden in CiviCRM maar niet in GAP.");
            while (gapCounter < gapLeden.Length && civiCounter < civiResult.Count)
            {
                while (gapCounter < gapLeden.Length && String.Compare(gapLeden[gapCounter].StamNrAdNr, civiResult.Values[civiCounter].First(), true) < 0)
                {
                    ++gapCounter;
                }
                if (gapCounter < gapLeden.Length && String.Compare(gapLeden[gapCounter].StamNrAdNr, civiResult.Values[civiCounter].First(), true) != 0)
                {
                    // Splits output van Civi in stamnummer en AD-nummer.
                    string[] components = civiResult.Values[civiCounter].First().Split(';');

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
                        Console.WriteLine(civiResult.Values[civiCounter].First());
                    }
                }
                ++civiCounter;
            }
            return teSyncen;
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
                            // Enkel personen waarvan AD-nummers al gekend zijn. In het andere geval
                            // zal gapmaintenance wel syncen.
                            && ld.GelieerdePersoon.Persoon.AdNummer.HasValue
                            // Enkel leden van actieve groepen
                            && ld.GroepsWerkJaar.Groep.StopDatum == null
                            select new LidInfo { StamNrAdNr = ld.GroepsWerkJaar.Groep.Code.Trim() + ";" + ld.GelieerdePersoon.Persoon.AdNummer, LidId = ld.ID };
                return alles.OrderBy(info => info.StamNrAdNr).ToArray();
            }
        }
    }
}
