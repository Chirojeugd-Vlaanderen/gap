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
using AutoMapper;
using Chiro.Cdf.Ioc;
using Chiro.Cdf.ServiceHelper;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic;
using Chiro.CiviSync.Workers;
using Chiro.Gap.Log;
using Chiro.Gap.UpdateApi.Client;
using Chiro.Mailchimp.Sync;
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
            Mapper.CreateMap<RelationshipRequest, Relationship>()
                .ForMember(dst => dst.ContactResult, opt => opt.Ignore());
            Mapper.CreateMap<LocBlockRequest, LocBlock>()
                .ForMember(dst => dst.AddressResult, opt => opt.Ignore())
                .ForMember(dst => dst.EventResult, opt => opt.Ignore());
            Mapper.CreateMap<MembershipRequest, Membership>()
                .ForMember(dst => dst.MembershipPaymentResult, opt => opt.Ignore());

            // Onderstaande mapping wordt in de tests gebruikt om een resultaat op
            // te leveren als er een event gecreerd wordt. We gaan er dan van uit dat
            // er geen filtering op de start- of einddatum staat.
            Mapper.CreateMap<EventRequest, Event>()
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.StartDate.Values.FirstOrDefault()))
                .ForMember(dst => dst.EndDate, opt => opt.MapFrom(src => src.EndDate.Values.FirstOrDefault()))
                .ForMember(dst => dst.LocBlockResult, opt => opt.Ignore())
                .ForMember(dst => dst.ContactResult, opt => opt.Ignore());
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
            Mapper.CreateMap<EventRequest, ApiResultValues<Event>>()
                .ForMember(dst => dst.Version, opt => opt.UseValue(3))
                .ForMember(dst => dst.Count, opt => opt.UseValue(1))
                .ForMember(dst => dst.ErrorMessage, opt => opt.UseValue(String.Empty))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.IsError, opt => opt.UseValue(0))
                .ForMember(dst => dst.Values, opt => opt.MapFrom(src => new[] {src}));
            Mapper.CreateMap<LocBlockRequest, ApiResultValues<LocBlock>>()
                .ForMember(dst => dst.Version, opt => opt.UseValue(3))
                .ForMember(dst => dst.Count, opt => opt.UseValue(1))
                .ForMember(dst => dst.ErrorMessage, opt => opt.UseValue(String.Empty))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.IsError, opt => opt.UseValue(0))
                .ForMember(dst => dst.Values, opt => opt.MapFrom(src => new[] { src }));
            Mapper.CreateMap<MembershipRequest, ApiResultValues<Membership>>()
                .ForMember(dst => dst.Version, opt => opt.UseValue(3))
                .ForMember(dst => dst.Count, opt => opt.UseValue(1))
                .ForMember(dst => dst.ErrorMessage, opt => opt.UseValue(String.Empty))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.IsError, opt => opt.UseValue(0))
                .ForMember(dst => dst.Values, opt => opt.MapFrom(src => new[] { src }));

            // Hierboven requests, hieronder entities. :-)

            Mapper.CreateMap<Contact, ApiResultValues<Contact>>()
                .ForMember(dst => dst.Version, opt => opt.UseValue(3))
                .ForMember(dst => dst.Count, opt => opt.UseValue(1))
                .ForMember(dst => dst.ErrorMessage, opt => opt.UseValue(String.Empty))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.IsError, opt => opt.UseValue(0))
                .ForMember(dst => dst.Values, opt => opt.MapFrom(src => new[] {src}));
            Mapper.CreateMap<Event, ApiResultValues<Event>>()
                .ForMember(dst => dst.Version, opt => opt.UseValue(3))
                .ForMember(dst => dst.Count, opt => opt.UseValue(1))
                .ForMember(dst => dst.ErrorMessage, opt => opt.UseValue(String.Empty))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.IsError, opt => opt.UseValue(0))
                .ForMember(dst => dst.Values, opt => opt.MapFrom(src => new[] {src}));

            Mapper.AssertConfigurationIsValid();
        }

        /// <summary>
        /// Creeert een dependency-injection container voor unit tests.
        /// 
        /// * Geen logging
        /// * Geen caching
        /// * De tests gedragen zich alsof het vandaag de gegeven datum is.
        /// * Er wordt een mock gebruikt i.p.v. de echte API's voor Civi en GAP.
        /// </summary>
        /// <param name="zogezegdeDatum">De datum die gebruikt moet worden als
        ///     zijnde de huidige datum.</param>
        /// <param name="container">De dependency injection container.</param>
        /// <param name="civiApiMock">Mock-object voor de CiviCRM-API</param>
        /// <param name="updateHelperMock">Mock-object voor UpdateApi</param>
        public static void IocOpzetten(DateTime zogezegdeDatum, out IDiContainer container, out Mock<ICiviCrmApi> civiApiMock, out Mock<IGapUpdateClient> updateHelperMock)
        {
            // Dependency injection opzetten om geen echte CiviCRM te moeten
            // aanroepen. (De binding CiviCRM-.NET heeft aparte unit tests)

            container = new UnityDiContainer();

            container.InitVolgensConfigFile();

            // We gebruiken een mock voor de CiviCrm API.
            civiApiMock = new Mock<ICiviCrmApi>();
            var channelProviderMock = new Mock<IChannelProvider>();
            channelProviderMock.Setup(src => src.GetChannel<ICiviCrmApi>()).Returns(civiApiMock.Object);
            container.InstantieRegistreren(channelProviderMock.Object);

            // Voor UpdateApi ook.
            updateHelperMock = new Mock<IGapUpdateClient>();
            container.InstantieRegistreren(updateHelperMock.Object);

            // Don't bother about mailchimp.
            var mailchimpMock = new Mock<IChimpSyncHelper>();
            container.InstantieRegistreren(mailchimpMock.Object);

            // Doe alsof het vandaag de zogezegde datum is.
            var datumProviderMock = new Mock<IDatumProvider>();
            datumProviderMock.Setup(src => src.Vandaag()).Returns(zogezegdeDatum);
            container.InstantieRegistreren(datumProviderMock.Object);

            // Loggen doen we niet.
            var logMock = new Mock<IMiniLog>();
            container.InstantieRegistreren(logMock.Object);

            // Cachen evenmin.
            var cacheMock = new Mock<ICiviCache>();
            container.InstantieRegistreren(cacheMock.Object);
        }
    }
}
