using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;

namespace Tips.Admin
{
    public partial class Default : System.Web.UI.Page
    {
        Models.TeamRepository teamRepository = new Tips.Models.TeamRepository();
        Models.MatchRepository matchRepository = new Tips.Models.MatchRepository();
        Models.UserRepository userRepository = new Tips.Models.UserRepository();
        Models.BlogRepository blogRepository = new Models.BlogRepository();
        Models.TopScorerRepository topScorerRepository = new Models.TopScorerRepository();
        TimeZone localZone = TimeZone.CurrentTimeZone;
        private DateTime dtUpdateTime;//DateTime.Now.AddHours(6);

        protected void Page_Load(object sender, EventArgs e)
        {
            // Set the current time to Swedish
            dtUpdateTime = localZone.ToUniversalTime(DateTime.Now).AddHours(2);            

            if (!IsPostBack)
            {
                if (Request.QueryString["ajax"] != null)
                {
                    Response.Write("[\"test\"]");
                    Response.End();
                }
                // UserMatches
                rptMatches.DataSource = matchRepository.GetAllMatches().ToList();
                rptMatches.DataBind();

                // Set Play-off teams
                SetPlayOffTeams();

                // Set all other team lists
                SetTeamLists();

                // Set the top scorer list
                SetTopScorer();

                // Set the user list in tab2
                rptUsers.DataSource = userRepository.GetAllUsers().ToList();//GetAllConfirmedUsers();
                rptUsers.DataBind();

                // Other settings
                XDocument xDoc = XDocument.Load(Server.MapPath(@"~/Models/SettingsExtensions.xml"));
                XAttribute xattr = xDoc.Root.Element("EnableNewEntries").Attribute("On");
                if (Boolean.Parse(xattr.Value))
                { 
                    lblDisableNewEntries.Text = "Systemet tillåter nya anmälningar.";
                    btnEnableDisableNewEntries.Text = "Förbjud nya anmälningar";
                }
                else
                {
                    lblDisableNewEntries.Text = "Systemet förbjuder nya anmälningar.";
                    btnEnableDisableNewEntries.Text = "Tillåt nya anmälningar";                
                }

                xattr = xDoc.Root.Element("CSSStyle").Attribute("type");
                ddlCssStyle.SelectedValue = xattr.Value;

                if (Request.QueryString["tab"] != null)
                    tabs.ActiveTab = tabOther;

                // blog entries
                IQueryable<Models.BlogEntry> blogEntries = blogRepository.GetAllBlogEntries();
                grdBlogEntries.DataSource = blogEntries.ToList();
                grdBlogEntries.DataBind();

                // Top scorers
                IQueryable<Models.TopScorer> topScorers = topScorerRepository.GetAllScorers();
                grdTopScorer.DataSource = topScorers.ToList();
                grdTopScorer.DataBind();

                // Teams
                List<Models.Team> allTeams = teamRepository.GetAllTeams().ToList();
                grdTeams.DataSource = allTeams;
                grdTeams.DataBind();

                // Matches
                grdMatches.DataSource = matchRepository.GetAllMatches().ToList();
                grdMatches.DataBind();
                ddlMatchAwayTeam.DataSource = allTeams;
                ddlMatchAwayTeam.DataBind();
                ddlMatchHomeTeam.DataSource = allTeams;
                ddlMatchHomeTeam.DataBind();
            }
        }

        protected void btnSubmitResult_OnClick(object sender, EventArgs e)
        {
            lblError.Text = "";

            SaveAnswers();

            UpdateUsers();
        }

        private void UpdateUsers()
        {
            try
            {
                // Reset all bonus points to zero
                userRepository.ResetAllBonusPoints();

                // Update each user individually
                IQueryable<Models.User> users = userRepository.GetAllActiveUsers();
                IQueryable<Models.Match> matches = matchRepository.GetAllMatches();
                Guid standingsGuid = System.Guid.NewGuid();
                foreach (Models.User user in users)
                {
                    Int16 intTotalPoints = 0;

                    // Update all user matches
                    foreach (Models.Match match in matches)
                    {
                        Models.UserMatch userMatch = user.UserMatches.SingleOrDefault(um => um.MatchID == match.ID);
                        if (match.ResultMark != null)
                        {
                            userMatch.Points = 0;
                            if (userMatch.HomeGoals == match.HomeGoals && userMatch.AwayGoals == match.AwayGoals)
                                userMatch.Points++;
                            if (match.ResultMark == "1" && (userMatch.HomeGoals > userMatch.AwayGoals))
                                userMatch.Points = (byte)(userMatch.Points + 2); //userMatch.Points++;
                            else if (match.ResultMark == "X" && (userMatch.HomeGoals == userMatch.AwayGoals))
                                userMatch.Points = (byte)(userMatch.Points + 2); //userMatch.Points++;
                            else if (match.ResultMark == "2" && (userMatch.HomeGoals < userMatch.AwayGoals))
                                userMatch.Points = (byte)(userMatch.Points + 2); //userMatch.Points++;
                        }
                        else
                            userMatch.Points = 0;
                        intTotalPoints += (Int16)userMatch.Points;
                    }

                    // Update playoff teams
                    List<Models.Team> teamList = teamRepository.GetTeams(Models.TeamRepository.TeamInqueryType.isInPlayoffs);
                    foreach (Models.Team pt in teamList)
                    {
                        Models.UserPlayoffTeam uteam = user.UserPlayoffTeams.SingleOrDefault(t => t.TeamID == pt.ID);
                        if (uteam != null)
                        {
                            uteam.Points = 2;

                            Models.BonusPoints bp = uteam.User.BonusPoints.SingleOrDefault(b => b.GroupID == uteam.Team.GroupID);
                            if (uteam.Points == 2 && bp.HalfPoint)
                                bp.Point = 2;
                            else if (uteam.Points == 2 && !bp.HalfPoint)
                                bp.HalfPoint = true;

                            if (uteam.Position == pt.PlayOffPos)
                                uteam.Points = (byte)(uteam.Points + 2);

                            intTotalPoints += (Int16)(uteam.Points + bp.Point);

                        }
                        

                    }

                    // Update quarter final teams
                    teamList = teamRepository.GetTeams(Models.TeamRepository.TeamInqueryType.isInQuarterFinals);
                    foreach (Models.Team pt in teamList)
                    {
                        Models.UserQFTeam uteam = user.UserQFTeams.SingleOrDefault(t => t.TeamID == pt.ID);
                        if (uteam != null)
                        {
                            uteam.Points = 4;
                            intTotalPoints += (Int16)uteam.Points;
                        }
                    }

                    // Update semi final teams
                    teamList = teamRepository.GetTeams(Models.TeamRepository.TeamInqueryType.isInSemiFinals);
                    foreach (Models.Team pt in teamList)
                    {
                        Models.UserSFTeam uteam = user.UserSFTeams.SingleOrDefault(t => t.TeamID == pt.ID);
                        if (uteam != null)
                        {
                            uteam.Points = 4;
                            intTotalPoints += (Int16)uteam.Points;
                        }
                    }

                    // Update final teams
                    teamList = teamRepository.GetTeams(Models.TeamRepository.TeamInqueryType.isInFinals);
                    foreach (Models.Team pt in teamList)
                    {
                        Models.UserFinalTeam uteam = user.UserFinalTeams.SingleOrDefault(t => t.TeamID == pt.ID);
                        if (uteam != null)
                        {
                            uteam.Points = 4;
                            intTotalPoints += (Int16)uteam.Points;
                        }
                    }

                    // Update bronze winner
                    teamList = teamRepository.GetTeams(Models.TeamRepository.TeamInqueryType.WonBronze);
                    foreach (Models.Team pt in teamList)
                    {
                        Models.UserBronzeTeam uteam = user.UserBronzeTeam.SingleOrDefault(t => t.TeamID == pt.ID);
                        if (uteam != null)
                        {
                            uteam.Points = 10;
                            intTotalPoints += (Int16)uteam.Points;
                        }
                    }

                    // Update silver winner
                    teamList = teamRepository.GetTeams(Models.TeamRepository.TeamInqueryType.WonSilver);
                    foreach (Models.Team pt in teamList)
                    {
                        Models.UserSilverTeam uteam = user.UserSilverTeam.SingleOrDefault(t => t.TeamID == pt.ID);
                        if (uteam != null)
                        {
                            uteam.Points = 10;
                            intTotalPoints += (Int16)uteam.Points;
                        }
                    }

                    // Update gold winner
                    teamList = teamRepository.GetTeams(Models.TeamRepository.TeamInqueryType.WonGold);
                    foreach (Models.Team pt in teamList)
                    {
                        Models.UserGoldTeam uteam = user.UserGoldTeam.SingleOrDefault(t => t.TeamID == pt.ID);
                        if (uteam != null)
                        {
                            uteam.Points = 10;
                            intTotalPoints += (Int16)uteam.Points;
                        }
                    }

                    //Update top scorer
                    List<Models.TopScorer> topScorers = topScorerRepository.GetWinner();
                    foreach (Models.TopScorer topScorer in topScorers)
                    {
                        if (user.TopScorerID == topScorer.ID)
                                intTotalPoints += 10;
                    }

                    // Create a user standing
                    Models.Standing uStanding = new Models.Standing();
                    uStanding.TotalPoints = intTotalPoints;
                    uStanding.UpdateDate = dtUpdateTime;
                    uStanding.Guid = standingsGuid;
                    
                    // Add the user standing
                    user.Standings.Add(uStanding);                    
                }

                // Save all the updated users
                userRepository.Save();

                // Add positions to all standings
                userRepository.SortStandings(dtUpdateTime);

                // Save all the updated standings
                userRepository.Save();
            }
            catch(Exception e)
            {
                lblError.Text = "Något gick fel när användarna skulle uppdateras. <br />" + e.Message;
                lblError.Focus();
            }
        }

        private void SaveAnswers()
        {
            for (int i = 0; i < rptMatches.Items.Count; i++)
            {
                HiddenField hdnMatchID = (HiddenField)rptMatches.Items[i].FindControl("hdnMatchID");
                TextBox txtHomeGoals = (TextBox)rptMatches.Items[i].FindControl("txtHomeGoals");
                TextBox txtAwayGoals = (TextBox)rptMatches.Items[i].FindControl("txtAwayGoals");
                Models.Match match = matchRepository.GetMatch(Int32.Parse(hdnMatchID.Value));
                // only correct the matches that have a result
                if (txtHomeGoals.Text.Length > 0 && txtAwayGoals.Text.Length > 0)
                {
                    match.HomeGoals = Byte.Parse(txtHomeGoals.Text);
                    match.AwayGoals = Byte.Parse(txtAwayGoals.Text);
                    if (match.HomeGoals > match.AwayGoals)
                        match.ResultMark = "1";
                    else if (match.HomeGoals == match.AwayGoals)
                        match.ResultMark = "X";
                    else
                        match.ResultMark = "2";
                }
                else
                    match.ResultMark = null;
            }
            // Save the mathes
            try
            {
                matchRepository.Save();
            }
            catch (Exception ex)
            {
                lblError.Text = "Något gick fel när en match skulle sparas. " + ex.Message;
                return;
            }

            // Reset all teams
            teamRepository.ResetAllTeams();

            // Re-add teams
            AddPlayoffTeams();
            AddQuarterFinalTeams();
            AddSemiFinalTeams();
            AddFinalTeams();
            AddTopScorer();

            teamRepository.Save();
        }

        private void SetTopScorer()
        {
            List<Models.TopScorer> topScorers = topScorerRepository.GetWinner();
            int counter = 0;
            foreach (Models.TopScorer topScorer in topScorers)
            {
                if (counter > 0)
                {
                    TextBox tbox = new TextBox();
                    tbox.ID = "txtAdditionalTopScorers" + counter;
                    tbox.Text = topScorer.DisplayName;
                    tbox.Style.Add("display", "block;");
                    pnlScorers.Controls.AddAt(counter-1, tbox);
                } 
                else
                    txtTopScorer.Text = topScorer.DisplayName;

                counter++;
            }
        }

        protected void AddMoreAnswerScorers(object sender, EventArgs e) { }

        private void SetPlayOffTeams()
        {
            ddlGruppA_Lag1.DataSource = teamRepository.GetTeams("A").ToList();
            ddlGruppA_Lag2.DataSource = teamRepository.GetTeams("A").ToList();
            ddlGruppB_Lag1.DataSource = teamRepository.GetTeams("B").ToList();
            ddlGruppB_Lag2.DataSource = teamRepository.GetTeams("B").ToList();
            ddlGruppC_Lag1.DataSource = teamRepository.GetTeams("C").ToList();
            ddlGruppC_Lag2.DataSource = teamRepository.GetTeams("C").ToList();
            ddlGruppD_Lag1.DataSource = teamRepository.GetTeams("D").ToList();
            ddlGruppD_Lag2.DataSource = teamRepository.GetTeams("D").ToList();
            ddlGruppE_Lag1.DataSource = teamRepository.GetTeams("E").ToList();
            ddlGruppE_Lag2.DataSource = teamRepository.GetTeams("E").ToList();
            ddlGruppF_Lag1.DataSource = teamRepository.GetTeams("F").ToList();
            ddlGruppF_Lag2.DataSource = teamRepository.GetTeams("F").ToList();
            ddlGruppG_Lag1.DataSource = teamRepository.GetTeams("G").ToList();
            ddlGruppG_Lag2.DataSource = teamRepository.GetTeams("G").ToList();
            ddlGruppH_Lag1.DataSource = teamRepository.GetTeams("H").ToList();
            ddlGruppH_Lag2.DataSource = teamRepository.GetTeams("H").ToList();

            ddlGruppA_Lag1.DataBind();
            ddlGruppA_Lag2.DataBind();
            Models.Team playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "A", 1);
            if (playoffTeam != null)
                ddlGruppA_Lag1.SelectedIndex = ddlGruppA_Lag1.Items.IndexOf(ddlGruppA_Lag1.Items.FindByValue(playoffTeam.ID.ToString()));
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "A", 2);
            if (playoffTeam != null)
                ddlGruppA_Lag2.SelectedIndex = ddlGruppA_Lag2.Items.IndexOf(ddlGruppA_Lag2.Items.FindByValue(playoffTeam.ID.ToString()));
            ddlGruppB_Lag1.DataBind();
            ddlGruppB_Lag2.DataBind();
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "B", 1);
            if (playoffTeam != null)
                ddlGruppB_Lag1.SelectedIndex = ddlGruppB_Lag1.Items.IndexOf(ddlGruppB_Lag1.Items.FindByValue(playoffTeam.ID.ToString()));
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "B", 2);
            if (playoffTeam != null)
                ddlGruppB_Lag2.SelectedIndex = ddlGruppB_Lag2.Items.IndexOf(ddlGruppB_Lag2.Items.FindByValue(playoffTeam.ID.ToString()));
            ddlGruppC_Lag1.DataBind();
            ddlGruppC_Lag2.DataBind();
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "C", 1);
            if (playoffTeam != null)
                ddlGruppC_Lag1.SelectedIndex = ddlGruppC_Lag1.Items.IndexOf(ddlGruppC_Lag1.Items.FindByValue(playoffTeam.ID.ToString()));
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "C", 2);
            if (playoffTeam != null)
                ddlGruppC_Lag2.SelectedIndex = ddlGruppC_Lag2.Items.IndexOf(ddlGruppC_Lag2.Items.FindByValue(playoffTeam.ID.ToString()));
            ddlGruppD_Lag1.DataBind();
            ddlGruppD_Lag2.DataBind();
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "D", 1);
            if (playoffTeam != null)
                ddlGruppD_Lag1.SelectedIndex = ddlGruppD_Lag1.Items.IndexOf(ddlGruppD_Lag1.Items.FindByValue(playoffTeam.ID.ToString()));
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "D", 2);
            if (playoffTeam != null)
                ddlGruppD_Lag2.SelectedIndex = ddlGruppD_Lag2.Items.IndexOf(ddlGruppD_Lag2.Items.FindByValue(playoffTeam.ID.ToString()));
            ddlGruppE_Lag1.DataBind();
            ddlGruppE_Lag2.DataBind();
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "E", 1);
            if (playoffTeam != null)
                ddlGruppE_Lag1.SelectedIndex = ddlGruppE_Lag1.Items.IndexOf(ddlGruppE_Lag1.Items.FindByValue(playoffTeam.ID.ToString()));
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "E", 2);
            if (playoffTeam != null)
                ddlGruppE_Lag2.SelectedIndex = ddlGruppE_Lag2.Items.IndexOf(ddlGruppE_Lag2.Items.FindByValue(playoffTeam.ID.ToString()));
            ddlGruppF_Lag1.DataBind();
            ddlGruppF_Lag2.DataBind();
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "F", 1);
            if (playoffTeam != null)
                ddlGruppF_Lag1.SelectedIndex = ddlGruppF_Lag1.Items.IndexOf(ddlGruppF_Lag1.Items.FindByValue(playoffTeam.ID.ToString()));
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "F", 2);
            if (playoffTeam != null)
                ddlGruppF_Lag2.SelectedIndex = ddlGruppF_Lag2.Items.IndexOf(ddlGruppF_Lag2.Items.FindByValue(playoffTeam.ID.ToString()));
            ddlGruppG_Lag1.DataBind();
            ddlGruppG_Lag2.DataBind();
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "G", 1);
            if (playoffTeam != null)
                ddlGruppG_Lag1.SelectedIndex = ddlGruppG_Lag1.Items.IndexOf(ddlGruppG_Lag1.Items.FindByValue(playoffTeam.ID.ToString()));
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "G", 2);
            if (playoffTeam != null)
                ddlGruppG_Lag2.SelectedIndex = ddlGruppG_Lag2.Items.IndexOf(ddlGruppG_Lag2.Items.FindByValue(playoffTeam.ID.ToString()));
            ddlGruppH_Lag1.DataBind();
            ddlGruppH_Lag2.DataBind();
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "H", 1);
            if (playoffTeam != null)
                ddlGruppH_Lag1.SelectedIndex = ddlGruppH_Lag1.Items.IndexOf(ddlGruppH_Lag1.Items.FindByValue(playoffTeam.ID.ToString()));
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "H", 2);
            if (playoffTeam != null)
                ddlGruppH_Lag2.SelectedIndex = ddlGruppH_Lag2.Items.IndexOf(ddlGruppH_Lag2.Items.FindByValue(playoffTeam.ID.ToString()));
        }

        private void SetTeamLists()
        {
            IQueryable<Models.Team> teamList = teamRepository.GetAllTeams();
            ddlKvart1.DataSource = teamList.ToList();
            ddlKvart1.DataBind();
            ddlKvart2.DataSource = teamList.ToList();
            ddlKvart2.DataBind();
            ddlKvart3.DataSource = teamList.ToList();
            ddlKvart3.DataBind();
            ddlKvart4.DataSource = teamList.ToList();
            ddlKvart4.DataBind();
            ddlKvart5.DataSource = teamList.ToList();
            ddlKvart5.DataBind();
            ddlKvart6.DataSource = teamList.ToList();
            ddlKvart6.DataBind();
            ddlKvart7.DataSource = teamList.ToList();
            ddlKvart7.DataBind();
            ddlKvart8.DataSource = teamList.ToList();
            ddlKvart8.DataBind();
            List<Models.Team> teamList2 = teamRepository.GetTeams(Models.TeamRepository.TeamInqueryType.isInQuarterFinals);
            for (int i = 0; i < teamList2.Count; i++)
            {
                DropDownList ddlKvart = (DropDownList)divQuarterFinalTeams.FindControl(String.Format("ddlKvart{0}", i + 1));
                ddlKvart.SelectedIndex = ddlKvart.Items.IndexOf(ddlKvart.Items.FindByValue(teamList2[i].ID.ToString()));
            }

            ddlSemi1.DataSource = teamList.ToList();
            ddlSemi1.DataBind();
            ddlSemi2.DataSource = teamList.ToList();
            ddlSemi2.DataBind();
            ddlSemi3.DataSource = teamList.ToList();
            ddlSemi3.DataBind();
            ddlSemi4.DataSource = teamList.ToList();
            ddlSemi4.DataBind();
            teamList2 = teamRepository.GetTeams(Models.TeamRepository.TeamInqueryType.isInSemiFinals);
            for (int i = 0; i < teamList2.Count; i++)
            {
                DropDownList ddlSemi = (DropDownList)divSemiFinalTeams.FindControl(String.Format("ddlSemi{0}", i + 1));
                ddlSemi.SelectedIndex = ddlSemi.Items.IndexOf(ddlSemi.Items.FindByValue(teamList2[i].ID.ToString()));
            }

            ddlFinal1.DataSource = teamList.ToList();
            ddlFinal1.DataBind();
            ddlFinal2.DataSource = teamList.ToList();
            ddlFinal2.DataBind();
            teamList2 = teamRepository.GetTeams(Models.TeamRepository.TeamInqueryType.isInFinals);
            for (int i = 0; i < teamList2.Count; i++)
            {
                DropDownList ddlFinal = (DropDownList)divFinalTeams.FindControl(String.Format("ddlFinal{0}", i + 1));
                ddlFinal.SelectedIndex = ddlFinal.Items.IndexOf(ddlFinal.Items.FindByValue(teamList2[i].ID.ToString()));
            }

            ddlBronze.DataSource = teamList.ToList();
            ddlBronze.DataBind();
            teamList2 = teamRepository.GetTeams(Models.TeamRepository.TeamInqueryType.WonBronze);
            if (teamList2.Count > 0)
                ddlBronze.SelectedIndex = ddlBronze.Items.IndexOf(ddlBronze.Items.FindByValue(teamList2[0].ID.ToString()));

            ddlSilver.DataSource = teamList.ToList();
            ddlSilver.DataBind();
            teamList2 = teamRepository.GetTeams(Models.TeamRepository.TeamInqueryType.WonSilver);
            if (teamList2.Count > 0)
                ddlSilver.SelectedIndex = ddlSilver.Items.IndexOf(ddlSilver.Items.FindByValue(teamList2[0].ID.ToString()));

            ddlGold.DataSource = teamList.ToList();
            ddlGold.DataBind();
            teamList2 = teamRepository.GetTeams(Models.TeamRepository.TeamInqueryType.WonGold);
            if (teamList2.Count > 0)
                ddlGold.SelectedIndex = ddlGold.Items.IndexOf(ddlGold.Items.FindByValue(teamList2[0].ID.ToString()));

        }

        private void AddTopScorer()
        {
            topScorerRepository.ResetWinner();
            Models.TopScorer topScorer = topScorerRepository.GetTopScorer(txtTopScorer.Text);
            if (topScorer != null)
                topScorer.IsWinner = true;

            foreach (string scorer in hdnTopScorers.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                topScorer = topScorerRepository.GetTopScorer(scorer);
                if (topScorer != null)
                    topScorer.IsWinner = true;
            }

            topScorerRepository.Save();

            SetTopScorer();
            
        }

        private void AddPlayoffTeams()
        {
            DropDownList ddlCurrentTeam;

            foreach (Control c in divPlayoffTeams.Controls)
            {
                if (c.ID != null)
                {
                    if (c.ID.StartsWith("ddl"))
                    {
                        ddlCurrentTeam = (DropDownList)c;
                        if (ddlCurrentTeam.SelectedValue != "-1")
                        {
                            Models.Team team = teamRepository.GetTeam(Int32.Parse(ddlCurrentTeam.SelectedValue));
                            if (team != null)
                            {
                                team.IsInPlayOffs = true;
                                if (c.ID.EndsWith("1"))
                                    team.PlayOffPos = 1;
                                else if (c.ID.EndsWith("2"))
                                    team.PlayOffPos = 2;
            }}}}}
        }

        private void AddQuarterFinalTeams()
        {
            DropDownList ddlCurrentTeam;

            foreach (Control c in divQuarterFinalTeams.Controls)
            {
                if (c.ID != null)
                {
                    if (c.ID.StartsWith("ddl"))
                    {
                        ddlCurrentTeam = (DropDownList)c;
                        if (ddlCurrentTeam.SelectedValue != "-1")
                        {
                            Models.Team team = teamRepository.GetTeam(Int32.Parse(ddlCurrentTeam.SelectedValue));
                            if (team != null)
                                team.IsInQuarterFinals = true;
                        }
                    }
                }
            }
        }

        private void AddSemiFinalTeams()
        {
            DropDownList ddlCurrentTeam;

            foreach (Control c in divSemiFinalTeams.Controls)
            {
                if (c.ID != null)
                {
                    if (c.ID.StartsWith("ddl"))
                    {
                        ddlCurrentTeam = (DropDownList)c;
                        if (ddlCurrentTeam.SelectedValue != "-1")
                        {
                            Models.Team team = teamRepository.GetTeam(Int32.Parse(ddlCurrentTeam.SelectedValue));
                            if (team != null)
                                team.IsInSemiFinals = true;
                        }
                    }
                }
            }
        }

        private void AddFinalTeams()
        {
            DropDownList ddlCurrentTeam;

            foreach (Control c in divFinalTeams.Controls)
            {
                if (c.ID != null)
                {
                    if (c.ID.StartsWith("ddl"))
                    {
                        ddlCurrentTeam = (DropDownList)c;
                        if (ddlCurrentTeam.SelectedValue != "-1")
                        {
                            Models.Team team = teamRepository.GetTeam(Int32.Parse(ddlCurrentTeam.SelectedValue));
                            if (team != null)
                                team.IsInFinal = true;
                        }
                    }
                }
            }

            Models.Team BronzeTeam = teamRepository.GetTeam(Int32.Parse(ddlBronze.SelectedValue));
            if (BronzeTeam != null)
                BronzeTeam.WonBronze = true;

            Models.Team SilverTeam = teamRepository.GetTeam(Int32.Parse(ddlSilver.SelectedValue));
            if (SilverTeam != null)
                SilverTeam.WonSilver = true;

            Models.Team GoldTeam = teamRepository.GetTeam(Int32.Parse(ddlGold.SelectedValue));
            if (GoldTeam != null)
                GoldTeam.WonGold = true;
        }

        protected void SortUsers(object sender, EventArgs e)
        {
            LinkButton thisButton = (LinkButton)sender;
            IQueryable<Models.User> userList = userRepository.GetAllConfirmedUsers();

            switch (thisButton.ID)
            { 
                case "lnkFirstName":
                    userList = userList.OrderBy(u => u.FirstName);
                    break;
                case "lnkLastName":
                    userList = userList.OrderBy(u => u.LastName); 
                    break;
                case "lnkHasPaid":
                    userList = userList.OrderBy(u => u.HasPaid); 
                    break;
                case "lnkPayCode":
                    userList = userList.OrderBy(u => u.PayCode); 
                    break;
                case "lnkDate":
                    userList = userList.OrderBy(u => u.PostedDate); 
                    break;
            }

            rptUsers.DataSource = userList.ToList();
            rptUsers.DataBind();
        }

        protected void SaveUsers(object sender, EventArgs e)
        {
            try
            {
                foreach (RepeaterItem item in rptUsers.Items)
                {
                    CheckBox chkHasPaid = (CheckBox)item.FindControl("chkHasPaid");
                    CheckBox chkIsConfirmed = (CheckBox)item.FindControl("chkIsConfirmed");
                    CheckBox chkIsWinner = (CheckBox)item.FindControl("chkIsWinner");
                    HiddenField hdnUserID = (HiddenField)item.FindControl("hdnUserID");
                    Models.User currentUser = userRepository.GetUser(Int32.Parse(hdnUserID.Value));
                    if (currentUser != null)
                    {
                        currentUser.HasPaid = chkHasPaid.Checked;
                        currentUser.IsConfirmed = chkIsConfirmed.Checked;
                        currentUser.IsWinner = chkIsWinner.Checked;
                    }
                }
                userRepository.Save();
            }
            catch (Exception ex)
            {
                lblError2.Text = "Kunde inte spara användarna " + ex.Message;
            }
        }

        protected void EnableDisableNewEntries(object sender, EventArgs e)
        {
            lblError3.Text = "";
            lblStats.Text = "";
            try
            {
                XDocument xDoc = XDocument.Load(Server.MapPath(@"~/Models/SettingsExtensions.xml"));
                XAttribute xattr = xDoc.Root.Element("EnableNewEntries").Attribute("On");

                if (btnEnableDisableNewEntries.Text.StartsWith("Tillåt"))
                {
                    lblDisableNewEntries.Text = "Systemet tillåter nya anmälningar.";
                    btnEnableDisableNewEntries.Text = "Förbjud nya anmälningar";
                    xattr.Value = "true";
                }
                else
                {
                    lblDisableNewEntries.Text = "Systemet förbjuder nya anmälningar.";
                    btnEnableDisableNewEntries.Text = "Tillåt nya anmälningar";
                    xattr.Value = "false";
                }

                xDoc.Save(Server.MapPath(@"~/Models/SettingsExtensions.xml"));
            }
            catch (Exception ex)
            {
                lblError3.Text = "Kunde inte uppdatera denna inställning. " + ex.Message;
            }
        }

        protected void GenerateStats(object sender, EventArgs e)
        {
            try
            {
                lblError3.Text = "";
                
                int intUserCount = userRepository.GetAllActiveUsers().Count();
                
                // For each match, generate the result percentages
                foreach (Models.Match match in matchRepository.GetAllMatches())
                {
                    double resultCounter = userRepository.CountUserMatchResult(match.ID, "1");
                    match.HomeWinPercent = resultCounter / intUserCount;
                    resultCounter = userRepository.CountUserMatchResult(match.ID, "X");
                    match.DrawPercent = resultCounter / intUserCount;
                    resultCounter = userRepository.CountUserMatchResult(match.ID, "2");
                    match.AwayWinPercent = resultCounter / intUserCount;
                }
                matchRepository.Save();

                // For each team, generate stats by user input
                Models.TeamStats teamStat;

                foreach (Models.Team team in teamRepository.GetAllTeams())
                {
                    if (team.TeamStats == null)
                        teamStat = new Models.TeamStats();
                    else
                        teamStat = team.TeamStats;

                    int intTeamCounter = userRepository.CountUserPlayOffTeams(team.ID);
                    teamStat.PlayoffPercent = (float)intTeamCounter / (float)intUserCount;

                    intTeamCounter = userRepository.CountUserQuarterFinalTeams(team.ID);
                    teamStat.QuarterFinalPercent = (float)intTeamCounter / (float)intUserCount;

                    intTeamCounter = userRepository.CountUserSemiFinalTeams(team.ID);
                    teamStat.SemiFinalPercent = (float)intTeamCounter / (float)intUserCount;

                    intTeamCounter = userRepository.CountUserFinalsTeams(team.ID);
                    teamStat.FinalPercent = (float)intTeamCounter / (float)intUserCount;

                    intTeamCounter = userRepository.CountUserBronzeTeams(team.ID);
                    teamStat.BronzePercent = (float)intTeamCounter / (float)intUserCount;

                    intTeamCounter = userRepository.CountUserSilverTeams(team.ID);
                    teamStat.SilverPercent = (float)intTeamCounter / (float)intUserCount;

                    intTeamCounter = userRepository.CountUserGoldTeams(team.ID);
                    teamStat.GoldPercent = (float)intTeamCounter / (float)intUserCount;

                    if (team.TeamStats == null)
                        team.TeamStats = (teamStat);
                }

                teamRepository.Save();

                IQueryable<Models.TopScorer> topScorers = topScorerRepository.GetAllScorers();
                foreach (Models.TopScorer topScorer in topScorers)
                {
                    topScorer.WinPercent = (float)topScorer.Users.Where(u => u.HasPaid).Count() / (float)intUserCount;
                }

                topScorerRepository.Save();

                lblStats.Text = "Statistiken generarades utan fel.";
            }
            catch (Exception ex)
            {
                lblError3.Text = "Något gick fel när statistiken skulle genereras. " + ex.Message;
            }
        }

        protected void SaveBlogEntry(object sender, EventArgs e)
        {
            lblError4.Text = "";
            try
            {
                Models.BlogEntry blogEntry;

                if (btnSendComment.Text.StartsWith("Upp"))
                    blogEntry = blogRepository.GetBlogEntry(Int32.Parse(hdnBlogEntryID.Value));
                else
                {
                    blogEntry = new Models.BlogEntry();
                    blogRepository.Add(blogEntry);
                    blogEntry.PostedDate = dtUpdateTime;
                }

                blogEntry.Title = txtBlogTitle.Text;
                blogEntry.Text = txtBlogText.Text; //htmlEditor.Content;

                blogRepository.Save();

                btnSendComment.Text = "Lägg till";
                txtBlogTitle.Text = "";
                txtBlogText.Text = "";

                grdBlogEntries.DataSource = blogRepository.GetAllBlogEntries().ToList();
                grdBlogEntries.DataBind();
            }
            catch (Exception ex)
            {
                lblError4.Text = "Kunde inte spara. " + ex.Message;
            }
        }

        protected void grdBlogEntries_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    foreach (Control control in cell.Controls)
                    {
                        ImageButton button = control as ImageButton;
                        if (button != null && button.CommandName == "Delete")
                            button.OnClientClick = "if (!confirm('Är du säker på att du vill radera inlägget?')) return;";
                    }
                }
            }
        }

        protected void EditBlogEntry(object sender, GridViewSelectEventArgs e)
        { 
            //Models.BlogEntry entry = grdBlogEntries.Rows[e.NewSelectedIndex].DataItem as Models.BlogEntry;
            //Models.BlogEntry entry = grdBlogEntries.SelectedRow.DataItem as Models.BlogEntry;

            Models.BlogEntry entry = blogRepository.GetBlogEntry((int)grdBlogEntries.DataKeys[e.NewSelectedIndex].Value);
            txtBlogTitle.Text = entry.Title;
            txtBlogText.Text = entry.Text;
            hdnBlogEntryID.Value = entry.ID.ToString();

            txtBlogTitle.Focus();

            btnSendComment.Text = "Uppdatera";
        }

        protected void DeleteBlogEntry(object sender, GridViewDeleteEventArgs e)
        {
            Models.BlogEntry entry = blogRepository.GetBlogEntry((int)grdBlogEntries.DataKeys[e.RowIndex].Value);

            blogRepository.Delete(entry);
            blogRepository.Save();

            grdBlogEntries.DataSource = blogRepository.GetAllBlogEntries().ToList();
            grdBlogEntries.DataBind();
        }

        protected void SaveMatch(object sender, EventArgs e)
        {
            lblErrorMatch.Text = "";
            try
            {
                Models.Match match;
                if (btnSaveMatch.Text.StartsWith("Upp"))
                    match = matchRepository.GetMatch(Int32.Parse(hdnMatchID.Value));
                else
                {
                    match = new Models.Match();
                    matchRepository.Add(match);
                }

                match.HomeTeamID = Int32.Parse(ddlMatchHomeTeam.SelectedValue);
                match.AwayTeamID =  Int32.Parse(ddlMatchAwayTeam.SelectedValue);
                match.Date = Convert.ToDateTime(txtMatchDate.Text);

                matchRepository.Save();

                btnSaveMatch.Text = "Lägg till";
                ddlMatchHomeTeam.SelectedIndex = 0;
                ddlMatchAwayTeam.SelectedIndex = 0;
                txtMatchDate.Text = "";

                grdMatches.DataSource = matchRepository.GetAllMatches().ToList();
                grdMatches.DataBind();
            }
            catch (Exception ex)
            {
                lblErrorMatch.Text = "Kunde inte spara. " + ex.Message;
            }

        }

        protected void SaveTeam(object sender, EventArgs e)
        {
            lblErrorTeam.Text = "";
            try
            {
                Models.Team team;
                if (btnSaveTeam.Text.StartsWith("Upp"))
                    team = teamRepository.GetTeam(Int32.Parse(hdnTeamID.Value));
                else
                {
                    team = new Models.Team();
                    teamRepository.Add(team);
                }

                team.TeamName = txtTeam.Text;
                team.GroupID = txtTeamGroup.Text;

                teamRepository.Save();

                btnSaveTeam.Text = "Lägg till";
                txtTeam.Text = "";
                txtTeamGroup.Text = "";

                grdTeams.DataSource = teamRepository.GetAllTeams().ToList();
                grdTeams.DataBind();

                SetPlayOffTeams();
            }
            catch (Exception ex)
            {
                lblErrorTeam.Text = "Kunde inte spara. " + ex.Message;
            }

        }

        protected void SaveTopScorer(object sender, EventArgs e)
        {
            lblError5.Text = "";
            try
            {
                Models.TopScorer topScorer;

                if (btnSaveTopScorer.Text.StartsWith("Upp"))
                    topScorer = topScorerRepository.GetTopScorer(Int32.Parse(hdnTopScorerID.Value));
                else
                {
                    topScorer = new Models.TopScorer();
                    topScorerRepository.Add(topScorer);                    
                }

                topScorer.FirstName = txtFirstName.Text;
                topScorer.LastName = txtLastName.Text;
                topScorer.DisplayName = String.Format("{0} {1}", topScorer.FirstName, topScorer.LastName);              

                topScorerRepository.Save();

                btnSaveTopScorer.Text = "Lägg till";
                txtFirstName.Text = "";
                txtLastName.Text = "";

                grdTopScorer.DataSource = topScorerRepository.GetAllScorers().ToList();
                grdTopScorer.DataBind();
            }
            catch (Exception ex)
            {
                lblError5.Text = "Kunde inte spara. " + ex.Message;
            }
        }

        protected void btnBatchImport_OnClick(object sender, EventArgs e)
        {
            lblError5.Text = "";
            try
            {
                Models.TopScorer topScorer;

                foreach(string name in txtTopScorerBatch.Text.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries))
                {
                    topScorer = new Models.TopScorer();
                    topScorerRepository.Add(topScorer);

                    string[] arrName = name.Split(new char[] {' '});
                    for (int i = 0; i < arrName.Length; i++)
                    {
                        if (i == 0)
                            topScorer.FirstName = arrName[i];
                        else
                            topScorer.LastName += arrName[i];
                    }
                    topScorer.DisplayName = String.Format("{0} {1}", topScorer.FirstName, topScorer.LastName);
                }

                topScorerRepository.Save();

                grdTopScorer.DataSource = topScorerRepository.GetAllScorers().ToList();
                grdTopScorer.DataBind();
            }
            catch (Exception ex)
            {
                lblError5.Text = "Kunde inte spara. " + ex.Message;
            }
        }

        protected void grdTopScorer_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    foreach (Control control in cell.Controls)
                    {
                        ImageButton button = control as ImageButton;
                        if (button != null && button.CommandName == "Delete")
                            button.OnClientClick = "if (!confirm('Är du säker på att du vill radera inlägget?')) return;";
                    }
                }
            }
        }

        protected void grdTeams_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    foreach (Control control in cell.Controls)
                    {
                        ImageButton button = control as ImageButton;
                        if (button != null && button.CommandName == "Delete")
                            button.OnClientClick = "if (!confirm('Är du säker på att du vill radera inlägget?')) return;";
                    }
                }
            }
        }

        protected void grdMatches_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    foreach (Control control in cell.Controls)
                    {
                        ImageButton button = control as ImageButton;
                        if (button != null && button.CommandName == "Delete")
                            button.OnClientClick = "if (!confirm('Är du säker på att du vill radera inlägget?')) return;";
                    }
                }
            }
        }

        protected void EditTopScorer(object sender, GridViewSelectEventArgs e)
        {
            //Models.BlogEntry entry = grdBlogEntries.Rows[e.NewSelectedIndex].DataItem as Models.BlogEntry;
            //Models.BlogEntry entry = grdBlogEntries.SelectedRow.DataItem as Models.BlogEntry;

            Models.TopScorer topScorer = topScorerRepository.GetTopScorer((int)grdTopScorer.DataKeys[e.NewSelectedIndex].Value);
            txtFirstName.Text = topScorer.FirstName;
            txtLastName.Text = topScorer.LastName;
            hdnTopScorerID.Value = topScorer.ID.ToString();

            txtFirstName.Focus();

            btnSaveTopScorer.Text = "Uppdatera";
        }

        protected void EditTeam(object sender, GridViewSelectEventArgs e)
        {
            Models.Team team = teamRepository.GetTeam((int)grdTeams.DataKeys[e.NewSelectedIndex].Value);
            txtTeam.Text = team.TeamName;
            txtTeamGroup.Text = team.GroupID;
            hdnTeamID.Value = team.ID.ToString();

            txtTeam.Focus();

            btnSaveTeam.Text = "Uppdatera";
        }

        protected void EditMatch(object sender, GridViewSelectEventArgs e)
        {
            Models.Match match = matchRepository.GetMatch((int)grdMatches.DataKeys[e.NewSelectedIndex].Value);
            ddlMatchHomeTeam.SelectedValue = match.HomeTeamID.ToString();
            ddlMatchAwayTeam.SelectedValue = match.AwayTeamID.ToString();
            txtMatchDate.Text = match.Date.ToString();
            hdnMatchID.Value = match.ID.ToString();

            btnSaveMatch.Text = "Uppdatera";
        }

        protected void DeleteTopScorer(object sender, GridViewDeleteEventArgs e)
        {
            lblError5.Text = "";
            try
            {
                Models.TopScorer topScorer = topScorerRepository.GetTopScorer((int)grdTopScorer.DataKeys[e.RowIndex].Value);

                topScorerRepository.Delete(topScorer);
                topScorerRepository.Save();

                grdTopScorer.DataSource = topScorerRepository.GetAllScorers().ToList();
                grdTopScorer.DataBind();
            }
            catch (Exception ex)
            {
                lblError5.Text = "Kunde inte ta bort. " + ex.Message;
            }
        }

        protected void DeleteTeam(object sender, GridViewDeleteEventArgs e)
        {
            lblErrorTeam.Text = "";
            try
            {
                Models.Team team = teamRepository.GetTeam((int)grdTeams.DataKeys[e.RowIndex].Value);

                teamRepository.Delete(team);
                teamRepository.Save();

                grdTeams.DataSource = teamRepository.GetAllTeams().ToList();
                grdTeams.DataBind();
            }
            catch (Exception ex)
            {
                lblErrorTeam.Text = "Kunde inte ta bort. " + ex.Message;
            }
        }

        protected void DeleteMatch(object sender, GridViewDeleteEventArgs e)
        {
            lblErrorMatch.Text = "";
            try
            {
                Models.Match match = matchRepository.GetMatch((int)grdMatches.DataKeys[e.RowIndex].Value);

                matchRepository.Delete(match);
                matchRepository.Save();

                grdMatches.DataBind();
            }
            catch (Exception ex)
            {
                lblErrorMatch.Text = "Kunde inte ta bort. " + ex.Message;
            }
        }

        protected void Logout(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Response.Redirect("Default.aspx");
        }

        protected void ChangeCSS(object sender, EventArgs e)
        {
            lblError3.Text = "";
            lblStats.Text = "";
            try
            {
                XDocument xDoc = XDocument.Load(Server.MapPath(@"~/Models/SettingsExtensions.xml"));
                XAttribute xattr = xDoc.Root.Element("CSSStyle").Attribute("type");

                xattr.Value = ddlCssStyle.SelectedValue;
                
                xDoc.Save(Server.MapPath(@"~/Models/SettingsExtensions.xml"));

                tabs.ActiveTab = tabOther;
                Server.Transfer(@"~/admin/Default.aspx?tab=other");
            }
            catch (Exception ex)
            {
                lblError3.Text = "Kunde inte uppdatera denna inställning. " + ex.Message;
            }
        }
    }
}
