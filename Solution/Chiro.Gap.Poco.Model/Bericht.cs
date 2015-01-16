/*
 * Copyright 2014 the GAP developers. See the NOTICE file at the 
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
using Chiro.Cdf.Poco;

namespace Chiro.Gap.Poco.Model
{
    public class Bericht: BasisEntiteit
    {
        public DateTime Tijd { get; set; }
        public int Niveau { get; set; }
        public string Boodschap { get; set; }
        public string StamNummer { get; set; }
        public int? AdNummer { get; set; }
        public virtual Persoon Persoon { get; set; }
        public virtual Persoon Gebruiker { get; set; }

        public override byte[] Versie { get; set; }
        public override int ID { get; set; }
    }
}
