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

using System;
using AutoMapper;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Mappers
{
    /// <summary>
    /// Helperfunctie voor mappings.
    /// 
    /// Niet zeker of deze manier van werken ideaal is, maar wel handig voor de
    /// migratie naar Automapper voorbij 3.3. (#5401)
    /// </summary>
    public class MappingHelper
    {
        private static readonly MapperConfiguration _configuration;

        static MappingHelper()
        {
            _configuration = new MapperConfiguration(CreateMappings);
        }

        public static void CreateMappings(IProfileExpression cfg)
        {
            cfg.CreateMap<GezinInfo, PersoonsAdresInfo>()
                .ForMember(
                    dst => dst.AdresType,
                    opt => opt.Ignore());

            cfg.CreateMap<CommunicatieDetail, CommunicatieInfo>();
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