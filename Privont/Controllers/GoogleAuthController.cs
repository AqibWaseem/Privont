using Google.Apis.Auth;
using System.Threading.Tasks;
using System.Web.Mvc;

public class GoogleAuthController : Controller
{
    // This action will handle the Google authentication
    public async Task<ActionResult> GoogleLoginCallback(string code)
    {
        var result = await GoogleJsonWebSignature.ValidateAsync(code);

        if (result != null)
        {
            string userId = result.Subject;
            string userEmail = result.Email;
            // You can use userId or userEmail to identify/authenticate the user

            // Perform further actions (e.g., sign the user in, set cookies, etc.)

            return RedirectToAction("Index", "Home"); // Redirect to a specific page after successful login
        }
        else
        {
            // Handle authentication failure
            return RedirectToAction("Login", "Account"); // Redirect to login page or show an error message
        }
    }
    public ActionResult RedirectGoogleLogin()
    {
        var clientId = "100431486657-5brqjdjqmvc5f34aha1delvpak0hrv31.apps.googleusercontent.com";
        var redirectUrl = "https://localhost:44346/Home/Login";

        var authUrl = $"https://accounts.google.com/o/oauth2/auth?response_type=code&scope=email%20profile&redirect_uri={redirectUrl}&client_id={clientId}";

        return Redirect(authUrl);
    }
}
