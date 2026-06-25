using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Tipset.Models;

namespace Tipset.Controllers
{
    public class BaseController : Controller
    {
        protected readonly SettingsRepository _settingsRepo;

        public BaseController(SettingsRepository settingsRepo)
        {
            _settingsRepo = settingsRepo;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.EnableNewEntries = _settingsRepo.GetBool("EnableNewEntries");
        }
    }
}
