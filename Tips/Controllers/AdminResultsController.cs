using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Tipset.Models;
using Tipset.ViewModels;

namespace Tipset.Controllers
{
    [Authorize]
    [Route("Admin/Results")]
    public class AdminResultsController : BaseController
    {
        private readonly TeamRepository _teamRepo;
        private readonly MatchRepository _matchRepo;
        private readonly UserRepository _userRepo;
        private readonly TopScorerRepository _scorerRepo;
        private readonly Tips_Entities _db;

        public AdminResultsController(TeamRepository teamRepo, MatchRepository matchRepo,
            UserRepository userRepo, TopScorerRepository scorerRepo, SettingsRepository settingsRepo, Tips_Entities db)
            : base(settingsRepo)
        {
            _teamRepo = teamRepo;
            _matchRepo = matchRepo;
            _userRepo = userRepo;
            _scorerRepo = scorerRepo;
            _db = db;
        }

        // ── GET /Admin/Results ──────────────────────────────────────────────
        [HttpGet("")]
        public ActionResult Index()
        {
            var vm = BuildViewModel();

            if (TempData["ResultsMessages"] is string json)
                vm.ResultsMessages = JsonSerializer.Deserialize<List<string>>(json);
            if (TempData["ErrorMessage"] is string err)
                vm.ErrorMessage = err;

            return View("~/Views/Admin/Results.cshtml", vm);
        }

        // ── POST: Save match results + recalculate points ──────────────────
        [HttpPost("Save")]
        [ValidateAntiForgeryToken]
        public ActionResult Save(AdminSaveResultsInput input)
        {
            var messages = new List<string>();
            string error = null;

            using var transaction = _db.Database.BeginTransaction();
            try
            {
                int scored = 0, cleared = 0;
                foreach (var m in input.Matches)
                {
                    var match = _matchRepo.GetMatch(m.MatchID);
                    if (match == null) continue;
                    if (byte.TryParse(m.HomeGoals, out byte hg) && byte.TryParse(m.AwayGoals, out byte ag))
                    {
                        match.HomeGoals = hg;
                        match.AwayGoals = ag;
                        match.ResultMark = hg > ag ? "1" : hg == ag ? "X" : "2";
                        scored++;
                    }
                    else
                    {
                        match.HomeGoals = null;
                        match.AwayGoals = null;
                        match.ResultMark = null;
                        cleared++;
                    }
                }
                _matchRepo.Save();
                messages.Add($"✔ Matchresultat sparade ({scored} satta, {cleared} rensade).");

                var allTeamMap = _teamRepo.ResetAndGetTeams();
                ApplyPlayoffTeams(input, allTeamMap);
                messages.Add("✔ Vidare från gruppen sparade.");
                ApplyKnockoutTeams(input, allTeamMap);
                _teamRepo.Save();
                messages.Add("✔ KO-faser (QF/SF/Final/medaljer) sparade.");

                _scorerRepo.ResetWinner();
                int scorerCount = 0;
                if (input.TopScorers != null)
                    foreach (var s in input.TopScorers.Where(s => !string.IsNullOrWhiteSpace(s)))
                    {
                        SetWinner(s);
                        scorerCount++;
                    }
                _scorerRepo.Save();
                messages.Add($"✔ Skyttekung sparad ({scorerCount} st).");
                transaction.Commit();
                messages.Add("✔ Matchresultat, lag och skyttekung sparade.");
                
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                error = ex.Message;
                messages.Add("❌ Fel: " + ex.Message);
                TempData["ResultsMessages"] = JsonSerializer.Serialize(messages);
                TempData["ErrorMessage"] = error;
                return RedirectToAction("Index");
            }

            try
            {
                UpdateUsers();
                messages.Add("✔ Användarpoäng omräknade.");
                messages.Add("✅ Allt sparat!");
            }
            catch (Exception ex)
            {
                error = ex.Message;
                messages.Add("❌ Fel vid poängberäkning: " + ex.Message);
            }

            TempData["ResultsMessages"] =  JsonSerializer.Serialize(messages);
            TempData["ErrorMessage"] = error;

            // Redirect-after-POST: GET reloads fresh, no resubmission risk,
            // no giant rebuilt payload riding on the POST response.
            return RedirectToAction("Index");
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private AdminResultsViewModel BuildViewModel()
        {
            var allTeams = _teamRepo.GetAllTeams().ToList();
            var winners = _scorerRepo.GetWinner().ToList();

            var vm = new AdminResultsViewModel
            {
                Matches = _matchRepo.GetAllMatches().ToList(),
                AllTeams = allTeams,
                TopScorers = _scorerRepo.GetAllScorers().ToList(),
                AllTopScorerWinners = winners.Select(t => t.DisplayName).ToList(),
                QFSelected = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.isInQuarterFinals).Select(t => t.ID).ToList(),
                SFSelected = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.isInSemiFinals).Select(t => t.ID).ToList(),
                FinSelected = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.isInFinals).Select(t => t.ID).ToList(),
                BronzeSelected = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.WonBronze).FirstOrDefault()?.ID ?? -1,
                SilverSelected = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.WonSilver).FirstOrDefault()?.ID ?? -1,
                GoldSelected = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.WonGold).FirstOrDefault()?.ID ?? -1,
            };

            var allPlayoffTeams = _teamRepo.GetPlayoffTeams().ToList();
            foreach (var g in new[] { "A", "B", "C", "D", "E", "F", "G", "I" })
            {
                vm.PlayoffSelected[g + "1"] = allPlayoffTeams.FirstOrDefault(t => t.GroupID == g && t.PlayOffPos == 1)?.ID ?? -1;
                vm.PlayoffSelected[g + "2"] = allPlayoffTeams.FirstOrDefault(t => t.GroupID == g && t.PlayOffPos == 2)?.ID ?? -1;
            }

            return vm;
        }

        private static void ApplyPlayoffTeams(AdminSaveResultsInput input, Dictionary<int, Team> teamMap)
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

            var ids = pairs.Select(p => p.Item2).Where(id => id > 0).Distinct().ToList();
            //var teamMap = ids.Count == 0
            //    ? new Dictionary<int, Team>()
            //    : _teamRepo.GetTeamsForUpdate(ids).ToDictionary(t => t.ID);

            foreach (var (g, id, pos) in pairs)
            {
                if (id <= 0 || !teamMap.TryGetValue(id, out var team)) continue;
                team.IsInPlayOffs = true;
                team.PlayOffPos = (byte)pos;
            }
        }

        private static void ApplyKnockoutTeams(AdminSaveResultsInput input, Dictionary<int, Team> teamMap)
        {
            var allIds = (input.QFTeams ?? new List<int>())
                .Concat(input.SFTeams ?? new List<int>())
                .Concat(input.FinalTeams ?? new List<int>())
                .Concat(new[] { input.BronzeTeam, input.SilverTeam, input.GoldTeam })
                .Where(id => id > 0)
                .Distinct().ToList();

            //var teamMap = allIds.Count == 0
            //    ? new Dictionary<int, Team>()
            //    : _teamRepo.GetTeamsForUpdate(allIds).ToDictionary(t => t.ID);

            foreach (var id in input.QFTeams ?? new List<int>())
            { if (teamMap.TryGetValue(id, out var t)) t.IsInQuarterFinals = true; }

            foreach (var id in input.SFTeams ?? new List<int>())
            { if (teamMap.TryGetValue(id, out var t)) t.IsInSemiFinals = true; }

            foreach (var id in input.FinalTeams ?? new List<int>())
            { if (teamMap.TryGetValue(id, out var t)) t.IsInFinal = true; }

            if (input.BronzeTeam > 0 && teamMap.TryGetValue(input.BronzeTeam, out var bt)) bt.WonBronze = true;
            if (input.SilverTeam > 0 && teamMap.TryGetValue(input.SilverTeam, out var st)) st.WonSilver = true;
            if (input.GoldTeam > 0 && teamMap.TryGetValue(input.GoldTeam, out var gt)) gt.WonGold = true;
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
            dtNow = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            var users = _userRepo.GetAllActiveUsersWithDetails();
            var matches = _matchRepo.GetAllMatches().AsNoTracking().ToList();
            var guid = Guid.NewGuid();

            // Reset all tracked points in-memory BEFORE raw SQL, so EF doesn't
            // overwrite the reset with stale loaded values on Save()
            foreach (var user in users)
            {
                foreach (var bp in user.BonusPoints)
                {
                    bp.Point = 0;
                    bp.HalfPoint = false;
                }
                foreach (var t in user.UserMatches) t.Points = 0;
                foreach (var t in user.UserPlayoffTeams) t.Points = 0;
                foreach (var t in user.UserQFTeams) t.Points = 0;
                foreach (var t in user.UserSFTeams) t.Points = 0;
                foreach (var t in user.UserFinalTeams) t.Points = 0;
                foreach (var t in user.UserBronzeTeam) t.Points = 0;
                foreach (var t in user.UserSilverTeam) t.Points = 0;
                foreach (var t in user.UserGoldTeam) t.Points = 0;
            }

            //_userRepo.ResetAllBonusPoints();

            var playoffTeams = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.isInPlayoffs).ToList();
            var qfTeams = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.isInQuarterFinals).ToList();
            var sfTeams = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.isInSemiFinals).ToList();
            var finalTeams = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.isInFinals).ToList();
            var bronzeTeams = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.WonBronze).ToList();
            var silverTeams = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.WonSilver).ToList();
            var goldTeams = _teamRepo.GetTeams(TeamRepository.TeamInqueryType.WonGold).ToList();
            var winners = _scorerRepo.GetWinner().ToList();

            var playoffTeamIds = new HashSet<int>(playoffTeams.Select(t => t.ID));
            var qfTeamIds = new HashSet<int>(qfTeams.Select(t => t.ID));
            var sfTeamIds = new HashSet<int>(sfTeams.Select(t => t.ID));
            var finalTeamIds = new HashSet<int>(finalTeams.Select(t => t.ID));
            var bronzeTeamIds = new HashSet<int>(bronzeTeams.Select(t => t.ID));
            var silverTeamIds = new HashSet<int>(silverTeams.Select(t => t.ID));
            var goldTeamIds = new HashSet<int>(goldTeams.Select(t => t.ID));
            var winnerIds = new HashSet<int>(winners.Select(w => w.ID));

            var playoffTeamById = playoffTeams.ToDictionary(t => t.ID);

            foreach (var user in users)
            {
                short total = 0;

                var userMatchById = user.UserMatches.ToDictionary(um => um.MatchID);
                var userPlayoffByTeamId = user.UserPlayoffTeams.ToDictionary(t => t.TeamID);
                var userQFByTeamId = user.UserQFTeams.ToDictionary(t => t.TeamID);
                var userSFByTeamId = user.UserSFTeams.ToDictionary(t => t.TeamID);
                var userFinalByTeamId = user.UserFinalTeams.ToDictionary(t => t.TeamID);
                var userBronzeByTeamId = user.UserBronzeTeam.ToDictionary(t => t.TeamID);
                var userSilverByTeamId = user.UserSilverTeam.ToDictionary(t => t.TeamID);
                var userGoldByTeamId = user.UserGoldTeam.ToDictionary(t => t.TeamID);

                foreach (var match in matches)
                {
                    if (!userMatchById.TryGetValue(match.ID, out var um)) continue;
                    um.Points = 0;
                    if (match.ResultMark != null)
                    {
                        if (um.HomeGoals == match.HomeGoals && um.AwayGoals == match.AwayGoals) um.Points++;
                        if (match.ResultMark == "1" && um.HomeGoals > um.AwayGoals) um.Points = (byte)(um.Points + 2);
                        else if (match.ResultMark == "X" && um.HomeGoals == um.AwayGoals) um.Points = (byte)(um.Points + 2);
                        else if (match.ResultMark == "2" && um.HomeGoals < um.AwayGoals) um.Points = (byte)(um.Points + 2);
                    }
                    total += (short)(um.Points ?? 0);
                }

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

                //user.Standings.Add(new Standing { TotalPoints = total, UpdateDate = dtNow, Guid = guid });
                _db.Standings.Add(new Standing
                {
                    UserID = user.ID,
                    TotalPoints = total,
                    UpdateDate = dtNow,
                    Guid = guid
                });
            }

            _userRepo.Save();
            _userRepo.SortStandings(dtNow);
            _userRepo.Save();
        }
    }
}