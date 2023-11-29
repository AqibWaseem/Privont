using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Facebook;
namespace Privont
{
    public class StartupConfigGoogleAuth
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/GoogleAuthLogin/Login")
            });

            //app.UseCookieAuthentication(new CookieAuthenticationOptions
            //{
            //    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
            //    LoginPath = new PathString("/FacebookAuthLogin/Login") // Customize login path
            //});

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = "100431486657-5brqjdjqmvc5f34aha1delvpak0hrv31.apps.googleusercontent.com",
                ClientSecret = "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",
                CallbackPath = new PathString(""),
                Provider = new GoogleOAuth2AuthenticationProvider
                {
                    OnAuthenticated = context =>
                    {
                        // Extract the user's email address and save it in the claim
                        context.Identity.AddClaim(new System.Security.Claims.Claim("urn:google:email", context.Identity.FindFirstValue(ClaimTypes.Email)));
                        return Task.CompletedTask;
                    }
                },
                 //Scope = { "profile", "email" }
            });
            var facebookOptions = new FacebookAuthenticationOptions
            {
                AppId = "1718192168650531",
                AppSecret = "7df78fcf56acbbb32197776ed60b0ca3",
                CallbackPath = new PathString("/FacebookAuthLogin/ExternalLoginCallback") // Customize callback path
            };
            facebookOptions.Scope.Add("email"); // Add additional permissions as needed
            facebookOptions.Scope.Add("public_profile"); // Add additional permissions as needed

            app.UseFacebookAuthentication(facebookOptions);
        }
    }
}