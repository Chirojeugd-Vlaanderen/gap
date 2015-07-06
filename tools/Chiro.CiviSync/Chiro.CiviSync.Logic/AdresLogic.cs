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
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Logic
{
    /// <summary>
    /// Address logic.
    /// </summary>
    public static class AdresLogic
    {
        /// <summary>
        /// Vergelijkt een <paramref name="address"/> met een <paramref name="addressRequest"/>, en geeft <c>true</c>
        /// als ze naar hetzelfde adres verwijzen.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="addressRequest"></param>
        /// <returns><c>true</c> als <paramref name="address"/> en <paramref name="addressRequest"/> naar hetzelfde
        /// adres verwijzen.</returns>
        public static bool IsHetzelfde(Address address, AddressRequest addressRequest)
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

        /// <summary>
        /// Zet straat, nummer en bus om naar 1 string.
        /// </summary>
        /// <param name="src">Adres</param>
        /// <returns>Straat, nummer en bus in 1 string.</returns>
        public static string StraatNrBus(Adres src)
        {
            return String.IsNullOrEmpty(src.Bus)
                ? String.Format("{0} {1}", src.Straat, src.HuisNr)
                : String.Format("{0} {1} bus {2}", src.Straat, src.HuisNr, src.Bus);
        }

        /// <summary>
        /// Probeert automatisch het Civi-provincie-ID van het gegeven aders te bepalen.
        /// </summary>
        /// <param name="src">Adres</param>
        /// <returns>Het Civi-provincie-ID voor dat adres.</returns>
        public static int ProvincieId(Adres src)
        {
            if (!String.IsNullOrEmpty(src.Land) &&
                !src.Land.StartsWith("Belgi", StringComparison.InvariantCultureIgnoreCase))
            {
                // trek uw plan met provincies in het buitenland.
                return 0;
            }

            int nr = src.PostNr;

            if (nr < 1300) return 5217;    // Brussel. eigenlijk geen provincie, maar kipadmin weet dat niet
            if (nr < 1500) return 1786;    // Waals Brabant
            if (nr < 2000) return 1793;    // Vlaams Brabant
            if (nr < 3000) return 1785;    // Antwerpen
            if (nr < 3500) return 1793;    // Vlaams Brabant heeft blijkbaar 2 ranges
            if (nr < 4000) return 1789;    // Limburg
            if (nr < 5000) return 1788;    // Luik
            if (nr < 6000) return 1791;    // Namen
            if (nr < 6600) return 1787;    // Henegouwen
            if (nr < 7000) return 1790;    // Luxemburg
            if (nr < 8000) return 1787;    // Ook 2 ranges voor Henegouwen
            if (nr < 9000) return 1794;    // West-Vlaanderen
            return 1792;                   // Oost-Vlaanderen
        }

        /// <summary>
        /// Zet een GAP-<paramref name="adrestype"/> om naar een CiviCRM location type.
        /// </summary>
        /// <param name="adrestype">Adrestype van GAP.</param>
        /// <returns>CiviCRM location type.</returns>
        public static int CiviLocationTypeId(AdresTypeEnum adrestype)
        {
            switch (adrestype)
            {
                case AdresTypeEnum.Thuis:
                    return 1;
                case AdresTypeEnum.Werk:
                    return 2;
                case AdresTypeEnum.Kot:
                    return 3;
                default:
                    return 4;
            }
        }
    }
}
