using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Tipset.Models;
using Tipset.ViewModels;

namespace Tipset.Controllers
{
    [Authorize]
    [Route("Admin/Matches")]
    public class AdminMatchesController : BaseController
    {
        private readonly MatchRepository _matchRepo;
        private readonly TeamRepository  _teamRepo;

        public AdminMatchesController(MatchRepository matchRepo, TeamRepository teamRepo, SettingsRepository settingsRepo)
            : base(settingsRepo)
        {
            _matchRepo = matchRepo;
            _teamRepo  = teamRepo;
        }

        [HttpGet("")]
        public ActionResult Index()
        {
            var vm = new AdminMatchesViewModel
            {
                Matches  = _matchRepo.GetAllMatches().ToList(),
                AllTeams = _teamRepo.GetAllTeams().ToList()
            };

            if (TempData["ErrorMessage"] is string err)
                vm.ErrorMessage = err;

            return View("~/Views/Admin/Matches.cshtml", vm);
        }

        [HttpPost("Save")]
        [ValidateAntiForgeryToken]
        public ActionResult Save(AdminMatchInput input)
        {
            string error = null;
            try
            {
                Match match = input.MatchID > 0
                    ? _matchRepo.GetMatch(input.MatchID)
                    : new Match();
                if (input.MatchID == 0) _matchRepo.Add(match);
                match.HomeTeamID = input.HomeTeamID;
                match.AwayTeamID = input.AwayTeamID;
                match.Date       = Convert.ToDateTime(input.Date);
                _matchRepo.Save();
            }
            catch (Exception ex) { error = ex.Message; }

            TempData["ErrorMessage"] = error;
            return RedirectToAction("Index");
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var match = _matchRepo.GetMatch(id);
            if (match != null) { _matchRepo.Delete(match); _matchRepo.Save(); }
            return RedirectToAction("Index");
        }
    }
}
