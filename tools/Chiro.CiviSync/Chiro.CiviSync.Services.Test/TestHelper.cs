﻿/*
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
using AutoMapper;
using Chiro.Cdf.Ioc;
using Chiro.Cdf.ServiceHelper;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Helpers;
using Chiro.Gap.Log;
using Moq;

namespace Chiro.CiviSync.Services.Test
{
    public static class TestHelper
    {
        public static void MappingsCreeren()
        {
            // Een paar mappings die goed van pas komen bij het opzetten van mocks:
            Mapper.CreateMap<ContactRequest, Contact>()
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
                .ForMember(dst => dst.PreferredCommunicationMethod, opt => opt.Ignore())
                .ForMember(dst => dst.PreferredLanguage, opt => opt.Ignore())
                .ForMember(dst => dst.FormalTitle, opt => opt.Ignore())
                .ForMember(dst => dst.CommunicationStyle, opt => opt.Ignore())
                .ForMember(dst => dst.JobTitle, opt => opt.Ignore())
                .ForMember(dst => dst.HouseholdName, opt => opt.Ignore())
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
                .ForMember(dst => dst.AddressResult, opt => opt.Ignore())
                .ForMember(dst => dst.PhoneResult, opt => opt.Ignore())
                .ForMember(dst => dst.EmailResult, opt => opt.Ignore())
                .ForMember(dst => dst.WebsiteResult, opt => opt.Ignore())
                .ForMember(dst => dst.ImResult, opt => opt.Ignore())
                .ForMember(dst => dst.MembershipResult, opt => opt.Ignore())
                .ForMember(dst => dst.RelationshipResult, opt => opt.Ignore());
            Mapper.CreateMap<RelationshipRequest, Relationship>();
            Mapper.CreateMap<ContactRequest, ApiResultValues<Contact>>()
                .ForMember(dst => dst.Version, opt => opt.UseValue(3))
                .ForMember(dst => dst.Count, opt => opt.UseValue(1))
                .ForMember(dst => dst.ErrorMessage, opt => opt.UseValue(String.Empty))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.IsError, opt => opt.UseValue(0))
                .ForMember(dst => dst.Values, opt => opt.MapFrom(src => new[] { src }));
            Mapper.CreateMap<RelationshipRequest, ApiResultValues<Relationship>>()
                .ForMember(dst => dst.Version, opt => opt.UseValue(3))
                .ForMember(dst => dst.Count, opt => opt.UseValue(1))
                .ForMember(dst => dst.ErrorMessage, opt => opt.UseValue(String.Empty))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.IsError, opt => opt.UseValue(0))
                .ForMember(dst => dst.Values, opt => opt.MapFrom(src => new[] { src }));            
        }

        /// <summary>
        /// Zet wat standaarddingen op voor dependency injection:
        /// 
        /// * Geen logging
        /// * De tests gedragen zich alsof het vandaag de gegeven datum is.
        /// * Er wordt een mock gebruikt i.p.v. de echte CiviCRM-API
        /// </summary>
        /// <param name="zogezegdeDatum">De datum die gebruikt moet worden als
        /// zijnde de huidige datum.</param>
        /// <returns>De gegenereerde mock, die je nog kunt configureren.</returns>
        public static Mock<ICiviCrmApi> IocOpzetten(DateTime zogezegdeDatum)
        {
            // Dependency injection opzetten om geen echte CiviCRM te moeten
            // aanroepen. (De binding CiviCRM-.NET heeft aparte unit tests)

            Factory.ContainerInit();

            // We gebruiken een mock voor de CiviCrm API.
            var civiApiMock = new Mock<ICiviCrmApi>();
            var channelProviderMock = new Mock<IChannelProvider>();
            channelProviderMock.Setup(src => src.GetChannel<ICiviCrmApi>()).Returns(civiApiMock.Object);
            Factory.InstantieRegistreren(channelProviderMock.Object);

            // Doe alsof het vandaag de zogezegde datum is.
            var datumHelperMock = new Mock<IDatumHelper>();
            datumHelperMock.Setup(src => src.Vandaag()).Returns(zogezegdeDatum);
            Factory.InstantieRegistreren(datumHelperMock.Object);

            // Loggen doen we niet.
            var logMock = new Mock<IMiniLog>();
            Factory.InstantieRegistreren(logMock.Object);

            return civiApiMock;
        }

        /// <summary>
        /// Zet wat standaarddingen op voor dependency injection:
        /// 
        /// * Geen logging
        /// * Er wordt een mock gebruikt i.p.v. de echte CiviCRM-API
        /// </summary>
        /// <returns>De gegenereerde mock, die je nog kunt configureren.</returns>
        internal static Mock<ICiviCrmApi> IocOpzetten()
        {
            return IocOpzetten(DateTime.Now);
        }
    }
}