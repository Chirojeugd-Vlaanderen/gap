﻿/*
 * Copyright 2017 the GAP developers. See the NOTICE file at the 
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

using System.Linq;
using System.ServiceModel;

namespace Chiro.Cdf.Authentication.Backend
{
    /// <summary>
    /// Authenticatie voor de WCF-backend.
    /// 
    /// Het AD-nummer wordt opgehaald uit de informatie in Chiro.Cdf.AdnrWcfExtension.
    /// </summary>
    public class BackendAuthenticator: IAuthenticator
    {
        public UserInfo WieBenIk()
        {
            var userInfo = OperationContext.Current.IncomingMessageProperties.FirstOrDefault(f => f.Key == "UserInfo").Value as UserInfo;

            if (userInfo != null)
            {
                return userInfo;
            }

            // Onder windows kom je hier normaal gesproken nooit.

            // Maar met Linux en Mono krijg ik de servicebehaviour met de IDispatchMessageInspector (nog?) niet aan de
            // praat. Dus voorlopig probeer ik de UserInfo nog eens rechtstreeks uit de http-request te halen,
            // maar op termijn moet dit zeker weg.

            var headers = OperationContext.Current.IncomingMessageHeaders;

            try
            {
                userInfo = headers.GetHeader<UserInfo>("adnr-header", "s");
            }
            catch (MessageHeaderException)
            {
                return null;
            }

            return userInfo;
        }
    }
}
