using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tips
{
    public partial class Answers : System.Web.UI.Page
    {
        Models.TeamRepository teamRepository = new Tips.Models.TeamRepository();
        Models.MatchRepository matchRepository = new Tips.Models.MatchRepository();
        Models.UserRepository userRepository = new Tips.Models.UserRepository();
        private DateTime dtUpdateTime = DateTime.Now;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // UserMatches
                rptMatches.DataSource = matchRepository.GetAllMatches().ToList();
                rptMatches.DataBind();

                // Set Play-off teams
                SetPlayOffTeams();

                // Set all other team lists
                SetTeamLists();

                // Set the top scorer
                SetTopScorer();
            }
        }

        private void SetTopScorer()
        {
            Models.TopScorerRepository topScorerRepository = new Models.TopScorerRepository();
            List<Models.TopScorer> topScorers = topScorerRepository.GetWinner();
            foreach (Models.TopScorer topScorer in topScorers)
            {
                if (topScorer != null)
                    lblTopScorer.Text += topScorer.DisplayName +"<br />";
            }
        }

        private void SetPlayOffTeams()
        {
            Models.Team playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "A", 1);
            if (playoffTeam != null)
                ddlGruppA_Lag1.Text = playoffTeam.TeamName;
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "A", 2);
            if (playoffTeam != null)
                ddlGruppA_Lag2.Text = playoffTeam.TeamName;
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "B", 1);
            if (playoffTeam != null)
                ddlGruppB_Lag1.Text = playoffTeam.TeamName;
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "B", 2);
            if (playoffTeam != null)
                ddlGruppB_Lag2.Text = playoffTeam.TeamName;
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "C", 1);
            if (playoffTeam != null)
                ddlGruppC_Lag1.Text = playoffTeam.TeamName;
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "C", 2);
            if (playoffTeam != null)
                ddlGruppC_Lag2.Text = playoffTeam.TeamName;
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "D", 1);
            if (playoffTeam != null)
                ddlGruppD_Lag1.Text = playoffTeam.TeamName;
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "D", 2);
            if (playoffTeam != null)
                ddlGruppD_Lag2.Text = playoffTeam.TeamName;
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "E", 1);
            if (playoffTeam != null)
                ddlGruppE_Lag1.Text = playoffTeam.TeamName;
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "E", 2);
            if (playoffTeam != null)
                ddlGruppE_Lag2.Text = playoffTeam.TeamName;
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "F", 1);
            if (playoffTeam != null)
                ddlGruppF_Lag1.Text = playoffTeam.TeamName;
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "F", 2);
            if (playoffTeam != null)
                ddlGruppF_Lag2.Text = playoffTeam.TeamName;
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "G", 1);
            if (playoffTeam != null)
                ddlGruppG_Lag1.Text = playoffTeam.TeamName;
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "G", 2);
            if (playoffTeam != null)
                ddlGruppG_Lag2.Text = playoffTeam.TeamName;
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "H", 1);
            if (playoffTeam != null)
                ddlGruppH_Lag1.Text = playoffTeam.TeamName;
            playoffTeam = teamRepository.GetTeam(Models.TeamRepository.TeamInqueryType.isInPlayoffs, "H", 2);
            if (playoffTeam != null)
                ddlGruppH_Lag2.Text = playoffTeam.TeamName;
        }

        private void SetTeamLists()
        {
            List<Models.Team> teamList = teamRepository.GetTeams(Models.TeamRepository.TeamInqueryType.isInQuarterFinals);
            for (int i = 0; i < teamList.Count; i++)
            {
                Label ddlKvart = (Label)divQuarterFinalTeams.FindControl(String.Format("ddlKvart{0}", i + 1));
                ddlKvart.Text = teamList[i].TeamName;
            }

            teamList = teamRepository.GetTeams(Models.TeamRepository.TeamInqueryType.isInSemiFinals);
            for (int i = 0; i < teamList.Count; i++)
            {
                Label ddlSemi = (Label)divSemiFinalTeams.FindControl(String.Format("ddlSemi{0}", i + 1));
                ddlSemi.Text = teamList[i].TeamName;
            }

            teamList = teamRepository.GetTeams(Models.TeamRepository.TeamInqueryType.isInFinals);
            for (int i = 0; i < teamList.Count; i++)
            {
                Label ddlFinal = (Label)divFinalTeams.FindControl(String.Format("ddlFinal{0}", i + 1));
                ddlFinal.Text = teamList[i].TeamName;
            }

            teamList = teamRepository.GetTeams(Models.TeamRepository.TeamInqueryType.WonBronze);
            if (teamList.Count > 0)
                ddlBronze.Text = teamList[0].TeamName;

            teamList = teamRepository.GetTeams(Models.TeamRepository.TeamInqueryType.WonSilver);
            if (teamList.Count > 0)
                ddlSilver.Text = teamList[0].TeamName;

            teamList = teamRepository.GetTeams(Models.TeamRepository.TeamInqueryType.WonGold);
            if (teamList.Count > 0)
                ddlGold.Text = teamList[0].TeamName;
        }
    }
}
