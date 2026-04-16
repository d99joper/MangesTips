using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using Tipset.Models;
using Tipset.ViewModels;

namespace Tipset.Controllers
{
    public class NyKupongController : BaseController
    {
        private readonly TeamRepository _teamRepository = new TeamRepository();
        private readonly MatchRepository _matchRepository = new MatchRepository();
        private readonly TopScorerRepository _topScorerRepository = new TopScorerRepository();
        private readonly UserRepository _userRepository = new UserRepository();

        [HttpGet]
        public ActionResult Index()
        {
            var vm = BuildViewModel(new NyKupongViewModel());
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(NyKupongViewModel vm)
        {
            // Re-attach lookup data stripped by POST
            RebuildLists(vm);

            if (!vm.IsOpen)
                return View(vm);

            if (!ValidateForm(vm))
                return View(vm);

            try
            {
                var user = BuildUser(vm);
                _userRepository.Add(user);
                _userRepository.Save();

                string body = BuildEmailBody(user.Guid);
                Helpers.SendEmail.SendEmail_SMTP(
                    "noreply@nodomain.com", "VMTipset",
                    user.EmailAddress, user.DisplayName,
                    "Var god bekräfta din anmälan till Manges VM-tips.", body);

                return RedirectToAction("Index", "AwaitingConfirmation");
            }
            catch (Exception ex)
            {
                vm.ErrorMessage = "Något gick fel när kupongen skulle sparas. " + ex.Message;
                return View(vm);
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private NyKupongViewModel BuildViewModel(NyKupongViewModel vm)
        {
            XDocument xDoc = XDocument.Load(Server.MapPath("~/Models/SettingsExtensions.xml"));
            vm.IsOpen = bool.Parse(xDoc.Root.Element("EnableNewEntries").Attribute("On").Value);

            if (vm.IsOpen)
            {
                RebuildLists(vm);
                // On fresh GET, initialise one entry per match so the view index aligns
                if (vm.Scores.Count == 0)
                    vm.Scores = vm.Matches.Select(m => new MatchScoreInput { MatchID = m.ID }).ToList();
            }

            return vm;
        }

        private void RebuildLists(NyKupongViewModel vm)
        {
            XDocument xDoc = XDocument.Load(Server.MapPath("~/Models/SettingsExtensions.xml"));
            vm.IsOpen = bool.Parse(xDoc.Root.Element("EnableNewEntries").Attribute("On").Value);
            vm.Matches = _matchRepository.GetAllMatches().ToList();
            vm.AllTeams = _teamRepository.GetAllTeams().ToList();
            foreach (var g in new[] { "A", "B", "C", "D", "E", "F", "G", "H" })
                vm.GroupTeams[g] = _teamRepository.GetTeams(g).ToList();
        }

        private bool ValidateForm(NyKupongViewModel vm)
        {
            bool valid = true;

            if (vm.QFTeams == null || vm.QFTeams.Any(t => t == -1) || vm.QFTeams.Distinct().Count() != vm.QFTeams.Length)
            { vm.QFError = true; valid = false; }

            if (vm.SFTeams == null || vm.SFTeams.Any(t => t == -1) || vm.SFTeams.Distinct().Count() != vm.SFTeams.Length)
            { vm.SFError = true; valid = false; }

            if (vm.FinalTeams == null || vm.FinalTeams.Any(t => t == -1) || vm.FinalTeams.Distinct().Count() != vm.FinalTeams.Length)
            { vm.FinalError = true; valid = false; }

            if (_topScorerRepository.GetTopScorer(vm.TopScorer ?? "") == null)
            { vm.ScorerError = "* Namnet du angett är inte giltigt."; valid = false; }

            return valid;
        }

        private User BuildUser(NyKupongViewModel vm)
        {
            var user = new User
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                EmailAddress = vm.Email,
                PostedDate = DateTime.Now,
                DisplayName = vm.FirstName + " " + vm.LastName,
                Guid = Guid.NewGuid()
            };

            // Match scores
            foreach (var s in vm.Scores)
            {
                if (!byte.TryParse(s.HomeGoals, out byte hg) || !byte.TryParse(s.AwayGoals, out byte ag))
                    continue;
                user.UserMatches.Add(new UserMatch
                {
                    MatchID = s.MatchID,
                    HomeGoals = hg,
                    AwayGoals = ag,
                    ResultMark = hg > ag ? "1" : hg == ag ? "X" : "2"
                });
            }

            // Playoff teams: groups A-H
            var playoffPairs = new Dictionary<string, (int p1, int p2)>
            {
                ["A"] = (vm.PlayoffA1, vm.PlayoffA2), ["B"] = (vm.PlayoffB1, vm.PlayoffB2),
                ["C"] = (vm.PlayoffC1, vm.PlayoffC2), ["D"] = (vm.PlayoffD1, vm.PlayoffD2),
                ["E"] = (vm.PlayoffE1, vm.PlayoffE2), ["F"] = (vm.PlayoffF1, vm.PlayoffF2),
                ["G"] = (vm.PlayoffG1, vm.PlayoffG2), ["H"] = (vm.PlayoffH1, vm.PlayoffH2)
            };
            foreach (var kv in playoffPairs)
            {
                user.UserPlayoffTeams.Add(new UserPlayoffTeam { TeamID = kv.Value.p1, Position = 1, Points = 0 });
                user.UserPlayoffTeams.Add(new UserPlayoffTeam { TeamID = kv.Value.p2, Position = 2, Points = 0 });
                var team = _teamRepository.GetTeam(kv.Value.p2);
                if (team != null)
                    user.BonusPoints.Add(new BonusPoints { GroupID = team.GroupID });
            }

            // QF / SF / Final
            foreach (var id in vm.QFTeams)   user.UserQFTeams.Add(new UserQFTeam { TeamID = id, Points = 0 });
            foreach (var id in vm.SFTeams)   user.UserSFTeams.Add(new UserSFTeam { TeamID = id, Points = 0 });
            foreach (var id in vm.FinalTeams) user.UserFinalTeams.Add(new UserFinalTeam { TeamID = id, Points = 0 });

            user.UserBronzeTeam.Add(new UserBronzeTeam { TeamID = vm.BronzeTeam, Points = 0 });
            user.UserSilverTeam.Add(new UserSilverTeam { TeamID = vm.SilverTeam, Points = 0 });
            user.UserGoldTeam.Add(new UserGoldTeam   { TeamID = vm.GoldTeam,   Points = 0 });

            user.TopScorerID = _topScorerRepository.GetTopScorer(vm.TopScorer).ID;

            return user;
        }

        private string BuildEmailBody(Guid guid)
        {
            string url = string.Format(
                "http://mangesvmtips2022.personablesolutions.com/Confirm?id={0}", guid);
            return "<p>Tack för din anmälan till Manges VM-tips.</p>" +
                   "<p>Din kupong är fortfarande inte bekräftad. Klicka på länken nedan för att bekräfta din anmälan.</p>" +
                   string.Format("<p><a href=\"{0}\">{0}</a></p>", url);
        }
    }
}
