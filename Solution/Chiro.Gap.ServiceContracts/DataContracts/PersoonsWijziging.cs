/*
 * Copyright 2014, Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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

using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// Datacontract voor wijzigingen aan een persoon. Velden die niet ingevuld zijn, worden als ongewijzigd beschouwd.
    /// </summary>
    [DataContract]
    public class PersoonsWijziging
    {
        /// <summary>
        /// De ID van de gelieerde persoon
        /// </summary>
        [DataMember]
        [Verplicht]
        public int GelieerdePersoonID { get; set; }

        /// <summary>
        /// Het aantal jaren dat de persoon in leeftijd afwijkt van zijn/haar Chirogeneratiegenoten
        /// </summary>
        [DataMember]
        [DisplayName(@"Chiroleeftijd")]
        [Range(-8, 3, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RangeError")]
        [DisplayFormat(DataFormatString = "{0:#0;-#0;#0}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        public int? ChiroLeefTijd { get; set; }

        /// <summary>
        /// De voornaam van de persoon
        /// </summary>
        [DisplayName(@"Voornaam")]
        [Verplicht]
        [StringLength(60, MinimumLength = 2)]
        [DataMember]
        public string VoorNaam { get; set; }

        /// <summary>
        /// De familienaam van de persoon
        /// </summary>
        [StringLength(160, MinimumLength = 1)]
        [Verplicht]
        [DisplayName(@"Familienaam")]
        [DataMember]
        public string Naam { get; set; }

        /// <summary>
        /// De geboortedatum van de persoon
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayName(@"Geboortedatum")]
        [DatumInVerleden]
        [DataMember]
        public DateTime? GeboorteDatum { get; set; }

        /// <summary>
        /// De datum waarop de persoon overleden is
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayName(@"Sterfdatum")]
        [DatumInVerleden]
        [DataMember]
        public DateTime? SterfDatum { get; set; }

        /// <summary>
        /// Enumwaarde voor het geslacht van de persoon
        /// </summary>
        [DataMember]
        public GeslachtsType? Geslacht
        {
            get;
            set;
        }

        /// <summary>
        /// VersieString van de Persoon.
        /// (Die van de gelieerde persoon nemen we niet mee, aangezien bij het wijzigen van een
        /// gelieerde persoon de persoon nagenoeg altijd mee zal wijzigen.  Tenzij enkel de
        /// Chiroleeftijd wordt aangepast natuurlijk.  We zullen zien hoe erg dat is.)
        /// </summary>
        [DataMember]
        [Verplicht]
        public string VersieString { get; set; }
    }
}
