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
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Services.Helpers
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
                .ForMember(dst => dst.ContactType, opt => opt.MapFrom(src => ContactType.Individual))
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
                .ForMember(dst => dst.ReturnFields, opt => opt.Ignore())
                .ForMember(dst => dst.ApiOptions, opt => opt.Ignore());

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
                .ForMember(dst => dst.StateProvinceId, opt => opt.MapFrom(src => ProvincieIDBepalen(src)))
                .ForMember(dst => dst.StreetAddress, opt => opt.MapFrom(src => StraatNrFormatteren(src)))
                .ForMember(dst => dst.CountryId, opt => opt.Ignore())
                .ForMember(dst => dst.ApiOptions, opt => opt.Ignore())
                .ForMember(dst => dst.ReturnFields, opt => opt.Ignore());

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