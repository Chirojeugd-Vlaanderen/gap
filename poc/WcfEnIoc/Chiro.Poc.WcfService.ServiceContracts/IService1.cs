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
﻿using System.Runtime.Serialization;
using System.ServiceModel;

namespace Chiro.Poc.WcfService.ServiceContracts
{
    /// <summary>
    /// Dom servicecontract, ter illustratie
    /// </summary>
    [ServiceContract]
    public interface IService1
    {
        /// <summary>
        /// Deze method levert gewoon een hello world-achtige string op.
        /// </summary>
        /// <returns>Een hello-world string</returns>
        [OperationContract]
        string Hallo();
    }
}
