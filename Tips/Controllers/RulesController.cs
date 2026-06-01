using Microsoft.AspNetCore.Mvc;
using Tipset.Models;

namespace Tipset.Controllers
{
    public class RulesController : BaseController
    {
        public RulesController(SettingsRepository settingsRepo) : base(settingsRepo) { }

        public ActionResult Index()
        {
            return View();
        }
    }
}
