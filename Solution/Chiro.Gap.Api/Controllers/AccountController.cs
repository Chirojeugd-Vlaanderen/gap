/*
 * Original code copied from
 * http://bitoftech.net/2014/06/01/token-based-authentication-asp-net-web-api-2-owin-asp-net-identity/
 * 
 * Extra controller actions 
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

using System;
using System.Threading.Tasks;
using System.Web.Http;
using Chiro.Cdf.Authentication.Oauth;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Api.Models;
using Microsoft.AspNet.Identity;

namespace Chiro.Gap.Api.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : BaseController
    {
        private AuthRepository _repo = null;

        public AccountController(ServiceHelper serviceHelper) : base(serviceHelper)
        {
            _repo = new AuthRepository();
        }

        [Authorize]
        [Route("Hello")]
        [AcceptVerbs("GET")]
        public IHttpActionResult Hello()
        {
            var authenticator = new OauthAuthenticator();
            var ik = authenticator.WieBenIk();
            return Ok(String.Format("AD-nr {0}", ik.AdNr));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }

            base.Dispose(disposing);
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}