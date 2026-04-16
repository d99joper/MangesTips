using System;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Tipset.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            try
            {
                string path = HttpContext.Server.MapPath("~/Models/SettingsExtensions.xml");
                XDocument xDoc = XDocument.Load(path);
                bool enableNewEntries = bool.Parse(
                    xDoc.Root.Element("EnableNewEntries").Attribute("On").Value);
                ViewBag.EnableNewEntries = enableNewEntries;
            }
            catch (Exception)
            {
                ViewBag.EnableNewEntries = false;
            }
        }
    }
}
