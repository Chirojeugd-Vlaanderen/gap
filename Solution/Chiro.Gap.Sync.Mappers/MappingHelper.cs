/*
 * Copyright 2008-2013, 2015, 2016 the GAP developers. See the NOTICE
 * file at the top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Migration to Automapper 4 Copyright 2017 Chirojeugd-Vlaanderen vzw.
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
using System.Linq;
using AutoMapper;
using Chiro.Gap.Poco.Model;
using Chiro.Kip.ServiceContracts.DataContracts;
using Adres = Chiro.Gap.Poco.Model.Adres;
using Groep = Chiro.Gap.Poco.Model.Groep;
using Persoon = Chiro.Gap.Poco.Model.Persoon;

namespace Chiro.Gap.Sync.Mappers
{
    /// <summary>
    /// Klasse om mappings naar de datacontracts voor de SyncService te definieren.
    /// </summary>
    public class MappingHelper
    {
        private static readonly MapperConfiguration _configuration;

        static MappingHelper()
        {
            _configuration = new MapperConfiguration(CreateMappings);
            _configuration.AssertConfigurationIsValid();
        }

        /// <summary>
        /// Regelt de mappings
        /// </summary>
        public static void CreateMappings(IProfileExpression cfg)
        {
            cfg.CreateMap<Persoon, Kip.ServiceContracts.DataContracts.Persoon>(); // Members met dezelfde naam mappen automatisch

            cfg.CreateMap<Adres, Kip.ServiceContracts.DataContracts.Adres>()
                .ForMember(dst => dst.Land, opt => opt.MapFrom(src => src.LandGet()))
                .ForMember(dst => dst.LandIsoCode, opt => opt.MapFrom(src => src.LandCodeGet()))
                .ForMember(dst => dst.PostNr,
                    opt =>
                        opt.MapFrom(
                            src =>
                                src is BelgischAdres
                                    ? (src as BelgischAdres).StraatNaam.PostNummer.ToString()
                                    : src.PostCodeGet()))
                .ForMember(dst => dst.Straat,
                    opt =>
                        opt.MapFrom(
                            src =>
                                src is BelgischAdres
                                    ? (src as BelgischAdres).StraatNaam.Naam
                                    : src is BuitenLandsAdres ? (src as BuitenLandsAdres).Straat : String.Empty))
                .ForMember(dst => dst.WoonPlaats, opt => opt.MapFrom(src => WoonPlaats(src)));

            cfg.CreateMap<CommunicatieVorm, CommunicatieMiddel>()
                .ForMember(dst => dst.IsBulk, opt => opt.MapFrom(src => src.Voorkeur))
                .ForMember(dst => dst.Type, opt => opt.MapFrom(src => (Kip.ServiceContracts.DataContracts.CommunicatieType)src.CommunicatieType.ID))
                .ForMember(dst => dst.Waarde, opt => opt.MapFrom(src => src.Nummer));

            cfg.CreateMap<GelieerdePersoon, PersoonDetails>()
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

            cfg.CreateMap<Uitstap, Bivak>()
                .ForMember(dst => dst.StamNummer, opt => opt.MapFrom(src => src.GroepsWerkJaar.Groep.Code.Trim()))
                .ForMember(dst => dst.UitstapID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dst => dst.WerkJaar, opt => opt.MapFrom(src => src.GroepsWerkJaar.WerkJaar))
                .ForMember(dst => dst.Naam, opt => opt.MapFrom(src => BivakNaam(src)));

            cfg.CreateMap<Groep, Kip.ServiceContracts.DataContracts.Groep>();

        }

        private static string VoorkeursMail(GelieerdePersoon gelieerdePersoon)
        {
            string result = (from a in gelieerdePersoon.Communicatie
                where a.CommunicatieType.ID == 3
                orderby a.Voorkeur descending
                select a.Nummer).FirstOrDefault();

            return result;
        }

        private static string WoonPlaats(Adres src)
        {
            return src == null
                ? String.Empty
                : src is BelgischAdres
                    ? (src as BelgischAdres).WoonPlaats.Naam
                    : src is BuitenLandsAdres ? (src as BuitenLandsAdres).WoonPlaats : String.Empty;
        }

        private static string BivakNaam(Uitstap src)
        {
            return String.Format("Bivak {0} - {1}, {2} {3}", src.GroepsWerkJaar.Groep.Code, src.GroepsWerkJaar.Groep.Naam,
                src.Plaats == null ? String.Empty : WoonPlaats(src.Plaats.Adres), src.DatumTot.Year);
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
