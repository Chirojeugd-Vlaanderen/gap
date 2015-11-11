/*
 * Copyright 2008-2015 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * Bijgewerkte authenticatie Copyright 2014 Johan Vervloet.
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
using System.Security.Permissions;
using System.ServiceModel;
using System.Text.RegularExpressions;
using Chiro.Ad.Domain;
using Chiro.Ad.ServiceContracts;
using Chiro.Ad.Workers;
using Chiro.Cdf.Mailer;

namespace Chiro.Ad.LoginService
{
    /// <summary>
    /// Class waarin de mogelijkheden van de service uitgewerkt zijn
    /// </summary>
    public class AdService : IAdService
    {
        private readonly IMailer _mailer;
        private readonly LoginManager _loginManager;

        /// <summary>
        /// Creeert een nieuwe service; gebruik <paramref name="mailer"/> om mails
        /// te versturen.
        /// </summary>
        /// <param name="loginManager">businesslogica voor logins</param>
        /// <param name="mailer">IMailer waarmee de service mails zal versturen</param>
        public AdService(LoginManager loginManager, IMailer mailer)
        {
            _mailer = mailer;
            _loginManager = loginManager;
        }

        /// <summary>
        /// Vraagt aan Active Directory om een account aan te maken met rechten om GAP
        /// te gebruiken. Als de persoon in kwestie al een account heeft, worden de 
        /// rechten uitgebreid.
        /// </summary>
        /// <param name="adNr">AD-nummer van de persoon die een login moet krijgen</param>
        /// <param name="voornaam">Voornaam van de persoon die een login moet krijgen</param>
        /// <param name="familienaam">Naam van de persoon die een login moet krijgen</param>
        /// <param name="mailadres">Mailadres van de persoon die een login moet krijgen</param>
        /// <returns>Als alles goed gaat: de loginnaam. Het kan zijn dat die al bestond - in dat geval 
        /// worden de GAP-rechten toegekend. Ofwel werd hij aangemaakt. In beide gevallen krijgt
        /// de persoon in kwestie een mailtje zodat hij of zij op de hoogte is.</returns>
        /// <example>
        ///     Dit is een voorbeeld van hoe deze service aangeroepen kan worden: 
        ///   <code>
        ///     string login;
        ///     ServiceClient client = null;
        ///     try
        ///     {
        ///         client = new ServiceClient();
        ///         login = client.GapLoginAanvragen(Int32.Parse(AdNrTextBox.Text), VoornaamTextBox.Text, NaamTextBox.Text, MailadresTextBox.Text);
        ///     }
        ///     catch
        ///     {
        ///         // Fout afhandelen
        ///     }
        ///     finally
        ///     {
        ///         if (client != null)
        ///         {
        ///             // Altijd de client afsluiten!
        ///             client.Close();
        ///         }
        ///     }
        ///   </code>
        /// </example>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        //[PrincipalPermission(SecurityAction.Demand, Name = @"KIPDORP\LoginSvcUser")]
        public string GapLoginAanvragen(int adNr, string voornaam, string familienaam, string mailadres)
        {
            // Validatie
            if (!Regex.IsMatch(mailadres, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
            {
                throw new FaultException<ArgumentException>(new ArgumentException(), "Ongeldig mailadres");
            }

            // Verwerking

            try
            {
                var gebruiker = new GapLogin(adNr, voornaam, familienaam);

                // We controleren of het mailadres hetzelfde is als hier opgegeven.
                if (gebruiker.Mailadres != string.Empty && gebruiker.Mailadres != mailadres)
                {
                    // TODO: dit verhuizen naar businesslogica

                    // Het is een ander, dus sturen we ook nog een mailtje naar het hier opgegeven adres,
                    // voor de zekerheid, met instructies om het wachtwoord van de login te kunnen aanpassen.
                    string boodschap = String.Format(Properties.Resources.VerschillendWachtwoordMail, voornaam);
                    _mailer.Verzenden(mailadres, "Je Chirologin", boodschap);
                }
                else if (gebruiker.Mailadres == string.Empty)
                {
                    // Nog geen mailadres ingevuld, dus is het een nieuwe login
                    gebruiker.Mailadres = mailadres;
                    gebruiker.Opslaan();
                    _loginManager.ActiverenEnMailen(gebruiker);
                }

                return gebruiker.Login;
            }
            catch (FormatException ex)
            {
                throw new FaultException<FormatException>(ex, ex.Message);
            }
            catch (ArgumentException ex)
            {
                throw new FaultException<ArgumentException>(ex, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                throw new FaultException<InvalidOperationException>(ex, ex.Message);
            }
        }


        public int? AdNummerOpHalen(string userName)
        {
            var gebruiker = new Chirologin(userName);
            return gebruiker.AdNr;
        }

        public string gebruikersNaamOphalen(int adNummer)
        {
            var gebruiker = new Chirologin(adNummer);
            return gebruiker.Login;
        }
    }
}