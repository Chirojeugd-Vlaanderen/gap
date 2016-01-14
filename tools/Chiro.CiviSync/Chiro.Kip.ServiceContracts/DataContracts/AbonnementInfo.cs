/*
 * Copyright 2015 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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
using System.Runtime.Serialization;

namespace Chiro.Kip.ServiceContracts.DataContracts
{
    /// <summary>
    /// Alle informatie voor Dubbelpunt in Mailchimp.
    /// </summary>
    [DataContract]
    public class AbonnementInfo
    {
        [DataMember]
        public int GapPersoonId { get; set; }
        [DataMember]
        public string EmailAdres { get; set; }
        [DataMember]
        public string VoorNaam { get; set; }
        [DataMember]
        public string Naam { get; set; }
        [DataMember]
        public string StamNr { get; set; }
        [DataMember]
        public Adres Adres { get; set; }
        /// <summary>
        /// 0: geen, 1: e-mail, 2: papier.
        /// </summary>
        [DataMember]
        public int AbonnementType { get; set; }

        /// <summary>
        /// E-mailadres voor mailchimp.
        /// </summary>
        /// <remarks>
        /// Mailchimp verwacht altijd een e-mailadres. Als er geen gegeven is, dan genereren we
        /// er een dummy op basis van het GAP-PersoonID.
        /// </remarks>
        public string MailChimpAdres
        {
            get { return string.IsNullOrEmpty(EmailAdres) ? string.Format("g{0}@chiro.be", GapPersoonId) : EmailAdres; }
        }
    }
}
