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
using System.DirectoryServices;
using System.Linq;

namespace Chiro.Ad.Domain
{
    /// <summary>
    /// Een aantal active-directory-gerelateerde operaties in een statische klasse.
    /// </summary>
    public static class LdapHelper
    {
        /// <summary>
        /// Voert een LDAP-query uit, en levert gevonden resultaat op, als dat uniek is.
        /// </summary>
        /// <param name="ldapRoot">Het domein waarin de account of groep zich bevindt, 
        /// eventueel de OU waarin we willen zoeken</param>
        /// <param name="filter">De zoekfilter die toegepast moet worden</param>
        /// <returns><c>null</c> als niet gevonden, het unieke resultaat als er een uniek resultaat is.
        /// Is het resultaat niet uniek, dan gooien we een exception</returns>
        public static DirectoryEntry ZoekenUniek(string ldapRoot, string filter)
        {
            var entry = new DirectoryEntry(ldapRoot) {AuthenticationType = AuthenticationTypes.Secure};
            var zoeker = new DirectorySearcher(entry) {Filter = filter};

            var resultaat = zoeker.FindAll();

            switch (resultaat.Count)
            {
                case 0:
                    return null;
                case 1:
                    return resultaat[0].GetDirectoryEntry();
                default:
                    throw new ArgumentException(); // Is een rare exception, ik weet het.
            }
        }
    }
}