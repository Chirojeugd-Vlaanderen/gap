/*
   Copyright 2013 Chirojeugd-Vlaanderen vzw

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
using AutoMapper;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.Kip.ServiceContracts.DataContracts;
using Chiro.CiviCrm.Domain;

namespace Chiro.CiviSync.Services
{
    public static class MappingHelper
    {
        public static void MappingsDefinieren()
        {
            Mapper.CreateMap<Persoon, Contact>()
                .ForMember(dst => dst.BirthDate, opt => opt.MapFrom(src => src.GeboorteDatum))
                .ForMember(dst => dst.ContactType, opt => opt.MapFrom(src => ContactType.Individual))
                .ForMember(dst => dst.DeceasedDate, opt => opt.MapFrom(src => src.SterfDatum))
                .ForMember(dst => dst.ExternalId, opt => opt.MapFrom(src => src.AdNummer))
                .ForMember(dst => dst.FirstName, opt => opt.MapFrom(src => src.VoorNaam))
                .ForMember(dst => dst.Gender, opt => opt.MapFrom(src => (Gender)(3 - (int)src.Geslacht)))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.CiviID))
                .ForMember(dst => dst.IsDeceased, opt => opt.MapFrom(src => src.SterfDatum != null))
                .ForMember(dst => dst.LastName, opt => opt.MapFrom(src => src.Naam));

            Mapper.CreateMap<Adres, Address>()
                .ForMember(dst => dst.City, opt => opt.MapFrom(src => src.WoonPlaats))
                .ForMember(dst => dst.ContactId, opt => opt.Ignore())
                .ForMember(dst => dst.CountryId, opt => opt.MapFrom(src => 1020))
                // TODO: map land via ISO-code; toe te voegen aan GAP-landentabel.
                .ForMember(dst => dst.LocationTypeId, opt => opt.Ignore())
                .ForMember(dst => dst.Id, opt => opt.Ignore())
                .ForMember(dst => dst.IsBilling, opt => opt.Ignore())
                .ForMember(dst => dst.IsPrimary, opt => opt.Ignore())
                .ForMember(dst => dst.PostalCode, opt => opt.MapFrom(src => src.PostNr))
                .ForMember(dst => dst.PostalCodeSuffix, opt => opt.MapFrom(src => src.PostCode))
                .ForMember(dst => dst.StateProvinceId, opt => opt.MapFrom(src => ProvincieIDBepalen(src)))
                .ForMember(dst => dst.StreetAddress, opt => opt.MapFrom(StraatNrFormatteren));

            Mapper.AssertConfigurationIsValid();
        }

        private static string StraatNrFormatteren(Adres src)
        {
            if (!String.IsNullOrEmpty(src.Bus))
            {
                return String.Format("{0} {1} bus {2}", src.Straat, src.HuisNr, src.Bus);
            }
            return String.Format("{0} {1}", src.Straat, src.HuisNr);
        }

        private static int ProvincieIDBepalen(Adres src)
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
    }
}