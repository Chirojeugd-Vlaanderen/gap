/*
 * Copyright 2008-2013 Chirojeugd-Vlaanderen vzw.
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

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Web.Mvc;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Model voor het toevoegen en eventueel meteen inschrijven van een persoon.
    /// </summary>
    public class NieuwePersoonModel: MasterViewModel, IAdresBewerkenModel
    {
        /// <summary>
        /// Persoonlijke gegevens van de nieuw toe te voegen persoon.
        /// </summary>
        public PersoonInfo NieuwePersoon { get; set; }
        /// <summary>
        /// Een eventuele ID als een broer zus waarvan de NIEUWE persoon gemaakt wordt gekend is.
        /// </summary>
        public int BroerzusID { get; set; }
        /// <summary>
        /// Als er bestaande personen gevonden zijn die hard op de nieuwe lijken, dan zitten
        /// hier de gegevens van de bestaande personen.
        /// </summary>
        public List<PersoonInfo> GelijkaardigePersonen { get; set; }
        /// <summary>
        /// Als Forceer <c>true</c> is, wordt er ook bewaard als GelijkaardigePersonen niet leeg is.
        /// </summary>
        public bool Forceer { get; set; }
        /// <summary>
        /// Informatie over het communicatietype 'telefoonnummer'.
        /// Wordt gebruikt voor de reguliere expressie (validatie) en het voorbeeld
        /// </summary>
        public CommunicatieTypeInfo TelefoonNummerType { get; set; }
        /// <summary>
        /// Informatie over het communicatietype 'e-mailadres'.
        /// Wordt gebruikt voor de reguliere expressie (validatie) en het voorbeeld
        /// </summary>
        public CommunicatieTypeInfo EMailType { get; set; }
        /// <summary>
        /// Telefoonnummer
        /// </summary>
        [DisplayName("Telefoon")]
        public CommunicatieInfo TelefoonNummer { get; set; }
        /// <summary>
        /// E-mailadres
        /// </summary>
        [DisplayName("E-mail")]
        public CommunicatieInfo EMail { get; set; }
        /// <summary>
        /// 'Geen' als de persoon niet ingeschreven moet worden,
        /// anders 'Kind' of 'Leiding'
        /// </summary>
        public LidType InschrijvenAls { get; set; }
        /// <summary>
        /// ID van het groepswerkjaar waarvoor de persoon ingeschreven zou kunnen worden.
        /// </summary>
        public int GroepsWerkJaarID { get; set; }
        /// <summary>
        /// De beschikbare afdelingen in huidig groepswerkjaar
        /// </summary>
        public List<AfdelingDetail> BeschikbareAfdelingen { get; set; }
        /// <summary>
        /// ID's van afdelingsjaren waarin de nieuwe persoon ingeschreven moet worden.
        /// </summary>
        public List<int> AfdelingsJaarIDs { get; set; }
        /// <summary>
        /// Type van het voorkeursadres
        /// </summary>
        public AdresTypeEnum AdresType { get; set; }

        public IEnumerable<LandInfo> AlleLanden { get; set; }
        public IEnumerable<WoonPlaatsInfo> BeschikbareWoonPlaatsen { get; set; }
        public string Land { get; set; }
        public int PostNr { get; set; }
        public string PostCode { get; set; }
        public string Straat { get; set; }
        public int? HuisNr { get; set; }
        public string Bus { get; set; }
        public string WoonPlaats { get; set; }
        public string WoonPlaatsBuitenLand { get; set; }
    }
}