using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tipset
{
    public partial class Statistics : System.Web.UI.Page
    {
        Models.TeamRepository teamRepository = new Models.TeamRepository();
        Models.TopScorerRepository topScorerRepository = new Models.TopScorerRepository();
        Models.UserRepository userRepository = new Models.UserRepository();
        Models.MatchRepository matchRepository = new Models.MatchRepository();      

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                rptMatches.DataSource = matchRepository.GetAllMatches().ToList();
                rptMatches.DataBind();

                grdTeamStats.DataSource = teamRepository.GetAllTeams().ToList();
                grdTeamStats.DataBind();

                rptTopScorer.DataSource = topScorerRepository.GetAllScorers().OrderByDescending(t => t.WinPercent).ToList();
                rptTopScorer.DataBind();

            }
        }

        protected void ViewTeamDetails(object sender, EventArgs e)
        { 
            
        }

        protected void grdTeamStats_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Models.Standing standing = e.Row.DataItem as Models.Standing;
                //e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(grdStandings, "Select$" + e.Row.RowIndex.ToString()));
                e.Row.Attributes.Add("onmouseover", "this.className='highlightrow';this.style.cursor='hand';");
                e.Row.Attributes.Add("onmouseout", "this.className='normalrow';this.style.cursor='cursor';");
            }
        }

        protected void SortTeams(object sender, GridViewSortEventArgs e)
        {
            IQueryable<Models.Team> teams = teamRepository.GetAllTeams();

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
                case "TeamName":
                    if (_sortOrder == SortDirection.Ascending)
                        teams = teams.OrderBy(t => t.TeamName);
                    else
                        teams = teams.OrderByDescending(t => t.TeamName);
                    break;
                case "Playoff":
                    if (_sortOrder == SortDirection.Ascending)
                        teams = teams.OrderBy(t => t.TeamStats.PlayoffPercent);
                    else
                        teams = teams.OrderByDescending(t => t.TeamStats.PlayoffPercent);
                    break;
                case "QuarterFinals":
                    if (_sortOrder == SortDirection.Ascending)
                        teams = teams.OrderBy(t => t.TeamStats.QuarterFinalPercent);
                    else
                        teams = teams.OrderByDescending(t => t.TeamStats.QuarterFinalPercent);
                    break;
                case "SemiFinals":
                    if (_sortOrder == SortDirection.Ascending)
                        teams = teams.OrderBy(t => t.TeamStats.SemiFinalPercent);
                    else
                        teams = teams.OrderByDescending(t => t.TeamStats.SemiFinalPercent);
                    break;
                case "Finals":
                    if (_sortOrder == SortDirection.Ascending)
                        teams = teams.OrderBy(t => t.TeamStats.FinalPercent);
                    else
                        teams = teams.OrderByDescending(t => t.TeamStats.FinalPercent);
                    break;
                case "Bronze":
                    if (_sortOrder == SortDirection.Ascending)
                        teams = teams.OrderBy(t => t.TeamStats.BronzePercent);
                    else
                        teams = teams.OrderByDescending(t => t.TeamStats.BronzePercent);
                    break;
                case "Silver":
                    if (_sortOrder == SortDirection.Ascending)
                        teams = teams.OrderBy(t => t.TeamStats.SilverPercent);
                    else
                        teams = teams.OrderByDescending(t => t.TeamStats.SilverPercent);
                    break;
                case "Gold":
                    if (_sortOrder == SortDirection.Ascending)
                        teams = teams.OrderBy(t => t.TeamStats.GoldPercent);
                    else
                        teams = teams.OrderByDescending(t => t.TeamStats.GoldPercent);
                    break;
            }

            Session["SortExpression"] = e.SortExpression;
            Session["SortOrder"] = _sortOrder;

            grdTeamStats.DataSource = teams.ToList();
            grdTeamStats.DataBind();
        }

    }
}
