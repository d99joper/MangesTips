using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Tipset.Models;
using Tipset.ViewModels;

namespace Tipset.Controllers
{
    [Authorize]
    [Route("Admin/Teams")]
    public class AdminTeamsController : BaseController
    {
        private readonly TeamRepository _teamRepo;

        public AdminTeamsController(TeamRepository teamRepo, SettingsRepository settingsRepo)
            : base(settingsRepo)
        {
            _teamRepo = teamRepo;
        }

        [HttpGet("")]
        public ActionResult Index()
        {
            var vm = new AdminTeamsViewModel
            {
                Teams = _teamRepo.GetAllTeams().ToList()
            };

            if (TempData["ErrorMessage"] is string err)
                vm.ErrorMessage = err;

            return View("~/Views/Admin/Teams.cshtml", vm);
        }

        [HttpPost("Save")]
        [ValidateAntiForgeryToken]
        public ActionResult Save(AdminTeamInput input)
        {
            string error = null;
            try
            {
                Team team = input.TeamID > 0
                    ? _teamRepo.GetTeam(input.TeamID)
                    : new Team();
                if (input.TeamID == 0) _teamRepo.Add(team);
                team.TeamName = input.TeamName;
                team.GroupID  = input.GroupID;
                _teamRepo.Save();
            }
            catch (Exception ex) { error = ex.Message; }

            TempData["ErrorMessage"] = error;
            return RedirectToAction("Index");
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var team = _teamRepo.GetTeam(id);
            if (team != null) { _teamRepo.Delete(team); _teamRepo.Save(); }
            return RedirectToAction("Index");
        }
    }
}
