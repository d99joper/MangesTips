
namespace Tipset.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Comment
    {
        public int ID { get; set; }
        public string PostedBy { get; set; }
        public System.DateTime PostedDate { get; set; }
        public string Text { get; set; }
        public Nullable<int> BlogEntryID { get; set; }
    
        public virtual BlogEntry BlogEntry { get; set; }
    }
}
