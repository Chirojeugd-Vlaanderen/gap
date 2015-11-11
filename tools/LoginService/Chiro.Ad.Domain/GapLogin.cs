/*
 * Copyright 2008-2015 the GAP developers. See the NOTICE file at the 
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
using System.DirectoryServices;

namespace Chiro.Ad.Domain
{
    /// <summary>
    /// Class voor bewerkingen op een GapLogin
    /// </summary>
    public class GapLogin : Chirologin
    {
        /// <summary>
        /// AD-pad van de node waar nieuwe logins aangemaakt worden voor GAP-accounts
        /// </summary>
        readonly static String ouPad =Properties.Settings.Default.GapOU;

        /// <summary>
        /// Zoekt of maakt een account in Active Directory.
        /// </summary>
        /// <param name="adNr">AD-nummer van de persoon in kwestie</param>
        /// <param name="voornaam">Voornaam van die persoon</param>
        /// <param name="familienaam">Familienaam van die persoon</param>
        /// <remarks>De account is nog niet actief en bevat nog geen mailadres.</remarks>
        public GapLogin(Int32 adNr, String voornaam, String familienaam)
            : base(DomeinEnum.Wereld, ouPad, adNr, voornaam, familienaam)
        {
            RechtenToekennen();
        }

        /// <summary>
        /// Geef de login de nodige rechten in Active Directory om aan de GAP-toepassing te kunnen
        /// </summary>
        private void RechtenToekennen()
        {
            if (!SecurityGroepen.Contains(Properties.Settings.Default.GapGebruikersGroep))
            {
                DirectoryEntry groep = LdapHelper.ZoekenUniek(Domein + Properties.Settings.Default.GapGroepenOU,
                                                            "Name=" + Properties.Settings.Default.GapGebruikersGroep);

                AanSecuritygroepToevoegen(groep);
            }
        }
    }
}