using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Tipset.Models;
using Tipset.ViewModels;
using Tipset.Helpers.Sanitization;

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
            var vm = BuildViewModel(tab);
            if (TempData["ErrorMessage"] != null)
                vm.ErrorMessage = TempData["ErrorMessage"].ToString();
            return View(vm);
        }

        // ── POST: Save match results + recalculate points ─────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveResults(AdminSaveResultsInput input)
        {
            var messages = new List<string>();
            string error = null;
            try
            {
                // 1. Save match scores
                int scored = 0, cleared = 0;
                foreach (var m in input.Matches)
                {
                    var match = _matchRepo.GetMatch(m.MatchID);
                    if (match == null) continue;
                    byte hg, ag;
                    if (byte.TryParse(m.HomeGoals, out hg) && byte.TryParse(m.AwayGoals, out ag))
                    {
                        match.HomeGoals  = hg;
                        match.AwayGoals  = ag;
                        match.ResultMark = hg > ag ? "1" : hg == ag ? "X" : "2";
                        scored++;
                    }
                    else
                    {
                        match.HomeGoals  = null;
                        match.AwayGoals  = null;
                        match.ResultMark = null;
                        cleared++;
                    }
                }
                _matchRepo.Save();
                messages.Add(string.Format("✔ Matchresultat sparade ({0} satta, {1} rensade).", scored, cleared));

                // 2. Reset teams then re-apply
                _teamRepo.ResetAllTeams();
                ApplyPlayoffTeams(input);
                messages.Add("✔ Vidare från gruppen sparade.");
                ApplyKnockoutTeams(input);
                _teamRepo.Save();
                messages.Add("✔ KO-faser (QF/SF/Final/medaljer) sparade.");

                // 3. Top scorers
                _scorerRepo.ResetWinner();
                int scorerCount = 0;
                if (input.TopScorers != null)
                    foreach (var s in input.TopScorers.Where(s => !string.IsNullOrWhiteSpace(s)))
                    {
                        SetWinner(s);
                        scorerCount++;
                    }
                _scorerRepo.Save();
                messages.Add(string.Format("✔ Skyttekung sparad ({0} st).", scorerCount));

                // 4. Recalculate user points
                UpdateUsers();
                messages.Add("✔ Användarpoäng omräknade.");

                messages.Add("✅ Allt sparat!");
            }
            catch (Exception ex)
            {
                error = ex.Message;
                messages.Add("❌ Fel: " + ex.Message);
            }

            var vm = BuildViewModel(0);
            vm.ErrorMessage     = error;
            vm.ResultsMessages  = messages;
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
                var ids     = input.Users.Select(r => r.UserID).ToList();
                var userMap = _userRepo.GetAllUsers()
                                       .Where(u => ids.Contains(u.ID))
                                       .ToDictionary(u => u.ID);
                foreach (var row in input.Users)
                {
                    if (!userMap.TryGetValue(row.UserID, out var user)) continue;
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
            try
            {
                var settings = new SettingsRepository();
                settings.Set("EnableNewEntries", (!settings.GetBool("EnableNewEntries")).ToString().ToLower());
            }
            catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }

            return RedirectToAction("Index", new { tab = 2 });
        }

        // ── POST: Generate statistics ─────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateStats()
        {
            string error = null, msg = null;
            var vm = BuildViewModel(2);
            try
            {
                int userCount = _userRepo.GetAllActiveUsers().Count();
                if (userCount == 0)
                {
                    vm.ErrorMessage = "Inga aktiva användare hittades. Statistik kunde inte genereras.";
                    return View("Index", vm);
                }

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

                foreach (var team in _teamRepo.GetAllTeams())
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
            catch (Exception ex) { error = ex.Message; }

            vm.ErrorMessage = error;
            vm.StatsMessage = msg;
            return View("Index", vm);
        }

        // ── POST: Save/update blog entry ──────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveBlogEntry(AdminBlogInput input)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(input.Title))
                errors.Add("Titeln får inte vara tom.");
            if (string.IsNullOrWhiteSpace(input.Text))
                errors.Add("Texten får inte vara tom.");

            if (errors.Count == 0)
            {
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
                    entry.Title = input.Title.Trim();
                    entry.Text  = HtmlSanitizer.Sanitize(input.Text);
                    _blogRepo.Save();
                }
                catch (Exception ex)
                {
                    errors.Add("Ett fel uppstod vid sparandet: " + ex.Message);
                }
            }

            var vm = BuildViewModel(3);
            if (errors.Count > 0)
                vm.ErrorMessage = string.Join(" ", errors);
            else
                vm.BlogMessage = input.BlogEntryID > 0
                    ? "✅ Inlägget uppdaterades."
                    : "✅ Nytt inlägg publicerades.";
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

            var vm = BuildViewModel(5);
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

            var vm = BuildViewModel(6);
            vm.ErrorMessage = error;
            return View("Index", vm);
        }

        // ── POST: Delete team ─────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTeam(int id)
        {
            var team = _teamRepo.GetTeam(id);
            if (team != null) { _teamRepo.Delete(team); _teamRepo.Save(); }
            return RedirectToAction("Index", new { tab = 4 });
        }

        // ── POST: Delete match ────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMatch(int id)
        {
            var match = _matchRepo.GetMatch(id);
            if (match != null) { _matchRepo.Delete(match); _matchRepo.Save(); }
            return RedirectToAction("Index", new { tab = 5 });
        }

        // ── POST: Batch import top scorers ───────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BatchImportTopScorers(AdminTopScorerBatchInput input)
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

            var vm = BuildViewModel(6);
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
            return RedirectToAction("Index", new { tab = 6 });
        }

        // ─────────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────────

        private AdminIndexViewModel BuildViewModel(int activeTab)
        {
            var allTeams = _teamRepo.GetAllTeams().ToList();
            var winners  = _scorerRepo.GetWinner().ToList();
            var vm = new AdminIndexViewModel
            {
                ActiveTab    = activeTab,
                Matches      = _matchRepo.GetAllMatches().ToList(),
                AllTeams     = allTeams,
                Users        = _userRepo.GetAllUsers().ToList(),
                BlogEntries  = _blogRepo.GetAllBlogEntries().ToList(),
                Teams        = allTeams,
                TopScorers   = _scorerRepo.GetAllScorers().ToList(),
                TopScorer    = winners.FirstOrDefault()?.DisplayName ?? "",
                AllTopScorerWinners = winners.Select(t => t.DisplayName).ToList(),
                QFSelected   = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.isInQuarterFinals).Select(t => t.ID).ToList(),
                SFSelected   = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.isInSemiFinals).Select(t => t.ID).ToList(),
                FinSelected  = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.isInFinals).Select(t => t.ID).ToList(),
                BronzeSelected = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.WonBronze).FirstOrDefault()?.ID ?? -1,
                SilverSelected = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.WonSilver).FirstOrDefault()?.ID ?? -1,
                GoldSelected   = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.WonGold).FirstOrDefault()?.ID   ?? -1,
            };

            var serializer = new JavaScriptSerializer();
            vm.BlogEntriesJson = serializer.Serialize(
                vm.BlogEntries.ToDictionary(
                    b => b.ID.ToString(),
                    b => new { title = b.Title, text = b.Text }
                )
            );

            // Playoff selections — load all playoff teams once, look up in memory
            var allPlayoffTeams = _teamRepo.GetPlayoffTeams().ToList();
            foreach (var g in new[] { "A","B","C","D","E","F","G","I" })
            {
                vm.PlayoffSelected[g + "1"] = allPlayoffTeams.FirstOrDefault(t => t.GroupID == g && t.PlayOffPos == 1)?.ID ?? -1;
                vm.PlayoffSelected[g + "2"] = allPlayoffTeams.FirstOrDefault(t => t.GroupID == g && t.PlayOffPos == 2)?.ID ?? -1;
            }

            vm.EnableNewEntries = new SettingsRepository().GetBool("EnableNewEntries");

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
                ("I",input.PlayoffI1,1),("I",input.PlayoffI2,2),
            };

            var ids     = pairs.Select(p => p.Item2).Where(id => id > 0).Distinct().ToList();
            var teamMap = _teamRepo.GetAllTeams().Where(t => ids.Contains(t.ID)).ToDictionary(t => t.ID);

            foreach (var (g, id, pos) in pairs)
            {
                if (id <= 0 || !teamMap.TryGetValue(id, out var team)) continue;
                team.IsInPlayOffs = true;
                team.PlayOffPos   = (byte)pos;
            }
        }

        private void ApplyKnockoutTeams(AdminSaveResultsInput input)
        {
            var allIds = (input.QFTeams ?? new List<int>())
                .Concat(input.SFTeams ?? new List<int>())
                .Concat(input.FinalTeams ?? new List<int>())
                .Concat(new[] { input.BronzeTeam, input.SilverTeam, input.GoldTeam }.Where(id => id > 0))
                .Distinct().ToList();
            var teamMap = _teamRepo.GetAllTeams().Where(t => allIds.Contains(t.ID)).ToDictionary(t => t.ID);

            foreach (var id in input.QFTeams ?? new List<int>())
            { if (teamMap.TryGetValue(id, out var t)) t.IsInQuarterFinals = true; }

            foreach (var id in input.SFTeams ?? new List<int>())
            { if (teamMap.TryGetValue(id, out var t)) t.IsInSemiFinals = true; }

            foreach (var id in input.FinalTeams ?? new List<int>())
            { if (teamMap.TryGetValue(id, out var t)) t.IsInFinal = true; }

            if (input.BronzeTeam > 0 && teamMap.TryGetValue(input.BronzeTeam, out var bt)) bt.WonBronze = true;
            if (input.SilverTeam > 0 && teamMap.TryGetValue(input.SilverTeam, out var st)) st.WonSilver = true;
            if (input.GoldTeam   > 0 && teamMap.TryGetValue(input.GoldTeam,   out var gt)) gt.WonGold   = true;
        }

        private void SetWinner(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName)) return;
            var scorer = _scorerRepo.GetTopScorer(displayName.Trim());
            if (scorer != null) scorer.IsWinner = true;
        }

        private void UpdateUsers()
        {
            // Truncate to seconds to avoid datetime precision mismatch with SQL Server
            var dtNow = DateTime.UtcNow.AddHours(2);
            dtNow = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            // Single query — all related collections eager-loaded to avoid N+1
            var users   = _userRepo.GetAllActiveUsersWithDetails();
            var matches = _matchRepo.GetAllMatches().ToList();
            var guid    = Guid.NewGuid();

            // Reset all bonus points first to avoid double-counting when applying bonuses
            _userRepo.ResetAllBonusPoints();

            // Fetch all team lists once, before the user loop
            var playoffTeams = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.isInPlayoffs).ToList();
            var qfTeams      = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.isInQuarterFinals).ToList();
            var sfTeams      = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.isInSemiFinals).ToList();
            var finalTeams   = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.isInFinals).ToList();
            var bronzeTeams  = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.WonBronze).ToList();
            var silverTeams  = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.WonSilver).ToList();
            var goldTeams    = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.WonGold).ToList();
            var winners      = _scorerRepo.GetWinner().ToList();

            // Pre-build lookup sets keyed by TeamID for O(1) checks in the inner loops
            var playoffTeamIds = new HashSet<int>(playoffTeams.Select(t => t.ID));
            var qfTeamIds      = new HashSet<int>(qfTeams.Select(t => t.ID));
            var sfTeamIds      = new HashSet<int>(sfTeams.Select(t => t.ID));
            var finalTeamIds   = new HashSet<int>(finalTeams.Select(t => t.ID));
            var bronzeTeamIds  = new HashSet<int>(bronzeTeams.Select(t => t.ID));
            var silverTeamIds  = new HashSet<int>(silverTeams.Select(t => t.ID));
            var goldTeamIds    = new HashSet<int>(goldTeams.Select(t => t.ID));
            var winnerIds      = new HashSet<int>(winners.Select(w => w.ID));

            // Index playoff teams by ID for position lookup
            var playoffTeamById = playoffTeams.ToDictionary(t => t.ID);

            foreach (var user in users)
            {
                short total = 0;

                // Build per-user lookups from the already-loaded collections
                var userMatchById       = user.UserMatches.ToDictionary(um => um.MatchID);
                var userPlayoffByTeamId = user.UserPlayoffTeams.ToDictionary(t => t.TeamID);
                var userQFByTeamId      = user.UserQFTeams.ToDictionary(t => t.TeamID);
                var userSFByTeamId      = user.UserSFTeams.ToDictionary(t => t.TeamID);
                var userFinalByTeamId   = user.UserFinalTeams.ToDictionary(t => t.TeamID);
                var userBronzeByTeamId  = user.UserBronzeTeam.ToDictionary(t => t.TeamID);
                var userSilverByTeamId  = user.UserSilverTeam.ToDictionary(t => t.TeamID);
                var userGoldByTeamId    = user.UserGoldTeam.ToDictionary(t => t.TeamID);

                foreach (var match in matches)
                {
                    if (!userMatchById.TryGetValue(match.ID, out var um)) continue;
                    um.Points = 0;
                    if (match.ResultMark != null)
                    {
                        if (um.HomeGoals == match.HomeGoals && um.AwayGoals == match.AwayGoals) um.Points++;
                        if      (match.ResultMark == "1" && um.HomeGoals > um.AwayGoals)   um.Points = (byte)(um.Points + 2);
                        else if (match.ResultMark == "X" && um.HomeGoals == um.AwayGoals)  um.Points = (byte)(um.Points + 2);
                        else if (match.ResultMark == "2" && um.HomeGoals < um.AwayGoals)   um.Points = (byte)(um.Points + 2);
                    }
                    total += (short)(um.Points ?? 0);
                }

                // Playoff bonus
                foreach (var teamId in playoffTeamIds)
                {
                    if (!userPlayoffByTeamId.TryGetValue(teamId, out var uteam)) continue;
                    uteam.Points = 2;
                    var bp = user.BonusPoints.SingleOrDefault(b => b.GroupID == uteam.Team.GroupID);
                    if (bp != null)
                    {
                        if (uteam.Points == 2 && bp.HalfPoint) bp.Point = 2;
                        else if (uteam.Points == 2 && !bp.HalfPoint) bp.HalfPoint = true;
                    }
                    if (playoffTeamById.TryGetValue(teamId, out var pt) && uteam.Position == pt.PlayOffPos)
                        uteam.Points = (byte)(uteam.Points + 2);
                    total += (short)uteam.Points;
                }
                // Add group bonus once per group
                foreach (var bp in user.BonusPoints)
                    total += bp.Point;

                foreach (var teamId in qfTeamIds)
                { if (userQFByTeamId.TryGetValue(teamId, out var u)) { u.Points = 4; total += 4; } }

                foreach (var teamId in sfTeamIds)
                { if (userSFByTeamId.TryGetValue(teamId, out var u)) { u.Points = 4; total += 4; } }

                foreach (var teamId in finalTeamIds)
                { if (userFinalByTeamId.TryGetValue(teamId, out var u)) { u.Points = 4; total += 4; } }

                foreach (var teamId in bronzeTeamIds)
                { if (userBronzeByTeamId.TryGetValue(teamId, out var u)) { u.Points = 5; total += 5; } }

                foreach (var teamId in silverTeamIds)
                { if (userSilverByTeamId.TryGetValue(teamId, out var u)) { u.Points = 5; total += 5; } }

                foreach (var teamId in goldTeamIds)
                { if (userGoldByTeamId.TryGetValue(teamId, out var u)) { u.Points = 15; total += 15; } }

                if (winnerIds.Contains(user.TopScorerID ?? -1)) total += 10;

                user.Standings.Add(new Standing { TotalPoints = total, UpdateDate = dtNow, Guid = guid });
            }

            _userRepo.Save();
            _userRepo.SortStandings(dtNow);
            _userRepo.Save();
        }
    }
}
