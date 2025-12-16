using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace Tips
{
    public partial class Blog : System.Web.UI.Page
    {
        Models.UserRepository userRepository = new Models.UserRepository();
        Models.BlogRepository commentRepository = new Models.BlogRepository();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                rptBlog.DataSource = commentRepository.GetAllBlogEntries().ToList();
                rptBlog.DataBind();
            }
        }

        

    }
}
