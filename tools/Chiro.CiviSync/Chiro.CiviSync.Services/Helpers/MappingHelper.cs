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
using Chiro.ChiroCivi.ServiceContracts.DataContracts;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Services.Helpers
{
    public static class MappingHelper
    {
        public static void MappingsDefinieren()
        {
            // Mappings van vanilla CiviCRM naar ChiroCivi (met custom fields)

            Mapper.CreateMap<Contact, ChiroContact>()
                .ForMember(dst => dst.GapId, opt => opt.Ignore());

            // Mappings van oude Kipadminobjecten naar ChiroCivi
            Mapper.CreateMap<Persoon, ChiroContact>()
                .ForMember(dst => dst.GapId, opt => opt.MapFrom(src => src.ID))
                .ForMember(dst => dst.BirthDate, opt => opt.MapFrom(src => src.GeboorteDatum))
                .ForMember(dst => dst.ContactType, opt => opt.MapFrom(src => ContactType.Individual))
                .ForMember(dst => dst.DeceasedDate, opt => opt.MapFrom(src => src.SterfDatum))
                .ForMember(dst => dst.ExternalIdentifier, opt => opt.MapFrom(src => src.AdNummer))
                .ForMember(dst => dst.FirstName, opt => opt.MapFrom(src => src.VoorNaam))
                .ForMember(dst => dst.Gender, opt => opt.MapFrom(src => (Gender)(3 - (int)src.Geslacht)))
                .ForMember(dst => dst.IsDeceased, opt => opt.MapFrom(src => src.SterfDatum != null))
                .ForMember(dst => dst.LastName, opt => opt.MapFrom(src => src.Naam))
                .ForMember(dst => dst.Id, opt => opt.Ignore())
                // Ondrstaande voor het gemak maar automatisch gegenereerd :-)
                .ForMember(dst => dst.ContactSubType, opt => opt.Ignore())
                .ForMember(dst => dst.SortName, opt => opt.Ignore())
                .ForMember(dst => dst.DisplayName, opt => opt.Ignore())
                .ForMember(dst => dst.DoNotEmail, opt => opt.Ignore())
                .ForMember(dst => dst.DoNotPhone, opt => opt.Ignore())
                .ForMember(dst => dst.DoNotSms, opt => opt.Ignore())
                .ForMember(dst => dst.DoNotTrade, opt => opt.Ignore())
                .ForMember(dst => dst.IsOptOut, opt => opt.Ignore())
                .ForMember(dst => dst.LegalIdentifier, opt => opt.Ignore())
                .ForMember(dst => dst.NickName, opt => opt.Ignore())
                .ForMember(dst => dst.LegalName, opt => opt.Ignore())
                .ForMember(dst => dst.ImageUrl, opt => opt.Ignore())
                .ForMember(dst => dst.PreferredLanguage, opt => opt.Ignore())
                .ForMember(dst => dst.MiddleName, opt => opt.Ignore())
                .ForMember(dst => dst.FormalTitle, opt => opt.Ignore())
                .ForMember(dst => dst.CommunicationStyle, opt => opt.Ignore())
                .ForMember(dst => dst.JobTitle, opt => opt.Ignore())
                .ForMember(dst => dst.HouseholdName, opt => opt.Ignore())
                .ForMember(dst => dst.OrganizationName, opt => opt.Ignore())
                .ForMember(dst => dst.SicCode, opt => opt.Ignore())
                .ForMember(dst => dst.ContactIsDeleted, opt => opt.Ignore())
                .ForMember(dst => dst.CurrentEmployer, opt => opt.Ignore())
                .ForMember(dst => dst.AddressId, opt => opt.Ignore())
                .ForMember(dst => dst.StreetAddress, opt => opt.Ignore())
                .ForMember(dst => dst.SupplementalAddress1, opt => opt.Ignore())
                .ForMember(dst => dst.SupplementalAddress2, opt => opt.Ignore())
                .ForMember(dst => dst.City, opt => opt.Ignore())
                .ForMember(dst => dst.PostalCodeSuffix, opt => opt.Ignore())
                .ForMember(dst => dst.PostalCode, opt => opt.Ignore())
                .ForMember(dst => dst.GeoCode1, opt => opt.Ignore())
                .ForMember(dst => dst.GeoCode2, opt => opt.Ignore())
                .ForMember(dst => dst.PhoneId, opt => opt.Ignore())
                .ForMember(dst => dst.PhoneType, opt => opt.Ignore())
                .ForMember(dst => dst.Phone, opt => opt.Ignore())
                .ForMember(dst => dst.EmailId, opt => opt.Ignore())
                .ForMember(dst => dst.Email, opt => opt.Ignore())
                .ForMember(dst => dst.OnHold, opt => opt.Ignore())
                .ForMember(dst => dst.ImId, opt => opt.Ignore())
                .ForMember(dst => dst.Provider, opt => opt.Ignore())
                .ForMember(dst => dst.Im, opt => opt.Ignore())
                .ForMember(dst => dst.WorldRegion, opt => opt.Ignore())
                .ForMember(dst => dst.IndividualPrefix, opt => opt.Ignore())
                .ForMember(dst => dst.IndividualSuffix, opt => opt.Ignore())
                .ForMember(dst => dst.StateProvinceName, opt => opt.Ignore())
                .ForMember(dst => dst.Country, opt => opt.Ignore())
                .ForMember(dst => dst.ChainedAddresses, opt => opt.Ignore())
                .ForMember(dst => dst.ApiOptions, opt => opt.Ignore())
                .ForMember(dst => dst.PreferredMailFormat, opt => opt.Ignore());

            Mapper.CreateMap<Adres, Address>()
                .ForMember(dst => dst.City, opt => opt.MapFrom(src => src.WoonPlaats))
                .ForMember(dst => dst.ContactId, opt => opt.Ignore())
                .ForMember(dst => dst.Country, opt => opt.MapFrom(src => src.LandIsoCode))
                .ForMember(dst => dst.LocationTypeId, opt => opt.Ignore())
                .ForMember(dst => dst.Id, opt => opt.Ignore())
                .ForMember(dst => dst.IsBilling, opt => opt.Ignore())
                .ForMember(dst => dst.IsPrimary, opt => opt.Ignore())
                .ForMember(dst => dst.PostalCode, opt => opt.MapFrom(src => src.PostNr))
                .ForMember(dst => dst.PostalCodeSuffix, opt => opt.MapFrom(src => src.PostCode))
                .ForMember(dst => dst.StateProvinceId, opt => opt.MapFrom(src => MappingHelper.ProvincieIDBepalen(src)))
                .ForMember(dst => dst.StreetAddress, opt => opt.MapFrom(src => MappingHelper.StraatNrFormatteren(src)))
                .ForMember(dst => dst.CountryId, opt => opt.Ignore())
                .ForMember(dst => dst.ApiOptions, opt => opt.Ignore());

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