using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tipset
{
    public partial class BlogPage : System.Web.UI.Page
    {
        Models.BlogRepository commentRepository = new Models.BlogRepository();
        private string strID;
        TimeZone localZone = TimeZone.CurrentTimeZone;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                strID = Request.QueryString["id"].ToString();

                if (!IsPostBack)
                {
                    Models.BlogEntry blogEntry = commentRepository.GetBlogEntry(Int32.Parse(strID));

                    lblBlogTitle.Text = blogEntry.Title;
                    lblBlogText.Text = blogEntry.Text;
                    lblBlogDate.Text = blogEntry.PostedDate.ToString();

                    rptComments.DataSource = blogEntry.Comments;
                    rptComments.DataBind();
                }
            }
            catch
            {
                lblError.Text = "Kunde inte hitta inlägget du sökte efter.";
                divAddComment.Visible = false;
            }
        }

        protected void rptComments_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    ((Panel)e.Item.FindControl("divDeleteComment")).Visible = true;
                    ((ImageButton)e.Item.FindControl("btnDeleteComment")).OnClientClick = "if (!confirm('Är du säker på att du vill radera kommentaren?')) return;";
                }
            }   
        }

        protected void btnDeleteComment_OnClick(object sender, EventArgs e)
        {
            HiddenField hdnCommentID = (HiddenField)((ImageButton)sender).NamingContainer.FindControl("hdnCommentID");
            Models.Comment c = commentRepository.GetComment(Int32.Parse(hdnCommentID.Value));
            commentRepository.Delete(c);
            commentRepository.Save();
            Models.BlogEntry blogEntry = commentRepository.GetBlogEntry(Int32.Parse(strID));

            rptComments.DataSource = blogEntry.Comments;
            rptComments.DataBind();
        }

        protected void btnSendComment(object sender, EventArgs e)
        {
            try
            {
                Models.BlogEntry blogEntry = commentRepository.GetBlogEntry(Int32.Parse(strID));
                
                Models.Comment c = new Models.Comment();
                c.PostedBy = Server.HtmlEncode(txtName.Text);
                c.PostedDate = localZone.ToUniversalTime(DateTime.Now).AddHours(2); //DateTime.Now;
                c.Text = Server.HtmlEncode(txtComment.Text);
                c.Text = c.Text.Replace("\n", "<br />");
                
                blogEntry.Comments.Add(c);

                commentRepository.Save();

                txtComment.Text = "";
                txtName.Text = "";

                
                rptComments.DataSource = blogEntry.Comments;
                rptComments.DataBind();
            }
            catch (Exception ex)
            {
                lblError.Text = "Kunde inte spara kommentaren " + ex.Message;
            }
        }

    }
}
