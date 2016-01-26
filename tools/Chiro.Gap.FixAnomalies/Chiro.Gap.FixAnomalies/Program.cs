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

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Workers;

namespace Chiro.Gap.FixAnomalies
{
    partial class Program
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

            LidRelatiesFixen(serviceHelper, apiKey, siteKey);
        }
    }
}
