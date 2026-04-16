using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace Tipset
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ProcessLogin(object sender, EventArgs e)
        {
            if(FormsAuthentication.Authenticate(txtUser.Text, txtPassword.Text))
                FormsAuthentication.RedirectFromLoginPage(txtUser.Text, false);
            else
                ErrorMessage.InnerHtml = "<b>Inloggningen misslyckades</b>";
        }
    }
}
