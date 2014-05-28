/*
 * Copyright 2014 Chirojeugd-Vlaanderen. See the NOTICE file at the 
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

using System.Linq;
using Chiro.Gap.Domain;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.Validatie
{
    public class LidVoorstelValidator: IValidator<LidVoorstel>
    {
        public bool Valideer(LidVoorstel teValideren)
        {
            return FoutNummer(teValideren) == null;
        }

        public Domain.FoutNummer? FoutNummer(LidVoorstel teValideren)
        {
            var gp = teValideren.GelieerdePersoon;
            var gwj = teValideren.GroepsWerkJaar;

            // Is de persoon al lid?

            bool isAlLid = gwj.Lid.Where(ld => !ld.NonActief)
                .Any(ld => ld.GelieerdePersoon.ID == gp.ID);

            if (isAlLid)
            {
                return Domain.FoutNummer.LidWasAlIngeschreven;
            }

            // We moeten kunnen bepalen hoe oud iemand is, om hem/haar ofwel in een afdeling te steken,
            // of te kijken of hij/zij oud genoeg is om leiding te zijn.

            if (!gp.GebDatumMetChiroLeefTijd.HasValue)
            {
                return Domain.FoutNummer.GeboorteDatumOntbreekt;
            }

            var geboortejaar = gp.GebDatumMetChiroLeefTijd.Value.Year;

            if (gwj.WerkJaar - geboortejaar < new LidValidator().MinimumLeeftijd)
            {
                return Domain.FoutNummer.LidTeJong;
            }

            // Geslacht is verplicht; kipadmin kan geen onzijdige mensen aan.
            if (gp.Persoon.Geslacht != GeslachtsType.Man && gp.Persoon.Geslacht != GeslachtsType.Vrouw)
            {
                return Domain.FoutNummer.OnbekendGeslacht;
            }

            if (gp.PersoonsAdres == null)
            {
                // refs #1786 - geen leden meer zonder adres.
                return Domain.FoutNummer.AdresOntbreekt;
            }

            if (!gwj.Groep.Niveau.HasFlag(Niveau.Groep) && !teValideren.LeidingMaken)
            {
                // in gewesten, verbonden: enkel leiding (geen kinderen)
                return Domain.FoutNummer.LidTypeVerkeerd;
            }

            if (teValideren.AfdelingsJarenIrrelevant) return null;

            // Als de afdelingsjaren niet irrelevant zijn, dan moeten we ook die nakijken.

            if (!teValideren.LeidingMaken && teValideren.AfdelingsJaren.Count() != 1)
            {
                // Exact 1 afdeling voor een kind.
                return Domain.FoutNummer.AfdelingKindVerplicht;
            }

            if (teValideren.AfdelingsJaren.Any(aj => !Equals(aj.GroepsWerkJaar, gwj)))
            {
                // Een afdelingsjaar dat niet hoort bij het gegeven groepswerkjaar.
                return Domain.FoutNummer.AfdelingNietBeschikbaar;
            }

            return null;
        }
    }
}
