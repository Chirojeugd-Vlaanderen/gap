/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using Chiro.Gap.Poco.Model;
using Chiro.Kip.ServiceContracts.DataContracts;
using Adres = Chiro.Gap.Poco.Model.Adres;
using CommunicatieType = Chiro.Kip.ServiceContracts.DataContracts.CommunicatieType;
using Groep = Chiro.Gap.Poco.Model.Groep;
using Persoon = Chiro.Gap.Poco.Model.Persoon;

namespace Chiro.Gap.Sync
{
    /// <summary>
    /// Klasse om mappings naar de datacontracts voor de SyncService te definieren.
    /// </summary>
    public static class MappingHelper
    {
        private static string LandGet(this Adres adres)
        {
            if (adres is BelgischAdres)
            {
                return null;
            }
            Debug.Assert(adres is BuitenLandsAdres);
            return ((BuitenLandsAdres)adres).Land.Naam;
        }

        private static string LandCodeGet(this Adres adres)
        {
            if (adres is BelgischAdres)
            {
                return "BE";
            }
            Debug.Assert(adres is BuitenLandsAdres);
            return ((BuitenLandsAdres)adres).Land.IsoCode;
        }

        private static string PostCodeGet(this Adres adres)
        {
            if (adres is BelgischAdres)
            {
                return null;
            }
            Debug.Assert(adres is BuitenLandsAdres);
            return ((BuitenLandsAdres)adres).PostCode;
        }

        /// <summary>
        /// Regelt de mappings
        /// </summary>
        public static void MappingsDefinieren()
        {
            Mapper.CreateMap<Persoon, Kip.ServiceContracts.DataContracts.Persoon>(); // Members met dezelfde naam mappen automatisch

            Mapper.CreateMap<Adres, Kip.ServiceContracts.DataContracts.Adres>()
                .ForMember(dst => dst.Land, opt => opt.MapFrom(src => src.LandGet()))
                .ForMember(dst => dst.LandIsoCode, opt => opt.MapFrom(src => src.LandCodeGet()))
                .ForMember(dst => dst.PostCode, opt => opt.MapFrom(src => src.PostCodeGet()))
                .ForMember(dst => dst.PostNr,
                           opt =>
                           opt.MapFrom(
                            src =>
                            src is BelgischAdres
                                ? (src as BelgischAdres).StraatNaam.PostNummer
                                : src is BuitenLandsAdres ? (src as BuitenLandsAdres).PostNummer : 0))
                .ForMember(dst => dst.Straat,
                           opt =>
                           opt.MapFrom(
                            src =>
                            src is BelgischAdres
                                ? (src as BelgischAdres).StraatNaam.Naam
                                : src is BuitenLandsAdres ? (src as BuitenLandsAdres).Straat : String.Empty))
                .ForMember(dst => dst.WoonPlaats,
                           opt =>
                           opt.MapFrom(
                            src =>
                            src is BelgischAdres
                                ? (src as BelgischAdres).WoonPlaats.Naam
                                : src is BuitenLandsAdres ? (src as BuitenLandsAdres).WoonPlaats : String.Empty));

            Mapper.CreateMap<CommunicatieVorm, CommunicatieMiddel>()
                .ForMember(dst => dst.GeenMailings, opt => opt.MapFrom(src => !src.IsVoorOptIn))
                .ForMember(dst => dst.Type, opt => opt.MapFrom(src => (CommunicatieType)src.CommunicatieType.ID))
                .ForMember(dst => dst.Waarde, opt => opt.MapFrom(src => src.Nummer));

            Mapper.CreateMap<GelieerdePersoon, PersoonDetails>()
                .ForMember(dst => dst.Persoon, opt => opt.MapFrom(src => src.Persoon))
                .ForMember(dst => dst.Adres, opt => opt.MapFrom(src => src.PersoonsAdres == null ? null : src.PersoonsAdres.Adres))
                .ForMember(dst => dst.AdresType,
                           opt =>
                           opt.MapFrom(
                            src =>
                            src.PersoonsAdres == null
                                ? AdresTypeEnum.Overige
                                : (AdresTypeEnum)src.PersoonsAdres.AdresType))
                .ForMember(dst => dst.Communicatie, opt => opt.MapFrom(src => src.Communicatie));

            Mapper.CreateMap<Uitstap, Bivak>()
                .ForMember(dst => dst.StamNummer, opt => opt.MapFrom(src => src.GroepsWerkJaar.Groep.Code))
                .ForMember(dst => dst.UitstapID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dst => dst.WerkJaar, opt => opt.MapFrom(src => src.GroepsWerkJaar.WerkJaar));

            Mapper.CreateMap<Groep, Kip.ServiceContracts.DataContracts.Groep>();

            Mapper.CreateMap<Abonnement, AbonnementInfo>()
                .ForMember(dst => dst.EmailAdres, opt => opt.MapFrom(src => VoorkeursMail(src.GelieerdePersoon)))
                .ForMember(dst => dst.Naam, opt => opt.MapFrom(src => src.GelieerdePersoon.Persoon.Naam))
                .ForMember(dst => dst.VoorNaam, opt => opt.MapFrom(src => src.GelieerdePersoon.Persoon.VoorNaam))
                .ForMember(dst => dst.Adres, opt => opt.MapFrom(src => src.GelieerdePersoon.PersoonsAdres.Adres))
                .ForMember(dst => dst.AbonnementType, opt => opt.MapFrom(src => (int) src.Type))
                .ForMember(dst => dst.StamNr, opt => opt.MapFrom(src => src.GelieerdePersoon.Groep.Code));

            Mapper.AssertConfigurationIsValid();
        }

        private static string VoorkeursMail(GelieerdePersoon gelieerdePersoon)
        {
            return (from a in gelieerdePersoon.Communicatie
                where a.CommunicatieType.ID == 3
                orderby a.Voorkeur descending
                select a.Nummer).FirstOrDefault();
        }
    }
}
