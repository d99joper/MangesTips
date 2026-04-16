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
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password, string returnUrl)
        {
            if (FormsAuthentication.Authenticate(username, password))
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
