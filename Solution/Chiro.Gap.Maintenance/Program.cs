/*
 * Copyright 2015 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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

using Chiro.Cdf.Ioc;
using Chiro.Cdf.Mailer;
using Chiro.Gap.Sync;

namespace Chiro.Gap.Maintenance
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Factory.ContainerInit();
            MappingHelper.MappingsDefinieren();

            var membershipMaintenance = Factory.Maak<MembershipMaintenance>();
            var relationshipMaintenance = Factory.Maak<RelationshipMaintenance>();
            var queueMonitor = new QueueMonitor();

            var mailer = Factory.Maak<IMailer>();

            if (queueMonitor.AantalBerichten(Properties.Settings.Default.QueueNaam) >
                Properties.Settings.Default.MaxBerichten)
            {
                mailer.Verzenden(Properties.Settings.Default.Afzender, Properties.Settings.Default.Ontvanger,
                    Properties.Settings.Default.Onderwerp, Properties.Settings.Default.Inhoud);
            }
            else
            {
                relationshipMaintenance.LedenZonderAdOpnieuwSyncen();
                membershipMaintenance.MembershipsMaken();
            }
        }
    }
}
