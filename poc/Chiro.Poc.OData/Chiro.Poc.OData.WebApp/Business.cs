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

namespace Chiro.Poc.OData.WebApp
{
    public static class Business
    {
        public static List<LidDetail> AlleLedenOphalen()
        {
            return new List<LidDetail>
                       {
                           new LidDetail
                               {
                                   ID = 1,
                                   Naam = "Nielske",
                                   Afdeling = "Speelclub",
                                   GeboorteDatum = new DateTime(2003, 07, 22)
                               },
                           new LidDetail
                               {
                                   ID = 2,
                                   Naam = "Warke",
                                   Afdeling = "Sloebers",
                                   GeboorteDatum = new DateTime(2004, 07, 29)
                               },
                           new LidDetail
                               {
                                   ID = 4,
                                   Naam = "Stanneke",
                                   Afdeling = "Rakwi's",
                                   GeboorteDatum = new DateTime(2001, 04, 07)
                               }
                       };
        }
    }
}