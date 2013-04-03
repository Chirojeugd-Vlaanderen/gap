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
using System.ServiceModel;
using System.Text;
using Chiro.Poc.Ioc;

namespace Chiro.Poc.ServiceGedoe
{
    public static class ServiceHelper
    {
        public static TResult CallService<TContract, TResult>(Func<TContract, TResult> operatie) where TContract: class
        {
            TResult result;

            var client = Factory.Maak<IServiceClient<TContract>>();

            using (client)
            {
                result = client.Call(operatie);
            }

            return result;
        }
    }
}
