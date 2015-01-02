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
using System.Collections.Generic;
using System.Linq;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Businesslogica wat betreft uitstappen en bivakken
    /// </summary>
    public class UitstappenManager : IUitstappenManager
    {
        /// <summary>
        /// Bepaalt of het de tijd van het jaar is voor de bivakaangifte
        /// </summary>
        /// <param name="groepsWerkJaar">huidige groepswerkjaar</param>
        /// <returns><c>true</c> als de bivakaangifte voor <paramref name="groepsWerkJaar"/> moet worden doorgegeven, 
        /// anders <c>false</c></returns>
        public bool BivakAangifteVanBelang(GroepsWerkJaar groepsWerkJaar)
        {
            DateTime startAangifte = Settings.Default.BivakAangifteStart;
            DateTime startAangifteDitWerkjaar = new DateTime(groepsWerkJaar.WerkJaar + 1, startAangifte.Month, startAangifte.Day);

            return (DateTime.Today >= startAangifteDitWerkjaar);
        }

        /// <summary>
        /// Bepaalt de status van de gegeven <paramref name="uitstap"/>
        /// </summary>
        /// <param name="uitstap">Uitstap, waarvan status bepaald moet worden</param>
        /// <returns>De status van de gegeven <paramref name="uitstap"/></returns>
        public BivakAangifteStatus StatusBepalen(Uitstap uitstap)
        {
            var resultaat = BivakAangifteStatus.Ok;

            if (uitstap.Plaats == null)
            {
                resultaat |= BivakAangifteStatus.PlaatsOntbreekt;
            }
            if (uitstap.ContactDeelnemer == null)
            {
                resultaat |= BivakAangifteStatus.ContactOntbreekt;
            }

            return resultaat;
        }

        /// <summary>
        /// Nagaan of alle vereisten voldaan zijn om de opgegeven gelieerde personen allemaal in te schrijven
        /// voor de opgegeven uitstap.
        /// </summary>
        /// <param name="uitstap">De uitstap waar we mensen voor willen inschrijven</param>
        /// <param name="gelieerdePersonen">De mensen die we willen inschrijven</param>
        /// <exception cref="FoutNummerException"></exception>
        /// <returns><c>True</c> als alle voorwaarden voldaan zijn, anders <c>false</c></returns>
        public bool InschrijvingenValideren(Uitstap uitstap, List<GelieerdePersoon> gelieerdePersonen)
        {
            // De gelieerde personen moeten aan een groep gekoppeld zijn.
            var groepen = (from gp in gelieerdePersonen select gp.Groep).Distinct().ToList();

            if (!groepen.Any())
            {
                return false;
            }

            if (uitstap.GroepsWerkJaar == null)
            {
                return false;
            }

            if (uitstap.GroepsWerkJaar.Groep == null)
            {
                return false;
            }

            // Als er meer dan 1 groep is, dan is er minstens een groep verschillend van de groep
            // van de uitstap (duivenkotenprincipe));););
            if (groepen.Count() > 1 || groepen.First().ID != uitstap.GroepsWerkJaar.Groep.ID)
            {
                throw new FoutNummerException(
                    FoutNummer.UitstapNietVanGroep,
                    Resources.FoutieveGroepUitstap);
            }

            return true;
        }
    }
}