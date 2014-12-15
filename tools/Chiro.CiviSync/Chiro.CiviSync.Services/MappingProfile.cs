using AutoMapper;
using Chiro.CiviCrm.Model;
using Chiro.Kip.ServiceContracts.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chiro.CiviSync.Services
{
    public class MappingProfile: Profile
    {
        protected override void Configure()
        {
            CreateMap<Persoon, Contact>()
                .ForMember(dst => dst.BirthDate, opt => opt.MapFrom(src => src.GeboorteDatum))
                .ForMember(dst => dst.ContactType, opt => opt.MapFrom(src => ContactType.Individual))
                .ForMember(dst => dst.DeceasedDate, opt => opt.MapFrom(src => src.SterfDatum))
                .ForMember(dst => dst.ExternalIdentifier, opt => opt.MapFrom(src => src.AdNummer))
                .ForMember(dst => dst.FirstName, opt => opt.MapFrom(src => src.VoorNaam))
                .ForMember(dst => dst.GenderId, opt => opt.MapFrom(src => (Gender)(3 - (int)src.Geslacht)))
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
                .ForMember(dst => dst.PreferredMailFormat, opt => opt.Ignore())
                .ForMember(dst => dst.MiddleName, opt => opt.Ignore())
                .ForMember(dst => dst.FormalTitle, opt => opt.Ignore())
                .ForMember(dst => dst.CommunicationStyleId, opt => opt.Ignore())
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
                .ForMember(dst => dst.PhoneTypeId, opt => opt.Ignore())
                .ForMember(dst => dst.Phone, opt => opt.Ignore())
                .ForMember(dst => dst.EmailId, opt => opt.Ignore())
                .ForMember(dst => dst.Email, opt => opt.Ignore())
                .ForMember(dst => dst.OnHold, opt => opt.Ignore())
                .ForMember(dst => dst.ImId, opt => opt.Ignore())
                .ForMember(dst => dst.ProviderId, opt => opt.Ignore())
                .ForMember(dst => dst.Im, opt => opt.Ignore())
                .ForMember(dst => dst.WorldRegion, opt => opt.Ignore())
                .ForMember(dst => dst.IndividualPrefix, opt => opt.Ignore())
                .ForMember(dst => dst.IndividualSuffix, opt => opt.Ignore())
                .ForMember(dst => dst.StateProvinceName, opt => opt.Ignore())
                .ForMember(dst => dst.Country, opt => opt.Ignore());

            CreateMap<Adres, Address>()
                            .ForMember(dst => dst.City, opt => opt.MapFrom(src => src.WoonPlaats))
                            .ForMember(dst => dst.ContactId, opt => opt.Ignore())
                            .ForMember(dst => dst.Country, opt => opt.MapFrom(src => src.LandIsoCode))
                            .ForMember(dst => dst.LocationTypeId, opt => opt.Ignore())
                            .ForMember(dst => dst.Id, opt => opt.Ignore())
                            .ForMember(dst => dst.IsBilling, opt => opt.Ignore())
                            .ForMember(dst => dst.IsPrimary, opt => opt.Ignore())
                            .ForMember(dst => dst.PostalCode, opt => opt.MapFrom(src => src.PostNr))
                            .ForMember(dst => dst.PostalCodeSuffix, opt => opt.MapFrom(src => src.PostCode))
                            .ForMember(dst => dst.StateProvinceId, opt => opt.MapFrom(src => MappingHelpers.ProvincieIDBepalen(src)))
                            .ForMember(dst => dst.StreetAddress, opt => opt.MapFrom(src => MappingHelpers.StraatNrFormatteren(src)))
                            .ForMember(dst => dst.CountryId, opt => opt.Ignore());

            Mapper.AssertConfigurationIsValid();
        }
    }
}