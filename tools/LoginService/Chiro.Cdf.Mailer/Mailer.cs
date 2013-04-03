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
        /// <returns><c>True</c> als het bericht verstuurd is, anders <c>false</c>.</returns>
        /// <remarks>
        /// Het mailtje dat verstuurd wordt, heeft html-opmaak. De <paramref name="body"/> die je hier
        /// meegeeft, komt tussen de body-tags in het bericht terecht.
        /// </remarks>
        public bool Verzenden(string ontvanger, string onderwerp, string body)
        {
            MailServiceReference.BerichtStatus status;

            // Ik zou hier ook graag met ServiceHelper werken, maar dat kan niet direct omdat ik niet
            // direct de interface van de service heb.  Bovendien ben ik niet zeker of ServiceHelper
            // werkt als het geen WCF-services zijn.

            using (var msr = new MailServiceReference.MailServiceSoapClient())
            {
                // Om te vermijden dat de gebruiker het mailtje om zeep helpt, zetten we <pre>...</pre> rond de inhoud.
                status = msr.VerstuurGapMail(ontvanger, onderwerp, String.Format("<pre>\n{0}\n</pre>", HttpUtility.HtmlEncode(body)));
            }
            return status.IsVerstuurd;
        }
    }
}
