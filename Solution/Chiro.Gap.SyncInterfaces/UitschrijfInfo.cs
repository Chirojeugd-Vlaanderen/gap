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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiro.Gap.SyncInterfaces
{
    /// <summary>
    /// Klasse met informatie voor uitschrijving van een lid dat niet (meer)
    /// in het GAP zit. Dit dient voornamelijk om onregelmatigheden op te lossen,
    /// zie #4554.
    /// </summary>
    public class UitschrijfInfo
    {
        public string StamNummer { get; set; }
        public int AdNummer { get; set; }
        public int WerkJaar { get; set; }
        public DateTime UitschrijfDatum { get; set; }
    }
}
