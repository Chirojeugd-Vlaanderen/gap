/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
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
using System.Text.RegularExpressions;

using Chiro.Gap.Domain;

namespace Chiro.Gap.Validatie
{
	/// <summary>
	/// Klasse die validatieregels controleert voor Communicatievormen
	/// </summary>
	public class CommunicatieVormValidator : Validator<ICommunicatie>
	{
        /// <summary>
        /// Vergelijkt de opgegeven waarde met de regex die in de databank opgegeven is
        /// voor dat communicatieType
        /// </summary>
        /// <param name="teValideren">De communicatievorm (bv. telefoonnummer, mailadres, ...</param>
        /// <returns>
        /// <c>null</c> als de waarde ('Nummer') voldoet aan de opgegeven Regex voor dat communicatietype,
        /// en anders een foutnummer.
        /// </returns>
	    public override FoutNummer? FoutNummer(ICommunicatie teValideren)
	    {
            if (teValideren != null && teValideren.Nummer != null &&
                Regex.IsMatch(teValideren.Nummer, teValideren.CommunicatieTypeValidatie, RegexOptions.IgnoreCase))
            {
                return null;
            }
            // domme algemene validatiefout.
            return Domain.FoutNummer.ValidatieFout;
	    }
	}
}
