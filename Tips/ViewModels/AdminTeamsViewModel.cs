using System.Collections.Generic;
using Tipset.Models;

namespace Tipset.ViewModels
{
    public class AdminTeamsViewModel
    {
        public List<Team> Teams { get; set; } = new List<Team>();
        public string ErrorMessage { get; set; }
    }
}
