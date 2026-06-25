using System.Collections.Generic;
using Tipset.Models;

namespace Tipset.ViewModels
{
    public class AdminMatchesViewModel
    {
        public List<Match> Matches  { get; set; } = new List<Match>();
        public List<Team>  AllTeams { get; set; } = new List<Team>();
        public string ErrorMessage { get; set; }
    }
}
