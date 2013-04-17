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
﻿using Chiro.Gap.Domain;

namespace Chiro.Gap.Validatie
{
    /// <summary>
    /// Klasse die een periode valideert. Ihb: is er een begin- en einddatum, en 
    /// ligt de begindatum voor de einddatum :)
    /// </summary>
    public class PeriodeValidator: Validator<IPeriode>
    {
        /// <summary>
        /// Valideert de periode <paramref name="teValideren"/>
        /// </summary>
        /// <param name="teValideren">te valideren periode</param>
        /// <returns><c>null</c> als de periode aan de regels voldoet, anders een foutnummer</returns>
        public override FoutNummer? FoutNummer(IPeriode teValideren)
        {
            if (teValideren.DatumVan != null && teValideren.DatumTot != null &&
                teValideren.DatumVan <= teValideren.DatumTot)
            {
                return null;
            }
            return Domain.FoutNummer.ChronologieFout;
        }
    }
}
