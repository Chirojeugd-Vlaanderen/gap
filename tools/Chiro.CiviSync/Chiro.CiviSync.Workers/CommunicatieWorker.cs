/*
   Copyright 2015 Chirojeugd-Vlaanderen vzw

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chiro.Cdf.ServiceHelper;

namespace Chiro.CiviSync.Workers
{
    public class CommunicatieWorker
    {
        private readonly ServiceHelper _serviceHelper;
        private string _apiKey;
        private string _siteKey;

        protected ServiceHelper ServiceHelper
        {
            get { return _serviceHelper; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceHelper">Helper to be used for WCF service calls</param>
        public CommunicatieWorker(ServiceHelper serviceHelper)
        {
            _serviceHelper = serviceHelper;
        }

        /// <summary>
        /// Configureer de keys voor API access.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="siteKey"></param>
        public void Configureren(string apiKey, string siteKey)
        {
            _apiKey = apiKey;
            _siteKey = siteKey;
        }


    }
}
