using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tipset
{
    public partial class StatDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (Request.QueryString["type"])
            { 
                case "Match":
                    GenerateMatchStats();
                    break;
                case "playoffs":
                    GeneratePlayoffStats();
                    break;
                case "topscorer":
                    GenerateTopscorerStats();
                    break;
                default:
                    break;
            }
        }

        private void GenerateTopscorerStats()
        {
            List<Models.User> users = Models.UserRepository.GetUsersForTopscorer(int.Parse(Request.QueryString["id"]));

            foreach (Models.User u in users)
            {
                TableRow tr = new TableRow();

                TableCell td = new TableCell();
                td.Text = u.DisplayName;
                tr.Cells.Add(td);

                tblDetails.Rows.Add(tr);
            }
        }

        private void GeneratePlayoffStats()
        {
            int teamID = int.Parse(Request.QueryString["teamid"]);   
            string strStage = Request.QueryString["stage"];
            List<Models.User> users = Models.UserRepository.GetUserPlayoffTeams(strStage, teamID);
                    
            foreach (Models.User u in users)
            {
                TableRow tr = new TableRow();

                TableCell td = new TableCell();
                if (Models.UserRepository.CorrectTeamInStage(teamID, strStage))
                    td.CssClass = "highlight";
                td.Text = u.DisplayName;
                tr.Cells.Add(td);

                tblDetails.Rows.Add(tr);
            }
        }

        private void GenerateMatchStats()
        {
            // Get the match
            List<Models.UserMatch> uMatches = Models.UserRepository.GetAllUserMatches(int.Parse(Request.QueryString["id"]), Request.QueryString["result"]);

            foreach (Models.UserMatch um in uMatches)
            {
                String css = "";

                TableRow tr = new TableRow();

                TableCell td = new TableCell();
                if (um.Match.HomeGoals == um.HomeGoals && um.Match.AwayGoals == um.AwayGoals)
                    css = "highlight";

                td.CssClass = css;
                td.Text = um.User.DisplayName;
                tr.Cells.Add(td);
                
                td = new TableCell();
                td.CssClass = css; 
                td.Text = String.Format("{0} - {1}", um.HomeGoals, um.AwayGoals);
                tr.Cells.Add(td);

                tblDetails.Rows.Add(tr);
            }
        }
    }
}