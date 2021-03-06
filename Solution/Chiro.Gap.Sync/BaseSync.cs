﻿/*
 * Copyright 2008-2014 the GAP developers. See the NOTICE file at the 
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

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Sync.Mappers;

namespace Chiro.Gap.Sync
{
    /// <summary>
    /// Base synchronization class.
    /// </summary>
    public class BaseSync
    {
        protected ServiceHelper ServiceHelper { get; }
        protected static MappingHelper MappingHelper { get; }

        /// <summary>
        /// Constructor.
        /// 
        /// De ServiceHelper wordt geïnjecteerd door de dependency injection container. Wat de
        /// ServiceHelper precies zal opleveren, hangt af van welke IChannelProvider geregistreerd
        /// is bij de container.
        /// </summary>
        /// <param name="serviceHelper">ServiceHelper, nodig voor service calls.</param>
        public BaseSync(ServiceHelper serviceHelper)
        {
            ServiceHelper = serviceHelper;
        }
        static BaseSync()
        {
            // TODO: kunnen we dit niet injecteren?
            MappingHelper = new MappingHelper();
        }
    }
}
