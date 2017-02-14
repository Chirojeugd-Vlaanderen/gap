// Source: http://bitoftech.net/2014/06/01/token-based-authentication-asp-net-web-api-2-owin-asp-net-identity/

using System;
using System.Threading.Tasks;
using Chiro.Gap.Api.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Chiro.Gap.Api
{
    public class AuthRepository : IDisposable
    {
        private AuthContext _ctx;

        private UserManager<ChiroIdentityUser> _userManager;

        public AuthRepository()
        {
            _ctx = new AuthContext();
            _userManager = new UserManager<ChiroIdentityUser>(new UserStore<ChiroIdentityUser>(_ctx));
        }

        public async Task<IdentityResult> RegisterUser(UserModel userModel)
        {
            ChiroIdentityUser user = new ChiroIdentityUser
            {
                UserName = userModel.UserName,
                AdNummer = userModel.AdNummer,
            };

            var result = await _userManager.CreateAsync(user, userModel.Password);

            return result;
        }

        public async Task<ChiroIdentityUser> FindUser(string userName, string password)
        {
            ChiroIdentityUser user = await _userManager.FindAsync(userName, password);

            return user;
        }

        public async Task<ChiroIdentityUser> FindUser(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user;
        }

        public async Task DeleteUser(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();

        }
    }
}