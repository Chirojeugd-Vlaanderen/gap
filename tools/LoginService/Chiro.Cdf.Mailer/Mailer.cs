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
using System.Net.Mail;
using System.Web;

namespace Chiro.Cdf.Mailer
{
    /// <summary>
    /// Class die ervoor zorgt dat er mailtjes verstuurd kunnen worden
    /// </summary>
    public class Mailer : IMailer
    {
        /// <summary>
        /// Verstuurt een mail naar <paramref name="ontvanger"/>, met gegeven <paramref name="onderwerp"/> en
        /// <paramref name="ontvanger"/>
        /// </summary>
        /// <param name="ontvanger">E-mailadres van de geadresseerde</param>
        /// <param name="onderwerp">Onderwerp van de mail</param>
        /// <param name="body">Inhoud van de mail</param>
        public void Verzenden(string ontvanger, string onderwerp, string body)
        {
            Verzenden(Properties.Settings.Default.StandaardAfzender, ontvanger, onderwerp, body);
        }

        /// <summary>
        /// Verstuurt een mail naar <paramref name="ontvanger"/>, met gegeven <paramref name="onderwerp"/> en
        /// <paramref name="ontvanger"/>
        /// </summary>
        /// <param name="afzender">E-mailadres van de afzender.</param>
        /// <param name="ontvanger">E-mailadres van de geadresseerde</param>
        /// <param name="onderwerp">Onderwerp van de mail</param>
        /// <param name="body">Inhoud van de mail</param>
        public void Verzenden(string afzender, string ontvanger, string onderwerp, string body)
        {
            var message = new MailMessage {From = new MailAddress(afzender), Subject = onderwerp, Body = body};
            message.To.Add(new MailAddress(ontvanger));

            using (var client = new SmtpClient(Properties.Settings.Default.SmtpServer))
            {
                client.Send(message);
            }
        }
    }
}
