using System.Collections.Generic;
using Tipset.Models;

namespace Tipset.ViewModels
{
    public class AdminBlogViewModel
    {
        public List<BlogEntry> BlogEntries     { get; set; } = new List<BlogEntry>();
        public string          BlogEntriesJson { get; set; } = "{}";
        public string ErrorMessage { get; set; }
        public string BlogMessage  { get; set; }
    }
}
