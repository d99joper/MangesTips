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
    public partial class Default : System.Web.UI.Page
    {
        Models.UserRepository userRepository = new Models.UserRepository();
        Models.BlogRepository commentRepository = new Models.BlogRepository();

        protected void Page_Load(object sender, EventArgs e)
        {
            XDocument xDoc = XDocument.Load(Server.MapPath(@"~/Models/SettingsExtensions.xml"));
            XAttribute xattr = xDoc.Root.Element("EnableNewEntries").Attribute("On");
            if (Boolean.Parse(xattr.Value))
            {
                wrap.InnerHtml = "<p>Här kommer den aktuella ställningen dyka upp när<br /> alla tipsrader är inlämnade.</p>";
                wrap.InnerHtml += "<p>För att inte riskera att de du tippar försvinner om <br />överföringen av kupongen misslyckas,<br />";
                wrap.InnerHtml += "rekommenderas att du laddar ner tipset i <a href=\"Docs/tipset2022.docx\">word</a><br />";
                wrap.InnerHtml += "eller <a href=\"Docs/tipset2022.xlsx\">excel</a> och tippar i lugn och ro,<br />";
                wrap.InnerHtml += "för att sedan fylla i det du tippat här på sidan.</p>";
                wrap.InnerHtml += "<p>Vill du lämna in en tipsrad följ länken nedan:<br />";
                wrap.InnerHtml += "<a href=\"Nykupong.aspx\">Fyll i en ny tipskupong</a></p>";
                wrap.InnerHtml += "<p>Lycka till!</p>";//<center><img src="images/loggan2012400.png" alt="Manges VM-tips" height="320px" /></center> 
                wrap.InnerHtml += "<center><img src=\"images/loggacolor.png\" alt=\"Manges VM-tips\" height=\"400px\" /></center> ";

                //Response.Redirect("Nykupong.aspx");
            }

            if (!IsPostBack)
            {
                ddlUpdates.DataSource = userRepository.GetStandingDates();
                ddlUpdates.DataBind();
                ddlUpdates.SelectedIndex = ddlUpdates.Items.Count -1;
                
            }

            if (ddlUpdates.Items.Count != 0)
            {
                Guid selectedGuid = new Guid(ddlUpdates.SelectedValue);
                List<Models.Standing> standings = userRepository.GetStandings(selectedGuid).ToList();

                grdStandings.DataSource = standings;
                grdStandings.DataBind();
            }
        }

        protected void SortUsers(object sender, GridViewSortEventArgs e)
        {
            IQueryable<Models.Standing> standings = userRepository.GetStandings(new Guid(ddlUpdates.SelectedValue));
            
            SortDirection _sortOrder = e.SortDirection;

            if (Session["SortExpression"] != null && Session["SortOrder"] != null)
            {
                string _sortBy = Session["SortExpression"].ToString();
                _sortOrder = (SortDirection)Session["SortOrder"];
                if (_sortBy == e.SortExpression)
                {
                    _sortOrder = _sortOrder == SortDirection.Descending ? SortDirection.Ascending : SortDirection.Descending;
                }
                else
                    _sortOrder = SortDirection.Ascending;
            }

            switch (e.SortExpression)
            { 
                case "Position":
                    if (_sortOrder == SortDirection.Ascending)
                        standings = standings.OrderBy(s => s.Position);
                    else
                        standings = standings.OrderByDescending(s => s.Position);
                    break;
                case "FirstName":
                    if (_sortOrder == SortDirection.Ascending)
                        standings = standings.OrderBy(s => s.User.FirstName);
                    else
                        standings = standings.OrderByDescending(s => s.User.FirstName);
                    break;
                case "LastName":
                    if (_sortOrder == SortDirection.Ascending)
                        standings = standings.OrderBy(s => s.User.LastName);
                    else
                        standings = standings.OrderByDescending(s => s.User.LastName);
                    break;
                case "TotalPoints":
                    if (_sortOrder == SortDirection.Ascending)
                        standings = standings.OrderBy(s => s.TotalPoints);
                    else
                        standings = standings.OrderByDescending(s => s.TotalPoints);
                    break;
            }

            Session["SortExpression"] = e.SortExpression;
            Session["SortOrder"] = _sortOrder;

            grdStandings.DataSource = standings;
            grdStandings.DataBind();
        }

        protected void ChangeStandings(object sender, EventArgs e)
        {
            Guid selectedGuid = new Guid(ddlUpdates.SelectedValue);
            IQueryable<Models.Standing> standings = userRepository.GetStandings(selectedGuid);

            grdStandings.DataSource = standings.ToList();
            grdStandings.DataBind();
        }

        protected void grdStandings_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Models.Standing standing = e.Row.DataItem as Models.Standing;
                e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(grdStandings, "Select$" + e.Row.RowIndex.ToString()));
                e.Row.Attributes.Add("onmouseover", "this.className='highlightrow';this.style.cursor='hand';");
                e.Row.Attributes.Add("onmouseout", "this.className='normalrow';this.style.cursor='cursor';");
            }
        }

        protected void ViewUserDetails(object sender, EventArgs e)
        {
            //GridViewRow selectedRow = grdStandings.Rows[e.NewSelectedIndex];
            Models.Standing standing = grdStandings.SelectedRow.DataItem as Models.Standing;
            Response.Redirect(String.Format("Details.aspx?id={0}", grdStandings.SelectedDataKey.Value));
        }

    }
}
