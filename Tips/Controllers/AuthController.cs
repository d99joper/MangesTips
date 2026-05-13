using System.Web.Mvc;
using System.Web.Security;

namespace Tipset.Controllers
{
    public class AuthController : BaseController
    {
        // GET /Auth/Login
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            FormsAuthentication.SignOut(); // Clear any stale auth cookie
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password, string returnUrl)
        {
            var storedPassword = System.Configuration.ConfigurationManager.AppSettings[username];
            if (storedPassword != null && storedPassword == password)
            {
                FormsAuthentication.SetAuthCookie(username, false);
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction("Index", "Admin");
            }
            ViewBag.Error = "Inloggningen misslyckades";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // GET /Auth/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}
