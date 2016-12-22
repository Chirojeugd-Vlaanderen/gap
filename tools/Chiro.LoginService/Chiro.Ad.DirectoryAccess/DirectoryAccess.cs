/*
 * Copyright 2008-2015 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Refactoring AD-access (#4938) Copyright 2016, Chirojeugd-Vlaanderen vzw.
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
using Chiro.Ad.DirectoryInterface;
using Chiro.Ad.Domain;
using System.Collections.Generic;

namespace Chiro.Ad.DirectoryAccess
{
    /// <summary>
    /// Access to active directory
    /// </summary>
    public class DirectoryAccess: IDirectoryAccess
    {
        /// <summary>
        /// Voert een LDAP-query uit, en levert gevonden resultaat op, als dat uniek is.
        /// </summary>
        /// <param name="ldapRoot">Het ldapRoot waarin de account of groep zich bevindt, 
        /// eventueel de OU waarin we willen zoeken</param>
        /// <param name="filter">De zoekfilter die toegepast moet worden</param>
        /// <returns><c>null</c> als niet gevonden, het unieke resultaat als er een uniek resultaat is.
        /// Is het resultaat niet uniek, dan gooien we een exception</returns>
        private static DirectoryEntry ZoekenUniek(string ldapRoot, string filter)
        {
            var entry = new DirectoryEntry(ldapRoot) { AuthenticationType = AuthenticationTypes.Secure };
            var zoeker = new DirectorySearcher(entry) { Filter = filter };

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

        /// <summary>
        /// Haalt AD-entry voor gebruiker met gegeven <paramref name="adNummer"/> op uit active directory.
        /// </summary>
        /// <param name="adNummer"></param>
        /// <param name="ldapRoot"></param>
        /// <returns>De gebruiker met gegeven <paramref name="adNummer"/></returns>
        private static DirectoryEntry EntryOphalen(int adNummer, string ldapRoot)
        {
            return ZoekenUniek(ldapRoot, string.Format("(pager={0})", adNummer));
        }

        /// <summary>
        /// Haalt AD-entry op voor gebruiker met gegeven <paramref name="voornaam"/> en <paramref name="familienaam"/>
        /// </summary>
        /// <param name="voornaam"></param>
        /// <param name="familienaam"></param>
        /// <param name="ldapRoot"></param>
        /// <returns></returns>
        private static DirectoryEntry EntryOphalen(string voornaam, string familienaam, string ldapRoot)
        {
            return ZoekenUniek(ldapRoot, string.Format("(&(givenName={0})(sn={1}))", voornaam, familienaam));
        }

        /// <summary>
        /// Haalt AD-entry voor gebruiker met gegeven <paramref name="login"/> op uit active directory.
        /// </summary>
        /// <param name="login"></param>
        /// <param name="ldapRoot"></param>
        /// <returns>De gebruiker met gegeven <paramref name="login"/></returns>
        private static DirectoryEntry EntryOphalen(string login, string ldapRoot)
        {
            return ZoekenUniek(ldapRoot, string.Format("(sAMAccountName={0})", login));
        }

        /// <summary>
        /// Reset het wachtwoord van de gegeven <paramref name="login"/>.
        /// </summary>
        /// <param name="login"></param>
        /// <param name="wachtwoord">Te gebruiken wachtwoord.</param>
        public void PasswordReset(Chirologin login, string wachtwoord)
        {
            using (var gebruiker = EntryOphalen(login.Login, login.Domein))
            {
                gebruiker.Invoke("setPassword", wachtwoord);
                gebruiker.CommitChanges();
            }
        }

        /// <summary>
        /// Activeert de gegeven <paramref name="login"/>.
        /// </summary>
        /// <param name="login"></param>
        public void GebruikerActiveren(Chirologin login)
        {
            using (var gebruiker = EntryOphalen(login.Login, login.Domein))
            {
                gebruiker.Properties["userAccountControl"].Value = 66048; // enabled
                gebruiker.CommitChanges();
            }
        }

        /// <summary>
        /// Zoekt de login van een bestaande Chirogebruiker op.
        /// </summary>
        /// <param name="ldapRoot"></param>
        /// <param name="adNr"></param>
        /// <returns></returns>
        public Chirologin GebruikerZoeken(string ldapRoot, int adNr)
        {
            using (var entry = EntryOphalen(adNr, ldapRoot))
            {
                return entry == null ? null : EntryToLogin(ldapRoot, entry);
            }
        }

        /// <summary>
        /// Zoekt de login van een bestaande Chirogebruiker op.
        /// </summary>
        /// <param name="ldapRoot"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public Chirologin GebruikerZoeken(string ldaproot, string username)
        {
            using (var entry = EntryOphalen(username, ldaproot))
            {
                if (entry != null)
                {
                    return EntryToLogin(ldaproot, entry);
                }
                return null;
            }
        }

        /// <summary>
        /// Zoekt de login van een bestaande Chirogebruiker op.
        /// </summary>
        /// <param name="ldapRoot"></param>
        /// <param name="voornaam"></param>
        /// <param name="familienaam"></param>
        /// <returns></returns>
        public Chirologin GebruikerZoeken(string ldapRoot, string voornaam, string familienaam)
        {
            using (var entry = EntryOphalen(voornaam, familienaam, ldapRoot))
            {
                return entry == null ? null : EntryToLogin(ldapRoot, entry);
            }
        }

        /// <summary>
        /// Creates a chirologin from a given Active Directory <paramref name="entry"/>.
        /// </summary>
        /// <param name="ldapRoot"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        private static Chirologin EntryToLogin(string ldapRoot, DirectoryEntry entry)
        {
            return new Chirologin
            {
                AdNr =
                    entry.Properties["pager"].Value == null
                        ? (int?) null
                        : int.Parse(entry.Properties["pager"].Value.ToString()),
                Beschrijving =
                    entry.Properties["description"].Value == null
                        ? null
                        : entry.Properties["description"].Value.ToString(),
                BestondAl = true,
                Domein = ldapRoot,
                Familienaam = entry.Properties["sn"].Value.ToString(),
                IsActief = int.Parse(entry.Properties["userAccountControl"].Value.ToString()) == 66048,
                Login = entry.Properties["sAMAccountName"].Value.ToString(),
                Mailadres = entry.Properties["mail"].Value == null ? null : entry.Properties["mail"].Value.ToString(),
                Path = entry.Path,
                SecurityGroepen = SecurityGroepen(entry),
                Voornaam = entry.Properties["givenName"].Value.ToString()
            };
        }

        /// <summary>
        /// Levert de security groepen op van het gegeven active directory <paramref name="entry"/>.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private static List<string> SecurityGroepen(DirectoryEntry entry)
        {
            var result = new List<string>();
            var groepen = entry.Properties["memberOf"];
            var enumerator = groepen.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (enumerator.Current == null) continue;
                var groep = enumerator.Current.ToString();
                // en.Current heeft de vorm CN=Naam,OU=xxx,OU=yyy
                // Het is Naam dat we eruit willen halen, dus de eerste drie tekens verwijderen, en dan lengte min die drie
                groep = groep.Substring(3, groep.IndexOf(",") - 3);

                if (!result.Contains(groep))
                {
                    result.Add(groep);
                }
            }
            return result;
        }

        /// <summary>
        /// Bewaart de login van een nieuwe gebruiker.
        /// </summary>
        /// <param name="login"></param>
        /// <param name="gebruikerOu">OU waarin de gebruiker gemaakt moet worden.</param>
        public void NieuweGebruikerBewaren(Chirologin login, string gebruikerOu)
        {
            var ou = new DirectoryEntry(login.Domein + gebruikerOu);

            // TODO: De CN moet uniek zijn, en niet voornaam en familienaam. Als we dit properder
            // doen, dan moeten we niet foefelen met de voornaam in LoginManager.
            using (var gebruiker = ou.Children.Add("CN=" + login.Naam, "user"))
            {
                // Vul de nodige gegevens in
                // sAMAccountName is een verplicht veld, userPrincipalName is nodig voor ADAM
                gebruiker.Properties["sAMAccountName"].Value = login.Voornaam + login.AdNr; // login.Login is soms te lang, mag maar 20 tekens hebben
                gebruiker.Properties["userPrincipalName"].Value = login.Login;

                gebruiker.Properties["mail"].Value = login.Mailadres;

                gebruiker.Properties["givenName"].Value = login.Voornaam;
                gebruiker.Properties["sn"].Value = login.Familienaam;

                gebruiker.Properties["pager"].Value = login.AdNr;

                // Pad opslaan in description zodat ze makkelijker terug te vinden zijn bij opzoekingen in AD-tool
                // (Dit klinkt als een design fout...)
                gebruiker.Properties["description"].Value = gebruiker.Path;
                gebruiker.CommitChanges();
                login.Path = gebruiker.Path;
            }
        }

        /// <summary>
        /// Voegt gegeven <paramref name="gebruiker" /> toe aan de security groep
        /// <paramref name="groep" />.
        /// </summary>
        /// <param name="gebruiker">Gebruiker toe te voegen aan <paramref name="groep"/>.</param>
        /// <param name="groep">Groep waaraan <paramref name="gebruiker"/> toegevoegd moet worden.</param>
        /// <param name="groepOu">OU van de security group.</param>
        public void GebruikerToevoegenAanGroep(Chirologin gebruiker, string groep, string groepOu)
        {
            using (var groepEntry = ZoekenUniek(gebruiker.Domein + groepOu, "Name=" + groep))
            {
            	groepEntry.Invoke("Add", gebruiker.Path);
                groepEntry.CommitChanges();
            }
        }
    }
}
