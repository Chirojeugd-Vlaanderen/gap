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
using System.Linq;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. Chirogroepen bevat
    /// </summary>
    public class ChiroGroepenManager : IChiroGroepenManager
    {
        private readonly IAutorisatieManager _autorisatieMgr;

        public ChiroGroepenManager(IAutorisatieManager autorisatieMgr)
        {
            _autorisatieMgr = autorisatieMgr;
        }

        /// <summary>
        /// Maakt een nieuwe afdeling voor een Chirogroep, zonder te persisteren
        /// </summary>
        /// <param name="groep">
        /// Chirogroep waarvoor afdeling moet worden gemaakt, met daaraan gekoppeld
        /// de bestaande afdelingen
        /// </param>
        /// <param name="naam">
        /// Naam van de afdeling
        /// </param>
        /// <param name="afkorting">
        /// Handige afkorting voor in schemaatjes
        /// </param>
        /// <returns>
        /// De toegevoegde (maar nog niet gepersisteerde) afdeling
        /// </returns>
        /// <exception cref="GeenGavException">
        /// Komt voor als de gebruiker geen GAV is voor de opgegeven <paramref name="groep"/>
        /// </exception>
        public Afdeling AfdelingToevoegen(ChiroGroep groep, string naam, string afkorting)
        {
            if (!_autorisatieMgr.IsGav(groep))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            // Controleren of de afdeling nog niet bestaat
            var bestaand = from afd in groep.Afdeling
                           where string.Compare(afd.Afkorting, afkorting, true) == 0
                                 || string.Compare(afd.Naam, naam, true) == 0
                           select afd;

            if (bestaand.FirstOrDefault() != null)
            {
                // TODO (#507): Check op bestaande afdeling door DB
                throw new BestaatAlException<Afdeling>(bestaand.FirstOrDefault());
            }

            var a = new Afdeling
                        {
                            Afkorting = afkorting, 
                            Naam = naam
                        };

            a.ChiroGroep = groep;
            groep.Afdeling.Add(a);

            return a;
        }
    }
}
