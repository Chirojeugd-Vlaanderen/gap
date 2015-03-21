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
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;

namespace Chiro.CiviSync.Helpers
{
    public class AdresHelper
    {
        /// <summary>
        /// Vergelijkt een <paramref name="address"/> met een <paramref name="addressRequest"/>, en geeft <c>true</c>
        /// als ze naar hetzelfde adres verwijzen.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="addressRequest"></param>
        /// <returns><c>true</c> als <paramref name="address"/> en <paramref name="addressRequest"/> naar hetzelfde
        /// adres verwijzen.</returns>
        public bool IsHetzelfde(Address address, AddressRequest addressRequest)
        {
            if (address == null || addressRequest == null)
            {
                return false;
            }
            return (String.Equals(address.City, addressRequest.City, StringComparison.InvariantCultureIgnoreCase) &&
                    String.Equals(address.Country, addressRequest.Country, StringComparison.InvariantCultureIgnoreCase) &&
                    address.PostalCode == addressRequest.PostalCode &&
                    String.Equals(address.PostalCodeSuffix, addressRequest.PostalCodeSuffix,
                        StringComparison.InvariantCultureIgnoreCase) &&
                    address.StateProvinceId == addressRequest.StateProvinceId &&
                    String.Equals(address.StreetAddress, addressRequest.StreetAddress,
                        StringComparison.InvariantCultureIgnoreCase)) &&
                   String.Equals(address.Name, addressRequest.Name, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
