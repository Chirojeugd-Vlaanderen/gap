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

using System;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.SyncInterfaces;
using Chiro.Kip.ServiceContracts;

namespace Chiro.Gap.Sync
{
    /// <summary>
    /// Deze klasse staat in voor het overzetten van verzekeringsgegevens naar Kipadmin.
    /// </summary>
    public class VerzekeringenSync : IVerzekeringenSync
    {
        /// <summary>
        /// Zet de gegeven <paramref name="persoonsVerzekering"/> over naar Kipadmin.
        /// </summary>
        /// <param name="persoonsVerzekering">Over te zetten persoonsverzekering</param>
        /// <param name="gwj">Bepaalt werkJaar en groep die factuur zal krijgen</param>
        public void Bewaren(PersoonsVerzekering persoonsVerzekering, GroepsWerkJaar gwj)
        {
            if (persoonsVerzekering.Persoon.AdNummer.HasValue)
            {
                // Verzekeren op basis van AD-nummer
                ServiceHelper.CallService<ISyncPersoonService>(svc => svc.LoonVerliesVerzekeren(
                    persoonsVerzekering.Persoon.AdNummer.Value,
                    gwj.Groep.Code,
                    gwj.WerkJaar));
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
