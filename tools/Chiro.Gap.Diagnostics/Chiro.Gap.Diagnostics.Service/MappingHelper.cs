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
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using AutoMapper;

using Chiro.Gap.Diagnostics.ServiceContracts.DataContracts;
using Chiro.Gap.Orm;

namespace Chiro.Gap.Diagnostics.Service
{
    /// <summary>
    /// Statische klasse die gebruikt wordt om automapper te configureren.
    /// </summary>
    public static class MappingHelper
    {
        /// <summary>
        /// Definieert de mappings waarvoor we automapper zullen gebruiken
        /// </summary>
        public static void MappingsDefinieren()
        {
            // Mappings voor diagnostic datacontracts

            Mapper.CreateMap<Groep, GroepContactInfo>()
                .ForMember(dst => dst.Plaats, opt => opt.MapFrom(src => src is ChiroGroep
                                                                             ? (src as ChiroGroep).Plaats
                                                                             : String.Empty))
                .ForMember(dst => dst.Contacten, opt => opt.Ignore());

            Mapper.AssertConfigurationIsValid();

            // Mappings voor kipsync

            Chiro.Gap.Sync.MappingHelper.MappingsDefinieren();
        }
    }
}