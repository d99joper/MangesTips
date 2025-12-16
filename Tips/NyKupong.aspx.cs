using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace VMTips_2022
{
    public partial class NyKupong : System.Web.UI.Page
    {
        private Models.User thisUser;
        Models.TeamRepository teamRepository = new VMTips_2022.Models.TeamRepository();
        Models.TopScorerRepository topScorerRepository = new Models.TopScorerRepository();

        protected void Page_Load(object sender, EventArgs e)
        {
            XDocument xDoc = XDocument.Load(Server.MapPath(@"~/Models/SettingsExtensions.xml"));
            XAttribute xattr = xDoc.Root.Element("EnableNewEntries").Attribute("On");
            if (Boolean.Parse(xattr.Value))
            {
                divPassedDeadline.Visible = false;
                wrap.Visible = true;
            }
            else
            {
                divPassedDeadline.Visible = true;
                wrap.Visible = false;
            }

            if (!IsPostBack)
            {
                // UserMatches
                Models.MatchRepository matchRepository = new VMTips_2022.Models.MatchRepository();
                rptUserMatches.DataSource = matchRepository.GetAllMatches().ToList();
                rptUserMatches.DataBind();
                
                // Validate Email
                regEmail.ValidationExpression = Models.User.EmailValidation.emailRegEx.ToString();

                // Set Play-off teams
                SetPlayOffTeams();

                // Set all other team lists
                SetTeamLists();

            }
        }

        protected void btnSubmit_OnClick(object sender, EventArgs e)
        {
            if (ValidPage())
            {
                thisUser = new VMTips_2022.Models.User();

                thisUser.FirstName = txtFirstName.Text;
                thisUser.LastName = txtLastName.Text;
                thisUser.EmailAddress = txtEmail.Text;
                thisUser.PostedDate = DateTime.Now;
                thisUser.DisplayName = String.Format("{0} {1}", thisUser.FirstName, thisUser.LastName);
                thisUser.Guid = System.Guid.NewGuid();

                for (int i = 0; i < rptUserMatches.Items.Count; i++)
                {
                    HiddenField hdnMatchID = (HiddenField)rptUserMatches.Items[i].FindControl("hdnMatchID");
                    TextBox txtHomeGoals = (TextBox)rptUserMatches.Items[i].FindControl("txtHomeGoals");
                    TextBox txtAwayGoals = (TextBox)rptUserMatches.Items[i].FindControl("txtAwayGoals");
                    Models.UserMatch userMatch = new VMTips_2022.Models.UserMatch();
                    userMatch.MatchID = Int32.Parse(hdnMatchID.Value);
                    userMatch.HomeGoals = Byte.Parse(txtHomeGoals.Text);
                    userMatch.AwayGoals = Byte.Parse(txtAwayGoals.Text);
                    if (userMatch.HomeGoals > userMatch.AwayGoals)
                        userMatch.ResultMark = "1";
                    if(userMatch.HomeGoals == userMatch.AwayGoals)
                        userMatch.ResultMark = "X";
                    if (userMatch.HomeGoals < userMatch.AwayGoals)
                        userMatch.ResultMark = "2";
                    thisUser.UserMatches.Add(userMatch);
                }

                AddPlayoffTeams(ref thisUser);
                AddQuarterFinalTeams(ref thisUser);
                AddSemiFinalTeams(ref thisUser);
                AddFinalTeams(ref thisUser);
                AddTopScorer(ref thisUser);

                // Save the user
                try
                {
                    Models.UserRepository userRepository = new Models.UserRepository();

                    userRepository.Add(thisUser);
                    userRepository.Save();
                }
                catch (Exception ex)
                {
                    lblError.Text = "Något gick fel när kupongen skulle sparas.  Kupongen har inte skickats." + ex.ToString();
                    return;
                }

                // Send an email 
                try
                {
                    string strBody = "<p>Tack för din anmälan till Manges VM-tips.</p>";
                    strBody = strBody + "<p>Din kupong är fortfarande inte bekräftad.  Genom att klicka på länken nedan bekräftar du automatiskt din anmälan.</p>";
                    strBody = strBody + String.Format("<p><a href=\"http://mangesvmtips2022.personablesolutions.com/confirm.aspx?id={0}\">http://mangesvmtips2022.personablesolutions.com/confirm.aspx?id={0}</a></p>", thisUser.Guid);
                    Helpers.SendEmail.SendEmail_SMTP("noreply@nodomain.com", "VMTipset", txtEmail.Text, String.Format("{0} {1}", txtFirstName.Text, txtLastName.Text), "Var god bekräfta din anmälan till Manges VM-tips.", strBody);

                    // Redirect to an informational page 
                    Response.Redirect("AwaitingConfirmation.aspx");
                }
                catch (Exception ex)
                {
                    lblError.Text = "Ett fel uppstod när ett epost skulle skickas till dig. Kontakta Mange eller försök igen." + ex.ToString();
                    return;
                }
            }
        }

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

            ddlGruppA_Lag1.DataTextField = "TeamName";
            ddlGruppA_Lag1.DataValueField = "ID";
            ddlGruppA_Lag1.DataBind();
            ddlGruppA_Lag2.DataTextField = "TeamName";
            ddlGruppA_Lag2.DataValueField = "ID";
            ddlGruppA_Lag2.DataBind();
            ddlGruppB_Lag1.DataTextField = "TeamName";
            ddlGruppB_Lag1.DataValueField = "ID";
            ddlGruppB_Lag1.DataBind();
            ddlGruppB_Lag2.DataTextField = "TeamName";
            ddlGruppB_Lag2.DataValueField = "ID";
            ddlGruppB_Lag2.DataBind();
            ddlGruppC_Lag1.DataTextField = "TeamName";
            ddlGruppC_Lag1.DataValueField = "ID";
            ddlGruppC_Lag1.DataBind();
            ddlGruppC_Lag2.DataTextField = "TeamName";
            ddlGruppC_Lag2.DataValueField = "ID";
            ddlGruppC_Lag2.DataBind();
            ddlGruppD_Lag1.DataTextField = "TeamName";
            ddlGruppD_Lag1.DataValueField = "ID";
            ddlGruppD_Lag1.DataBind();
            ddlGruppD_Lag2.DataTextField = "TeamName";
            ddlGruppD_Lag2.DataValueField = "ID";
            ddlGruppD_Lag2.DataBind();
            ddlGruppE_Lag1.DataTextField = "TeamName";
            ddlGruppE_Lag1.DataValueField = "ID";
            ddlGruppE_Lag1.DataBind();
            ddlGruppE_Lag2.DataTextField = "TeamName";
            ddlGruppE_Lag2.DataValueField = "ID";
            ddlGruppE_Lag2.DataBind();
            ddlGruppF_Lag1.DataTextField = "TeamName";
            ddlGruppF_Lag1.DataValueField = "ID";
            ddlGruppF_Lag1.DataBind();
            ddlGruppF_Lag2.DataTextField = "TeamName";
            ddlGruppF_Lag2.DataValueField = "ID";
            ddlGruppF_Lag2.DataBind();
            ddlGruppG_Lag1.DataTextField = "TeamName";
            ddlGruppG_Lag1.DataValueField = "ID";
            ddlGruppG_Lag1.DataBind();
            ddlGruppG_Lag2.DataTextField = "TeamName";
            ddlGruppG_Lag2.DataValueField = "ID";
            ddlGruppG_Lag2.DataBind();
            ddlGruppH_Lag1.DataTextField = "TeamName";
            ddlGruppH_Lag1.DataValueField = "ID";
            ddlGruppH_Lag1.DataBind();
            ddlGruppH_Lag2.DataTextField = "TeamName";
            ddlGruppH_Lag2.DataValueField = "ID";
            ddlGruppH_Lag2.DataBind();
        }

        private void SetTeamLists()
        {
            List<Models.Team> teamList = teamRepository.GetAllTeams().ToList();
            ddlKvart1.DataSource = teamList;
            ddlKvart1.DataBind();
            ddlKvart2.DataSource = teamList;
            ddlKvart2.DataBind();
            ddlKvart3.DataSource = teamList;
            ddlKvart3.DataBind();
            ddlKvart4.DataSource = teamList;
            ddlKvart4.DataBind();
            ddlKvart5.DataSource = teamList;
            ddlKvart5.DataBind();
            ddlKvart6.DataSource = teamList;
            ddlKvart6.DataBind();
            ddlKvart7.DataSource = teamList;
            ddlKvart7.DataBind();
            ddlKvart8.DataSource = teamList;
            ddlKvart8.DataBind();

            ddlSemi1.DataSource = teamList;
            ddlSemi1.DataBind();
            ddlSemi2.DataSource = teamList;
            ddlSemi2.DataBind();
            ddlSemi3.DataSource = teamList;
            ddlSemi3.DataBind();
            ddlSemi4.DataSource = teamList;
            ddlSemi4.DataBind();

            ddlFinal1.DataSource = teamList;
            ddlFinal1.DataBind();
            ddlFinal2.DataSource = teamList;
            ddlFinal2.DataBind();

            ddlBronze.DataSource = teamList;
            ddlBronze.DataBind();

            ddlSilver.DataSource = teamList;
            ddlSilver.DataBind();

            ddlGold.DataSource = teamList;
            ddlGold.DataBind();
            
        }

        private bool ValidPage()
        {
            bool retVal = true;
            
            // Hide all controls
            lblErrorQuarter.Visible = false;
            lblErrorSemi.Visible = false;
            lblErrorFinal.Visible = false;
            lblError.Text = "";
            lblErrorScorer.Text = "";

            retVal = ValidateSection(divFinalTeams, lblErrorFinal);
            retVal = ValidateSection(divSemiFinalTeams, lblErrorSemi) && retVal;
            retVal = ValidateSection(divQuarterFinalTeams, lblErrorQuarter) && retVal;

            Models.TopScorer ts = topScorerRepository.GetTopScorer(txtTopScorer.Text);
            if (ts == null)
            {
                lblErrorScorer.Text = "* Namnet du angett är inte giltigt.";
                retVal = false;
            }

            return retVal;
        }

        private bool ValidateSection(Control startControl, Label lblErrorSection)
        {
            DropDownList ddlCurrentTeam;
            List<int> teamIDList = new List<int>();

            foreach (Control c in startControl.Controls)
            {
                if (c.ID != null)
                {
                    if (c.ID.Contains("ddl"))
                    {
                        ddlCurrentTeam = (DropDownList)c;
                        int currentTeamID = Int32.Parse(ddlCurrentTeam.SelectedValue);
                        if (teamIDList.Contains(currentTeamID))
                        {
                            lblErrorSection.Visible = true;
                            ddlCurrentTeam.Focus();
                            return false;
                        }
                        else
                            teamIDList.Add(currentTeamID);
                    }
                }
            }

            return true;
        }

        private void AddPlayoffTeams(ref Models.User thisUser)
        {
            DropDownList ddlCurrentTeam;

            foreach (Control c in divPlayoffTeams.Controls)
            {
                try
                {
                    if (c.ID != null)
                    {
                        if (c.ID.StartsWith("ddl"))
                        {
                            ddlCurrentTeam = (DropDownList)c;
                            Models.UserPlayoffTeam playoffTeam = new Models.UserPlayoffTeam();                            

                            playoffTeam.TeamID = Int32.Parse(ddlCurrentTeam.SelectedValue);
                            if (c.ID.EndsWith("1"))
                                playoffTeam.Position = 1;
                            else
                            {
                                playoffTeam.Position = 2;
                                // Add a placeholder for the bonuspoint
                                Models.BonusPoints bp = new Models.BonusPoints();
                                Models.Team thisTeam = teamRepository.GetTeam(Int32.Parse(ddlCurrentTeam.SelectedValue));
                                bp.GroupID = thisTeam.GroupID;
                                thisUser.BonusPoints.Add(bp);
                            }
                            playoffTeam.Points = 0;
                            thisUser.UserPlayoffTeams.Add(playoffTeam);
                        }
                    }
                }
                catch(Exception e)
                {
                    lblError.Text = "Fel när slutspelslag skulle sparas. " + e.Message;
                    throw new Exception();
                }
            }
        }

        private void AddQuarterFinalTeams(ref Models.User thisUser)
        {
            DropDownList ddlCurrentTeam;

            foreach (Control c in divQuarterFinalTeams.Controls)
            {
                if (c.ID != null)
                {
                    if (c.ID.StartsWith("ddl"))
                    {
                        ddlCurrentTeam = (DropDownList)c;
                        Models.UserQFTeam quarterTeam = new Models.UserQFTeam();
                        quarterTeam.TeamID = Int32.Parse(ddlCurrentTeam.SelectedValue);
                        quarterTeam.Points = 0;
                        thisUser.UserQFTeams.Add(quarterTeam);
                    }
                }
            }
        }

        private void AddSemiFinalTeams(ref Models.User thisUser)
        {
            DropDownList ddlCurrentTeam;

            foreach (Control c in divSemiFinalTeams.Controls)
            {
                if (c.ID != null)
                {
                    if (c.ID.StartsWith("ddl"))
                    {
                        ddlCurrentTeam = (DropDownList)c;
                        Models.UserSFTeam semiTeam = new Models.UserSFTeam();
                        semiTeam.TeamID = Int32.Parse(ddlCurrentTeam.SelectedValue);
                        semiTeam.Points = 0;
                        thisUser.UserSFTeams.Add(semiTeam);
                    }
                }
            }
        }

        private void AddFinalTeams(ref Models.User thisUser)
        {
            DropDownList ddlCurrentTeam;

            foreach (Control c in divFinalTeams.Controls)
            {
                if (c.ID != null)
                {
                    if (c.ID.StartsWith("ddl"))
                    {
                        ddlCurrentTeam = (DropDownList)c;
                        Models.UserFinalTeam finalTeam = new Models.UserFinalTeam();
                        finalTeam.TeamID = Int32.Parse(ddlCurrentTeam.SelectedValue);
                        finalTeam.Points = 0;
                        thisUser.UserFinalTeams.Add(finalTeam);
                    }
                }
            }

            Models.UserBronzeTeam BronzeTeam = new Models.UserBronzeTeam();
            BronzeTeam.TeamID = Int32.Parse(ddlBronze.SelectedValue);
            BronzeTeam.Points = 0;
            thisUser.UserBronzeTeam.Add(BronzeTeam);

            Models.UserSilverTeam SilverTeam = new Models.UserSilverTeam();
            SilverTeam.TeamID = Int32.Parse(ddlSilver.SelectedValue);
            SilverTeam.Points = 0;
            thisUser.UserSilverTeam.Add(SilverTeam);

            Models.UserGoldTeam GoldTeam = new Models.UserGoldTeam();
            GoldTeam.TeamID = Int32.Parse(ddlGold.SelectedValue);
            GoldTeam.Points = 0;
            thisUser.UserGoldTeam.Add(GoldTeam);

        }

        private void AddTopScorer(ref VMTips_2022.Models.User thisUser)
        {
            Models.TopScorer topScorer = topScorerRepository.GetTopScorer(txtTopScorer.Text);
            thisUser.TopScorerID = topScorer.ID;
        }
    }
}
