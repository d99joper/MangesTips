using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Tipset.Models;
using Tipset.ViewModels;

namespace Tipset.Controllers
{
    [Authorize]
    [Route("Admin/TopScorers")]
    public class AdminTopScorersController : BaseController
    {
        private readonly TopScorerRepository _scorerRepo;

        public AdminTopScorersController(TopScorerRepository scorerRepo, SettingsRepository settingsRepo)
            : base(settingsRepo)
        {
            _scorerRepo = scorerRepo;
        }

        [HttpGet("")]
        public ActionResult Index()
        {
            var vm = new AdminTopScorersViewModel
            {
                TopScorers = _scorerRepo.GetAllScorers().ToList()
            };

            if (TempData["ErrorMessage"] is string err)
                vm.ErrorMessage = err;

            return View("~/Views/Admin/TopScorers.cshtml", vm);
        }

        [HttpPost("Save")]
        [ValidateAntiForgeryToken]
        public ActionResult Save(AdminTopScorerInput input)
        {
            string error = null;
            try
            {
                TopScorer scorer = input.TopScorerID > 0
                    ? _scorerRepo.GetTopScorer(input.TopScorerID)
                    : new TopScorer();
                if (input.TopScorerID == 0) _scorerRepo.Add(scorer);
                scorer.FirstName   = input.FirstName;
                scorer.LastName    = input.LastName;
                scorer.DisplayName = $"{input.FirstName} {input.LastName}";
                _scorerRepo.Save();
            }
            catch (Exception ex) { error = ex.Message; }

            TempData["ErrorMessage"] = error;
            return RedirectToAction("Index");
        }

        [HttpPost("BatchImport")]
        [ValidateAntiForgeryToken]
        public ActionResult BatchImport(AdminTopScorerBatchInput input)
        {
            string error = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(input.Names))
                {
                    foreach (var name in input.Names.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var parts = name.Trim().Split(new[] { ' ' }, 2);
                        var scorer = new TopScorer
                        {
                            FirstName   = parts[0],
                            LastName    = parts.Length > 1 ? parts[1] : "",
                            DisplayName = name.Trim()
                        };
                        _scorerRepo.Add(scorer);
                    }
                    _scorerRepo.Save();
                }
            }
            catch (Exception ex) { error = ex.Message; }

            TempData["ErrorMessage"] = error;
            return RedirectToAction("Index");
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var scorer = _scorerRepo.GetTopScorer(id);
            if (scorer != null) { _scorerRepo.Delete(scorer); _scorerRepo.Save(); }
            return RedirectToAction("Index");
        }
    }
}
