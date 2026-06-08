using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Tipset.Helpers;
using Tipset.Models;
using Tipset.ViewModels;

namespace Tipset.Controllers
{
    public class DetailsController : BaseController
    {
        private readonly UserRepository _userRepository;
        private readonly TopScorerRepository _topScorerRepository;
        private readonly IWebHostEnvironment _env;

        public DetailsController(UserRepository userRepository, TopScorerRepository topScorerRepository,
            IWebHostEnvironment env, SettingsRepository settingsRepo)
            : base(settingsRepo)
        {
            _userRepository = userRepository;
            _topScorerRepository = topScorerRepository;
            _env = env;
        }

        [Route("Details/pdf/{guid}")]
        public ActionResult Pdf(Guid? guid)
        {
            try
            {
                if (guid == null)
                    return File(PdfGenerator.RenderErrorPDF("Din kupong kunde inte hittas."), "application/pdf", "Manges VM-tips.pdf");

                User currentUser = _userRepository.GetUser(guid.Value);
                if (currentUser == null)
                    return File(PdfGenerator.RenderErrorPDF("Din kupong kunde inte hittas."), "application/pdf", "Manges VM-tips.pdf");

                byte[] pdfBytes = PdfGenerator.RenderCompletePDF(currentUser, _env);
                return File(pdfBytes, "application/pdf", currentUser.DisplayName + " Manges VM-tips.pdf");
            }
            catch (Exception ex)
            {
                return File(PdfGenerator.RenderErrorPDF("Din kupong kunde inte hittas. " + ex.Message), "application/pdf", "Manges VM-tips.pdf");
            }
        }

        public ActionResult Index(int id)
        {
            var vm = new DetailsViewModel();
            try
            {
                User currentUser = _userRepository.GetUser(id);

                vm.DisplayName = currentUser.DisplayName;
                var latestStandings = currentUser.Standings?.LastOrDefault();

                vm.Position = latestStandings?.Position;
                vm.TotalPoints = latestStandings?.TotalPoints ?? 0;
                vm.PdfUrl = Url.Action("Pdf", "Details", new { guid = currentUser.Guid });

                SetPreviousYears(vm, currentUser.DisplayName);

                vm.UserMatches = currentUser.UserMatches;

                SetPlayoffTeams(vm, currentUser);
                SetFinalsTeams(vm, currentUser);
                SetTopScorer(vm, currentUser);
            }
            catch (Exception ex)
            {
                vm.ErrorMessage = "Användaren kunde inte hittas. " + ex.Message;
            }

            return View(vm);
        }

        private static readonly List<(string year, string label, string urlTemplate)> PreviousYearConfigs
            = new List<(string, string, string)>
        {
            ("2010", "VM-tips 2010",  "http://mangesvmtips.personablesolutions.com/Details.aspx?id={0}"),
            ("2012", "EM-tips 2012",  "http://mangesemtips2012.personablesolutions.com/Details.aspx?id={0}"),
            ("2014", "VM-tips 2014",  "http://mangesvmtips2014.personablesolutions.com/Details.aspx?id={0}"),
            ("2016", "EM-tips 2016",  "http://mangesemtips2016.personablesolutions.com/Details.aspx?id={0}"),
            ("2018", "VM-tips 2018",  "http://mangesvmtips2018.personablesolutions.com/Details.aspx?id={0}"),
            ("2021", "EM-tips 2021",  "http://mangesemtips2021.personablesolutions.com/Details.aspx?id={0}"),
            ("2022", "VM-tips 2021",  "http://mangesvmtips2022.personablesolutions.com/Details.aspx?id={0}"),
            ("2024", "EM-tips 2024",  "http://mangesemtips2024.personablesolutions.com/Details.aspx?id={0}"),
        };

        private void SetPreviousYears(DetailsViewModel vm, string displayName)
        {
            foreach (var (year, label, urlTemplate) in PreviousYearConfigs)
            {
                var user = _userRepository.GetPreviousYearUser(year, displayName);
                if (user == null) continue;

                var standing = user.Standings.LastOrDefault();
                if (standing == null) continue;

                vm.PreviousYearsHtml.Add(string.Format(
                    "Placering {0}: <a href=\"{1}\" target=\"_blank\">{2}</a><br />",
                    label,
                    string.Format(urlTemplate, user.ID),
                    standing.Position));
            }
        }

        private void SetTopScorer(DetailsViewModel vm, User currentUser)
        {
            var topScorers = _topScorerRepository.GetWinner();
            string postfix = "";
            foreach (var topScorer in topScorers)
            {
                if (currentUser.TopScorer != null && currentUser.TopScorerID == topScorer.ID)
                    postfix = " 4p";
            }
            vm.TopScorer = currentUser.TopScorer?.DisplayName + postfix;
        }

        private void SetPlayoffTeams(DetailsViewModel vm, User currentUser)
        {
            vm.PlayoffTeams = new Dictionary<string, string>();
            vm.GroupBonus = new Dictionary<string, string>();

            foreach (var group in new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I" })
            {
                int correctInGroup = 0;
                foreach (int pos in new[] { 1, 2 })
                {
                    var upteam = currentUser.UserPlayoffTeams.SingleOrDefault(u => u.Position == pos && u.Team.GroupID == group);
                    if (upteam != null)
                    {
                        var display = upteam.Team.TeamName;
                        if (upteam.Team.IsInPlayOffs)
                        {
                            display += string.Format(" {0}p", upteam.Points);
                            correctInGroup++;
                        }
                        vm.PlayoffTeams[pos + group] = display;
                    }
                    else
                    {
                        vm.PlayoffTeams[pos + group] = "";
                    }
                }
                vm.GroupBonus[group] = correctInGroup == 2 ? "+2p" : "";
            }
        }

        private void SetFinalsTeams(DetailsViewModel vm, User currentUser)
        {
            vm.QFRows = new List<string[]>();
            var qfTeams = currentUser.UserQFTeams.ToList();
            for (int i = 0; i < qfTeams.Count; i += 2)
            {
                var row = new string[2];
                row[0] = FormatTeam(qfTeams[i].Team.TeamName, qfTeams[i].Points);
                row[1] = (i + 1 < qfTeams.Count) ? FormatTeam(qfTeams[i + 1].Team.TeamName, qfTeams[i + 1].Points) : "";
                vm.QFRows.Add(row);
            }

            vm.SFRows = new List<string[]>();
            var sfTeams = currentUser.UserSFTeams.ToList();
            for (int i = 0; i < sfTeams.Count; i += 2)
            {
                var row = new string[2];
                row[0] = FormatTeam(sfTeams[i].Team.TeamName, sfTeams[i].Points);
                row[1] = (i + 1 < sfTeams.Count) ? FormatTeam(sfTeams[i + 1].Team.TeamName, sfTeams[i + 1].Points) : "";
                vm.SFRows.Add(row);
            }

            vm.FinalTeams = currentUser.UserFinalTeams.Select(t => FormatTeam(t.Team.TeamName, t.Points)).ToArray();

            var bronze = currentUser.UserBronzeTeam.ElementAt(0);
            vm.Bronze = FormatTeam(bronze.Team.TeamName, bronze.Points);

            var silver = currentUser.UserSilverTeam.ElementAt(0);
            vm.Silver = FormatTeam(silver.Team.TeamName, silver.Points);

            var gold = currentUser.UserGoldTeam.ElementAt(0);
            vm.Gold = FormatTeam(gold.Team.TeamName, gold.Points);
        }

        private static string FormatTeam(string teamName, byte? points)
        {
            return points.HasValue && points.Value != 0
                ? string.Format("{0} {1}p", teamName, points.Value)
                : teamName;
        }
    }
}
