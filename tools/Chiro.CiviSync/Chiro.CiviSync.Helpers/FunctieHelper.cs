/*
   Copyright 2015 Chirojeugd-Vlaanderen vzw

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

using System.Collections.Generic;
using System.Linq;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Helpers
{
    public class FunctieHelper
    {
        public static readonly IDictionary<FunctieEnum, string> KipCode = new Dictionary<FunctieEnum, string>
        {
            {FunctieEnum.ContactPersoon, "GG1"},
            {FunctieEnum.GroepsLeiding, "GG2"},
            {FunctieEnum.Vb, "GV1"},
            {FunctieEnum.FinancieelVerantwoordelijke, "FI"},
            {FunctieEnum.JeugdRaad, "JR"},
            {FunctieEnum.KookPloeg, "KK"},
            {FunctieEnum.Proost, "GP"},
            {FunctieEnum.GroepsLeidingsBijeenkomsten, "2BJ"},
            {FunctieEnum.SomVerantwoordelijke, "2SO"},
            {FunctieEnum.IkVerantwoordelijke, "2IK"},
            {FunctieEnum.RibbelVerantwoordelijke, "2AP"},
            {FunctieEnum.SpeelclubVerantwoordelijke, "2AS"},
            {FunctieEnum.RakwiVerantwoordelijke, "2AR"},
            {FunctieEnum.TitoVerantwoordelijke, "2AT"},
            {FunctieEnum.KetiVerantwoordelijke, "2AK"},
            {FunctieEnum.AspiVerantwoordelijke, "2AA"},
            {FunctieEnum.SomGewesten, "3SO"},
            {FunctieEnum.OpvolgingStadsGroepen, "3VS"},
            {FunctieEnum.Verbondsraad, "3VR"},
            {FunctieEnum.Verbondskern, "3KE"},
            {FunctieEnum.StartDagVerantwoordelijker, "3SD"},
            {FunctieEnum.SbVerantwoordelijke, "3BS"}
        };

        /// <summary>
        /// Zet <paramref name="functies"/> van het GAP om naar functies voor
        /// ChiroCivi, die eigenlijk bepaald worden door de codes in de oude
        /// Kipadmin.
        /// </summary>
        /// <param name="functies">Om te zetten functies.</param>
        /// <returns>Functiecodes uit ChiroCivi.</returns>
        public string[] KipCodes(IEnumerable<FunctieEnum> functies)
        {
            return functies == null ? null : (from f in functies select KipCode[f]).ToArray();
        }
    }
}
