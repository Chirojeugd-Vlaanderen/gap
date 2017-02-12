using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Chiro.Gap.Api
{
    /// <summary>
    /// Entity-Framework hocus-pocus voor gebruikers.
    /// 
    /// De bedoeling is dat we dit verschuiven naar de GAP-backend.
    /// </summary>
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        public AuthContext()
            : base("AuthContext")
        {

        }
    }
}