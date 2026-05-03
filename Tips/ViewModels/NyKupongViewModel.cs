using System.Collections.Generic;
using Tipset.Models;

namespace Tipset.ViewModels
{
    public class MatchScoreInput
    {
        public int MatchID { get; set; }
        public string HomeGoals { get; set; }
        public string AwayGoals { get; set; }
    }

    public class NyKupongViewModel
    {
        // Form state
        public bool IsOpen { get; set; }
        public string ErrorMessage { get; set; }
        public bool QFError { get; set; }
        public bool SFError { get; set; }
        public bool FinalError { get; set; }
        public string ScorerError { get; set; }

        // Data for rendering dropdowns
        public List<Match> Matches { get; set; } = new List<Match>();
        public Dictionary<string, List<Team>> GroupTeams { get; set; } = new Dictionary<string, List<Team>>();
        public List<Team> AllTeams { get; set; } = new List<Team>();

        // --- User input (bound on POST, repopulated on error) ---

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        // Match scores — Scores[i].MatchID / HomeGoals / AwayGoals
        public List<MatchScoreInput> Scores { get; set; } = new List<MatchScoreInput>();

        // Playoff: one int per group+position (default -1 = not selected)
        public int PlayoffA1 { get; set; } = -1;
        public int PlayoffA2 { get; set; } = -1;
        public int PlayoffB1 { get; set; } = -1;
        public int PlayoffB2 { get; set; } = -1;
        public int PlayoffC1 { get; set; } = -1;
        public int PlayoffC2 { get; set; } = -1;
        public int PlayoffD1 { get; set; } = -1;
        public int PlayoffD2 { get; set; } = -1;
        public int PlayoffE1 { get; set; } = -1;
        public int PlayoffE2 { get; set; } = -1;
        public int PlayoffF1 { get; set; } = -1;
        public int PlayoffF2 { get; set; } = -1;
        public int PlayoffG1 { get; set; } = -1;
        public int PlayoffG2 { get; set; } = -1;
        public int PlayoffH1 { get; set; } = -1;
        public int PlayoffH2 { get; set; } = -1;

        // Quarter-finals (8 teams)
        public int[] QFTeams { get; set; } = { -1, -1, -1, -1, -1, -1, -1, -1 };

        // Semi-finals (4 teams)
        public int[] SFTeams { get; set; } = { -1, -1, -1, -1 };

        // Finals (2 teams)
        public int[] FinalTeams { get; set; } = { -1, -1 };

        // Individual awards
        public string TopScorer { get; set; }
        public int BronzeTeam { get; set; } = -1;
        public int SilverTeam { get; set; } = -1;
        public int GoldTeam { get; set; } = -1;
    }
}
