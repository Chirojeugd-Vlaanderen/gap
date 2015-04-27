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

using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Log;

namespace Chiro.CiviSync.Workers
{
    public class BaseWorker
    {
        protected string ApiKey { get; private set; }
        protected string SiteKey { get; private set; }

        private readonly ServiceHelper _serviceHelper;
        private readonly IMiniLog _log;

        protected IMiniLog Log
        {
            get { return _log; }
        }

        protected ServiceHelper ServiceHelper
        {
            get { return _serviceHelper; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceHelper">Helper to be used for WCF service calls</param>
        /// <param name="log">Logger</param>
        public BaseWorker(ServiceHelper serviceHelper, IMiniLog log)
        {
            _serviceHelper = serviceHelper;
            _log = log;            
        }

        /// <summary>
        /// Configureer de keys voor API access.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="siteKey"></param>
        public void Configureren(string apiKey, string siteKey)
        {
            ApiKey = apiKey;
            SiteKey = siteKey;
        }
    }
}
