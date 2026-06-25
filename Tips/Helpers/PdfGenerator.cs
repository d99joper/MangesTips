using System;
using System.IO;
using System.Linq;
using ceTe.DynamicPDF;
using ceTe.DynamicPDF.PageElements;
using Microsoft.AspNetCore.Hosting;
using Tipset.Models;

namespace Tipset.Helpers
{
    public static class PdfGenerator
    {
        public static byte[] RenderCompletePDF(User currentUser, IWebHostEnvironment env)
        {
            Document document = new Document();
            document.Author = "Jonas Persson";
            document.Title = "Manges VM-tips";

            Page page = new Page(PageSize.A4, PageOrientation.Portrait, 54.0f);

            page.Elements.Add(new Label("Manges VM-tips", 0, -20, 500, 10, Font.HelveticaBold, 16, TextAlign.Center));
            page.Elements.Add(new Label(currentUser.DisplayName, 2, 10, 300, 10, Font.Helvetica, 12));

            // 1. Tipsraden
            page.Elements.Add(new Label("1. Tipsraden", 0, 40, 150, 5, Font.Helvetica, 11));
            int i = 0;
            foreach (UserMatch um in currentUser.UserMatches)
            {
                page.Elements.Add(new Label(String.Format("{0:dd/M}", um.Match.Date),                   2,   55 + i, 25, 5, Font.Helvetica, 11, TextAlign.Left));
                page.Elements.Add(new Label(um.Match.HomeTeam.GroupID.ToString(),                       27,  55 + i, 15, 5, Font.Helvetica, 11, TextAlign.Left));
                page.Elements.Add(new Label(um.Match.HomeTeam.TeamName,                                 40,  55 + i, 85, 5, Font.Helvetica, 11, TextAlign.Left));
                page.Elements.Add(new Label(" - ",                                                      127, 55 + i, 15, 5, Font.Helvetica, 11, TextAlign.Center));
                page.Elements.Add(new Label(um.Match.AwayTeam.TeamName,                                 142, 55 + i, 85, 5, Font.Helvetica, 11, TextAlign.Left));
                page.Elements.Add(new Label(String.Format("{0}-{1}", um.HomeGoals, um.AwayGoals),       227, 55 + i, 50, 5, Font.Helvetica, 11, TextAlign.Left));
                i += 14;
            }

            // Logo — placed in the right column (x=260), capped so it doesn't bleed into
            // section 2 (starts at y≈406) or outside the right page edge (available width ≈227pt).
            // logo20262color.gif: 538×650px @96dpi → 403×488pt natural → scale 0.5 → 202×244pt ✓
            // loggo2018.gif:      200×433px @96dpi → 150×325pt natural → scale 0.8 → 120×260pt ✓
            string imagePath = System.IO.Path.Combine(env.WebRootPath ?? env.ContentRootPath, "images", "logo20262color.gif");
            float imageScale = 0.5f;
            if (System.IO.File.Exists(imagePath))
                page.Elements.Add(new Image(imagePath, 260, 30, imageScale));

            // 2. Vidare från gruppen
            float x = 260;
            float y = 420;
            page.Elements.Add(new Label("2. Vidare från gruppen", x, y - 14, 150, 5, Font.Helvetica, 11));
            page.Elements.Add(new Label("Grupp A", x,          y, 60, 5, Font.Helvetica, 11, TextAlign.Center));
            page.Elements.Add(new Label("Grupp B", x + 60,     y, 60, 5, Font.Helvetica, 11, TextAlign.Center));
            page.Elements.Add(new Label("Grupp C", x + 60 * 2, y, 60, 5, Font.Helvetica, 11, TextAlign.Center));
            page.Elements.Add(new Label("Grupp D", x + 60 * 3, y, 60, 5, Font.Helvetica, 11, TextAlign.Center));
            y += 48;
            page.Elements.Add(new Label("Grupp E", x,          y, 60, 5, Font.Helvetica, 11, TextAlign.Center));
            page.Elements.Add(new Label("Grupp F", x + 60,     y, 60, 5, Font.Helvetica, 11, TextAlign.Center));
            page.Elements.Add(new Label("Grupp G", x + 60 * 2, y, 60, 5, Font.Helvetica, 11, TextAlign.Center));
            page.Elements.Add(new Label("Grupp I", x + 60 * 3, y, 60, 5, Font.Helvetica, 11, TextAlign.Center));

            i = 0;
            foreach (UserPlayoffTeam up in currentUser.UserPlayoffTeams.OrderBy(u => u.Team.GroupID).ThenBy(u => u.Position))
            {
                string teamName = up.Team.TeamName.Length > 10 ? up.Team.TeamName.Substring(0, 9) + "." : up.Team.TeamName;
                var lblTeam = new Label(teamName, 5, 0, 65, 5, Font.Helvetica, 11, TextAlign.Left);

                lblTeam.Y = (i % 2 == 0) ? y - 34 : y - 20;

                switch (up.Team.GroupID)
                {
                    case "A": case "E": lblTeam.X = x;          break;
                    case "B": case "F": lblTeam.X = x + 60;     break;
                    case "C": case "G": lblTeam.X = x + 60 * 2; break;
                    case "D": case "H": case "I": lblTeam.X = x + 60 * 3; break;
                }
                if (up.Team.GroupID == "E" || up.Team.GroupID == "F" ||
                    up.Team.GroupID == "G" || up.Team.GroupID == "H" || up.Team.GroupID == "I")
                    lblTeam.Y += 48;

                page.Elements.Add(lblTeam);
                i++;
            }

            // 3. Kvartsfinal
            i = 0;
            y += 50;
            page.Elements.Add(new Label("3. Vilka åtta lag går till kvartsfinal?", x, y, 300, 5, Font.Helvetica, 11));
            foreach (UserQFTeam uqf in currentUser.UserQFTeams)
            {
                var lblTeam = new Label(uqf.Team.TeamName, x, y + 14, 65, 5, Font.Helvetica, 11, TextAlign.Left);
                lblTeam.X += (i % 4) * 65;
                if (i > 3) lblTeam.Y += 15;
                page.Elements.Add(lblTeam);
                i++;
            }

            // 4. Semifinal
            i = 0;
            y += 50;
            page.Elements.Add(new Label("4. Vilka fyra lag går till semifinal?", x, y, 300, 5, Font.Helvetica, 11));
            foreach (UserSFTeam usf in currentUser.UserSFTeams)
            {
                var lblTeam = new Label(usf.Team.TeamName, x, y + 14, 75, 5, Font.Helvetica, 11, TextAlign.Left);
                lblTeam.X += i * 65;
                page.Elements.Add(lblTeam);
                i++;
            }

            // 5. Final
            i = 0;
            y += 34;
            page.Elements.Add(new Label("5. Vilka två lag går till final?", x, y, 300, 5, Font.Helvetica, 11));
            foreach (UserFinalTeam uft in currentUser.UserFinalTeams)
            {
                var lblTeam = new Label(uft.Team.TeamName, x, y + 14, 75, 5, Font.Helvetica, 11, TextAlign.Left);
                lblTeam.X += i * 65;
                page.Elements.Add(lblTeam);
                i++;
            }

            // 6. Skyttekung
            y += 35;
            page.Elements.Add(new Label("6. Skyttekung: ", x, y, 70, 5, Font.Helvetica, 11));
            if (currentUser.TopScorer?.DisplayName != null)
                page.Elements.Add(new Label(currentUser.TopScorer.DisplayName, x + 75, y, 150, 5, Font.Helvetica, 11, TextAlign.Left));

            // 7. Brons
            y += 25;
            page.Elements.Add(new Label("7. Brons: ", x, y, 50, 5, Font.Helvetica, 11));
            page.Elements.Add(new Label(currentUser.UserBronzeTeam.ElementAt(0).Team.TeamName, x + 55, y, 75, 5, Font.Helvetica, 11, TextAlign.Left));

            // 8. Silver
            y += 25;
            page.Elements.Add(new Label("8. Silver: ", x, y, 50, 5, Font.Helvetica, 11));
            page.Elements.Add(new Label(currentUser.UserSilverTeam.ElementAt(0).Team.TeamName, x + 55, y, 75, 5, Font.Helvetica, 11, TextAlign.Left));

            // 9. Guld
            y += 25;
            page.Elements.Add(new Label("9. Guld: ", x, y, 50, 5, Font.Helvetica, 11));
            page.Elements.Add(new Label(currentUser.UserGoldTeam.ElementAt(0).Team.TeamName, x + 55, y, 75, 5, Font.Helvetica, 11, TextAlign.Left));

            document.Pages.Add(page);

            return document.Draw();
        }

        public static byte[] RenderErrorPDF(string message)
        {
            Document document = new Document();
            document.Author = "Jonas Persson";
            document.Title = "Manges VM-tips";

            Page page = new Page(PageSize.A4, PageOrientation.Portrait, 54.0f);
            page.Elements.Add(new Label(message, 0, 0, 504, 100, Font.Helvetica, 18, TextAlign.Center));
            document.Pages.Add(page);

            return document.Draw();
        }
    }
}
