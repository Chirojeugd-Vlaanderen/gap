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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Logic
{
    public static class PersoonLogic
    {
        /// <summary>
        /// Zet een gegeven GAP-<paramref name="geslacht"/> om naar een gender in
        /// CiviCrm.
        /// </summary>
        /// <param name="geslacht">Om te zetten geslacht</param>
        /// <returns>CiviCRM-gender</returns>
        public static Gender? GeslachtNaarGender(GeslachtsEnum geslacht)
        {
            switch (geslacht)
            {
                case GeslachtsEnum.Man:
                    return Gender.Male;
                case GeslachtsEnum.Vrouw:
                    return Gender.Female;
                case GeslachtsEnum.X:
                    return Gender.Transgender;
                default:
                    return null;
            }
        }
    }
}
