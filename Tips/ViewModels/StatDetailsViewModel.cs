using System.Collections.Generic;

namespace Tipset.ViewModels
{
    public class StatDetailRow
    {
        public string DisplayName { get; set; }
        public bool IsHighlighted { get; set; }
        public string Extra { get; set; }
    }

    public class StatDetailsViewModel
    {
        public List<StatDetailRow> Rows { get; set; }
    }
}
