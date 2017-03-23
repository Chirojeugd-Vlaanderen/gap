/*
   Copyright 2013-2015 Chirojeugd-Vlaanderen vzw

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
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
        /// Maakt het gegeven <paramref name="adres"/> het standaardadres van de gegeven <paramref name="bewoners"/>.
        /// Als het adres al bestond voor de gegeven bewoner, dan wordt het bestaande adres het standaardadres.
        /// Zo niet, wordt een nieuw adres als standaardadres gekoppeld, en het oude voorkeursadres verwijderd.
        /// </summary>
        /// <param name="adres">Nieuw standaardadres van de gegeven <paramref name="bewoners"/></param>
        /// <param name="bewoners">Bewoners die het nieuw <paramref name="adres"/> krijgen</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void StandaardAdresBewaren(Adres adres, IEnumerable<Bewoner> bewoners)
        {
            var nieuwAdres = MappingHelper.Map<Adres, AddressRequest>(adres);

            foreach (var bewoner in bewoners)
            {
                int? contactId = CiviIdGet(bewoner.Persoon, "Adres bewaren.");
                if (contactId == null) continue;

                // Adressen ophalen via chaining :-)
                var adNummer = bewoner.Persoon.AdNummer;
                var civiContact = ServiceHelper.CallService<ICiviCrmApi, Contact>(svc => svc.ContactGetSingle(
                    _apiKey,
                    _siteKey,
                    new ContactRequest
                    {
                        ExternalIdentifier = adNummer.ToString(),
                        ReturnFields = "id",
                        AddressGetRequest = new AddressRequest()
                    }));

                var adressen = civiContact.AddressResult.Values;

                // Als het adres al bestaat, dan kunnen we het overschrijven om casing te veranderen,
                // voorkeursadres te zetten, en adrestype te bewaren.

                var bestaande =
                    (from a in adressen where _adresWorker.IsHetzelfde(a, nieuwAdres) select a).FirstOrDefault();

                if (bestaande != null)
                {
                    nieuwAdres.Id = bestaande.Id;
                    // Neem het ID van het bestaande adres over, zodanig dat we het bestaande
                    // overschrijven.
                }

                nieuwAdres.ContactId = civiContact.Id;
                nieuwAdres.IsBilling = true;
                nieuwAdres.IsPrimary = true;

                switch (bewoner.AdresType)
                {
                    case AdresTypeEnum.Thuis:
                        nieuwAdres.LocationTypeId = 1;
                        break;
                    case AdresTypeEnum.Werk:
                        nieuwAdres.LocationTypeId = 2;
                        break;
                    default:
                        nieuwAdres.LocationTypeId = 4; // (other)
                        // TODO: kot (type moet uit de database gehaald worden)
                        break;
                }

                var result = ServiceHelper.CallService<ICiviCrmApi, ApiResult>(svc => svc.AddressSave(_apiKey, _siteKey, nieuwAdres));
                result.AssertValid();
                _log.Loggen(
                    Niveau.Info,
                    String.Format(
                        "Nieuw adres voor {0} {1} (gid {2} cid {3}): {4}, {5} {6} {7}. {8}",
                        bewoner.Persoon.VoorNaam, bewoner.Persoon.Naam, bewoner.Persoon.ID, civiContact.Id,
                        nieuwAdres.StreetAddress, nieuwAdres.PostalCode, nieuwAdres.PostalCodeSuffix, nieuwAdres.City,
                        nieuwAdres.Country),
                    null,
                    bewoner.Persoon.AdNummer,
                    bewoner.Persoon.ID);


                // Verwijder oude voorkeuradres.

                var teVerwijderenAdressen = (from adr in adressen
                                             where adr.IsPrimary && adr.Id != nieuwAdres.Id
                                             select adr).ToList();
                foreach (var tvAdres in teVerwijderenAdressen)
                {
                    int adresId = tvAdres.Id;
                    result = ServiceHelper.CallService<ICiviCrmApi, ApiResult>(svc => svc.AddressDelete(_apiKey, _siteKey, new DeleteRequest(adresId)));
                    result.AssertValid();
                    _log.Loggen(
                        Niveau.Info,
                        String.Format(
                            "Oud voorkeursadres voor {0} {1} verwijderd (gid {2} cid {3}): {4}, {5} {6} {7}. {8}",
                            bewoner.Persoon.VoorNaam, bewoner.Persoon.Naam, bewoner.Persoon.ID, civiContact.Id,
                            tvAdres.StreetAddress, tvAdres.PostalCode, tvAdres.PostalCodeSuffix, tvAdres.City,
                            tvAdres.Country),
                        null,
                        bewoner.Persoon.AdNummer,
                        bewoner.Persoon.ID);
                }
            }
        }
    }
}