/*
 * Copyright 2008-2017 the GAP developers. See the NOTICE file at the 
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
using System;
using System.Collections;
using System.Collections.Generic;
using Chiro.Ad.DirectoryInterface;
using Chiro.Ad.Domain;
using Chiro.Cdf.Mailer;
using System.Linq;

namespace Chiro.Ad.Workers
{
    /// <summary>
    /// De bedoeling is dat de businesslogica voor de logins verhuist naar deze klasse.
    /// Zie ticket #1152.
    /// </summary>
    public class LoginManager
    {
        private readonly IMailer _mailer;
        private readonly IDirectoryAccess _directoryAccess;

        /// <summary>
        /// Standaardconstructor
        /// </summary>
        /// <param name="mailer">IMailer via dewelke mails verstuurd zullen worden.</param>
        /// <param name="directoryAccess">interface voor toegang tot active directory</param>
        public LoginManager(IMailer mailer, IDirectoryAccess directoryAccess)
        {
            _mailer = mailer;
            _directoryAccess = directoryAccess;
        }


        /// <summary>
        /// Het opgegeven wachtwoord instellen en de account enablen
        /// </summary>
        /// <param name="login">Login waarvoor wachtwoord moet worden ingesteld</param>
        /// <param name="nieuwWachtWoord">Het wachtwoord dat ingesteld moet worden. Als dat een lege
        /// string is, wordt een willekeurig wachtwoord gegenereerd.</param>
        public void Activeren(Chirologin login, string nieuwWachtWoord)
        {
            var wachtWoord = nieuwWachtWoord == string.Empty ? WachtWoordMaken() : nieuwWachtWoord;
            _directoryAccess.PasswordReset(login, wachtWoord);
            _directoryAccess.GebruikerActiveren(login);
        }

        /// <summary>
        /// Maakt een nieuw willekeurig wachtwoord
        /// </summary>
        /// <returns>Het wachtwoord</returns>
        private static string WachtWoordMaken()
        {
            return RandomPassword.Generate();
        }


        /// <summary>
        /// Activeert de gegeven <paramref name="login"/>, en stuurt de gebruiker een mailtje
        /// </summary>
        /// <param name="login">Te activeren login</param>
        public void ActiverenEnMailen (Chirologin login)
        {
            string boodschap;

            // In principe wordt dit enkel aangeroepen als de gebruiker al bestond.

            string wachtWoord = WachtWoordMaken();
            Activeren(login, wachtWoord);

            boodschap = string.Format(Properties.Resources.GapAccountInfoMail, login.Naam, login.Login, wachtWoord, login.Mailadres);

            _mailer.Verzenden(login.Mailadres, "Je GAP-login", boodschap);
        }

        /// <summary>
        /// Zoekt een login op, op basis van AD-nummer.
        /// 
        /// Kipdorplogins hebben voorrang op Chiro-wereld-logins.
        /// </summary>
        /// <param name="adNr">AD-nummer van te zoeken login.</param>
        /// <returns>Login op basis van AD-nummer.</returns>
        public Chirologin Zoeken(int adNr)
        {
            // Probeer eerst kipdorp. Als dat niet lukt wereld.
            return _directoryAccess.GebruikerZoeken(Properties.Settings.Default.LdapLokaalRoot, adNr)
                ?? _directoryAccess.GebruikerZoeken(Properties.Settings.Default.LdapWereldRoot, adNr);
        }

        /// <summary>
        /// Zoekt een login op, op basis van username.
        /// 
        /// Kipdorplogins hebben voorrang op Chiro-wereld-logins.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public Chirologin Zoeken(string username)
        {
            var login = username.Split('\\').LastOrDefault();
           
            return _directoryAccess.GebruikerZoeken(Properties.Settings.Default.LdapLokaalRoot, login)
                ?? _directoryAccess.GebruikerZoeken(Properties.Settings.Default.LdapWereldRoot, login);
        }

        /// <summary>
        /// Zoekt een login, of maakt aan als nog niet gevonden.
        /// </summary>
        /// <param name="domein">Domein waarin de login gemaakt moet worden.</param>
        /// <param name="adNr">AD-nummer van persoon voor wie een login is gevraagd.</param>
        /// <param name="voornaam">Voornaam van persoon voor wie een login is gevraagd.</param>
        /// <param name="familienaam">Familienaam van persoon voor wie een login is gevraagd.</param>
        /// <param name="mailadres">E-mailadres van persoon voor wie een login is gevraagd.</param>
        /// <returns></returns>
        public Chirologin ZoekenOfMaken(DomeinEnum domein, int adNr, string voornaam, string familienaam, string mailadres)
        {
            string ldapRoot;

            switch (domein)
            {
                case DomeinEnum.Lokaal:
                    ldapRoot = Properties.Settings.Default.LdapLokaalRoot;
                    break;
                case DomeinEnum.Wereld:
                    ldapRoot = Properties.Settings.Default.LdapWereldRoot;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(domein));
            }

            var login = _directoryAccess.GebruikerZoeken(ldapRoot, adNr);

            if (login != null)
            {
                // Als we een gebruiker hebben gevonden, moeten we niets meer doen. We leveren de
                // bestaande login op.
                return login;
            }

            // Er bestaat nog geen gebruiker. Nu moeten we nakijken of er al een andere gebruiker bestaat met dezelfde
            // naam, want daar kan Active Directory niet mee lachen. In dat laatste geval foefelen we wat, tot we toch
            // verder kunnen. (foefelen in de zin van cijfers aan de voornaam plakken)
            // UPDATE: Vermoedelijk mag dat wel, eenzelfde voornaam en familienaam, maar moet de 'common name'
            // (waarvoor wij voornaam en familienaam nemen) uniek zijn.

            int teller = 0;
            string oorspronkelijkeVoornaam = voornaam;
            string oorspronkelijkeFamilienaam = familienaam;

            while (_directoryAccess.GebruikerZoeken(ldapRoot, voornaam, familienaam) != null)
            {
                voornaam = String.Format("{0}{1}", oorspronkelijkeVoornaam, ++teller);
            }

            // Als gebruikersnaam nemen we normaal voornaam.famielienaam, eventueel met de teller er achter. We gaan
            // er (eigenlijk ten onrechte, maar soit) van uit dat die nog niet bestaat.
            // 't Is een beetje raar dat voor de naam van de user het volgnummer aan de voornaam wordt geplakt, en
            // voor de login aan de familienaam, maar dat is dan voorlopig nog maar zo. Wie wil, kan dit fixen.

            // haal spaties, weglatingstekens en koppeltekens uit de naam
            oorspronkelijkeVoornaam = oorspronkelijkeVoornaam.Trim().Replace("'", string.Empty).Replace(" ", string.Empty).Replace("-", string.Empty);
            oorspronkelijkeFamilienaam = oorspronkelijkeFamilienaam.Trim().Replace("'", string.Empty).Replace(" ", string.Empty).Replace("-", string.Empty);

            // vervang lettertekens met accenten of een trema door andere
            oorspronkelijkeVoornaam = oorspronkelijkeVoornaam.Replace("é", "e").Replace("è", "e").Replace("ä", "a").Replace("à", "a").Replace("ù", "u").Replace("î", "i").Replace("ï", "i").Replace("ê", "e").Replace("ë", "e");
            oorspronkelijkeFamilienaam = oorspronkelijkeFamilienaam.Replace("é", "e").Replace("è", "e").Replace("ä", "a").Replace("à", "a").Replace("ù", "u").Replace("î", "i").Replace("ï", "i").Replace("ê", "e").Replace("ë", "e");

            // vervang andere tekens
            oorspronkelijkeVoornaam = oorspronkelijkeVoornaam.Replace("ç", "c");
            oorspronkelijkeFamilienaam = oorspronkelijkeFamilienaam.Replace("ç", "c");

            login = new Chirologin
            {
                // lowercase login ziet er professioneler uit :-)
                Login =
                    String.Format("{0}.{1}{2}", oorspronkelijkeVoornaam, oorspronkelijkeFamilienaam,
                        teller > 0 ? String.Format(".{0}", teller) : String.Empty).ToLower(),
                Voornaam = voornaam,
                Familienaam = familienaam,
                AdNr = adNr,
              	Mailadres = mailadres,
                Domein = ldapRoot,
                SecurityGroepen = new List<string>()
            };
            _directoryAccess.NieuweGebruikerBewaren(login, Properties.Settings.Default.GapGebruikersOU);
            return login;
        }

        /// <summary>
        /// Voegt de gegeven <paramref name="gebruiker" /> toe aan de gebruikersgroep
        /// van de GAP-users.
        /// </summary>
        public void GapRechtenToekennenEnMailen(Chirologin gebruiker)
        {
            string groep = Properties.Settings.Default.GapGebruikersGroep;
            string groepOu = Properties.Settings.Default.GapGroepenOU;
            if (!((IList) gebruiker.SecurityGroepen).Contains(groep))
            {
                _directoryAccess.GebruikerToevoegenAanGroep(gebruiker, groep, groepOu);
            }

            var boodschap = string.Format(Properties.Resources.GapRechtenInfoMail, gebruiker.Voornaam);

            _mailer.Verzenden(gebruiker.Mailadres, "GAP-toegang", boodschap);
        }
    }
}
