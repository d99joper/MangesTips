using System;
using System.Text;
using System.Web.Mvc;
using Tipset.Models;

namespace Tipset.Controllers
{
    public class ConfirmController : Controller
    {
        private readonly UserRepository _userRepository = new UserRepository();

        public ActionResult Index(Guid? id)
        {
            if (!id.HasValue)
            {
                ViewBag.Message = "Felaktigt anrop till denna sida.";
                return View();
            }

            try
            {
                User user = _userRepository.GetUser(id.Value);

                if (user == null)
                {
                    ViewBag.Message = "Din kupong kunde inte hittas. Var god kontakta Magnus.";
                    return View();
                }

                if (user.IsConfirmed)
                {
                    ViewBag.Message = string.Format(
                        "Du är redan anmäld. Du skickade in din kupong {0:dd/MM/yyyy}.",
                        user.PostedDate);
                    return View();
                }

                string payCode = RandomString(6);
                user.PayCode = payCode;
                user.IsConfirmed = true;
                _userRepository.Save();

                ViewBag.Message = "Tack! Din kupong är nu registrerad.";

                try
                {
                    string body = BuildConfirmationEmailBody(payCode, user.Guid.ToString());
                    Helpers.SendEmail.SendEmail_SMTP(
                        "noreply@nodomain.com", "Manges VM-tips",
                        user.EmailAddress, user.DisplayName,
                        "Du är anmäld till VM-tipset.", body);

                    ViewBag.MoreInfo =
                        "Ett mail har skickats till dig innehållandes en pdf-fil med din tipskupong.<br />" +
                        "Mailet innehåller även en alfanumerisk kod som du kan använda för att enklare identifiera dig om du tänkt betala via internetbank.<br />" +
                        "Efter den sista anmälningsdagen kommer alla kuponger att visas på hemsidan.";
                }
                catch
                {
                    ViewBag.Message =
                        "Din kupong är registrerad, men ett fel uppstod då sidan försökte skicka dig ett bekräftelsemail. " +
                        "Kontakta Magnus om hur du vill betala.";
                }
            }
            catch
            {
                ViewBag.Message = "Din kupong kunde inte hittas. Var god kontakta Magnus.";
            }

            return View();
        }

        private static string RandomString(int size)
        {
            var sb = new StringBuilder(size);
            var rng = new Random();
            for (int i = 0; i < size; i++)
                sb.Append((char)(rng.Next(26) + 65));
            return sb.ToString().ToLower();
        }

        private static string BuildConfirmationEmailBody(string payCode, string guid)
        {
            string pdfUrl = string.Format(
                "http://mangesvmtips2022.personablesolutions.com/pdfgenerator.aspx?id={0}", guid);
            return "Hej!" +
                   "<p>Du är nu anmäld till Manges VM-tips.</p>" +
                   "<p>Din kupong har nu skickats till Magnus.</p>" +
                   string.Format("<p>Om du tänkt betala via internetbank eller swish, ange följande kod: <strong>{0}</strong></p>", payCode) +
                   string.Format("<p>Du kan skriva ut din kupong: <a href=\"{0}\">{0}</a></p>", pdfUrl) +
                   "<p>Tack för din anmälan, och vi önskar dig mycket lycka till!</p>";
        }
    }
}
