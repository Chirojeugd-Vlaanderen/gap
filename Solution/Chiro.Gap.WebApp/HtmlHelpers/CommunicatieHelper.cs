/*
 * Copyright 2017 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the
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
using System.Web.Mvc;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.HtmlHelpers
{
    /// <summary>
    /// HTML-helper die communicatievormen rendert.
    /// </summary>
    public static class CommunicatieHelper
    {
        /// <summary>
        /// Rendert een ID om te gebruiken in de HTML-output voor de gegeven communicatievorm.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        /// <remarks>
        /// FIXME: Deze logica staat vermoedelijk niet op zijn plaats.
        /// </remarks>
        public static string CommunicatieHtmlId(this HtmlHelper htmlHelper, CommunicatieDetail detail)
        {
            // FIXME: Ik vermoed dat we veel beter overal cv gebruiken als ID-prefix, maar ik vrees dat we dan
            // ergens jquery-functionaliteit gaan breken.
            string idPrefix;
            switch (detail.CommunicatieTypeID)
            {
                case (int) CommunicatieTypeEnum.TelefoonNummer:
                    idPrefix = "tel";
                    break;
                case (int) CommunicatieTypeEnum.Email:
                    idPrefix = "email";
                    break;
                default:
                    idPrefix = "cv";
                    break;
            }
            return String.Format("{0}{1}", idPrefix, detail.ID);
        }

        public static string Communicatie(this HtmlHelper htmlHelper, CommunicatieDetail detail)
        {
            string info;

            switch (detail.CommunicatieTypeID)
            {
                case (int) CommunicatieTypeEnum.TelefoonNummer:
                    info = htmlHelper.Telefoon(detail.Nummer);
                    break;
                case (int) CommunicatieTypeEnum.Email:
                    info = String.Format("<a href='mailto:{0}'>{0}</a>", htmlHelper.Encode(detail.Nummer));
                    break;
                default:
                    info = htmlHelper.Encode(detail.Nummer);
                    break;
            }
            // FIXME: het feit dat we hier rare ID's genereren die vermodelijk in de jquery-magie van de view gebruikt
            // worden, breekt de separation of concerns. Daar wordt best nog eens iets aan gedaan.
            string id = htmlHelper.CommunicatieHtmlId(detail);
            string tag = detail.Voorkeur ? "strong" : "span";
            string result = String.Format("<{0} id='{1}' class='contact'>{2}</{0}> ", tag, id, info);
            if (!String.IsNullOrEmpty(detail.Nota))
            {
                result += String.Format("<br />({0})", htmlHelper.Encode(detail.Nota));
            }
            return result;
        }
    }
}