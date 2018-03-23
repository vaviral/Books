using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Google;
using Microsoft.AspNet.Identity;

[assembly: OwinStartup(typeof(GoogleLogin.Startup))]

namespace GoogleLogin
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "",
                ClientSecret = ""
            });
        }
    }
}
