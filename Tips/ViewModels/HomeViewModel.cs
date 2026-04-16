using System;
using System.Collections.Generic;
using Tipset.Models;

namespace Tipset.ViewModels
{
    public class HomeViewModel
    {
        public bool EnableNewEntries { get; set; }
        public List<StandingDate> StandingDates { get; set; }
        public Guid? SelectedDate { get; set; }
        public List<Standing> Standings { get; set; }
        public string SortColumn { get; set; }
        public string SortDir { get; set; }
    }
}
