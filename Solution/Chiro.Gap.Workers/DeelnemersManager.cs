/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
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
using System.Linq;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// TODO (#190): documenteren
    /// </summary>
    public class DeelnemersManager
    {
        private readonly IAutorisatieManager _autorisatieMgr;

        public DeelnemersManager(IAutorisatieManager autorisatieManager)
        {
            _autorisatieMgr = autorisatieManager;
        }

        /// <summary>
        /// Stelt de gegeven <paramref name="deelnemer"/> in als contactpersoon voor de uitstap waaraan
        /// hij deelneemt.
        /// </summary>
        /// <param name="deelnemer">
        /// De als contactpersoon in te stellen deelnemer
        /// </param>
        /// <returns>
        /// Diezelfde deelnemer
        /// </returns>
        /// <remarks>
        /// De deelnemer moet aan zijn uitstap gekoppeld zijn
        /// </remarks>
        public Deelnemer InstellenAlsContact(Deelnemer deelnemer)
        {
            if (!_autorisatieMgr.IsGav(deelnemer))
            {
                throw new GeenGavException();
            }
            else
            {
                Debug.Assert(deelnemer.Uitstap != null);

                if (deelnemer.UitstapWaarvoorVerantwoordelijk.FirstOrDefault() == null)
                {
                    // Een deelnemer kan alleen contact voor zijn eigen uitstap zijn.  Is de deelnemer
                    // al contact voor een uitstap, dan volgt daaruit dat hij al contact is voor zijn
                    // eigen uitstap.
                    var vorigeVerantwoordelijke = deelnemer.Uitstap.ContactDeelnemer;

                    if (vorigeVerantwoordelijke != null)
                    {
                        vorigeVerantwoordelijke.UitstapWaarvoorVerantwoordelijk = null;
                    }

                    deelnemer.Uitstap.ContactDeelnemer = deelnemer;
                    deelnemer.UitstapWaarvoorVerantwoordelijk.Add(deelnemer.Uitstap);
                }
            }

            return deelnemer;
        }
    }
}