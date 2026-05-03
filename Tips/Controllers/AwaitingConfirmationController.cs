using System.Web.Mvc;

namespace Tipset.Controllers
{
    public class AwaitingConfirmationController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
