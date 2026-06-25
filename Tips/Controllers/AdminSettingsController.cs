using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Tipset.Models;
using Tipset.ViewModels;

namespace Tipset.Controllers
{
    [Authorize]
    [Route("Admin/Settings")]
    public class AdminSettingsController : BaseController
    {
        private readonly TeamRepository      _teamRepo;
        private readonly MatchRepository     _matchRepo;
        private readonly UserRepository      _userRepo;
        private readonly TopScorerRepository _scorerRepo;

        public AdminSettingsController(TeamRepository teamRepo, MatchRepository matchRepo,
            UserRepository userRepo, TopScorerRepository scorerRepo, SettingsRepository settingsRepo)
            : base(settingsRepo)
        {
            _teamRepo   = teamRepo;
            _matchRepo  = matchRepo;
            _userRepo   = userRepo;
            _scorerRepo = scorerRepo;
        }

        [HttpGet("")]
        public ActionResult Index()
        {
            var vm = new AdminSettingsViewModel
            {
                EnableNewEntries = _settingsRepo.GetBool("EnableNewEntries")
            };

            if (TempData["ErrorMessage"] is string err)
                vm.ErrorMessage = err;
            if (TempData["StatsMessage"] is string msg)
                vm.StatsMessage = msg;

            return View("~/Views/Admin/Settings.cshtml", vm);
        }

        [HttpPost("Toggle")]
        [ValidateAntiForgeryToken]
        public ActionResult Toggle()
        {
            try
            {
                _settingsRepo.Set("EnableNewEntries", (!_settingsRepo.GetBool("EnableNewEntries")).ToString().ToLower());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost("GenerateStats")]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateStats()
        {
            string error = null, msg = null;
            try
            {
                int userCount = _userRepo.GetAllActiveUsers().Count();
                if (userCount == 0)
                {
                    error = "Inga aktiva användare hittades. Statistik kunde inte genereras.";
                }
                else
                {
                    var matchResultCounts = _userRepo.GetMatchResultCounts();
                    foreach (var match in _matchRepo.GetAllMatches())
                    {
                        match.HomeWinPercent = matchResultCounts.TryGetValue(match.ID + "_1", out var hw) ? hw / userCount : 0;
                        match.DrawPercent    = matchResultCounts.TryGetValue(match.ID + "_X", out var dr) ? dr / userCount : 0;
                        match.AwayWinPercent = matchResultCounts.TryGetValue(match.ID + "_2", out var aw) ? aw / userCount : 0;
                    }
                    _matchRepo.Save();

                    var playoffCounts = _userRepo.GetPlayoffTeamCounts();
                    var qfCounts      = _userRepo.GetQFTeamCounts();
                    var sfCounts      = _userRepo.GetSFTeamCounts();
                    var finalCounts   = _userRepo.GetFinalTeamCounts();
                    var bronzeCounts  = _userRepo.GetBronzeTeamCounts();
                    var silverCounts  = _userRepo.GetSilverTeamCounts();
                    var goldCounts    = _userRepo.GetGoldTeamCounts();

                    foreach (var team in _teamRepo.GetAllTeamsWithStats())
                    {
                        var ts = team.TeamStats ?? new TeamStats();
                        ts.PlayoffPercent      = playoffCounts.TryGetValue(team.ID, out var po) ? (float)po / userCount : 0f;
                        ts.QuarterFinalPercent = qfCounts.TryGetValue(team.ID, out var qf)      ? (float)qf / userCount : 0f;
                        ts.SemiFinalPercent    = sfCounts.TryGetValue(team.ID, out var sf)       ? (float)sf / userCount : 0f;
                        ts.FinalPercent        = finalCounts.TryGetValue(team.ID, out var fi)    ? (float)fi / userCount : 0f;
                        ts.BronzePercent       = bronzeCounts.TryGetValue(team.ID, out var br)   ? (float)br / userCount : 0f;
                        ts.SilverPercent       = silverCounts.TryGetValue(team.ID, out var si)   ? (float)si / userCount : 0f;
                        ts.GoldPercent         = goldCounts.TryGetValue(team.ID, out var go)     ? (float)go / userCount : 0f;
                        if (team.TeamStats == null) team.TeamStats = ts;
                    }
                    _teamRepo.Save();

                    foreach (var scorer in _scorerRepo.GetAllScorers())
                        scorer.WinPercent = userCount > 0 ? (float)scorer.Users.Count(u => u.HasPaid) / userCount : 0f;
                    _scorerRepo.Save();

                    msg = "Statistiken genererades utan fel.";
                }
            }
            catch (Exception ex) { error = ex.Message; }

            TempData["ErrorMessage"] = error;
            TempData["StatsMessage"] = msg;
            return RedirectToAction("Index");
        }
    }
}
