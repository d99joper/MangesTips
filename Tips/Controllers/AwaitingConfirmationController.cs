using Microsoft.AspNetCore.Mvc;
using Tipset.Models;

namespace Tipset.Controllers
{
    public class AwaitingConfirmationController : BaseController
    {
        public AwaitingConfirmationController(SettingsRepository settingsRepo) : base(settingsRepo) { }

        public ActionResult Index()
        {
            return View();
        }
    }
}
