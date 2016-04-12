/*
 * Copyright 2008-2016 the GAP developers. See the NOTICE file at the 
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
﻿using System;
﻿using System.Text;
using Chiro.Ad.Domain;
using Chiro.Cdf.Mailer;

namespace Chiro.Ad.Workers
{
    /// <summary>
    /// De bedoeling is dat de businesslogica voor de logins verhuist naar deze klasse.
    /// Zie ticket #1152.
    /// </summary>
    public class LoginManager
    {
        private readonly IMailer _mailer;

        /// <summary>
        /// Standaardconstructor
        /// </summary>
        /// <param name="mailer">IMailer via dewelke mails verstuurd zullen worden.</param>
        public LoginManager(IMailer mailer)
        {
            _mailer = mailer;
        }


        /// <summary>
        /// Het opgegeven wachtwoord instellen en de account enablen
        /// </summary>
        /// <param name="login">Login waarvoor wachtwoord moet worden ingesteld</param>
        /// <param name="nieuwWachtWoord">Het wachtwoord dat ingesteld moet worden. Als dat een lege
        /// string is, wordt een willekeurig wachtwoord gegenereerd.</param>
        public void Activeren(GapLogin login, string nieuwWachtWoord)
        {
            var wachtWoord = nieuwWachtWoord == string.Empty ? WachtWoordMaken() : nieuwWachtWoord;
            login.Activeren();
            login.Wachtwoord = wachtWoord;
            login.Opslaan();
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
        public void ActiverenEnMailen (GapLogin login)
        {
            string boodschap;

            // Alleen als de gebruiker niet-actief is, moet er nog een wachtwoord ingesteld worden
            if (login.IsActief == false)
            {
                string wachtWoord = WachtWoordMaken();
                Activeren(login, wachtWoord);

                boodschap = string.Format(Properties.Resources.GapAccountInfoMail, login.Naam, login.Login, wachtWoord);
            }
            else // Bestaande account
            {
                // OPM: als de account al bestond en alleen GAP-rechten had, dan klopt dat mailtje niet
                var b = new StringBuilder();
                b.Append(string.Format(Properties.Resources.AccountUitbreidingMailAanhef, login.Naam));
                b.Append(Properties.Resources.RechtenUitbreidingGap);
                b.Append(Properties.Resources.AccountMailAfsluiting);

                boodschap = b.ToString();
            }

            _mailer.Verzenden(login.Mailadres, "Je GAP-login", boodschap);
        }
    }
}
