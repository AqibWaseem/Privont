using Privont.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Privont.Controllers
{
    public class GoogleAuthLoginController : Controller
    {
        //GoogleAuthLogin
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "GoogleAuthLogin", new { ReturnUrl = returnUrl }));
        }

        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
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

            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);

            return RedirectToLocal(returnUrl);
        }

        public ActionResult LogOut()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        public ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
       
    }
    
}