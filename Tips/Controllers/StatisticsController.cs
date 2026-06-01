using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Tipset.Models;
using Tipset.ViewModels;

namespace Tipset.Controllers
{
    public class StatisticsController : BaseController
    {
        private readonly TeamRepository _teamRepository;
        private readonly MatchRepository _matchRepository;
        private readonly TopScorerRepository _topScorerRepository;
        private readonly UserRepository _userRepository;

        public StatisticsController(TeamRepository teamRepository, MatchRepository matchRepository,
            TopScorerRepository topScorerRepository, UserRepository userRepository, SettingsRepository settingsRepo)
            : base(settingsRepo)
        {
            _teamRepository = teamRepository;
            _matchRepository = matchRepository;
            _topScorerRepository = topScorerRepository;
            _userRepository = userRepository;
        }

        public ActionResult Index(string sort, string dir, int tab = 0)
        {
            var vm = new StatisticsViewModel
            {
                SortColumn = sort,
                SortDir = dir ?? "asc",
                ActiveTab = tab,
                Matches = _matchRepository.GetAllMatches().ToList()
            };

            IQueryable<Team> teams = _teamRepository.GetAllTeams();
            bool asc = vm.SortDir == "asc";
            switch (vm.SortColumn)
            {
                case "TeamName":
                    teams = asc ? teams.OrderBy(t => t.TeamName) : teams.OrderByDescending(t => t.TeamName); break;
                case "Playoff":
                    teams = asc ? teams.OrderBy(t => t.TeamStats.PlayoffPercent) : teams.OrderByDescending(t => t.TeamStats.PlayoffPercent); break;
                case "QuarterFinals":
                    teams = asc ? teams.OrderBy(t => t.TeamStats.QuarterFinalPercent) : teams.OrderByDescending(t => t.TeamStats.QuarterFinalPercent); break;
                case "SemiFinals":
                    teams = asc ? teams.OrderBy(t => t.TeamStats.SemiFinalPercent) : teams.OrderByDescending(t => t.TeamStats.SemiFinalPercent); break;
                case "Finals":
                    teams = asc ? teams.OrderBy(t => t.TeamStats.FinalPercent) : teams.OrderByDescending(t => t.TeamStats.FinalPercent); break;
                case "Bronze":
                    teams = asc ? teams.OrderBy(t => t.TeamStats.BronzePercent) : teams.OrderByDescending(t => t.TeamStats.BronzePercent); break;
                case "Silver":
                    teams = asc ? teams.OrderBy(t => t.TeamStats.SilverPercent) : teams.OrderByDescending(t => t.TeamStats.SilverPercent); break;
                case "Gold":
                    teams = asc ? teams.OrderBy(t => t.TeamStats.GoldPercent) : teams.OrderByDescending(t => t.TeamStats.GoldPercent); break;
            }
            vm.Teams = teams.ToList();
            vm.TopScorers = _topScorerRepository.GetAllScorers().OrderByDescending(t => t.WinPercent).ToList();

            return View(vm);
        }

        public ActionResult StatDetails(string type, int? id, string result, string stage, int? teamid)
        {
            var vm = new StatDetailsViewModel { Rows = new List<StatDetailRow>() };

            switch (type)
            {
                case "Match":
                    foreach (var um in _userRepository.GetAllUserMatches(id.Value, result))
                    {
                        bool correct = um.Match.HomeGoals == um.HomeGoals && um.Match.AwayGoals == um.AwayGoals;
                        vm.Rows.Add(new StatDetailRow
                        {
                            DisplayName = um.User.DisplayName,
                            IsHighlighted = correct,
                            Extra = string.Format("{0} - {1}", um.HomeGoals, um.AwayGoals)
                        });
                    }
                    break;
                case "playoffs":
                    bool correctTeam = _userRepository.CorrectTeamInStage(teamid.Value, stage);
                    foreach (var u in _userRepository.GetUserPlayoffTeams(stage, teamid.Value))
                        vm.Rows.Add(new StatDetailRow { DisplayName = u.DisplayName, IsHighlighted = correctTeam });
                    break;
                case "topscorer":
                    foreach (var u in _userRepository.GetUsersForTopscorer(id.Value))
                        vm.Rows.Add(new StatDetailRow { DisplayName = u.DisplayName, IsHighlighted = false });
                    break;
                default:
                    return NotFound();
            }

            return PartialView(vm);
        }
    }
}
