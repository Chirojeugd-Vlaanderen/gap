/*
 * Copyright 2017 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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

using AutoMapper;
using Chiro.Gap.Api.Models;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.Api
{
    public static class MappingHelper
    {
        public static void CreateMappings()
        {
            // TODO: Damn, nog steeds automapper 3. (zie #5401).

            Mapper.CreateMap<GroepInfo, GroepModel>()
                .ForMember(dst => dst.StamNummer, opt => opt.MapFrom(src => src.StamNummer.Trim()))
                .ForMember(dst => dst.GroepId, opt => opt.MapFrom(src => src.ID));

            Mapper.CreateMap<PersoonsAdresInfo, AdresModel>()
                .ForMember(dst => dst.AdresId, opt => opt.MapFrom(src => src.ID))
                .ForMember(dst => dst.Straat, opt => opt.MapFrom(src => src.StraatNaamNaam))
                .ForMember(dst => dst.Huisnr, opt => opt.MapFrom(src => src.HuisNr))
                .ForMember(dst => dst.Postcode, opt => opt.MapFrom(src => src.IsBelgisch ? src.PostNr.ToString() : src.PostCode))
                .ForMember(dst => dst.Woonplaats, opt => opt.MapFrom(src => src.WoonPlaatsNaam))
                .ForMember(dst => dst.Land, opt => opt.MapFrom(src => src.LandNaam))
                .ForMember(dst => dst.Adrestype, opt => opt.MapFrom(src => src.AdresType));

            Mapper.CreateMap<PersoonLidInfo, PersoonModel>()
                .ForMember(dst => dst.AdNummer, opt => opt.MapFrom(src => src.PersoonDetail.AdNummer))
                .ForMember(dst => dst.PersoonId, opt => opt.MapFrom(src => src.PersoonDetail.PersoonID))
                .ForMember(dst => dst.Voornaam, opt => opt.MapFrom(src => src.PersoonDetail.VoorNaam))
                .ForMember(dst => dst.Familienaam, opt => opt.MapFrom(src => src.PersoonDetail.Naam))
                .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => src.PersoonDetail.Geslacht))
                .ForMember(dst => dst.Geboortedatum, opt => opt.MapFrom(src => src.PersoonDetail.GeboorteDatum))
                .ForMember(dst => dst.IsIngeschreven, opt => opt.MapFrom(src => src.PersoonDetail.IsLid))
                .ForMember(dst => dst.IsLeiding, opt => opt.MapFrom(src => src.PersoonDetail.IsLeiding))
                .ForMember(dst => dst.GeboortejaarCorrectie, opt => opt.MapFrom(src => src.PersoonDetail.ChiroLeefTijd))
                .ForMember(dst => dst.LidgeldBetaald, opt => opt.MapFrom(src => src.LidInfo.LidgeldBetaald))
                .ForMember(dst => dst.EindeInstapperiode, opt => opt.MapFrom(src => src.LidInfo.EindeInstapperiode))
                .ForMember(dst => dst.Adressen, opt => opt.MapFrom(src => src.PersoonsAdresInfo));
        }
    }
}