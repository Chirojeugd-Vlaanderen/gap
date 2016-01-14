/*
 * Copyright 2015 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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
using Chiro.Ad.ServiceContracts;
using Chiro.Cdf.ServiceHelper;
using Chiro.Kip.ServiceContracts;
using Moq;

namespace Chiro.Gap.Services.Dev
{
    /// <summary>
    /// De DevChannelProvider vermijdt dat Kipadmin of Active Directory worden aangesproken tijdens
    /// het ontwikkelen/debuggen/testen.
    /// </summary>
    /// <remarks>Uiteraard mag deze niet gebruikt worden in de live omgeving!</remarks>
    public class DevChannelProvider: IChannelProvider
    {
        public I GetChannel<I>() where I : class
        {
            if (typeof(I) == typeof(IAdService))
            {
                // De AdServiceMock genereert pseude-gebruikersnamen.
                return new AdServiceMock() as I;
            }
            if (typeof(I) == typeof(ISyncPersoonService))
            {
                // Deze mock doet niets. Dat is ook niet nodig, want we zijn een message queue.
                return new Mock<ISyncPersoonService>().Object as I;
            }
            // We verwachten hier geen andere services. In principe zou ik die generiek kunnen
            // mocken, maar ik gooi hier een exception, opdat we zouden merken dat er iets mis is.
            throw new NotImplementedException();
        }

        public I GetChannel<I>(string instanceName) where I : class
        {
            return GetChannel<I>();
        }

        public bool TryGetChannel<I>(out I channel) where I : class
        {
            channel = GetChannel<I>();
            return (channel != null);
        }
    }
}
