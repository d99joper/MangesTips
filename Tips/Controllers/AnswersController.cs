using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tipset.Models;
using Tipset.ViewModels;

namespace Tipset.Controllers
{
    public class AnswersController : BaseController
    {
        private readonly TeamRepository _teamRepository = new TeamRepository();
        private readonly MatchRepository _matchRepository = new MatchRepository();
        private readonly TopScorerRepository _topScorerRepository = new TopScorerRepository();

        public ActionResult Index()
        {
            var vm = new AnswersViewModel();

            vm.Matches = _matchRepository.GetAllMatches().ToList();

            vm.PlayoffTeams = new Dictionary<string, string>();
            foreach (var group in new[] { "A", "B", "C", "D", "E", "F", "G", "H" })
            {
                foreach (int pos in new[] { 1, 2 })
                {
                    var team = _teamRepository.GetTeam(TeamRepository.TeamInqueryType.isInPlayoffs, group, (byte)pos);
                    vm.PlayoffTeams[pos + group] = team?.TeamName ?? "";
                }
            }

            vm.QFTeams = _teamRepository.GetTeams(TeamRepository.TeamInqueryType.isInQuarterFinals)
                .ConvertAll(t => t.TeamName);
            vm.SFTeams = _teamRepository.GetTeams(TeamRepository.TeamInqueryType.isInSemiFinals)
                .ConvertAll(t => t.TeamName);
            vm.FinalTeams = _teamRepository.GetTeams(TeamRepository.TeamInqueryType.isInFinals)
                .ConvertAll(t => t.TeamName);

            var bronzeTeams = _teamRepository.GetTeams(TeamRepository.TeamInqueryType.WonBronze);
            vm.Bronze = bronzeTeams.Count > 0 ? bronzeTeams[0].TeamName : "";

            var silverTeams = _teamRepository.GetTeams(TeamRepository.TeamInqueryType.WonSilver);
            vm.Silver = silverTeams.Count > 0 ? silverTeams[0].TeamName : "";

            var goldTeams = _teamRepository.GetTeams(TeamRepository.TeamInqueryType.WonGold);
            vm.Gold = goldTeams.Count > 0 ? goldTeams[0].TeamName : "";

            var topScorers = _topScorerRepository.GetWinner();
            vm.TopScorer = topScorers.Count > 0
                ? string.Join(", ", topScorers.ConvertAll(t => t.DisplayName))
                : "";

            return View(vm);
        }
    }
}
