/*
   Copyright 2013, 2015 Chirojeugd-Vlaanderen vzw

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
using System.Text.RegularExpressions;
using AutoMapper;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.EntityRequests;
using Chiro.CiviCrm.Api.DataContracts.Filters;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Mapping
{
    public static class MappingHelper
    {
        public static readonly Regex GsmNummerExpression = new Regex("^(0|\\+32)4[6-9]");

        public static void MappingsDefinieren()
        {
            // Mappings van oude Kipadminobjecten naar ChiroCivi
            Mapper.CreateMap<Persoon, ContactRequest>()
                .ForMember(dst => dst.GapId, opt => opt.MapFrom(src => src.ID))
                .ForMember(dst => dst.BirthDate, opt => opt.MapFrom(src => src.GeboorteDatum))
                .ForMember(dst => dst.ContactType, opt => opt.UseValue(ContactType.Individual))
                .ForMember(dst => dst.ContactSubType, opt => opt.UseValue(String.Empty))
                .ForMember(dst => dst.DeceasedDate, opt => opt.MapFrom(src => src.SterfDatum))
                .ForMember(dst => dst.ExternalIdentifier, opt => opt.MapFrom(src => src.AdNummer))
                .ForMember(dst => dst.FirstName, opt => opt.MapFrom(src => src.VoorNaam))
                .ForMember(dst => dst.MiddleName, opt => opt.Ignore())
                .ForMember(dst => dst.OrganizationName, opt => opt.Ignore())
                .ForMember(dst => dst.Gender, opt => opt.MapFrom(src => (Gender) (3 - (int) src.Geslacht)))
                // IsDeceased komt niet direct mee met DeceasedDate, zie
                // http://forum.civicrm.org/index.php/topic,35553.0.html
                .ForMember(dst => dst.IsDeceased, opt => opt.MapFrom(src => src.SterfDatum != null))
                .ForMember(dst => dst.LastName, opt => opt.MapFrom(src => src.Naam))
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

            Mapper.CreateMap<Adres, AddressRequest>()
                .ForMember(dst => dst.City, opt => opt.MapFrom(src => src.WoonPlaats))
                .ForMember(dst => dst.ContactId, opt => opt.Ignore())
                .ForMember(dst => dst.Country, opt => opt.MapFrom(src => src.LandIsoCode))
                .ForMember(dst => dst.LocationTypeId, opt => opt.Ignore())
                .ForMember(dst => dst.Id, opt => opt.Ignore())
                .ForMember(dst => dst.IsBilling, opt => opt.Ignore())
                .ForMember(dst => dst.IsPrimary, opt => opt.Ignore())
                .ForMember(dst => dst.PostalCode, opt => opt.MapFrom(src => src.PostNr))
                .ForMember(dst => dst.PostalCodeSuffix, opt => opt.MapFrom(src => src.PostCode))
                .ForMember(dst => dst.StateProvinceId, opt => opt.MapFrom(src => AdresLogic.ProvincieId(src)))
                .ForMember(dst => dst.StreetAddress, opt => opt.MapFrom(src => AdresLogic.StraatNrBus(src)))
                .ForMember(dst => dst.IdValueExpression, opt => opt.Ignore())
                .ForMember(dst => dst.CountryId, opt => opt.Ignore())
                .ForMember(dst => dst.Name, opt => opt.Ignore())
                .ForMember(dst => dst.LocBlockGetRequest, opt => opt.Ignore())
                .ForMember(dst => dst.LocBlockSaveRequest, opt => opt.Ignore())
                .ForMember(dst => dst.ApiOptions, opt => opt.Ignore())
                .ForMember(dst => dst.ReturnFields, opt => opt.Ignore());

            Mapper.CreateMap<Bivak, EventRequest>()
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
                .ForMember(dst => dst.KipId, opt => opt.Ignore())
                .ForMember(dst => dst.BronBoekhouding, opt => opt.Ignore())
                .ForMember(dst => dst.AnalytischeCode, opt => opt.Ignore())
                .ForMember(dst => dst.OrganiserendePersoon1Id, opt => opt.Ignore())
                .ForMember(dst => dst.OrganiserendePersoon2Id, opt => opt.Ignore())
                .ForMember(dst => dst.OrganiserendePersoon3Id, opt => opt.Ignore())
                .ForMember(dst => dst.OrganiserendePloeg1Id, opt => opt.Ignore())
                .ForMember(dst => dst.OrganiserendePloeg2Id, opt => opt.Ignore())
                .ForMember(dst => dst.OrganiserendePloeg3Id, opt => opt.Ignore())
                .ForMember(dst => dst.AantalVormingsUren, opt => opt.Ignore())
                .ForMember(dst => dst.OfficieelCursusNummer, opt => opt.Ignore())
                .ForMember(dst => dst.LocBlockGetRequest, opt => opt.Ignore())
                .ForMember(dst => dst.LocBlockSaveRequest, opt => opt.Ignore())
                .ForMember(dst => dst.ContactGetRequest, opt => opt.Ignore())
                .ForMember(dst => dst.ContactSaveRequest, opt => opt.Ignore())
                .ForMember(dst => dst.ReturnFields, opt => opt.Ignore())
                .ForMember(dst => dst.ApiOptions, opt => opt.Ignore());

            Mapper.AssertConfigurationIsValid();
        }

        /// <summary>
        /// Zet een lijst communicatievormen van GAP om naar een lijst entities voor CiviCRM
        /// (van de types Email, Phone, Website, Im).
        /// </summary>
        /// <param name="communicatie">Lijst communicatievormen</param>
        /// <param name="contactId">Te gebruiken contact-ID voor civi-entities. Mag <c>null</c> zijn.</param>
        /// <returns>Lijst met e-mailadressen, telefoonnummers, websites, im (als Object).</returns>
        public static Object[] CiviCommunicatie(IList<CommunicatieMiddel> communicatie, int? contactId)
        {
            var telefoonNummers = from c in communicatie
                where c.Type == CommunicatieType.TelefoonNummer || c.Type == CommunicatieType.Fax
                select new Phone
                {
                    ContactId = contactId,
                    PhoneNumber = c.Waarde,
                    PhoneType =
                        c.Type == CommunicatieType.Fax
                            ? PhoneType.Fax
                            : GsmNummerExpression.IsMatch(c.Waarde) ? PhoneType.Mobile : PhoneType.Phone
                };
            var eMailAdressen = from c in communicatie
                where c.Type == CommunicatieType.Email
                select new Email
                {
                    ContactId = contactId,
                    EmailAddress = c.Waarde,
                    IsBulkMail = !c.GeenMailings
                };
            // wat betreft websites ondersteunt GAP enkel twitter en 'iets anders'
            var websites = from c in communicatie
                where
                    c.Type == CommunicatieType.WebSite || c.Type == CommunicatieType.Twitter ||
                    c.Type == CommunicatieType.StatusNet
                select new Website
                {
                    ContactId = contactId,
                    Url = c.Type == CommunicatieType.Twitter ? c.Waarde.Replace("@", "https://twitter.com/") : c.Waarde,
                    WebsiteType = c.Type == CommunicatieType.Twitter ? WebsiteType.Twitter : WebsiteType.Main
                };
            // wat betreft IM kent GAP enkel MSN en XMPP.
            var im = from c in communicatie
                where c.Type == CommunicatieType.Msn || c.Type == CommunicatieType.Xmpp
                select new Im
                {
                    ContactId = contactId,
                    Name = c.Waarde,
                    Provider = c.Type == CommunicatieType.Msn ? Provider.Msn : Provider.Jabber
                };
            return telefoonNummers.Union<Object>(eMailAdressen).Union(websites).Union(im).ToArray();
        }

        /// <summary>
        /// Zet een communicatievorm van GAP om naar een communicatierequest voor CiviCRM
        /// (van het type Email, Phone, Website of Im).
        /// </summary>
        /// <param name="communicatie">Communicatievorm</param>
        /// <param name="contactId">Te gebruiken contact-ID voor de requests. Mag <c>null</c> zijn.</param>
        /// <returns>het gevraagde request als Object.</returns>
        public static Object CiviCommunicatie(CommunicatieMiddel communicatie, int? contactId)
        {
            return CiviCommunicatie(new[] {communicatie}, contactId).First();
        }
    }
}