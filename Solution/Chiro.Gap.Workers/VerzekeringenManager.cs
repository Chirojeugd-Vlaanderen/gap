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
    /// Manager voor al wat met verzekeringen te maken heeft.
    /// TODO (#1045): Dit was misschien beter een 'PersoonsVerzekeringenManager' geweest?
    /// </summary>
    public class VerzekeringenManager : IVerzekeringenManager
    {
        private readonly IAutorisatieManager _autorisatieMgr;
        private readonly IGroepsWerkJarenManager _groepsWerkJaarManager;

        public VerzekeringenManager(
            IAutorisatieManager auMgr,
            IGroepsWerkJarenManager gwjMgr)
        {
            _groepsWerkJaarManager = gwjMgr;
            _autorisatieMgr = auMgr;
        }

        /// <summary>
        /// Verzekert een lid
        /// </summary>
        /// <param name="l">
        /// Te verzekeren lid, met daaraan gekoppeld alle verzekeringen
        /// </param>
        /// <param name="verz">
        /// Type van de verzekering
        /// </param>
        /// <param name="beginDatum">
        /// Begindatum van de verzekering; moet in de toekomst liggen.
        /// </param>
        /// <param name="eindDatum">
        /// Einddatum van de verzekering
        /// </param>
        /// <returns>
        /// Het gecreeerde PersoonsVerzekringsobject.
        /// </returns>
        public PersoonsVerzekering Verzekeren(Lid l, VerzekeringsType verz, DateTime beginDatum, DateTime eindDatum)
        {
            if (!_autorisatieMgr.IsGav(l))
            {
                throw new GeenGavException();
            }

            if (beginDatum > eindDatum)
            {
                throw new FoutNummerException(FoutNummer.ChronologieFout, Resources.FouteDatumVolgorde);
            }

            if (beginDatum > DateTime.Now)
            {
                throw new FoutNummerException(FoutNummer.ChronologieFout, Resources.VerzekeringInVerleden);
            }

            if (verz.TotEindeWerkJaar && eindDatum != _groepsWerkJaarManager.EindDatum(l.GroepsWerkJaar))
            {
                throw new FoutNummerException(
                    FoutNummer.ValidatieFout,
                    Resources.OngeldigeEindDatumVerzekering);
            }

            // Onderstaande controle op 'al bestaande' verzekering, gebeurt niet door de database, omdat
            // dit meer inhoudt dan een unique index/unique constraint.
            var query = from v in l.GelieerdePersoon.Persoon.PersoonsVerzekering
                        where v.VerzekeringsType.ID == verz.ID
                              && (v.Tot > beginDatum && beginDatum >= v.Van || v.Van < eindDatum && eindDatum <= v.Tot)
                        select v;

            var bestaande = query.FirstOrDefault();

            if (bestaande != null)
            {
                throw new BlokkerendeObjectenException<PersoonsVerzekering>(
                    bestaande,
                    Resources.OverlappendeVerzekering);
            }

            var pv = new PersoonsVerzekering
                         {
                             Van = beginDatum,
                             Tot = eindDatum,
                             Persoon = l.GelieerdePersoon.Persoon,
                             VerzekeringsType = verz
                         };

            l.GelieerdePersoon.Persoon.PersoonsVerzekering.Add(pv);
            verz.PersoonsVerzekering.Add(pv);

            return pv;
        }

    }
}