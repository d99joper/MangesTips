using System;
using Tips.Models;
using System.Linq;
using ceTe.DynamicPDF;
using ceTe.DynamicPDF.PageElements;

namespace Tips
{
    public partial class pdfGenerator : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            Models.User thisUser;

            Label lblTest = new Label("PDF rocks!", 0, 0, 504, 100, Font.Helvetica, 18, TextAlign.Center );

            try
            {
                string strGuid = Request.QueryString["id"].ToString();
                Guid thisGuid = new Guid(strGuid);
                try
                {
                    Models.UserRepository userRepository = new Models.UserRepository();
                    thisUser = userRepository.GetUser(thisGuid);

                    if (thisUser != null)//(thisUser.IsConfirmed)
                    {
                        RenderCompletePDF(thisUser);
                    }
                    else
                    {
                        lblTest.Text = "Din kupong kunde inte hittas.";
                        RenderErrorPDF(lblTest);
                    }
                }
                catch
                {
                    lblTest.Text = "Din kupong kunde inte hittas.";
                    RenderErrorPDF(lblTest);
                }
            }
            catch
            {
                lblTest.Text = "Felaktigt anrop till denna sida.";
                RenderErrorPDF(lblTest);
            }

        }

        private void RenderCompletePDF(User currentUser)
        {
            Document document = new Document();
            document.Author = "Jonas Persson";
            document.Title = "Manges VM-tips";

            Page page = new Page(PageSize.A4, PageOrientation.Portrait, 54.0f);

            Label lblTitle = new Label("Manges VM-tips", 0, -20, 500, 10, Font.HelveticaBold, 16, TextAlign.Center);
            Label lblName = new Label(currentUser.DisplayName, 2, 10, 300, 10, Font.Helvetica, 12);
            page.Elements.Add(lblTitle);
            page.Elements.Add(lblName);

            // render the entire user coupon

            // add user matches
            Label lblUserMatches = new Label("1. Tipsraden", 0, 40, 150, 5, Font.Helvetica, 11);
            page.Elements.Add(lblUserMatches);
            int i = 0;
            foreach (UserMatch um in currentUser.UserMatches)
            {
                Label lblDate = new Label(String.Format("{0:dd/M}", um.Match.Date), 2, 55 + i, 25, 5, Font.Helvetica, 11, TextAlign.Left);
                Label lblGroup = new Label(um.Match.HomeTeam.GroupID.ToString(), 27, 55 + i, 15, 5, Font.Helvetica, 11, TextAlign.Left);
                Label lblHomeTeam = new Label(um.Match.HomeTeam.TeamName, 40, 55 + i, 85, 5, Font.Helvetica, 11, TextAlign.Left);
                Label lblSeperator = new Label(" - ", 127, 55 + i, 15, 5, Font.Helvetica, 11, TextAlign.Center);
                Label lblAwayTeam = new Label(um.Match.AwayTeam.TeamName, 142, 55 + i, 85, 5, Font.Helvetica, 11, TextAlign.Left);
                Label lblResult = new Label(String.Format("{0}-{1}", um.HomeGoals, um.AwayGoals), 227, 55 + i, 50, 5, Font.Helvetica, 11, TextAlign.Left);
                
                page.Elements.Add(lblDate);
                page.Elements.Add(lblGroup);
                page.Elements.Add(lblHomeTeam);
                page.Elements.Add(lblSeperator);
                page.Elements.Add(lblAwayTeam);
                page.Elements.Add(lblResult);
                
                i += 14;
            }

            Image image = new Image(Server.MapPath(@"images\loggo2018.gif"), 260, 30, 0.8f);
            //Image image = new Image(Server.MapPath(@"images\mangesvmtips.gif"), 260, 30, 0.8f);
            page.Elements.Add(image);

            // Add the play off teams
            float x = 260;
            float y = 420;
            Label lblPlayOffTitle = new Label("2. Vidare från gruppen", x, y-14, 150, 5, Font.Helvetica, 11);
            page.Elements.Add(lblPlayOffTitle);
            Label lblGroupA = new Label("Grupp A", x , y, 60, 5, Font.Helvetica, 11, TextAlign.Center);
            page.Elements.Add(lblGroupA);
            Label lblGroupB = new Label("Grupp B", x + 60, y, 60, 5, Font.Helvetica, 11, TextAlign.Center);
            page.Elements.Add(lblGroupB);
            Label lblGroupC = new Label("Grupp C", x + 60 * 2, y, 60, 5, Font.Helvetica, 11, TextAlign.Center);
            page.Elements.Add(lblGroupC);
            Label lblGroupD = new Label("Grupp D", x + 60 * 3, y, 60, 5, Font.Helvetica, 11, TextAlign.Center);
            page.Elements.Add(lblGroupD);
            y += 48;
            Label lblGroupE = new Label("Grupp E", x, y, 60, 5, Font.Helvetica, 11, TextAlign.Center);
            page.Elements.Add(lblGroupE);
            Label lblGroupF = new Label("Grupp F", x + 60, y, 60, 5, Font.Helvetica, 11, TextAlign.Center);
            page.Elements.Add(lblGroupF);
            Label lblGroupG = new Label("Grupp G", x + 60 * 2, y, 60, 5, Font.Helvetica, 11, TextAlign.Center);
            page.Elements.Add(lblGroupG);
            Label lblGroupH = new Label("Grupp H", x + 60 * 3, y, 60, 5, Font.Helvetica, 11, TextAlign.Center);
            page.Elements.Add(lblGroupH);
            i = 0;
            foreach (UserPlayoffTeam up in currentUser.UserPlayoffTeams.OrderBy(u => u.Team.GroupID).ThenBy(u => u.Position))
            {
                string strTeamName = up.Team.TeamName.Length > 10 ? up.Team.TeamName.Substring(0, 9) + "." : up.Team.TeamName;
                Label lblTeam = new Label(strTeamName, 5, 0, 65, 5, Font.Helvetica, 11, TextAlign.Left);

                if(i%2 == 0)
                    lblTeam.Y = y - 34;
                else
                    lblTeam.Y = y - 20;

                if (up.Team.GroupID == "A" || up.Team.GroupID == "E")
                    lblTeam.X = x;
                if (up.Team.GroupID == "B" || up.Team.GroupID == "F")
                    lblTeam.X = x + 60;
                if (up.Team.GroupID == "C" || up.Team.GroupID == "G")
                    lblTeam.X = x + 60 * 2;
                if (up.Team.GroupID == "D" || up.Team.GroupID == "H")
                    lblTeam.X = x + 60 * 3;
                if (up.Team.GroupID == "E" || up.Team.GroupID == "F" || up.Team.GroupID == "G" || up.Team.GroupID == "H")
                    lblTeam.Y += 48;

                page.Elements.Add(lblTeam);
                i++;
            }

            i = 0;
            y += 50;
            Label lblQFTitle = new Label("3. Vilka åtta lag går till kvartsfinal?", x, y, 300, 5, Font.Helvetica, 11);
            page.Elements.Add(lblQFTitle);
            foreach (UserQFTeam uqf in currentUser.UserQFTeams)
            {
                Label lblTeam = new Label(uqf.Team.TeamName, x, y + 14, 65, 5, Font.Helvetica, 11, TextAlign.Left);

                lblTeam.X += i % 4 * 65;

                if (i > 3)
                    lblTeam.Y += 15;

                page.Elements.Add(lblTeam);
                i++;
            }

            i = 0;
            y += 50;
            Label lblSFTitle = new Label("4. Vilka fyra lag går till semifinal?", x, y, 300, 5, Font.Helvetica, 11);
            page.Elements.Add(lblSFTitle);
            foreach (UserSFTeam uqf in currentUser.UserSFTeams)
            {
                Label lblTeam = new Label(uqf.Team.TeamName, x, y + 14, 75, 5, Font.Helvetica, 11, TextAlign.Left);

                lblTeam.X += i * 65;

                //if (i > 1)
                //    lblTeam.Y += 15;

                page.Elements.Add(lblTeam);
                i++;
            }

            i = 0;
            y += 34;
            Label lblFinalTitle = new Label("5. Vilka två lag går till final?", x, y, 300, 5, Font.Helvetica, 11);
            page.Elements.Add(lblFinalTitle);
            foreach (UserFinalTeam uqf in currentUser.UserFinalTeams)
            {
                Label lblTeam = new Label(uqf.Team.TeamName, x, y + 14, 75, 5, Font.Helvetica, 11, TextAlign.Left);

                lblTeam.X += i * 65;

                page.Elements.Add(lblTeam);
                i++;
            }

            y += 35;
            Label lblTopScorerTitle = new Label("6. Skyttekung: ", x, y, 70, 5, Font.Helvetica, 11);
            page.Elements.Add(lblTopScorerTitle);
            if (currentUser.TopScorer.DisplayName != null)
            {
                Label lblTopScorerName = new Label(currentUser.TopScorer.DisplayName, x + 75, y, 150, 5, Font.Helvetica, 11, TextAlign.Left);
                page.Elements.Add(lblTopScorerName);
            }

            y += 25;
            Label lblBronzeTitle = new Label("7. Brons: ", x, y, 50, 5, Font.Helvetica, 11);
            page.Elements.Add(lblBronzeTitle);
            Label lblBronzeTeam = new Label(currentUser.UserBronzeTeam.ElementAt(0).Team.TeamName, x + 55, y, 75, 5, Font.Helvetica, 11, TextAlign.Left);
            page.Elements.Add(lblBronzeTeam);

            y += 25;
            Label lblSilverTitle = new Label("8. Silver: ", x, y, 50, 5, Font.Helvetica, 11);
            page.Elements.Add(lblSilverTitle);
            Label lblSilverTeam = new Label(currentUser.UserSilverTeam.ElementAt(0).Team.TeamName, x + 55, y, 75, 5, Font.Helvetica, 11, TextAlign.Left);
            page.Elements.Add(lblSilverTeam);

            y += 25;
            Label lblGoldTitle = new Label("9. Guld: ", x, y, 50, 5, Font.Helvetica, 11);
            page.Elements.Add(lblGoldTitle);
            Label lblGoldTeam = new Label(currentUser.UserGoldTeam.ElementAt(0).Team.TeamName, x + 55, y, 75, 5, Font.Helvetica, 11, TextAlign.Left);
            page.Elements.Add(lblGoldTeam);

            document.Pages.Add(page);

            document.DrawToWeb(String.Format("{0} Manges VM-tips", currentUser.DisplayName + ".pdf"));
        }

        private void RenderErrorPDF(Label lblError)
        {
            Document document = new Document();
            document.Author = "Jonas Persson";
            document.Title = "Manges VM-tips";

            Page page = new Page(PageSize.A4, PageOrientation.Portrait, 54.0f);

            page.Elements.Add(lblError);
            
            document.Pages.Add(page);

            document.DrawToWeb(String.Format("{0} Manges VM-tips", ""));
        }
    }
}
