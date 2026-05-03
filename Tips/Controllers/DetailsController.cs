using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tipset.Models;
using Tipset.ViewModels;

namespace Tipset.Controllers
{
    public class DetailsController : BaseController
    {
        private readonly UserRepository _userRepository = new UserRepository();
        private readonly TopScorerRepository _topScorerRepository = new TopScorerRepository();

        public ActionResult Index(int id)
        {
            var vm = new DetailsViewModel();
            try
            {
                User currentUser = _userRepository.GetUser(id);

                vm.DisplayName = currentUser.DisplayName;
                vm.Position = currentUser.Standings.Last().Position;
                vm.TotalPoints = currentUser.Standings.Last().TotalPoints;
                vm.PdfUrl = Url.Content("~/pdfGenerator.aspx") + "?id=" + currentUser.Guid;

                var user2010 = _userRepository.GetVM2010User(currentUser.DisplayName);
                if (user2010 != null)
                    vm.Vm2010Html = string.Format("Placering VM-tips 2010: <a href=\"http://mangesvmtips.personablesolutions.com/Details.aspx?id={0}\" target=\"_blank\">{1}</a><br />", user2010.ID, user2010.Standings_2010.Last().Position);

                var user2012 = _userRepository.GetEM2012User(currentUser.DisplayName);
                if (user2012 != null)
                    vm.Em2012Html = string.Format("Placering EM-tips 2012: <a href=\"http://mangesemtips2012.personablesolutions.com/Details.aspx?id={0}\" target=\"_blank\">{1}</a><br />", user2012.ID, user2012.Standings_2012.Last().Position);

                var user2014 = _userRepository.GetVM2014User(currentUser.DisplayName);
                if (user2014 != null)
                    vm.Vm2014Html = string.Format("Placering VM-tips 2014: <a href=\"http://mangesvmtips2014.personablesolutions.com/Details.aspx?id={0}\" target=\"_blank\">{1}</a><br />", user2014.ID, user2014.Standings_2014.Last().Position);

                var user2016 = _userRepository.GetEM2016User(currentUser.DisplayName);
                if (user2016 != null)
                    vm.Em2016Html = string.Format("Placering EM-tips 2016: <a href=\"http://mangesemtips2016.personablesolutions.com/Details.aspx?id={0}\" target=\"_blank\">{1}</a><br />", user2016.ID, user2016.Standings_2016.Last().Position);

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

            foreach (var group in new[] { "A", "B", "C", "D", "E", "F", "G", "H" })
            {
                foreach (int pos in new[] { 1, 2 })
                {
                    var upteam = currentUser.UserPlayoffTeams.SingleOrDefault(u => u.Position == pos && u.Team.GroupID == group);
                    if (upteam != null)
                    {
                        var display = upteam.Team.TeamName;
                        if (upteam.Team.IsInPlayOffs)
                            display += string.Format(" {0}p", upteam.Points);
                        vm.PlayoffTeams[pos + group] = display;
                    }
                    else
                    {
                        vm.PlayoffTeams[pos + group] = "";
                    }
                }
                vm.GroupBonus[group] = currentUser.BonusPoints.Any(b => b.GroupID == group && b.Point == 1) ? "+1p" : "";
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
