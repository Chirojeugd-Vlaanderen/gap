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
using System.Linq;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Manager die informatie ophaalt over gebruikersrechten van personen waar jij 
    /// gebruikersrecht op hebt.
    /// </summary>
    public class GebruikersRechtenManager : IGebruikersRechtenManager
    {
        /// <summary>
        /// Verlengt het gegeven <paramref name="gebruikersRecht"/> (indien mogelijk)
        /// </summary>
        /// <param name="gebruikersRecht">
        /// Te verlengen gebruikersrecht
        /// </param>
        public void Verlengen(GebruikersRechtV2 gebruikersRecht)
        {
            if (!gebruikersRecht.IsVerlengbaar)
            {
                // Als er gebruikersrecht is, maar dat is niet verlengbaar, dan gooien
                // we er een exception tegenaan.
                throw new FoutNummerException(FoutNummer.GebruikersRechtNietVerlengbaar,
                                              Resources.GebruikersRechtNietVerlengbaar);
            }

            gebruikersRecht.VervalDatum = NieuweVervalDatum();
        }

        /// <summary>
        /// Bepaalt een nieuwe vervaldatum voor nieuwe of te verlengen gebruikersrechten.
        /// </summary>
        /// <returns>De standaardvervaldatum voor gebruikersrechten die vandaag worden gemaakt of verlengd.</returns>
        private DateTime NieuweVervalDatum()
        {
            // Vervaldatum aanpassen. Als de toegang in de zomer verlengd wordt (vanaf overgangsperiode), 
            // gaat het waarschijnlijk al over rechten voor het komend werkjaar.

            DateTime beginOvergang = new DateTime(
                DateTime.Now.Year,
                Settings.Default.BeginOvergangsPeriode.Month,
                Settings.Default.BeginOvergangsPeriode.Day);

            int jaar = DateTime.Now >= beginOvergang ? DateTime.Now.Year + 1 : DateTime.Now.Year;

            return new DateTime(
                jaar,
                Settings.Default.EindeGebruikersRecht.Month,
                Settings.Default.EindeGebruikersRecht.Day);
        }

        /// <summary>
        /// Pas de vervaldatum van het gegeven <paramref name="gebruikersRecht"/> aan, zodanig dat
        /// het niet meer geldig is.  ZONDER TE PERSISTEREN.
        /// </summary>
        /// <param name="gebruikersRecht">
        /// Te vervallen gebruikersrecht
        /// </param>
        public void Intrekken(GebruikersRechtV2 gebruikersRecht)
        {
            Intrekken(new[] { gebruikersRecht });
        }

        /// <summary>
        /// Pas de vervaldatum van het de <paramref name="gebruikersRechten"/> aan, zodanig dat
        /// ze niet meer geldig zijn.  ZONDER TE PERSISTEREN.
        /// </summary>
        /// <param name="gebruikersRechten">
        /// Te vervallen gebruikersrechten
        /// </param>
        /// <remarks>Gebruikersrechten die al vervallen zijn, blijven onaangeroerd</remarks>
        public void Intrekken(GebruikersRechtV2[] gebruikersRechten)
        {
            foreach (var gr in gebruikersRechten.Where(r => r.VervalDatum > DateTime.Now))
            {
                gr.VervalDatum = DateTime.Now.AddDays(-1);
            }
        }

        /// <summary>
        /// Levert het gebruikersrecht op dat een <paramref name="gelieerdePersoon"/> heeft op zijn eigen groep. 
        /// If any.  Als <paramref name="gelieerdePersoon"/> geen gebruikersrechten heeft op zijn groep, wordt 
        /// <c>null</c> opgeleverd.
        /// </summary>
        /// <param name="gelieerdePersoon">Een gelieerde persoon</param>
        /// <returns>
        /// Het gebruikersrecht op dat een <paramref name="gelieerdePersoon"/> heeft op zijn eigen groep. If any. 
        /// Als <paramref name="gelieerdePersoon"/> geen gebruikersrechten heeft op zijn groep, wordt <c>null</c> 
        /// opgeleverd.
        /// </returns>
        public GebruikersRechtV2 GebruikersRechtGet(GelieerdePersoon gelieerdePersoon)
        {
            return
                gelieerdePersoon.Groep.GebruikersRechtV2.FirstOrDefault(
                    gr => gr.Persoon.ID == gelieerdePersoon.Persoon.ID);
        }

        /// <summary>
        /// Zoekt het gebruikersrecht op van <paramref name="persoon"/> op <paramref name="groep"/>. Als dat nog niet
        /// bestaat, maak er een aan. Voeg de gevraagde permissies toe. De vervaldatum wordt vervangen door de
        /// standaardvervaldatum op dit moment.
        /// </summary>
        /// <param name="persoon">Persoon die gebruikersrechten moet krijgen.</param>
        /// <param name="groep">Groep waarvoor de persoon gebruikersrechten moet krijgen.</param>
        /// <param name="persoonlijkeGegevens">Permissies op persoonlijke gegevens.</param>
        /// <param name="groepsGegevens">Permissies op de gegevens van de groep.</param>
        /// <param name="personenInAfdeling">Permissies op de leden in de eigen afdeling.</param>
        /// <param name="personenInGroep">Permissies op alle personen van de eigen groep.</param>
        public void ToekennenOfWijzigen(Persoon persoon, Groep groep, Permissies persoonlijkeGegevens, Permissies groepsGegevens,
            Permissies personenInAfdeling, Permissies personenInGroep)
        {
            var gebruikersRecht = persoon.GebruikersRechtV2.FirstOrDefault(gr => gr.Groep.Equals(groep)) ??
                                  new GebruikersRechtV2 {Groep = groep, Persoon = persoon};
            gebruikersRecht.PersoonlijkeGegevens |= persoonlijkeGegevens;
            gebruikersRecht.GroepsGegevens |= groepsGegevens;
            gebruikersRecht.PersonenInAfdeling |= personenInAfdeling;
            gebruikersRecht.PersonenInGroep |= personenInGroep;
            gebruikersRecht.VervalDatum = NieuweVervalDatum();
        }
    }
}
