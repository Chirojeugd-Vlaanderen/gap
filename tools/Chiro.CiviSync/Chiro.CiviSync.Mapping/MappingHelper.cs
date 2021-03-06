﻿/*
   Copyright 2013,2015,2016,2017 Chirojeugd-Vlaanderen vzw

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
using Chiro.CiviCrm.Api.DataContracts.Filters;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Mapping
{
    /// <summary>
    /// Deze klasse definieert enkel nog mappings.
    /// </summary>
    public class MappingHelper
    {
        private static readonly MapperConfiguration _configuration;

        static MappingHelper()
        {
            _configuration = new MapperConfiguration(MappingsDefinieren);
            _configuration.AssertConfigurationIsValid();
        }

        public static void MappingsDefinieren(IProfileExpression cfg)
        {
            // Mappings van oude Kipadminobjecten naar ChiroCivi
            cfg.CreateMap<Persoon, ContactRequest>()
                .ForMember(dst => dst.GapId, opt => opt.MapFrom(src => src.ID))
                .ForMember(dst => dst.BirthDate, opt => opt.MapFrom(src => src.GeboorteDatum))
                .ForMember(dst => dst.ContactType, opt => opt.UseValue(ContactType.Individual))
                .ForMember(dst => dst.ContactSubType, opt => opt.UseValue(String.Empty))
                .ForMember(dst => dst.DeceasedDate, opt => opt.MapFrom(src => src.SterfDatum))
                .ForMember(dst => dst.ExternalIdentifier, opt => opt.MapFrom(src => src.AdNummer))
                .ForMember(dst => dst.FirstName, opt => opt.MapFrom(src => src.VoorNaam))
                .ForMember(dst => dst.MiddleName, opt => opt.Ignore())
                .ForMember(dst => dst.OrganizationName, opt => opt.Ignore())
                .ForMember(dst => dst.LegalName, opt => opt.Ignore())
                .ForMember(dst => dst.Gender, opt => opt.MapFrom(src => PersoonLogic.GeslachtNaarGender(src.Geslacht)))
                // IsDeceased komt niet direct mee met DeceasedDate, zie
                // http://forum.civicrm.org/index.php/topic,35553.0.html
                .ForMember(dst => dst.IsDeceased, opt => opt.MapFrom(src => src.SterfDatum != null))
                .ForMember(dst => dst.LastName, opt => opt.MapFrom(src => src.Naam))
                .ForMember(dst => dst.IsOptOut, opt => opt.MapFrom(src => !src.NieuwsBrief))
                .ForMember(dst => dst.KaderNiveau, opt => opt.UseValue((KaderNiveau?)null))
                .ForMember(dst => dst.Id, opt => opt.Ignore())
                .ForMember(dst => dst.IdValueExpression, opt => opt.Ignore())
                .ForMember(dst => dst.PreferredMailFormat, opt => opt.Ignore())
                .ForMember(dst => dst.AddressGetRequest, opt => opt.Ignore())
                .ForMember(dst => dst.AddressSaveRequest, opt => opt.Ignore())
                .ForMember(dst => dst.PhoneGetRequest, opt => opt.Ignore())
                .ForMember(dst => dst.PhoneSaveRequest, opt => opt.Ignore())
                .ForMember(dst => dst.EmailGetRequest, opt => opt.Ignore())
                .ForMember(dst => dst.EmailSaveRequest, opt => opt.Ignore())
                .ForMember(dst => dst.WebsiteGetRequest, opt => opt.Ignore())
                .ForMember(dst => dst.WebsiteSaveRequest, opt => opt.Ignore())
                .ForMember(dst => dst.ImGetRequest, opt => opt.Ignore())
                .ForMember(dst => dst.ImSaveRequest, opt => opt.Ignore())
                .ForMember(dst => dst.RelationshipGetRequest, opt => opt.Ignore())
                .ForMember(dst => dst.RelationshipSaveRequest, opt => opt.Ignore())
                .ForMember(dst => dst.MembershipGetRequest, opt => opt.Ignore())
                .ForMember(dst => dst.MembershipSaveRequest, opt => opt.Ignore())
                .ForMember(dst => dst.ReturnFields, opt => opt.Ignore())
                .ForMember(dst => dst.ApiOptions, opt => opt.Ignore());

            cfg.CreateMap<Adres, AddressRequest>()
                .ForMember(dst => dst.City, opt => opt.MapFrom(src => src.WoonPlaats))
                .ForMember(dst => dst.ContactId, opt => opt.Ignore())
                .ForMember(dst => dst.Country, opt => opt.MapFrom(src => src.LandIsoCode))
                // Default mappen we naar een thuisadres.
                .ForMember(dst => dst.LocationTypeId, opt => opt.UseValue(1))
                .ForMember(dst => dst.Id, opt => opt.Ignore())
                .ForMember(dst => dst.IsBilling, opt => opt.Ignore())
                .ForMember(dst => dst.IsPrimary, opt => opt.Ignore())
                .ForMember(dst => dst.PostalCode, opt => opt.MapFrom(src => src.PostNr))
                .ForMember(dst => dst.StateProvinceId, opt => opt.MapFrom(src => AdresLogic.ProvincieId(src)))
                .ForMember(dst => dst.StreetAddress, opt => opt.MapFrom(src => AdresLogic.StraatNrBus(src)))
                .ForMember(dst => dst.IdValueExpression, opt => opt.Ignore())
                .ForMember(dst => dst.CountryId, opt => opt.Ignore())
                .ForMember(dst => dst.Name, opt => opt.Ignore())
                .ForMember(dst => dst.LocBlockGetRequest, opt => opt.Ignore())
                .ForMember(dst => dst.LocBlockSaveRequest, opt => opt.Ignore())
                .ForMember(dst => dst.ApiOptions, opt => opt.Ignore())
                .ForMember(dst => dst.ReturnFields, opt => opt.Ignore())
                .ForMember(dst => dst.PostalCodeSuffix, opt => opt.Ignore());

            cfg.CreateMap<Bivak, EventRequest>()
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => new Filter<DateTime?>(src.DatumVan)))
                .ForMember(dst => dst.EndDate, opt => opt.MapFrom(src => new Filter<DateTime?>(src.DatumTot)))
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.Naam))
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => src.Opmerkingen))
                // Het contact-ID van de organisator moet je zelf nog berekenen!
                .ForMember(dst => dst.OrganiserendePloeg1Id, opt => opt.Ignore())
                .ForMember(dst => dst.GapUitstapId, opt => opt.MapFrom(src => src.UitstapID))
                .ForMember(dst => dst.EventTypeId, opt => opt.UseValue((int)EvenementType.Bivak))
                .ForMember(dst => dst.Id, opt => opt.Ignore())
                // Bivakplaats moet je ook zelf in orde brengen.
                .ForMember(dst => dst.LocBlockId, opt => opt.Ignore())
                .ForMember(dst => dst.ParticipantListingId, opt => opt.Ignore())
                .ForMember(dst => dst.IsPublic, opt => opt.Ignore())
                .ForMember(dst => dst.IsOnlineRegistration, opt => opt.Ignore())
                .ForMember(dst => dst.IsMonetary, opt => opt.Ignore())
                .ForMember(dst => dst.FinancialTypeId, opt => opt.Ignore())
                .ForMember(dst => dst.IsMap, opt => opt.Ignore())
                .ForMember(dst => dst.IsActive, opt => opt.Ignore())
                .ForMember(dst => dst.FeeLabel, opt => opt.Ignore())
                .ForMember(dst => dst.IsShowLocation, opt => opt.Ignore())
                .ForMember(dst => dst.DefaultRoleId, opt => opt.Ignore())
                .ForMember(dst => dst.IsEmailConfirm, opt => opt.Ignore())
                .ForMember(dst => dst.IsPayLater, opt => opt.Ignore())
                .ForMember(dst => dst.PayLaterText, opt => opt.Ignore())
                .ForMember(dst => dst.PayLaterReceipt, opt => opt.Ignore())
                .ForMember(dst => dst.IsPartialPayment, opt => opt.Ignore())
                .ForMember(dst => dst.IsMultipleRegistrations, opt => opt.Ignore())
                .ForMember(dst => dst.AllowSameParticipantEmails, opt => opt.Ignore())
                .ForMember(dst => dst.HasWaitlist, opt => opt.Ignore())
                .ForMember(dst => dst.IsTemplate, opt => opt.Ignore())
                .ForMember(dst => dst.CreatedId, opt => opt.Ignore())
                .ForMember(dst => dst.CreatedDate, opt => opt.Ignore())
                .ForMember(dst => dst.Currency, opt => opt.Ignore())
                .ForMember(dst => dst.IsShare, opt => opt.Ignore())
                .ForMember(dst => dst.IsConfirmEnabled, opt => opt.Ignore())
                .ForMember(dst => dst.ContributionTypeId, opt => opt.Ignore())
                .ForMember(dst => dst.LocBlockIdValueExpression, opt => opt.Ignore())
                .ForMember(dst => dst.LocBlockIdFilter, opt => opt.Ignore())
                .ForMember(dst => dst.KipId, opt => opt.Ignore())
                .ForMember(dst => dst.AnalytischeCode, opt => opt.Ignore())
                .ForMember(dst => dst.CourseResponsableId, opt => opt.Ignore())
                .ForMember(dst => dst.OrganiserendePloeg1Id, opt => opt.Ignore())
                .ForMember(dst => dst.OrganiserendePloeg2Id, opt => opt.Ignore())
                .ForMember(dst => dst.OrganiserendePloeg3Id, opt => opt.Ignore())
                .ForMember(dst => dst.AantalVormingsUren, opt => opt.Ignore())
                .ForMember(dst => dst.OfficieelCursusNummer, opt => opt.Ignore())
                .ForMember(dst => dst.LocBlockGetRequest, opt => opt.Ignore())
                .ForMember(dst => dst.LocBlockSaveRequest, opt => opt.Ignore())
                .ForMember(dst => dst.ContactGetRequest, opt => opt.Ignore())
                .ForMember(dst => dst.ContactSaveRequest, opt => opt.Ignore())
                .ForMember(dst => dst.IdValueExpression, opt => opt.Ignore())
                .ForMember(dst => dst.ReturnFields, opt => opt.Ignore())
                .ForMember(dst => dst.ApiOptions, opt => opt.Ignore());
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

        /// <summary>
        /// Map een object van type <typeparamref name="T1"/> naar een object
        /// van type <typeparamref name="T2"/>.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        public void Map<T1,T2>(T1 src, T2 dst)
        {
            var mapper = _configuration.CreateMapper();
            mapper.Map(src, dst);
        }
    }
}