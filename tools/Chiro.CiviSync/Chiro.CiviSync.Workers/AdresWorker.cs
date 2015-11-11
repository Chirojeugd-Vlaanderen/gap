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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chiro.Cdf.ServiceHelper;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic;
using Chiro.Gap.Log;

namespace Chiro.CiviSync.Workers
{
    public class AdresWorker: BaseWorker
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceHelper">Helper to be used for WCF service calls</param>
        /// <param name="log">Logger</param>
        /// <param name="cache">Te gebruiken cache</param>
        public AdresWorker(ServiceHelper serviceHelper, IMiniLog log, ICiviCache cache) : base(serviceHelper, log, cache)
        {
        }

        /// <summary>
        /// Vergelijkt een <paramref name="address"/> met een <paramref name="addressRequest"/>, en geeft <c>true</c>
        /// als ze naar hetzelfde adres verwijzen.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="addressRequest"></param>
        /// <returns><c>true</c> als <paramref name="address"/> en <paramref name="addressRequest"/> naar hetzelfde
        /// adres verwijzen.</returns>
        /// <remarks>
        /// Als geen (civi-)land-ID is gegeven in CountryID, dan wordt Country geacht de iso-code van het land
        /// te zijn.
        /// </remarks>
        public bool IsHetzelfde(IAddress address, IAddress addressRequest)
        {
            if (address == null || addressRequest == null)
            {
                return false;
            }

            string land1 = LandCodeGet(address);
            string land2 = LandCodeGet(addressRequest);

            return (string.Equals(address.City, addressRequest.City, StringComparison.InvariantCultureIgnoreCase) &&
                    string.Equals(land1, land2, StringComparison.InvariantCultureIgnoreCase) &&
                    address.PostalCode == addressRequest.PostalCode &&
                    string.Equals(address.PostalCodeSuffix, addressRequest.PostalCodeSuffix,
                        StringComparison.InvariantCultureIgnoreCase) &&
                    string.Equals(address.StreetAddress, addressRequest.StreetAddress,
                        StringComparison.InvariantCultureIgnoreCase)) &&
                   string.Equals(address.Name, addressRequest.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Levert de ISO-code van het land van het gegeven adres op.
        /// </summary>
        /// <param name="address"></param>
        /// <returns>ISO-code van het land van het gegeven adres.</returns>
        /// <remarks> Als geen (civi-)land-ID is gegeven in CountryID, dan wordt Country geacht de iso-code van het land
        /// te zijn.</remarks>
        public string LandCodeGet(IAddress address)
        {
            if (address.CountryId > 0)
            {
                var result = Cache.LandCodeGet(address.CountryId);
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
                var apiResult = ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Country>>(
                    svc => svc.CountryGet(ApiKey, SiteKey,
                        new CountryRequest {Id = address.CountryId, ReturnFields = "iso_code"}));
                apiResult.AssertValid();

                if (apiResult.Count == 0)
                {
                    return null;
                }
                result = apiResult.Values.First().IsoCode;
                Cache.LandCodeSet(address.CountryId, result);
                return result;
            }

            // Geen country-ID gegeven. Doe iets met 'country'.

            return string.IsNullOrEmpty(address.Country) ? AdresLogic.StandaardLandCode : address.Country;
        }
    }
}
