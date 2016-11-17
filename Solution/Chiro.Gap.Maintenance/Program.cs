/*
 * Copyright 2015 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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

using System;
using Chiro.Cdf.Ioc.Factory;
using Chiro.Cdf.Mailer;
using Chiro.Gap.Maintenance.Properties;
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

            if (queueMonitor.AantalBerichten(Settings.Default.QueueNaam) >
                Settings.Default.MaxBerichten)
            {
                mailer.Verzenden(Settings.Default.Afzender, Settings.Default.Ontvanger,
                    Settings.Default.Onderwerp, Settings.Default.Inhoud);
                Console.WriteLine("Queue nog te vol. Verzend enkel waarschuwingsmail.");
            }
            else
            {
                relationshipMaintenance.LedenZonderAdOpnieuwSyncen();
                membershipMaintenance.MembershipsMaken();
            }
        }
    }
}
