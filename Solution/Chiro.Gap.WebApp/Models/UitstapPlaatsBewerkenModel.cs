/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
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

using System.Collections.Generic;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Model voor bewerken van de plaats van een uitstap.  (duh)
    /// </summary>
    public class UitstapPlaatsBewerkenModel : UitstapModel, IAdresBewerkenModel
    {
        public UitstapPlaatsBewerkenModel()
        {
            Uitstap = new UitstapOverzicht { Adres = new AdresInfo() };
        }

        #region Implementation of IAdresBewerkenModel

        public IEnumerable<LandInfo> AlleLanden { get; set; }

        public IEnumerable<WoonPlaatsInfo> BeschikbareWoonPlaatsen { get; set; }

        public string Land
        {
            get { return Uitstap.Adres.LandNaam; }
            set { Uitstap.Adres.LandNaam = value; }
        }

        public int PostNr
        {
            get { return Uitstap.Adres.PostNr; }
            set { Uitstap.Adres.PostNr = value; }
        }

        public string PostCode
        {
            get { return Uitstap.Adres.PostCode; }
            set { Uitstap.Adres.PostCode = value; }
        }

        public string StraatNaamNaam
        {
            get { return Uitstap.Adres.StraatNaamNaam; }
            set { Uitstap.Adres.StraatNaamNaam = value; }
        }

        public int? HuisNr
        {
            get { return Uitstap.Adres.HuisNr; }
            set { Uitstap.Adres.HuisNr = value; }
        }

        public string Bus
        {
            get { return Uitstap.Adres.Bus; }
            set { Uitstap.Adres.Bus = value; }
        }

        public string WoonPlaatsNaam
        {
            get { return Uitstap.Adres.WoonPlaatsNaam; }
            set { Uitstap.Adres.WoonPlaatsNaam = value; }
        }

        public string WoonPlaatsBuitenLand { get; set; }

        #endregion
    }
}