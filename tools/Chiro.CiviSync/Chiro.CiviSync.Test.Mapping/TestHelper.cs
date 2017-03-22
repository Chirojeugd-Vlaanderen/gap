/*
   Copyright 2015, 2016, 2017 Chirojeugd-Vlaanderen vzw

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
using Moq;

namespace Chiro.CiviSync.Test.Mapping
{
    public class TestHelper
    {
        private static readonly MapperConfiguration _configuration;

        static TestHelper()
        {
            _configuration = new MapperConfiguration(MappingsCreeren);
            _configuration.AssertConfigurationIsValid();
        }

        public static void MappingsCreeren(IProfileExpression cfg)
        {
            // Een paar mappings die goed van pas komen bij het opzetten van mocks:
            cfg.CreateMap<ContactRequest, Contact>()
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
                .ForMember(dst => dst.RelationshipResult, opt => opt.Ignore())
                // Onderstaande 3 velden zijn wel gemaakt in de results, omdat
                // het intranet ze ophaalt, maar niet in de requests, omdat
                // GAP er niets mee doet. Daarom ignoren we ze in deze mappings;
                // het is toch maar om te testen.
                .ForMember(dst => dst.StopgezetOp, opt => opt.Ignore())
                .ForMember(dst => dst.Parochie, opt => opt.Ignore())
                .ForMember(dst => dst.RedenStopzetting, opt => opt.Ignore());
            cfg.CreateMap<RelationshipRequest, Relationship>()
                .ForMember(dst => dst.ContactResult, opt => opt.Ignore());
            cfg.CreateMap<LocBlockRequest, LocBlock>()
                .ForMember(dst => dst.AddressResult, opt => opt.Ignore())
                .ForMember(dst => dst.EventResult, opt => opt.Ignore());
            cfg.CreateMap<MembershipRequest, Membership>()
                .ForMember(dst => dst.MembershipPaymentResult, opt => opt.Ignore())
                .ForMember(dst => dst.ContactResult, opt => opt.Ignore());

            // Onderstaande mapping wordt in de tests gebruikt om een resultaat op
            // te leveren als er een event gecreerd wordt. We gaan er dan van uit dat
            // er geen filtering op de start- of einddatum staat.
            cfg.CreateMap<EventRequest, Event>()
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.StartDate.Values.FirstOrDefault()))
                .ForMember(dst => dst.EndDate, opt => opt.MapFrom(src => src.EndDate.Values.FirstOrDefault()))
                .ForMember(dst => dst.LocBlockResult, opt => opt.Ignore())
                .ForMember(dst => dst.ContactResult, opt => opt.Ignore())
                .ForMember(dst => dst.CourseResponsableId,
                    opt => opt.MapFrom(src => src.CourseResponsableId.Values.FirstOrDefault()));
            cfg.CreateMap<ContactRequest, ApiResultValues<Contact>>()
                .ForMember(dst => dst.Version, opt => opt.UseValue(3))
                .ForMember(dst => dst.Count, opt => opt.UseValue(1))
                .ForMember(dst => dst.ErrorMessage, opt => opt.UseValue(String.Empty))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.IsError, opt => opt.UseValue(0))
                .ForMember(dst => dst.Values, opt => opt.MapFrom(src => new[] { src }));
            cfg.CreateMap<RelationshipRequest, ApiResultValues<Relationship>>()
                .ForMember(dst => dst.Version, opt => opt.UseValue(3))
                .ForMember(dst => dst.Count, opt => opt.UseValue(1))
                .ForMember(dst => dst.ErrorMessage, opt => opt.UseValue(String.Empty))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.IsError, opt => opt.UseValue(0))
                .ForMember(dst => dst.Values, opt => opt.MapFrom(src => new[] { src }));
            cfg.CreateMap<EventRequest, ApiResultValues<Event>>()
                .ForMember(dst => dst.Version, opt => opt.UseValue(3))
                .ForMember(dst => dst.Count, opt => opt.UseValue(1))
                .ForMember(dst => dst.ErrorMessage, opt => opt.UseValue(String.Empty))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.IsError, opt => opt.UseValue(0))
                .ForMember(dst => dst.Values, opt => opt.MapFrom(src => new[] {src}));
            cfg.CreateMap<LocBlockRequest, ApiResultValues<LocBlock>>()
                .ForMember(dst => dst.Version, opt => opt.UseValue(3))
                .ForMember(dst => dst.Count, opt => opt.UseValue(1))
                .ForMember(dst => dst.ErrorMessage, opt => opt.UseValue(String.Empty))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.IsError, opt => opt.UseValue(0))
                .ForMember(dst => dst.Values, opt => opt.MapFrom(src => new[] { src }));
            cfg.CreateMap<MembershipRequest, ApiResultValues<Membership>>()
                .ForMember(dst => dst.Version, opt => opt.UseValue(3))
                .ForMember(dst => dst.Count, opt => opt.UseValue(1))
                .ForMember(dst => dst.ErrorMessage, opt => opt.UseValue(String.Empty))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.IsError, opt => opt.UseValue(0))
                .ForMember(dst => dst.Values, opt => opt.MapFrom(src => new[] { src }));

            // Hierboven requests, hieronder entities. :-)

            cfg.CreateMap<Contact, ApiResultValues<Contact>>()
                .ForMember(dst => dst.Version, opt => opt.UseValue(3))
                .ForMember(dst => dst.Count, opt => opt.UseValue(1))
                .ForMember(dst => dst.ErrorMessage, opt => opt.UseValue(String.Empty))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.IsError, opt => opt.UseValue(0))
                .ForMember(dst => dst.Values, opt => opt.MapFrom(src => new[] {src}));
            cfg.CreateMap<Event, ApiResultValues<Event>>()
                .ForMember(dst => dst.Version, opt => opt.UseValue(3))
                .ForMember(dst => dst.Count, opt => opt.UseValue(1))
                .ForMember(dst => dst.ErrorMessage, opt => opt.UseValue(String.Empty))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.IsError, opt => opt.UseValue(0))
                .ForMember(dst => dst.Values, opt => opt.MapFrom(src => new[] {src}));
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
        /// <param name="civiApiMock">Mock-object voor de CiviCRM-API</param>
        /// <param name="updateHelperMock">Mock-object voor UpdateApi</param>
        /// <returns>De dependency injection container.</returns>
        public IDiContainer IocOpzetten(DateTime zogezegdeDatum, out Mock<ICiviCrmApi> civiApiMock, out Mock<IGapUpdateClient> updateHelperMock)
        {
            // Dependency injection opzetten om geen echte CiviCRM te moeten
            // aanroepen. (De binding CiviCRM-.NET heeft aparte unit tests)

            var container = new UnityDiContainer();

            container.InitVolgensConfigFile();

            // We gebruiken een mock voor de CiviCrm API.
            civiApiMock = new Mock<ICiviCrmApi>();
            var channelProviderMock = new Mock<IChannelProvider>();
            channelProviderMock.Setup(src => src.GetChannel<ICiviCrmApi>()).Returns(civiApiMock.Object);
            container.InstantieRegistreren(channelProviderMock.Object);

            // Voor UpdateApi ook.
            updateHelperMock = new Mock<IGapUpdateClient>();
            container.InstantieRegistreren(updateHelperMock.Object);

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

            return container;
        }

        /// <summary>
        /// Map object of type <typeparamref name="T1"/> to object of type
        /// <typeparamref name="T2"/>.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="src"></param>
        /// <returns></returns>
        public T2 Map<T1, T2>(T1 src)
        {
            var mapper = _configuration.CreateMapper();
            return mapper.Map<T1, T2>(src);
        }
    }
}
