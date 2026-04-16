using System.Collections.Generic;
using Tipset.Models;

namespace Tipset.ViewModels
{
    public class StatisticsViewModel
    {
        public List<Match> Matches { get; set; }
        public List<Team> Teams { get; set; }
        public List<TopScorer> TopScorers { get; set; }
        public string SortColumn { get; set; }
        public string SortDir { get; set; }
        public int ActiveTab { get; set; }
    }
}
