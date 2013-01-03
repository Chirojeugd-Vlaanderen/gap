// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

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