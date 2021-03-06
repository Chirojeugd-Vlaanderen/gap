﻿/*
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
using System.Diagnostics;

namespace Chiro.Gap.Maintenance
{
    public class QueueMonitor
    {
        /// <summary>
        /// Levert het aantal berichten in de message queue.
        /// </summary>
        /// <returns>Het aantal berichten in de message queue :-).</returns>
        public int AantalBerichten(string queueName)
        {
            int result;
            try
            {
                var queueCounter = new PerformanceCounter("MSMQ Queue", "Messages in Queue", queueName);
                result = (int)queueCounter.NextValue();
            }
            catch (InvalidOperationException)
            {
                // In sommige gevallen wordt bij een lege queue een exception gegooid.
                // Maar niet altijd. Het is me niet helemaal duidelijk wanneer wel en wanneer
                // niet, maar voor de zekerheid catch ik deze exception dus maar.
                result = 0;
            }
            return result;
        }
    }
}
