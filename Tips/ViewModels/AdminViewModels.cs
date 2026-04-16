using System.Collections.Generic;
using Tipset.Models;

namespace Tipset.ViewModels
{
    public class AdminIndexViewModel
    {
        // ── Tab 1: Match results ─────────────────────────────────────────────
        public List<Match> Matches { get; set; } = new List<Match>();
        public List<Team> AllTeams { get; set; } = new List<Team>();

        // Playoff dropdowns: selected team IDs (keyed "A1","A2",..."H2")
        public Dictionary<string, int> PlayoffSelected { get; set; } = new Dictionary<string, int>();

        // QF / SF / Final / medals
        public List<int> QFSelected  { get; set; } = new List<int>();
        public List<int> SFSelected  { get; set; } = new List<int>();
        public List<int> FinSelected { get; set; } = new List<int>();
        public int BronzeSelected { get; set; } = -1;
        public int SilverSelected { get; set; } = -1;
        public int GoldSelected   { get; set; } = -1;

        // Top scorer
        public string TopScorer { get; set; }

        // ── Tab 2: Users ─────────────────────────────────────────────────────
        public List<User> Users { get; set; } = new List<User>();

        // ── Tab 3: Other settings ────────────────────────────────────────────
        public bool EnableNewEntries { get; set; }

        // ── Tab 4: Blog ──────────────────────────────────────────────────────
        public List<BlogEntry> BlogEntries { get; set; } = new List<BlogEntry>();

        // ── Tab 5: Teams & Matches ───────────────────────────────────────────
        public List<Team> Teams { get; set; } = new List<Team>();

        // ── Tab 6: Top scorers ────────────────────────────────────────────────
        public List<TopScorer> TopScorers { get; set; } = new List<TopScorer>();

        // ── Shared: error / status messages ─────────────────────────────────
        public string ErrorMessage     { get; set; }
        public string StatsMessage     { get; set; }
        public string UsersMessage     { get; set; }
        public int ActiveTab           { get; set; } = 0;
    }

    public class AdminSaveResultsInput
    {
        public List<MatchScoreInput> Matches     { get; set; } = new List<MatchScoreInput>();

        // Playoff group selections — flat, same shape as NyKupong
        public int PlayoffA1 { get; set; } public int PlayoffA2 { get; set; }
        public int PlayoffB1 { get; set; } public int PlayoffB2 { get; set; }
        public int PlayoffC1 { get; set; } public int PlayoffC2 { get; set; }
        public int PlayoffD1 { get; set; } public int PlayoffD2 { get; set; }
        public int PlayoffE1 { get; set; } public int PlayoffE2 { get; set; }
        public int PlayoffF1 { get; set; } public int PlayoffF2 { get; set; }
        public int PlayoffG1 { get; set; } public int PlayoffG2 { get; set; }
        public int PlayoffH1 { get; set; } public int PlayoffH2 { get; set; }

        public int[] QFTeams     { get; set; } = { -1,-1,-1,-1,-1,-1,-1,-1 };
        public int[] SFTeams     { get; set; } = { -1,-1,-1,-1 };
        public int[] FinalTeams  { get; set; } = { -1,-1 };
        public int   BronzeTeam  { get; set; } = -1;
        public int   SilverTeam  { get; set; } = -1;
        public int   GoldTeam    { get; set; } = -1;
        public string TopScorer  { get; set; }
        public string AdditionalTopScorers { get; set; }  // pipe-delimited
    }

    public class AdminUserRow
    {
        public int    UserID       { get; set; }
        public bool   HasPaid      { get; set; }
        public bool   IsConfirmed  { get; set; }
        public bool   IsWinner     { get; set; }
    }

    public class AdminSaveUsersInput
    {
        public List<AdminUserRow> Users { get; set; } = new List<AdminUserRow>();
    }

    public class AdminBlogInput
    {
        public int    BlogEntryID  { get; set; }  // 0 = new
        public string Title        { get; set; }
        public string Text         { get; set; }
    }

    public class AdminMatchInput
    {
        public int    MatchID      { get; set; }   // 0 = new
        public int    HomeTeamID   { get; set; }
        public int    AwayTeamID   { get; set; }
        public string Date         { get; set; }
    }

    public class AdminTeamInput
    {
        public int    TeamID       { get; set; }   // 0 = new
        public string TeamName     { get; set; }
        public string GroupID      { get; set; }
    }

    public class AdminTopScorerInput
    {
        public int    TopScorerID  { get; set; }   // 0 = new
        public string FirstName    { get; set; }
        public string LastName     { get; set; }
    }
}
