// Source: http://bitoftech.net/2014/06/01/token-based-authentication-asp-net-web-api-2-owin-asp-net-identity/
using System;
using System.Web.Http;
using Chiro.Gap.Api;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        HttpConfiguration config = new HttpConfiguration();

        ConfigureOAuth(app);
        UnityConfig.RegisterComponents(config);
        WebApiConfig.Register(config);
        app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
        app.UseWebApi(config);
    }

    public void ConfigureOAuth(IAppBuilder app)
    {
        OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
        {
            AllowInsecureHttp = true,
            TokenEndpointPath = new PathString("/token"),
            AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
            Provider = new SimpleAuthorizationServerProvider()
        };

        // Token Generation
        app.UseOAuthAuthorizationServer(OAuthServerOptions);
        app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

    }
}
