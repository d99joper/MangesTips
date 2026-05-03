using System.Collections.Generic;
using Tipset.Models;

namespace Tipset.ViewModels
{
    public class AnswersViewModel
    {
        public List<Match> Matches { get; set; }

        // Playoff teams per group: key = "A1","A2","B1",... value = team name or ""
        public Dictionary<string, string> PlayoffTeams { get; set; }

        public List<string> QFTeams { get; set; }
        public List<string> SFTeams { get; set; }
        public List<string> FinalTeams { get; set; }

        public string TopScorer { get; set; }
        public string Bronze { get; set; }
        public string Silver { get; set; }
        public string Gold { get; set; }
    }
}
