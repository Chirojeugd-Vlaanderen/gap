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
using System.Text;

namespace Chiro.Poc.ServiceGedoe
{
    /// <summary>
    /// Implementatie van IServiceClient die service methods van een WCF-service aanspreekt.  
    /// De nodige informatie over de WCF-service wordt opgehaald uit de App.Conf van de toepassing.
    /// </summary>
    /// <typeparam name="TContract">Het servicecontract van de aan te spreken service</typeparam>
    public class OnlineServiceClient<TContract> : System.ServiceModel.ClientBase<TContract>, IServiceClient<TContract> where TContract : class
    {
        /// <summary>
        /// Roep een service method aan met een <typeparamref name="TResult"/> als resultaat
        /// </summary>
        /// <typeparam name="TResult">type van het resultaat van de service call</typeparam>
        /// <param name="operatie">lambda-expressie die omschrijft welke service call aangeroepen moet worden.</param>
        /// <returns>Het resultaat van de service call</returns>
        public TResult Call<TResult>(Func<TContract,TResult> operatie)
        {
            return operatie.Invoke(Channel);
        }
    }
}
