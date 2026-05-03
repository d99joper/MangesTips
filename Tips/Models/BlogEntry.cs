
namespace Tipset.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BlogEntry
    {
        public BlogEntry()
        {
            this.Comments = new HashSet<Comment>();
        }
    
        public int ID { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public System.DateTime PostedDate { get; set; }
    
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
