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

using Nancy;

namespace Chiro.Gap.UpdateApi.Modules
{
    /// <summary>
    /// Dit is een hacky module, ENKEL VOOR STAGING, niet voor live.
    /// De bedoeling is dat je via deze module het terugzetten van backups
    /// van gap en kipadmin naar staging kunt aansturen.
    /// </summary>
    public class DbHackModule: NancyModule
    {
        public DbHackModule()
        {
            Post["/dbhack/restorekip"] = _ =>
            {
                TestHacks.TestHacks.KipadminRestoren();
                return HttpStatusCode.NoContent;
            };
            Post["/dbhack/restoregap"] = _ =>
            {
                TestHacks.TestHacks.GapRestoren();
                return HttpStatusCode.NoContent;
            };
        }
    }
}