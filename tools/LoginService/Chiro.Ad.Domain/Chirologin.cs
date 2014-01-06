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
using System.Collections;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security;
using System.Text.RegularExpressions;

namespace Chiro.Ad.Domain
{
    /// <summary>
    /// Algemene class voor beheer van Chirologins
    /// </summary>
    public class Chirologin
    {
        #region  properties

        /// <summary>
        /// De account zelf
        /// </summary>
        private readonly DirectoryEntry _gebruiker;

        /// <summary>
        /// Het domein in Active Directory waar de account zich bevindt
        /// </summary>
        public String Domein { get; set; }

        /// <summary>
        /// De login van de gebruiker
        /// </summary>
        public String Login
        {
            get
            {
                return _gebruiker.Properties["sAMAccountName"].Value.ToString();
            }
        }

        /// <summary>
        /// De naam van de gebruiker
        /// </summary>
        public String Naam
        {
            get
            {
                // Verwijder "CN=" uit de info die AD geeft, de rest is de naam
                return _gebruiker.Name.Remove(0, 3);
            }
        }

        /// <summary>
        /// Het AD-nummer van de gebruiker
        /// </summary>
        public Int32 AdNr
        {
            get
            {
                return _gebruiker.Properties["pager"].Value == null ? 0 : Int32.Parse(_gebruiker.Properties["pager"].Value.ToString());
            }
            set
            {
                _gebruiker.Properties["pager"].Value = value;
            }
        }

        /// <summary>
        /// Het mailadres van de gebruiker
        /// </summary>
        public String Mailadres
        {
            get
            {
                if (_gebruiker.Properties["mail"].Value != null)
                {
                    return _gebruiker.Properties["mail"].Value.ToString();
                }
                return string.Empty;
            }
            set
            {
                if (Regex.IsMatch(value, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
                {
                    _gebruiker.Properties["mail"].Value = value;
                }
                else
                {                    
                    throw new FormatException(Properties.Resources.OngeldigMailadresFout);
                }
            }
        }

        /// <summary>
        /// De beschrijving (Description) van de gebruiker
        /// </summary>
        public String Beschrijving
        {
            get
            {
                if (_gebruiker.Properties["description"].Value != null)
                {
                    return _gebruiker.Properties["description"].Value.ToString();
                }
                return string.Empty;
            }
            set
            {
                _gebruiker.Properties["description"].Value = value;
            }
        }

        /// <summary>
        /// Waar bevindt de account zich precies in Active Directory?
        /// </summary>
        public String Path
        {
            get
            {
                return _gebruiker.Path;
            }
        }

        /// <summary>
        /// Is de account enabled?
        /// </summary>
        public bool IsActief
        {
            get
            {
                return (int)_gebruiker.Properties["userAccountControl"].Value == 66048;
            }
            private set
            {
                if (value == false)
                {
                    _gebruiker.Properties["userAccountControl"].Value = 66050; // disabled
                }
            }
        }

        /// <summary>
        /// Het AD-wachtwoord van de gebruiker
        /// </summary>
        public String Wachtwoord
        {
            set
            {
                _gebruiker.Invoke("setPassword", value);
            }
        }

        /// <summary>
        /// Securitygroepen waar de account member van is
        /// </summary>
        public ArrayList SecurityGroepen
        {
            get
            {
                var groepenlijst = new ArrayList();
                PropertyValueCollection collectie = _gebruiker.Properties["memberOf"];
                IEnumerator en = collectie.GetEnumerator();

                while (en.MoveNext())
                {
                    if (en.Current != null)
                    {
                        var groep = en.Current.ToString();
                        // en.Current heeft de vorm CN=Naam,OU=xxx,OU=yyy
                        // Het is Naam dat we eruit willen halen, dus de eerste drie tekens verwijderen, en dan lengte min die drie
                        groep = groep.Substring(3, groep.IndexOf(",") - 3);

                        if (!groepenlijst.Contains(groep))
                        {
                            groepenlijst.Add(groep);
                        }
                    }
                }
                return groepenlijst;
            }
        }

        /// <summary>
        /// Geeft aan of de account al bestond of dat hij nieuw aangemaakt is
        /// </summary>
        public bool BestondAl { get; set; }

        #endregion

        /// <summary>
        /// De standaardconstructor
        /// </summary>
        /// <param name="domein">Rootpad in Active Directory</param>
        /// <param name="ouPad">Relatief pad naar de organisational unit waar nieuwe accounts in aangemaakt worden</param>
        /// <param name="adNr">Het AD-nummer van de nieuwe gebruiker</param>
        /// <param name="voornaam">De voornaam van de nieuwe gebruiker</param>
        /// <param name="familienaam">De naam van de nieuwe gebruiker</param>
        public Chirologin(DomeinEnum domein, String ouPad, Int32 adNr, String voornaam, String familienaam)
        {
            string naamVoluit = String.Concat(voornaam, " ", familienaam);
            DirectoryEntry ou;

            // Controleer of er nog geen account bestaat met die login, en suggereer eventueel een andere
            const int voornaamSeed = 2;
            const int familienaamSeed = 5;

            switch (domein)
            {
                case DomeinEnum.Lokaal:
                    Domein = Properties.Settings.Default.LdapLokaalRoot;
                    break;
                case DomeinEnum.Wereld:
                    Domein = Properties.Settings.Default.LdapWereldRoot;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("domein");
            }

            _gebruiker = LdapHelper.ZoekenUniek(Domein, string.Format("(pager={0})", adNr));

            // Als we een uniek resultaat hebben, is onze gebruiker gevonden.
            // Als we geen resultaat vinden, dan hebben we nog werk.

            if (_gebruiker == null)
            {
                // Er bestaat nog geen gebruiker. Nu moeten we nakijken of er al een andere gebruiker bestaat met dezelfde
                // naam, want daar kan Active Directory niet mee lachen. In dat laatste geval foefelen we wat, tot we toch
                // verder kunnen. (foefelen in de zin van cijfers aan de voornaam plakken)

                int teller = 0;
                string oorspronkelijkeVoornaam = voornaam;

                while (
                    LdapHelper.ZoekenUniek(Domein, string.Format("(&(givenName={0})(sn={1}))", voornaam, familienaam)) !=
                    null)
                {
                    voornaam = String.Format("{0}{1}", oorspronkelijkeVoornaam, ++teller);
                }

                // Dan zou het nog kunnen dat we ergens een dubbele login hebben, en dat kan ook niet. Bart had hiervoor
                // al een truuk, namelijk minder letters gebruiken van de familienaam, en meer van de voornaam. Maar voor het
                // geval dat niet moest volstaan, plakken we daar ook een nummer achter

                int teller2 = 0;
                bool accountGemaakt = false;

                while (!accountGemaakt)
                {
                    for (int i = 0; !accountGemaakt && i < 4; i++)
                    {
                        string login = LoginSuggereren(voornaam, familienaam, familienaamSeed - i, voornaamSeed + i);

                        if (teller2 > 0)
                        {
                            login = String.Format("{0}{1}", login, teller2);
                        }

                        // Controleer of er nog geen account bestaat met die login
                        // Als we niets vinden, kunnen we hiermee verder.
                        if (LdapHelper.ZoekenUniek(Domein, string.Concat("sAMAccountName=", login)) == null)
                        {
                            // Zoek de 'organisational unit' op waar de account in terecht moet komen en zet de account erin.    
                            ou = new DirectoryEntry(Domein + ouPad);
                            _gebruiker = ou.Children.Add("CN=" + naamVoluit, "user");

                            // Vul de nodige gegevens in
                            // sAMAccountName is een verplicht veld, userPrincipalName is nodig voor ADAM
                            _gebruiker.Properties["sAMAccountName"].Value = login;
                            _gebruiker.Properties["userPrincipalName"].Value = login;

                            _gebruiker.Properties["givenName"].Value = voornaam;
                            _gebruiker.Properties["sn"].Value = familienaam;

                            _gebruiker.Properties["pager"].Value = adNr;

                            // Pad opslaan in description zodat ze makkelijker terug te vinden zijn bij opzoekingen in AD-tool
                            _gebruiker.Properties["description"].Value = _gebruiker.Path;

                            Opslaan();
                            accountGemaakt = true;
                        }
                    }
                    ++teller2;
                }
            }
        }

        /// <summary>
        /// Login samenstellen op basis van voornaam en naam, en het opgegeven aantal letters van elk
        /// </summary>
        /// <param name="voornaam">De voornaam</param>
        /// <param name="familienaam">De familienaam</param>
        /// <param name="familienaamSeed">Het aantal letters dat we gebruiken van de familienaam</param>
        /// <param name="voornaamSeed">Het aantal letters dat we gebruiken van de voornaam</param>
        /// <returns>Een mogelijke gebruikersnaam, samengesteld op basis van de opgegeven parameters</returns>
        private static string LoginSuggereren(string voornaam, string familienaam, int familienaamSeed, int voornaamSeed)
        {
            String login;

            if (familienaam.ToUpper().StartsWith("DE "))
            {
                familienaam = familienaam.Remove(0, 3);
            }
            if (familienaam.ToUpper().StartsWith("VAN DER "))
            {
                familienaam = familienaam.Remove(0, 8);
            }
            if (familienaam.ToUpper().StartsWith("VAN DEN "))
            {
                familienaam = familienaam.Remove(0, 8);
            }
            if (familienaam.ToUpper().StartsWith("VAN DE "))
            {
                familienaam = familienaam.Remove(0, 7);
            }
            if (familienaam.ToUpper().StartsWith("VAN "))
            {
                familienaam = familienaam.Remove(0, 4);
            }

            // haal spaties, weglatingstekens en koppeltekens uit de naam
            voornaam = voornaam.Trim().Replace("'", string.Empty).Replace(" ", string.Empty).Replace("-", string.Empty);
            familienaam = familienaam.Trim().Replace("'", string.Empty).Replace(" ", string.Empty).Replace("-", string.Empty);

            // vervang lettertekens met accenten of een trema door andere
            voornaam = voornaam.Replace("é", "e").Replace("è", "e").Replace("à", "a").Replace("ù", "u").Replace("î", "i").Replace("ï", "i").Replace("ê", "e").Replace("ë", "e");
            familienaam = familienaam.Replace("é", "e").Replace("è", "e").Replace("à", "a").Replace("ù", "u").Replace("î", "i").Replace("ï", "i").Replace("ê", "e").Replace("ë", "e");

            // vervang andere tekens
            voornaam = voornaam.Replace("ç", "c");
            familienaam = familienaam.Replace("ç", "c");

            if (familienaam.Length > familienaamSeed)
            {
                login = string.Concat(familienaam.Substring(0, familienaamSeed), voornaam.Substring(0, voornaamSeed));
            }
            else
            {
                Int32 lengte = familienaamSeed - familienaam.Length;
                if (voornaam.Length >= voornaamSeed + lengte)
                {
                    login = string.Concat(familienaam, voornaam.Substring(0, voornaamSeed + lengte));
                }
                else
                {
                    login = string.Concat(familienaam, voornaam);
                }
            }
            return login;
        }

        /// <summary>
        /// De wijzigingen persisteren in Active Directory
        /// </summary>
        public void Opslaan()
        {
            // De bedoeling is dat deze service runt als een user die in de AD-groep 'account operators' zit voor
            // het domein waar de gebruikers worden gemaakt.
            // In dit geval: KIPDORP\LoginSvcUser die in CHIROPUBLIC\Account Operators zit

            // Als ik de service nu eens run als account met account operator rechten.  Kan ik dan niet gewoon
            // als mijzelf de changes committen?

            _gebruiker.CommitChanges();
        }

        /// <summary>
        /// Activeert de gebruiker in Active Directory
        /// </summary>
        public void Activeren()
        {
            _gebruiker.Properties["userAccountControl"].Value = 66048; // enabled
        }

        /// <summary>
        /// De account toevoegen aan de opgegeven securitygroep
        /// </summary>
        /// <param name="groep">De node in Active Directory waar de securitygroep zich bevindt</param>
        public void AanSecuritygroepToevoegen(DirectoryEntry groep)
        {
            groep.Invoke("Add", _gebruiker.Path);
            groep.CommitChanges();
        }
    }
}