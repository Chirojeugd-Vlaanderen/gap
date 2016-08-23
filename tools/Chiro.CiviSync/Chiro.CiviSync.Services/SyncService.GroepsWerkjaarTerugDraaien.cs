/*
   Copyright 2016 Chirojeugd-Vlaanderen vzw

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

using System;
using System.Diagnostics;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic;
using Chiro.Gap.Log;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {

        /// <summary>
        /// Herstelt lidrelaties naar de toestand op de gegeven <paramref name="datum"/>.
        /// </summary>
        /// <param name="stamNummer">Stamnummer van ploeg waarvan lidrelaties hersteld moeten worden.</param>
        /// <param name="datum">De lidrelaties worden hersteld naar de toestand zoals ze waren op deze datum.</param>
        public void GroepsWerkjaarTerugDraaien(string stamNummer, DateTime datum)
        {
            Debug.Assert(datum > DateTime.MinValue);
            int? civiGroepId = _contactWorker.ContactIdGet(stamNummer);
            if (civiGroepId == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaande groep {0} - geen werkjaar afgesloten.", stamNummer),
                    stamNummer, null, null);
                return;
            }

            var request = new ChiroWerkjaarRequest
            {
                StamNummer = stamNummer,
                Date = datum
            };

            var result = ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                svc =>
                    svc.ChiroWerkjaarTerugdraaien(_apiKey, _siteKey, request));
            result.AssertValid();

            _log.Loggen(Niveau.Info,
                String.Format(
                    "Groep {0} terug naar situatie op {1:dd/MM/yyyy}. {2} actieve lidrelatie(s).",
                    stamNummer, datum, result.Count),
                stamNummer, null, null);

        }
    }
}