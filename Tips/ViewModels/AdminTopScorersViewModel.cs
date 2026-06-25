using System.Collections.Generic;
using Tipset.Models;

namespace Tipset.ViewModels
{
    public class AdminTopScorersViewModel
    {
        public List<TopScorer> TopScorers { get; set; } = new List<TopScorer>();
        public string ErrorMessage { get; set; }
    }
}
