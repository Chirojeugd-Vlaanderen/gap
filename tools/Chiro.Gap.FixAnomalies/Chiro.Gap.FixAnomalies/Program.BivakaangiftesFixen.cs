/*
   Copyright 2016 Chirojeugd-Vlaanderen vzw

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
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chiro.Cdf.Poco;
using Chiro.Cdf.ServiceHelper;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Filters;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.Gap.FixAnomalies.Properties;
using Chiro.Gap.Poco.Context;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Sync;

namespace Chiro.Gap.FixAnomalies
{
    public partial class Program
    {
        private static void BivakAangiftesFixen(ServiceHelper serviceHelper, string apiKey, string siteKey)
        {
            Console.WriteLine(Resources.Program_BivakAangiftesFixen_Ophalen_volledige_bivakaangiftes_uit_CiviCRM_);

            int jaar = DateTime.Now.Year;
            var periodeStart = new DateTime(jaar, Properties.Settings.Default.BivakPeriodeStartMaand, Properties.Settings.Default.BivakPeriodeStartDag);
            var periodeEinde = new DateTime(jaar, Properties.Settings.Default.BivakPeriodeEindeMaand, Properties.Settings.Default.BivakPeriodeEindeDag);

            var request = new EventRequest
            {
                EndDate = new Filter<DateTime?>(WhereOperator.Gte, periodeStart),
                StartDate = new Filter<DateTime?>(WhereOperator.Lte, periodeEinde),
                // BIVAK
                EventTypeId = 100,
                OrganiserendePersoon1Id = new Filter<int?>(WhereOperator.IsNotNull),
                LocBlockIdFilter = new Filter<int>(WhereOperator.IsNotNull),
                // Organiserende ploeg 1 en GAP-uitstapID
                ReturnFields = "custom_48,custom_53",
                ApiOptions = new ApiOptions { Limit = 0 }
            };

            var civiResult =
                serviceHelper.CallService<ICiviCrmApi, ApiResultValues<Event>>(
                    svc => svc.EventGet(apiKey, siteKey, request));
            Console.WriteLine(Resources.Program_Main_Dat_zijn_er__0__, civiResult.Count);

            var civiBivakken = (from r in civiResult.Values
                // Negeer uitstappen die Ingrid manueel heeft ingevoerd (zie #5151)
                where r.GapUitstapId != null
                orderby r.GapUitstapId
                select r).ToList();

            Console.WriteLine(Resources.Program_BivakAangiftesFixen_Volledige_bivakaangiftes_ophalen_uit_het_GAP_);
            var gapBivakken = AlleBivakken(periodeStart, periodeEinde);
            Console.WriteLine(Resources.Program_Main_Dat_zijn_er__0__, gapBivakken.Length);

            var overTeZettenBivakken = OntbrekendInCiviZoeken(civiBivakken, gapBivakken);
            Console.WriteLine(Resources.Program_BivakAangiftesFixen__0__bivakken_uit_GAP_niet_gevonden_in_Civi_, overTeZettenBivakken.Count);

            // TODO: command line switch om deze vraag te vermijden.
            Console.Write(Resources.Program_Main_Meteen_syncen__);
            string input = Console.ReadLine();

            if (input.ToUpper() == "J" || input.ToUpper() == "Y")
            {
                BivakkenNaarCivi(overTeZettenBivakken, serviceHelper);
            }
        }

        private static void BivakkenNaarCivi(List<BivakInfo> overTeZettenBivakken, ServiceHelper serviceHelper)
        {
            int counter = 0;
            var sync = new BivakSync(serviceHelper);
            using (var context = new ChiroGroepEntities())
            {
                var repositoryProvider = new RepositoryProvider(context);
                var uitstappenRepo = repositoryProvider.RepositoryGet<Uitstap>();
                foreach (var b in overTeZettenBivakken)
                {
                    sync.Bewaren(uitstappenRepo.ByID(b.GapUitstapId));
                    Console.Write("{0} ", ++counter);
                }
            }
        }

        private static List<BivakInfo> OntbrekendInCiviZoeken(List<Event> civiBivakken, BivakInfo[] gapBivakken)
        {
            int civiCounter = 0;
            int gapCounter = 0;
            var teSyncen = new List<BivakInfo>();
            int aantalciviBivakken = civiBivakken.Count;

            // Normaal zijn de bivakken uit het GAP hetzelfde gesorteerd als die uit Civi.
            // Overloop de GAP-leden, en kijk of ze ook in de Civi-leden voorkomen.

            Console.WriteLine(Resources.Program_OntbrekendInCiviZoeken_Opzoeken_bivakken_in_GAP_die_niet_volledig_in_Civi_zitten_);
            while (gapCounter < gapBivakken.Length && civiCounter < aantalciviBivakken)
            {
                while (civiCounter < aantalciviBivakken && gapBivakken[gapCounter].GapUitstapId > civiBivakken[civiCounter].GapUitstapId)
                {
                    ++civiCounter;
                }
                if (civiCounter < aantalciviBivakken && gapBivakken[gapCounter].GapUitstapId != civiBivakken[civiCounter].GapUitstapId)
                {
                    teSyncen.Add(gapBivakken[gapCounter]);
                    Console.WriteLine("[{0} {1}]", gapBivakken[gapCounter].StamNr, gapBivakken[gapCounter].GapUitstapId);
                }
                ++gapCounter;
            }
            return teSyncen;
        }

        private static BivakInfo[] AlleBivakken(DateTime periodeStart, DateTime periodeEinde)
        {
            // Dependency injection is hier nog niet in orde.
            using (var context = new ChiroGroepEntities())
            {
                var repositoryProvider = new RepositoryProvider(context);
                var uitstappenRepo = repositoryProvider.RepositoryGet<Uitstap>();

                var alles = from biv in uitstappenRepo.Select()
                    where biv.DatumTot >= periodeStart &&
                          biv.DatumVan <= periodeEinde &&
                          biv.IsBivak &&
                          biv.ContactDeelnemer != null &&
                          biv.Plaats != null
                    select new BivakInfo() {StamNr = biv.GroepsWerkJaar.Groep.Code.Trim(), GapUitstapId = biv.ID};
                return alles.OrderBy(info => info.GapUitstapId).ToArray();
            }
        }
    }
}
