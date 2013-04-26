using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

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
            response.ContentType = "text/plain";

            var decryptor = new CredentialsDecryptor(Properties.Settings.Default.EncryptieKey,
                                                     Properties.Settings.Default.HashKey);

            if (encryptedUserInfo != null)
            {
                var result = decryptor.Decrypteren(encryptedUserInfo);
                response.Write(result.Naam);
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