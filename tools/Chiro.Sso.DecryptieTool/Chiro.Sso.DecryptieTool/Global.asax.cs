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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Xml.Serialization;

namespace Chiro.Sso.DecryptieTool
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string encryptedUserInfo = HttpContext.Current.Request.Params["user"];
            string hash = HttpContext.Current.Request.Params["hash"];

            var response = base.Response;
            response.ContentType = "application/xml";

            var decryptor = new CredentialsDecryptor(Properties.Settings.Default.EncryptieKey,
                                                     Properties.Settings.Default.HashKey);

            if (encryptedUserInfo != null)
            {
                var result = decryptor.VerifierenEnDecrypteren(encryptedUserInfo, hash);
                var serializer = new XmlSerializer(result.GetType());
                serializer.Serialize(response.OutputStream, result);
            }


            base.CompleteRequest();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}