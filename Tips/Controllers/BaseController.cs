using System.Web.Mvc;
using Tipset.Models;

namespace Tipset.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.EnableNewEntries = new SettingsRepository().GetBool("EnableNewEntries");
        }
    }
}
