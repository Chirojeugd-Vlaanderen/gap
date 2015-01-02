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
using System.Linq.Expressions;

using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. personen bevat
    /// </summary>
    public class PersonenManager : IPersonenManager
    {
        /// <summary>
        /// Verhuist een persoon van oudAdres naar nieuwAdres.  Persisteert niet.
        /// </summary>
        /// <param name="verhuizer">
        /// Te verhuizen GelieerdePersoon
        /// </param>
        /// <param name="oudAdres">
        /// Oud adres, met personen gekoppeld
        /// </param>
        /// <param name="nieuwAdres">
        /// Nieuw adres, met personen gekoppeld
        /// </param>
        /// <param name="adresType">
        /// Adrestype voor de nieuwe koppeling persoon-adres
        /// </param>
        /// <remarks>
        /// Als de persoon niet gekoppeld is aan het oude adres,
        /// zal hij of zij ook niet verhuizen
        /// </remarks>
        public void Verhuizen(Persoon verhuizer, Adres oudAdres, Adres nieuwAdres, AdresTypeEnum adresType)
        {
            Verhuizen(new[] { verhuizer }, oudAdres, nieuwAdres, adresType);
        }

        /// <summary>
        /// Verhuist een persoon van oudAdres naar nieuwAdres.  Persisteert niet.
        /// </summary>
        /// <param name="verhuizers">
        /// Te verhuizen personen
        /// </param>
        /// <param name="oudAdres">
        /// Oud adres, met personen gekoppeld
        /// </param>
        /// <param name="nieuwAdres">
        /// Nieuw adres, met personen gekoppeld
        /// </param>
        /// <param name="adresType">
        /// Adrestype voor de nieuwe koppeling persoon-adres
        /// </param>
        /// <returns>
        /// De koppelingen tussen de <paramref name="verhuizers"/> en hun <paramref name="nieuwAdres"/>.
        /// </returns>
        /// <remarks>
        /// Als de persoon niet gekoppeld is aan het oude adres,
        /// zal hij of zij ook niet verhuizen
        /// </remarks>
        public List<PersoonsAdres> Verhuizen(IList<Persoon> verhuizers, Adres oudAdres, Adres nieuwAdres, AdresTypeEnum adresType)
        {
            // Vind personen waarvan het adres al gekoppeld is.
            var bestaand =
                verhuizers.SelectMany(p => p.PersoonsAdres.Where(pa => pa.Adres.ID == nieuwAdres.ID)).ToList();

            if (bestaand.FirstOrDefault() != null)
            {
                // Geef een exception met daarin de persoonsadresobjecten die al bestaan
                throw new BlokkerendeObjectenException<PersoonsAdres>(
                    bestaand,
                    bestaand.Count(),
                    Resources.WonenDaarAl);
            }

            var persoonsAdressen =
                verhuizers.SelectMany(p => p.PersoonsAdres.Where(pa => pa.Adres.ID == oudAdres.ID)).ToList();

            foreach (var pa in persoonsAdressen)
            {
                // verwijder koppeling oud adres->persoonsadres
                pa.Adres.PersoonsAdres.Remove(pa);

                // adrestype
                pa.AdresType = adresType;

                // koppel persoonsadres aan nieuw adres
                pa.Adres = nieuwAdres;

                nieuwAdres.PersoonsAdres.Add(pa);
            }

            return persoonsAdressen;
        }
    }
}