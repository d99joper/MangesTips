using Tipset.Models;

namespace Tipset.ViewModels
{
    public class AdminResultsViewModel
    {
        public List<Match> Matches { get; set; } = new List<Match>();
        public List<Team> AllTeams { get; set; } = new List<Team>();

        public Dictionary<string, int> PlayoffSelected { get; set; } = new Dictionary<string, int>();

        public List<int> QFSelected { get; set; } = new List<int>();
        public List<int> SFSelected { get; set; } = new List<int>();
        public List<int> FinSelected { get; set; } = new List<int>();
        public int BronzeSelected { get; set; } = -1;
        public int SilverSelected { get; set; } = -1;
        public int GoldSelected { get; set; } = -1;

        public List<string> AllTopScorerWinners { get; set; } = new List<string>();
        public List<TopScorer> TopScorers { get; set; } = new List<TopScorer>();

        public string ErrorMessage { get; set; }
        public List<string> ResultsMessages { get; set; } = new List<string>();
    }
}