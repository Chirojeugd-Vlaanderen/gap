/*
 * Copyright 2016 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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

using System.Web.Http;
using Chiro.Cdf.ServiceHelper;

namespace Chiro.Gap.Api.Controllers
{
    public class BaseController : ApiController
    {
        private readonly ServiceHelper _serviceHelper;
        protected ServiceHelper ServiceHelper { get { return _serviceHelper; } }

        public BaseController(ServiceHelper serviceHelper)
        {
            _serviceHelper = serviceHelper;
        }
    }
}