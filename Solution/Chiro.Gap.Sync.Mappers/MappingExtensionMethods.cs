/*
 * Copyright 2017 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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

using System.Diagnostics;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.Sync.Mappers
{
    public static class MappingExtensionMethods
    {
        public static string LandGet(this Adres adres)
        {
            if (adres is BelgischAdres)
            {
                return null;
            }
            Debug.Assert(adres is BuitenLandsAdres);
            return ((BuitenLandsAdres)adres).Land.Naam;
        }

        public static string LandCodeGet(this Adres adres)
        {
            if (adres is BelgischAdres)
            {
                return "BE";
            }
            Debug.Assert(adres is BuitenLandsAdres);
            return ((BuitenLandsAdres)adres).Land.IsoCode;
        }

        public static string PostCodeGet(this Adres adres)
        {
            if (adres is BelgischAdres)
            {
                return null;
            }
            Debug.Assert(adres is BuitenLandsAdres);
            return ((BuitenLandsAdres)adres).PostCode;
        }
    }
}