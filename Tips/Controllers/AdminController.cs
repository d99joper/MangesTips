using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using Tipset.Models;
using Tipset.ViewModels;

namespace Tipset.Controllers
{
    [Authorize]
    public class AdminController : BaseController
    {
        private readonly TeamRepository      _teamRepo      = new TeamRepository();
        private readonly MatchRepository     _matchRepo     = new MatchRepository();
        private readonly UserRepository      _userRepo      = new UserRepository();
        private readonly BlogRepository      _blogRepo      = new BlogRepository();
        private readonly TopScorerRepository _scorerRepo    = new TopScorerRepository();

        // ── GET /Admin ────────────────────────────────────────────────────────
        public ActionResult Index(int tab = 0)
        {
            return View(BuildViewModel(tab));
        }

        // ── POST: Save match results + recalculate points ─────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveResults(AdminSaveResultsInput input)
        {
            string error = null;
            try
            {
                // 1. Save match scores
                foreach (var m in input.Matches)
                {
                    var match = _matchRepo.GetMatch(m.MatchID);
                    if (match == null) continue;
                    byte hg, ag;
                    if (byte.TryParse(m.HomeGoals, out hg) && byte.TryParse(m.AwayGoals, out ag))
                    {
                        match.HomeGoals = hg;
                        match.AwayGoals = ag;
                        match.ResultMark = hg > ag ? "1" : hg == ag ? "X" : "2";
                    }
                    else match.ResultMark = null;
                }
                _matchRepo.Save();

                // 2. Reset teams then re-apply
                _teamRepo.ResetAllTeams();
                ApplyPlayoffTeams(input);
                ApplyKnockoutTeams(input);
                _teamRepo.Save();

                // 3. Top scorers
                _scorerRepo.ResetWinner();
                SetWinner(input.TopScorer);
                if (!string.IsNullOrEmpty(input.AdditionalTopScorers))
                    foreach (var s in input.AdditionalTopScorers.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                        SetWinner(s);
                _scorerRepo.Save();

                // 4. Recalculate user points
                UpdateUsers();
            }
            catch (Exception ex) { error = ex.Message; }

            var vm = BuildViewModel(0);
            vm.ErrorMessage = error;
            return View("Index", vm);
        }

        // ── POST: Save user flags ─────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveUsers(AdminSaveUsersInput input)
        {
            string error = null;
            try
            {
                foreach (var row in input.Users)
                {
                    var user = _userRepo.GetUser(row.UserID);
                    if (user == null) continue;
                    user.HasPaid     = row.HasPaid;
                    user.IsConfirmed = row.IsConfirmed;
                    user.IsWinner    = row.IsWinner;
                }
                _userRepo.Save();
            }
            catch (Exception ex) { error = ex.Message; }

            var vm = BuildViewModel(1);
            vm.UsersMessage = error ?? "Användarna sparades.";
            return View("Index", vm);
        }

        // ── POST: Toggle EnableNewEntries ─────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleNewEntries()
        {
            string error = null;
            try
            {
                var xDoc  = XDocument.Load(Server.MapPath("~/Models/SettingsExtensions.xml"));
                var xattr = xDoc.Root.Element("EnableNewEntries").Attribute("On");
                xattr.Value = xattr.Value == "true" ? "false" : "true";
                xDoc.Save(Server.MapPath("~/Models/SettingsExtensions.xml"));
            }
            catch (Exception ex) { error = ex.Message; }

            var vm = BuildViewModel(2);
            vm.ErrorMessage = error;
            return View("Index", vm);
        }

        // ── POST: Generate statistics ─────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateStats()
        {
            string error = null, msg = null;
            try
            {
                int userCount = _userRepo.GetAllActiveUsers().Count();
                foreach (var match in _matchRepo.GetAllMatches())
                {
                    match.HomeWinPercent = _userRepo.CountUserMatchResult(match.ID, "1") / (double)userCount;
                    match.DrawPercent    = _userRepo.CountUserMatchResult(match.ID, "X") / (double)userCount;
                    match.AwayWinPercent = _userRepo.CountUserMatchResult(match.ID, "2") / (double)userCount;
                }
                _matchRepo.Save();

                foreach (var team in _teamRepo.GetAllTeams())
                {
                    var ts = team.TeamStats ?? new TeamStats();
                    ts.PlayoffPercent      = (float)_userRepo.CountUserPlayOffTeams(team.ID)      / userCount;
                    ts.QuarterFinalPercent = (float)_userRepo.CountUserQuarterFinalTeams(team.ID) / userCount;
                    ts.SemiFinalPercent    = (float)_userRepo.CountUserSemiFinalTeams(team.ID)    / userCount;
                    ts.FinalPercent        = (float)_userRepo.CountUserFinalsTeams(team.ID)       / userCount;
                    ts.BronzePercent       = (float)_userRepo.CountUserBronzeTeams(team.ID)       / userCount;
                    ts.SilverPercent       = (float)_userRepo.CountUserSilverTeams(team.ID)       / userCount;
                    ts.GoldPercent         = (float)_userRepo.CountUserGoldTeams(team.ID)         / userCount;
                    if (team.TeamStats == null) team.TeamStats = ts;
                }
                _teamRepo.Save();

                foreach (var scorer in _scorerRepo.GetAllScorers())
                    scorer.WinPercent = (float)scorer.Users.Count(u => u.HasPaid) / userCount;
                _scorerRepo.Save();

                msg = "Statistiken genererades utan fel.";
            }
            catch (Exception ex) { error = ex.Message; }

            var vm = BuildViewModel(2);
            vm.ErrorMessage = error;
            vm.StatsMessage = msg;
            return View("Index", vm);
        }

        // ── POST: Save/update blog entry ──────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveBlogEntry(AdminBlogInput input)
        {
            string error = null;
            try
            {
                BlogEntry entry;
                if (input.BlogEntryID > 0)
                    entry = _blogRepo.GetBlogEntry(input.BlogEntryID);
                else
                {
                    entry = new BlogEntry { PostedDate = DateTime.Now };
                    _blogRepo.Add(entry);
                }
                entry.Title = input.Title;
                entry.Text  = input.Text;
                _blogRepo.Save();
            }
            catch (Exception ex) { error = ex.Message; }

            var vm = BuildViewModel(3);
            vm.ErrorMessage = error;
            return View("Index", vm);
        }

        // ── POST: Delete blog entry ───────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBlogEntry(int id)
        {
            var entry = _blogRepo.GetBlogEntry(id);
            if (entry != null) { _blogRepo.Delete(entry); _blogRepo.Save(); }
            return RedirectToAction("Index", new { tab = 3 });
        }

        // ── POST: Save/add match ──────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveMatch(AdminMatchInput input)
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

            var vm = BuildViewModel(4);
            vm.ErrorMessage = error;
            return View("Index", vm);
        }

        // ── POST: Save/add team ───────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveTeam(AdminTeamInput input)
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

            var vm = BuildViewModel(4);
            vm.ErrorMessage = error;
            return View("Index", vm);
        }

        // ── POST: Save/add top scorer ─────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveTopScorer(AdminTopScorerInput input)
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

            var vm = BuildViewModel(5);
            vm.ErrorMessage = error;
            return View("Index", vm);
        }

        // ── POST: Delete top scorer ───────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTopScorer(int id)
        {
            var scorer = _scorerRepo.GetTopScorer(id);
            if (scorer != null) { _scorerRepo.Delete(scorer); _scorerRepo.Save(); }
            return RedirectToAction("Index", new { tab = 5 });
        }

        // ─────────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────────

        private AdminIndexViewModel BuildViewModel(int activeTab)
        {
            var allTeams = _teamRepo.GetAllTeams().ToList();
            var vm = new AdminIndexViewModel
            {
                ActiveTab    = activeTab,
                Matches      = _matchRepo.GetAllMatches().ToList(),
                AllTeams     = allTeams,
                Users        = _userRepo.GetAllUsers().ToList(),
                BlogEntries  = _blogRepo.GetAllBlogEntries().ToList(),
                Teams        = allTeams,
                TopScorers   = _scorerRepo.GetAllScorers().ToList(),
                TopScorer    = _scorerRepo.GetWinner().FirstOrDefault()?.DisplayName ?? "",
                QFSelected   = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.isInQuarterFinals).Select(t => t.ID).ToList(),
                SFSelected   = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.isInSemiFinals).Select(t => t.ID).ToList(),
                FinSelected  = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.isInFinals).Select(t => t.ID).ToList(),
                BronzeSelected = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.WonBronze).FirstOrDefault()?.ID ?? -1,
                SilverSelected = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.WonSilver).FirstOrDefault()?.ID ?? -1,
                GoldSelected   = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.WonGold).FirstOrDefault()?.ID   ?? -1,
            };

            // Playoff selections
            foreach (var g in new[] { "A","B","C","D","E","F","G","H" })
            {
                var t1 = _teamRepo.GetTeam(TeamRepository.TeamInqueryType.isInPlayoffs, g, 1);
                var t2 = _teamRepo.GetTeam(TeamRepository.TeamInqueryType.isInPlayoffs, g, 2);
                vm.PlayoffSelected[g + "1"] = t1?.ID ?? -1;
                vm.PlayoffSelected[g + "2"] = t2?.ID ?? -1;
            }

            // Settings XML
            try
            {
                var xDoc  = XDocument.Load(Server.MapPath("~/Models/SettingsExtensions.xml"));
                var xattr = xDoc.Root.Element("EnableNewEntries").Attribute("On");
                vm.EnableNewEntries = bool.Parse(xattr.Value);
            }
            catch { }

            return vm;
        }

        private void ApplyPlayoffTeams(AdminSaveResultsInput input)
        {
            var pairs = new[]
            {
                ("A",input.PlayoffA1,1),("A",input.PlayoffA2,2),
                ("B",input.PlayoffB1,1),("B",input.PlayoffB2,2),
                ("C",input.PlayoffC1,1),("C",input.PlayoffC2,2),
                ("D",input.PlayoffD1,1),("D",input.PlayoffD2,2),
                ("E",input.PlayoffE1,1),("E",input.PlayoffE2,2),
                ("F",input.PlayoffF1,1),("F",input.PlayoffF2,2),
                ("G",input.PlayoffG1,1),("G",input.PlayoffG2,2),
                ("H",input.PlayoffH1,1),("H",input.PlayoffH2,2),
            };
            foreach (var (g, id, pos) in pairs)
            {
                if (id <= 0) continue;
                var team = _teamRepo.GetTeam(id);
                if (team == null) continue;
                team.IsInPlayOffs = true;
                team.PlayOffPos   = (byte)pos;
            }
        }

        private void ApplyKnockoutTeams(AdminSaveResultsInput input)
        {
            foreach (var id in input.QFTeams ?? new int[0])
            { var t = _teamRepo.GetTeam(id); if (t != null) t.IsInQuarterFinals = true; }

            foreach (var id in input.SFTeams ?? new int[0])
            { var t = _teamRepo.GetTeam(id); if (t != null) t.IsInSemiFinals = true; }

            foreach (var id in input.FinalTeams ?? new int[0])
            { var t = _teamRepo.GetTeam(id); if (t != null) t.IsInFinal = true; }

            if (input.BronzeTeam > 0) { var t = _teamRepo.GetTeam(input.BronzeTeam); if (t != null) t.WonBronze = true; }
            if (input.SilverTeam > 0) { var t = _teamRepo.GetTeam(input.SilverTeam); if (t != null) t.WonSilver = true; }
            if (input.GoldTeam   > 0) { var t = _teamRepo.GetTeam(input.GoldTeam);   if (t != null) t.WonGold   = true; }
        }

        private void SetWinner(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName)) return;
            var scorer = _scorerRepo.GetTopScorer(displayName.Trim());
            if (scorer != null) scorer.IsWinner = true;
        }

        private void UpdateUsers()
        {
            var dtNow = DateTime.UtcNow.AddHours(2);
            var users   = _userRepo.GetAllActiveUsers();
            var matches = _matchRepo.GetAllMatches();
            var guid    = Guid.NewGuid();

            foreach (var user in users)
            {
                short total = 0;

                foreach (var match in matches)
                {
                    var um = user.UserMatches.SingleOrDefault(x => x.MatchID == match.ID);
                    if (um == null) continue;
                    um.Points = 0;
                    if (match.ResultMark != null)
                    {
                        if (um.HomeGoals == match.HomeGoals && um.AwayGoals == match.AwayGoals) um.Points++;
                        if (match.ResultMark == "1" && um.HomeGoals > um.AwayGoals)  um.Points = (byte)(um.Points + 2);
                        else if (match.ResultMark == "X" && um.HomeGoals == um.AwayGoals) um.Points = (byte)(um.Points + 2);
                        else if (match.ResultMark == "2" && um.HomeGoals < um.AwayGoals)  um.Points = (byte)(um.Points + 2);
                    }
                    total += (short)(um.Points ?? 0);
                }

                // Playoff bonus
                foreach (var pt in _teamRepo.GetTeams(TeamRepository.TeamInqueryType.isInPlayoffs))
                {
                    var uteam = user.UserPlayoffTeams.SingleOrDefault(t => t.TeamID == pt.ID);
                    if (uteam == null) continue;
                    uteam.Points = 2;
                    var bp = uteam.User.BonusPoints.SingleOrDefault(b => b.GroupID == uteam.Team.GroupID);
                    if (bp != null)
                    {
                        if (uteam.Points == 2 && bp.HalfPoint) bp.Point = 2;
                        else if (uteam.Points == 2 && !bp.HalfPoint) bp.HalfPoint = true;
                    }
                    if (uteam.Position == pt.PlayOffPos) uteam.Points = (byte)(uteam.Points + 2);
                    total += (short)(uteam.Points + (bp?.Point ?? 0));
                }

                foreach (var t in _teamRepo.GetTeams(TeamRepository.TeamInqueryType.isInQuarterFinals))
                { var u = user.UserQFTeams.SingleOrDefault(x => x.TeamID == t.ID); if (u != null) { u.Points = 4; total += 4; } }

                foreach (var t in _teamRepo.GetTeams(TeamRepository.TeamInqueryType.isInSemiFinals))
                { var u = user.UserSFTeams.SingleOrDefault(x => x.TeamID == t.ID); if (u != null) { u.Points = 4; total += 4; } }

                foreach (var t in _teamRepo.GetTeams(TeamRepository.TeamInqueryType.isInFinals))
                { var u = user.UserFinalTeams.SingleOrDefault(x => x.TeamID == t.ID); if (u != null) { u.Points = 4; total += 4; } }

                foreach (var t in _teamRepo.GetTeams(TeamRepository.TeamInqueryType.WonBronze))
                { var u = user.UserBronzeTeam.SingleOrDefault(x => x.TeamID == t.ID); if (u != null) { u.Points = 10; total += 10; } }

                foreach (var t in _teamRepo.GetTeams(TeamRepository.TeamInqueryType.WonSilver))
                { var u = user.UserSilverTeam.SingleOrDefault(x => x.TeamID == t.ID); if (u != null) { u.Points = 10; total += 10; } }

                foreach (var t in _teamRepo.GetTeams(TeamRepository.TeamInqueryType.WonGold))
                { var u = user.UserGoldTeam.SingleOrDefault(x => x.TeamID == t.ID); if (u != null) { u.Points = 10; total += 10; } }

                foreach (var scorer in _scorerRepo.GetWinner())
                    if (user.TopScorerID == scorer.ID) total += 10;

                user.Standings.Add(new Standing { TotalPoints = total, UpdateDate = dtNow, Guid = guid });
            }

            _userRepo.Save();
            _userRepo.SortStandings(dtNow);
            _userRepo.Save();
        }
    }
}
