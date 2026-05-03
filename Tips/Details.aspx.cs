using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Tipset
{
    public partial class Details : System.Web.UI.Page
    {
        Models.UserRepository userRepository = new Models.UserRepository();
        Models.TeamRepository teamRepository = new Models.TeamRepository();
        Models.MatchRepository matchRepository = new Models.MatchRepository();
        Models.TopScorerRepository topScorerRepository = new Models.TopScorerRepository();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string strUserID = Request.QueryString["id"].ToString();
                Models.User currentUser = userRepository.GetUser(Int32.Parse(strUserID));
                
                // Set header labels
                lblDisplayName.Text = currentUser.DisplayName;
                lblPlace.Text = currentUser.Standings.Last().Position.ToString();
                //lblPlace.Text = currentUser.Standings[currentUser.Standings.Count - 1].Position.ToString();
                lblPoints.Text = currentUser.Standings.Last().TotalPoints.ToString();
                Models.User_2010 user2010 = userRepository.GetVM2010User(currentUser.DisplayName);
                if(user2010 != null)
                    lblvmtips.Text = String.Format("Placering VM-tips 2010: <a href=\"http://mangesvmtips.personablesolutions.com/Details.aspx?id={0}\" target=\"_blank\">{1}</a><br />", user2010.ID, user2010.Standings_2010.Last().Position);

                Models.User_2012 user2012 = userRepository.GetEM2012User(currentUser.DisplayName);                
                if (user2012 != null)
                    lblemtips.Text = String.Format("Placering EM-tips 2012: <a href=\"http://mangesemtips2012.personablesolutions.com/Details.aspx?id={0}\" target=\"_blank\">{1}</a><br />", user2012.ID, user2012.Standings_2012.Last().Position);

                Models.User_2014 user2014 = userRepository.GetVM2014User(currentUser.DisplayName);
                if (user2014 != null)
                    lblvmtips2014.Text = String.Format("Placering VM-tips 2014: <a href=\"http://mangesvmtips2014.personablesolutions.com/Details.aspx?id={0}\" target=\"_blank\">{1}</a><br />", user2014.ID, user2014.Standings_2014.Last().Position);

                Models.User_2016 user2016 = userRepository.GetEM2016User(currentUser.DisplayName);
                if (user2016 != null)
                    lblemtips2016.Text = String.Format("Placering EM-tips 2016: <a href=\"http://mangesemtips2016.personablesolutions.com/Details.aspx?id={0}\" target=\"_blank\">{1}</a><br />", user2016.ID, user2016.Standings_2016.Last().Position);

                hplPDF.NavigateUrl = String.Format("pdfGenerator.aspx?id={0}", currentUser.Guid);

                rptUserMatches.DataSource = currentUser.UserMatches;
                rptUserMatches.DataBind();

                // Set playoff teams
                SetPlayoffTeams(currentUser);
                SetFinalsTeams(currentUser);
                SetTopScorer(currentUser);
            }
            catch(Exception ex) 
            {
                lblError.Text = "Användaren kunde inte hittas. " + ex.Message;
            }
        }

        private void SetTopScorer(Tipset.Models.User currentUser)
        {
            List<Models.TopScorer> topScorers = topScorerRepository.GetWinner();
            string postfix = "";

            foreach(Models.TopScorer topScorer in topScorers)
            {
                if (currentUser.TopScorer != null)
                {
                    if (currentUser.TopScorerID == topScorer.ID)
                        postfix = " 4p";
                }
            }
            lblTopScorer.Text = currentUser.TopScorer.DisplayName + postfix;
        }

        private void SetPlayoffTeams(Models.User currentUser)
        {
            try
            {
                // Get the team and compare with answer
                Models.UserPlayoffTeam upteam = currentUser.UserPlayoffTeams.SingleOrDefault(u => u.Position == 1 && u.Team.GroupID == "A");
                lblPO1A.Text = upteam.Team.TeamName;
                if (upteam.Team.IsInPlayOffs)
                    lblPO1A.Text += String.Format(" {0}p", upteam.Points);
                upteam = currentUser.UserPlayoffTeams.SingleOrDefault(u => u.Position == 2 && u.Team.GroupID == "A");
                lblPO2A.Text = upteam.Team.TeamName;
                if (upteam.Team.IsInPlayOffs)
                    lblPO2A.Text += String.Format(" {0}p", upteam.Points);
                upteam = currentUser.UserPlayoffTeams.SingleOrDefault(u => u.Position == 1 && u.Team.GroupID == "B");
                lblPO1B.Text = upteam.Team.TeamName;
                if (upteam.Team.IsInPlayOffs)
                    lblPO1B.Text += String.Format(" {0}p", upteam.Points);
                upteam = currentUser.UserPlayoffTeams.SingleOrDefault(u => u.Position == 2 && u.Team.GroupID == "B");
                lblPO2B.Text = upteam.Team.TeamName;
                if (upteam.Team.IsInPlayOffs)
                    lblPO2B.Text += String.Format(" {0}p", upteam.Points);
                upteam = currentUser.UserPlayoffTeams.SingleOrDefault(u => u.Position == 1 && u.Team.GroupID == "C");
                lblPO1C.Text = upteam.Team.TeamName;
                if (upteam.Team.IsInPlayOffs)
                    lblPO1C.Text += String.Format(" {0}p", upteam.Points);
                upteam = currentUser.UserPlayoffTeams.SingleOrDefault(u => u.Position == 2 && u.Team.GroupID == "C");
                lblPO2C.Text = upteam.Team.TeamName;
                if (upteam.Team.IsInPlayOffs)
                    lblPO2C.Text += String.Format(" {0}p", upteam.Points);
                upteam = currentUser.UserPlayoffTeams.SingleOrDefault(u => u.Position == 1 && u.Team.GroupID == "D");
                lblPO1D.Text = upteam.Team.TeamName;
                if (upteam.Team.IsInPlayOffs)
                    lblPO1D.Text += String.Format(" {0}p", upteam.Points);
                upteam = currentUser.UserPlayoffTeams.SingleOrDefault(u => u.Position == 2 && u.Team.GroupID == "D");
                lblPO2D.Text = upteam.Team.TeamName;
                if (upteam.Team.IsInPlayOffs)
                    lblPO2D.Text += String.Format(" {0}p", upteam.Points);
                upteam = currentUser.UserPlayoffTeams.SingleOrDefault(u => u.Position == 1 && u.Team.GroupID == "E");
                lblPO1E.Text = upteam.Team.TeamName;
                if (upteam.Team.IsInPlayOffs)
                    lblPO1E.Text += String.Format(" {0}p", upteam.Points);
                upteam = currentUser.UserPlayoffTeams.SingleOrDefault(u => u.Position == 2 && u.Team.GroupID == "E");
                lblPO2E.Text = upteam.Team.TeamName;
                if (upteam.Team.IsInPlayOffs)
                    lblPO2E.Text += String.Format(" {0}p", upteam.Points);
                upteam = currentUser.UserPlayoffTeams.SingleOrDefault(u => u.Position == 1 && u.Team.GroupID == "F");
                lblPO1F.Text = upteam.Team.TeamName;
                if (upteam.Team.IsInPlayOffs)
                    lblPO1F.Text += String.Format(" {0}p", upteam.Points);
                upteam = currentUser.UserPlayoffTeams.SingleOrDefault(u => u.Position == 2 && u.Team.GroupID == "F");
                lblPO2F.Text = upteam.Team.TeamName;
                if (upteam.Team.IsInPlayOffs)
                    lblPO2F.Text += String.Format(" {0}p", upteam.Points);
                upteam = currentUser.UserPlayoffTeams.SingleOrDefault(u => u.Position == 1 && u.Team.GroupID == "G");
                lblPO1G.Text = upteam.Team.TeamName;
                if (upteam.Team.IsInPlayOffs)
                    lblPO1G.Text += String.Format(" {0}p", upteam.Points);
                upteam = currentUser.UserPlayoffTeams.SingleOrDefault(u => u.Position == 2 && u.Team.GroupID == "G");
                lblPO2G.Text = upteam.Team.TeamName;
                if (upteam.Team.IsInPlayOffs)
                    lblPO2G.Text += String.Format(" {0}p", upteam.Points);
                upteam = currentUser.UserPlayoffTeams.SingleOrDefault(u => u.Position == 1 && u.Team.GroupID == "H");
                lblPO1H.Text = upteam.Team.TeamName;
                if (upteam.Team.IsInPlayOffs)
                    lblPO1H.Text += String.Format(" {0}p", upteam.Points);
                upteam = currentUser.UserPlayoffTeams.SingleOrDefault(u => u.Position == 2 && u.Team.GroupID == "H");
                lblPO2H.Text = upteam.Team.TeamName;
                if (upteam.Team.IsInPlayOffs)
                    lblPO2H.Text += String.Format(" {0}p", upteam.Points);

                // Check for Bonus points
                if (currentUser.BonusPoints.Where(b => b.GroupID == "A" && b.Point == 1).Count() != 0)
                    lblBonusA.Text = " +1p";
                if (currentUser.BonusPoints.Where(b => b.GroupID == "B" && b.Point == 1).Count() != 0)
                    lblBonusB.Text = " +1p";
                if (currentUser.BonusPoints.Where(b => b.GroupID == "C" && b.Point == 1).Count() != 0)
                    lblBonusC.Text = " +1p";
                if (currentUser.BonusPoints.Where(b => b.GroupID == "D" && b.Point == 1).Count() != 0)
                    lblBonusD.Text = " +1p";
                if (currentUser.BonusPoints.Where(b => b.GroupID == "E" && b.Point == 1).Count() != 0)
                    lblBonusE.Text = " +1p";
                if (currentUser.BonusPoints.Where(b => b.GroupID == "F" && b.Point == 1).Count() != 0)
                    lblBonusF.Text = " +1p";
                if (currentUser.BonusPoints.Where(b => b.GroupID == "G" && b.Point == 1).Count() != 0)
                    lblBonusG.Text = " +1p";
                if (currentUser.BonusPoints.Where(b => b.GroupID == "H" && b.Point == 1).Count() != 0)
                    lblBonusH.Text = " +1p";

            }
            catch (Exception e)
            {
                lblError.Text = "Problem med slutspelslagen" + e.Message;   
            }
            
        }

        private void SetFinalsTeams(Models.User currentUser)
        {
            HtmlTableRow row = new HtmlTableRow();

            int counter = 0;

            foreach (Models.UserQFTeam team in currentUser.UserQFTeams)
            {
                if (counter % 2 == 0)
                    row = new HtmlTableRow();

                HtmlTableCell cell = new HtmlTableCell();
                row.Cells.Add(cell);

                if (team.Points != 0)
                    cell.InnerHtml = String.Format("{0} {1}p", team.Team.TeamName, team.Points);
                else
                    cell.InnerHtml = team.Team.TeamName;
                if (counter % 2 == 1)
                    tblQuarterFinal.Rows.Add(row);

                counter++;
            }

            counter = 0;
            foreach (Models.UserSFTeam team in currentUser.UserSFTeams)
            {
                if (counter % 2 == 0)
                    row = new HtmlTableRow();

                HtmlTableCell cell = new HtmlTableCell();
                row.Cells.Add(cell);

                if (team.Points != 0)
                    cell.InnerHtml = String.Format("{0} {1}p", team.Team.TeamName, team.Points);
                else
                    cell.InnerHtml = team.Team.TeamName;
                if (counter % 2 == 1)
                    tblSemiFinalTeams.Rows.Add(row);

                counter++;
            }

            row = new HtmlTableRow();
            foreach (Models.UserFinalTeam team in currentUser.UserFinalTeams)
            {
                HtmlTableCell cell = new HtmlTableCell();
                row.Cells.Add(cell);
                if (team.Points != 0)
                    cell.InnerHtml = String.Format("{0} {1}p", team.Team.TeamName, team.Points);
                else
                    cell.InnerHtml = team.Team.TeamName;

            }

            tblFinalTeams.Rows.Add(row);

            if (currentUser.UserBronzeTeam.ElementAt(0).Points != 0)
                lblBronze.Text = String.Format("{0} {1}p", currentUser.UserBronzeTeam.ElementAt(0).Team.TeamName, currentUser.UserBronzeTeam.ElementAt(0).Points);
            else
                lblBronze.Text = currentUser.UserBronzeTeam.ElementAt(0).Team.TeamName;

            if (currentUser.UserSilverTeam.ElementAt(0).Points != 0)
                lblSilver.Text = String.Format("{0} {1}p", currentUser.UserSilverTeam.ElementAt(0).Team.TeamName, currentUser.UserSilverTeam.ElementAt(0).Points);
            else
                lblSilver.Text = currentUser.UserSilverTeam.ElementAt(0).Team.TeamName;

            if (currentUser.UserGoldTeam.ElementAt(0).Points != 0)
                lblGold.Text = String.Format("{0} {1}p", currentUser.UserGoldTeam.ElementAt(0).Team.TeamName, currentUser.UserGoldTeam.ElementAt(0).Points);
            else
                lblGold.Text = currentUser.UserGoldTeam.ElementAt(0).Team.TeamName;
        }
    }
}
