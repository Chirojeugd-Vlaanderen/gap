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

using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Chiro.Gap.Api.Models;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.Api.Mappers
{
    /// <summary>
    /// Helperfunctie voor mappings.
    /// 
    /// Niet zeker of deze manier van werken ideaal is, maar wel handig voor de
    /// migratie naar Automapper voorbij 3.3. (#5401)
    /// </summary>
    public static class MappingHelper
    {
        private static readonly MapperConfiguration _configuration;

        static MappingHelper()
        {
            _configuration = new MapperConfiguration(CreateMappings);
        }
        public static void CreateMappings(IProfileExpression cfg)
        {
            // TODO: Damn, nog steeds automapper 3. (zie #5401).

            cfg.CreateMap<GroepInfo, GroepModel>()
                .ForMember(dst => dst.StamNummer, opt => opt.MapFrom(src => src.StamNummer.Trim()))
                .ForMember(dst => dst.GroepId, opt => opt.MapFrom(src => src.ID));

            cfg.CreateMap<PersoonsAdresInfo, AdresModel>()
                .ForMember(dst => dst.AdresId, opt => opt.MapFrom(src => src.ID))
                .ForMember(dst => dst.Straat, opt => opt.MapFrom(src => src.StraatNaamNaam))
                .ForMember(dst => dst.Huisnr, opt => opt.MapFrom(src => src.HuisNr))
                .ForMember(dst => dst.Postcode, opt => opt.MapFrom(src => src.IsBelgisch ? src.PostNr.ToString() : src.PostCode))
                .ForMember(dst => dst.Woonplaats, opt => opt.MapFrom(src => src.WoonPlaatsNaam))
                .ForMember(dst => dst.Land, opt => opt.MapFrom(src => src.LandNaam))
                .ForMember(dst => dst.Adrestype, opt => opt.MapFrom(src => src.AdresType));
                // Voorkeursadres moet achteraf nog gefixt worden.

            cfg.CreateMap<CommunicatieDetail, ContactinfoModel>()
                .ForMember(dst => dst.Info, opt => opt.MapFrom(src => src.Nummer))
                .ForMember(dst => dst.IsVoorkeur, opt => opt.MapFrom(src => src.Voorkeur))
                .ForMember(dst => dst.Opmerking, opt => opt.MapFrom(src => src.Nota));

            cfg.CreateMap<PersoonLidInfo, PersoonModel>()
                .ForMember(dst => dst.AdNummer, opt => opt.MapFrom(src => src.PersoonDetail.AdNummer))
                .ForMember(dst => dst.PersoonId, opt => opt.MapFrom(src => src.PersoonDetail.PersoonID))
                .ForMember(dst => dst.Voornaam, opt => opt.MapFrom(src => src.PersoonDetail.VoorNaam))
                .ForMember(dst => dst.Familienaam, opt => opt.MapFrom(src => src.PersoonDetail.Naam))
                .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => src.PersoonDetail.Geslacht))
                .ForMember(dst => dst.Geboortedatum, opt => opt.MapFrom(src => src.PersoonDetail.GeboorteDatum))
                .ForMember(dst => dst.IsIngeschreven, opt => opt.MapFrom(src => src.PersoonDetail.IsLid))
                .ForMember(dst => dst.IsLeiding, opt => opt.MapFrom(src => src.PersoonDetail.IsLeiding))
                .ForMember(dst => dst.Leeftijdscorrectie, opt => opt.MapFrom(src => src.PersoonDetail.ChiroLeefTijd))
                .ForMember(dst => dst.LidgeldBetaald, opt => opt.MapFrom(src => src.LidInfo.LidgeldBetaald))
                .ForMember(dst => dst.EindeInstapperiode, opt => opt.MapFrom(src => src.LidInfo.EindeInstapperiode))
                .ForMember(dst => dst.Adressen, opt => opt.MapFrom(src => MapAddressen(src)))
                .ForMember(dst => dst.Telefoon,
                    opt => opt.MapFrom(src => MapCommunicatie(src, CommunicatieTypeEnum.TelefoonNummer)))
                .ForMember(dst => dst.Email, opt => opt.MapFrom(src => MapCommunicatie(src, CommunicatieTypeEnum.Email)))
                .ForMember(dst => dst.Afdelingen, opt => opt.MapFrom(src => src.LidInfo.AfdelingAfkortingLijst))
                .ForMember(dst => dst.Functies, opt => opt.MapFrom(src => src.LidInfo.Functies.Select(fn => fn.Code)))
                .ForMember(dst => dst.Categorieen,
                    opt => opt.MapFrom(src => src.PersoonDetail.CategorieLijst.Select(ct => ct.Code)));
        }

        private static IList<ContactinfoModel> MapCommunicatie(PersoonLidInfo src, CommunicatieTypeEnum communicatyeType)
        {
            var lijst = new List<ContactinfoModel>();
            foreach (var i in src.CommunicatieInfo.Where(ci => ci.CommunicatieTypeID == (int)communicatyeType))
            {
                var info = Mapper.Map<CommunicatieDetail, ContactinfoModel>(i);
                info.PersoonId = src.PersoonDetail.PersoonID;
                lijst.Add(info);
            }
            return lijst;
        }

        private static IList<AdresModel> MapAddressen(PersoonLidInfo src)
        {
            var lijst = new List<AdresModel>();
            foreach (var a in src.PersoonsAdresInfo)
            {
                var adres = Mapper.Map<PersoonsAdresInfo, AdresModel>(a);
                adres.IsVoorkeur = (src.PersoonDetail.VoorkeursAdresID == a.PersoonsAdresID);
                lijst.Add(adres);
            }
            return lijst;
        }
    }
}