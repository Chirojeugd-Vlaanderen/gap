using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiro.Cdf.Authentication.Cas
{
    public class CasAuthenticator: IAuthenticator
    {
        public UserInfo WieBenIk()
        {
            var attributen = System.Web.HttpContext.Current.Session["userAttributeTable"];
            throw new NotImplementedException();
        }
    }
}
