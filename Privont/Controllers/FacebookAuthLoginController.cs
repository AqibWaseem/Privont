using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Privont.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Privont.Controllers
{
    public class FacebookAuthLoginController : Controller
    {
        // GET: FacebookAuthLogin
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Redirect to external provider for authentication
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "FacebookAuthLogin", new { ReturnUrl = returnUrl }));
        }

        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            // Process loginInfo to sign in the user or handle the user's data accordingly
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            var identity = new ClaimsIdentity(
                new[]
                {
                new Claim(ClaimTypes.NameIdentifier, loginInfo.ExternalIdentity.FindFirstValue(ClaimTypes.NameIdentifier)),
                new Claim(ClaimTypes.Email, loginInfo.Email),
                new Claim(ClaimTypes.Name, loginInfo.DefaultUserName)
                },
                DefaultAuthenticationTypes.ApplicationCookie);

            //AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);
            // Redirect user to original requested URL or another location
            return RedirectToAction("Index", "Home");
        }

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }
    }
}