/*
 * Copyright 2008-2017 the GAP developers. See the NOTICE file at the 
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

namespace Chiro.Cdf.Intranet
{
    /// <summary>
    /// Class die de mailadrescontrole mockt. Opmerking: hier is geen enkel adres verdacht.
    /// </summary>
    public class FakeMailadrescontrole : IMailadrescontrole
    {
        public int BetrouwbaarheidsscoreOphalenOpNaam(string voornaam, string naam, string email)
        {
            // ik doe maar iets
            return 3;
        }

        public int BetrouwbaarheidsscoreOphalenOpNaamEnGeboortejaar(string voornaam, string naam, int geboortejaar, string email)
        {
            // ik doe maar iets
            return 3;
        }

        public bool MailadresIsVerdacht(string voornaam, string naam, string email)
        {
            // ik doe maar iets
            return false;
        }

        public bool MailadresIsVerdacht(string voornaam, string naam, int geboortejaar, string email)
        {
            // ik doe maar iets
            return false;
        }
    }
}
