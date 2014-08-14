/*
 * Copyright 2013 Ben Bridts.
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
using System.Linq;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WebApi.Models
{
    public class AdresModel
    {
        public int Id { get; set; }
        public string Straat { get; set; }
        public int? Nr { get; set; }
        public string Bus { get; set; }
        public string Postcode { get; set; }
        public string Gemeente { get; set; }
        public string Land { get; set; }
        public bool InBelgie { get; set; }
        public string Versie { get; set; }

        public IQueryable<PersoonModel> Personen { get; set; }

        public AdresModel(Adres adres)
        {
            Id = adres.ID;
            Nr = adres.HuisNr;
            Bus = adres.Bus;

            Versie = BitConverter.ToString(adres.Versie);

            var belgischAdres = adres as BelgischAdres;
            var buitenLandsAdres = adres as BuitenLandsAdres;

            if (belgischAdres != null)
            {
                InBelgie = true;
                Straat = belgischAdres.StraatNaam.Naam;
                Postcode = belgischAdres.WoonPlaats.PostNummer.ToString();
                Gemeente = belgischAdres.WoonPlaats.Naam;
                // TaalID zou eigenlijk een Enum moeten zijn
                switch (belgischAdres.WoonPlaats.TaalID)
                {
                    case 1:
                        Land = "België";
                        break;
                    case 2:
                        Land = "Belgique";
                        break;
                    case 3:
                        Land = "Belgien";
                        break;
                    default:
                        Land = "Belgium";
                        break;
                }
            }
            else if (buitenLandsAdres != null)
            {
                InBelgie = false;
                Straat = buitenLandsAdres.Straat;
                Postcode = buitenLandsAdres.PostCode + " " + buitenLandsAdres.PostNummer.ToString();
                Gemeente = buitenLandsAdres.WoonPlaats;
                Land = buitenLandsAdres.Land.Naam;
            }
        }
    }
}