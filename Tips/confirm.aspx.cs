using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tips
{
    public partial class confirm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Models.User thisUser;

            try
            {
                string strGuid = Request.QueryString["id"].ToString();
                Guid thisGuid = new Guid(strGuid);
                try
                {
                    Models.UserRepository userRepository = new Models.UserRepository();
                    thisUser = userRepository.GetUser(thisGuid);

                    if (thisUser.IsConfirmed)
                    {
                        lblMessage.InnerHtml = String.Format("Du är redan anmäld. Du skickade in din kupong {0:dd/MM/yyyy}", thisUser.PostedDate);
                    }
                    else
                    {
                        string strRandom = RandomString(6, true);
                        thisUser.PayCode = strRandom;
                        thisUser.IsConfirmed = true;
                        userRepository.Save();

                        try
                        { // Send the email with the internet bank code
                            
                            string strBody = GetMessageBody(thisUser.PayCode, thisUser.Guid.ToString());
                            Helpers.SendEmail.SendEmail_SMTP("noreply@nodomain.com", "Manges VM-tips", thisUser.EmailAddress, thisUser.DisplayName, "Du är anmäld till VM-tipset.", strBody);

                            // set some more info on success
                            lblMoreInfo.Text = "Ett mail har skickats till dig innehållandes en pdf-fil med din tipskupong <br />";
                            lblMoreInfo.Text += "Mailet innehåller även en alfanumerisk kod som du kan använda för att enklare identifiera dig om du tänkt betala via internetbank. <br />";
                            lblMoreInfo.Text += "Efter den sista anmälningsdagen till Manges VM-tips kommer alla kuponger att visas på http://mangesvmtips2022.personablesolutions.com.  ";
                        }
                        catch
                        {
                            lblMessage.InnerText = "Din kupong är registrerad, men ett fel uppstod då sidan försökte skicka dig ett bekräftelsemail.  Kontakta Magnus om hur du vill betala.";
                        }
                    }

                }
                catch {
                    lblMessage.InnerText = "Din kupong kunde inte hittas.  Var god kontakta Magnus.";
                }
            }
            catch {
                lblMessage.InnerText = "Felaktigt anrop till denna sida.";
            }
        }

        private string GetMessageBody(string strRandom, string guid)
        {
            string strBody = "Hej!<p>Du är nu anmäld till Manges VM-tips.</p>";
            strBody += "<p>Din kupong har nu skickats till Magnus. Mange kommer att kontakta dig för att bekräfta ditt deltagande och hur betalning ska gå till.</p>";
            strBody += String.Format("<p>Om du tänkt betala via en internetbank eller swish, var god ange följande kod för att Mange lättare ska kunna identifiera dig: {0}", strRandom);
            strBody += String.Format("<p>Du kan skriva ut din kupong om du klickar på följande länk:<br /> <a href=\"{0}\">{0}</a>", String.Format("http://mangesvmtips2022.personablesolutions.com/pdfgenerator.aspx?id={0}", guid));
            strBody += "<p>Tack för din anmälan, och vi önskar dig mycket spänning och lycka till!</p>";

            return strBody;
        }

        private string RandomString(int size, bool lowerCase)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
    }
}
