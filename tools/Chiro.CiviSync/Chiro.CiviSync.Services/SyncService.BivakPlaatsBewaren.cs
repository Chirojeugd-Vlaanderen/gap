/*
   Copyright 2015 Chirojeugd-Vlaanderen vzw

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
using System.Linq;
using System.ServiceModel;
using AutoMapper;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic;
using Chiro.Gap.Log;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {
        /// <summary>
        /// Bewaart <paramref name="plaatsNaam"/> en <paramref name="adres"/> voor een bivak
        /// in Kipadmin.
        /// </summary>
        /// <param name="uitstapId">
        /// ID van de uitstap in GAP
        /// </param>
        /// <param name="plaatsNaam">
        /// Naam van de bivakplaats
        /// </param>
        /// <param name="adres">
        /// Adres van de bivakplaats
        /// </param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void BivakPlaatsBewaren(int uitstapId, string plaatsNaam, Adres adres)
        {
            Event bivak;
            string stamNr;

            ValideerBivak(uitstapId, out bivak, out stamNr);
            if (bivak == null)
            {
                return;
            }

            var oudLocBlock = bivak.LocBlockResult.Count > 0 ? bivak.LocBlockResult.Values.First() : null;
            Address oudAdres = null;

            if (oudLocBlock != null)
            {
                if (oudLocBlock.AddressResult.Count != 1)
                {
                    _log.Loggen(Niveau.Warning,
                        String.Format(
                            "Geen adres voor locblock {1} bij bivak van {2} met GAP-uitstapID {0}.",
                            uitstapId, oudLocBlock.Id, stamNr), stamNr, null, null);
                    oudAdres = null;
                }
                else
                {
                    oudAdres = oudLocBlock.AddressResult.Values.First();
                }
            }

            var nieuwAdres = Mapper.Map<Adres, AddressRequest>(adres);
            // Location type heeft eigenlijk weinig zin voor loc block. Maar omdat de API wil dat
            // er iets gegeven is, kies ik billing (5).
            nieuwAdres.LocationTypeId = 5;
            nieuwAdres.Name = plaatsNaam;
            if (_adresWorker.IsHetzelfde(oudAdres, nieuwAdres))
            {
                _log.Loggen(Niveau.Info,
                    String.Format("Locblock {4} niet gewijzigd voor bivak van {0}, GAP-ID {1}: {2}, {3}", stamNr,
                        uitstapId,
                        plaatsNaam, adres.WoonPlaats, bivak.LocBlockId), stamNr, null, null);
                return;
            }

            // Bestaat er al een locblock met het oude adres?
            nieuwAdres.LocBlockGetRequest = new LocBlockRequest
            {
                AddressIdValueExpression = "$value.id"
            };
            var result =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Address>>(
                    svc => svc.AddressGet(_apiKey, _siteKey, nieuwAdres));
            result.AssertValid();
            var locBlock = (from adr in result.Values
                where adr.LocBlockResult.Count >= 1
                select adr.LocBlockResult.Values.First()).FirstOrDefault();
            if (locBlock == null)
            {
                // Nee. Maak een nieuw, en koppel meteen aan dat evenement.
                var locBlockRequest = new LocBlockRequest
                {
                    Address = nieuwAdres,
                    EventSaveRequest = new[]
                    {
                        new EventRequest
                        {
                            Id = bivak.Id,
                            LocBlockIdValueExpression = "$value.id"
                        }
                    }
                };
                var locBlockResult =
                    ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<LocBlock>>(
                        svc => svc.LocBlockSave(_apiKey, _siteKey, locBlockRequest));
                locBlockResult.AssertValid();

                _log.Loggen(Niveau.Info,
                    String.Format("Nieuw locblock {4} gemaakt voor bivak van {0}, GAP-ID {1}: {2}, {3}", stamNr,
                        uitstapId,
                        plaatsNaam, adres.WoonPlaats, locBlockResult.Id), stamNr, null, null);
            }
            else
            {
                // Ja. Zet locblockId van event.
                var eventRequest = new EventRequest
                {
                    Id = bivak.Id,
                    LocBlockId = locBlock.Id
                };
                var eventResult =
                    ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Event>>(
                        svc => svc.EventSave(_apiKey, _siteKey, eventRequest));
                eventResult.AssertValid();

                _log.Loggen(Niveau.Info,
                    String.Format("Bestaand locblock {4} gebruikt voor bivak van {0}, GAP-ID {1}: {2}, {3}", stamNr,
                        uitstapId,
                        plaatsNaam, adres.WoonPlaats, locBlock.Id), stamNr, null, null);
            }
        }
    }
}