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
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Bevat de businesslogica voor bivakplaatsen.
    /// </summary>
    public class PlaatsenManager
    {
        private readonly IAutorisatieManager _autorisatieManager;

        public PlaatsenManager(IAutorisatieManager auMgr)
        {
            _autorisatieManager = auMgr;
        }

        /// <summary>
        /// Maakt een bivakplaats op basis van de naam <paramref name="plaatsNaam"/>, het
        /// <paramref name="adres"/> van de bivakplaats, en de ingevende
        /// <paramref name="groep"/>.
        /// </summary>
        /// <param name="plaatsNaam">
        /// Naam van de bivakplaats
        /// </param>
        /// <param name="adres">
        /// Adres van de bivakplaats
        /// </param>
        /// <param name="groep">
        /// Groep die de bivakplaats ingeeft
        /// </param>
        /// <returns>
        /// De nieuwe plaats; niet gepersisteerd.
        /// </returns>
        private Plaats Maken(string plaatsNaam, Adres adres, Groep groep)
        {
            var resultaat = new Plaats { Naam = plaatsNaam, Adres = adres, Groep = groep, ID = 0 };

            adres.BivakPlaats.Add(resultaat);
            groep.BivakPlaats.Add(resultaat);

            return resultaat;
        }
    }
}