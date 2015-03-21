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
using Chiro.CiviCrm.Api.DataContracts;

namespace Chiro.CiviSync.Helpers
{
    public static class ApiResultExtensions
    {
        /// <summary>
        /// Throws an exception of the API <paramref name="result"/> is an error.
        /// </summary>
        /// <param name="result">A result of the CiviCRM API</param>
        public static void AssertValid(this ApiResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }
            if (result.IsError > 0)
            {
                throw new InvalidOperationException(result.ErrorMessage);
            }
        }
    }
}
