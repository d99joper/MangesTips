using System.Collections.Generic;
using Tipset.Models;

namespace Tipset.ViewModels
{
    public class DetailsViewModel
    {
        public string ErrorMessage { get; set; }

        public string DisplayName { get; set; }
        public short? Position { get; set; }
        public short TotalPoints { get; set; }

        public string Vm2010Html { get; set; }
        public string Em2012Html { get; set; }
        public string Vm2014Html { get; set; }
        public string Em2016Html { get; set; }

        public string PdfUrl { get; set; }

        public IEnumerable<UserMatch> UserMatches { get; set; }

        // Playoff teams: key = "1A", "2A", "1B", etc., value = "TeamName [Xp]"
        public Dictionary<string, string> PlayoffTeams { get; set; }

        // Group bonus points: key = "A"–"H", value = "+1p" or ""
        public Dictionary<string, string> GroupBonus { get; set; }

        // Quarter-final and semi-final teams as rows of 2 cells each
        public List<string[]> QFRows { get; set; }
        public List<string[]> SFRows { get; set; }

        // Final teams (one row)
        public string[] FinalTeams { get; set; }

        public string TopScorer { get; set; }
        public string Bronze { get; set; }
        public string Silver { get; set; }
        public string Gold { get; set; }
    }
}
