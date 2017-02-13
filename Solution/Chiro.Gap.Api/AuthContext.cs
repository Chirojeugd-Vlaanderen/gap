// Source: http://bitoftech.net/2014/06/01/token-based-authentication-asp-net-web-api-2-owin-asp-net-identity/

using Microsoft.AspNet.Identity.EntityFramework;

namespace Chiro.Gap.Api
{
    /// <summary>
    /// Entity-Framework hocus-pocus voor gebruikers.
    /// </summary>
    public class AuthContext : IdentityDbContext<ChiroIdentityUser>
    {
        public AuthContext()
            : base("AuthContext")
        {

        }
    }
}