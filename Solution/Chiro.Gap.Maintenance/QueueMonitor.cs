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

using System.Diagnostics;

namespace Chiro.Gap.Maintenance
{
    public class QueueMonitor
    {
        /// <summary>
        /// Levert het aantal berichten in de message queue.
        /// </summary>
        /// <returns><c>true</c> als alles in orde lijkt. Anders <c>false</c>.</returns>
        public int AantalBerichten(string queueName)
        {
            var queueCounter = new PerformanceCounter("MSMQ queue", "Messages in Queue", queueName);
            return (int)queueCounter.NextValue();
        }
    }
}
